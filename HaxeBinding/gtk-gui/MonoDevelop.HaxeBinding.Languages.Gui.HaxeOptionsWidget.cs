
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.HaxeBinding.Languages.Gui
{
	public partial class HaxeOptionsWidget
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.Frame frame1;
		private global::Gtk.Alignment GtkAlignment;
		private global::Gtk.VBox vbox2;
		private global::Gtk.CheckButton EnableCompilationServerCheckBox;
		private global::Gtk.Table table1;
		private global::Gtk.Entry PortNumberEntry;
		private global::Gtk.Label PortNumberLabel;
		private global::Gtk.Label GtkLabel2;
		private global::Gtk.Frame frame2;
		private global::Gtk.Alignment GtkAlignment1;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.HaxeBinding.Languages.Gui.HaxeOptionsWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.HaxeBinding.Languages.Gui.HaxeOptionsWidget";
			// Container child MonoDevelop.HaxeBinding.Languages.Gui.HaxeOptionsWidget.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			this.GtkAlignment.TopPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.EnableCompilationServerCheckBox = new global::Gtk.CheckButton ();
			this.EnableCompilationServerCheckBox.CanFocus = true;
			this.EnableCompilationServerCheckBox.Name = "EnableCompilationServerCheckBox";
			this.EnableCompilationServerCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Enable compilation server");
			this.EnableCompilationServerCheckBox.DrawIndicator = true;
			this.EnableCompilationServerCheckBox.UseUnderline = true;
			this.vbox2.Add (this.EnableCompilationServerCheckBox);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.EnableCompilationServerCheckBox]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.PortNumberEntry = new global::Gtk.Entry ();
			this.PortNumberEntry.CanFocus = true;
			this.PortNumberEntry.Name = "PortNumberEntry";
			this.PortNumberEntry.IsEditable = true;
			this.PortNumberEntry.InvisibleChar = '•';
			this.table1.Add (this.PortNumberEntry);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.PortNumberEntry]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.PortNumberLabel = new global::Gtk.Label ();
			this.PortNumberLabel.Name = "PortNumberLabel";
			this.PortNumberLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Port number");
			this.table1.Add (this.PortNumberLabel);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.PortNumberLabel]));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.table1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.table1]));
			w4.Position = 1;
			this.GtkAlignment.Add (this.vbox2);
			this.frame1.Add (this.GtkAlignment);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Compilation Server</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel2;
			this.vbox1.Add (this.frame1);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frame1]));
			w7.Position = 0;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			this.GtkAlignment1.TopPadding = ((uint)(12));
			this.frame2.Add (this.GtkAlignment1);
			this.vbox1.Add (this.frame2);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frame2]));
			w9.Position = 1;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
