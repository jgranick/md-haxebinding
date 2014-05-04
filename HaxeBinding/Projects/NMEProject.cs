using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Diagnostics;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.HaxeBinding.Tools;


namespace MonoDevelop.HaxeBinding.Projects
{

	[DataInclude(typeof(NMEProjectConfiguration))]
    public class NMEProject : Project
	{
		
		[ItemProperty("AdditionalArguments", DefaultValue="")]
		string mAdditionalArguments = string.Empty;
		
		public string AdditionalArguments {
			get { return mAdditionalArguments;  }
			set { mAdditionalArguments = value; }
		}
		
		
		[ItemProperty("TargetNMMLFile", DefaultValue="")]
		string mTargetNMMLFile = string.Empty;
		
		public string TargetNMMLFile {
			get { return mTargetNMMLFile;  }
			set { mTargetNMMLFile = value; }
		}


		public NMEProject () : base()
		{

		}
		
		
		public override void Dispose ()
		{
			HaxeCompilerManager.StopServer ();
			base.Dispose ();
		}


		public NMEProject (ProjectCreateInformation info, XmlElement projectOptions) : base()
		{
			if (projectOptions.Attributes ["TargetNMMLFile"] != null)
			{
				
				TargetNMMLFile = GetOptionAttribute (info, projectOptions, "TargetNMMLFile");
				
			}
			
			if (projectOptions.Attributes ["AdditionalArguments"] != null)
			{
				
				AdditionalArguments = GetOptionAttribute (info, projectOptions, "AdditionalArguments");
				
			}
			
			NMEProjectConfiguration configuration;
			
			string[] targets = new string[] { "Android", "BlackBerry", "Flash", "HTML5", "iOS", "Linux", "Mac", "webOS", "Windows" };
			
			foreach (string target in targets)
			{
				
				configuration = (NMEProjectConfiguration)CreateConfiguration ("Debug");
				configuration.DebugMode = true;
				configuration.Platform = target;
				
				if (target == "iOS")
				{
					
					configuration.AdditionalArguments = "-simulator";
						
				}
				
				Configurations.Add (configuration);
				
			}
			
			foreach (string target in targets)
			{
				
				configuration = (NMEProjectConfiguration)CreateConfiguration ("Release");
				configuration.DebugMode = false;
				configuration.Platform = target;
				
				if (target == "iOS")
				{
					
					configuration.AdditionalArguments = "-simulator";
						
				}
				
				Configurations.Add (configuration);
				
			}
		}
		
		
		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			NMEProjectConfiguration conf = new NMEProjectConfiguration ();
			conf.Name = name;
			return conf;
		}
		
		
		protected override BuildResult DoBuild (IProgressMonitor monitor, ConfigurationSelector configurationSelector)
		{
			NMEProjectConfiguration haxeConfig = (NMEProjectConfiguration)GetConfiguration (configurationSelector);
			return NMECommandLineToolsManager.Compile (this, haxeConfig, monitor);
		}
		
		
		protected override void DoClean (IProgressMonitor monitor, ConfigurationSelector configurationSelector)
		{
			NMEProjectConfiguration haxeConfig = (NMEProjectConfiguration)GetConfiguration (configurationSelector);
			NMECommandLineToolsManager.Clean (this, haxeConfig, monitor);
		}
		
		
		protected override void DoExecute (IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			NMEProjectConfiguration haxeConfig = (NMEProjectConfiguration)GetConfiguration (configurationSelector);
			NMECommandLineToolsManager.Run (this, haxeConfig, monitor, context);
		}
		
		
		protected string GetOptionAttribute (ProjectCreateInformation info, XmlElement projectOptions, string attributeName)
		{
			string value = projectOptions.Attributes [attributeName].InnerText;
			value = value.Replace ("${ProjectName}", info.ProjectName);
			return value;
		}
		
		
		public override IEnumerable<string> GetProjectTypes ()
		{
			yield return "NME";
			//foreach (var t in base.GetProjectTypes ())
				//yield return t;
		}
		
		
		public override bool IsCompileable (string fileName)
		{
			return true;
		}
		
		
		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			NMEProjectConfiguration haxeConfig = (NMEProjectConfiguration)GetConfiguration (configurationSelector);
			return NMECommandLineToolsManager.CanRun (this, haxeConfig, context);
		}


		public override string ProjectType {
			get { return "NME"; }
		}
		

		public override string[] SupportedLanguages {
			get { return new string[] { "", "Haxe", "NMML" }; }
		}
		
	}
	
}