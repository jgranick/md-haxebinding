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
    public class HaxeOutputOptionsPanel : MultiConfigItemOptionsPanel
    {
        private HaxeOutputOptionsWidget mWidget;

        public override Gtk.Widget CreatePanelWidget()
        {
            return (mWidget = new HaxeOutputOptionsWidget());
        }

        public override void LoadConfigData()
        {
            mWidget.Load((HaxeProjectConfiguration)CurrentConfiguration);
        }

        public override void ApplyChanges()
        {
            mWidget.Store();
        }
    }

    [System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HaxeOutputOptionsWidget : Gtk.Bin
    {
        HaxeProjectConfiguration mConfiguration;

        public HaxeOutputOptionsWidget()
        {
            this.Build();
            // FIXME: Why is Show() necessary?
            this.Show();
        }

        public void Load(HaxeProjectConfiguration configuration)
        {
            mConfiguration = configuration;

            wOutputFileNameEntry.Text = configuration.OutputFileName;
            wOutputDirectoryEntry.Text = configuration.OutputDirectory;
            wParametersEntry.Text = configuration.CompilerParameters;
        }

        public void Store()
        {
            if (mConfiguration == null)
                return;

            if (wOutputFileNameEntry != null && wOutputFileNameEntry.Text.Length > 0)
                mConfiguration.OutputFileName = wOutputFileNameEntry.Text.Trim();

            if (wOutputDirectoryEntry != null && wOutputDirectoryEntry.Text.Length > 0)
                mConfiguration.OutputDirectory = wOutputDirectoryEntry.Text.Trim();

            if (wParametersEntry != null && wParametersEntry.Text.Length > 0)
                mConfiguration.CompilerParameters = wParametersEntry.Text.Trim();
        }

        protected virtual void OnWOutputDirectoryButtonClicked (object sender, System.EventArgs e)
        {
            Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog("Output Path", this.Toplevel as Gtk.Window, FileChooserAction.SelectFolder,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);

            if (fc.Run() == (int)ResponseType.Accept) {
                wOutputDirectoryEntry.Text = fc.Filename;
            }

            fc.Destroy();
        }
    }
}
