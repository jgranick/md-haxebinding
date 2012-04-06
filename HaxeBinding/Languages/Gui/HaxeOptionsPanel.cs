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

    [System.ComponentModel.Category("HaxeContext")]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HaxeOptionsWidget : Gtk.Bin
    {
        public HaxeOptionsWidget()
        {
            this.Build();

            wSdkPathEntry.Text = PropertyService.Get<string>("CBinding.FlexSdkPath");
            wPlayerPathEntry.Text = PropertyService.Get<string>("CBinding.FlexPlayerPath");
            wBrowserPathEntry.Text = PropertyService.Get<string>("CBinding.FlexBrowserPath");
        }

        public bool Store()
        {
            PropertyService.Set("CBinding.FlexSdkPath", wSdkPathEntry.Text);
            PropertyService.Set("CBinding.FlexPlayerPath", wPlayerPathEntry.Text);
            PropertyService.Set("CBinding.FlexBrowserPath", wBrowserPathEntry.Text);
            PropertyService.SaveProperties();
            return true;
        }

        protected virtual void OnWSdkPathButtonClicked (object sender, System.EventArgs e)
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
        }
    }
}
