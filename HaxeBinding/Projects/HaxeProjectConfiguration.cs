using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects;


// This file was taken from the old "FlexBinding" add-in and has not been adapted yet


namespace MonoDevelop.HaxeBinding.Projects
{
    public class HaxeProjectConfiguration : ProjectConfiguration
    {
        [ItemProperty("CompilerParameters", DefaultValue="")]
        string mCompilerParameters = string.Empty;

        [ItemProperty("OutputFileName", DefaultValue="")]
        string mOutputFileName = string.Empty;

        public string CompilerParameters {
            get { return mCompilerParameters;  }
            set { mCompilerParameters = value; }
        }

        public string OutputFileName {
            get { return mOutputFileName;  }
            set { mOutputFileName = value; }
        }

        public void AdjustOutputFileName(TargetFormat format)
        {
            OutputFileName = Path.GetFileNameWithoutExtension(OutputFileName) + "." + format.ToString().ToLower();
        }

        public override void CopyFrom(ItemConfiguration configuration)
        {
            base.CopyFrom(configuration);

            HaxeProjectConfiguration other = (HaxeProjectConfiguration)configuration;

            mOutputFileName = other.mOutputFileName;
            mCompilerParameters = other.mCompilerParameters;
        }
    }
}
