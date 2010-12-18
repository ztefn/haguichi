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
using GConf;


namespace Config
{

    public static class Client
    {

        private static GConf.Client client;
        
        
        public static void Init ()
        {
            
            /* Preventing segfault when configuration server is not started, fix from https://bugzilla.gnome.org/show_bug.cgi?id=593561 */
            GLib.GType.Init ();
            
            client = new GConf.Client ();
            client.AddNotify ( Config.Settings.ConfPath, new NotifyEventHandler ( ValueChanged ) );
            
        }
        
        
        private static void ValueChanged ( object sender, NotifyEventArgs args )
        {
            
            string key = args.Key;
            object val = args.Value;
            
            try
            {
                
                Debug.Log ( Debug.Domain.Info, "Config.Client.ValueChanged", "Updating GUI for GConf key " + key );
                
                if ( key.Contains ( Config.Settings.NetworkTemplate.KeyName ) )
                {
                    MainWindow.networkView.networkTemplate = ( string ) val;
                }
                
                if ( key.Contains ( Config.Settings.MemberTemplate.KeyName ) )
                {
                    MainWindow.networkView.memberTemplate = ( string ) val;
                }
                
                if ( key.Contains ( Config.Settings.HamachiDataPath.KeyName ) )
                {
                    Haguichi.preferencesWindow.pathButton.SetCurrentFolder ( ( string ) val );
                }
                
                if ( key.Contains ( Config.Settings.UpdateInterval.KeyName ) )
                {
                    Haguichi.preferencesWindow.intervalSpin.Value = ( int ) ( ( double ) val );
                    Haguichi.preferencesWindow.SetIntervalString ();
                }
                
                if ( key.Contains ( Config.Settings.ConnectOnStartup.KeyName ) )
                {
                    Haguichi.preferencesWindow.connectOnStartup.Active = ( bool ) val;
                    MainWindow.autoconnectCheckbox.Active = ( bool ) val;
                }
                
                if ( key.Contains ( Config.Settings.ReconnectOnConnectionLoss.KeyName ) )
                {
                    Haguichi.preferencesWindow.reconnectOnConnectionLoss.Active = ( bool ) val;
                }
                
                if ( key.Contains ( Config.Settings.DisconnectOnQuit.KeyName ) )
                {
                    Haguichi.preferencesWindow.disconnectOnQuit.Active = ( bool ) val;
                }
                
                if ( key.Contains ( Config.Settings.AskBeforeRunningTunCfg.KeyName ) )
                {
                    Haguichi.preferencesWindow.askTunCfg.Active = ( bool ) val;
                }
                
                if ( key.Contains ( Config.Settings.NotifyOnMemberJoin.KeyName ) )
                {
                    Haguichi.preferencesWindow.notifyOnMemberJoin.Active = ( bool ) val;
                    
                    if ( Config.Settings.DemoMode )
                    {
                        Controller.NotifyMemberJoined ( "T-800", "Skynet" ); 
                    }
                }
                
                if ( key.Contains ( Config.Settings.NotifyOnMemberLeave.KeyName ) )
                {
                    Haguichi.preferencesWindow.notifyOnMemberLeave.Active = ( bool ) val;
                    
                    if ( Config.Settings.DemoMode )
                    {
                        Controller.NotifyMemberLeft ( "T-800", "Skynet" );
                    }
                }
                
                if ( key.Contains ( Config.Settings.NotifyOnMemberOnline.KeyName ) )
                {
                    Haguichi.preferencesWindow.notifyOnMemberOnline.Active = ( bool ) val;
                    
                    if ( Config.Settings.DemoMode )
                    {
                        Controller.NotifyMemberOnline ( "T-800", "Skynet" );
                    }
                }
                
                if ( key.Contains ( Config.Settings.NotifyOnMemberOffline.KeyName ) )
                {
                    Haguichi.preferencesWindow.notifyOnMemberOffline.Active = ( bool ) val;
                    
                    if ( Config.Settings.DemoMode )
                    {
                        Controller.NotifyMemberOffline ( "T-800", "Skynet" );   
                    }
                }
                
                if ( key.Contains ( Config.Settings.ShowTrayIcon.KeyName ) )
                {
                    Haguichi.preferencesWindow.showTrayIcon.Active = ( bool ) val;
                }
                
                if ( key.Contains ( Config.Settings.StartInTray.KeyName ) )
                {
                    Haguichi.preferencesWindow.startInTray.Active = ( bool ) val;
                }
                
                if ( key.Contains ( Config.Settings.ShowStatusbar.KeyName ) )
                {
                    MainWindow.menuBar.showStatusbar.Active = ( bool ) val;
                    MainWindow.ShowStatusbar ( ( bool ) val );
                }
                
                if ( key.Contains ( Config.Settings.ShowOfflineMembers.KeyName ) )
                {
                    MainWindow.menuBar.showOfflineMembers.Active = ( bool ) val;
                    MainWindow.ShowOfflineMembers ( ( bool ) val );
                }
                
                if ( key.Contains ( Config.Settings.SortNetworkListBy.KeyName ) )
                {
                    if ( ( string ) val == "status" )
                    {
                        MainWindow.menuBar.sortByStatus.Active = true;
                        MainWindow.menuBar.sortByName.Active   = false;
                    }
                    else
                    {
                        MainWindow.menuBar.sortByStatus.Active = false;
                        MainWindow.menuBar.sortByName.Active   = true;
                    }
                    
                    MainWindow.networkView.GoSort ( ( string ) val );
                }
                
                if ( key.Contains ( Config.Settings.GoOnlineInNewNetwork.KeyName ) )
                {
                    Haguichi.joinWindow.goOnline.Active   = ( bool ) val;
                    Haguichi.createWindow.goOnline.Active = ( bool ) val;
                }
                
            }
            catch
            {
                Debug.Log ( Debug.Domain.Info, "Config.Client.ValueChanged", "Failed updating GUI for GConf key " + key );
            }
            
        }
        
        
        public static void Set ( Config.Key ck, object val )
        {
            
            try
            {
                client.Set ( Config.Settings.ConfPath + "/" + ck.KeyName, val );
            }
            catch ( Exception e )
            {
                string debug = String.Format ( "Failed setting GConf key \"{0}\" to \"{1}\". Falling back to default setting." , ck.KeyName, val );
                Debug.Log ( Debug.Domain.Info, "Config.Client.Set", debug );
            }
            
        }

        
        public static object Get ( Config.Key ck )
        {
            
            try
            {
                return ( object ) client.Get ( Config.Settings.ConfPath + "/" + ck.KeyName );
            }
            catch ( Exception e )
            {
                Config.Client.Set ( ck, ck.DefaultValue );
                
                string debug = String.Format ( "Failed getting GConf key \"{0}\". Falling back to default setting." , ck.KeyName );
                Debug.Log ( Debug.Domain.Info, "Config.Client.Get", debug );
                
                return ( object ) ck.DefaultValue;
            }
            
        }
        
    }

}
