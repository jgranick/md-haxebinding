using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Diagnostics;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects;

using MonoDevelop.HaxeBinding.Tools;


// This file was taken from the old "FlexBinding" add-in and has not been adapted yet


namespace MonoDevelop.HaxeBinding.Projects
{
    public enum Language { Haxe }

    public enum TargetFormat { SWF }

    public enum RunMode { SystemPlugin, CustomPlugin, StandalonePlayer }

    [DataInclude(typeof(HaxeProjectConfiguration))]
    public class HaxeProject : Project
    {
        [ProjectPathItemProperty("MainSource")]
        string mMainSource;

        [ItemProperty("MainLanguage")]
        Language mMainLanguage;

        [ItemProperty("TargetFormat")]
        TargetFormat mTargetFormat;

        [ItemProperty("RunMode")]
        RunMode mRunMode;

        public HaxeProject() : base()
        {
        }

        public HaxeProject(ProjectCreateInformation info, XmlElement projectOptions, string language) : base()
        {
            switch (language)
            {
                case "Haxe":
                    MainLanguage = Language.Haxe;
                    break;
            }

            if (projectOptions != null) {
                if (projectOptions.Attributes["TargetFormat"] != null) {
                    TargetFormat = (TargetFormat)Enum.Parse(typeof(TargetFormat), projectOptions.Attributes["TargetFormat"].InnerText);
                }
                else {
                    TargetFormat = TargetFormat.SWF;
                }
            }

            if (info != null) {
                MainSource = info.ProjectName + ".hx";
                Name = info.ProjectName;
            }

            HaxeProjectConfiguration configuration;

            configuration = (HaxeProjectConfiguration)CreateConfiguration("Debug");
            configuration.DebugMode = true;
            configuration.OutputFileName = GetDefaultOutputFileName(configuration);
            Configurations.Add(configuration);

            configuration = (HaxeProjectConfiguration)CreateConfiguration("Release");
            configuration.DebugMode = false;
            configuration.OutputFileName = GetDefaultOutputFileName(configuration);
            Configurations.Add(configuration);

            foreach (HaxeProjectConfiguration conf in Configurations) {
                conf.OutputDirectory = new FilePath(Path.Combine(info != null ? info.BinPath.FileName : ".", conf.Id));
            }
        }

        public override string ProjectType {
            get { return "Haxe"; }
        }
		
		// TODO find StockIcon in new API
        /*public override string StockIcon {
            get { return "flex-mxml-project"; }
        }*/

        public override string[] SupportedLanguages {
            get { return new string[] { "", "Haxe", "NMML" }; }
        }

        public string MainSource {
            get { return mMainSource;  }
            set {
                mMainSource = value;
				
				MainLanguage = Language.Haxe;
            }
        }

        public Language MainLanguage {
            get { return mMainLanguage;  }
            set { mMainLanguage = value; }
        }

        public TargetFormat TargetFormat {
            get { return mTargetFormat;  }
            set { mTargetFormat = value; }
        }

        public RunMode RunMode {
            get { return mRunMode;  }
            set { mRunMode = value; }
        }

        public string Compiler {
            get {
                string cmd = "mxmlc";
                return cmd;
            }
        }

        public string GetDefaultOutputFileName(HaxeProjectConfiguration configuration)
        {
            return Path.GetFileNameWithoutExtension(MainSource) + "." + TargetFormat.ToString().ToLower();
        }

        public override bool IsCompileable(string fileName)
        {
            string ext = Path.GetExtension(fileName.ToLower());
            return ext == ".hx" || ext == ".nmml";
        }
		
		protected override BuildResult DoBuild (IProgressMonitor monitor, ConfigurationSelector configurationSelector)
		{
			HaxeProjectConfiguration haxeConfig = (HaxeProjectConfiguration)GetConfiguration(configurationSelector);
			return HaxeCompilerManager.Compile(this, haxeConfig, monitor);
		}

        /*protected override BuildResult DoBuild(IProgressMonitor monitor, string configurationName)
        {
            FlexProjectConfiguration configuration = (FlexProjectConfiguration)GetConfiguration(configurationName);
            return FlexCompilerManager.Compile(this, configuration, monitor);
        }*/

		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			return TargetFormat == TargetFormat.SWF;
		}
		
        /*protected override bool OnGetCanExecute(ExecutionContext context, string configurationName)
        {
            return TargetFormat == TargetFormat.SWF;
        }*/

