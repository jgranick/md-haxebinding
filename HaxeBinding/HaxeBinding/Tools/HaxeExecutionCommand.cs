using System;
using MonoDevelop.Core.Execution;
using System.Collections.Generic;

namespace MonoDevelop.HaxeBinding
{
	public class HaxeExecutionCommand : ProcessExecutionCommand
	{
		public Array Pathes;

		public HaxeExecutionCommand () : base()
		{
		}
		public HaxeExecutionCommand (string command) : base (command)
		{
		}
		public HaxeExecutionCommand (string command, string arguments) : base (command, arguments)
		{
		}
		public HaxeExecutionCommand (string command, string arguments, string workingDirectory) : base (command, arguments, workingDirectory)
		{
		}
		public HaxeExecutionCommand (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables) : base (command, arguments, workingDirectory, environmentVariables)
		{
		}
	}
}

