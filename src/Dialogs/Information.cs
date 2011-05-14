/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2011 Stephen Brandt <stephen@stephenbrandt.com>
 * 
 * Haguichi is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 * 
 * Haguichi is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Haguichi; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using Gtk;
using GLib;


namespace Dialogs
{

    public class Information : Dialog
    {
        
        private Image image;
        
        private Button closeButton;
        
        private Label versionLabel;
        private Label versionEntry;
        
        private Label addressLabel;
        private Label addressEntry;
        
        private Label idLabel;
        private Label idEntry;
        
        private Label accountLabel;
        private Label accountEntry;
        
        private Label nickLabel;
        private Label nickEntry;
        
        
        public Information ( string title ) : base ()
        {

            this.Title           = title;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.SkipTaskbarHint = true;
            this.TransientFor    = Haguichi.mainWindow.ReturnWindow ();
            this.IconList        = MainWindow.appIcons;
            this.BorderWidth     = 3;
            this.DeleteEvent    += OnDeleteEvent;
            
            this.ActionArea.Destroy ();
            
            
            image = new Image ( Stock.DialogInfo, IconSize.Dialog );
            image.Yalign = 0;
            image.Ypad = 4;
            
            
            closeButton = new Button ( Stock.Close );
            closeButton.Clicked += Dismiss;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( closeButton );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            
            versionEntry = new Label ();
            versionEntry.Xalign = 0;
            versionEntry.Xpad = 4;
            versionEntry.SetSizeRequest ( 160, 0 ); /* Triggers minimum dialog size */
            versionEntry.Selectable = true;
            
            versionLabel = new Label ( TextStrings.version + " " );
            versionLabel.Xalign = 0;
            versionLabel.Ypad = 4;
            
            
            addressEntry = new Label ();
            addressEntry.Xalign = 0;
            addressEntry.Xpad = 4;
            addressEntry.Selectable = true;
            
            addressLabel = new Label ( TextStrings.address + " " );
            addressLabel.Xalign = 0;
            addressLabel.Ypad = 4;
            
            
            idEntry = new Label ();
            idEntry.Xalign = 0;
            idEntry.Xpad = 4;
            idEntry.Selectable = true;
            
            idLabel = new Label ( TextStrings.id + " " );
            idLabel.Xalign = 0;
            idLabel.Ypad = 4;
            
            
            accountEntry = new Label ();
            accountEntry.Xalign = 0;
            accountEntry.Xpad = 4;
            accountEntry.Selectable = true;
            
            accountLabel = new Label ( TextStrings.account + " " );
            accountLabel.Xalign = 0;
            accountLabel.Ypad = 4;
            
            
            nickEntry = new Label ();
            nickEntry.Xalign = 0;
            nickEntry.Xpad = 4;
            nickEntry.Selectable = true;
            
            nickLabel = new Label ( TextStrings.nick + " " );
            nickLabel.Xalign = 0;
            nickLabel.Ypad = 4;
            
            
            Table table = new Table ( 6, 2, false );
            table.Attach ( versionLabel, 0, 1, 0, 1 );
            table.Attach ( versionEntry, 1, 2, 0, 1 );
            table.Attach ( addressLabel, 0, 1, 1, 2 );
            table.Attach ( addressEntry, 1, 2, 1, 2 );
            table.Attach ( idLabel, 0, 1, 2, 3 );
            table.Attach ( idEntry, 1, 2, 2, 3 );
            table.Attach ( accountLabel , 0, 1, 3, 4 );
            table.Attach ( accountEntry , 1, 2, 3, 4 );
            table.Attach ( nickLabel , 0, 1, 4, 5 );
            table.Attach ( nickEntry , 1, 2, 4, 5 );
            
            
            HBox hbox = new HBox ();
            hbox.Add ( image );
            hbox.Add ( table );
           
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( hbox [ image ] ) );
            bc3.Padding = 9;
            bc3.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( hbox [ table ] ) );
            bc4.Padding = 3;
            
            
            HBox hbox2 = new HBox ();
            hbox2.Add ( buttonBox );
            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( hbox2 [ buttonBox ] ) );
            bc7.Padding = 6;
            
            
            this.VBox.Add ( hbox );
            this.VBox.Add ( hbox2 );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( this.VBox [ hbox ] ) );
            bc5.Padding = 3;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( this.VBox [ hbox2 ] ) );
            bc6.Padding = 6;
            
            this.VBox.ShowAll ();
            
            
            closeButton.CanDefault = true;
            closeButton.GrabDefault ();
            
            SetAddress ();
            SetClientId ();
            SetVersion ();
            SetAccount ( "" );
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        public void Open ()
        {
            
            this.Show ();
            this.Present ();
            
        }


        private void Dismiss ()
        {
            
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
                idLabel.Hide ();
                idEntry.Hide ();
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
                accountLabel.Hide ();
                accountEntry.Hide ();
            }
            
        }
        
        
        public void SetNick ( string nick )
        {
            
            nick = Markup.EscapeText ( nick );
            
            if ( nick == "" )
            {
                nick = "<i>" + TextStrings.anonymous + "</i>";
            }
            
            nickEntry.Markup = nick;
            
        }
        
    }

}
