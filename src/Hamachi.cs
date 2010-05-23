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


public class Hamachi
{
    
    public Hamachi ()
    {
    }
    

    public static string Retrieve ( string filename, string args, string nfo, int cut )
    {
        
        string rawOutput = Command.ReturnOutput ( filename, args );
        
        int startIndex = rawOutput.LastIndexOf ( nfo );
        string nfoOutput = rawOutput.Substring ( startIndex );
        
        int endIndex = nfoOutput.IndexOf ( "\n" ) - cut;
            
        return nfoOutput.Substring ( cut, endIndex );
        
    }
    

    public static string Init ()
    {
        
        string output = Command.ReturnOutput ( "hamachi-init", "" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Init", output );
        
        return output;
        
    }
    
    
    public static void TunCfg ()
    {
        
        string output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), ( string ) Config.Client.Get ( Config.Settings.CommandForTunCfg ) );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.TunCfg", output );
        
    }
    
    
    public static string Start ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "start" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Start", output );
        
        return output;
        
    }
    
    
    public static string Stop ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "stop" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Stop", output );
        
        return output;
    }
    
    
    public static string Login ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "login" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Login", output );
        
        return output;
    }
    
    
    public static string Logout ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "logout" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Logout", output );
        
        return output;
        
    }
    
    
    public static string GetState ()
    {
        
        string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
        string output =  Command.ReturnOutput ( "cat", filePath );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetState", output );
        
        return output;
        
    }
    
    
    public static string GetIdentity ()
    {
        
        string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
        string output = "";
        
        try
        {
            output = Retrieve ( "cat", filePath, "Identity", 11 );
        }
        catch {}
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetIdentity", output );
        
        return output;
        
    }
    
    
    public static string GetInfo ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetInfo", output );
        
        return output;
        
    }
    
    
    public static string GetNicks ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "get-nicks" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetNicks", output );
        
        return output;
        
    }
    
    
    public static void GoOnline ( string name )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "go-online '" + name + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOnline", output );
        
    }
    
    
    public static void GoOnline ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "go-online '" + network.Name + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOnline", output );
        
    }
    
    
    public static void GoOffline ( string name )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "go-offline '" + name + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOffline", output );
        
    }
    
    
    public static void GoOffline ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "go-offline '" + network.Name + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOffline", output );
        
    }
    
    
    public static void Delete ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "delete '" + network.Name + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Delete", output );

        if ( output.IndexOf ( ".. failed, you are not an owner" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedDeleteNetworkHeading, network.Name );
            string message = TextStrings.failedDeleteNetworkMessage;
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error" );
        }
        
    }
    
    
    public static void Leave ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "leave '" + network.Name + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Leave", output );

        if ( output.IndexOf ( ".. failed, you are an owner" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedLeaveNetworkHeading, network.Name );
            string message = TextStrings.failedLeaveNetworkMessage;
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error" );
        }
        
    }
    
    
    public static void Evict ( Member member )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "evict '" + member.Network + "' " + member.Address );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Evict", output );

        if ( output.IndexOf ( ".. failed, denied" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedEvictMemberHeading, member.Nick );
            string message = String.Format ( TextStrings.failedEvictMemberMessage, member.Network );
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error" );
        }
        
    }
    
    
    public static string GetNick ()
    {
        
        //return Retrieve ( "hamachi", "", "nickname :", 11 );
        
        string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
        string output = "";
        
        try
        {
            output = Retrieve ( "cat", filePath, "Nickname", 11 );
        }
        catch {}
        
        try
        {
            output = Retrieve ( "cat", filePath, "RenameTo", 11 );
        }
        catch {}
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetNick", output );
        
        return output; 
        
    }
    
    
    public static string GetVersion ()
    {
        
        string output;
        
        try
        {
            output = Retrieve ( "hamachi", "", "version  :", 23 );
        }
        catch
        {
            output = "";
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetVersion", output );
        
        return output;
        
    }
    
    
    public static string GetPID ()
    {
        
        string output;
        
        try
        {
            output = Retrieve ( "hamachi", "", "pid      :", 11 );
        }
        catch
        {
            output = "";
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetPID", output );
        
        return output;
        
    }
    
    
    public static string GetStatus ()
    {
        
        return Retrieve ( "hamachi", "", "status   :", 11 );
        
    }
    
    
    private static string GetList ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "list" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetList", "\n" + output );
        
        return output;
        
    }
    
     
    public static ArrayList ReturnList ()
    {
        
        ArrayList networks = new ArrayList ();
    
        string output = GetList ();
        
        string[] split = output.Split ( Environment.NewLine.ToCharArray () );
        string curNetName = "";
        
        foreach ( string s in split )
        {
           
            if ( s.Length > 5 ) // Check string for minimum chars
            {
            
                if ( s.IndexOf ( "[" ) == 3 ) // String contains network
                {
                    Status status = new Status ( s.Substring ( 1, 1 ) );
                    string name = s.Substring ( 4, s.IndexOf ( "]" ) - 4 );
                    //bool isOwner = CheckNetworkOwner ( name );
            
                    Network network = new Network ( status, name );
                    networks.Add ( network );
                    
                    curNetName = name;
                }
                
                if ( s.Length >= 49 )
                {
                    Status status = new Status ( s.Substring ( 5, 1 ) );
                    string address = s.Substring ( 7, 17 ).TrimEnd ();
                    string nick = s.Substring ( 24, 25 ).TrimEnd ();
                    
                    if ( ( nick == "" ) || ( nick == "anonymous" ) )
                    {
                        nick = TextStrings.anonymous;
                    }
                    
                    string tunnel = "";
                    
                    try
                    {
                        tunnel = s.Substring ( 51, 17 ).TrimEnd ();
                    }
                    catch ( Exception e )
                    {
                        // No tunnel
                    }

                    Member member = new Member ( status, curNetName, address, nick, tunnel );
                    
                    foreach (Network network in networks)
                    {
                        if ( network.Name == curNetName )
                        {
                            network.AddMember ( member );
                        }
                    }
                    
                }
                
            }
            
        }
        
        return networks;
    }
    
    
    public static string SetNick ( string nick )
    {
        
        string filePath;
        string output;
        
        if ( Controller.StatusCheck () < 3 )
        {
            filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
            output = Command.ReturnOutput ( "bash", "-c \"echo 'RenameTo   " + nick + "' >> " + filePath + "\"" );
        }
        else
        {
            output = Command.ReturnOutput ( "hamachi", "set-nick '" + nick + "'" );
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SetNick", output );
        
        return output;
        
    }
    
    
    public static string JoinNetwork ( string name, string password )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "join '" + name + "' '" + password + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.JoinNetwork", output );
        
        return output;
        
    }
    
    
    
    public static string CreateNetwork ( string name, string password )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "create '" + name + "' '" + password + "'" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.CreateNetwork", output );
        
        return output;
        
    }
    
}
