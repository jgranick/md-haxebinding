using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;


// This file was taken from the old "FlexBinding" add-in and has not been adapted yet


namespace MonoDevelop.HaxeBinding.Projects.Gui
{
	public class HaxeOutputOptionsPanel : MultiConfigItemOptionsPanel
	{
		private HaxeOutputOptionsWidget mWidget;

		public override Gtk.Widget CreatePanelWidget ()
		{
			return (mWidget = new HaxeOutputOptionsWidget ());
		}

		public override void LoadConfigData ()
		{
			mWidget.Load ((HaxeProjectConfiguration)CurrentConfiguration);
		}

		public override void ApplyChanges ()
		{
			mWidget.Store ();
		}
	}

	[System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HaxeOutputOptionsWidget : Gtk.Bin
	{
		HaxeProjectConfiguration mConfiguration;

		public HaxeOutputOptionsWidget ()
		{
			this.Build ();
			// FIXME: Why is Show() necessary?
			this.Show ();
		}

		public void Load (HaxeProjectConfiguration configuration)
		{
			mConfiguration = configuration;

			wOutputFileNameEntry.Text = configuration.OutputFileName;
			wOutputDirectoryEntry.Text = configuration.OutputDirectory;
			wParametersEntry.Text = configuration.CompilerParameters;
		}

		public void Store ()
		{
			if (mConfiguration == null)
				return;

			if (wOutputFileNameEntry != null && wOutputFileNameEntry.Text.Length > 0)
				mConfiguration.OutputFileName = wOutputFileNameEntry.Text.Trim ();

			if (wOutputDirectoryEntry != null && wOutputDirectoryEntry.Text.Length > 0)
				mConfiguration.OutputDirectory = wOutputDirectoryEntry.Text.Trim ();

			if (wParametersEntry != null && wParametersEntry.Text.Length > 0)
				mConfiguration.CompilerParameters = wParametersEntry.Text.Trim ();
		}

		protected virtual void OnWOutputDirectoryButtonClicked (object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog ("Output Path", this.Toplevel as Gtk.Window, FileChooserAction.SelectFolder,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);

			if (fc.Run () == (int)ResponseType.Accept) {
				wOutputDirectoryEntry.Text = fc.Filename;
			}

			fc.Destroy ();
		}
	}
}