		protected override void DoExecute (IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			HaxeProjectConfiguration configuration = (HaxeProjectConfiguration)GetConfiguration(configurationSelector);
			
			switch (mRunMode)
            {
                case RunMode.SystemPlugin:
                    DoExecuteWithSystemPlugin(monitor, context, configuration);
                    break;

               /* case RunMode.CustomPlugin:
                    DoExecuteWithExe(monitor, context, PropertyService.Get<string>("CBinding.FlexBrowserPath"), configuration);
                    break;

                case RunMode.StandalonePlayer:
                    DoExecuteWithExe(monitor, context, PropertyService.Get<string>("CBinding.FlexPlayerPath"), configuration);
                    break;*/
            }
		}
		
        /*protected override void DoExecute(IProgressMonitor monitor, ExecutionContext context, string configurationName)
        {
            FlexProjectConfiguration configuration = (FlexProjectConfiguration)GetConfiguration(configurationName);

            switch (mRunMode)
            {
                case RunMode.SystemPlugin:
                    DoExecuteWithSystemPlugin(monitor, context, configuration);
                    break;

                case RunMode.CustomPlugin:
                    DoExecuteWithExe(monitor, context, PropertyService.Get<string>("CBinding.FlexBrowserPath"), configuration);
                    break;

                case RunMode.StandalonePlayer:
                    DoExecuteWithExe(monitor, context, PropertyService.Get<string>("CBinding.FlexPlayerPath"), configuration);
                    break;
            }
        }*/

        void DoExecuteWithSystemPlugin(IProgressMonitor monitor, ExecutionContext context, HaxeProjectConfiguration configuration)
        {
            monitor.Log.WriteLine("Invoking system default browser...");

            String args = "file://"+Path.GetFullPath(Path.Combine(configuration.OutputDirectory, configuration.OutputFileName));

            Process.Start(args);
        }

        void DoExecuteWithExe(IProgressMonitor monitor, ExecutionContext context, string exe, HaxeProjectConfiguration configuration)
        {
            monitor.Log.WriteLine("Running project using '{0}' ...", exe);

            if (string.IsNullOrEmpty(exe)) {
                monitor.ReportError(String.Format("No custom player or browser configured."), null);
                return;
            }

            string[] parts = exe.Split(' ');
            string args = "file://"+Path.GetFullPath(Path.Combine(configuration.OutputDirectory, configuration.OutputFileName));
            if (parts.Length > 1)
                args = string.Join(" ", parts, 1, parts.Length-1) + " " + args;
            exe = parts[0];

            IConsole console;
            if (configuration.ExternalConsole)
                console = context.ExternalConsoleFactory.CreateConsole(false);
            else
                console = context.ConsoleFactory.CreateConsole(false);

            AggregatedOperationMonitor operationMonitor = new AggregatedOperationMonitor(monitor);

            try {
                NativeExecutionCommand cmd = new NativeExecutionCommand(exe);
                cmd.Arguments = args;
                cmd.WorkingDirectory = Path.GetFullPath(configuration.OutputDirectory);

                if (!context.ExecutionHandler.CanExecute(cmd)) {
                    monitor.ReportError(String.Format("Cannot execute '{0} {1}'.", exe, args), null);
                    return;
                }

                IProcessAsyncOperation operation = context.ExecutionHandler.Execute(cmd, console);

                operationMonitor.AddOperation(operation);
                operation.WaitForCompleted();

                monitor.Log.WriteLine("Player exited with code {0}.", operation.ExitCode);
            }
            catch (Exception) {
                    monitor.ReportError(String.Format("Error while executing '{0} {1}'.", exe, args), null);
            }
            finally {
                operationMonitor.Dispose();
                console.Dispose();
            }
        }

		public override FilePath GetOutputFileName (ConfigurationSelector configurationSelector)
		{
			HaxeProjectConfiguration configuration = (HaxeProjectConfiguration)GetConfiguration(configurationSelector);
			return GetDefaultOutputFileName(configuration);
		}
		
        /*protected override string OnGetOutputFileName(string configurationName)
        {
            return GetDefaultOutputFileName((FlexProjectConfiguration)GetConfiguration(configurationName));
        }*/

        public override SolutionItemConfiguration CreateConfiguration(string name)
        {
            HaxeProjectConfiguration conf = new HaxeProjectConfiguration();
            conf.Name = name;
            return conf;
        }

        protected override void OnFileRenamedInProject(ProjectFileRenamedEventArgs e)
        {
			//e.
            //if (e.OldName == MainSource)
            //    MainSource = e.NewName;
            //base.OnFileRenamedInProject(e);
        }

        /* protected override void OnFileAddedToProject(ProjectFileEventArgs e)
        {
            base.OnFileAddedToProject(e);

            if (!Loading && !IsCompileable(e.ProjectFile.Name) && e.ProjectFile.BuildAction == BuildAction.Compile)
                e.ProjectFile.BuildAction = BuildAction.None;
        }

        protected override void OnFileChangedInProject(ProjectFileEventArgs e)
        {
            base.OnFileChangedInProject (e);
       }*/
    }
}
