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


namespace Menus
{

    public class Quick : Menu
    {
    
        private AccelGroup accelGroup;
        
        private CheckMenuItem show;
        private ImageMenuItem connect;
        private ImageMenuItem disconnect;
        private ImageMenuItem join;
        private ImageMenuItem create;
        private ImageMenuItem info;
        private ImageMenuItem quit;
        
        
        public Quick ()
        {
            
            show = new CheckMenuItem ( TextStrings.showApp );
            show.Toggled += delegate {
                if ( show.Active )
                {
                    MainWindow.Show ();
                }
                else
                {
                    MainWindow.Hide ();
                }
            };
            
            connect = new ImageMenuItem ( Stock.Connect, accelGroup );
            connect.Activated += GlobalEvents.StartHamachi;
            
            disconnect = new ImageMenuItem ( Stock.Disconnect, accelGroup );
            disconnect.Activated += GlobalEvents.StopHamachi;
            
            join = new ImageMenuItem ( TextStrings.joinNetworkLabel );
            join.Activated += GlobalEvents.JoinNetwork;
            
            create = new ImageMenuItem ( TextStrings.createNetworkLabel );
            create.Activated += GlobalEvents.CreateNetwork;
            
            info = new ImageMenuItem ( Stock.Info, accelGroup );
            info.Activated += GlobalEvents.Information;
            
            quit = new ImageMenuItem ( Stock.Quit, accelGroup );
            quit.Activated += GlobalEvents.QuitApp;
            
            
            this.Add ( show );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( connect );
            this.Add ( disconnect );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( join );
            this.Add ( create );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( info );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( quit );
            
            this.ShowAll ();

        }
        
        
        public void SetVisibility ( bool visible )
        {
            
            show.Active = visible;
            
        }
        
        
        public void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                
                case "Connecting":
                    connect.Sensitive    = false;
                
                    break;
                    
                case "Connected":
                    connect.Hide ();
                    disconnect.Show ();
                    disconnect.Sensitive = true;
                    join.Sensitive       = true;
                    create.Sensitive     = true;
                
                    break;
                    
                case "Disconnected":
                    connect.Show ();
                    connect.Sensitive    = true;
                    disconnect.Hide ();
                    join.Sensitive       = false;
                    create.Sensitive     = false;
                
                    break;
                
                case "Not configured":
                    connect.Sensitive    = false;
                    disconnect.Hide ();
                    join.Sensitive       = false;
                    create.Sensitive     = false;
                
                    break;
                
                case "Not installed":
                    connect.Sensitive    = false;
                    disconnect.Hide ();
                    join.Sensitive       = false;
                    create.Sensitive     = false;
                
                    break;
                
            }
            
        }
        
    }

}
