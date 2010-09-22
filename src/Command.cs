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
using System.ComponentModel;
using System.Diagnostics;

    
public static class Command
{
    
    private static bool inProgress = false;
    
    
    public static void DetermineSudo ()
    {
        
        BackgroundWorker worker = new BackgroundWorker {};
        
        worker.DoWork += DetermineSudoThread;
        worker.RunWorkerAsync ();
        
    }
    
    
    private static void DetermineSudoThread ( object sender, DoWorkEventArgs e )
    {
        
        string sudo    = "sudo";
        string curSudo = ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser );
        
        if ( ReturnOutput ( curSudo, "--help" ) != "error" )
        {
            sudo = curSudo;
        }   
        if ( ReturnOutput ( "gksudo", "--help" ) != "error" )
        {
            Config.Client.Set ( Config.Settings.CommandForSuperUser, "gksudo" );
            sudo = "gksudo";
        }
        if ( ReturnOutput ( "gnomesu", "--help" ) != "error" )
        {
            Config.Client.Set ( Config.Settings.CommandForSuperUser, "gnomesu" );
            sudo = "gnomesu";
        }
        if ( ReturnOutput ( "kdesudo", "--help" ) != "error" )
        {
            Config.Client.Set ( Config.Settings.CommandForSuperUser, "kdesudo" );
            sudo = "kdesudo";
        }
        if ( ReturnOutput ( "kdesu", "--help" ) != "error" )
        {
            Config.Client.Set ( Config.Settings.CommandForSuperUser, "kdesu" );
            sudo = "kdesu";
        }
        
        Debug.Log ( Debug.Domain.Environment, "Settings.Init", "Command for sudo: " + sudo );
        
    }
    
    
    public static void Execute ( string command )
    {
        
        string [] commands = command.Split ( new char [] { ' ' }, 2 );
        
        if ( commands.GetLength ( 0 ) == 1 )
        {
            Execute ( commands [0], "" );
        }
        if ( commands.GetLength ( 0 ) == 2 )
        {
            Execute ( commands [0], commands [1] );
        }
        
    }
    
    
    public static void Execute ( string filename, string args )
    {
        
        try
        {
            ProcessStartInfo ps = new ProcessStartInfo ( filename, args );
            ps.UseShellExecute = false;
            ps.RedirectStandardOutput = true;
            
            using ( Process p = Process.Start ( ps ) )
            {
                p.Close ();
                p.Dispose ();
            }
        }
        catch ( Exception e )
        {
            // Nothing
        }
        
    }
    
    
    public static string ReturnOutput ( string filename, string args )
    {
        
        string val = "error";
        
        while ( inProgress )
        {
            // Wait
        }
        
        inProgress = true;
        
        try
        {
            ProcessStartInfo ps = new ProcessStartInfo ( filename, args );
            ps.UseShellExecute = false;
            ps.RedirectStandardOutput = true;
            
            using ( Process p = Process.Start ( ps ) )
            {
                int timeout = ( int ) ( ( double ) Config.Client.Get ( Config.Settings.CommandTimeout ) );
                
                if ( p.WaitForExit ( 1000 * timeout ) )
                {
                    val = p.StandardOutput.ReadToEnd ();
                }
                else
                {
                    val = "timeout";
                }
                p.Close ();
                p.Dispose ();
            }
        }
        catch ( Exception e )
        {
            // Nothing
        }
        
        inProgress = false;
        
        return val;
        
    }
    
    
    public static string ReturnDefault ()
    {
        
        string command = "";
        string [] commands = ( string [] ) Config.Client.Get ( Config.Settings.CustomCommands );
        
        foreach ( string c in commands )
        {
            
            string[] cArray = c.Split ( new char[] { ';' }, 5 );
            
            if ( ( cArray.GetLength ( 0 ) == 5 ) && ( cArray [ 1 ] == "true" ) )
            {
                command = cArray [ 4 ];
            }

        }
        
        return command;
    }
    
    
    public static void OpenURL ( string url )
    {
        
        try
        {
            Process.Start ( url );
        }
        catch
        {
            Debug.Log ( Debug.Domain.Error, "Command.OpenURL", "Failed opening URL '" + url + "'" );
        }
        
    }
    
}
