using System;
using MonoDevelop.Core.Execution;
using System.Collections.Generic;
using System.Collections;

namespace MonoDevelop.HaxeBinding
{
	public class OpenFLExecutionCommand : NativeExecutionCommand
	{
		public Array Pathes;

		public OpenFLExecutionCommand () : base()
		{
		}
		public OpenFLExecutionCommand (string command) : base (command)
		{
		}
		public OpenFLExecutionCommand (string command, string arguments) : base (command, arguments)
		{
		}
		public OpenFLExecutionCommand (string command, string arguments, string workingDirectory) : base (command, arguments, workingDirectory)
		{
		}
		public OpenFLExecutionCommand (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables) : base (command, arguments, workingDirectory, environmentVariables)
		{
		}
	}
}

