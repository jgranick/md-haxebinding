using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Mono.Debugging.Client;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using Mono.Unix.Native;
using MonoDevelop.Core.Serialization;
using MonoDevelop.HaxeBinding.Projects;
using MonoDevelop.HaxeBinding.Tools;
using System.Text.RegularExpressions;

namespace MonoDevelop.HaxeBinding
{
	public struct Break
	{
		public Break(string filename, int line) {
			this.filename = filename;
			this.line = line;
		}

		public string filename;
		public int line;
	}

	public class HxcppDbgSession: DebuggerSession
	{
		enum outputType { interrupt, backtrace, backtraceinfo };
		Process debugger;
		Process proc;
		StreamReader appout;
		StreamReader sout;
		StreamWriter sin;
		Thread thread;
		Thread appthread;
		static Boolean dbgCreated = false; //just a hack to prevent debugger creation on every session
		Array classPathes = null;
		bool running = false;

		object debuggerLock = new object ();
		object syncLock = new object ();

		HxcppCommandResult lastResult = new HxcppCommandResult ();

		private List<Break> breaks = new List<Break>();
		private Regex breakAdded = new Regex (@"Breakpoint (\d+) set and enabled\.");
		private Regex threadStopped = new Regex(@"Thread (\d+) stopped in (\d+)\.");

		protected override void OnRun (DebuggerStartInfo startInfo)
		{
			Console.WriteLine ("in OnRun of debug session");
			lock (debuggerLock) {
				// TODO: add for haxe projects too
				if (startInfo is HxcppDebuggerStartInfo) {
					LogWriter (false, "It's hxcpp\n");
					classPathes = ((HxcppDebuggerStartInfo)startInfo).Pathes;
				} else {
					LogWriter (false, "It's not hxcpp\n");
					classPathes = new string[0];
				}
				CreateDebugger ();
				StartDebugger ();
				StartProcess (startInfo);
				running = true;
				OnStarted ();
			}
		}

		private void StartProcess(DebuggerStartInfo startInfo)
		{
			var psi = new ProcessStartInfo (startInfo.Command) {
				Arguments = string.Format (startInfo.Arguments),
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				WorkingDirectory = startInfo.WorkingDirectory,
			};

			proc = Process.Start(psi);

			appout = proc.StandardOutput;
			proc.EnableRaisingEvents = true;
			proc.Exited += delegate {
				Exit();
			};

			LogWriter(false, "Process started\n");

			appthread = new Thread (AppOutput);
			appthread.Name = "Application output interpeter";
			appthread.IsBackground = true;
			appthread.Start ();
		}

		protected override void OnAttachToProcess (long processId)
		{
		}

		public override void Dispose ()
		{
			if(thread != null)
				thread.Abort ();
			if (appthread != null)
				appthread.Abort ();
			base.Dispose ();
		}

		protected override void OnSetActiveThread (long processId, long threadId)
		{
		}

		protected override void OnStop ()
		{
			if(LogWriter != null)
				LogWriter(false, "Stopped debugging\n");
			if (running) {
				RunCommand (true, "break", new string[0]);
				running = false;
			}
			//Frontend.NotifyTargetEvent (new TargetEventArgs (TargetEventType.TargetInterrupted));
			//OnTargetEvent (new TargetEventArgs (TargetEventType.TargetInterrupted));
			//StopDebugger ();
		}

		protected override void OnDetach ()
		{
		}

		protected override void OnExit ()
		{
			Frontend.NotifyTargetEvent (new TargetEventArgs (TargetEventType.TargetExited));
			StopDebugger ();
		}

		protected override void OnStepLine ()
		{
		}

		protected override void OnNextLine ()
		{
		}

		protected override void OnStepInstruction ()
		{
		}

		protected override void OnNextInstruction ()
		{
		}

		protected override void OnFinish ()
		{
			StopDebugger ();
		}

		protected override BreakEventInfo OnInsertBreakEvent (BreakEvent be)
		{
			LogWriter(false, "Break inserted\n");
			Breakpoint bp = be as Breakpoint;
			if (bp == null)
				throw new NotSupportedException ();

			BreakEventInfo bi = new BreakEventInfo ();

			lock (debuggerLock) {
				LogWriter(false, "Location is " + PathHelper.CutOffClassPath(classPathes, bp.FileName) + ":" + bp.Line + '\n');
				breaks.Add (new Break (PathHelper.CutOffClassPath (classPathes, bp.FileName), bp.Line));
			}

			//bi.Handle = TODO: add returned success value (break count etc)
			bi.SetStatus (BreakEventStatus.Bound, null);
			return bi;
		}

		protected override void OnRemoveBreakEvent (BreakEventInfo binfo)
		{
			if(LogWriter != null)
				LogWriter(false, "Break removed " + binfo.ToString());
		}

		protected override void OnEnableBreakEvent (BreakEventInfo binfo, bool enable)
		{
			LogWriter(false, "Break enabled " + binfo.ToString());
		}

