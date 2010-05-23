
using System;
using Gtk;


namespace Dialogs
{

    public class Information : Dialog
    {
        
        private Label nickLabel;
        private Label nickEntry;
        private HBox  nickBox;
        
        private Label idLabel;
        private Label idEntry;
        private HBox  idBox;
        
        private Label versionLabel;
        private Label versionEntry;
        private HBox  versionBox;
        
        private Button okBut;
        
        
        public Information ( string title ) : base ()
        {

            this.Title = title;
            this.HasSeparator = false;
            this.Resizable = false;
            this.SkipTaskbarHint = true;
            this.IconList = MainWindow.appIcons;
            this.BorderWidth = 6;
            this.ActionArea.Destroy ();
            this.DeleteEvent += OnDeleteEvent;
            
            
            Image img = new Image ( Stock.DialogInfo, IconSize.Dialog );
            img.Yalign = 0;
            img.Xpad = 3;
            
            okBut = new Button ( Stock.Close );
            okBut.Clicked += Dismiss;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( okBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            
            nickEntry = new Label ();
            nickEntry.Xalign = 0;
            nickEntry.WidthChars = 20;
            nickEntry.Selectable = true;
            
            nickLabel = new Label ( TextStrings.nick + ":  " );
            nickLabel.Xalign = 0;
            
            nickBox = new HBox ();
            nickBox.Add ( nickLabel );
            nickBox.Add ( nickEntry );
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( nickBox [ nickEntry ] ) );
            bc8.Expand = false;
            
            
            idEntry = new Label ();
            idEntry.Xalign = 0;
            idEntry.WidthChars = 20;
            idEntry.Selectable = true;
            
            idLabel = new Label ( TextStrings.address + ":  " );
            idLabel.Xalign = 0;
            
            idBox = new HBox ();
            idBox.Add ( idLabel );
            idBox.Add ( idEntry );
            
            Box.BoxChild bc9 = ( ( Box.BoxChild ) ( idBox [ idEntry ] ) );
            bc9.Expand = false;
            
            
            versionEntry = new Label ();
            versionEntry.Xalign = 0;
            versionEntry.WidthChars = 20;
            versionEntry.Selectable = true;
            
            versionLabel = new Label ( TextStrings.hamachiVersion + ":  " );
            versionLabel.Xalign = 0;
            
            versionBox = new HBox ();
            versionBox.Add ( versionLabel );
            versionBox.Add ( versionEntry );
            
            Box.BoxChild bc10 = ( ( Box.BoxChild ) ( versionBox [ versionEntry ] ) );
            bc10.Expand = false;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( versionBox );
            vbox.Add ( idBox );
            vbox.Add ( nickBox );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbox [ idBox ] ) );
            bc6.Padding = 6;
            bc6.Expand = false;
            
            Box.BoxChild bc12 = ( ( Box.BoxChild ) ( vbox [ nickBox ] ) );
            bc12.Padding = 6;
            bc12.Expand = false;
            
            Box.BoxChild bc13 = ( ( Box.BoxChild ) ( vbox [ versionBox ] ) );
            bc13.Padding = 6;
            bc13.Expand = false;
            

            HBox Contents = new HBox ();
            Contents.Add ( vbox );

            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( Contents [ vbox ] ) );
            bc7.Padding = 6;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( img );
            hbox.Add ( Contents );
           
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( hbox [ img ] ) );
            bc1.Expand = false;
            
            
            this.VBox.Add ( hbox );
            this.ShowAll ();
            this.Hide ();
            
            SetIdentity ();
            SetVersion ();
            
        }
        
        
        private void OnDeleteEvent (object obj, DeleteEventArgs args )
        {
            Dismiss ();
            args.RetVal = true;
        }
        
        
        public void Open ()
        {
            if ( this.Visible )
            {
                this.Present ();
            }
            else
            {
                this.Visible = true;
                this.Show ();
            }
        }


        private void Dismiss ()
        {
            this.Visible = false;
            this.Hide ();
        }
        
        
        private void Dismiss ( object obj, EventArgs args )
        {
            Dismiss ();
        }
        
        
        public void SetNick ( string nick )
        {
            
            if ( nick == "" )
            {
                nick = "<i>" + TextStrings.unavailable + "</i>";
            }
            
            nickEntry.Markup = nick;
            
        }
        
        
        public void SetIdentity ()
        {
            
            string id = Hamachi.GetIdentity ();
            
            if ( id == "" )
            {
                id = "<i>" + TextStrings.unavailable + "</i>";
            }
            
            idEntry.Markup = id;
            
        }
        
        
        public void SetVersion ()
        {
            
            string version = Hamachi.GetVersion ();
            
            if ( version == "" )
            {
                version = "<i>" + TextStrings.unavailable + "</i>";
            }
            
            versionEntry.Markup = version;
            
        }
        
    }

}
