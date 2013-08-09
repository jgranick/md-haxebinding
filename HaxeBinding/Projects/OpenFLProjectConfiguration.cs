using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;


namespace MonoDevelop.HaxeBinding.Projects
{

	public class OpenFLProjectConfiguration : ProjectConfiguration
	{
		
		[ItemProperty("AdditionalArguments", DefaultValue="")]
		string mAdditionalArguments = string.Empty;

		public string AdditionalArguments {
			get { return mAdditionalArguments;  }
			set { mAdditionalArguments = value; }
		}


		public override void CopyFrom (ItemConfiguration configuration)
		{
			base.CopyFrom (configuration);

			OpenFLProjectConfiguration other = (OpenFLProjectConfiguration)configuration;
			mAdditionalArguments = other.mAdditionalArguments;
		}
		
	}
	
}