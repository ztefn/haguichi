
using System;
using Gtk;


namespace Dialogs
{

    public class Information : Dialog
    {
        
        private Label nickLabel;
        private Label nickEntry;
        private HBox  nickBox;
        
        private Label accountLabel;
        private Label accountEntry;
        private HBox  accountBox;
        
        private Label addressLabel;
        private Label addressEntry;
        private HBox  addressBox;
        
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
            nickEntry.WidthChars = 25;
            nickEntry.Selectable = true;
            
            nickLabel = new Label ( TextStrings.nick + "  " );
            nickLabel.Xalign = 0;
            
            nickBox = new HBox ();
            nickBox.Add ( nickLabel );
            nickBox.Add ( nickEntry );
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( nickBox [ nickEntry ] ) );
            bc8.Expand = false;
            
            
            accountEntry = new Label ();
            accountEntry.Xalign = 0;
            accountEntry.WidthChars = 25;
            accountEntry.Selectable = true;
            
            accountLabel = new Label ( TextStrings.account + "  " );
            accountLabel.Xalign = 0;
            
            accountBox = new HBox ();
            accountBox.Add ( accountLabel );
            accountBox.Add ( accountEntry );
            
            Box.BoxChild bc17 = ( ( Box.BoxChild ) ( accountBox [ accountEntry ] ) );
            bc17.Expand = false;
            
            
            addressEntry = new Label ();
            addressEntry.Xalign = 0;
            addressEntry.WidthChars = 25;
            addressEntry.Selectable = true;
            
            addressLabel = new Label ( TextStrings.address + "  " );
            addressLabel.Xalign = 0;
            
            addressBox = new HBox ();
            addressBox.Add ( addressLabel );
            addressBox.Add ( addressEntry );
            
            Box.BoxChild bc9 = ( ( Box.BoxChild ) ( addressBox [ addressEntry ] ) );
            bc9.Expand = false;
            
            
            idEntry = new Label ();
            idEntry.Xalign = 0;
            idEntry.WidthChars = 25;
            idEntry.Selectable = true;
            
            idLabel = new Label ( TextStrings.id + "  " );
            idLabel.Xalign = 0;
            
            idBox = new HBox ();
            idBox.Add ( idLabel );
            idBox.Add ( idEntry );
            
            Box.BoxChild bc15 = ( ( Box.BoxChild ) ( idBox [ idEntry ] ) );
            bc15.Expand = false;
            
            
            versionEntry = new Label ();
            versionEntry.Xalign = 0;
            versionEntry.WidthChars = 25;
            versionEntry.Selectable = true;
            
            versionLabel = new Label ( TextStrings.version + "  " );
            versionLabel.Xalign = 0;
            
            versionBox = new HBox ();
            versionBox.Add ( versionLabel );
            versionBox.Add ( versionEntry );
            
            Box.BoxChild bc10 = ( ( Box.BoxChild ) ( versionBox [ versionEntry ] ) );
            bc10.Expand = false;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( versionBox );
            vbox.Add ( addressBox );
            vbox.Add ( idBox );
            vbox.Add ( accountBox );
            vbox.Add ( nickBox );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbox [ addressBox ] ) );
            bc6.Padding = 6;
            bc6.Expand = false;
            
            Box.BoxChild bc14 = ( ( Box.BoxChild ) ( vbox [ idBox ] ) );
            bc14.Padding = 6;
            bc14.Expand = false;
            
            Box.BoxChild bc12 = ( ( Box.BoxChild ) ( vbox [ nickBox ] ) );
            bc12.Padding = 6;
            bc12.Expand = false;
            
            Box.BoxChild bc16 = ( ( Box.BoxChild ) ( vbox [ accountBox ] ) );
            bc16.Padding = 6;
            bc16.Expand = false;
            
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
            this.VBox.ShowAll ();
            
            SetAddress ();
            SetClientId ();
            SetVersion ();
            SetAccount ( "" );
            
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
        
        
        public void SetVersion ()
        {
            
            string version = Hamachi.GetVersion ();
            
            if ( version == "" )
            {
                version = "<i>" + TextStrings.unavailable + "</i>";
            }
            
            versionEntry.Markup = version;
            
        }
        
        
        public void SetAddress ()
        {
            
            string address = Hamachi.GetAddress ();
            
            if ( address == "" )
            {
                address = "<i>" + TextStrings.unavailable + "</i>";
            }
            
            addressEntry.Markup = address;
            
        }
        
        
        public void SetClientId ()
        {
            
            if ( Hamachi.ApiVersion > 1 )
            {
                string id = Hamachi.GetClientId ();
                
                if ( id == "" )
                {
                    id = "<i>" + TextStrings.unavailable + "</i>";
                }
                
                idEntry.Markup = id;
            }
            else
            {
                idBox.Hide ();
            }
            
        }
        
        
        public void SetAccount ( string account )
        {
            
            if ( Hamachi.ApiVersion > 1 )
            {
                if ( account == "" )
                {
                    account = "<i>" + TextStrings.unavailable + "</i>";
                }
                
                accountEntry.Markup = account;
            }
            else
            {
                accountBox.Hide ();
            }
            
        }
        
        
        public void SetNick ( string nick )
        {
            
            if ( nick == "" )
            {
                nick = "<i>" + TextStrings.unavailable + "</i>";
            }
            
            nickEntry.Markup = nick;
            
        }
        
    }

}