		protected override void OnUpdateBreakEvent (BreakEventInfo binfo)
		{
			LogWriter(false, "Break updated " + binfo.ToString());
		}

		protected override void OnContinue ()
		{
			LogWriter (false, "Continue execution\n");
			RunCommand(false, "continue");
			running = true;
			//OnTargetEvent (new TargetEventArgs (TargetEventType.TargetReady));
		}

		protected override Backtrace OnGetThreadBacktrace (long processId, long threadId)
		{
			return null;
		}

		protected override ProcessInfo[] OnGetProcesses ()
		{
			return null;
		}

		protected override ThreadInfo[] OnGetThreads (long processId)
		{
			return null;
		}

		void StopDebugger()
		{
			running = false;
			thread.Abort ();
			appthread.Abort ();
			if (debugger != null && !debugger.HasExited) {
				debugger.Kill ();
			}
		}

		void StartDebugger ()
		{
			LogWriter (false, "Debugger started\n");
			string tmp_dir = Path.GetTempPath ();
			debugger = new Process ();
			debugger.StartInfo.FileName = "neko";
			debugger.StartInfo.Arguments = tmp_dir + "server.n";
			debugger.StartInfo.UseShellExecute = false;
			debugger.StartInfo.RedirectStandardInput = true;
			debugger.StartInfo.RedirectStandardOutput = true;
			debugger.StartInfo.RedirectStandardError = true;
			debugger.StartInfo.CreateNoWindow = true;
			debugger.Start ();

			sout = debugger.StandardOutput;
			sin = debugger.StandardInput;

			thread = new Thread (OutputInterpreter);
			thread.Name = "Debugger output interpeter";
			thread.IsBackground = true;
			thread.Start ();
		}

		// Thread for parsing debugger output
		void OutputInterpreter()
		{
			TargetEventType type;
			string line;
			while ((line = sout.ReadLine ()) != null) 
			{
				LogWriter (false, line + '\n');
				if (threadStopped.Match (line).Success) {
					type = TargetEventType.TargetInterrupted;
					ProcessResult (line, outputType.interrupt, threadStopped.Match (line)); // yea, shit-code
				} else {
					//type = TargetEventType.TargetStopped;
					Console.WriteLine ("just blanla in output");
					continue;
				}
				FireTargetEvent (type);
			}
		}

		void ProcessResult(string line, outputType eventType, Match matchResult) 
		{
			lock (syncLock) {
				// after getting the result and putting it to the lastResult var we are pulsing lock
				Monitor.PulseAll (syncLock);
			}
		}

		ThreadInfo GetThread (long id)
		{
			return new ThreadInfo (0, id, "Thread #" + id, null);
		}

		void FireTargetEvent(TargetEventType type)
		{
			TargetEventArgs args = new TargetEventArgs (type);
			if (type != TargetEventType.TargetExited) {
				//GdbCommandResult res = RunCommand ("-stack-info-depth");
				//int fcount = int.Parse (res.GetValue ("depth"));
				//
				//GdbBacktrace bt = new GdbBacktrace (this, activeThread, fcount, curFrame);
				//args.Backtrace = new Backtrace (bt);
				//args.Thread = GetThread (activeThread);
				HxcppBacktrace bt = new HxcppBacktrace (this, lastResult.depth, 0);
				args.Backtrace = new Backtrace (bt);
				args.Thread = GetThread (0);
			}
			OnTargetEvent (args);
		}

		//yeah one mor dirty hack, cause i don't know how to attach proc to current session
		void AppOutput()
		{
			string line;
			while ((line = appout.ReadLine ()) != null) 
			{
				LogWriter (false, line + '\n');
			}
		}

		void CreateDebugger ()
		{
			if (dbgCreated)
				return;
			ProcessStartInfo dbgCreateInfo = new ProcessStartInfo ();
			string tmp_dir = Path.GetTempPath ();

			dbgCreateInfo.FileName = "haxe";
			dbgCreateInfo.Arguments = "-main debugger.HaxeServer -lib debugger -neko " + tmp_dir + "server.n";
			dbgCreateInfo.UseShellExecute = false;
			dbgCreateInfo.RedirectStandardOutput = true;
			dbgCreateInfo.RedirectStandardError = true;
			dbgCreateInfo.CreateNoWindow = true;

			using (Process process = Process.Start (dbgCreateInfo))
			{
				process.WaitForExit ();
				dbgCreated = true;
			}
		}

		private void RunCommand (bool waitForAnswer, string command, params string[] args)
		{
			lock (debuggerLock) {
				lock (syncLock) {
					sin.WriteLine (command + " " + string.Join (" ", args));

					if(waitForAnswer)
						if (!Monitor.Wait (syncLock, 4000))
							throw new InvalidOperationException ("Command execution timeout.");
				}

			}
		}
	}
}

