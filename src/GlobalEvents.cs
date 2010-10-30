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
        
        Controller.StatusCheck ();
        
        if ( Config.Settings.DemoMode )
        {
            ConnectionEstablished ();
        }
        else if ( Controller.lastStatus == 2 )
        {
            Dialogs.Message msgDlg = new Dialogs.Message ( TextStrings.connectErrorHeading, TextStrings.connectErrorNoInternetConnection, "Error" );
        }
        else
        {
            MainWindow.SetMode ( "Connecting" );
            Controller.GoConnect ();
        }
        
    }
    
    
    public static void StopHamachi ( object obj, EventArgs args )
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
        
        if ( Config.Settings.SetNickAfterLogin == true )
        {
            GLib.Timeout.Add ( 2000, new GLib.TimeoutHandler ( SetNickAfterLogin ) );
        }
        
        UpdateCycle ();
        
    }
    
    
    private static bool SetNickAfterLogin ()
    {
        
        Hamachi.SetNick ( Config.Settings.LastNick );
        UpdateNick ( Config.Settings.LastNick );
        
        return false;
        
    }
    
    
    private static void UpdateCycle ()
    {
        
        uint interval = ( uint ) ( 1000 * ( double ) Config.Client.Get ( Config.Settings.UpdateInterval ) );
        
        Controller.numUpdateCycles += 1;
        Controller.continueUpdate = true;
        
        GLib.Timeout.Add ( interval, new GLib.TimeoutHandler ( Controller.UpdateConnection ) );
        
    }
    
    
    
    public static void WaitForInternetCycle ()
    {
        
        uint interval = ( uint ) ( 1000 );
        
        GLib.Timeout.Add ( interval, new GLib.TimeoutHandler ( Controller.WaitForInternet ) );
        
    }
    
    
    public static void ConnectionLost ()
    {
        
        ConnectionStopped ();
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.ReconnectOnConnectionLoss ) )
        {
            WaitForInternetCycle ();
        }
        
    }
    
    
    public static void ConnectionStopped ()
    {
        
        /* Stop update interval */
        Controller.continueUpdate = false;
        
        Haguichi.connection.ClearNetworks ();
        Haguichi.connection.Status = new Status ( " " );
        
        if ( Hamachi.ApiVersion > 1 )
        {
            Controller.lastStatus = 4;
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            Controller.lastStatus = 3;
        }
        
        MainWindow.SetMode ( "Disconnected" );
        
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
    
    
    public static void UpdateNick ( string nick )
    {
        
        Config.Settings.LastNick = nick;
        
        Haguichi.mainWindow.SetNick ( nick );
        Haguichi.nickWindow.SetNick ( nick );
        Haguichi.informationWindow.SetNick ( nick );
        
    }
    
    
    public static void UpdateNick ()
    {
        
        if ( Config.Settings.LastNick.Length > 0 )
        {
            UpdateNick ( Config.Settings.LastNick );
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
        UpdateNick ( nick );
        
    }
    
    
    public static void ChangeNick ( object obj, EventArgs args )
    {
        
        Haguichi.nickWindow.Open ();
        
    }
    
    
    public static void Information ( object obj, EventArgs args )
    {
        
        Haguichi.informationWindow.Open ();
        
    }
    
    
    public static void JoinNetwork ( object obj, EventArgs args )
    {
        
        Haguichi.joinWindow.Open ();
        
    }
    
    
    public static void CreateNetwork ( object obj, EventArgs args )
    {
        
        Haguichi.createWindow.Open ();
        
    }
    
    
    public static void Attach ( object obj, EventArgs args )
    {
        
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
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.DisconnectOnQuit ) )
        {
            StopHamachi ( obj, args );
        }
        
        Debug.Log ( Debug.Domain.Environment, "GlobalEvents.QuitApp", "Unregistering process" );
        Platform.UnregisterProcess ();
        
        Application.Quit ();
        
    }
    
}
