/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2012 Stephen Brandt <stephen@stephenbrandt.com>
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
using System.Diagnostics;
using System.Threading;
using Gtk;
using Widgets;


namespace Dialogs
{

    public class JoinCreate : Dialog
    {
        
        private string Mode;
        
        private string NetworkName;
        private string NetworkPassword;
        
        private Label heading;
        
        private Label nameLabel;
        private Entry nameEntry;
        
        private Label passwordLabel;
        private Entry passwordEntry;
        
        private MessageBar messageBar;
        
        private Button cancelBut;
        private Button joinBut;
        private Button createBut;
        
        
        public JoinCreate ( string mode, string title ) : base ()
        {
            
            GlobalEvents.SetModalDialog ( this );
            
            this.Mode            = mode;
            
            this.Title           = title;
            this.TransientFor    = Haguichi.mainWindow.ReturnWindow ();
            this.IconList        = MainWindow.appIcons;
            this.Modal           = true;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.SkipTaskbarHint = true;
            this.DeleteEvent    += OnDeleteEvent;
            
            this.ActionArea.Destroy ();
            
            
            heading = new Label ();
            heading.Xalign = 0;
            
            messageBar = new MessageBar ();
            
            
            cancelBut = new Button ( Stock.Cancel );
            cancelBut.Clicked += Dismiss;
            
            createBut = new Button ( Stock.New ); // Using an stock button to trigger underscore interpretation within the label text, seems a GTK bug or misdesign
            createBut.CanDefault = true;
            createBut.Label = TextStrings.createLabel;
            createBut.Clicked += GoCreate;
            
            joinBut = new Button ( Stock.Add ); // Using an stock button to trigger underscore interpretation within the label text, seems a GTK bug or misdesign
            joinBut.CanDefault = true;
            joinBut.Label = TextStrings.joinLabel;
            joinBut.Clicked += GoJoin;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( cancelBut );
            buttonBox.Add ( createBut );
            buttonBox.Add ( joinBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            
            nameEntry = new Entry ();
            nameEntry.Changed += CheckNameLength;
            nameEntry.Changed += HideMessage;
            nameEntry.ActivatesDefault = true;
            nameEntry.WidthChars = 30;
            nameEntry.MaxLength = 40;
            nameLabel = new Label ( TextStrings.nameLabel + "  " );
            nameLabel.Xalign = 0;
            nameLabel.MnemonicWidget = nameEntry;

            passwordEntry = new Entry ();
            passwordEntry.Changed += HideMessage;
            passwordEntry.ActivatesDefault = true;
            passwordEntry.Visibility = false;
            passwordEntry.WidthChars = 30;
            passwordLabel = new Label ( TextStrings.passwordLabel + "  " );
            passwordLabel.Xalign = 0;
            passwordLabel.MnemonicWidget = passwordEntry;
            
            Table inputTable = new Table ( 2, 2, false );
            inputTable.RowSpacing = 6;
            inputTable.Attach ( nameLabel, 0, 1, 0, 1 );
            inputTable.Attach ( nameEntry, 1, 2, 0, 1 );
            inputTable.Attach ( passwordLabel, 0, 1, 1, 2 );
            inputTable.Attach ( passwordEntry, 1, 2, 1, 2 );
            
            
            VBox vbox = new VBox ();
            vbox.Add ( heading );
            vbox.Add ( inputTable );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ heading ] ) );
            bc8.Padding = 6;
            bc8.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc14 = ( ( Box.BoxChild ) ( vbox [ inputTable ] ) );
            bc14.Padding = 6;
            bc14.Expand = false;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( vbox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc1.Padding = 12;
            
            
            VBox container = new VBox ();
            container.Add ( messageBar );
            container.Add ( hbox );
            container.ShowAll ();
            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( container [ hbox ] ) );
            bc7.Padding = 6;
            
            
            this.Remove ( this.VBox );
            this.Add ( container );
            
            HideMessage ();
            SetMode ( mode );
            
            this.Show ();
            
