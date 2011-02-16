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
using System.Threading;
using Gtk;


public class GlobalEvents
{
    
    public static void ConfigureHamachi ( object obj, EventArgs args )
    {
        
        Hamachi.Configure ();
        
    }
    
    
    public static void StartHamachi ( object obj, EventArgs args )
    {
        
        StartHamachi ();
        
    }
    
    
    public static void StartHamachi ()
    {
        
        Controller.restoreConnection = false;
        Controller.GoConnect ();
        
    }
    
    
    public static void StopHamachi ( object obj, EventArgs args )
    {
        
        StopHamachi ();
        
    }
    
    
    public static void StopHamachi ()
    {
        
        Thread thread = new Thread ( StopHamachiThread );
        thread.Start ();
        
        ConnectionStopped ();
        
    }
    
    
    private static void StopHamachiThread ()
    {

        if ( Config.Settings.DemoMode )
        {
            // Do nothing
        }
        else if ( Hamachi.ApiVersion > 1 )
        {
            Hamachi.Logout ();
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            Hamachi.Stop ();
        }
        
    }

    
    public static void RunTunCfg ( object obj, EventArgs args )
    {
        
        Hamachi.TunCfg ();
        
    }
    
    
    public static void ConnectionEstablished ()
    {
        
        MainWindow.SetMode ( "Connected" );
        
        SetAttach ();
        
        Haguichi.informationWindow.SetVersion ();
        Haguichi.informationWindow.SetAddress ();
        Haguichi.informationWindow.SetClientId ();
        
        GLib.Timeout.Add ( 2000, new GLib.TimeoutHandler ( SetNickAfterLogin ) );
        
        Controller.restoreConnection = false;
        Controller.numUpdateCycles ++;
        Controller.UpdateCycle ();
        
    }
    
    
    public static void ConnectionStopped ()
    {
        
        MainWindow.SetMode ( "Disconnected" );
        
        if ( Controller.restoreConnection )
        {
            Controller.RestoreConnectionCycle ();
            return;
        }
        
        Controller.continueUpdate = false; // Stop update interval
        
        Haguichi.connection.ClearNetworks ();
        
        if ( Hamachi.ApiVersion > 1 )
        {
            Controller.lastStatus = 4;
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            Controller.lastStatus = 3;
        }
        
        SetAttach ();
        
    }
    
    
    public static void About ( object obj, EventArgs args )
    {
        
        Haguichi.aboutDialog.Open ();
        
    }
    
    
    public static void Preferences ( object obj, EventArgs args )
    {
        
        Haguichi.preferencesWindow.Open ();
        
    }
    
    
    private static bool SetNickAfterLogin ()
    {
        
        string nick   = ( string ) Config.Client.Get ( Config.Settings.Nickname );
        string output = Hamachi.SetNick ( nick );
        
        if ( output.Contains ( ".. failed, busy" ) )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
    
    public static void UpdateNick ( string nick )
    {
        
        Haguichi.mainWindow.SetNick ( nick );
        Haguichi.informationWindow.SetNick ( nick );
        
    }
    
    
    public static void UpdateNick ()
    {
        
        string nick = ( string ) Config.Client.Get ( Config.Settings.Nickname );
        
        if ( Config.Settings.DemoMode )
        {
            UpdateNick ( "Joe Demo" );
        }
        else if ( nick.Length > 0 )
        {
            UpdateNick ( nick );
        }
        else
        {
            Thread thread = new Thread ( UpdateNickThread );
            thread.Start ();
        }
        
    }
    
    
    private static void UpdateNickThread ()
    {
        
        string nick = Hamachi.GetNick ();
        
        Config.Client.Set ( Config.Settings.Nickname, nick );
        
        UpdateNick ( nick );
         
     }
    
    
    public static void ChangeNick ( object obj, EventArgs args )
    {
        
        MainWindow.Show ();
        Dialogs.ChangeNick nickDlg = new Dialogs.ChangeNick ( TextStrings.changeNickTitle );
        
    }
    
    
    public static void Information ( object obj, EventArgs args )
    {
        
        Haguichi.informationWindow.Open ();
        
    }
    
    
    public static void JoinNetwork ( object obj, EventArgs args )
    {
        
        MainWindow.Show ();
        Dialogs.JoinCreate joinDlg = new Dialogs.JoinCreate ( "Join", TextStrings.joinNetworkTitle );
        
    }
    
    
    public static void CreateNetwork ( object obj, EventArgs args )
    {
        
        MainWindow.Show ();
        Dialogs.JoinCreate createDlg = new Dialogs.JoinCreate ( "Create", TextStrings.createNetworkTitle );
        
    }
    
    
    public static void Attach ( object obj, EventArgs args )
    {
        
        MainWindow.Show ();
        Dialogs.Attach attachDlg = new Dialogs.Attach ();
        
    }
    
    
    public static void SetAttach ()
    {
          
        if ( Hamachi.ApiVersion > 1 )
        {
            string account = Hamachi.GetAccount ();
            
            Haguichi.informationWindow.SetAccount ( account );
            
            if ( ( ( account == "" ) ||
                   ( account == "-" ) ) &&
                 ( Controller.lastStatus >= 6 ) )
            {
                MainWindow.menuBar.SetAttach ( true, true );
                MainWindow.quickMenu.SetAttach ( true, true );
            }
            else if ( ( account == "" ) ||
                      ( account == "-" ) )
            {
                MainWindow.menuBar.SetAttach ( true, false );
                MainWindow.quickMenu.SetAttach ( true, false );
            }
            else
            {
                MainWindow.menuBar.SetAttach ( false, false );
                MainWindow.quickMenu.SetAttach ( false, false );
            }
        }
        else
        {
            MainWindow.menuBar.SetAttach ( false, false );
            MainWindow.quickMenu.SetAttach ( false, false );
        }
        
        
    }
    
    
    public static void Help ( object obj, EventArgs args )
    {
        
        Command.OpenURL ( TextStrings.helpURL );
        
    }
    
    
    public static void QuitApp ( object obj, EventArgs args )
    {
        
        MainWindow.Hide ();
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.DisconnectOnQuit ) )
        {
            if ( ( Hamachi.ApiVersion > 1 ) &&
                 ( Controller.lastStatus > 4 ) )
            {
                Hamachi.Logout ();
            }
            else if ( ( Hamachi.ApiVersion == 1 ) &&
                      ( Controller.lastStatus > 3 ) )
            {
                Hamachi.Stop ();
            }
        }
        
        Debug.Log ( Debug.Domain.Environment, "GlobalEvents.QuitApp", "Unregistering process" );
        Platform.UnregisterProcess ();
        
        Application.Quit ();
        
    }
    
}
