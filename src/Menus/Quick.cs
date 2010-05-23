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


namespace Menus
{

    public class Quick : Menu
    {
    
        AccelGroup accelGroup;
        
        ImageMenuItem connect;
        ImageMenuItem disconnect;
        ImageMenuItem change;
        ImageMenuItem join;
        ImageMenuItem create;
        ImageMenuItem info;
        ImageMenuItem preferences;
        ImageMenuItem quit;
        
        
        public Quick()
        {
            
            connect = new ImageMenuItem ( Stock.Connect, accelGroup );
            connect.Activated += GlobalEvents.StartHamachi;
            
            disconnect = new ImageMenuItem ( Stock.Disconnect, accelGroup );
            disconnect.Activated += GlobalEvents.StopHamachi;
            
            change = new ImageMenuItem ( TextStrings.changeNickLabel );
            change.Activated += GlobalEvents.ChangeNick;
            
            join = new ImageMenuItem ( TextStrings.joinNetworkLabel );
            join.Activated += GlobalEvents.JoinNetwork;
            
            create = new ImageMenuItem ( TextStrings.createNetworkLabel );
            create.Activated += GlobalEvents.CreateNetwork;
            
            info = new ImageMenuItem ( Stock.Info, accelGroup );
            info.Activated += GlobalEvents.Information;
            
            preferences = new ImageMenuItem ( Stock.Preferences, accelGroup );
            preferences.Activated += GlobalEvents.Preferences;
            
            quit = new Gtk.ImageMenuItem ( Stock.Quit, accelGroup );
            quit.Activated += GlobalEvents.QuitApp;
            
            
            this.Add ( connect );
            this.Add ( disconnect );
            this.Add ( change );
            this.Add ( join );
            this.Add ( create );
            this.Add ( info );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( preferences );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( quit );
            
            this.ShowAll ();

        }
        
        
        public void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                case "Initiate":
                    connect.Sensitive = false;
                    disconnect.Visible = false;
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = false;
                
                    break;
                    
                case "Connecting":
                    // Nothing todo
                
                    break;
                    
                case "Connected":
                    connect.Visible = false;
                    disconnect.Visible = true;
                    change.Sensitive = true;
                    join.Sensitive = true;
                    create.Sensitive = true;
                    info.Sensitive = true;
                
                    break;
                    
                case "Disconnected":
                    connect.Visible = true;
                    connect.Sensitive = true;
                    disconnect.Visible = false;
                    change.Sensitive = true;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
                case "Not configured":
                    connect.Sensitive = false;
                    disconnect.Visible = false;
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
                case "Not installed":
                    connect.Sensitive = false;
                    disconnect.Visible = false;
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
            }
            
        }
        
    }

}
