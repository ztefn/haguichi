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

    public class ChangePassword : Dialog
    {
        
        private Network Network;
        
        private Label heading;
        
        private Label passwordLabel;
        private Entry passwordEntry;
        private HBox  passwordBox;
        
        private Button cancelBut;
        private Button changeBut;
        
        
        public ChangePassword ( Network network ) : base ()
        {
            
            this.Network = network;
            
            this.Title = TextStrings.changePasswordTitle;
            this.Modal = true;
            this.HasSeparator = false;
            this.Resizable = false;
            this.SkipTaskbarHint = true;
            this.IconList = MainWindow.appIcons;
            this.BorderWidth = 6;
            this.ActionArea.Destroy ();
            this.DeleteEvent += OnDeleteEvent;
            
            
            heading = new Label ();
            heading.Markup = String.Format ( TextStrings.changePasswordMessage );
            heading.Xalign = 0;
            heading.Ypad = 6;

            HBox headBox = new HBox ();
            headBox.Add ( heading );
            
            
            cancelBut = new Button ( Stock.Cancel );
            cancelBut.Clicked += Dismiss;
            
            changeBut = new Button ( Stock.Edit ); // Using an stock button to trigger underscore interpretation within the label text, seems a GTK bug or misdesign
            changeBut.CanDefault = true;
            changeBut.Label = TextStrings.changeLabel;
            changeBut.Clicked += GoChangePassword;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( cancelBut );
            buttonBox.Add ( changeBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            passwordEntry = new Entry ();
            passwordEntry.ActivatesDefault = true;
            passwordEntry.WidthChars = 35;
            passwordEntry.MaxLength = 40;
            passwordEntry.Visibility = false;
            passwordLabel = new Label ( TextStrings.passwordLabel + "  " );
            passwordLabel.Xalign = 0;
            passwordLabel.MnemonicWidget = passwordEntry;

            
            passwordBox = new HBox ();
            passwordBox.Add ( passwordLabel );
            passwordBox.Add ( passwordEntry );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( passwordBox [ passwordEntry ] ) );
            bc5.Expand = false;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( headBox );
            vbox.Add ( passwordBox );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ headBox ] ) );
            bc8.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbox [ passwordBox ] ) );
            bc6.Padding = 6;
            bc6.Expand = false;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( vbox );

            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc7.Padding = 6;
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( headBox [ heading ] ) );
            bc2.Expand = false;
            
            
            this.VBox.Add ( hbox );
            this.ShowAll ();
            
            passwordEntry.GrabFocus ();
            changeBut.GrabDefault ();
            
        }
        
        
        private void OnDeleteEvent (object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        private void GoChangePassword ( object obj, EventArgs args )
        {
            
            this.Name = passwordEntry.GetChars ( 0, -1 );
            
            Hamachi.SetPassword ( this.Network.Id, this.Name );
            
            Dismiss ();
            
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
