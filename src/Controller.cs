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
using System.Collections;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Gtk;


public class Controller
{
    
    public static bool continueUpdate;
    public static int numUpdateCycles;
    public static int lastStatus;
    
    private static ArrayList oNetworksList;
    private static ArrayList nNetworksList;
    
    private static Gdk.Pixbuf notifyIcon;
    
    
    public static void Init ()
    {
        
        notifyIcon = MainWindow.appIcons [4];
        
        oNetworksList = new ArrayList ();
        nNetworksList = new ArrayList ();
        
        GlobalEvents.UpdateNick ();
        
        lastStatus = StatusCheck ();
        
        if ( lastStatus >= 5 )
        {
            MainWindow.SetMode ( "Connecting" );
            
            if ( Hamachi.apiVersion > 1 )
            {
                GetNetworkList ();
            }
            else if ( Hamachi.apiVersion == 1 )
            {
                GetNicksAndNetworkList ();
            }
        }
        else if ( ( lastStatus >= 3 ) &&
                  ( ( bool ) Config.Client.Get ( Config.Settings.ConnectOnStartup ) ) )
        {
            MainWindow.SetMode ( "Connecting" );
            GLib.Timeout.Add ( 500, new GLib.TimeoutHandler ( ConnectAfterTimeout ) );
        }
        else if ( ( lastStatus >= 2 ) &&
                  ( ( bool ) Config.Client.Get ( Config.Settings.ConnectOnStartup ) ) )
        {
            MainWindow.SetMode ( "Disconnected" );
            GlobalEvents.WaitForInternetCycle ();
        }
        else if ( lastStatus >= 2 )
        {
            MainWindow.SetMode ( "Disconnected" );
        }
        else if ( lastStatus >= 1 )
        {
            MainWindow.SetMode ( "Not configured" );
            Dialogs.NotConfigured dlgNotConfigured = new Dialogs.NotConfigured ( TextStrings.notConfiguredHeading, TextStrings.notConfiguredMessage, "Info" );
        }
        else
        {
            MainWindow.SetMode ( "Not installed" );
            Dialogs.NotInstalled dlgNotInstalled = new Dialogs.NotInstalled ( TextStrings.notInstalledHeading, TextStrings.notInstalledMessage, "Info" );
        }
   
    }
    
    
    private static void GetNicksAndNetworkList ()
    {
        
        MainWindow.statusBar.Push ( 0, TextStrings.gettingNicks );
        
        Hamachi.GetNicks ();
        
        uint wait = ( uint ) ( 1000 * ( double ) Config.Client.Get ( Config.Settings.GetNicksWaitTime ) );
        GLib.Timeout.Add ( wait, new GLib.TimeoutHandler ( TimedGetNetworkList ) );
        
    }
    
    
    public static int StatusCheck ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return 6;   
        }
        
        string output = Hamachi.GetInfo ();
        
        Regex regex;
        
        if ( output == "error" ) // 'bash: hamachi: command not found' causes exception
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Not installed." );
            return 0;
        }
        
        if ( ( output.IndexOf ( "Cannot find configuration directory" ) == 0 ) ||
             ( output.IndexOf ( "You do not have permission to control the hamachid daemon." ) == 0 ) )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "Not configured." );
            return 1;
        }
        
        if ( !HasInternetConnection () )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.StatusCheck", "No internet." );
            return 2;
        }
        
        if ( output.IndexOf ( "Hamachi does not seem to be running." ) == 0 )
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
        catch ( System.Exception ex )
        {
            return false;
        }
        
    }
    
    
    public static bool WaitForInternet ()
    {
        
        if ( HasInternetConnection () )
        {
            GlobalEvents.StartHamachi ();
            return false;
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.WaitForInternet", "Waiting for internet connection" );
            return true;   
        }
        
    }
    
    
    public static void HamachiGoStart ()
    {
        
        lastStatus = StatusCheck ();
        
        if ( lastStatus >= 3 )
        {
            string output = Hamachi.Start ();         
            
            if ( ( output.IndexOf ( ".. ok" ) != -1 ) ||
                 ( output == "" ) ||
                 ( output.IndexOf ( "Hamachi is already started" ) == 0 ) )
            {
                /* Ok, started */
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Started!" );
            }
            else if ( output.IndexOf ( "Starting LogMeIn Hamachi VPN tunneling engine logmein-hamachi" ) == 0 )
            {
                /* Hamachi is starting */
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Hamachi is starting." );
                
                string output2 = Hamachi.GetInfo ();
                Regex regex = new Regex ( "status([ ]+):([ ]+)logging in" );
                
                if ( output2.IndexOf ( "You do not have permission to control the hamachid daemon." ) == 0 )
                {
                    Dialogs.NotConfigured dlgNotConfigured = new Dialogs.NotConfigured ( TextStrings.notConfiguredHeading, TextStrings.notConfiguredMessage, "Info" );
                }
                else if ( regex.IsMatch ( output2 ) )
                {
                    /* Wait a moment hoping Hamachi will be fully started by then */
                    System.Threading.Thread.Sleep ( 2000 );
                }
            }
            else if ( output.IndexOf ( "cfg: failed to load" ) != -1 )
            {
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Not properly configured, showing dialog." );
                Dialogs.Message dlg3 = new Dialogs.Message ( TextStrings.configErrorHeading, TextStrings.configErrorMessage, "Error" );
            }
            else if ( output.IndexOf ( "tap: connect() failed 2 (No such file or directory)" ) != -1 )
            {
                /* Not able to start, running tuncfg required */
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "TunCfg required." );
                MainWindow.statusBar.Push ( 0, TextStrings.runningTuncfg );
                
                if ( ( bool ) Config.Client.Get ( Config.Settings.AskBeforeRunningTunCfg ) )
                {
                    Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Asking before running tuncfg." );
                    Dialogs.RunTunCfg dlg = new Dialogs.RunTunCfg ();
                    
                    if ( dlg.ResponseText == "Ok" )
                    {
                        Hamachi.TunCfg ();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Running tuncfg." );
                    Hamachi.TunCfg ();
                }
                
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Trying to start again." );
                string output2 = Hamachi.Start ();
                
                if ( lastStatus >= 4 )
                {
                    Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Yes, started now!" );
                }
                else
                {
                    Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Not yet started, go start?" );
                }
            }
            else if ( output.IndexOf ( "tap: connect() failed 111 (Connection refused)" ) != -1 )
            {
                /* Not able to start, connection refused */
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Connection refused, showing dialog" );
                Dialogs.Message dlg2 = new Dialogs.Message ( TextStrings.connectErrorHeading, TextStrings.connectErrorConnectionRefused, "Error" );
                GlobalEvents.ConnectionStopped ();
            }
            else
            {
                Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Failed to start for unknown reason." );
                GlobalEvents.ConnectionStopped ();
            }
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoStart", "Not yet installed or configured." );
            GlobalEvents.ConnectionStopped ();
        }
        
    }
    
    
    public static void HamachiGoConnect ()
    {
        
        lastStatus = StatusCheck ();
        
        if ( lastStatus >= 4 )
        {
            MainWindow.statusBar.Push ( 0, TextStrings.loggingIn );
            GLib.Timeout.Add ( 10, new GLib.TimeoutHandler ( TimedGoLogin ) );
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.GoConnect", "Not yet started, go start." );
            MainWindow.statusBar.Push ( 0, TextStrings.starting );
            GLib.Timeout.Add ( 10, new GLib.TimeoutHandler ( TimedGoStart ) );
        }
        
    }
    
    
    public static bool ConnectAfterTimeout ()
    {
        
        HamachiGoConnect ();
        return false;
        
    }
    
    
    private static bool TimedGoLogin ()
    {
        
        string output = Hamachi.Login ();
        
        if ( ( output.IndexOf ( ".. ok" ) != -1 ) ||
             ( output.IndexOf ( "Already logged in" ) == 0 ) ) // Ok, logged in.
        {
            Debug.Log ( Debug.Domain.Info, "Controller.TimedGoLogin", "Connected!" );
            
            if ( Hamachi.apiVersion > 1 )
            {
                GetNetworkList ();
            }
            else if ( Hamachi.apiVersion == 1 )
            {
                GetNicksAndNetworkList ();
            }
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.TimedGoLogin", "Error connecting, showing dialog." );
            Dialogs.Message dlg1 = new Dialogs.Message ( TextStrings.connectErrorHeading, TextStrings.connectErrorLoginFailed, "Error" );
            GlobalEvents.ConnectionStopped ();
        }
        return false;
        
    }
    
    
    private static bool TimedGoStart ()
    {
        
        HamachiGoStart ();
        
        lastStatus = StatusCheck ();
        
        if ( lastStatus >= 4 )
        {
            Debug.Log ( Debug.Domain.Info, "Controller.TimedGoStart", "Started, now go connect." );
            MainWindow.statusBar.Push ( 0, TextStrings.loggingIn );
            HamachiGoConnect ();
        }
        else
        {
            Debug.Log ( Debug.Domain.Info, "Controller.TimedGoStart", "Unable to start, giving up." );
            GlobalEvents.ConnectionStopped ();
        }
        return false;
        
    }
    
    
    private static bool TimedGetNetworkList ()
    {
        
        GetNetworkList ();
        
        return false;
        
    }
    
    
    private static void GetNetworkList ()
    {
        
        Haguichi.connection.ClearNetworks ();
        Haguichi.connection.Status = new Status ( "*" );
        
        ArrayList networks = Hamachi.ReturnList ();
        Haguichi.connection.Networks = networks;

        
        MainWindow.networkView.FillTree ();
        GlobalEvents.ConnectionEstablished ();
        
    }
    
    
    public static bool UpdateConnection ()
    {
        
        if ( ( !continueUpdate ) ||
             ( numUpdateCycles > 1 ) )
        {
            numUpdateCycles -= 1;
            return false;
        }
        
        Debug.Log ( Debug.Domain.Info, "Controller.UpdateConnection", "Retrieving connection status..." );
        
        MainWindow.statusBar.Push ( 0, TextStrings.updating );
        
        BackgroundWorker worker = new BackgroundWorker {};
        worker.DoWork += UpdateConnectionThread;
        worker.RunWorkerAsync ();
        
        /*
         * Wait a moment for the thread to finish then continue in the main thread,
         * because GtkTreeView doesn't get updated when the GtkTreeModel is changed async... :(
         */
        uint wait = ( uint ) ( 1000 * ( double ) Config.Client.Get ( Config.Settings.GetListWaitTime ) );
        GLib.Timeout.Add ( wait, new GLib.TimeoutHandler ( UpdateList ) );
        
        return true;
        
    }
    
    
    private static void UpdateConnectionThread ( object o, DoWorkEventArgs args )
    {
        
        lastStatus = StatusCheck ();
    
        if ( ( HasInternetConnection () ) &&
             ( lastStatus >= 6 ) )
        {
            
            if ( Hamachi.apiVersion == 1 )
            {
                 // Update nicks
                Hamachi.GetNicks ();
            }
            
            nNetworksList = Hamachi.ReturnList ();
        }
        
    }
    
    
    private static bool UpdateList ()
    {
        
        if ( !continueUpdate )
        {
            /* Check again if we should update because this function is called after timeout */
            return false;
        }
        
        if ( Config.Settings.DemoMode )
        {
            
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Demo mode, not really updating list." );
            
            /* Continue update interval */
            continueUpdate = true;
            
            MainWindow.statusBar.Push ( 0, TextStrings.connected );
            
        }
        else if ( ( HasInternetConnection () ) &&
                  ( Haguichi.connection.Status.statusInt == 1 ) &&
                  ( lastStatus >= 6 ) ) // We're connected allright
        {
        
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Connected, updating list." );
            
            oNetworksList = ( ArrayList ) Haguichi.connection.Networks.Clone (); // Cloning to prevent exception (InvalidOperationException: List has changed)
            
            
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
                                 ( !oMember.IsEvicted ) &&
                                 ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberLeave ) ) )
                            {
                                string body = String.Format ( TextStrings.notifyMemberLeftMessage, oMember.Nick, oNetwork.Name );
                                Notify n = new Notify ( TextStrings.notifyMemberLeftHeading, body, notifyIcon );
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
                                 ( nMember.Status.statusInt == 1 ) &&
                                 ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberOnline ) ) )
                            {
                                string body = String.Format ( TextStrings.notifyMemberOnlineMessage, nMember.Nick, oNetwork.Name );
                                Notify n = new Notify ( TextStrings.notifyMemberOnlineHeading, body, notifyIcon );
                            }
                            if ( ( oMember.Status.statusInt == 1 ) &&
                                 ( nMember.Status.statusInt == 0 ) &&
                                 ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberOffline ) ) )
                            {
                                string body = String.Format ( TextStrings.notifyMemberOfflineMessage, nMember.Nick, oNetwork.Name );
                                Notify n = new Notify ( TextStrings.notifyMemberOfflineHeading, body, notifyIcon );
                            }
                            
                            oMember.Update ( nMember.Status, nMember.Network, nMember.Address, nMember.Nick, nMember.ClientId, nMember.Tunnel );
                            
                            MainWindow.networkView.UpdateMember ( oNetwork, oMember );
                        }
                        else
                        {
                            /* Member not in old list, adding... */
                            
                            oNetwork.AddMember ( nMember );
                            
                            MainWindow.networkView.AddMember ( oNetwork, nMember );
                            
                            if ( ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberJoin ) )
                            {
                                string body = String.Format ( TextStrings.notifyMemberJoinedMessage, nMember.Nick, oNetwork.Name );
                                Notify n = new Notify ( TextStrings.notifyMemberJoinedHeading, body, notifyIcon );
                            }
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
            
            /* Continue update interval */
            continueUpdate = true;
            
            MainWindow.statusBar.Push ( 0, TextStrings.connected );
            
        }
        else if ( ( Haguichi.connection.Status.statusInt == 1 ) &&
                  ( ( lastStatus < 6 ) || !HasInternetConnection () ) ) // We're not connected, but should be
        {
            
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Connection lost." );
            
            GlobalEvents.ConnectionLost ();
            
        }
        else // Connection already stopped
        {
            
            Debug.Log ( Debug.Domain.Info, "Controller.UpdateList", "Disconnected, shall not update list." );
            
        }
        
        return false; // Stop calling timeout handler
        
    }
    
}
