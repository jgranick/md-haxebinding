using System;
using System.IO;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;

using MonoDevelop.HaxeBinding.Projects;


// This file was taken from the old "FlexBinding" add-in and has not been adapted yet


namespace MonoDevelop.HaxeBinding.Projects.Gui
{
    public class HaxeProjectOptionsPanel : ItemOptionsPanel
    {
        HaxeProjectOptionsWidget mWidget;

        public override Gtk.Widget CreatePanelWidget()
        {
            mWidget = new HaxeProjectOptionsWidget();
            mWidget.Load((HaxeProject)ConfiguredProject);
            return mWidget;
        }

        public override void ApplyChanges()
        {
            mWidget.Store();
        }
    }

    [System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HaxeProjectOptionsWidget : Gtk.Bin
    {
        HaxeProject mProject;

        public HaxeProjectOptionsWidget()
        {
            this.Build();
        }

        public void Load(HaxeProject project)
        {
            mProject = project;

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

            wMainSourceEntry.Text = mProject.MainSource;
        }

        public void Store()
        {
            if (mProject == null)
                return;

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
            foreach (HaxeProjectConfiguration configuration in mProject.Configurations) {
                configuration.AdjustOutputFileName(mProject.TargetFormat);
            }
        }

        protected virtual void OnWMainSourceButtonClicked (object sender, System.EventArgs e)
        {
            Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog("Main source file", this.Toplevel as Gtk.Window, FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
            fc.SetFilename(mProject.MainSource);

            if (fc.Run() == (int)ResponseType.Accept) {
                wMainSourceEntry.Text = fc.Filename;

                string ext = System.IO.Path.GetExtension(fc.Filename.ToLower());
                switch (ext)
                {
                    case ".mxml": wLanguageCombo.Active = 0; break;
                    case ".as": wLanguageCombo.Active = 1; break;
                }
            }

            fc.Destroy();
        }
    }
}
