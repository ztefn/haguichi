/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2010 Stephen Brandt <stephen@stephenbrandt.com>
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


namespace Dialogs
{

    public class Attach : Dialog
    {
        
        private Label heading;
        
        private Label idLabel;
        private Entry idEntry;
        private HBox  idBox;
        
        private CheckButton withNetworks;
        
        private Button cancelBut;
        private Button attachBut;
        
        
        public Attach () : base ()
        {
            
            this.Title = TextStrings.attachTitle;
            this.Modal = true;
            this.HasSeparator = false;
            this.Resizable = false;
            this.SkipTaskbarHint = true;
            this.IconList = MainWindow.appIcons;
            this.BorderWidth = 6;
            this.ActionArea.Destroy ();
            this.DeleteEvent += OnDeleteEvent;
            
            
            heading = new Label ();
            heading.Markup = String.Format ( TextStrings.attachMessage );
            heading.Xalign = 0;
            heading.Ypad = 6;

            HBox headBox = new HBox ();
            headBox.Add ( heading );
            
            
            cancelBut = new Button ( Stock.Cancel );
            cancelBut.Clicked += Dismiss;
            
            attachBut = new Button ( Stock.Edit ); // Using an stock button to trigger underscore interpretation within the label text, seems a GTK bug or misdesign
            attachBut.CanDefault = true;
            attachBut.Label = TextStrings.attachButtonLabel;
            attachBut.Clicked += GoAttach;
            attachBut.Sensitive = false;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( cancelBut );
            buttonBox.Add ( attachBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            idEntry = new Entry ();
            idEntry.ActivatesDefault = true;
            idEntry.MaxLength = 100;
            idEntry.Changed += CheckEntryLength;
            
            idLabel = new Label ( TextStrings.accountLabel + "  " );
            idLabel.Xalign = 0;
            idLabel.MnemonicWidget = idEntry;

            
            idBox = new HBox ();
            idBox.Add ( idLabel );
            idBox.Add ( idEntry );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( idBox [ idLabel ] ) );
            bc1.Expand = false;
            
            withNetworks = new CheckButton ( TextStrings.attachWithNetworksCheckbox );
            
            
            VBox vbox = new VBox ();
            vbox.Add ( headBox );
            vbox.Add ( idBox );
            vbox.Add ( withNetworks );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ headBox ] ) );
            bc8.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbox [ idBox ] ) );
            bc6.Padding = 6;
            bc6.Expand = false;
            
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( vbox [ withNetworks ] ) );
            bc3.Padding = 3;
            bc3.Expand = false;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( vbox );

            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc7.Padding = 6;
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( headBox [ heading ] ) );
            bc2.Expand = false;
            
            
            this.VBox.Add ( hbox );
            this.ShowAll ();
            
            idEntry.GrabFocus ();
            
        }
        
        
        private void OnDeleteEvent (object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        private void GoAttach ( object obj, EventArgs args )
        {
            
            string output;
            
            if ( Config.Settings.DemoMode )
            {
                output = ".. failed, not found";
            }
            else
            {
                output = Hamachi.Attach ( idEntry.GetChars ( 0, -1 ), withNetworks.Active );
            }
            
            if ( output.IndexOf ( ".. ok" ) != -1 )
            {
                GlobalEvents.SetAttach ();
                
                Dismiss ();
                return;
            }
            else if ( output.IndexOf ( ".. failed, not found" ) != -1 )
            {
                Dialogs.Message msgDlg = new Dialogs.Message ( TextStrings.attachErrorHeading, TextStrings.attachErrorAccountNotFound, "Error" );
                return;
            }
            else if ( output.IndexOf ( ".. failed" ) != -1 )
            {
                Dialogs.Message msgDlg = new Dialogs.Message ( TextStrings.attachErrorHeading, TextStrings.errorUnknown, "Error" );
                return;
            }
            else
            {
                // Unknown output
            }
            
        }
        
        
        private void CheckEntryLength ( object obj, EventArgs args )
        {
            
            string id = idEntry.GetChars ( 0, -1 );
            
            if ( id.Length > 0 )
            {
                attachBut.Sensitive = true;
                attachBut.GrabDefault ();
            }
            else
            {
                attachBut.Sensitive = false;
            }
            
        }
        
        
        private void Dismiss ()
        {
            
            this.Destroy ();
            
        }
        
        
        private void Dismiss ( object obj, EventArgs args )
        {
            
            Dismiss ();
            
        }
        
    }

}
