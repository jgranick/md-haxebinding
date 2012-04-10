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

	static class HaxeCompilerManager
	{
		
		private static string cacheArgumentsGlobal;
		private static string cacheArgumentsPlatform;
		private static string cacheHXML;
		private static string cachePlatform;
		private static DateTime cacheNMMLTime;
		
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

		
		public static BuildResult Compile (HaxeProject project, HaxeProjectConfiguration configuration, IProgressMonitor monitor)
		{
			string exe = "haxe";
			//string args = project.TargetHXMLFile;
			
			string hxmlPath = Path.GetFullPath (project.TargetHXMLFile);
			
			if (!File.Exists (hxmlPath))
			{
				hxmlPath = Path.Combine (project.BaseDirectory, project.TargetHXMLFile);
			}
			
			string hxml = File.ReadAllText (hxmlPath);
			hxml = hxml.Replace (Environment.NewLine, " ");
			string[] hxmlArgs = hxml.Split (' ');
			
			bool createNext = false;
			
			foreach (string hxmlArg in hxmlArgs)
			{
				if (createNext)
				{
					if (!hxmlArg.StartsWith ("-"))
					{
						string path = Path.GetFullPath (Path.GetDirectoryName (hxmlArg));
						if (!Directory.Exists (path))
						{
							path = Path.Combine (project.BaseDirectory, hxmlArg);
							if (!Directory.Exists (Path.GetDirectoryName (path)))
							{
								Directory.CreateDirectory (Path.GetDirectoryName (path));
							}
						}
					}
					createNext = false;
				}
				
				if (hxmlArg == "-js" || hxmlArg == "-swf" || hxmlArg == "-swf9" || hxmlArg == "-neko")
				{
					createNext = true;
				}
			}
			
			string args = String.Join (" ", hxmlArgs);
			
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
			int exitCode = DoCompilation (exe, args, project.BaseDirectory, monitor, ref error);
			
			BuildResult result = ParseOutput (project, error);
			if (result.CompilerOutput.Trim ().Length != 0)
				monitor.Log.WriteLine (result.CompilerOutput);

			if (result.ErrorCount == 0 && exitCode != 0)
			{
				if (!string.IsNullOrEmpty (error))
					result.AddError (error);
				else
					result.AddError ("The compiler appears to have crashed without any error output.");
			}
			
			FileService.DeleteFile (error);
			return result;
		}
		
		
		private static BuildError CreateErrorFromString (HaxeProject project, string text)
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

			ProcessStartInfo pinfo = new ProcessStartInfo (cmd, args);
			pinfo.UseShellExecute = false;
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;
			pinfo.WorkingDirectory = wd;

			using (MonoDevelop.Core.Execution.ProcessWrapper pw = Runtime.ProcessService.StartProcess(pinfo, monitor.Log, errwr, null))
			{
				pw.WaitForOutput ();
				exitcode = pw.ExitCode;
			}
			errwr.Close ();

			return exitcode;
		}
		
		
		public static string GetCompletionData (Project project, string classPath, string fileName, int position)
		{
			string exe = "haxe";
			string args = "";
			
			if (project is NMEProject) {
				
				NMEProjectConfiguration configuration = project.GetConfiguration (MonoDevelop.Ide.IdeApp.Workspace.ActiveConfiguration) as NMEProjectConfiguration;
				
				string platform = configuration.Platform.ToLower ();
				string path = ((NMEProject)project).TargetNMMLFile;
				
				if (!File.Exists (path))
				{
					path = Path.Combine (project.BaseDirectory, path);
				}
				
				DateTime time = File.GetLastWriteTime (Path.GetFullPath (path));
				
				if (!time.Equals (cacheNMMLTime) || platform != cachePlatform || configuration.AdditionalArguments != cacheArgumentsPlatform || ((NMEProject)project).AdditionalArguments != cacheArgumentsGlobal)
				{
					cacheHXML = NMECommandLineToolsManager.GetHXMLData ((NMEProject)project, configuration);
					cacheNMMLTime = time;
					cachePlatform = platform;
					cacheArgumentsGlobal = ((NMEProject)project).AdditionalArguments;
					cacheArgumentsPlatform = configuration.AdditionalArguments;
				}
				
				args = cacheHXML + " " + ((NMEProject)project).AdditionalArguments + " " + configuration.AdditionalArguments;
				
			} else if (project is HaxeProject) {
				
				HaxeProjectConfiguration configuration = project.GetConfiguration (MonoDevelop.Ide.IdeApp.Workspace.ActiveConfiguration) as HaxeProjectConfiguration;
				
				args = "\"" + ((HaxeProject)project).TargetHXMLFile + "\" " + ((HaxeProject)project).AdditionalArguments + " " + configuration.AdditionalArguments;
				
			} else {
				
				return "";
				
			}
			
			args += " -cp \"" + classPath + "\" --display \"" + fileName + "\"@" + position + " -D use_rtti_doc";
			
			Process process = new Process ();
			process.StartInfo.FileName = exe;
			process.StartInfo.Arguments = args;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.WorkingDirectory = project.BaseDirectory;
			process.Start ();
			
			string result = process.StandardError.ReadToEnd ();
			process.WaitForExit ();
			return result;
		}


		static BuildResult ParseOutput (HaxeProject project, string stderr)
		{
			BuildResult result = new BuildResult ();

			StringBuilder output = new StringBuilder ();
			ParserOutputFile (project, result, output, stderr);

			result.CompilerOutput = output.ToString ();

			return result;
		}
		
		
		static void ParserOutputFile (HaxeProject project, BuildResult result, StringBuilder output, string filename)
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


		public static void Run (HaxeProject project, HaxeProjectConfiguration configuration, IProgressMonitor monitor, ExecutionContext context)
		{
			/*string exe = "haxelib";
			string args = "run nme run \"" + project.TargetNMMLFile + "\" " + configuration.Platform.ToLower ();
			
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

			IConsole console;
			if (configuration.ExternalConsole)
				console = context.ExternalConsoleFactory.CreateConsole (false);
			else
				console = context.ConsoleFactory.CreateConsole (false);

			AggregatedOperationMonitor operationMonitor = new AggregatedOperationMonitor (monitor);

			try
			{
				NativeExecutionCommand cmd = new NativeExecutionCommand (exe);
				cmd.Arguments = args;
				cmd.WorkingDirectory = project.BaseDirectory.FullPath;

				if (!context.ExecutionHandler.CanExecute (cmd))
				{
					monitor.ReportError (String.Format ("Cannot execute '{0} {1}'.", exe, args), null);
					return;
				}
				
				IProcessAsyncOperation operation = context.ExecutionHandler.Execute (cmd, console);
				
				operationMonitor.AddOperation (operation);
				operation.WaitForCompleted ();

				monitor.Log.WriteLine ("Player exited with code {0}.", operation.ExitCode);
			}
			catch (Exception)
			{
				monitor.ReportError (String.Format ("Error while executing '{0} {1}'.", exe, args), null);
			}
			finally
			{
				operationMonitor.Dispose ();
				console.Dispose ();
			}*/
		}
		
	}
	
}
