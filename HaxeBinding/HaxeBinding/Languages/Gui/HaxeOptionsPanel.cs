using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;


// This file was taken from the old "FlexBinding" add-in and has not been adapted yet


namespace MonoDevelop.HaxeBinding.Languages.Gui
{
    public class HaxeOptionsPanel : OptionsPanel
    {
        HaxeOptionsWidget mWidget;

        public override Gtk.Widget CreatePanelWidget()
        {
            return mWidget = new HaxeOptionsWidget();
        }

        public override void ApplyChanges()
        {
            mWidget.Store();
        }
    }

    [System.ComponentModel.Category("HaxeBinding")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HaxeOptionsWidget : Gtk.Bin
    {
        public HaxeOptionsWidget()
        {
            this.Build();
			
			if (!PropertyService.HasValue ("HaxeBinding.EnableCompilationServer"))
			{
				EnableCompilationServerCheckBox.Active = true;
			}
			else
			{
				EnableCompilationServerCheckBox.Active = PropertyService.Get<bool> ("HaxeBinding.EnableCompilationServer");
			}
			
			if (!PropertyService.HasValue ("HaxeBinding.CompilationServerPort"))
			{
				PortNumberEntry.Text = "6000";
			}
			else
			{
				PortNumberEntry.Text = PropertyService.Get<int> ("HaxeBinding.CompilationServerPort").ToString ();
			}
        }

        public bool Store()
        {
			PropertyService.Set ("HaxeBinding.EnableCompilationServer", EnableCompilationServerCheckBox.Active);
			PropertyService.Set ("HaxeBinding.CompilationServerPort", Convert.ToInt32 (PortNumberEntry.Text));
            PropertyService.SaveProperties();
            return true;
        }

        /*protected virtual void OnWSdkPathButtonClicked (object sender, System.EventArgs e)
        {
            Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog("Flex SDK Path", this.Toplevel as Gtk.Window, FileChooserAction.SelectFolder,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
            if (!String.IsNullOrEmpty(wSdkPathEntry.Text))
                fc.SetFilename(wSdkPathEntry.Text);

            if (fc.Run() == (int)ResponseType.Accept) {
                wSdkPathEntry.Text = fc.Filename;
            }

            fc.Destroy();
        }

        protected virtual void OnWPlayerPathButtonClicked (object sender, System.EventArgs e)
        {
            Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog("Standalone Player Path", this.Toplevel as Gtk.Window, FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
            if (!String.IsNullOrEmpty(wSdkPathEntry.Text))
                fc.SetFilename(wPlayerPathEntry.Text);

            if (fc.Run() == (int)ResponseType.Accept) {
                wPlayerPathEntry.Text = fc.Filename;
            }

            fc.Destroy();
        }

        protected virtual void OnWBrowserPathButtonClicked (object sender, System.EventArgs e)
        {
            Gtk.FileChooserDialog fc =
                new Gtk.FileChooserDialog("Browser Path", this.Toplevel as Gtk.Window, FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);
            if (!String.IsNullOrEmpty(wBrowserPathEntry.Text))
                fc.SetFilename(wBrowserPathEntry.Text);

            if (fc.Run() == (int)ResponseType.Accept) {
                wBrowserPathEntry.Text = fc.Filename;
            }

            fc.Destroy();
        }*/
    }
}
