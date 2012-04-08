using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.HaxeBinding.Projects;

using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Ide.CodeCompletion;


namespace MonoDevelop.HaxeBinding.Tools
{
	static class NMECommandLineToolsManager
	{
		public static BuildResult Compile (NMEProject project, NMEProjectConfiguration configuration, IProgressMonitor monitor)
		{
			string args = "run nme build " + project.TargetNMMLFile + " " + configuration.Platform.ToLower ();
			
			if (configuration.DebugMode) {
				
				args += " -debug";
				
			}
			
			if (project.AdditionalArguments != "") {
				
				args += " " + project.AdditionalArguments;
				
			}
			
			if (configuration.AdditionalArguments != "") {
				
				args += " " + configuration.AdditionalArguments;
				
			}
			
			//string output = "";
			string error = "";
			
			int exitCode = DoCompilation ("haxelib", args, project.BaseDirectory, monitor, ref error);
			
			BuildResult result = ParseOutput (error, project.BaseDirectory);
			if (result.CompilerOutput.Trim ().Length != 0)
				monitor.Log.WriteLine (result.CompilerOutput);

			// If compiler crashes, output entire error string.

			if (result.ErrorCount == 0 && exitCode != 0) {
				if (!string.IsNullOrEmpty (error))
					result.AddError (error);
				else
					result.AddError ("The compiler appears to have crashed without any error output.");
			}
			
			//FileService.DeleteFile(output);
			FileService.DeleteFile (error);
			return result;
			
			/*StringBuilder sb = new StringBuilder();

            sb.AppendFormat(" -output \"{0}/{1}\" ", configuration.OutputDirectory, configuration.OutputFileName);

            if (configuration.DebugMode)
                sb.Append("-compiler.debug=true ");

            if (!String.IsNullOrEmpty(configuration.CompilerParameters)) {
                sb.Append(configuration.CompilerParameters);
                sb.Append(" ");
            }

            sb.Append(project.MainSource);

            string output = "";
            string error  = "";

            string cmd = Path.Combine(Path.Combine(PropertyService.Get<string>("CBinding.FlexSdkPath"), "bin"), project.Compiler);
            string args = sb.ToString();
            monitor.Log.WriteLine(cmd+" "+args);

            int exitCode = DoCompilation(cmd, args, project.BaseDirectory, ref output, ref error);

            BuildResult result = ParseOutput(output, error);
            if (result.CompilerOutput.Trim().Length != 0)
                monitor.Log.WriteLine(result.CompilerOutput);

            // If compiler crashes, output entire error string.

            if (result.ErrorCount == 0 && exitCode != 0) {
                if (!string.IsNullOrEmpty(error))
                    result.AddError(error);
                else
                    result.AddError("The compiler appears to have crashed without any error output.");
            }

            FileService.DeleteFile(output);
            FileService.DeleteFile(error);
            return result;*/
		}
		
		public static string GetCompletionData (NMEProject project, string classPath, string fileName, int position)
		{
			string hxml = GetHXMLPath (project);
			
			string args = "\"" + hxml + "\" -cp \"" + classPath + "\" --display \"" + fileName + "\"@" + position + " -D use_rtti_doc";
			
			Process p = new Process ();
			p.StartInfo.FileName = "haxe";
			p.StartInfo.Arguments = args;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.WorkingDirectory = project.BaseDirectory;
			p.Start();
			
			string data = p.StandardError.ReadToEnd ();
			
			p.WaitForExit();
			
			//MonoDevelop.Ide.MessageService.ShowMessage ("haxe " + args);
			//MonoDevelop.Ide.MessageService.ShowMessage ("Data: " + data.Length);
			
			return data;
			
		}
		
		public static string GetHXMLPath (NMEProject project)
		{
			string directory = Path.GetDirectoryName (project.TargetNMMLFile);
			
			if (directory == "") {
				
				directory = project.BaseDirectory;
				
			}
			
			return Path.Combine (directory, Path.GetFileNameWithoutExtension (project.TargetNMMLFile) + ".hxml");
		}
		
