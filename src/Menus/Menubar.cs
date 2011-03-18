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
    
    public class Menubar : MenuBar
    {
        
        private Menu clientMenu;
        private Menu editMenu;
        private Menu viewMenu;
        private Menu helpMenu;
        
        
        private MenuItem clientMenuItem;
        private MenuItem editMenuItem;
        private MenuItem viewMenuItem;
        private MenuItem helpMenuItem;
        
        
        private ImageMenuItem configure;
        private ImageMenuItem connect;
        private ImageMenuItem disconnect;
        private ImageMenuItem change;
        private ImageMenuItem join;
        private ImageMenuItem create;
        private ImageMenuItem attach;
        private ImageMenuItem info;
        private ImageMenuItem close;
        private ImageMenuItem quit;
        
        private ImageMenuItem preferences;
        
        private ImageMenuItem update;
        public  CheckMenuItem showStatusbar;
        public  CheckMenuItem showAlternatingRowColors;
        private RadioMenuItem layoutGroup;
        public  RadioMenuItem layoutNormal;
        public  RadioMenuItem layoutLarge;
        public  CheckMenuItem showOfflineMembers;
        private RadioMenuItem sortGroup;
        public  RadioMenuItem sortByName;
        public  RadioMenuItem sortByStatus;
        
        private ImageMenuItem help;
        private ImageMenuItem about;
        
        
        public Menubar ()
        {
            
            configure = new ImageMenuItem ( TextStrings.configureLabel );
            configure.Activated += GlobalEvents.ConfigureHamachi;
            
            connect = new ImageMenuItem ( Stock.Connect, MainWindow.accelGroup );
            connect.Activated += GlobalEvents.StartHamachi;
            connect.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.O, Gdk.ModifierType.ControlMask, AccelFlags.Visible ) );
            
            disconnect = new ImageMenuItem ( Stock.Disconnect, MainWindow.accelGroup );
            disconnect.Activated += GlobalEvents.StopHamachi;
            disconnect.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.D, Gdk.ModifierType.ControlMask, AccelFlags.Visible ) );
            
            change = new ImageMenuItem ( TextStrings.changeNickLabel );
            change.Activated += GlobalEvents.ChangeNick;
            
            join = new ImageMenuItem ( TextStrings.joinNetworkLabel );
            join.Activated += GlobalEvents.JoinNetwork;
            
            create = new ImageMenuItem ( TextStrings.createNetworkLabel );
            create.Activated += GlobalEvents.CreateNetwork;
            
            attach = new ImageMenuItem ( TextStrings.attachMenuLabel );
            attach.Activated += GlobalEvents.Attach;
            
            info = new ImageMenuItem ( Stock.Info, MainWindow.accelGroup );
            info.Activated += GlobalEvents.Information;
            info.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.F2, Gdk.ModifierType.None, AccelFlags.Visible ) );
            
            close = new ImageMenuItem ( Stock.Close, MainWindow.accelGroup );
            close.Activated += MainWindow.Hide;
            close.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.W, Gdk.ModifierType.ControlMask, AccelFlags.Visible ) );
            
            quit = new ImageMenuItem ( Stock.Quit, MainWindow.accelGroup );
            quit.Activated += GlobalEvents.QuitApp;
            
            clientMenu = new Menu ();
            clientMenu.Append ( configure );
            clientMenu.Append ( connect );
            clientMenu.Append ( disconnect );
            clientMenu.Add    ( new SeparatorMenuItem () );
            clientMenu.Append ( change );
            clientMenu.Append ( join );
            clientMenu.Append ( create );
            clientMenu.Append ( attach );
            clientMenu.Add    ( new SeparatorMenuItem () );
            clientMenu.Append ( info );
            clientMenu.Add    ( new SeparatorMenuItem() );
            clientMenu.Append ( close );
            clientMenu.Append ( quit );
            
            clientMenuItem = new MenuItem ( TextStrings.clientLabel );
            clientMenuItem.Submenu = clientMenu;
            
            
            preferences = new ImageMenuItem ( Stock.Preferences, MainWindow.accelGroup );
            preferences.Activated += GlobalEvents.Preferences;

            editMenu = new Menu ();
            editMenu.Append ( preferences );
            
            editMenuItem = new MenuItem ( TextStrings.editLabel );
            editMenuItem.Submenu = editMenu;
            
            
            update = new ImageMenuItem ( TextStrings.updateLabel );
            update.Image = new Image ( Stock.Refresh, IconSize.Menu );
            update.Activated += Controller.UpdateConnection;
            update.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.F5, Gdk.ModifierType.None, AccelFlags.Visible ) );
            
            showStatusbar = new CheckMenuItem ( TextStrings.checkboxShowStatusbar );
            showStatusbar.Active = (bool) Config.Client.Get ( Config.Settings.ShowStatusbar );
            showStatusbar.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.ShowStatusbar, showStatusbar.Active );
            };
            
            showAlternatingRowColors = new CheckMenuItem ( TextStrings.checkboxShowAlternatingRowColors );
            showAlternatingRowColors.Active = (bool) Config.Client.Get ( Config.Settings.ShowAlternatingRowColors );
            showAlternatingRowColors.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.ShowAlternatingRowColors, showAlternatingRowColors.Active );
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
            sortByName.Toggled += ChangeSortBy;
            
            layoutGroup = new RadioMenuItem ( "layout" );
            
            layoutNormal = new RadioMenuItem ( layoutGroup, TextStrings.layoutNormal );
            layoutLarge = new RadioMenuItem ( layoutGroup, TextStrings.layoutLarge );
            if ( ( string ) Config.Client.Get( Config.Settings.NetworkListLayout ) == "large" )
            {
                layoutLarge.Active = true;
            }
            else
            {
                layoutNormal.Active = true;
            }
            layoutNormal.Toggled += ChangeLayout;
            
            viewMenu = new Menu ();
            viewMenu.Append ( update );
            viewMenu.Add    ( new SeparatorMenuItem () );
            viewMenu.Append ( layoutNormal );
            viewMenu.Append ( layoutLarge );
            viewMenu.Add    ( new SeparatorMenuItem () );
            viewMenu.Append ( sortByName );
            viewMenu.Append ( sortByStatus );
            viewMenu.Add    ( new SeparatorMenuItem () );
            viewMenu.Append ( showOfflineMembers );
            viewMenu.Append ( showAlternatingRowColors );
            viewMenu.Add    ( new SeparatorMenuItem () );
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
            
            
            this.Append ( clientMenuItem );
            this.Append ( editMenuItem );
            this.Append ( viewMenuItem );
            this.Append ( helpMenuItem );
            
            this.ShowAll ();
            
        }
        
        
        public void SetClose ( bool visible )
        {
            
            close.Visible = visible;
            
        }
        
        
        public void SetAttach ( bool visible, bool sensitive )
        {
            
            attach.Visible   = visible;
            attach.Sensitive = sensitive;
            
        }
        
        
        public void SetMode ( string mode )
        {
            
            SetClose ( ( bool ) Config.Client.Get ( Config.Settings.ShowTrayIcon ) );
            
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
                    update.Sensitive = true;
                
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
                    update.Sensitive = false;
                
                    break;
                
                case "Not configured":
                    configure.Show ();
                    connect.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                    update.Sensitive = false;
                
                    break;
                
                case "Not installed":
                    configure.Hide ();
                    connect.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    change.Sensitive = false;
                    join.Sensitive = false;
                    create.Sensitive = false;
                    info.Sensitive = true;
                    update.Sensitive = false;
                
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
        
        
        private void ChangeLayout ( object obj, EventArgs args )
        {
            
            if ( layoutLarge.Active )
            {
                Config.Client.Set ( Config.Settings.NetworkListLayout, "large" );
            }
            else
            {
                Config.Client.Set ( Config.Settings.NetworkListLayout, "normal" );
            }
            
        }
        
    }
    
}
