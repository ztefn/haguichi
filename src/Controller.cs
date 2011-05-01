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
using System.Collections;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Gtk;


public static class Controller
{
    
    public  static bool continueUpdate;
    public  static bool manualUpdate;
    public  static bool restoreConnection;
    public  static int restoreCountdown;
    public  static int lastStatus = -2;
    public  static int numUpdateCycles;
    private static int numWaitForInternetCycles;
    private static string startOutput;
    
    private static Hashtable membersLeftHash;
    private static Hashtable membersOnlineHash;
    private static Hashtable membersOfflineHash;
    private static Hashtable membersJoinedHash;
            
    private static ArrayList oNetworksList;
    private static ArrayList nNetworksList;
    
    private static Gdk.Pixbuf notifyIcon;
    
    
    public static void Init ()
    {
        
        MainWindow.messageBar.Hide ();
        
        notifyIcon = MainWindow.appIcons [4];
        
        oNetworksList = new ArrayList ();
        nNetworksList = new ArrayList ();
        
        GlobalEvents.UpdateNick ();
        GlobalEvents.SetAttach ();
        
        StatusCheck ();
        
        if ( lastStatus >= 6 )
        {
            restoreConnection = ( bool ) Config.Client.Get ( Config.Settings.ReconnectOnConnectionLoss );
            
            MainWindow.SetMode ( "Connected" );
            
            if ( Hamachi.ApiVersion == 1 )
            {
                Hamachi.GetNicks ();
            }
            
            GetNetworkList ();
        }
        else if ( lastStatus >= 5 )
        {
            restoreConnection = ( bool ) Config.Client.Get ( Config.Settings.ReconnectOnConnectionLoss );
            
            MainWindow.SetMode ( "Connecting" );
            GoConnect ();
        }
        else if ( ( lastStatus >= 3 ) &&
                  ( ( bool ) Config.Client.Get ( Config.Settings.ConnectOnStartup ) ) )
        {
            restoreConnection = true;
            
            MainWindow.SetMode ( "Connecting" );
            GoConnect ();
        }
        else if ( ( lastStatus >= 2 ) &&
                  ( ( bool ) Config.Client.Get ( Config.Settings.ConnectOnStartup ) ) )
        {
            restoreConnection = true;
            
            MainWindow.SetMode ( "Disconnected" );
            WaitForInternetCycle ();
        }
        else if ( lastStatus >= 2 )
        {
            MainWindow.SetMode ( "Disconnected" );
        }
        else if ( lastStatus >= 1 )
        {
            MainWindow.SetMode ( "Not configured" );
            
            Button configureButton = new Button ( TextStrings.configureLabel );
            configureButton.Clicked += delegate
            {
                Hamachi.Configure ();
            };
            
            MainWindow.messageBar.SetMessage ( TextStrings.notConfiguredHeading, TextStrings.notConfiguredMessage, MessageType.Warning, false );
            MainWindow.messageBar.AddButton ( configureButton );
        }
        else
        {
            MainWindow.SetMode ( "Not installed" );
            
            Button downloadButton = new Button ( TextStrings.downloadLabel );
            downloadButton.Clicked += delegate
            {
                Command.OpenURL ( TextStrings.getHamachiURL );
            };
            
            MainWindow.messageBar.SetMessage ( TextStrings.notInstalledHeading, TextStrings.notInstalledMessage, MessageType.Error, false );
            MainWindow.messageBar.AddButton ( downloadButton );
        }
        
    }
    
    
    public static void StatusCheck ()
    {
        
        lastStatus = StatusInt ();
        
    }
    
    
    private static int StatusInt ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return 6;
        }
        
        if ( !HasInternetConnection () && lastStatus > 1 )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "No internet connection." ); // We don't want to call Hamachi if there's no Internet connection...
            return 2;
        }
        
        string output;
        
        if ( lastStatus == -2 )
        {
            output = Hamachi.lastInfo; // Reuse last info requested by Hamachi.Init when launching to increase startup speed
        }
        else
        {
            output = Hamachi.GetInfo ();
        }
        
        Regex regex;
        
        if ( output == "error" ) // 'bash: hamachi: command not found' causes exception
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Not installed." );
            return 0;
        }
        
        if ( ( output.Contains ( "Cannot find configuration directory" ) ) ||
             ( output.Contains ( "You do not have permission to control the hamachid daemon." ) ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Not configured." );
            return 1;
        }
        
        if ( !HasInternetConnection () )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "No internet connection." );
            return 2;
        }
        
        if ( output.Contains ( "Hamachi does not seem to be running." ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Not started." );
            return 3;
        }
        
        regex = new Regex ( "status([ ]+):([ ]+)offline" );
        if ( regex.IsMatch ( output ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Not logged in." );
            return 4;
        }
        
        regex = new Regex ( "status([ ]+):([ ]+)logging in" );
        if ( regex.IsMatch ( output ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Logging in." );
            return 5;
        }
        
        regex = new Regex ( "status([ ]+):([ ]+)logged in" );
        if ( regex.IsMatch ( output ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Logged in." );
            return 6;
        }
        
        Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Unknown." );
        return -1;
    
    }
    
    
    public static bool HasInternetConnection ()
    {
        
        try
        {
            TcpClient client = new TcpClient ( "www.google.com", 80 );
            client.Close ();
            
            return true;
        }
        catch ( Exception e )
        {
            return false;
        }
        
    }
    
    
    private static bool WaitForInternet ()
    {
        
        if ( numWaitForInternetCycles > 1 )
        {
            numWaitForInternetCycles --;
            return false;
        }
        else if ( !restoreConnection )
        {
            numWaitForInternetCycles --;
            return false;
        }
        else if ( HasInternetConnection () )
        {
            numWaitForInternetCycles --;
            GlobalEvents.StartHamachi ();
            return false;
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.WaitForInternet", "Waiting for internet connection" );
            return true;   
        }
        
    }
    
    
    public static void GoStartThread ()
    {
        
        string output = startOutput;
        
        if ( ( output.Contains ( ".. ok" ) ) ||
             ( output == "empty" ) ||
             ( output.Contains ( "Hamachi is already started" ) ) )
        {
            /* Ok, started */
            Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Started, now go login." );
            
            Application.Invoke ( delegate
            {
                MainWindow.statusBar.Push ( 0, TextStrings.loggingIn );
            });
            GoLoginThread ();
        }
        else if ( output.Contains ( "Starting LogMeIn Hamachi VPN tunneling engine logmein-hamachi" ) )
        {
            /* Hamachi is starting */
            Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Hamachi is starting." );
            
            /* Wait a moment hoping Hamachi will be fully started by then */
            Thread.Sleep ( 2000 );
            
            StatusCheck ();
            
            if ( lastStatus >= 4 )
            {
                Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Started, now go login." );
                
                Application.Invoke ( delegate
                {
                    MainWindow.statusBar.Push ( 0, TextStrings.loggingIn );
                });
                GoLoginThread ();
            }
            else
            {
                Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Still not finished started, stopping now." );
                
                Application.Invoke ( delegate
                {
                    GlobalEvents.ConnectionStopped ();
                });
            }
        }
        else if ( output.Contains ( "cfg: failed to load" ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Not properly configured, showing dialog." );
            Application.Invoke ( delegate
            {
                GlobalEvents.ConnectionStopped ();
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (),
                                      TextStrings.configErrorHeading,
                                      TextStrings.configErrorMessage,
                                      "Error",
                                      output );
            });
        }
        else if ( output.Contains ( "tap: connect() failed 2 (No such file or directory)" ) )
        {
            /* Not able to start, running tuncfg required */
            Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Tuncfg required." );
            Application.Invoke ( delegate
            {
                MainWindow.statusBar.Push ( 0, TextStrings.runningTuncfg );
                GoRunTuncfgAndTryStartAgain ();
            });
        }
        else if ( output.Contains ( "tap: connect() failed 111 (Connection refused)" ) )
        {
            /* Not able to start, connection refused */
            Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Connection refused, showing dialog" );
            
            Application.Invoke ( delegate
            {
                GlobalEvents.ConnectionStopped ();
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (),
                                      TextStrings.connectErrorHeading,
                                      TextStrings.connectErrorConnectionRefused,
                                      "Error",
                                      output );
            });
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoStartThread", "Failed to start for unknown reason." );
            
            Application.Invoke ( delegate
            {
                GlobalEvents.ConnectionStopped ();
            });
        }
        
        startOutput = null;
        
    }
    
    
    public static void GoConnectThread ()
    {
        
        Application.Invoke ( delegate
        {
            MainWindow.SetMode ( "Connecting" );
            MainWindow.messageBar.Hide ();
        });
        
        StatusCheck ();
        
        if ( Config.Settings.DemoMode )
        {
            Application.Invoke ( delegate
            {
                GlobalEvents.ConnectionEstablished ();
            });
        }
        else if ( lastStatus == 2 )
        {
            Application.Invoke ( delegate
            {
                GlobalEvents.ConnectionStopped ();
                MainWindow.messageBar.SetMessage ( TextStrings.connectErrorHeading, TextStrings.connectErrorNoInternetConnection, MessageType.Error, true );
            });
        }
        else if ( lastStatus >= 4 )
        {
            Application.Invoke ( delegate
            {
                MainWindow.statusBar.Push ( 0, TextStrings.loggingIn );
            });
            GoLoginThread ();
        }
        else if ( lastStatus >= 3 )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoConnectThread", "Not yet started, go start." );
            
            Application.Invoke ( delegate
            {
                MainWindow.statusBar.Push ( 0, TextStrings.starting );
                
                if ( Hamachi.ApiVersion > 1 )
                {
                    GoStart ();
                }
            });
            
            if ( Hamachi.ApiVersion == 1 )
            {
                startOutput = Hamachi.Start ();
                GoStartThread ();
            }
        }
        
    }
    
    
    private static void GoStart ()
    {
        
        startOutput = Hamachi.Start ();
        
        Thread thread = new Thread ( GoStartThread );
        thread.Start ();
        
    }
    
    
    public static void GoConnect ()
    {
        
        Thread thread = new Thread ( GoConnectThread );
        thread.Start ();
        
    }
    
    
    private static void GoLoginThread ()
    {
        
        string output = Hamachi.Login ();
        
        if ( ( output.Contains ( ".. ok" ) ) ||
             ( output.Contains ( "Already logged in" ) ) ) // Ok, logged in.
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoLoginThread", "Connected!" );
            
            lastStatus = 6;
            
            if ( Hamachi.ApiVersion == 1 )
            {
                Hamachi.GetNicks ();
                Hamachi.GetInfo (); // Get latest info to be able to update information dialog correcltly
            }
            
            Thread.Sleep ( 1000 ); // Wait a second to get an updated list 
            
            Application.Invoke ( delegate
            {
               GetNetworkList ();
            });
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoLoginThread", "Error connecting." );
            
            Application.Invoke ( delegate
            {
                GlobalEvents.ConnectionStopped ();
                MainWindow.messageBar.SetMessage ( TextStrings.connectErrorHeading, TextStrings.connectErrorLoginFailed, MessageType.Error, true );
            });
        }
        
    }
    
    
    private static void GoRunTuncfgAndTryStartAgain ()
    {

        if ( ( bool ) Config.Client.Get ( Config.Settings.AskBeforeRunningTunCfg ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoRunTuncfgAndTryStartAgain", "Asking before running tuncfg." );
            Dialogs.RunTunCfg dlg = new Dialogs.RunTunCfg ();
            
            if ( dlg.ResponseText == "Ok" )
            {
                Hamachi.TunCfg ();
            }
            else
            {
                Application.Invoke ( delegate
                {
                    GlobalEvents.ConnectionStopped ();
                });
                return;
            }
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoRunTuncfgAndTryStartAgain", "Running tuncfg." );
            Hamachi.TunCfg ();
        }
        
        Debug.Log ( Debug.Domain.Info, "Controller.GoRunTuncfgAndTryStartAgain", "Trying to start again." );
        
        Thread thread = new Thread ( GoConnectThread );
        thread.Start ();
        
    }
    
    
    private static void GetNetworkList ()
    {
        
        Haguichi.connection.ClearNetworks ();
        
        ArrayList networks = Hamachi.ReturnList ();
        Haguichi.connection.Networks = networks;
        
        MainWindow.networkView.FillTree ();
        GlobalEvents.ConnectionEstablished ();
        
    }
    
    
    public static void UpdateConnection ( object obj, EventArgs args )
    {
        
        UpdateConnection ();
        
    }
    
    
    public static void UpdateConnection ()
    {
        
        manualUpdate = true;
        UpdateConnectionTimeout ();
        
    }
    
    
    private static bool UpdateConnectionTimeout ()
    {
        
        Debug.Log ( Debug.Domain.Info, "Controller.UpdateConnectionTimeout", "Number of active update cycles: " + numUpdateCycles );
        
        if ( numUpdateCycles > 1 )
        {
            numUpdateCycles --;
            return false;
        }
        
        if ( continueUpdate )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateConnection", "Retrieving connection status..." );
            
            MainWindow.statusBar.Push ( 0, TextStrings.updating );
            
            Thread thread = new Thread ( UpdateConnectionThread );
            thread.Start ();
        }
        else
        {
            numUpdateCycles --;
        }
        
        return false;
        
    }
    
    
    private static void UpdateConnectionThread ()
    {
        
        StatusCheck ();
    
        if ( lastStatus >= 6 )
        {
            
            if ( Hamachi.ApiVersion == 1 )
            {
                 // Update nicks
                Hamachi.GetNicks ();
            }
            
            nNetworksList = Hamachi.ReturnList ();
        }
        
        if ( continueUpdate )
        {
            Application.Invoke ( delegate
            {
                UpdateList ();
            });
        }
        else
        {
            numUpdateCycles --;
        }
        
    }
    
    
    private static void UpdateList ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Demo mode, not really updating list." );
            
            if ( manualUpdate )
            {
                manualUpdate = false;
            }
            else
            {
                UpdateCycle (); // Continue update interval
            }
            
            MainWindow.statusBar.Pop ( 0 );
            
        }
        else if ( lastStatus == 2 )
        {
            
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Internet connection lost." );
            
            GlobalEvents.ConnectionStopped ();
            numUpdateCycles --;
            
        }
        else if ( lastStatus < 6 )
        {
            
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Hamachi connection lost." );
            
            if ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnConnectionLoss ) )
            {
                new Notify ( TextStrings.notifyConnectionLost, "", notifyIcon );
            }
            
            GlobalEvents.ConnectionStopped ();
            numUpdateCycles --;
            
        }
        else if ( lastStatus >= 6 ) // We're connected allright
        {
        
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Connected, updating list." );
            
            oNetworksList = ( ArrayList ) Haguichi.connection.Networks.Clone (); // Cloning to prevent exception (InvalidOperationException: List has changed)
            
            membersLeftHash    = new Hashtable ();
            membersOnlineHash  = new Hashtable ();
            membersOfflineHash = new Hashtable ();
            membersJoinedHash  = new Hashtable ();
            
            
            Hashtable oNetworksHash = new Hashtable ();
            
            foreach ( Network oNetwork in oNetworksList )
            {
                oNetworksHash.Add ( oNetwork.Id, oNetwork );
            }
            
            Hashtable nNetworksHash = new Hashtable ();
            
            foreach ( Network nNetwork in nNetworksList )
            {
                nNetworksHash.Add ( nNetwork.Id, nNetwork );
            }
            
            
            foreach ( Network oNetwork in oNetworksList )
            {
                if ( !nNetworksHash.ContainsKey ( oNetwork.Id ) )
                {
                    /* Network not in new list, removing... */
                    
                    Haguichi.connection.RemoveNetwork ( oNetwork );
                    
                    MainWindow.networkView.RemoveNetwork ( oNetwork );
                }
            }
            
            foreach ( Network nNetwork in nNetworksList )
            {
                if ( oNetworksHash.ContainsKey ( nNetwork.Id ) )
                {
                    /* Network in new and old list, updating... */
                    
                    Network oNetwork = ( Network ) oNetworksHash [ nNetwork.Id ];
                    
                    oNetwork.Update ( nNetwork.Status, nNetwork.Id, nNetwork.Name );
                    
                    MainWindow.networkView.UpdateNetwork ( oNetwork );
                    
                    
                    /* Check all network members */
                    
                    ArrayList oMembersList = ( ArrayList ) oNetwork.Members.Clone (); // Cloning to prevent exception (InvalidOperationException: List has changed)
                    ArrayList nMembersList = nNetwork.Members;
                    
    
                    Hashtable oMembersHash = new Hashtable ();
                    
                    foreach ( Member oMember in oMembersList )
                    {
                        oMembersHash.Add ( oMember.ClientId, oMember );
                    }
                    
                    
                    Hashtable nMembersHash = new Hashtable ();
                    
                    foreach ( Member nMember in nMembersList )
                    {
                        nMembersHash.Add ( nMember.ClientId, nMember );
                    }
                    
                    
                    foreach ( Member oMember in oMembersList )
                    {
                        if ( !nMembersHash.ContainsKey ( oMember.ClientId ) )
                        {
                            /* Member not in new list, removing... */
                            
                            oNetwork.RemoveMember ( oMember );
                            
                            MainWindow.networkView.RemoveMember ( oNetwork, oMember );
                            
                            if ( ( oMember.Status.statusInt < 3 ) &&
                                 ( !oMember.IsEvicted ) )
                            {
                                AddMemberToHash ( membersLeftHash, oMember, oNetwork );
                            }
                        }
                    }
                    
                    foreach ( Member nMember in nMembersList )
                    {
                        if ( oMembersHash.ContainsKey ( nMember.ClientId ) )
                        {
                            /* Member in old and new list, updating... */
                            
                            Member oMember = ( Member ) oMembersHash [ nMember.ClientId ];
                            
                            if ( ( oMember.Status.statusInt == 0 ) &&
                                 ( nMember.Status.statusInt == 1 ) )
                            {
                                AddMemberToHash ( membersOnlineHash, nMember, oNetwork );
                            }
                            if ( ( oMember.Status.statusInt == 1 ) &&
                                 ( nMember.Status.statusInt == 0 ) )
                            {
                                AddMemberToHash ( membersOfflineHash, nMember, oNetwork );
                            }
                            
                            oMember.Update ( nMember.Status, nMember.Network, nMember.Address, nMember.Nick, nMember.ClientId, nMember.Tunnel );
                            
                            MainWindow.networkView.UpdateMember ( oNetwork, oMember );
                        }
                        else
                        {
                            /* Member not in old list, adding... */
                            
                            oNetwork.AddMember ( nMember );
                            
                            MainWindow.networkView.AddMember ( oNetwork, nMember );
                            
                            AddMemberToHash ( membersJoinedHash, nMember, oNetwork );
                        }
                    }
                    
                }
                else
                {
                    /* Network not in old list, adding... */
                    
                    Haguichi.connection.AddNetwork ( nNetwork );
                    
                    MainWindow.networkView.AddNetwork ( nNetwork );
                }
            }
            
            MainWindow.networkView.CollapseOrExpandNetworks ();
            
            NotifyMembersJoined ();
            NotifyMembersLeft ();
            NotifyMembersOnline ();
            NotifyMembersOffline ();
            
            if ( manualUpdate )
            {
                manualUpdate = false;
            }
            else
            {
                UpdateCycle (); // Continue update interval
            }
            
            MainWindow.statusBar.Pop ( 0 );
            
        }
        
    }
    
    
    private static void AddMemberToHash ( Hashtable hash, Member member, Network network )
    {
        
        ArrayList array = new ArrayList ();
        
        if ( hash.ContainsKey ( member.ClientId ) )
        {
            array = ( ArrayList ) hash [ member.ClientId ];
            hash.Remove ( member.ClientId );
        }
        
        array.Add ( new string [] { member.Nick, network.Name } );
        
        hash.Add ( member.ClientId, array );
        
    }
    
    
    public static void WaitForInternetCycle ()
    {
        
        numWaitForInternetCycles ++;
        
        uint interval = ( uint ) ( 1000 );
        GLib.Timeout.Add ( interval, new GLib.TimeoutHandler ( WaitForInternet ) );
        
    }
    
    
    public static void RestoreConnectionCycle ()
    {
        
        Debug.Log ( Debug.Domain.Info, "Controller.RestoreConnectionCycle", "Trying to reconnect..." );
        
        restoreCountdown = ( int ) ( ( double ) Config.Client.Get ( Config.Settings.ReconnectInterval ) );
        MainWindow.SetMode ( "Countdown" );
        
        uint interval = ( uint ) ( 1000 );
        GLib.Timeout.Add ( interval, new GLib.TimeoutHandler ( RestoreConnection ) );
        
    }
    
    
    private static bool RestoreConnection ()
    {
        
        if ( restoreConnection )
        {
            restoreCountdown --;
            
            if ( restoreCountdown == 0 )
            {
                GlobalEvents.StartHamachi ();
            }
            else
            {
                MainWindow.SetMode ( "Countdown" );
                return true;   
            }
        }
        
        return false;
        
    }
    
    
    public static bool UpdateCycle ()
    {
        
        continueUpdate = true;
        
        uint interval = ( uint ) ( 1000 * ( double ) Config.Client.Get ( Config.Settings.UpdateInterval ) );
        
        if ( interval > 0 )
        {
            GLib.Timeout.Add ( interval, new GLib.TimeoutHandler ( UpdateConnectionTimeout ) );
        }
        else
        {
            GLib.Timeout.Add ( 1000, new GLib.TimeoutHandler ( UpdateCycle ) );
        }
        
        return false;
        
    }
    
    
    public static void NotifyMembersJoined ()
    {
        
        foreach ( ArrayList member in membersJoinedHash.Values )
        {
            string [] network = ( string [] ) member [0];
            NotifyMemberJoined ( network [0], ( string ) network [1], ( member.Count - 1 ) );
        }
        
    }
    
    
    public static void NotifyMemberJoined ( string nick, string network, int more )
    {
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberJoin ) )
        {
            string message = TextStrings.notifyMemberJoinedMessage;
            if ( more > 0 )
            {
                message = TextStrings.notifyMemberJoinedMessagePlural ( more );
            }
            
            string body = String.Format ( message, nick, network, more );
            new Notify ( TextStrings.notifyMemberJoinedHeading, body, notifyIcon );
        }
        
    }
    
    
    public static void NotifyMembersLeft ()
    {
        
        foreach ( ArrayList member in membersLeftHash.Values )
        {
            string [] network = ( string [] ) member [0];
            NotifyMemberLeft ( network [0], ( string ) network [1], ( member.Count - 1 ) );
        }
        
    }
    
    
    public static void NotifyMemberLeft ( string nick, string network, int more )
    {
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberLeave ) )
        {
            string message = TextStrings.notifyMemberLeftMessage;
            if ( more > 0 )
            {
                message = TextStrings.notifyMemberLeftMessagePlural ( more );
            }
            
            string body = String.Format ( message, nick, network, more );
            new Notify ( TextStrings.notifyMemberLeftHeading, body, notifyIcon );
        }
        
    }
    
    
    public static void NotifyMembersOnline ()
    {
        
        foreach ( ArrayList member in membersOnlineHash.Values )
        {
            string [] network = ( string [] ) member [0];
            NotifyMemberOnline ( network [0], ( string ) network [1], ( member.Count - 1 ) );
        }
        
    }
    
    
    public static void NotifyMemberOnline ( string nick, string network, int more )
    {
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberOnline ) )
        {
            string message = TextStrings.notifyMemberOnlineMessage;
            if ( more > 0 )
            {
                message = TextStrings.notifyMemberOnlineMessagePlural ( more );
            }
            
            string body = String.Format ( message, nick, network, more );
            new Notify ( TextStrings.notifyMemberOnlineHeading, body, notifyIcon );
        }
        
    }
    
    
    public static void NotifyMembersOffline ()
    {
        
        foreach ( ArrayList member in membersOfflineHash.Values )
        {
            string [] network = ( string [] ) member [0];
            NotifyMemberOffline ( network [0], ( string ) network [1], ( member.Count - 1 ) );
        }
        
    }
    
    
    public static void NotifyMemberOffline ( string nick, string network, int more )
    {
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberOffline ) )
        {
            string message = TextStrings.notifyMemberOfflineMessage;
            if ( more > 0 )
            {
                message = TextStrings.notifyMemberOfflineMessagePlural ( more );
            }
            
            string body = String.Format ( message, nick, network, more );
            new Notify ( TextStrings.notifyMemberOfflineHeading, body, notifyIcon );
        }
        
    }
    
}
