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
using Gtk;


namespace Menus
{
    
    public class Menubar : MenuBar
    {
        
        private Menu clientMenu;
        private Menu configMenu;
        private Menu editMenu;
        private Menu viewMenu;
        private Menu helpMenu;
        
        
        private MenuItem clientMenuItem;
        private MenuItem editMenuItem;
        private MenuItem viewMenuItem;
        private MenuItem helpMenuItem;
        
        
        private ImageMenuItem connect;
        private ImageMenuItem disconnect;
        private ImageMenuItem config;
        private ImageMenuItem open;
        private ImageMenuItem save;
        private ImageMenuItem restore;
        private ImageMenuItem join;
        private ImageMenuItem create;
        private ImageMenuItem change;
        private ImageMenuItem attach;
        private ImageMenuItem info;
        private ImageMenuItem close;
        private ImageMenuItem quit;
        
        private ImageMenuItem find;
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
        
        
        private FileFilter tar;
        private FileChooserDialog chooser;
        
        
        public Menubar ()
        {
            
            tar = new FileFilter ();
            tar.Name = TextStrings.configFileFilterTitle;
            tar.AddMimeType ( "application/x-tar" );
            tar.AddMimeType ( "application/x-compressed-tar" );
            tar.AddMimeType ( "application/x-bzip-compressed-tar" );
            tar.AddMimeType ( "application/x-lzma-compressed-tar" );
            tar.AddMimeType ( "application/x-xz-compressed-tar" );
            
            
            connect = new ImageMenuItem ( Stock.Connect, MainWindow.accelGroup );
            connect.Activated += GlobalEvents.StartHamachi;
            connect.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.O, Gdk.ModifierType.ControlMask, AccelFlags.Visible ) );
            
