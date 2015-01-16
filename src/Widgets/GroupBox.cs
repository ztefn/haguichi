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


namespace Widgets
{
    
    public class GroupBox : VBox
    {
        
        private VBox grpBox;
        private Label grpLabel;
        
        
        public GroupBox ( string label ) : base ()
        {
            
            this.BorderWidth = 6;
            
            if ( label != "" )
            {
                grpLabel = new Label ();
                grpLabel.Xalign = 0;
                grpLabel.Xpad = 6;
                grpLabel.Ypad = 6;
                grpLabel.Markup = String.Format ( "<b>{0}</b>", label );
                
                this.Add ( grpLabel );
            }
            
            VBox emptyBox = new VBox ();
            
            grpBox = new VBox ();
            grpBox.Spacing = 6;
            
            HBox hbox = new HBox ();
            hbox.PackStart ( emptyBox, false, false, 6 );
            hbox.PackStart ( grpBox, true, true, 3 );
            
            this.Add ( hbox );
            
        }
        
        public void AddWidget ( Widget widget )
        {
            
            this.grpBox.PackStart ( widget, false, false, 0 );
            
        }
        
    }
    
}
