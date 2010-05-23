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

    public class Base : Dialog
    {
        
        public VBox Contents;
        
        
        public Base ( string title, string header, string message, string icon ) : base ()
        {
        
            this.Title = title;
            this.IconList = MainWindow.appIcons;
            this.HasSeparator = false;
            this.Resizable = false;
            this.BorderWidth = 6;


            Image img = new Image ( "", IconSize.Dialog );
            img.Yalign = 0;
            img.Xpad = 3;
            
            switch ( icon )
            {
                case "Authentication":  
                    img.Stock = Stock.DialogAuthentication;
                    break;
                case "Error":  
                    img.Stock = Stock.DialogError;
                    break;
                case "Info":  
                    img.Stock = Stock.DialogInfo;
                    break;
                case "Question":  
                    img.Stock = Stock.DialogQuestion;
                    break;
                case "Warning":  
                    img.Stock = Stock.DialogWarning;
                    break;
            }

            Label headerLabel = new Label ();
            headerLabel.Markup = String.Format ( "<span size=\"large\" weight=\"bold\">{0}</span>", header );
            headerLabel.Xalign = 0;
            headerLabel.Xpad = 9;
            
            Label messageLabel = new Label ();
            messageLabel.Markup = String.Format ( "{0}", message );
            messageLabel.Xalign = 0;
            messageLabel.Xpad = 9;
            messageLabel.Ypad = 12;
           
            Contents = new VBox ();
            Contents.Add ( headerLabel );
            Contents.Add ( messageLabel );
            
            Box.BoxChild bc0 = ( ( Box.BoxChild ) ( Contents [ headerLabel ] ) );
            bc0.Expand = false;

            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( Contents [ messageLabel ] ) );
            bc4.Expand = false;
            
            HBox hbox = new HBox ();
            hbox.Add ( img );
            hbox.Add ( Contents );
           
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( hbox [ img ] ) );
            bc1.Expand = false;
           
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( hbox [ Contents ] ) );
            bc2.Expand = false;
           
            this.VBox.Add ( hbox );
            
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( this.VBox [ hbox ] ) );
            bc3.Padding = 3;
            
            this.VBox.ShowAll ();

        }
        
        
        public void AddContent ( Widget widget )
        {
            this.Contents.Add ( widget );   
            widget.Show ();
        }

    }

}
