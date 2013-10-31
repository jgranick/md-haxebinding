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
		IProcessAsyncOperation console;
		Thread thread;

		object debuggerLock = new object ();

		protected override void OnRun (DebuggerStartInfo startInfo)
		{
			lock (debuggerLock) {
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
			proc = new Process ();
			proc.StartInfo.FileName = "gdb";
			proc.StartInfo.Arguments = "-quiet -fullname -i=mi2";
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
	}
}

