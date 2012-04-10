using System;
using System.IO;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;


namespace MonoDevelop.HaxeBinding.Projects.Gui
{

	public class HaxeProjectOptionsPanel : ItemOptionsPanel
	{
		HaxeProjectOptionsWidget mWidget;


		public override Gtk.Widget CreatePanelWidget ()
		{	
			mWidget = new HaxeProjectOptionsWidget ();
			mWidget.Load ((HaxeProject)ConfiguredProject);
			return mWidget;
		}


		public override void ApplyChanges ()
		{
			mWidget.Store ();
		}
	}

	[System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HaxeProjectOptionsWidget : Gtk.Bin
	{
		HaxeProject mProject;


		public HaxeProjectOptionsWidget ()
		{
			this.Build ();
		}


		public void Load (HaxeProject project)
		{
			mProject = project;
			
			TargetHXMLFileEntry.Text = mProject.TargetHXMLFile;
			AdditionalArgumentsEntry.Text = mProject.AdditionalArguments;
		}


		public void Store ()
		{
			if (mProject == null)
				return;
			
			mProject.TargetHXMLFile = TargetHXMLFileEntry.Text.Trim ();
			mProject.AdditionalArguments = AdditionalArgumentsEntry.Text.Trim ();
		}

		
		protected void OnTargetHXMLFileButtonClicked (object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog ("Target HXML file", this.Toplevel as Gtk.Window, FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
			
			Gtk.FileFilter filterHXML = new Gtk.FileFilter ();
			filterHXML.Name = "HXML Files";
			filterHXML.AddPattern ("*.nmml");
			
			Gtk.FileFilter filterAll = new Gtk.FileFilter ();
			filterAll.Name = "All Files";
			filterAll.AddPattern ("*");
			
			fc.AddFilter (filterHXML);
			fc.AddFilter (filterAll);
			
			if (mProject.TargetHXMLFile != "")
			{
				fc.SetFilename (mProject.TargetHXMLFile);
			}
			else
			{
				fc.SetFilename (mProject.BaseDirectory);
			}

			if (fc.Run () == (int)ResponseType.Accept)
			{
				string path = PathHelper.ToRelativePath (fc.Filename, mProject.BaseDirectory);
				
				TargetHXMLFileEntry.Text = path;
			}

			fc.Destroy ();
		}
		
	}
	
}