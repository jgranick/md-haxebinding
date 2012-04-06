// This file is part of the MonoDevelop Flex Language Binding.
//
// Copyright (c) 2009 Studio Associato Di Nunzio e Di Gregorio
//
//  Authors:
//     Federico Di Gregorio <fog@initd.org>
//
// This source code is licenced under The MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
