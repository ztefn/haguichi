/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2013 Stephen Brandt <stephen@stephenbrandt.com>
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

    public class GroupBox : Frame
    {

        private VBox grpBox;
        private Label grpLabel;
        
        
        public GroupBox ( string label ) : base ()
        {
        
            this.ShadowType = ShadowType.None;
            this.BorderWidth = 6;
            
            if ( label != "" )
            {
                grpLabel = new Label ();
                grpLabel.Ypad = 6;
                grpLabel.Markup = String.Format ( "<b>{0}</b>", label );
                
                this.LabelWidget = grpLabel;
            }

            VBox emptyBox = new VBox ();
            
            grpBox = new VBox ();
            grpBox.Spacing = 6;
            
            HBox hbox = new HBox ();
            hbox.Add ( emptyBox );
            hbox.Add ( grpBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( hbox [ emptyBox ] ) );
            bc1.Padding = 6;
            bc1.Expand = false;
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( hbox [ grpBox ] ) );
            bc2.Padding = 3;
            
            this.Add ( hbox );

        }
        
        public void AddWidget ( Widget widget )
        {
        
            this.grpBox.Add ( widget );
            
        }
        
    }
    
}
