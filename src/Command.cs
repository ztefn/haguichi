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
using System.Diagnostics;
using System.Threading;

    
public static class Command
{
    
    private static int timeout;
    private static bool inProgress = false;
    
    public static string SudoArguments    = "";
    public static string SudoCommandStart = "-- ";
    public static string SudoCommandEnd   = "";
    
    
    public static void Init ()
    {
        
        SetTimeout ();
        DetermineSudo ();
        
    }
    
    
    public static void SetTimeout ()
    {
        
        double dTimeout = ( double ) Config.Client.Get ( Config.Settings.CommandTimeout );
        
        if ( dTimeout < 30.0 )
        {
            dTimeout = 60.0;
            Config.Client.Set ( Config.Settings.CommandTimeout, dTimeout );
        }
        
        timeout = ( int ) dTimeout;
        
    }
    
    
    public static void DetermineSudo ()
    {
        
        Thread thread = new Thread ( DetermineSudoThread );
        thread.Start ();
        
    }
    
    
    private static void DetermineSudoThread ()
    {
        
        string sudo    = "";
        string curSudo = ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser );
        
        if ( Exists ( curSudo ) )
        {
            sudo = curSudo;
        }
        else if ( Exists ( "gksudo" ) )
        {
            sudo = "gksudo";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        else if ( Exists ( "gksu" ) )
        {
            sudo = "gksu";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        else if ( Exists ( "gnomesu" ) )
        {
            sudo = "gnomesu";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        else if ( Exists ( "kdesudo" ) )
        {
            sudo = "kdesudo";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        else if ( Exists ( "kdesu" ) )
        {
            sudo = "kdesu";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        else if ( Exists ( "beesu" ) )
        {
            sudo = "beesu";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        else if ( Exists ( "sudo" ) )
        {
            sudo = "sudo";
            Config.Client.Set ( Config.Settings.CommandForSuperUser, sudo );
        }
        
        if ( sudo.StartsWith ( "gksu" ) )
        {
            SudoArguments = "-m \"" + TextStrings.enterPassword + "\" ";
        }
        else if ( sudo == "beesu" )
        {
            SudoCommandStart = "-c '";
            SudoCommandEnd   = "'";
        }
        
        Debug.Log ( Debug.Domain.Environment, "Settings.Init", "Command for sudo: " + sudo );
        
    }
    
    
    public static bool Exists ( string command )
    {
        
        string output = ReturnOutput ( "bash", "-c \"command -v " + command + " &>/dev/null || echo 'command not found'\"" );
        
        if ( output.Contains ( "command not found" ) )
        {
            return false;
        }
        else
        {
            return true;
        }
        
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
        catch
        {
            // Nothing
        }
        
    }
    
    
    public static string ReturnOutput ( string filename, string args )
    {
        
        string val = "error";
        
        if ( filename == "hamachi" )
        {
            while ( inProgress )
            {
                // Wait
            }
            
            inProgress = true;
        }
        
        try
        {
            ProcessStartInfo ps = new ProcessStartInfo ( filename, args );
            ps.UseShellExecute = false;
            ps.RedirectStandardOutput = true;
            
            using ( Process p = Process.Start ( ps ) )
            {
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
        catch
        {
            // Nothing
        }
        
        if ( filename == "hamachi" )
        {
            inProgress = false;
        }
        
        return val;
        
    }
    
    
    public static string ReturnCustom ( Member member, string commandIPv4, string commandIPv6, string priority )
    {
        
        string command = "";
        
        if ( Hamachi.IpVersion == "Both" )
        {
            if ( priority == "IPv4" )
            {
                if ( member.IPv4 != "" )
                {
                    command = commandIPv4;
                    command = command.Replace ( "%A", member.IPv4 );
                }
                else
                {
                    command = commandIPv6;
                    command = command.Replace ( "%A", member.IPv6 );
                }
            }
            if ( priority == "IPv6" )
            {
                if ( member.IPv6 != "" )
                {
                    command = commandIPv6;
                    command = command.Replace ( "%A", member.IPv6 );
                }
                else
                {
                    command = commandIPv4;
                    command = command.Replace ( "%A", member.IPv4 );
                }
            }
        }
        else if ( Hamachi.IpVersion == "IPv4" )
        {
            command = commandIPv4;
            command = command.Replace ( "%A", member.IPv4 );
        }
        else if ( Hamachi.IpVersion == "IPv6" )
        {
            command = commandIPv6;
            command = command.Replace ( "%A", member.IPv6 );
        }
        
        command = command.Replace ( "%N",  member.Nick     );
        command = command.Replace ( "%ID", member.ClientId );
        
        command = command.Replace ( "{COLON}", ";" );
        
        return command;
        
    }
    
    
    public static string [] ReturnDefault ()
    {
        
        string [] command  = new string [] { "" };
        string [] commands = ( string [] ) Config.Client.Get ( Config.Settings.CustomCommands );
        
        foreach ( string c in commands )
        {
            string [] cArray = c.Split ( new char [] { ';' }, 7 );
            
            if ( ( cArray.GetLength ( 0 ) == 7 ) &&
                 ( cArray [1] == "true" ) )
            {
                command = cArray;
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
