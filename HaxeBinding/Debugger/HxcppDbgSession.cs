using System;
using System.IO;
using MonoDevelop.Core.Execution;
using Mono.Debugging.Backend;
using Mono.Debugging.Client;
using MonoDevelop.Debugger;
using System.Collections.Generic;

namespace MonoDevelop.HaxeBinding
{
	public class HxcppDbgSession: DebuggerSession
	{
		protected override void OnRun (DebuggerStartInfo startInfo)
		{
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
	}
}

