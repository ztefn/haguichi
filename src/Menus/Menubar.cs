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
    
    public class Menubar : MenuBar
    {
        
        private Menu accountMenu;
        private Menu editMenu;
        private Menu viewMenu;
        private Menu helpMenu;
        
        
        private MenuItem accountMenuItem;
        private MenuItem editMenuItem;
        private MenuItem viewMenuItem;
        private MenuItem helpMenuItem;
        
        
        private ImageMenuItem connect;
        private ImageMenuItem disconnect;
        private ImageMenuItem change;
        private ImageMenuItem join;
        private ImageMenuItem create;
        private ImageMenuItem info;
        private ImageMenuItem quit;
        
        private ImageMenuItem preferences;
        
        public  CheckMenuItem showStatusbar;
        public  CheckMenuItem showOfflineMembers;
        private RadioMenuItem sortGroup;
        public  RadioMenuItem sortByName;
        public  RadioMenuItem sortByStatus;
        
        private ImageMenuItem help;
        private ImageMenuItem about;
        
        
        public Menubar()
        {
            
            connect = new ImageMenuItem ( Stock.Connect, MainWindow.accelGroup );
            connect.Activated += GlobalEvents.StartHamachi;
            
            disconnect = new ImageMenuItem ( Stock.Disconnect, MainWindow.accelGroup );
            disconnect.Activated += GlobalEvents.StopHamachi;
            
            change = new ImageMenuItem ( TextStrings.changeNickLabel );
            change.Activated += GlobalEvents.ChangeNick;
            
            join = new ImageMenuItem ( TextStrings.joinNetworkLabel );
            join.Activated += GlobalEvents.JoinNetwork;
            
            create = new ImageMenuItem ( TextStrings.createNetworkLabel );
            create.Activated += GlobalEvents.CreateNetwork;
            
            info = new ImageMenuItem ( Stock.Info, MainWindow.accelGroup );
            info.Activated += GlobalEvents.Information;
            
            quit = new ImageMenuItem ( Stock.Quit, MainWindow.accelGroup );
            quit.Activated += GlobalEvents.QuitApp;
            
            accountMenu = new Menu ();
            accountMenu.Append ( connect );
            accountMenu.Append ( disconnect );
            accountMenu.Append ( change );
            accountMenu.Append ( join );
            accountMenu.Append ( create );
            accountMenu.Append ( info );
            accountMenu.Add    ( new SeparatorMenuItem() );
            accountMenu.Append ( quit );
            
            accountMenuItem = new MenuItem ( TextStrings.accountLabel );
            accountMenuItem.Submenu = accountMenu;
            
            
            preferences = new ImageMenuItem ( Stock.Preferences, MainWindow.accelGroup );
            preferences.Activated += GlobalEvents.Preferences;

            editMenu = new Menu ();
            editMenu.Append ( preferences );
            
            editMenuItem = new MenuItem ( TextStrings.editLabel );
            editMenuItem.Submenu = editMenu;
            
            
            showStatusbar = new CheckMenuItem ( TextStrings.checkboxShowStatusbar );
            showStatusbar.Active = (bool) Config.Client.Get ( Config.Settings.ShowStatusbar );
            showStatusbar.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.ShowStatusbar, showStatusbar.Active );
            };
            
            showOfflineMembers = new CheckMenuItem ( TextStrings.checkboxShowOfflineMembers );
            showOfflineMembers.Active = ( bool ) Config.Client.Get ( Config.Settings.ShowOfflineMembers );
            showOfflineMembers.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.ShowOfflineMembers, showOfflineMembers.Active );
            };
            
            sortGroup = new RadioMenuItem ( "sort" );
            
            sortByName = new RadioMenuItem ( sortGroup, TextStrings.sortByName );
            sortByStatus = new RadioMenuItem ( sortGroup, TextStrings.sortByStatus );
            if ( ( string ) Config.Client.Get( Config.Settings.SortNetworkListBy ) == "status" )
            {
                sortByStatus.Active = true;
            }
            else
            {
                sortByName.Active = true;
            }
            sortByName.Activated += ChangeSortBy;
            sortByStatus.Activated += ChangeSortBy;
            
            viewMenu = new Menu ();
            viewMenu.Append ( showOfflineMembers );
            viewMenu.Add    ( new SeparatorMenuItem() );
            viewMenu.Append ( sortByName );
            viewMenu.Append ( sortByStatus );
            viewMenu.Add    ( new SeparatorMenuItem() );
            viewMenu.Append ( showStatusbar );
            
            viewMenuItem = new MenuItem ( TextStrings.viewLabel );
            viewMenuItem.Submenu = viewMenu;
            
            help = new ImageMenuItem ( TextStrings.onlineHelpLabel );
            help.Image = new Image ( Stock.Help, IconSize.Menu );
            help.Activated += GlobalEvents.Help;
            help.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.F1, Gdk.ModifierType.None, AccelFlags.Visible ) );
                    
            about = new ImageMenuItem ( Stock.About, MainWindow.accelGroup );
            about.Activated += GlobalEvents.About;
            
            helpMenu = new Menu ();
            helpMenu.Append ( help );
            helpMenu.Append ( about );
            
            helpMenuItem = new MenuItem ( TextStrings.helpLabel );
            helpMenuItem.Submenu = helpMenu;
            
            
            this.Append ( accountMenuItem );
            this.Append ( editMenuItem );
            this.Append ( viewMenuItem );
            this.Append ( helpMenuItem );
            
            this.ShowAll();
            
        }
        
        
        public void SetMode ( string mode )
        {
            
            switch ( mode )
            {
                case "Connecting":
                    // Nothing todo
                
                    break;
                    
                case "Connected":
                    connect.Hide ();
                    disconnect.Show ();
                    change.Sensitive = true;
                    join.Sensitive = true;
                    create.Sensitive = true;
                    info.Sensitive = true;
                
                    break;
                    
                case "Disconnected":
                    connect.Show ();
                    connect.Sensitive = true;
                    disconnect.Hide ();
                    change.Sensitive = true;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
                case "Not configured":
                    connect.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
                case "Not installed":
                    connect.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                
                    break;
                
            }
            
        }
        
        
        private void ChangeSortBy ( object obj, EventArgs args )
        {
            
            if ( sortByStatus.Active )
            {
                Config.Client.Set ( Config.Settings.SortNetworkListBy, "status" );
            }
            else
            {
                Config.Client.Set ( Config.Settings.SortNetworkListBy, "name" );
            }
            
        }
        
    }
    
}
