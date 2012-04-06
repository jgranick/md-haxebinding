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
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;

namespace MonoDevelop.HaxeBinding.Projects.Gui
{
    public class NMEOutputOptionsPanel : MultiConfigItemOptionsPanel
    {
        private NMEOutputOptionsWidget mWidget;

        public override Gtk.Widget CreatePanelWidget()
        {
            return (mWidget = new NMEOutputOptionsWidget());
        }

        public override void LoadConfigData()
        {
            mWidget.Load((NMEProjectConfiguration)CurrentConfiguration);
        }

        public override void ApplyChanges()
        {
            mWidget.Store();
        }
    }

    [System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class NMEOutputOptionsWidget : Gtk.Bin
    {
        NMEProjectConfiguration mConfiguration;

        public NMEOutputOptionsWidget()
        {
            this.Build();
            // FIXME: Why is Show() necessary?
            this.Show();
        }

        public void Load(NMEProjectConfiguration configuration)
        {
            mConfiguration = configuration;
			
			AdditionalArgumentsEntry.Text = configuration.AdditionalArguments;

            //wOutputFileNameEntry.Text = configuration.OutputFileName;
            //wOutputDirectoryEntry.Text = configuration.OutputDirectory;
            //wParametersEntry.Text = configuration.CompilerParameters;
        }

        public void Store()
        {
            if (mConfiguration == null)
                return;
			
			mConfiguration.AdditionalArguments = AdditionalArgumentsEntry.Text.Trim ();
        }

        protected virtual void OnWOutputDirectoryButtonClicked (object sender, System.EventArgs e)
        {
			
        }
    }
}
