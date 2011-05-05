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
        private HBox  nameBox;
        
        private Label passwordLabel;
        private Entry passwordEntry;
        private HBox  passwordBox;
        
        private MessageBar messageBar;
        
        private Image    failImg;
        private Label    failLabel;
        private EventBox failBox;
        private HBox     failHBox;
        
        public  CheckButton goOnline;
        
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
            nameEntry.ActivatesDefault = true;
            nameEntry.FocusGrabbed += HideMessage;
            nameEntry.WidthChars = 30;
            nameEntry.MaxLength = 40;
            nameEntry.Changed += CheckNameLength;
            nameLabel = new Label ( TextStrings.nameLabel + "  " );
            nameLabel.Xalign = 0;
            nameLabel.MnemonicWidget = nameEntry;

            
            nameBox = new HBox ();
            nameBox.Add ( nameLabel );
            nameBox.Add ( nameEntry );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( nameBox [ nameEntry ] ) );
            bc5.Expand = false;
            
            passwordEntry = new Entry ();
            passwordEntry.ActivatesDefault = true;
            passwordEntry.FocusGrabbed += HideMessage;
            passwordEntry.Visibility = false;
            passwordEntry.WidthChars = 30;
            passwordLabel = new Label ( TextStrings.passwordLabel + "  " );
            passwordLabel.Xalign = 0;
            passwordLabel.MnemonicWidget = passwordEntry;
            
            
            passwordBox = new HBox ();
            passwordBox.Add ( passwordLabel );
            passwordBox.Add ( passwordEntry );
            

            VBox inputBox = new VBox ();
            inputBox.Add ( nameBox );
            inputBox.Add ( passwordBox );
            
            goOnline = new CheckButton ( TextStrings.checkboxGoOnlineInNewNetwork );
            goOnline.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.GoOnlineInNewNetwork, goOnline.Active );
            };
            
            VBox vbox = new VBox ();
            vbox.Add ( heading );
            vbox.Add ( inputBox );
            vbox.Add ( goOnline );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ heading ] ) );
            bc8.Padding = 6;
            bc8.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( inputBox [ nameBox ] ) );
            bc6.Padding = 3;
            bc6.Expand = false;
            
            Box.BoxChild bc10 = ( ( Box.BoxChild ) ( inputBox [ passwordBox ] ) );
            bc10.Padding = 3;
            bc10.Expand = false;
            
            Box.BoxChild bc15 = ( ( Box.BoxChild ) ( vbox [ goOnline ] ) );
            bc15.Padding = 3;
            bc15.Expand = false;
            
            Box.BoxChild bc14 = ( ( Box.BoxChild ) ( vbox [ inputBox ] ) );
            bc14.Padding = 3;
            bc14.Expand = false;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( vbox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc1.Padding = 12;
            
            
            
            VBox container = new VBox ();
            container.Add ( messageBar );
            container.Add ( hbox );
            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( container [ hbox ] ) );
            bc7.Padding = 6;
            
            this.Remove ( this.VBox );
            this.Add ( container );
            this.ShowAll ();
            
            HideMessage ();
            
            if ( Hamachi.ApiVersion > 1 )
            {
                goOnline.Hide ();
            }
            else if ( Hamachi.ApiVersion == 1 )
            {
                goOnline.Active = ( bool ) Config.Client.Get ( Config.Settings.GoOnlineInNewNetwork );
            }
            
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
                if ( ( Hamachi.ApiVersion == 1 ) &&
                     ( bool ) Config.Client.Get ( Config.Settings.GoOnlineInNewNetwork ) )
                {
                    Hamachi.GoOnline ( this.NetworkName );
                }
                
                Application.Invoke ( delegate
                {
                    Dismiss ();
                    Controller.UpdateConnection (); // Update list
                });
                
                return;
            }
            else if ( output.Contains ( ".. failed, network not found" ) )
            {
                ShowMessage ( TextStrings.errorNetworkNotFound );
                return;
            }
            else if ( output.Contains ( ".. failed, invalid password" ) )
            {
                ShowMessage ( TextStrings.errorInvalidPassword );
                return;
            }
            else if ( output.Contains ( ".. failed, the network is full" ) )
            {
                ShowMessage ( TextStrings.errorNetworkFull );
                return;
            }
            else if ( output.Contains ( ".. failed, network is locked" ) )
            {
                ShowMessage ( TextStrings.errorNetworkLocked );
                return;
            }
            else if ( output.Contains ( ".. failed, you are already a member" ) )
            {
                ShowMessage ( TextStrings.errorNetworkAlreadyJoined );
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
                    };
                    
                    Button yesButton = new Button ( Stock.Yes );
                    yesButton.Clicked += delegate
                    {
                        output = Hamachi.SendJoinRequest ( this.NetworkName, this.NetworkPassword );
                        if ( output.Contains ( ".. ok, request sent, waiting for approval" ) )
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
                });
                return;
            }
            else if ( output.Contains ( ".. failed" ) )
            {
                ShowMessage ( TextStrings.errorUnknown );
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
                    Network network = new Network ( new Status ( "*" ), Hamachi.RandomNetworkId (), this.NetworkName );
                    MainWindow.networkView.AddNetwork ( network );
                    
                    Dismiss ();
                });
                return;
            }
            
            string output = Hamachi.CreateNetwork ( this.NetworkName, this.NetworkPassword );
            
            if ( output.Contains ( ".. ok" ) )
            {
                if ( ( Hamachi.ApiVersion == 1 ) &&
                     ( bool ) Config.Client.Get ( Config.Settings.GoOnlineInNewNetwork ) )
                {
                    Hamachi.GoOnline ( this.NetworkName );
                }
                
                Application.Invoke ( delegate
                {
                    Dismiss ();
                    Controller.UpdateConnection (); // Update list
                });
                return;
            }
            else if ( output.Contains ( "Network name must be between 4 and 64 characters long" ) )
            {
                ShowMessage ( TextStrings.errorNetworkNameTooShort );
                return;
            }
            else if ( output.Contains ( ".. failed, network name is already taken" ) )
            {
                ShowMessage ( TextStrings.errorNetworkNameTaken );
                return;
            }
            else if ( output.Contains ( ".. failed" ) )
            {
                ShowMessage ( TextStrings.errorUnknown );
                return;
            }
            else
            {
                // Unknown output
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
                    
                    nameEntry.GrabFocus ();
                    
                    CheckNameLength ();
                    
                    break;
                 
            }
            
        }
        
    }

}
