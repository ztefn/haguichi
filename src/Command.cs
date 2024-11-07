/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2024 Stephen Brandt <stephen@stephenbrandt.com>
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
using System.Text.RegularExpressions;
using System.Threading;


public static class Command
{
    
    private static int timeout;
    
    public static string Sudo;
    public static string SudoArgs;
    public static string SudoStart;
    
    public static string Terminal;
    public static string FileManager;
    public static string RemoteDesktop;
    
    
    public static void Init ()
    {
        
        SetTimeout ();
        
        DetermineSudo ();
        DetermineTerminal ();
        DetermineFileManager ();
        DetermineRemoteDesktop ();
        
    }
    
    
    public static void SetTimeout ()
    {
        
        double dTimeout = ( double ) Config.Settings.CommandTimeout.Value;
        
        if ( dTimeout < 30.0 )
        {
            dTimeout = 60.0;
            Config.Settings.CommandTimeout.Value = dTimeout;
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
        
        Sudo      = "";
        SudoArgs  = "";
        SudoStart = "-- ";
        
        string sudoCommand     = ( string ) Config.Settings.CommandForSuperUser.Value;
        string [] sudoCommands = { "pkexec", "gksudo", "gksu", "gnomesu", "kdesudo", "kdesu", "sudo" };
        
        if ( ( Array.Exists ( sudoCommands, delegate ( string s ) { return s.Equals ( sudoCommand ); } ) ) &&
             ( Exists ( sudoCommand ) ) )
        {
            Sudo = sudoCommand;
        }
        else
        {
            foreach ( string c in sudoCommands )
            {
                if ( ( Sudo == "" ) &&
                     ( Exists ( c ) ) )
                {
                    Sudo = c;
                }
            }
        }
        
        if ( Sudo == "pkexec" )
        {
            SudoStart = "";
        }
        else if ( Sudo.StartsWith ( "gksu" ) )
        {
            SudoArgs = "--sudo-mode -D \"" + TextStrings.appName + "\" ";
        }
        
        Debug.Log ( Debug.Domain.Environment, "Command.DetermineSudoThread", "Command for sudo: " + Sudo );
        
    }
    
    
    public static void DetermineTerminal ()
    {
        
        Thread thread = new Thread ( DetermineTerminalThread );
        thread.Start ();
        
    }
    
    
    private static void DetermineTerminalThread ()
    {
        
        Terminal = "gnome-terminal";
        
        if ( Exists ( "gnome-terminal" ) )
        {
            // Keep
        }
        else if ( Exists ( "mate-terminal" ) )
        {
            Terminal = "mate-terminal";
        }
        else if ( Exists ( "pantheon-terminal" ) )
        {
            Terminal = "pantheon-terminal";
        }
        else if ( Exists ( "xfce4-terminal" ) )
        {
            Terminal = "xfce4-terminal";
        }
        else if ( Exists ( "konsole" ) )
        {
            Terminal = "konsole";
        }
        else if ( Exists ( "xterm" ) )
        {
            Terminal = "xterm";
        }
        
        Debug.Log ( Debug.Domain.Environment, "Command.DetermineTerminalThread", "Command for terminal: " + Terminal );
        
    }
    
    
    public static void DetermineFileManager ()
    {
        
        Thread thread = new Thread ( DetermineFileManagerThread );
        thread.Start ();
        
    }
    
    
    private static void DetermineFileManagerThread ()
    {
        
        FileManager = "nautilus";
        
        if ( Exists ( "nautilus" ) )
        {
            // Keep
        }
        else if ( Exists ( "caja" ) )
        {
            FileManager = "caja";
        }
        else if ( Exists ( "nemo" ) )
        {
            FileManager = "nemo";
        }
        else if ( Exists ( "pantheon-files" ) )
        {
            FileManager = "pantheon-files";
        }
        else if ( Exists ( "thunar" ) )
        {
            FileManager = "thunar";
        }
        else if ( Exists ( "dolphin" ) )
        {
            FileManager = "dolphin";
        }
        
        Debug.Log ( Debug.Domain.Environment, "Command.DetermineFileManagerThread", "Command for file manager: " + FileManager );
        
    }
    
    
    public static void DetermineRemoteDesktop ()
    {
        
        Thread thread = new Thread ( DetermineRemoteDesktopThread );
        thread.Start ();
        
    }
    
    
    private static void DetermineRemoteDesktopThread ()
    {
        
        RemoteDesktop = "vinagre";
        
        if ( Exists ( "vinagre" ) )
        {
            // Keep
        }
        else if ( Exists ( "krdc" ) )
        {
            RemoteDesktop = "krdc";
        }
        
        Debug.Log ( Debug.Domain.Environment, "Command.DetermineRemoteDesktopThread", "Command for remote desktop: " + RemoteDesktop );
        
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
            Process p = new Process ();
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.Start ();
            p.Close ();
            p.Dispose ();
        }
        catch
        {
            // Nothing
        }
        
    }
    
    
    public static string ReturnOutput ( string filename, string args )
    {
        
        string output = "error";
        
        try
        {
            output = "";
            
            Process p = new Process ();
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += ( o, e ) => { output += e.Data + "\n"; };    
            p.Start ();
            p.BeginOutputReadLine ();
            
            if ( !p.WaitForExit ( 1000 * timeout ) )
            {
                output = "timeout";
                p.Kill ();
            }

            p.WaitForExit ();
            p.Close ();
            p.Dispose ();
        }
        catch
        {
            // Nothing
        }
        
        if ( output.Contains ( ".. failed, busy" ) ) // Keep trying until it's not busy anymore
        {
            Debug.Log ( Debug.Domain.Hamachi, "Command.ReturnOutput", "Hamachi is busy, waiting to try again..." );
            
            Thread.Sleep ( 100 );
            output = ReturnOutput ( filename, args );
        }
        
        return output;
        
    }
    
    
    public static string ReturnCustom ( Member member, string commandIPv4, string commandIPv6, string priority )
    {
        
        string command = "";
        string address = "";
        
        if ( Hamachi.IpVersion == "Both" )
        {
            if ( priority == "IPv4" )
            {
                if ( member.IPv4 != "" )
                {
                    command = commandIPv4;
                    address = member.IPv4;
                }
                else
                {
                    command = commandIPv6;
                    address = member.IPv6;
                }
            }
            if ( priority == "IPv6" )
            {
                if ( member.IPv6 != "" )
                {
                    command = commandIPv6;
                    address = member.IPv6;
                }
                else
                {
                    command = commandIPv4;
                    address = member.IPv4;
                }
            }
        }
        else if ( Hamachi.IpVersion == "IPv4" )
        {
            command = commandIPv4;
            address = member.IPv4;
        }
        else if ( Hamachi.IpVersion == "IPv6" )
        {
            command = commandIPv6;
            address = member.IPv6;
        }
        
        return ReplaceVariables ( command, address, member.Nick, member.ClientId );
        
    }
    
    
    public static string ReplaceVariables ( string command, string address, string nick, string id )
    {
        
        command = command.Replace ( "%A",  address );
        command = command.Replace ( "%N",  nick    );
        command = command.Replace ( "%ID", id      );
        
        string quote = "\"";
        
        if ( Terminal == "konsole" )
        {
            quote = "";   
        }
        
        command = Regex.Replace ( command, "%TERMINAL (.*)", Terminal + " -e " + quote + "$1" + quote, RegexOptions.Singleline );
        
        command = command.Replace ( "%FILEMANAGER",   FileManager   );
        command = command.Replace ( "%REMOTEDESKTOP", RemoteDesktop );
        
        command = command.Replace ( "{COLON}", ";" );
        
        return command;
        
    }
    
    
    public static string [] ReturnDefault ()
    {
        
        string [] command  = new string [] { "" };
        string [] commands = ( string [] ) Config.Settings.CustomCommands.Value;
        
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