		public static void Run (NMEProject project, NMEProjectConfiguration configuration, IProgressMonitor monitor, ExecutionContext context)
		{
			string exe = "haxelib";
			string args = "run nme run " + project.TargetNMMLFile + " " + configuration.Platform.ToLower ();
			
			if (configuration.DebugMode) {
				
				args += " -debug";
				
			}
			
			if (project.AdditionalArguments != "") {
				
				args += " " + project.AdditionalArguments;
				
			}
			
			if (configuration.AdditionalArguments != "") {
				
				args += " " + configuration.AdditionalArguments;
				
			}

			IConsole console;
			if (configuration.ExternalConsole)
				console = context.ExternalConsoleFactory.CreateConsole (false);
			else
				console = context.ConsoleFactory.CreateConsole (false);

			AggregatedOperationMonitor operationMonitor = new AggregatedOperationMonitor (monitor);

			try {
				
				NativeExecutionCommand cmd = new NativeExecutionCommand (exe);
				cmd.Arguments = args;
				cmd.WorkingDirectory = project.BaseDirectory.FullPath;

				if (!context.ExecutionHandler.CanExecute (cmd)) {
					monitor.ReportError (String.Format ("Cannot execute '{0} {1}'.", exe, args), null);
					return;
				}
				
				IProcessAsyncOperation operation = context.ExecutionHandler.Execute (cmd, console);
				
				operationMonitor.AddOperation (operation);
				
				//while (!operation.IsCompleted) {
					
				//MonoDevelop.Ide.IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor ("MonoDevelop.Ide.ApplicationOutput", GettextCatalog.GetString ("Application Output"), MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true).Log.WriteLine ("hi");
					
				//}
				
				operation.WaitForCompleted ();

				monitor.Log.WriteLine ("Player exited with code {0}.", operation.ExitCode);
			} catch (Exception) {
				monitor.ReportError (String.Format ("Error while executing '{0} {1}'.", exe, args), null);
			} finally {
				operationMonitor.Dispose ();
				console.Dispose ();
			}
		}

		static void ParserOutputFile (BuildResult result, StringBuilder output, string filename, string working_directory)
		{
			StreamReader reader = File.OpenText (filename);

			string line;
			while ((line = reader.ReadLine()) != null) {
				output.AppendLine (line);

				line = line.Trim ();
				if (line.Length == 0 || line.StartsWith ("\t"))
					continue;

				BuildError error = CreateErrorFromString (line);
				if (error != null)
				{
					if (error.FileName.Length>0)
						error.FileName = working_directory + "/" + error.FileName;
					result.Append (error);
				}
			}

			reader.Close ();
		}

		static BuildResult ParseOutput (string stderr, string working_directory)
		{
			BuildResult result = new BuildResult ();

			StringBuilder output = new StringBuilder ();

			//ParserOutputFile(result, output, stdout);
			ParserOutputFile (result, output, stderr, working_directory);

			result.CompilerOutput = output.ToString ();

			return result;
		}

		static int DoCompilation (string cmd, string args, string wd, IProgressMonitor monitor, ref string error)
		{
			int exitcode = 0;

			//output = Path.GetTempFileName();
			error = Path.GetTempFileName ();

			//StreamWriter outwr = new StreamWriter(output);
			StreamWriter errwr = new StreamWriter (error);

			ProcessStartInfo pinfo = new ProcessStartInfo (cmd, args);
			pinfo.UseShellExecute = false;
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;
			pinfo.WorkingDirectory = wd;

			using (MonoDevelop.Core.Execution.ProcessWrapper pw = Runtime.ProcessService.StartProcess(pinfo, monitor.Log, errwr, null)) {
				pw.WaitForOutput ();
				exitcode = pw.ExitCode;
			}
			//outwr.Close();
			errwr.Close ();

			return exitcode;
		}

		static Regex mErrorFull = new Regex (@"^(?<file>.+)\((?<line>\d+)\):\s(col:\s)?(?<column>\d*)\s?(?<level>\w+):\s(?<message>.*)\.?$",
                                            RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex mErrorFile = new Regex (@"^(?<file>.+):(?<line>\d+):\s(?<message>.*)\.?$",
                                            RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex mErrorCmdLine = new Regex (@"^command line: (?<level>\w+):\s(?<message>.*)\.?$",
                                              RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex mErrorSimple = new Regex (@"^(?<level>\w+):\s(?<message>.*)\.?$",
                                              RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex mErrorIgnore = new Regex (@"^(Updated|Recompile|Reason|Files changed):.*",
                                              RegexOptions.Compiled);

		static BuildError CreateErrorFromString (string text)
		{
			Match match = mErrorIgnore.Match (text);
			if (match.Success)
				return null;

			match = mErrorFull.Match (text);
			if (!match.Success)
				match = mErrorCmdLine.Match (text);
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
				error.FileName = "";

			if (Int32.TryParse (match.Result ("${line}"), out n))
				error.Line = n;
			else
				error.Line = 0;

			if (Int32.TryParse (match.Result ("${column}"), out n))
				error.Column = n;
			else
				error.Column = -1;

			return error;
		}
	}
}
