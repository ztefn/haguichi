/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2014 Stephen Brandt <stephen@stephenbrandt.com>
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
using Mono.Unix;
using Gtk;


namespace Menus
{
    
    public class CommandMenuItem : ImageMenuItem
    {
        
        public CommandMenuItem ( string icon, string label, string commandIPv4, string commandIPv6, string priority ) : base ( Catalog.GetString ( label ) )
        {
            
            this.Activated += delegate
            {
                Member lastMember = MainWindow.networkView.lastMember;
                Command.Execute ( Command.ReturnCustom ( lastMember, commandIPv4, commandIPv6, priority ) );            
            };
            
            if ( icon != "none" )
            {
                Image img = new Image ();
                img.SetFromIconName ( icon, IconSize.Menu );
                
                this.Image = img;
            }
            
        }
        
    }
    
}
