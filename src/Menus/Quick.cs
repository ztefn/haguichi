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
        
        private ImageMenuItem configure;
        private ImageMenuItem connect;
        private ImageMenuItem disconnect;
        private ImageMenuItem change;
        private ImageMenuItem join;
        private ImageMenuItem create;
        private ImageMenuItem attach;
        private ImageMenuItem info;
        private ImageMenuItem preferences;
        private ImageMenuItem about;
        private ImageMenuItem quit;
        
        
        public Quick()
        {
            
            configure = new ImageMenuItem ( TextStrings.configureLabel );
            configure.Activated += GlobalEvents.ConfigureHamachi;
            
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
            
            attach = new ImageMenuItem ( TextStrings.attachMenuLabel );
            attach.Activated += GlobalEvents.Attach;
            
            info = new ImageMenuItem ( Stock.Info, accelGroup );
            info.Activated += GlobalEvents.Information;
            
            preferences = new ImageMenuItem ( Stock.Preferences, accelGroup );
            preferences.Activated += GlobalEvents.Preferences;
            
            about = new ImageMenuItem ( Stock.About, accelGroup );
            about.Activated += GlobalEvents.About;
            
            quit = new Gtk.ImageMenuItem ( Stock.Quit, accelGroup );
            quit.Activated += GlobalEvents.QuitApp;
            
            
            this.Add ( configure );
            this.Add ( connect );
            this.Add ( disconnect );
            this.Add ( info );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( change );
            this.Add ( join );
            this.Add ( create );
            this.Add ( attach );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( preferences );
            this.Add ( about );
            this.Add ( new SeparatorMenuItem () );
            this.Add ( quit );
            
            this.ShowAll ();

        }
        
        
        public void SetAttach ( bool visible, bool sensitive )
        {
            attach.Visible   = visible;
            attach.Sensitive = sensitive;
        }
        
        
        public void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                case "Connecting":
                    connect.Sensitive = false;
                
                    break;
                    
                case "Connected":
                    configure.Hide ();
                    connect.Hide ();
                    disconnect.Show ();
                    change.Sensitive = true;
                    join.Sensitive = true;
                    create.Sensitive = true;
                    info.Sensitive = true;
                
                    break;
                    
                case "Disconnected":
                    configure.Hide ();
                    connect.Show ();
                    connect.Sensitive = true;
                    disconnect.Hide ();
                    change.Sensitive = true;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
                case "Not configured":
                    configure.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
                case "Not installed":
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
            }
            
        }
        
    }

}
