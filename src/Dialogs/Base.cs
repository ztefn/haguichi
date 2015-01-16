/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2015 Stephen Brandt <stephen@stephenbrandt.com>
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

    public class Base : Dialog
    {
        
        public string ResponseText;
        public VBox Contents;
        
        
        public Base ( Window parent, string title, string header, string message, string icon ) : base ()
        {
            
            this.Title           = title;
            this.TransientFor    = parent;
            this.IconList        = MainWindow.appIcons;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.BorderWidth     = 5;
            this.Response       += SetResponseText;
            
            Image img = new Image ();
            img.SetFromIconName ( "dialog-" + icon.ToLower (), IconSize.Dialog );
            img.Yalign = 0;
            img.Xpad = 6;
            
            Label headerLabel = new Label ();
            headerLabel.Markup = String.Format ( "<span size=\"large\" weight=\"bold\">{0}</span>", header );
            headerLabel.Xalign = 0;
            headerLabel.Selectable = true;
            
            Label messageLabel = new Label ();
            messageLabel.Markup = String.Format ( "{0}", message );
            messageLabel.Xalign = 0;
            messageLabel.Ypad = 12;
            messageLabel.Selectable = true;
            
            Contents = new VBox ();
            Contents.PackStart ( headerLabel, false, false, 0 );
            Contents.PackStart ( messageLabel, false, false, 0 );
            
            HBox hbox = new HBox ();
            hbox.PackStart ( img, false, false, 0 );
            hbox.PackStart ( Contents, false, false, 6 );
            
            this.VBox.PackStart ( hbox, true, true, 5 );
            
            this.VBox.ShowAll ();
            
        }
        
        
        public void AddContent ( Widget widget )
        {
            
            this.Contents.Add ( widget );   
            widget.Show ();
            
        }
        
        
        public void SetResponseText ( object o, ResponseArgs args )
        {
            
            this.ResponseText = args.ResponseId.ToString ();
            
        }

    }

}