            disconnect = new ImageMenuItem ( Stock.Disconnect, MainWindow.accelGroup );
            disconnect.Activated += GlobalEvents.StopHamachi;
            disconnect.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.D, Gdk.ModifierType.ControlMask, AccelFlags.Visible ) );
            
            open = new ImageMenuItem ( TextStrings.configFolderLabel );
            open.Image = new Image ( Stock.Open, IconSize.Menu );
            open.Activated += delegate
            {
                Command.OpenURL ( Hamachi.DataPath );
            };
            
            save = new ImageMenuItem ( TextStrings.configSaveLabel );
            save.Image = new Image ( Stock.Save, IconSize.Menu );
            save.Activated += SaveBackup;
            
            restore = new ImageMenuItem ( TextStrings.configRestoreLabel );
            restore.Activated += RestoreBackup;
            
            configMenu = new Menu ();
            configMenu.Add ( open );
            configMenu.Add ( new SeparatorMenuItem () );
            configMenu.Add ( save );
            configMenu.Add ( restore );
            
            config = new ImageMenuItem ( TextStrings.configLabel );
            config.Submenu = configMenu;
            
            join = new ImageMenuItem ( TextStrings.joinNetworkLabel );
            join.Activated += GlobalEvents.JoinNetwork;
            
            create = new ImageMenuItem ( TextStrings.createNetworkLabel );
            create.Activated += GlobalEvents.CreateNetwork;
            
            change = new ImageMenuItem ( TextStrings.changeNickLabel );
            change.Activated += GlobalEvents.ChangeNick;
            
            attach = new ImageMenuItem ( TextStrings.attachLabel );
            attach.Activated += GlobalEvents.Attach;
            
            info = new ImageMenuItem ( Stock.Info, MainWindow.accelGroup );
            info.Activated += GlobalEvents.Information;
            info.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.F2, Gdk.ModifierType.None, AccelFlags.Visible ) );
            
            close = new ImageMenuItem ( Stock.Close, MainWindow.accelGroup );
            close.Activated += MainWindow.Hide;
            
            quit = new ImageMenuItem ( Stock.Quit, MainWindow.accelGroup );
            quit.Activated += GlobalEvents.QuitApp;
            
            clientMenu = new Menu ();
            clientMenu.Append ( connect );
            clientMenu.Append ( disconnect );
            clientMenu.Add    ( new SeparatorMenuItem () );
            clientMenu.Append ( config );
            clientMenu.Add    ( new SeparatorMenuItem () );
            clientMenu.Append ( join );
            clientMenu.Append ( create );
            clientMenu.Add    ( new SeparatorMenuItem () );
            clientMenu.Append ( change );
            clientMenu.Append ( attach );
            clientMenu.Add    ( new SeparatorMenuItem() );
            clientMenu.Append ( info );
            clientMenu.Add    ( new SeparatorMenuItem() );
            clientMenu.Append ( close );
            clientMenu.Append ( quit );
            
            clientMenuItem = new MenuItem ( TextStrings.clientLabel );
            clientMenuItem.Submenu = clientMenu;
            
            
            find = new ImageMenuItem ( Stock.Find, MainWindow.accelGroup );
            find.Activated += delegate
            {
                MainWindow.searchBar.Show ();
                MainWindow.searchEntry.GrabFocus ();
            };
            
            preferences = new ImageMenuItem ( Stock.Preferences, MainWindow.accelGroup );
            preferences.Activated += GlobalEvents.Preferences;
            preferences.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.P, Gdk.ModifierType.ControlMask, AccelFlags.Visible ) );
            
            editMenu = new Menu ();
            editMenu.Append ( find );
            editMenu.Add    ( new SeparatorMenuItem () );
            editMenu.Append ( preferences );
            
            editMenuItem = new MenuItem ( TextStrings.editLabel );
            editMenuItem.Submenu = editMenu;
            
            
            update = new ImageMenuItem ( TextStrings.updateLabel );
            update.Image = new Image ( Stock.Refresh, IconSize.Menu );
            update.Activated += Controller.UpdateConnection;
            update.AddAccelerator ( "activate", MainWindow.accelGroup, new AccelKey ( Gdk.Key.F5, Gdk.ModifierType.None, AccelFlags.Visible ) );
            
            showStatusbar = new CheckMenuItem ( TextStrings.checkboxShowStatusbar );
            showStatusbar.Active = ( bool ) Config.Settings.ShowStatusbar.Value;
            showStatusbar.Toggled += delegate
            {
                Config.Settings.ShowStatusbar.Value = showStatusbar.Active;
            };
            
            showAlternatingRowColors = new CheckMenuItem ( TextStrings.checkboxShowAlternatingRowColors );
            showAlternatingRowColors.Active = ( bool ) Config.Settings.ShowAlternatingRowColors.Value;
            showAlternatingRowColors.Toggled += delegate
            {
                Config.Settings.ShowAlternatingRowColors.Value = showAlternatingRowColors.Active;
            };
            
            showOfflineMembers = new CheckMenuItem ( TextStrings.checkboxShowOfflineMembers );
            showOfflineMembers.Active = ( bool ) Config.Settings.ShowOfflineMembers.Value;
            showOfflineMembers.Toggled += delegate
            {
                Config.Settings.ShowOfflineMembers.Value = showOfflineMembers.Active;
            };
            
            sortGroup = new RadioMenuItem ( "sort" );
            
            sortByName = new RadioMenuItem ( sortGroup, TextStrings.sortByName );
            sortByStatus = new RadioMenuItem ( sortGroup, TextStrings.sortByStatus );
            if ( ( string ) Config.Settings.SortNetworkListBy.Value == "status" )
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
            if ( ( string ) Config.Settings.NetworkListLayout.Value == "large" )
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
        
        
        public void SetConfig ()
        {
            
            if ( System.IO.Directory.Exists ( Hamachi.DataPath ) )
            {
                open.Sensitive = true;
                save.Sensitive = true;
            }
            else
            {
                open.Sensitive = false;
                save.Sensitive = false;
            }
            
        }
        
        
        public void SetMode ( string mode )
        {
            
            SetClose ( ( bool ) Config.Settings.ShowTrayIcon.Value );
            
            join.Sensitive   = false;
            create.Sensitive = false;
            find.Sensitive   = false;
            update.Sensitive = false;
            
            
            switch ( mode )
            {
                
                case "Connecting":
                    
                    connect.Sensitive = false;
                    
                    break;
                    
                case "Connected":
                    
                    connect.Hide ();
                    disconnect.Show ();
                    
                    join.Sensitive   = true;
                    create.Sensitive = true;
                    find.Sensitive   = true;
                    update.Sensitive = true;
                    
                    break;
                    
                case "Disconnected":
                    
                    connect.Show ();
                    connect.Sensitive = true;
                    disconnect.Hide ();
                    
                    break;
                    
                case "Not configured":
                    
                    connect.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    
                    break;
                    
                case "Not installed":
                    
                    connect.Show ();
                    connect.Sensitive = false;
                    disconnect.Hide ();
                    
                    break;
                    
            }
            
        }
        
        
        private void SaveBackup ( object obj, EventArgs args )
        {
            
            chooser = new FileChooserDialog ( TextStrings.configSaveTitle, Haguichi.mainWindow.ReturnWindow (), FileChooserAction.Save, Stock.Cancel, ResponseType.Cancel, Stock.Save, ResponseType.Accept );
            chooser.Modal = true;
            chooser.DoOverwriteConfirmation = true;
            chooser.AddFilter ( tar );
            chooser.SetCurrentFolder ( Environment.GetFolderPath ( Environment.SpecialFolder.Personal ) );
            chooser.CurrentName = "logmein-hamachi-config_" + DateTime.Now.ToString("yyyy-MM-dd") + ".tar.gz";
            chooser.ShowAll ();
            
            chooser.Response += delegate ( object o, ResponseArgs a)
            {
                if ( a.ResponseId == ResponseType.Accept )
                {
                    Hamachi.SaveBackup ( chooser.Filename );
                }
                
                chooser.Destroy ();
            };
            
        }
        
        
        private void RestoreBackup ( object obj, EventArgs args )
        {
            
            chooser = new FileChooserDialog ( TextStrings.configRestoreTitle, Haguichi.mainWindow.ReturnWindow (), FileChooserAction.Open, Stock.Cancel, ResponseType.Cancel, TextStrings.configRestoreButtonLabel, ResponseType.Accept );
            chooser.Modal = true;
            chooser.AddFilter ( tar );
            chooser.SetCurrentFolder ( Environment.GetFolderPath ( Environment.SpecialFolder.Personal ) );
            chooser.ShowAll ();
            
            chooser.Response += delegate ( object o, ResponseArgs a)
            {
                chooser.Hide ();
                
                if ( a.ResponseId == ResponseType.Accept )
                {
                    Hamachi.RestoreBackup ( chooser.Filename );
                }
                
                chooser.Destroy ();
            };
            
        }
        
        
        private void ChangeSortBy ( object obj, EventArgs args )
        {
            
            if ( sortByStatus.Active )
            {
                Config.Settings.SortNetworkListBy.Value = "status";
            }
            else
            {
                Config.Settings.SortNetworkListBy.Value = "name";
            }
            
        }
        
        
        private void ChangeLayout ( object obj, EventArgs args )
        {
            
            if ( layoutLarge.Active )
            {
                Config.Settings.NetworkListLayout.Value = "large";
            }
            else
            {
                Config.Settings.NetworkListLayout.Value = "normal";
            }
            
        }
        
    }
    
}
