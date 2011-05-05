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
    
    public static void SetModalDialog ( Dialog dialog )
    {
        
        Haguichi.modalDialog = dialog;
        
        if ( Platform.IndicatorSession != null )
        {
            Platform.IndicatorSession.SetModality ( ( dialog != null ) );
        }
        
    }
    
    
    public static void StartHamachi ( object obj, EventArgs args )
    {
        
        StartHamachi ();
        
    }
    
    
    public static void StartHamachi ()
    {
        
        Controller.restoreCountdown = 0;
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
        
        Controller.restoreConnection = false;
        
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
        
        Thread thread = new Thread ( SetNickAfterLoginThread );
        thread.Start ();
        
        Controller.restoreConnection = ( bool ) Config.Client.Get ( Config.Settings.ReconnectOnConnectionLoss );
        Controller.numUpdateCycles ++;
        Controller.UpdateCycle ();
        
    }
    
    
    public static void ConnectionStopped ()
    {
        
        MainWindow.SetMode ( "Disconnected" );
        
        Controller.continueUpdate = false; // Stop update interval
        
        if ( Controller.restoreConnection )
        {
            if ( Controller.lastStatus == 2 )
            {
                Controller.WaitForInternetCycle ();
            }
            else
            {
                Controller.RestoreConnectionCycle ();
            }
            return;
        }
        
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
        
        About ();
        
    }
    
    
    public static void About ()
    {
        
        Haguichi.aboutDialog.Open ();
        
    }
    
    
    public static void Preferences ( object obj, EventArgs args )
    {
        
        Preferences ();
        
    }
    
    
    public static void Preferences ()
    {
        
        Haguichi.preferencesWindow.Open ();
        
    }
    
    
    private static void SetNickAfterLoginThread ()
    {
        
        string nick = ( string ) Config.Client.Get ( Config.Settings.Nickname );
        Hamachi.SetNick ( nick );
        
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
        
        ChangeNick ();
        
    }
    
    
    public static void ChangeNick ()
    {
        
        MainWindow.Show ();
        new Dialogs.ChangeNick ( TextStrings.changeNickTitle );
        
    }
    
    
    public static void Information ( object obj, EventArgs args )
    {
        
        Information ();
        
    }
    
    
    public static void Information ()
    {
        
        Haguichi.informationWindow.Open ();
        
    }
    
    
    public static void JoinNetwork ( object obj, EventArgs args )
    {
        
        JoinNetwork ();
        
    }
    
    
    public static void JoinNetwork ()
    {
        
        MainWindow.Show ();
        new Dialogs.JoinCreate ( "Join", TextStrings.joinNetworkTitle );
        
    }
    
    
    public static void CreateNetwork ( object obj, EventArgs args )
    {
        
        CreateNetwork ();
        
    }
    
    
    public static void CreateNetwork ()
    {
        
        MainWindow.Show ();
        new Dialogs.JoinCreate ( "Create", TextStrings.createNetworkTitle );
        
    }
    
    
    public static void Attach ( object obj, EventArgs args )
    {
        
        Attach ();
        
    }
    
    
    public static void Attach ()
    {
        
        MainWindow.Show ();
        new Dialogs.Attach ();
        
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
            }
            else if ( ( account == "" ) ||
                      ( account == "-" ) )
            {
                MainWindow.menuBar.SetAttach ( true, false );
            }
            else
            {
                MainWindow.menuBar.SetAttach ( false, false );
            }
        }
        else
        {
            MainWindow.menuBar.SetAttach ( false, false );
        }
        
    }
    
    
    public static void Help ( object obj, EventArgs args )
    {
        
        Help ();
        
    }
    
    
    public static void Help ()
    {
        
        Command.OpenURL ( TextStrings.helpURL );
        
    }
    
    
    public static void QuitApp ( object obj, EventArgs args )
    {
        
        QuitApp ();
        
    }
    
    public static void QuitApp ()
    {
        
        MainWindow.Hide ();
        
        if ( Platform.IndicatorSession != null )
        {
            Platform.IndicatorSession.QuitApp ();
        }
        
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
        Platform.UnregisterSession ();
        
        Application.Quit ();
        
    }
    
}
