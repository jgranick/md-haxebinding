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

	[DataInclude(typeof(HaxeProjectConfiguration))]
    public class HaxeProject : Project
	{
		
		[ItemProperty("AdditionalArguments", DefaultValue="")]
		string mAdditionalArguments = string.Empty;
		
		public string AdditionalArguments {
			get { return mAdditionalArguments;  }
			set { mAdditionalArguments = value; }
		}
		
		
		[ItemProperty("TargetHXMLFile", DefaultValue="")]
		string mTargetHXMLFile = string.Empty;
		
		public string TargetHXMLFile {
			get { return mTargetHXMLFile;  }
			set { mTargetHXMLFile = value; }
		}


		public HaxeProject () : base()
		{
			
		}
		
		
		public override void Dispose ()
		{
			HaxeCompilerManager.StopServer ();
			base.Dispose ();
		}


		public HaxeProject (ProjectCreateInformation info, XmlElement projectOptions) : base()
		{
			if (projectOptions.Attributes ["TargetHXMLFile"] != null)
			{
				
				TargetHXMLFile = GetOptionAttribute (info, projectOptions, "TargetHXMLFile");
				
			}
			
			if (projectOptions.Attributes ["AdditionalArguments"] != null)
			{
				
				AdditionalArguments = GetOptionAttribute (info, projectOptions, "AdditionalArguments");
				
			}
			
			HaxeProjectConfiguration configuration;
			
			
			configuration = (HaxeProjectConfiguration)CreateConfiguration ("Debug");
			configuration.DebugMode = true;
			//configuration.Platform = target;
			Configurations.Add (configuration);
			
			configuration = (HaxeProjectConfiguration)CreateConfiguration ("Release");
			configuration.DebugMode = false;
			//configuration.Platform = target;
			Configurations.Add (configuration);
		}
		
		
		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			HaxeProjectConfiguration conf = new HaxeProjectConfiguration ();
			conf.Name = name;
			return conf;
		}
		
		
		protected override BuildResult DoBuild (IProgressMonitor monitor, ConfigurationSelector configurationSelector)
		{
			HaxeProjectConfiguration haxeConfig = (HaxeProjectConfiguration)GetConfiguration (configurationSelector);
			return HaxeCompilerManager.Compile (this, haxeConfig, monitor);
		}
		
		
		protected override void DoClean (IProgressMonitor monitor, ConfigurationSelector configuration)
		{
			//base.DoClean (monitor, configuration);
		}
		
		
		protected override void DoExecute (IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			HaxeProjectConfiguration haxeConfig = (HaxeProjectConfiguration)GetConfiguration (configurationSelector);
			HaxeCompilerManager.Run (this, haxeConfig, monitor, context);
		}
		
		
		protected string GetOptionAttribute (ProjectCreateInformation info, XmlElement projectOptions, string attributeName)
		{
			string value = projectOptions.Attributes [attributeName].InnerText;
			value = value.Replace ("${ProjectName}", info.ProjectName);
			return value;
		}
		
		
		public override IEnumerable<string> GetProjectTypes ()
		{
			yield return "Haxe";
			//foreach (var t in base.GetProjectTypes ())
				//yield return t;
		}
		
		
		public override bool IsCompileable (string fileName)
		{
			return true;
		}
		
		
		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configurationSelector)
		{
			HaxeProjectConfiguration haxeConfig = (HaxeProjectConfiguration)GetConfiguration (configurationSelector);
			return HaxeCompilerManager.CanRun (this, haxeConfig, context);
		}


		public override string ProjectType {
			get { return "Haxe"; }
		}
		

		public override string[] SupportedLanguages {
			get { return new string[] { "", "Haxe", "HXML" }; }
		}
		
	}
	
}