using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Projects;
using MonoDevelop.HaxeBinding.Projects;


namespace MonoDevelop.HaxeBinding.Tools
{

	static class OpenFLCommandLineToolsManager
	{
		
		private static Regex mErrorFull = new Regex (@"^(?<file>.+)\((?<line>\d+)\):\s(col:\s)?(?<column>\d*)\s?(?<level>\w+):\s(?<message>.*)\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		// example: test.hx:11: character 7 : Unterminated string
		private static Regex mErrorFileChar = new Regex (@"^(?<file>.+):(?<line>\d+):\s(character\s)(?<column>\d*)\s:\s(?<message>.*)\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		// example: test.hx:11: characters 0-5 : Unexpected class
		private static Regex mErrorFileChars = new Regex (@"^(?<file>.+):(?<line>\d+):\s(characters\s)(?<column>\d+)-(\d+)\s:\s(?<message>.*)\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		// example: test.hx:10: lines 10-28 : Class not found : Sprite
		private static Regex mErrorFile = new Regex (@"^(?<file>.+):(?<line>\d+):\s(?<message>.*)\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		
		private static Regex mErrorCmdLine = new Regex (@"^command line: (?<level>\w+):\s(?<message>.*)\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static Regex mErrorSimple = new Regex (@"^(?<level>\w+):\s(?<message>.*)\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static Regex mErrorIgnore = new Regex (@"^(Updated|Recompile|Reason|Files changed):.*", RegexOptions.Compiled);


		public static void Clean (OpenFLProject project, OpenFLProjectConfiguration configuration, IProgressMonitor monitor)
		{
			ProcessStartInfo info = new ProcessStartInfo ();
			
			info.FileName = "haxelib";
			info.Arguments = "run openfl clean \"" + project.TargetProjectXMLFile + "\" " + configuration.Platform.ToLower () + " " + project.AdditionalArguments + " " + configuration.AdditionalArguments;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.WorkingDirectory = project.BaseDirectory;
			//info.WindowStyle = ProcessWindowStyle.Hidden;
			info.CreateNoWindow = true;
			
			using (Process process = Process.Start (info))
			{
				process.WaitForExit ();
			}
		}


		public static BuildResult Compile (OpenFLProject project, OpenFLProjectConfiguration configuration, IProgressMonitor monitor)
		{
			string args = "run openfl build \"" + project.TargetProjectXMLFile + "\" " + configuration.Platform.ToLower ();
			
			if (configuration.DebugMode)
			{
				args += " -debug";
			}
			
			if (project.AdditionalArguments != "")
			{
				args += " " + project.AdditionalArguments;
			}
			
			if (configuration.AdditionalArguments != "")
			{
				args += " " + configuration.AdditionalArguments;
			}
			
			string error = "";
			int exitCode = DoCompilation ("haxelib", args, project.BaseDirectory, monitor, ref error);
			
			BuildResult result = ParseOutput (project, error);
			if (result.CompilerOutput.Trim ().Length != 0)
				monitor.Log.WriteLine (result.CompilerOutput);
			
			if (result.ErrorCount == 0 && exitCode != 0)
			{
				string errorMessage = File.ReadAllText (error);
				if (!string.IsNullOrEmpty (errorMessage))
				{
					result.AddError (errorMessage);
				}
				else
				{
					result.AddError ("Build failed. Go to \"Build Output\" for more information");
				}
			}
			
			FileService.DeleteFile (error);
			return result;
		}
		
		
		private static BuildError CreateErrorFromString (OpenFLProject project, string text)
		{
			Match match = mErrorIgnore.Match (text);
			if (match.Success)
				return null;

			match = mErrorFull.Match (text);
			if (!match.Success)
				match = mErrorCmdLine.Match (text);
			if (!match.Success)
				match = mErrorFileChar.Match (text);
			if (!match.Success)
				match = mErrorFileChars.Match (text);
			if (!match.Success)
				match = mErrorFile.Match (text);
			    
			if (!match.Success)
				match = mErrorSimple.Match (text);
			if (!match.Success)
				return null;

			int n;

			BuildError error = new BuildError ();
			error.FileName = match.Result ("${file}") ?? "";
			error.IsWarning = match.Result ("${level}").ToLower () == "warning";
			error.ErrorText = match.Result ("${message}");
			
			if (error.FileName == "${file}")
			{
				error.FileName = "";
			}
			else
			{
				if (!File.Exists (error.FileName))
				{
					if (File.Exists (Path.GetFullPath (error.FileName)))
					{						
						error.FileName = Path.GetFullPath (error.FileName);
					}
					else
					{
						error.FileName = Path.Combine (project.BaseDirectory, error.FileName);
					}
				}
			}

			if (Int32.TryParse (match.Result ("${line}"), out n))
				error.Line = n;
			else
				error.Line = 0;

			if (Int32.TryParse (match.Result ("${column}"), out n))
				error.Column = n+1; //haxe counts zero based
			else
				error.Column = -1;
			
			return error;
		}
		
		
		private static int DoCompilation (string cmd, string args, string wd, IProgressMonitor monitor, ref string error)
		{
			int exitcode = 0;
			error = Path.GetTempFileName ();
			StreamWriter errwr = new StreamWriter (error);

			try
			{
				ProcessStartInfo pinfo = new ProcessStartInfo (cmd, args);
				pinfo.UseShellExecute = false;
				pinfo.RedirectStandardOutput = true;
				pinfo.RedirectStandardError = true;
				pinfo.WorkingDirectory = wd;

				MonoDevelop.Core.Execution.ProcessWrapper pw = Runtime.ProcessService.StartProcess (pinfo, monitor.Log, errwr, null);
				pw.WaitForOutput ();
				exitcode = pw.ExitCode;

				errwr.Close ();
			}
			finally
			{
				if (errwr != null)
				{
					errwr.Dispose ();
				}
			}

			return exitcode;
		}
		
		
		public static string GetHXMLData (OpenFLProject project, OpenFLProjectConfiguration configuration)
		{
			ProcessStartInfo info = new ProcessStartInfo ();
			
			info.FileName = "haxelib";
			info.Arguments = "run openfl update \"" + project.TargetProjectXMLFile + "\" " + configuration.Platform.ToLower () + " " + project.AdditionalArguments + " " + configuration.AdditionalArguments;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.WorkingDirectory = project.BaseDirectory;
			//info.WindowStyle = ProcessWindowStyle.Hidden;
			info.CreateNoWindow = true;
			
			using (Process process = Process.Start (info))
			{
				process.WaitForExit ();
			}
			
			info.Arguments = "run openfl display \"" + project.TargetProjectXMLFile + "\" " + configuration.Platform.ToLower () + " " + project.AdditionalArguments + " " + configuration.AdditionalArguments;
			
			using (Process process = Process.Start (info))
			{
				string data = process.StandardOutput.ReadToEnd ();
				process.WaitForExit ();
				return data.Replace (Environment.NewLine, " ");
			}
		}


		static BuildResult ParseOutput (OpenFLProject project, string stderr)
		{
			BuildResult result = new BuildResult ();

			StringBuilder output = new StringBuilder ();
			ParserOutputFile (project, result, output, stderr);

			result.CompilerOutput = output.ToString ();

			return result;
		}
		
		
		static void ParserOutputFile (OpenFLProject project, BuildResult result, StringBuilder output, string filename)
		{
			StreamReader reader = File.OpenText (filename);

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				output.AppendLine (line);

				line = line.Trim ();
				if (line.Length == 0 || line.StartsWith ("\t"))
					continue;
				
				BuildError error = CreateErrorFromString (project, line);
				if (error != null)
					result.Append (error);
			}

			reader.Close ();
		}
		
		
		private static ExecutionCommand CreateExecutionCommand (OpenFLProject project, OpenFLProjectConfiguration configuration)
		{
			string exe = "haxelib";
			string args = "run openfl run \"" + project.TargetProjectXMLFile + "\" " + configuration.Platform.ToLower ();
			
			if (configuration.DebugMode)
			{
				args += " -debug";
			}
			
			if (project.AdditionalArguments != "")
			{
				args += " " + project.AdditionalArguments;
			}
			
			if (configuration.AdditionalArguments != "")
			{
				args += " " + configuration.AdditionalArguments;
			}

			NativeExecutionCommand cmd = new NativeExecutionCommand (exe);
			cmd.Arguments = args;
			cmd.WorkingDirectory = project.BaseDirectory.FullPath;
			
			return cmd;
		}
		
		
		public static bool CanRun (OpenFLProject project, OpenFLProjectConfiguration configuration, ExecutionContext context)
		{
			ExecutionCommand cmd = CreateExecutionCommand (project, configuration);
			if (cmd == null)
			{
				return false;
			}
			return context.ExecutionHandler.CanExecute (cmd);
		}


		public static void Run (OpenFLProject project, OpenFLProjectConfiguration configuration, IProgressMonitor monitor, ExecutionContext context)
		{
			ExecutionCommand cmd = CreateExecutionCommand (project, configuration);

			IConsole console;
			if (configuration.ExternalConsole)
				console = context.ExternalConsoleFactory.CreateConsole (false);
			else
				console = context.ConsoleFactory.CreateConsole (false);

			AggregatedOperationMonitor operationMonitor = new AggregatedOperationMonitor (monitor);

			try
			{
				if (!context.ExecutionHandler.CanExecute (cmd))
				{
					monitor.ReportError (String.Format ("Cannot execute '{0}'.", cmd), null);
					return;
				}
				
				IProcessAsyncOperation operation = context.ExecutionHandler.Execute (cmd, console);
				
				operationMonitor.AddOperation (operation);
				operation.WaitForCompleted ();

				monitor.Log.WriteLine ("Player exited with code {0}.", operation.ExitCode);
			}
			catch (Exception)
			{
				monitor.ReportError (String.Format ("Error while executing '{0}'.", cmd), null);
			}
			finally
			{
				operationMonitor.Dispose ();
				console.Dispose ();
			}
		}
		
	}
	
}