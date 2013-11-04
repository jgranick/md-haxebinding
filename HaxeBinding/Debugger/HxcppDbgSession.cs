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
		Process proc;
		StreamReader sout;
		StreamWriter sin;
		IProcessAsyncOperation console;
		Thread thread;
		static Boolean dbgCreated = false; //just a hack to prevent debugger creation on every session

		object debuggerLock = new object ();
		object syncLock = new object ();

		protected override void OnRun (DebuggerStartInfo startInfo)
		{
			Console.WriteLine ("in OnRun of debug session");
			lock (debuggerLock) {
				// Create a script to be run in a terminal
				string script = Path.GetTempFileName ();
				string ttyfile = Path.GetTempFileName ();
				string ttyfileDone = ttyfile + "_done";
				string tty;

				try {
					File.WriteAllText (script, "tty > " + ttyfile + "\ntouch " + ttyfileDone + "\nsleep 10000d");
					Mono.Unix.Native.Syscall.chmod (script, FilePermissions.ALLPERMS);

					console = Runtime.ProcessService.StartConsoleProcess (script, "", ".", ExternalConsoleFactory.Instance.CreateConsole (true), null);
					DateTime tim = DateTime.Now;
					while (!File.Exists (ttyfileDone)) {
						System.Threading.Thread.Sleep (100);
						if ((DateTime.Now - tim).TotalSeconds > 10)
							throw new InvalidOperationException ("Console could not be created.");
					}
					tty = File.ReadAllText (ttyfile).Trim (' ','\n');
				} finally {
					try {
						if (File.Exists (script))
							File.Delete (script);
						if (File.Exists (ttyfile))
							File.Delete (ttyfile);
						if (File.Exists (ttyfileDone))
							File.Delete (ttyfileDone);
					} catch {
						// Ignore
					}
				}


				CreateDebugger ();
				StartDebugger ();
				OnStarted ();
				RunCommand ("codsdf", "");
			}



			/*Process dproc = new Process ();
			dproc.StartInfo.WorkingDirectory = CurrentArgs.ProjectBase;
			dproc.StartInfo.FileName = "haxelib";
			dproc.StartInfo.Arguments = "run openfl run \"" + CurrentArgs.TargetProjectXMLFile + "\" " + CurrentArgs.Platform + " -debug";
			dproc.StartInfo.UseShellExecute = false;
			dproc.Start ();
			Trace.WriteLine ("After process creation");
			Console.WriteLine (dproc.StartInfo.Arguments);
			Console.WriteLine (dproc.Id);*/
		}

		protected override void OnAttachToProcess (long processId)
		{
		}

		public override void Dispose ()
		{
		}

		protected override void OnSetActiveThread (long processId, long threadId)
		{
		}

		protected override void OnStop ()
		{
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
			proc.Kill ();
			thread.Abort ();
		}

		void StartDebugger ()
		{
			Console.WriteLine ("Debugger started");
			string tmp_dir = Path.GetTempPath ();
			proc = new Process ();
			proc.StartInfo.FileName = "neko";
			proc.StartInfo.Arguments = tmp_dir + "server.n";
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardInput = true;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.CreateNoWindow = true;
			proc.Start ();

			sout = proc.StandardOutput;
			sin = proc.StandardInput;

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
				Console.WriteLine (line);
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

