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
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;

namespace MonoDevelop.HaxeBinding.Projects.Gui
{
    public class NMEProjectOptionsPanel : ItemOptionsPanel
    {
        NMEProjectOptionsWidget mWidget;

        public override Gtk.Widget CreatePanelWidget()
        {	
            mWidget = new NMEProjectOptionsWidget();
            mWidget.Load((NMEProject)ConfiguredProject);
            return mWidget;
        }

        public override void ApplyChanges()
        {
            mWidget.Store();
        }
    }

    [System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class NMEProjectOptionsWidget : Gtk.Bin
    {
        NMEProject mProject;

        public NMEProjectOptionsWidget()
        {
            this.Build();
        }

        public void Load(NMEProject project)
        {
            mProject = project;
			
			TargetNMMLFileEntry.Text = mProject.TargetNMMLFile;
			AdditionalArgumentsEntry.Text = mProject.AdditionalArguments;
			
			/*
            switch (mProject.TargetFormat)
            {
                case TargetFormat.SWF: wTargetCombo.Active = 0; break;
            }

            switch (mProject.MainLanguage)
            {
                case Language.Haxe : wLanguageCombo.Active = 0; break;
            }

            switch (mProject.RunMode)
            {
                case RunMode.StandalonePlayer : wRunCombo.Active = 0; break;
                case RunMode.SystemPlugin : wRunCombo.Active = 1; break;
                case RunMode.CustomPlugin : wRunCombo.Active = 2; break;
            }

            wMainSourceEntry.Text = mProject.MainSource;*/
        }

        public void Store()
        {
            if (mProject == null)
                return;
			
			mProject.TargetNMMLFile = TargetNMMLFileEntry.Text.Trim ();
			mProject.AdditionalArguments = AdditionalArgumentsEntry.Text.Trim ();
			
			/*
            switch (wTargetCombo.Active)
            {
                case 0: mProject.TargetFormat = TargetFormat.SWF ; break;
            }

            switch (wRunCombo.Active)
            {
                case 0: mProject.RunMode = RunMode.StandalonePlayer ; break;
                case 1: mProject.RunMode = RunMode.SystemPlugin ; break;
                case 2: mProject.RunMode = RunMode.CustomPlugin ; break;
            }

            if (!String.IsNullOrEmpty(wMainSourceEntry.Text))
                mProject.MainSource = wMainSourceEntry.Text;

            // FIXME: Why the updated FlexProjectCOnfiguration configuration is not saved?
            foreach (NMEProjectConfiguration configuration in mProject.Configurations) {
                configuration.AdjustOutputFileName(mProject.TargetFormat);
            }*/
        }
		
		protected void OnTargetNMMLFileButtonClicked (object sender, System.EventArgs e)
		{
            Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog("Target NMML file", this.Toplevel as Gtk.Window, FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
			
			Gtk.FileFilter filter = new Gtk.FileFilter ();
			filter.Name = "NMML files";
			filter.AddPattern ("*.nmml");
        	fc.AddFilter(filter);
			
			if (mProject.TargetNMMLFile != "") {
				
				fc.SetFilename(mProject.TargetNMMLFile);
				
			} else {
				
				fc.SetFilename(mProject.BaseDirectory);
				
			}

            if (fc.Run() == (int)ResponseType.Accept) {
				
				string path = PathHelper.ToRelativePath (fc.Filename, mProject.BaseDirectory);
				
				TargetNMMLFileEntry.Text = path;
            }

            fc.Destroy();
		}
    }
}
