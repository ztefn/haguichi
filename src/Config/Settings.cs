/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2012 Stephen Brandt <stephen@stephenbrandt.com>
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
using System.Collections;


namespace Config
{
    
    public static class Settings
    {
        
        public static string LocalePath                     = System.AppDomain.CurrentDomain.BaseDirectory + "../../share/locale";
        public static string DefaultHamachiDataPath         = Environment.GetFolderPath ( Environment.SpecialFolder.Personal ) + "/.hamachi";
        public static string ConfPath                       = "/apps/" + TextStrings.appName.ToLower ();
        
        public static bool Debugging                        = false;
        public static bool DemoMode                         = false;
        public static bool WinMinimized                     = false;
        public static bool WinMaximized                     = false;
        
        public static string [] DefaultCommands             = { "true;true;folder-remote;_Browse Shares;nautilus smb://%A/;nautilus smb://[%A]/;IPv4",
                                                                "true;false;preferences-desktop-remote-desktop;_View Remote Desktop;vinagre %A;vinagre [%A];IPv4",
                                                                "true;false;utilities-terminal;_Ping;gnome-terminal -x ping %A;gnome-terminal -x ping6 %A;IPv4" };
        
        public static string [] SessionDefaultCommands;
        
        public static Key ConnectOnStartup                  = new Key ( "behavior/connect_on_startup", false );
        public static Key ReconnectOnConnectionLoss         = new Key ( "behavior/reconnect_on_connection_loss", true );
        public static Key DisconnectOnQuit                  = new Key ( "behavior/disconnect_on_quit", true );
        public static Key GoOnlineInNewNetwork              = new Key ( "behavior/go_online_in_new_network", true );
        public static Key StartInTray                       = new Key ( "behavior/start_in_tray", false );
        public static Key CustomCommands                    = new Key ( "commands/customizable", DefaultCommands );
        public static Key CommandForSuperUser               = new Key ( "commands/super_user", "gksudo" );
        public static Key CommandForTunCfg                  = new Key ( "commands/tuncfg", "/sbin/tuncfg" );
        public static Key Nickname                          = new Key ( "config/nickname", "" );
        public static Key Protocol                          = new Key ( "config/protocol", "both" );
        public static Key HamachiDataPath                   = new Key ( "config/hamachi_data_path", DefaultHamachiDataPath );
        public static Key CommandTimeout                    = new Key ( "config/command_timeout", 60.0 );
        public static Key ReconnectInterval                 = new Key ( "config/reconnect_interval", 30.0 );
        public static Key UpdateInterval                    = new Key ( "config/update_interval", 15.0 );
        public static Key CollapsedNetworks                 = new Key ( "main_window/collapsed_networks", new string [] {} );
        public static Key WinHeight                         = new Key ( "main_window/height", 480 );
        public static Key MemberTemplateSmall               = new Key ( "main_window/member_template_small", "%N%_*" );
        public static Key MemberTemplateLarge               = new Key ( "main_window/member_template_large", "%N%_*<br><span size=\"smaller\">%S <i>%A</i></span>" );
        public static Key NetworkListIconSizeSmall          = new Key ( "main_window/network_list_icon_size_small", 12 );
        public static Key NetworkListIconSizeLarge          = new Key ( "main_window/network_list_icon_size_large", 18 );
        public static Key NetworkListLayout                 = new Key ( "main_window/network_list_layout", "normal" );
        public static Key NetworkTemplateSmall              = new Key ( "main_window/network_template_small", "<b>%N</b> (%O/%T)%_*" );
        public static Key NetworkTemplateLarge              = new Key ( "main_window/network_template_large", "<b>%N</b> (%O/%T)%_*<br><span size=\"smaller\">%S</span>" );
        public static Key ShowAlternatingRowColors          = new Key ( "main_window/show_alternating_row_colors", false );
        public static Key ShowOfflineMembers                = new Key ( "main_window/show_offline_members", true );
        public static Key ShowStatusbar                     = new Key ( "main_window/show_statusbar", true );
        public static Key ShowTrayIcon                      = new Key ( "main_window/show_tray_icon", true );
        public static Key SortNetworkListBy                 = new Key ( "main_window/sort_by", "name" );
        public static Key WinWidth                          = new Key ( "main_window/width", 250 );
        public static Key WinX                              = new Key ( "main_window/x_pos", 100 );
        public static Key WinY                              = new Key ( "main_window/y_pos", 90 );
        public static Key NotifyOnConnectionLoss            = new Key ( "notifications/connection_loss", true );
        public static Key NotifyOnMemberJoin                = new Key ( "notifications/member_join", true );
        public static Key NotifyOnMemberLeave               = new Key ( "notifications/member_leave", true );
        public static Key NotifyOnMemberOffline             = new Key ( "notifications/member_offline", true );
        public static Key NotifyOnMemberOnline              = new Key ( "notifications/member_online", true );
        
        
        public static void Init ()
        {
            
            if ( Environment.GetEnvironmentVariable ( "KDE_FULL_SESSION" ) == "true" )
            {
                SessionDefaultCommands = new string [] { "true;true;folder-remote;_Browse Shares;dolphin smb://%A/;dolphin smb://[%A]/;IPv4",
                                                         "true;false;preferences-desktop-remote-desktop;_View Remote Desktop;krdc %A;krdc [%A];IPv4",
                                                         "true;false;utilities-terminal;_Ping;konsole -e ping %A;konsole -e ping6 %A;IPv4" };
                
                Debug.Log ( Debug.Domain.Environment, "Settings.Init", "Default commands for this session: KDE" );
            }
            else
            {
                SessionDefaultCommands = DefaultCommands;
                
                Debug.Log ( Debug.Domain.Environment, "Settings.Init", "Default commands for this session: GNOME" );
            }
            
        }
        
    }

}
