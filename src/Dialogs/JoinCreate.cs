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
using System.Diagnostics;
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
            
            this.Mode = mode;
            
            this.Title = title;
            this.HasSeparator = false;
            this.Resizable = false;
            this.SkipTaskbarHint = true;
            this.IconList = MainWindow.appIcons;
            this.BorderWidth = 6;
            this.ActionArea.Destroy ();
            this.DeleteEvent += OnDeleteEvent;
            
            
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
            passwordEntry.MaxLength = 40;
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
            
            if ( Hamachi.ApiVersion > 1 )
            {
                goOnline.Hide ();
            }
            
            SetMode ( mode );
            
            HideFailure ();
                        
        }
        
        
        private void OnDeleteEvent (object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        public void Open ()
        {
            
            goOnline.Active = (bool) Config.Client.Get ( Config.Settings.GoOnlineInNewNetwork );
            
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
            SetMode ( "Reset" );
            
        }
        
        
        private void Dismiss ( object obj, EventArgs args )
        {
            
            Dismiss ();
            
        }
        
        
        private void GoJoin ( object obj, EventArgs args )
        {
            
            SetMode ( "Joining" );
            
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
            
            if ( output.IndexOf ( ".. failed, network not found" ) != -1 )
            {
                ShowFailure ( TextStrings.errorNetworkNotFound );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed, invalid password" ) != -1 )
            {
                ShowFailure ( TextStrings.errorInvalidPassword );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed, the network is full" ) != -1 )
            {
                ShowFailure ( TextStrings.errorNetworkFull );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed, network is locked" ) != -1 )
            {
                ShowFailure ( TextStrings.errorNetworkLocked );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed, you are already a member" ) != -1 )
            {
                ShowFailure ( TextStrings.errorNetworkAlreadyJoined );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed, manual approval required" ) != -1 )
            {
                Dialogs.SendRequest dlg = new Dialogs.SendRequest ( TextStrings.sendRequestTitle, TextStrings.sendRequestMessage , "Question", this.NetworkName, this.NetworkPassword );
                
                SetMode ( "Normal" );
                
                if ( dlg.ResponseText == "Ok" )
                {
                    Dismiss ();
                }
            }
            else if ( output.IndexOf ( ".. failed" ) != -1 )
            {
                ShowFailure ( TextStrings.errorUnknown );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. ok" ) != -1 )
            {
                Dismiss ();
                
                if ( ( bool ) Config.Client.Get ( Config.Settings.GoOnlineInNewNetwork ) )
                {
                    Hamachi.GoOnline ( this.NetworkName );
                }
                
                Controller.UpdateConnection (); // Update list
            }
            else
            {
                // Unknown output
            }
            
            CheckNameLength ( obj, args );
            
        }
        
        
        private void GoCreate ( object obj, EventArgs args )
        {
            
            SetMode ( "Creating" );
            
            this.NetworkName     = nameEntry.GetChars ( 0, -1 );
            this.NetworkPassword = passwordEntry.GetChars ( 0, -1 );
            
            if ( Config.Settings.DemoMode )
            {
                Network network = new Network ( new Status ( "*" ), Hamachi.RandomNetworkId (), this.NetworkName );
                MainWindow.networkView.AddNetwork ( network );
                Dismiss ();
                return;
            }
            
            string output = Hamachi.CreateNetwork ( this.NetworkName, this.NetworkPassword );
            
            if ( output.IndexOf ( "Network name must be between 4 and 64 characters long" ) != -1 )
            {
                ShowFailure ( TextStrings.errorNetworkNameTooShort );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed, network name is already taken" ) != -1 )
            {
                ShowFailure ( TextStrings.errorNetworkNameTaken );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. failed" ) != -1 )
            {
                ShowFailure ( TextStrings.errorUnknown );
                SetMode ( "Normal" );
            }
            else if ( output.IndexOf ( ".. ok" ) != -1 )
            {
                Dismiss ();
                
                if ( ( bool ) Config.Client.Get ( Config.Settings.GoOnlineInNewNetwork ) )
                {
                    Hamachi.GoOnline ( this.NetworkName );
                }
                
                Controller.UpdateConnection (); // Update list
            }
            else
            {
                // Unknown output
            }
            
            CheckNameLength ( obj, args );
            
        }
        
        
        private void ShowFailure ( string message )
        {
            
            failLabel.Markup = String.Format ( "<span weight=\"bold\">{0}</span>", message );
            failBox.ShowAll ();
            
        }
        
        
        private void HideFailure ()
        {
            
            failBox.HideAll ();
            
        }
        
        
        private void HideFailure ( object obj, EventArgs args )
        {
            
            HideFailure ();
            
        }
        
        
        private void CheckNameLength ( object obj, EventArgs args )
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
                                        
                    if ( Mode == "Join" )
                    {
                        SetMode ( "Join" );
                    }
                    if ( Mode == "Create" )
                    {
                        SetMode ( "Create" );
                    }
                    
                    cancelBut.Sensitive = true;
                    
                    createBut.Label = TextStrings.createLabel;
                    joinBut.Label = TextStrings.joinLabel;
                
                    nameEntry.Sensitive = true;
                    passwordEntry.Sensitive = true;
                    
                    break;
                
                case "Reset":
                         
                    SetMode ( "Normal" );
                
                    HideFailure ();
                    
                    nameEntry.GrabFocus ();
                
                    nameEntry.DeleteText ( 0, -1 );
                    passwordEntry.DeleteText ( 0, -1 );
                    
                    break;
                
            }
            
        }
        
    }

}
