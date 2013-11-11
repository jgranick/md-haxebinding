using System;
using System.IO;
using MonoDevelop.Core.Execution;
using Mono.Debugging.Backend;
using Mono.Debugging.Client;
using MonoDevelop.Debugger;
using System.Collections.Generic;

namespace MonoDevelop.HaxeBinding
{
	public class HxcppDebuggerFactory: IDebuggerEngine
	{
		// Just a dumb hack, cause i don't know how to detect can we debug or not yet
		// TODO: make "if OpenflExecutionCommand and HxcppExecution command"
		public bool CanDebugCommand (ExecutionCommand command)
		{
			return true;
		}

		public DebuggerStartInfo CreateDebuggerStartInfo (ExecutionCommand command)
		{
			NativeExecutionCommand pec = (NativeExecutionCommand) command;
			HxcppDebuggerStartInfo startInfo = new HxcppDebuggerStartInfo ();
			if (command is OpenFLExecutionCommand) {
				startInfo.Pathes = ((OpenFLExecutionCommand)command).Pathes;
			} else {
				startInfo.Pathes = new string[0];
			}
			startInfo.Command = pec.Command;
			startInfo.Arguments = pec.Arguments;
			startInfo.WorkingDirectory = pec.WorkingDirectory;
			if (pec.EnvironmentVariables.Count > 0) {
				foreach (KeyValuePair<string,string> val in pec.EnvironmentVariables)
					startInfo.EnvironmentVariables [val.Key] = val.Value;
			}
			return startInfo;
		}

		public DebuggerSession CreateSession ()
		{
			HxcppDbgSession ds = new HxcppDbgSession ();
			return ds;
		}

		// Returns just ans empty list, cause we can't attach this debugger
		public ProcessInfo[] GetAttachableProcesses ()
		{
			return new ProcessInfo[0];
		}
	}
}

