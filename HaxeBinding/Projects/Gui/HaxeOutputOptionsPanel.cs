using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;


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
			this.Show ();
		}


		public void Load (HaxeProjectConfiguration configuration)
		{
			mConfiguration = configuration;
			
			AdditionalArgumentsEntry.Text = configuration.AdditionalArguments;
		}


		public void Store ()
		{
			if (mConfiguration == null)
				return;
			
			mConfiguration.AdditionalArguments = AdditionalArgumentsEntry.Text.Trim ();
		}
		
	}
	
}