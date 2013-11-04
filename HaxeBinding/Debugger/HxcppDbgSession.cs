using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.Collections;
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

namespace MonoDevelop.HaxeBinding
{
	public class HxcppDbgSession: DebuggerSession
	{
		Process debugger;
		Process proc;
		StreamReader appout;
		StreamReader sout;
		StreamWriter sin;
		IProcessAsyncOperation console;
		Thread thread;
		Thread appthread;
		static Boolean dbgCreated = false; //just a hack to prevent debugger creation on every session

		object debuggerLock = new object ();
		object syncLock = new object ();

		protected override void OnRun (DebuggerStartInfo startInfo)
		{
			Console.WriteLine ("in OnRun of debug session");
			lock (debuggerLock) {
				CreateDebugger ();
				StartDebugger ();
				StartProcess (startInfo);
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
				Stop();
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
			LogWriter(false, "Stopped debugging\n");
			StopDebugger ();
		}

		protected override void OnDetach ()
		{
		}

		protected override void OnExit ()
		{
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
			return null;
		}

		protected override void OnRemoveBreakEvent (BreakEventInfo binfo)
		{
		}

		protected override void OnEnableBreakEvent (BreakEventInfo binfo, bool enable)
		{
		}

		protected override void OnUpdateBreakEvent (BreakEventInfo binfo)
		{
		}

		protected override void OnContinue ()
		{
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
			thread.Abort ();
			appthread.Abort ();
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
			string line;
			while ((line = sout.ReadLine ()) != null) 
			{
				LogWriter (false, line);
			}
		}

		//yeah one mor dirty hack, cause i don't know how to attach proc to current session
		void AppOutput()
		{
			string line;
			while ((line = appout.ReadLine ()) != null) 
			{
				LogWriter (false, line);
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

		public void RunCommand (string command, params string[] args)
		{
			lock (debuggerLock) {
				lock (syncLock) {
					//lastResult = null;

					//lock (eventLock) {
					//	running = true;
					//}

					//if (logGdb)
					//	Console.WriteLine ("gdb<: " + command + " " + string.Join (" ", args));

					sin.WriteLine (command + " " + string.Join (" ", args));

					if (!Monitor.Wait (syncLock, 4000))
						throw new InvalidOperationException ("Command execution timeout.");
					//if (lastResult.Status == CommandStatus.Error)
					//	throw new InvalidOperationException (lastResult.ErrorMessage);
					//return lastResult;
				}

			}
		}

	}
}

