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
using System.Threading;
using Gtk;
using Widgets;


namespace Dialogs
{

    public class Attach : Dialog
    {
        
        private MessageBar messageBar;
        
        private Label heading;
        
        private Label idLabel;
        private Entry idEntry;
        private HBox  idBox;
        
        private CheckButton withNetworks;
        
        private Button cancelBut;
        private Button attachBut;
        
        
        public Attach () : base ()
        {
            
            GlobalEvents.SetModalDialog ( this );
            
            this.Title           = TextStrings.attachTitle;
            this.TransientFor    = Haguichi.mainWindow.ReturnWindow ();
            this.IconList        = MainWindow.appIcons;
            this.Modal           = true;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.SkipTaskbarHint = true;
            this.DeleteEvent    += OnDeleteEvent;
            
            this.ActionArea.Destroy ();
            
            
            messageBar = new MessageBar ();
            
            heading = new Label ();
            heading.Markup = String.Format ( TextStrings.attachMessage );
            heading.Xalign = 0;
            
            
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
            idEntry.Changed += HideMessage;
            
            idLabel = new Label ( TextStrings.accountLabel + "  " );
            idLabel.Xalign = 0;
            idLabel.MnemonicWidget = idEntry;

            
            idBox = new HBox ();
            idBox.Add ( idLabel );
            idBox.Add ( idEntry );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( idBox [ idLabel ] ) );
            bc1.Expand = false;
            
            withNetworks = new CheckButton ( TextStrings.attachWithNetworksCheckbox );
            withNetworks.Active = true;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( heading );
            vbox.Add ( idBox );
            vbox.Add ( withNetworks );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ heading ] ) );
            bc8.Padding = 6;
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
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc2.Padding = 12;
            
            
            VBox container = new VBox ();
            container.Add ( messageBar );
            container.Add ( hbox );
            container.ShowAll ();

            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( container [ hbox ] ) );
            bc7.Padding = 6;
            
            
            this.Remove ( this.VBox );
            this.Add ( container );
            
            HideMessage ();
            idEntry.GrabFocus ();
            
            this.Show ();
            
        }
        
        
        private void GoAttach ( object obj, EventArgs args )
        {
            
            SetMode ( "Attaching" );
            
            Thread thread = new Thread ( GoAttachThread );
            thread.Start ();
            
        }
        
        
        private void GoAttachThread ()
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
                Application.Invoke ( delegate
                {
                    Dismiss ();
                });
                
                Thread.Sleep ( 2000 ); // Wait a couple of seconds to get an updated account
                
                Application.Invoke ( delegate
                {
                    GlobalEvents.SetAttach ();
                });
                return;
            }
            else if ( ( output.IndexOf ( ".. failed, not found" ) != -1 ) ||
                      ( output.IndexOf ( ".. failed, [248]" ) != -1 ) )
            {
                ShowMessage ( TextStrings.attachErrorAccountNotFound );
                return;
            }
            else if ( output.IndexOf ( ".. failed" ) != -1 )
            {
                ShowMessage ( TextStrings.errorUnknown );
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
        
        
        private void ShowMessage ( string message )
        {
            
            Application.Invoke ( delegate
            {
                SetMode ( "Normal" );
                
                messageBar.SetMessage ( message, null, MessageType.Error );
            });
            
        }
        
        
        private void HideMessage ( object obj, EventArgs args )
        {
            
            HideMessage ();
            
        }
        
        
        private void HideMessage ()
        {
            
            messageBar.Hide ();
            
        }
        
        
        private void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                
                case "Attaching":
                
                    idEntry.Sensitive       = false;
                    withNetworks.Sensitive  = false;
                    cancelBut.Sensitive     = false;
                    attachBut.Sensitive     = false;
                    attachBut.Label         = TextStrings.attachingLabel;
                    
                    break;
                    
                case "Normal":
                
                    idEntry.Sensitive       = true;
                    withNetworks.Sensitive  = true;
                    cancelBut.Sensitive     = true;
                    attachBut.Sensitive     = true;
                    attachBut.Label         = TextStrings.attachButtonLabel;
                    
                    break;
                 
            }
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        private void Dismiss ()
        {
            
            GlobalEvents.SetModalDialog ( null );
            
            this.Destroy ();
            
        }
        
        
        private void Dismiss ( object obj, EventArgs args )
        {
            
            Dismiss ();
            
        }
        
    }

}
