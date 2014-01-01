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
using GConf;


namespace Config
{
    
    public class Key
    {
        
        private Client client;
        private object _value;
        
        public readonly string KeyName;
        public readonly object DefaultValue;
        
        
        public Key ( string keyName, object defaultValue )
        {
            
            KeyName = keyName;
            DefaultValue = defaultValue;
            
            GLib.GType.Init ();
            
            client = new Client ();
            client.AddNotify ( Config.Settings.ConfPath + keyName, new NotifyEventHandler ( ValueChanged ) );
            
            GetValue ();
            
        }
        
        
        public object Value
        {
            
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                SetValue ( value );
            }
            
        }
        
        
        private void SetValue ( object val )
        {
            
            Debug.Log ( Debug.Domain.Info, "Config.Key.SetValue", "Setting value for GConf key " + KeyName + "..." );
            
            try
            {
                client.Set ( Config.Settings.ConfPath + KeyName, val );
            }
            catch
            {
                Debug.Log ( Debug.Domain.Error, "Config.Key.SetValue", "Failed setting value for GConf key " + KeyName );
            }
            
        }
        
        
        private void GetValue ()
        {
            
            Debug.Log ( Debug.Domain.Info, "Config.Key.GetValue", "Getting value for GConf key " + KeyName + "..." );
            
            try
            {
                _value = ( object ) client.Get ( Config.Settings.ConfPath + KeyName );
            }
            catch
            {
                Debug.Log ( Debug.Domain.Error, "Config.Key.GetValue", "Failed getting value for GConf key " + KeyName + ", falling back to default" );
                Value = DefaultValue;
            }
            
        }
        
        
        private void ValueChanged ( object sender, NotifyEventArgs args )
        {
            
            _value = args.Value;
            string key = args.Key.Replace ( Config.Settings.ConfPath, "" );
            
            Debug.Log ( Debug.Domain.Info, "Config.Key.ValueChanged", "Value for GConf key " + key + " has been changed" );
                
            if ( ( key == Config.Settings.NetworkTemplateSmall.KeyName ) ||
                 ( key == Config.Settings.NetworkTemplateLarge.KeyName ) ||
                 ( key == Config.Settings.MemberTemplateSmall.KeyName ) ||
                 ( key == Config.Settings.MemberTemplateLarge.KeyName ) ||
                 ( key == Config.Settings.NetworkListIconSizeSmall.KeyName ) ||
                 ( key == Config.Settings.NetworkListIconSizeLarge.KeyName ) )
            {
                MainWindow.networkView.SetLayout ();
            }
            else if ( key == Config.Settings.Nickname.KeyName )
            {
                GlobalEvents.UpdateNick ( ( string ) _value );
            }
            else if ( key == Config.Settings.Protocol.KeyName )
            {
                GlobalEvents.UpdateProtocol ( ( string ) _value );
            }
            else if ( key == Config.Settings.UpdateInterval.KeyName )
            {
                Haguichi.preferencesDialog.intervalSpin.Value = ( double ) _value;
                Haguichi.preferencesDialog.SetIntervalString ();
            }
            else if ( key == Config.Settings.UpdateNetworkList.KeyName )
            {
                Haguichi.preferencesDialog.updateNetworkList.Active = ( bool ) _value;
            }
            else if ( key == Config.Settings.ConnectOnStartup.KeyName )
            {
                Haguichi.preferencesDialog.connectOnStartup.Active = ( bool ) _value;
                MainWindow.autoconnectCheckbox.Active = ( bool ) _value;
            }
            else if ( key == Config.Settings.ReconnectOnConnectionLoss.KeyName )
            {
                Haguichi.preferencesDialog.reconnectOnConnectionLoss.Active = ( bool ) _value;
            }
            else if ( key == Config.Settings.DisconnectOnQuit.KeyName )
            {
                Haguichi.preferencesDialog.disconnectOnQuit.Active = ( bool ) _value;
            }
            else if ( key == Config.Settings.NotifyOnConnectionLoss.KeyName )
            {
                Haguichi.preferencesDialog.notifyOnConnectionLoss.Active = ( bool ) _value;
                
                if ( ( Config.Settings.DemoMode ) &&
                     ( ( bool ) _value ) )
                {
                    new Notify ( TextStrings.notifyConnectionLost, "" );
                }
            }
            else if ( key == Config.Settings.NotifyOnMemberJoin.KeyName )
            {
                Haguichi.preferencesDialog.notifyOnMemberJoin.Active = ( bool ) _value;
                
                if ( Config.Settings.DemoMode )
                {
                    Controller.NotifyMemberJoined ( "T-800", "Skynet", 0 ); 
                }
            }
            else if ( key == Config.Settings.NotifyOnMemberLeave.KeyName )
            {
                Haguichi.preferencesDialog.notifyOnMemberLeave.Active = ( bool ) _value;
                
                if ( Config.Settings.DemoMode )
                {
                    Controller.NotifyMemberLeft ( "T-800", "Skynet", 0 );
                }
            }
            else if ( key == Config.Settings.NotifyOnMemberOnline.KeyName )
            {
                Haguichi.preferencesDialog.notifyOnMemberOnline.Active = ( bool ) _value;
                
                if ( Config.Settings.DemoMode )
                {
                    Controller.NotifyMemberOnline ( "T-800", "Skynet", 1 );
                }
            }
            else if ( key == Config.Settings.NotifyOnMemberOffline.KeyName )
            {
                Haguichi.preferencesDialog.notifyOnMemberOffline.Active = ( bool ) _value;
                
                if ( Config.Settings.DemoMode )
                {
                    Controller.NotifyMemberOffline ( "T-800", "Skynet", 2 );   
                }
            }
            else if ( key == Config.Settings.ShowTrayIcon.KeyName )
            {
                Haguichi.preferencesDialog.showTrayIcon.Active = ( bool ) _value;
                MainWindow.menuBar.SetClose ( ( bool ) _value );
            }
            else if ( key == Config.Settings.StartInTray.KeyName )
            {
                Haguichi.preferencesDialog.startInTray.Active = ( bool ) _value;
            }
            else if ( key == Config.Settings.ShowAlternatingRowColors.KeyName )
            {
                MainWindow.menuBar.showAlternatingRowColors.Active = ( bool ) _value;
                MainWindow.networkView.RulesHint = ( bool ) _value;
            }
            else if ( key == Config.Settings.ShowStatusbar.KeyName )
            {
                MainWindow.menuBar.showStatusbar.Active = ( bool ) _value;
                MainWindow.ShowStatusbar ( ( bool ) _value );
            }
            else if ( key == Config.Settings.ShowOfflineMembers.KeyName )
            {
                MainWindow.menuBar.showOfflineMembers.Active = ( bool ) _value;
                MainWindow.ShowOfflineMembers ( ( bool ) _value );
            }
            else if ( key == Config.Settings.SortNetworkListBy.KeyName )
            {
                if ( ( string ) _value == "status" )
                {
                    MainWindow.menuBar.sortByStatus.Active = true;
                    MainWindow.menuBar.sortByName.Active   = false;
                }
                else
                {
                    MainWindow.menuBar.sortByStatus.Active = false;
                    MainWindow.menuBar.sortByName.Active   = true;
                }
                
                MainWindow.networkView.GoSort ( ( string ) _value );
            }
            else if ( key == Config.Settings.NetworkListLayout.KeyName )
            {
                if ( ( string ) _value == "large" )
                {
                    MainWindow.menuBar.layoutLarge.Active  = true;
                    MainWindow.menuBar.layoutNormal.Active = false;
                }
                else
                {
                    MainWindow.menuBar.layoutLarge.Active  = false;
                    MainWindow.menuBar.layoutNormal.Active = true;
                }
                
                MainWindow.networkView.SetLayout ( ( string ) _value );
            }
            else if ( key == Config.Settings.CommandTimeout.KeyName )
            {
                Command.SetTimeout ();
            }
            else if ( key == Config.Settings.CommandForSuperUser.KeyName )
            {
                Command.DetermineSudo ();
            }
            
        }
        
    }

}
