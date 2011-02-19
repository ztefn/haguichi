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
            
            Haguichi.modalDialog = this;
            
            this.Mode            = mode;
            
            this.Title           = title;
            this.TransientFor    = Haguichi.mainWindow.ReturnWindow ();
            this.IconList        = MainWindow.appIcons;
            this.Modal           = true;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.SkipTaskbarHint = true;
            this.BorderWidth     = 6;
            this.DeleteEvent    += OnDeleteEvent;
            
            this.ActionArea.Destroy ();
            
            
            heading = new Label ();
            heading.Xalign = 0;
            heading.Ypad = 6;

            HBox headBox = new HBox ();
            headBox.Add ( heading );
            
            
            failImg = new Image ( Stock.DialogError, IconSize.Button );
            
            failLabel = new Label ();
            failLabel.Xalign = 0;

            failBox = new EventBox ();
            failHBox = new HBox ();
            failHBox.Add ( failImg );
            failHBox.Add ( failLabel );
            failBox.Add ( failHBox );
            
            Box.BoxChild bc11 = ( ( Box.BoxChild ) ( failHBox [ failLabel ] ) );
            bc11.Padding = 6;
            bc11.Expand = false;
            
            Box.BoxChild bc12 = ( ( Box.BoxChild ) ( failHBox [ failImg ] ) );
            bc12.Expand = false;
            
            
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
            nameEntry.FocusGrabbed += HideFailure;
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
            passwordEntry.FocusGrabbed += HideFailure;
            passwordEntry.Visibility = false;
            passwordEntry.WidthChars = 30;
            passwordLabel = new Label ( TextStrings.passwordLabel + "  " );
            passwordLabel.Xalign = 0;
            passwordLabel.MnemonicWidget = passwordEntry;
            
            
            passwordBox = new HBox ();
            passwordBox.Add ( passwordLabel );
            passwordBox.Add ( passwordEntry );
            
            Box.BoxChild bc9 = ( ( Box.BoxChild ) ( passwordBox [ passwordEntry ] ) );
            

            VBox inputBox = new VBox ();
            inputBox.Add ( nameBox );
            inputBox.Add ( passwordBox );
            
            goOnline = new CheckButton ( TextStrings.checkboxGoOnlineInNewNetwork );
            goOnline.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.GoOnlineInNewNetwork, goOnline.Active );
            };
            
            VBox vbox = new VBox ();
            vbox.Add ( headBox );
            vbox.Add ( failBox );
            vbox.Add ( inputBox );
            vbox.Add ( goOnline );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ headBox ] ) );
            //bc8.Padding = 6;
            bc8.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc13 = ( ( Box.BoxChild ) ( vbox [ failBox ] ) );
            bc13.Padding = 6;
            bc13.Expand = false;
            
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
            
            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc7.Padding = 6;
                        
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( headBox [ heading ] ) );
            bc2.Expand = false;

            
            this.VBox.Add ( hbox );
            this.VBox.ShowAll ();
            
            HideFailure ();
            
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
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        private void Dismiss ()
        {
            
            Haguichi.modalDialog = null;
            
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
                ShowFailure ( TextStrings.errorNetworkNotFound );
                return;
            }
            else if ( output.Contains ( ".. failed, invalid password" ) )
            {
                ShowFailure ( TextStrings.errorInvalidPassword );
                return;
            }
            else if ( output.Contains ( ".. failed, the network is full" ) )
            {
                ShowFailure ( TextStrings.errorNetworkFull );
                return;
            }
            else if ( output.Contains ( ".. failed, network is locked" ) )
            {
                ShowFailure ( TextStrings.errorNetworkLocked );
                return;
            }
            else if ( output.Contains ( ".. failed, you are already a member" ) )
            {
                ShowFailure ( TextStrings.errorNetworkAlreadyJoined );
                return;
            }
            else if ( output.Contains ( ".. failed, manual approval required" ) )
            {
                Application.Invoke ( delegate
                {
                    Dialogs.SendRequest dlg = new Dialogs.SendRequest ( this, TextStrings.sendRequestTitle, TextStrings.sendRequestMessage , "Question", this.NetworkName, this.NetworkPassword );
                    
                    SetMode ( "Normal" );
                    if ( dlg.ResponseText == "Ok" )
                    {
                        Dismiss ();
                    }
                });
                return;
            }
            else if ( output.Contains ( ".. failed" ) )
            {
                ShowFailure ( TextStrings.errorUnknown );
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
                ShowFailure ( TextStrings.errorNetworkNameTooShort );
                return;
            }
            else if ( output.Contains ( ".. failed, network name is already taken" ) )
            {
                ShowFailure ( TextStrings.errorNetworkNameTaken );
                return;
            }
            else if ( output.Contains ( ".. failed" ) )
            {
                ShowFailure ( TextStrings.errorUnknown );
                return;
            }
            else
            {
                // Unknown output
            }
            
        }
        
        
        private void ShowFailure ( string message )
        {
            
            Application.Invoke ( delegate
            {
                SetMode ( "Normal" );
                
                failLabel.Markup = String.Format ( "<span weight=\"bold\">{0}</span>", message );
                failBox.ShowAll ();
            });
            
        }
        
        
        private void HideFailure ( object obj, EventArgs args )
        {
            
            HideFailure ();
            
        }
        
        
        private void HideFailure ()
        {
            
            failBox.HideAll ();
            
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
            
            string head;
            string mesg;
            
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
