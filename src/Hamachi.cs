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
using System.Text.RegularExpressions;


public static class Hamachi
{
    
    public  static int ApiVersion;
    private static string lastInfo;
    private static Random random;
    
    
    public static void Init ()
    {
        
        GetInfo ();
        ApiVersion = DetermineApiVersion ();
        
    }
    
    
    public static int DetermineApiVersion ()
    {
        
        /*
         *  <= 0.9.9.9-20 : 1
         *  >= 2.0.0.11   : 2
         * 
         *  Returns version string for all Hamachi versions if started 1 2
         * 
         *  1. Not installed                                                                                    0
         * 
         *  2. Not configured:
         *     a. Cannot find configuration directory /home/stephen/.hamachi. Have you run 'hamachi-init' ?     1
         * 
         *  3. Not started:
         *     a. Hamachi does not seem to be running. Have you run 'hamachi start' ?                           1
         * 
         *     b. Hamachi does not seem to be running.                                                          2
         *        Run '/etc/init.d/logmein-hamachi start' to start daemon.
         * 
         * 
         */
        
        if ( Config.Settings.DemoMode )
        {
            return 2;
        }
        
        string output = lastInfo;
        
        if ( output == "error" ) // 'bash: hamachi: command not found' causes exception
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "0" );
            return 0;
        }
        
        if ( ( output.IndexOf ( "Have you run 'hamachi-init' ?" ) != -1 ) ||
             ( output.IndexOf ( "Have you run 'hamachi start' ?" ) != -1 ) ||
             ( output.IndexOf ( "hamachi-lnx-0.9.9.9" ) != -1 ) )
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "1" );
            return 1;
        }
        
        if ( ( output.IndexOf ( "Run '/etc/init.d/logmein-hamachi start' to start daemon." ) != -1 ) ||
             ( output.IndexOf ( "You do not have permission to control the hamachid daemon." ) != -1 ) )
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "2" );
            return 2;
        }
        
        Regex regex = new Regex ( "version([ ]+):([ ]+)(.+)" );
        Version version = new Version ( regex.Match ( output ).Groups[3].ToString () );
        if ( version.Major == 0 )
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "1" );
            return 1;
        }
        if ( version.Major >= 2 )
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "2" );
            return 2;
        }
        
        return -1;
        
    }
    

    public static string Retrieve ( string filename, string args, string nfo, int cut )
    {
        
        string rawOutput = Command.ReturnOutput ( filename, args );
        
        int startIndex = rawOutput.LastIndexOf ( nfo );
        string nfoOutput = rawOutput.Substring ( startIndex );
        
        int endIndex = nfoOutput.IndexOf ( "\n" ) - cut;
            
        return nfoOutput.Substring ( cut, endIndex );
        
    }
    
    
    public static string Retrieve ( string filename, string args, string nfo )
    {
        
        string output = Command.ReturnOutput ( filename, args );
        
        return Retrieve ( output, nfo );
        
    }
    
    
    public static string Retrieve ( string output, string nfo )
    {
        
        Regex regex = new Regex ( nfo + "([ ]*):([ ]+)(.+)" );
            
        return regex.Match ( output ).Groups[3].ToString ();
        
    }
    
    
    public static void Configure ()
    {
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), "-- bash -c \"echo 'Ipc.User      " + System.Environment.UserName + "' >> /var/lib/logmein-hamachi/h2-engine-override.cfg; /etc/init.d/logmein-hamachi restart\"" );
            
            if ( output.IndexOf ( "Restarting LogMeIn Hamachi VPN tunneling engine logmein-hamachi" ) != -1 )
            {
                Controller.Init ();
            }
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            output = Command.ReturnOutput ( "hamachi-init", "" );
            
            if ( output.IndexOf ( "Authentication information has been created." ) != -1 )
            {
                Config.Client.Set ( Config.Settings.HamachiDataPath, Config.Settings.DefaultHamachiDataPath );
                Controller.Init ();
            }
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Configure", output );
        
    }
    
    
    public static void TunCfg ()
    {
        
        string output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), ( string ) Config.Client.Get ( Config.Settings.CommandForTunCfg ) );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.TunCfg", output );
        
    }
    
    
    public static string Start ()
    {
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), "/etc/init.d/logmein-hamachi start" );
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            output = Command.ReturnOutput ( "hamachi", "start" );
            if ( output == "" )
            {
                output = "empty";
            }
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Start", output );
        
        return output;
        
    }
    
    
    public static string Stop ()
    {
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), "/etc/init.d/logmein-hamachi stop" );
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            output = Command.ReturnOutput ( "hamachi", "stop" );
        }
        
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
    
    
    public static string GetAccount ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return "-";
        }
        
        string output;
        
        try
        {
            output = Retrieve ( lastInfo, "lmi account" );
        }
        catch
        {
            output = "";
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetAccount", output );
        
        return output;
        
    }
    
    
    public static string GetClientId ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return "090-123-456";
        }
        
        string output;
        
        try
        {
            output = Retrieve ( lastInfo, "client id" );
        }
        catch
        {
            output = "";
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetClient", output );
        
        return output;
        
    }
    
    
    public static string GetAddress ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return "5.123.456.789";
        }
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            try
            {
                output = Retrieve ( lastInfo, "address" );
            }
            catch {}
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
            
            try
            {
                output = Retrieve ( "cat", filePath, "Identity", 11 );
            }
            catch {}
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetAddress", output );
        
        return output;
        
    }
    
    
    public static string GetInfo ()
    {
        
        lastInfo = Command.ReturnOutput ( "hamachi", "" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetInfo", lastInfo );
        
        return lastInfo;
        
    }
    
    
    public static string GetNicks ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "get-nicks" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetNicks", output );
        
        return output;
        
    }
    
    
    public static string GoOnline ( string id )
    {
        
        string output = "";
        
        if ( !Config.Settings.DemoMode )
        {
            output = Command.ReturnOutput ( "hamachi", "go-online \"" + Utilities.CleanString ( id ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOnline", output );
        }
        
        return output;
        
    }
    
    
    public static string GoOnline ( Network network )
    {
        
        return GoOnline ( network.Id );
        
    }
    
    
    public static string GoOffline ( string id )
    {
        
        string output = "";
        
        if ( !Config.Settings.DemoMode )
        {
            output = Command.ReturnOutput ( "hamachi", "go-offline \"" + Utilities.CleanString ( id ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOffline", output );
        }
        
        return output;
        
    }
    
    
    public static string GoOffline ( Network network )
    {
        
        return GoOffline ( network.Id );
        
    }
    
    
    public static void Delete ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "delete \"" + Utilities.CleanString ( network.Id ) + "\"" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Delete", output );

        if ( output.IndexOf ( ".. failed, you are not an owner" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedDeleteNetworkHeading, network.Name );
            string message = TextStrings.failedDeleteNetworkMessage;
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error", output );
        }
        
    }
    
    
    public static void Leave ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "leave \"" + Utilities.CleanString ( network.Id ) + "\"" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Leave", output );

        if ( output.IndexOf ( ".. failed, you are an owner" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedLeaveNetworkHeading, network.Name );
            string message = TextStrings.failedLeaveNetworkMessageIsOwner;
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error", output );
        }
        else if ( output.IndexOf ( ".. failed, denied" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedLeaveNetworkHeading, network.Name );
            string message = TextStrings.failedLeaveNetworkMessageDenied;
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error", output );
        }
        
    }
    
    
    public static void Approve ( Member member )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "approve \"" + Utilities.CleanString ( member.Network ) + "\" " + member.ClientId );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Approve", output );
        
    }
    
    
    public static void Reject ( Member member )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "reject \"" + Utilities.CleanString ( member.Network ) + "\" " + member.ClientId );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Reject", output );
        
    }
    
    
    public static void Evict ( Member member )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "evict \"" + Utilities.CleanString ( member.Network ) + "\" " + member.ClientId );
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Evict", output );

        if ( output.IndexOf ( ".. failed, denied" ) != -1 )
        {
            string heading = String.Format ( TextStrings.failedEvictMemberHeading, member.Nick );
            string message = String.Format ( TextStrings.failedEvictMemberMessage, member.Network );
            
            Dialogs.Message delDlg = new Dialogs.Message ( heading, message, "Error", output );
        }
        
    }
    
    
    public static string GetNick ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return "Joe Demo";
        }
        
        string nick = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            nick = Retrieve ( "hamachi", "", "nickname" );
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
            
            try
            {
                nick = Retrieve ( "cat", filePath, "Nickname", 11 );
            }
            catch {}
            
            try
            {
                nick = Retrieve ( "cat", filePath, "RenameTo", 11 );
            }
            catch {}
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetNick", nick );
        
        return nick;
        
    }
    
    
    public static string GetVersion ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return "2.0.0.11";
        }
        
        string output;
        
        try
        {
            output = Retrieve ( lastInfo, "version" );
            output = output.Replace ( "hamachi-lnx-", "" );
        }
        catch
        {
            output = "";
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetVersion", output );
        
        return output;
        
    }
    
    
    public static string GetStatus ()
    {
        
        return Retrieve ( "hamachi", "", "status" );
        
    }
    
    
    private static string GetList ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "list" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetList", "\n" + output );
        
        return output;
        
    }
    
    
    public static string RandomAddress ()
    {
        
        string address  = "5.";
               address += random.Next ( 100, 255 );
               address += ".";
               address += random.Next ( 100, 255 );
               address += ".";
               address += random.Next ( 100, 255 );
        
        return address;
        
    }
    
    
    public static string RandomClientId ()
    {
        
        string id  = "0";
               id += random.Next ( 90, 95 );
               id += "-";
               id += random.Next ( 100, 999 );
               id += "-";
               id += random.Next ( 100, 999 );
        
        return id;
        
    }
    
    
    public static string RandomNetworkId ()
    {
        
        string id  = "0";
               id += random.Next ( 40, 45 );
               id += "-";
               id += random.Next ( 100, 999 );
               id += "-";
               id += random.Next ( 100, 999 );
        
        return id;
        
    }
    
    
    public static ArrayList ReturnList ()
    {
        
        ArrayList networks = new ArrayList ();
        string output = "";
        
        if ( Config.Settings.DemoMode )
        {
            random = new Random ();
            
            output  = " * [" + RandomNetworkId () + "]  Contributors               \n";
            output += "       " + RandomClientId () + "   Enrico                     " + RandomAddress () + "\n";
            output += "     x " + RandomClientId () + "   Holmen                     " + RandomAddress () + "\n";
            output += "     * " + RandomClientId () + "   scrawl                     " + RandomAddress () + "\n";
            output += "       " + RandomClientId () + "   Sergey                     " + RandomAddress () + "\n";
            output += "     ? " + RandomClientId () + " \n";
            output += " * [" + RandomNetworkId () + "]  Portal Ubuntu              \n";
            output += "     x 092-466-858   Soker                      " + RandomAddress () + "\n";
            output += "   [" + RandomNetworkId () + "]  WebUpd8                    \n";
            output += "       094-409-761   Andrew                     " + RandomAddress () + "\n";
            output += "     * " + RandomClientId () + "   MastroPino                 " + RandomAddress () + "\n";
        }
        else
        {
            output = GetList ();
        }
        
        string [] split = output.Split ( Environment.NewLine.ToCharArray () );
        string curNetworkId = "";
        
        int peerMinLength  = 0;
        
        int clientStart    = 0;
        int clientLength   = 0;
        int addressStart   = 0;
        int addressLength  = 0;
        int nickStart      = 0;
        int nickLength     = 0;
        int tunnelStart    = 0;
        
        if ( Hamachi.ApiVersion > 1 )
        {
            peerMinLength  = 61;
        
            clientStart    = 7;
            clientLength   = 12;
            addressStart   = 48;
            addressLength  = 13;
            nickStart      = 21;
            nickLength     = 25;
            tunnelStart    = 80;
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            peerMinLength  = 49;
            
            clientStart    = 7;
            clientLength   = 17;
            addressStart   = 7;
            addressLength  = 17;
            nickStart      = 24;
            nickLength     = 25;
            tunnelStart    = 51;
        }
        
        foreach ( string s in split )
        {
           
            if ( s.Length > 5 ) // Check string for minimum chars
            {
            
                if ( s.IndexOf ( "[" ) == 3 ) // String contains network
                {
                    
                    Status status = new Status ( s.Substring ( 1, 1 ) );
                    string id     = s.Substring ( 4, s.IndexOf ( "]" ) - 4 );
                    string name   = id;
                    
                    try
                    {
                        name = s.Substring ( s.IndexOf ( "]" ) + 3, 27 ).TrimEnd ();
                        
                        if ( name.Length == 0 )
                        {
                            name = id;
                        }
                    }
                    catch ( Exception e )
                    {
                        // No name
                    }

                    Network network = new Network ( status, id, name );
                    networks.Add ( network );
                    
                    curNetworkId = id;
                    
                }
                else if ( s.IndexOf ( "?" ) == 5 ) // Unapproved peer
                {
                    
                    Status status = new Status ( s.Substring ( 5, 1 ) );
                    string client = s.Substring ( clientStart, clientLength ).TrimEnd ();
                    string nick   = TextStrings.unknown;
                    
                    Member member = new Member ( status, curNetworkId, "", nick, client, "" );
                    
                    foreach (Network network in networks)
                    {
                        if ( network.Id == curNetworkId )
                        {
                            network.AddMember ( member );
                        }
                    }
                    
                }
                else if ( s.Length >= peerMinLength )
                {
                    
                    Status status = new Status ( s.Substring ( 5, 1 ) );
                    string client = s.Substring ( clientStart, clientLength ).TrimEnd ();
                    string address = s.Substring ( addressStart, addressLength ).TrimEnd ();
                    string nick = s.Substring ( nickStart, nickLength ).TrimEnd ();
                    
                    if ( ( nick == "" ) || ( nick == "anonymous" ) )
                    {
                        nick = TextStrings.anonymous;
                    }
                    
                    string tunnel = "";
                    
                    try
                    {
                        tunnel = s.Substring ( tunnelStart ).TrimEnd ();
                    }
                    catch ( Exception e )
                    {
                        // No tunnel
                    }

                    Member member = new Member ( status, curNetworkId, address, nick, client, tunnel );
                    
                    if ( ( nick.Length == 25 ) &&
                         ( Hamachi.ApiVersion > 1 ) )
                    {
                        member.GetLongNick ();
                    }
                    
                    foreach (Network network in networks)
                    {
                        if ( network.Id == curNetworkId )
                        {
                            network.AddMember ( member );
                        }
                    }
                    
                }
                
            }
            
        }
        
        return networks;
    }
    
    
    public static void SetNick ( string nick )
    {
        
        if ( !Config.Settings.DemoMode )
        {
            Config.Settings.SetNickAfterLogin = false;
            
            string output = "";
            int status = Controller.lastStatus;
            
            if ( ( Hamachi.ApiVersion == 1 ) &&
                 ( status < 4 ) )
            {
                string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
                output = Command.ReturnOutput ( "bash", "-c \"echo 'RenameTo   " + nick + "' >> " + filePath + "\"" );
            }
            else if ( ( Hamachi.ApiVersion == 1 ) &&
                      ( status >= 4 ) )
            {
                output = Command.ReturnOutput ( "hamachi", "set-nick \"" + Utilities.CleanString ( nick ) + "\"" );
            }
            else if ( ( Hamachi.ApiVersion > 1 ) &&
                      ( status >= 6 ) )
            {
                output = Command.ReturnOutput ( "hamachi", "set-nick \"" + Utilities.CleanString ( nick ) + "\"" );
            }
            else if ( Hamachi.ApiVersion > 1 )
            {
                Config.Settings.SetNickAfterLogin = true;   
            }
            
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SetNick", output );
        }
        
    }
    
    
    public static string Attach ( string accountId, bool withNetworks )
    {
        
        string output  = "";
        string command = "attach";
        
        if ( withNetworks )
        {
            command += "-net";
        }
        
        output = Command.ReturnOutput ( "hamachi", command + " \"" + Utilities.CleanString ( accountId ) + "\"" );
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Attach", output );
        
        return output;
        
    }
    
    
    public static string SendJoinRequest ( string name, string password )
    {
        
        string output = "";
        
        if ( !Config.Settings.DemoMode )
        {
            output = Command.ReturnOutput ( "hamachi", "do-join \"" + Utilities.CleanString ( name ) + "\" \"" + Utilities.CleanString ( password ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SendJoinRequest", output );
        }
        
        return output;
        
    }
    
    
    public static string JoinNetwork ( string name, string password )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "join \"" + Utilities.CleanString ( name ) + "\" \"" + Utilities.CleanString ( password ) + "\"" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.JoinNetwork", output );
        
        return output;
        
    }
    
    
    public static string SetAccess ( string networkId, string locking, string approve )
    {
        
        string output = "";
        
        if ( !Config.Settings.DemoMode )
        {
            output = Command.ReturnOutput ( "hamachi", "set-access \"" + Utilities.CleanString ( networkId ) + "\" " + locking + " " + approve );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SetAccess", output );
        }
        
        return output;
        
    }
    
    
    public static string SetPassword ( string networkId, string password )
    {
        
        string output = "";
        
        if ( !Config.Settings.DemoMode )
        {
            output = Command.ReturnOutput ( "hamachi", "set-pass \"" + Utilities.CleanString ( networkId ) + "\" \"" + Utilities.CleanString ( password ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SetPassword", output );
        }
        
        return output;
        
    }
    
    
    public static string CreateNetwork ( string name, string password )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "create \"" + Utilities.CleanString ( name ) + "\" \"" + Utilities.CleanString ( password ) + "\"" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.CreateNetwork", output );
        
        return output;
        
    }
    
}
