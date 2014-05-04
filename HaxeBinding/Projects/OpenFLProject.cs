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

	[DataInclude(typeof(OpenFLProjectConfiguration))]
    public class OpenFLProject : Project
	{
		
		[ItemProperty("AdditionalArguments", DefaultValue="")]
		string mAdditionalArguments = string.Empty;
		
		public string AdditionalArguments {
			get { return mAdditionalArguments;  }
			set { mAdditionalArguments = value; }
		}
		
		
		[ItemProperty("TargetProjectXMLFile", DefaultValue="")]
		string mTargetProjectXMLFile = string.Empty;
		
		public string TargetProjectXMLFile {
			get { return mTargetProjectXMLFile;  }
			set { mTargetProjectXMLFile = value; }
		}


		public OpenFLProject () : base()
		{

		}
		
		
		public override void Dispose ()
		{
			HaxeCompilerManager.StopServer ();
			base.Dispose ();
		}


		public OpenFLProject (ProjectCreateInformation info, XmlElement projectOptions) : base()
		{
			if (projectOptions.Attributes ["TargetProjectXMLFile"] != null)
			{
				
				TargetProjectXMLFile = GetOptionAttribute (info, projectOptions, "TargetProjectXMLFile");
				
			}
			
			if (projectOptions.Attributes ["AdditionalArguments"] != null)
			{
				
				AdditionalArguments = GetOptionAttribute (info, projectOptions, "AdditionalArguments");
				
			}
			
			OpenFLProjectConfiguration configuration;
			
			string[] targets = new string[] { "Android", "BlackBerry", "Flash", "HTML5", "iOS", "Linux", "Mac", "webOS", "Windows" };
			
			foreach (string target in targets)
			{
				
				configuration = (OpenFLProjectConfiguration)CreateConfiguration ("Debug");
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
				
				configuration = (OpenFLProjectConfiguration)CreateConfiguration ("Release");
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
			OpenFLProjectConfiguration conf = new OpenFLProjectConfiguration ();
			conf.Name = name;
			return conf;
		}
		
		
		protected override BuildResult DoBuild (IProgressMonitor monitor, ConfigurationSelector configurationSelector)
		{
			OpenFLProjectConfiguration haxeConfig = (OpenFLProjectConfiguration)GetConfiguration (configurationSelector);
			return OpenFLCommandLineToolsManager.Compile (this, haxeConfig, monitor);
		}
		
		
		protected override void DoClean (IProgressMonitor monitor, ConfigurationSelector configurationSelector)
		{
			OpenFLProjectConfiguration haxeConfig = (OpenFLProjectConfiguration)GetConfiguration (configurationSelector);
			OpenFLCommandLineToolsManager.Clean (this, haxeConfig, monitor);
		}
		
		
		protected override void DoExecute (IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			OpenFLProjectConfiguration haxeConfig = (OpenFLProjectConfiguration)GetConfiguration (configurationSelector);
			OpenFLCommandLineToolsManager.Run (this, haxeConfig, monitor, context);
		}
		
		
		protected string GetOptionAttribute (ProjectCreateInformation info, XmlElement projectOptions, string attributeName)
		{
			string value = projectOptions.Attributes [attributeName].InnerText;
			value = value.Replace ("${ProjectName}", info.ProjectName);
			return value;
		}
		
		
		public override IEnumerable<string> GetProjectTypes ()
		{
			yield return "OpenFL";
			//foreach (var t in base.GetProjectTypes ())
				//yield return t;
		}
		
		
		public override bool IsCompileable (string fileName)
		{
			return true;
		}
		
		
		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			OpenFLProjectConfiguration haxeConfig = (OpenFLProjectConfiguration)GetConfiguration (configurationSelector);
			return OpenFLCommandLineToolsManager.CanRun (this, haxeConfig, context);
		}


		public override string ProjectType {
			get { return "OpenFL"; }
		}
		

		public override string[] SupportedLanguages {
			get { return new string[] { "", "Haxe", "XML" }; }
		}
		
	}
	
}