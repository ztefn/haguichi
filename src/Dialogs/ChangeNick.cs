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


namespace Dialogs
{

    public class ChangeNick : Dialog
    {
        
        private Label heading;
        
        private Label nickLabel;
        private Entry nickEntry;
        private HBox  nickBox;
        
        private Button cancelBut;
        private Button changeBut;
        
        
        public ChangeNick ( string title ) : base ()
        {
            
            this.Title           = title;
            this.TransientFor    = Haguichi.mainWindow.ReturnWindow ();
            this.IconList        = MainWindow.appIcons;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.SkipTaskbarHint = true;
            this.BorderWidth     = 6;
            this.DeleteEvent    += OnDeleteEvent;
            
            this.ActionArea.Destroy ();
            
            
            heading = new Label ();
            heading.Markup = String.Format ( TextStrings.changeNickMessage );
            heading.Xalign = 0;
            heading.Ypad = 6;

            HBox headBox = new HBox ();
            headBox.Add ( heading );
            
            
            cancelBut = new Button ( Stock.Cancel );
            cancelBut.Clicked += Dismiss;
            
            changeBut = new Button ( Stock.Edit ); // Using an stock button to trigger underscore interpretation within the label text, seems a GTK bug or misdesign
            changeBut.CanDefault = true;
            changeBut.Label = TextStrings.changeLabel;
            changeBut.Clicked += GoChangeNick;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( cancelBut );
            buttonBox.Add ( changeBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            nickEntry = new Entry ();
            nickEntry.ActivatesDefault = true;
            nickEntry.MaxLength = 64;
            
            nickLabel = new Label ( TextStrings.nickLabel + "  " );
            nickLabel.Xalign = 0;
            nickLabel.MnemonicWidget = nickEntry;

            
            nickBox = new HBox ();
            nickBox.Add ( nickLabel );
            nickBox.Add ( nickEntry );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( nickBox [ nickLabel ] ) );
            bc5.Expand = false;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( headBox );
            vbox.Add ( nickBox );
            vbox.Add ( buttonBox );
            
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ headBox ] ) );
            bc8.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc4.Padding = 6;
            bc4.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbox [ nickBox ] ) );
            bc6.Padding = 6;
            bc6.Expand = false;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( vbox );

            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc7.Padding = 6;
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( headBox [ heading ] ) );
            bc2.Expand = false;
            
            
            this.VBox.Add ( hbox );
            this.VBox.ShowAll ();
            
            changeBut.GrabDefault ();
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        
        
        private void GoChangeNick ( object obj, EventArgs args )
        {
            
            SetMode ( "Changing" );
            
            this.Name = nickEntry.GetChars ( 0, -1 );
            
            Hamachi.SetNick ( this.Name );
            GlobalEvents.UpdateNick ( this.Name );
            
            Dismiss ();
            
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
                nickEntry.GrabFocus ();
            }
            
        }


        private void Dismiss ()
        {
            
            this.Visible = false;
            this.Hide ();
            GlobalEvents.UpdateNick ();
            SetMode ( "Reset" );
            
        }
        
        
        private void Dismiss ( object obj, EventArgs args )
        {
            
            Dismiss ();
            
        }
        
        
        public void SetNick ( string nick )
        {
            
            nickEntry.Text = nick;
            
        }
        
        
        private void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                
                case "Changing":
                    cancelBut.Sensitive = false;
                    
                    changeBut.Sensitive = false;
                    changeBut.Label = TextStrings.changingLabel;
                    
                    nickEntry.Sensitive = false;
                    
                    break;
                    
                case "Reset":
                    cancelBut.Sensitive = true;
                    
                    changeBut.Sensitive = true;
                    changeBut.Label = TextStrings.changeLabel;
                    
                    nickEntry.Sensitive = true;
                    
                    break;
                
            }
            
        }
        
    }

}
