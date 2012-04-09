using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;


namespace MonoDevelop.HaxeBinding.Projects.Gui
{

	public class NMEOutputOptionsPanel : MultiConfigItemOptionsPanel
	{
		private NMEOutputOptionsWidget mWidget;


		public override Gtk.Widget CreatePanelWidget ()
		{
			return (mWidget = new NMEOutputOptionsWidget ());
		}


		public override void LoadConfigData ()
		{
			mWidget.Load ((NMEProjectConfiguration)CurrentConfiguration);
		}


		public override void ApplyChanges ()
		{
			mWidget.Store ();
		}
	}

	[System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class NMEOutputOptionsWidget : Gtk.Bin
	{
		NMEProjectConfiguration mConfiguration;


		public NMEOutputOptionsWidget ()
		{
			this.Build ();
			this.Show ();
		}


		public void Load (NMEProjectConfiguration configuration)
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