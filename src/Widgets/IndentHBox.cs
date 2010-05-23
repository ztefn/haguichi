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


namespace Widgets
{

    public class IndentHBox : HBox
    {
        
        private int indent;
        
        
        public IndentHBox ( int count ) : base ()
        {
            Init ( count );
        }
        
        
        public IndentHBox () : base ()
        {
            Init ( 1 );
        }
        
        
        private void Init ( int count )
        {
            indent = count * 9;
            
            VBox emptyBox = new VBox ();

            this.Add ( emptyBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( this [ emptyBox ] ) );
            bc1.Padding = ( uint ) indent;
            bc1.Expand = false;
        }
        
        
        public void AddWidget ( Widget widget )
        {
            this.Add ( widget );
        }
        
    }
    
}
