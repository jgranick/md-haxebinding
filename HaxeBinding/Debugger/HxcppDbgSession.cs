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

namespace MonoDevelop.HaxeBinding
{
	public class HxcppDbgSession: DebuggerSession
	{
		Process proc;
		StreamReader sout;
		StreamWriter sin;
		Thread thread;
		static Boolean dbgCreated = false; //just a hack to prevent debugger creation on every session

		object debuggerLock = new object ();

		protected override void OnRun (DebuggerStartInfo startInfo)
		{
			lock (debuggerLock) {
				CreateDebugger ();
				StartDebugger ();
			}
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
		}

		protected override void OnDetach ()
		{
		}

		protected override void OnExit ()
		{
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

		// TODO: replace gdb to custom debugger
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
			//info.WindowStyle = ProcessWindowStyle.Hidden;
			dbgCreateInfo.CreateNoWindow = true;

			using (Process process = Process.Start (dbgCreateInfo))
			{
				process.WaitForExit ();
				dbgCreated = true;
			}
		}
	}
}