            nameEntry.GrabFocus ();
            
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
        
        
        private void GoJoin ( object obj, EventArgs args )
        {
            
            SetMode ( "Joining" );
            
            Thread thread = new Thread ( GoJoinThread );
            thread.Start ();
            
        }
        
        
        private void GoJoinThread ()
        {
            
            this.NetworkName     = nameEntry.GetChars ( 0, -1 );
            this.NetworkPassword = passwordEntry.GetChars ( 0, -1 );
            
            string output = "";
            
            if ( Config.Settings.DemoMode )
            {
                output = ".. failed, manual approval required";
            }
            else
            {
                output = Hamachi.JoinNetwork ( this.NetworkName, this.NetworkPassword );
            }
            
            if ( output.Contains ( ".. ok" ) )
            {
                if ( Hamachi.ApiVersion == 1 )
                {
                    Hamachi.GoOnline ( this.NetworkName );
                }
                
                Thread.Sleep ( 1000 ); // Wait a second to get an updated list
                
                Application.Invoke ( delegate
                {
                    Dismiss ();
                    Controller.UpdateConnection (); // Update list
                });
                
                Thread.Sleep ( 2000 );
                Hamachi.SetNick ( ( string ) Config.Client.Get ( Config.Settings.Nickname ) ); // Set nick to make sure any clients in this network will see it
                
                return;
            }
            else if ( output.Contains ( ".. failed, network not found" ) )
            {
                ShowMessage ( output, TextStrings.errorNetworkNotFound );
                return;
            }
            else if ( output.Contains ( ".. failed, invalid password" ) )
            {
                ShowMessage ( output, TextStrings.errorInvalidPassword );
                return;
            }
            else if ( output.Contains ( ".. failed, the network is full" ) )
            {
                ShowMessage ( output, TextStrings.errorNetworkFull );
                return;
            }
            else if ( output.Contains ( ".. failed, network is locked" ) )
            {
                ShowMessage ( output, TextStrings.errorNetworkLocked );
                return;
            }
            else if ( output.Contains ( ".. failed, you are already a member" ) )
            {
                ShowMessage ( output, TextStrings.errorNetworkAlreadyJoined );
                return;
            }
            else if ( output.Contains ( ".. failed, manual approval required" ) )
            {
                Application.Invoke ( delegate
                {
                    Button noButton = new Button ( Stock.No );
                    noButton.Clicked += delegate
                    {
                        HideMessage ();
                        SetMode ( "Normal" );
                        nameEntry.GrabFocus ();
                    };
                    
                    Button yesButton = new Button ( Stock.Yes );
                    yesButton.Clicked += delegate
                    {
                        output = Hamachi.SendJoinRequest ( this.NetworkName, this.NetworkPassword );
                        if ( ( output.Contains ( ".. ok, request sent, waiting for approval" ) ) ||
                             ( Config.Settings.DemoMode ) )
                        {
                            MainWindow.messageBar.SetMessage ( TextStrings.requestSentMessage, null, MessageType.Info, 3000 );
                        }
                        
                        Dismiss ();
                    };
                    
                    messageBar.SetMessage ( TextStrings.sendRequestTitle, TextStrings.sendRequestMessage, MessageType.Question );
                    messageBar.AddButton ( noButton );
                    messageBar.AddButton ( yesButton );
                    
                    yesButton.CanDefault = true;
                    yesButton.GrabDefault ();
                    yesButton.GrabFocus ();
                });
                return;
            }
            else if ( output.Contains ( ".. failed" ) )
            {
                ShowMessage ( output, TextStrings.errorUnknown );
                return;
            }
            else
            {
                // Unknown output
            }
            
        }
        
        
        private void GoCreate ( object obj, EventArgs args )
        {
            
            SetMode ( "Creating" );
            
            Thread thread = new Thread ( GoCreateThread );
            thread.Start ();
            
        }
        
        
        private void GoCreateThread ()
        {
            
            this.NetworkName     = nameEntry.GetChars ( 0, -1 );
            this.NetworkPassword = passwordEntry.GetChars ( 0, -1 );
            
            if ( Config.Settings.DemoMode )
            {
                Application.Invoke ( delegate
                {
                    Network network = new Network ( new Status ( "*" ), Hamachi.RandomNetworkId (), this.NetworkName, "", 5 );
                    MainWindow.networkView.AddNetwork ( network );
                    
                    Dismiss ();
                });
                return;
            }
            
            string output = Hamachi.CreateNetwork ( this.NetworkName, this.NetworkPassword );
            
            if ( output.Contains ( ".. ok" ) )
            {
                if ( Hamachi.ApiVersion == 1 )
                {
                    Hamachi.GoOnline ( this.NetworkName );
                }
                
                Thread.Sleep ( 1000 ); // Wait a second to get an updated list
                
                Application.Invoke ( delegate
                {
                    Dismiss ();
                    Controller.UpdateConnection (); // Update list
                });
                
                Thread.Sleep ( 2000 );
                Hamachi.SetNick ( ( string ) Config.Client.Get ( Config.Settings.Nickname ) ); // Set nick to make sure any clients in this network will see it
                
                return;
            }
            else if ( output.Contains ( "Network name must be between 4 and 64 characters long" ) )
            {
                ShowMessage ( output, TextStrings.errorNetworkNameTooShort );
                return;
            }
            else if ( output.Contains ( ".. failed, network name is already taken" ) )
            {
                ShowMessage ( output, TextStrings.errorNetworkNameTaken );
                return;
            }
            else if ( output.Contains ( ".. failed" ) )
            {
                ShowMessage ( output, TextStrings.errorUnknown );
                return;
            }
            else
            {
                // Unknown output
            }
            
        }
        
        
        private void ShowMessage ( string output, string message )
        {
            
            Application.Invoke ( delegate
            {
                SetMode ( "Normal" );
                
                if ( output.Contains ( "password" ) )
                {
                    passwordEntry.GrabFocus ();
                }
                else
                {
                    nameEntry.GrabFocus ();
                }
                
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
        
        
        private void CheckNameLength ( object obj, EventArgs args )
        {
            
            CheckNameLength ();
            
        }
        
        
        private void CheckNameLength ()
        {
            
            string chars = nameEntry.GetChars ( 0, -1 );
            
            if ( chars.Length > 0 )
            {
                createBut.Sensitive = true;
                joinBut.Sensitive = true;
                
                if ( Mode == "Join" )
                {
                    joinBut.GrabDefault ();
                }
                if ( Mode == "Create" )
                {
                    createBut.GrabDefault ();
                }
            }
            else
            {
                createBut.Sensitive = false;
                joinBut.Sensitive = false;
            }
            
        }
        
        
        private void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                
                case "Join":
                
                    createBut.Hide ();
                    
                    joinBut.Show ();
                    joinBut.Sensitive = false;

                    heading.Markup = String.Format ( TextStrings.joinNetworkMessage );
                    
                    break;
                    
                case "Create":
                
                    joinBut.Hide ();
                    
                    createBut.Show ();
                    createBut.Sensitive = false;
                    
                    heading.Markup = String.Format ( TextStrings.createNetworkMessage );
                    
                    break;
                    
                case "Joining":
                
                    cancelBut.Sensitive = false;
                    
                    joinBut.Sensitive = false;
                    joinBut.Label = TextStrings.joiningLabel;
                    
                    nameEntry.Sensitive = false;
                    passwordEntry.Sensitive = false;
                    
                    break;
                    
                case "Creating":
                
                    cancelBut.Sensitive = false;
                    
                    createBut.Sensitive = false;
                    createBut.Label = TextStrings.creatingLabel;
                    
                    nameEntry.Sensitive = false;
                    passwordEntry.Sensitive = false;
                    
                    break;
                    
                case "Normal":
                
                    SetMode ( Mode );
                    
                    cancelBut.Sensitive = true;
                    
                    createBut.Label = TextStrings.createLabel;
                    joinBut.Label = TextStrings.joinLabel;
                    
                    nameEntry.Sensitive = true;
                    passwordEntry.Sensitive = true;
                    
                    CheckNameLength ();
                    
                    break;
                 
            }
            
        }
        
    }

}
