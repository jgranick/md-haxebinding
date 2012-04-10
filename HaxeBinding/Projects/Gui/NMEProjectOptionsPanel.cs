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


		public override Gtk.Widget CreatePanelWidget ()
		{	
			mWidget = new NMEProjectOptionsWidget ();
			mWidget.Load ((NMEProject)ConfiguredProject);
			return mWidget;
		}


		public override void ApplyChanges ()
		{
			mWidget.Store ();
		}
	}

	[System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class NMEProjectOptionsWidget : Gtk.Bin
	{
		NMEProject mProject;


		public NMEProjectOptionsWidget ()
		{
			this.Build ();
		}


		public void Load (NMEProject project)
		{
			mProject = project;
			
			TargetNMMLFileEntry.Text = mProject.TargetNMMLFile;
			AdditionalArgumentsEntry.Text = mProject.AdditionalArguments;
		}


		public void Store ()
		{
			if (mProject == null)
				return;
			
			mProject.TargetNMMLFile = TargetNMMLFileEntry.Text.Trim ();
			mProject.AdditionalArguments = AdditionalArgumentsEntry.Text.Trim ();
		}

		
		protected void OnTargetNMMLFileButtonClicked (object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog ("Target NMML file", this.Toplevel as Gtk.Window, FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
			
			Gtk.FileFilter filterNMML = new Gtk.FileFilter ();
			filterNMML.Name = "NMML Files";
			filterNMML.AddPattern ("*.nmml");
			
			Gtk.FileFilter filterAll = new Gtk.FileFilter ();
			filterAll.Name = "All Files";
			filterAll.AddPattern ("*");
			
			fc.AddFilter (filterNMML);
			fc.AddFilter (filterAll);
			
			if (mProject.TargetNMMLFile != "")
			{
				fc.SetFilename (mProject.TargetNMMLFile);
			}
			else
			{
				fc.SetFilename (mProject.BaseDirectory);
			}

			if (fc.Run () == (int)ResponseType.Accept)
			{
				string path = PathHelper.ToRelativePath (fc.Filename, mProject.BaseDirectory);
				
				TargetNMMLFileEntry.Text = path;
			}

			fc.Destroy ();
		}
		
	}
	
}