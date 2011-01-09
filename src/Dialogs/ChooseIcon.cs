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

    public class ChooseIcon : Dialog
    {
        
        private Dialogs.AddEditCommand Opener;
        private string CurrentIcon;
        private Button CurrentBut;
        private Button NoneBut;
        
        private Table Table;
        private uint Rows;
        private uint Columns;
        
        
        public ChooseIcon ( Dialogs.AddEditCommand opener, string currentIcon ) : base ()
        {
            
            this.Opener = opener;
            this.CurrentIcon = currentIcon;
            
            this.Rows = 10;
            this.Columns = 6;
            
            this.Modal = true;
            this.HasSeparator = false;
            this.Resizable = false;
            this.SkipTaskbarHint = true;
            this.IconList = MainWindow.appIcons;
            this.WindowPosition = WindowPosition.Mouse;
            this.BorderWidth = 9;
            this.ActionArea.Destroy ();
            this.DeleteEvent += OnDeleteEvent;
            this.Title = TextStrings.chooseIconTitle;
            
            Table = new Table ( Rows, Columns, false );
            
            Fill ();
            
            this.VBox.Add ( Table );
            this.ShowAll ();
            
            try
            {
                CurrentBut.GrabFocus ();
            }
            catch
            {
                NoneBut.GrabFocus ();
            }
            
        }
        
        
        private void Fill ()
        {
            
            uint countLeft   = 0;
            uint countRight  = 1;
            uint countTop    = 0;
            uint countBottom = 1;
            
            string [] icons = { "folder-remote",
                                "network-workgroup",
                                "network-server",
                                "printer",
                                "preferences-desktop-remote-desktop",
                                "application-x-remote-connection",
                                "applications-internet",
                                "computer",
                                "go-home",
                                "user-home",
                                "folder-open",
                                "utilities-terminal",
                                "utilities-system-monitor",
                                "dialog-password",
                                "system-search",
                                "audio-x-generic",
                                "video-x-generic",
                                "x-office-address-book",
                                "package-x-generic",
                                "input-gaming",
                                "system-users",
                                "drive-harddisk",
                                "drive-optical",
                                "drive-removable-media",
                                "camera-web",
                                "audio-input-microphone",
                                "network-wired",
                                "network-wireless",
                                "modem",
                                "call-start",
                                "call-stop",
                                "document-send",
                                "face-cool",
                                "face-devilish",
                                "system-shutdown" };
            
            foreach ( string icon in icons )
            {
                //Console.WriteLine ( "left: " + countLeft + "  right: " + countRight + "  top: " + countTop + "  bottom: " + countBottom );
                
                Image img = new Image ();
                img.SetFromIconName ( icon, IconSize.LargeToolbar );
                
                Button iconBut = new Button ();
                iconBut.Image = img;
                iconBut.Relief = ReliefStyle.None;
                iconBut.TooltipText = icon;
                iconBut.HasTooltip = false;
                iconBut.Clicked += delegate {
                    
                    Opener.SetIcon ( iconBut.TooltipText );
                    Dismiss ();
                    
                };
                if ( CurrentIcon == icon )
                {
                    CurrentBut = iconBut;
                }
                
                Table.Attach ( iconBut, countLeft, countRight, countTop, countBottom );
                
                if ( countLeft < Columns )
                {
                    
                    countLeft   += 1;
                    countRight  += 1;
                }
                else
                {
                    if ( countTop < Columns )
                    {
                        countTop    += 1;
                        countBottom += 1;
                    }
                    else
                    {
                        countTop     = 0;
                        countBottom  = 1;
                    }
                    
                    countLeft    = 0;
                    countRight   = 1;
                }
            }
            
            NoneBut = new Button ( TextStrings.noIconLabel );
            NoneBut.TooltipText = "none";
            NoneBut.HasTooltip = false;
            NoneBut.Clicked += delegate {
                    
                Opener.SetIcon ( NoneBut.TooltipText );
                Dismiss ();
                
            };
            
            Table.Attach ( NoneBut, 0, 7, 5, 6 );
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
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
