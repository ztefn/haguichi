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
        
        int status = Controller.StatusCheck ();
        
        if ( Config.Settings.DemoMode )
        {
            ConnectionEstablished ();
        }
        else if ( status == 2 )
        {
            Dialogs.Message dlg1 = new Dialogs.Message ( TextStrings.connectErrorHeading, TextStrings.connectErrorNoInternetConnection, "Error" );
        }
        else
        {
            MainWindow.SetMode ( "Connecting" );
            GLib.Timeout.Add ( 10, new GLib.TimeoutHandler ( Controller.ConnectAfterTimeout ) );
        }
        
    }
    
    
    public static void StopHamachi ( object obj, EventArgs args )
    {
        
        if ( Config.Settings.DemoMode )
        {
            // Do nothing
        }
        else if ( Hamachi.apiVersion > 1 )
        {
            Hamachi.Logout ();
        }
        else if ( Hamachi.apiVersion == 1 )
        {
            Hamachi.Stop ();
        }
        
        ConnectionStopped ();
        
    }

    
    public static void RunTunCfg ( object obj, EventArgs args )
    {
        
        Hamachi.TunCfg ();
        
    }
    
    
    public static void ConnectionEstablished ()
    {
        
        MainWindow.SetMode ( "Connected" );
        
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
        
        int status = Controller.lastStatus;
        
        if ( status >= 2 )
        {
            MainWindow.SetMode ( "Disconnected" );
        }
        else if ( status >= 1 )
        {
            MainWindow.SetMode ( "Not configured" );
        }
        else
        {
            MainWindow.SetMode ( "Not installed" );
        }
        
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
            Thread updateThread = new Thread ( UpdateNickThread );
            updateThread.Start();
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
        
        Debug.Log ( Debug.Domain.Enviroment, "GlobalEvents.QuitApp", "Unregistering process" );
        Platform.UnregisterProcess ();
        
        Application.Quit ();
        
    }
    
}
