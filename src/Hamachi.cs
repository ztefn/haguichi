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

using Gtk;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


public static class Hamachi
{
    
    public  static int ApiVersion;
    public  static string IpVersion = "IPv4";
    public  static string lastInfo;
    private static string ScriptDirectory;
    private static Random random;
    
    
    public static void Init ()
    {
        
        GetInfo ();
        ApiVersion = DetermineApiVersion ();
        ScriptDirectory = DetermineScriptDirectory ();
        
    }
    
    
    public static int DetermineApiVersion ()
    {
        
        /* 
         *  <= 0.9.9.9-20 : 1
         *  >= 2.0.0.11   : 2
         *  >= 2.1.0.17   : 3
         *  >= 2.1.0.68   : 4
         * 
         */
        
        if ( Config.Settings.DemoMode )
        {
            return 3;
        }
        
        string output = Command.ReturnOutput ( "hamachi", "-h" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.DetermineApiVersion", output );
        
        if ( ( output == "timeout" ) ||
             ( output == "error" ) ) // 'bash: hamachi: command not found' causes exception
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "0" );
            return 0;
        }
        
        if ( output.StartsWith ( "LogMeIn Hamachi, a zero-config virtual private networking utility" ) )
        {
            if ( output.Contains ( "vpn-alias" ) )
            {
                Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "4" );
                return 4;
            }
            else if ( output.Contains ( "set-ip-mode" ) )
            {
                Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "3" );
                return 3;
            }
            else
            {
                Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "2" );
                return 2;
            }
        }
        
        if ( ( output.StartsWith ( "Hamachi, a zero-config virtual private networking utility" ) ) ||
             ( output == "" ) )
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "1" );
            return 1;
        }
        
        Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineApiVersion", "-1" );
        return -1;
        
    }
    
    
    private static string DetermineScriptDirectory ()
    {
        
        ScriptDirectory = "/etc/init.d"; // Standard for most distro's
        
        if ( Directory.Exists ( "/etc/init.d" ) )
        {
            // Ok, keep it
        }
        else if ( Directory.Exists ( "/etc/rc.d/init.d" ) )
        {
            ScriptDirectory = "/etc/rc.d/init.d"; // Red Hat based distro's
        }
        else if ( Directory.Exists ( "/etc/rc.d" ) )
        {
            ScriptDirectory = "/etc/rc.d"; // Arch, Slackware
        }
        
        Debug.Log ( Debug.Domain.Environment, "Hamachi.DetermineScriptDirectory", ScriptDirectory );
        return ScriptDirectory;
        
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
            output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), Command.SudoArguments + Command.SudoCommandStart + "bash -c \"echo \'Ipc.User      " + System.Environment.UserName + "\' >> /var/lib/logmein-hamachi/h2-engine-override.cfg; " + ScriptDirectory + "/logmein-hamachi restart --skip-redirect\"" + Command.SudoCommandEnd );
            
            if ( output.Contains ( "Restarting LogMeIn Hamachi VPN tunneling engine logmein-hamachi" ) )
            {
                Controller.Init ();
            }
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            output = Command.ReturnOutput ( "hamachi-init", "" );
            
            if ( output.Contains ( "Authentication information has been created." ) )
            {
                Config.Client.Set ( Config.Settings.HamachiDataPath, Config.Settings.DefaultHamachiDataPath );
                Controller.Init ();
            }
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Configure", output );
        
    }
    
    
    public static void TunCfg ()
    {
        
        string output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), Command.SudoArguments + ( string ) Config.Client.Get ( Config.Settings.CommandForTunCfg ) );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.TunCfg", output );
        
    }
    
    
    public static string Start ()
    {
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), Command.SudoArguments + Command.SudoCommandStart + ScriptDirectory + "/logmein-hamachi start --skip-redirect" + Command.SudoCommandEnd );
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            output = Command.ReturnOutput ( "hamachi", "start" );
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Start", output );
        
        return output;
        
    }
    
    
    public static string Stop ()
    {
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), Command.SudoArguments + Command.SudoCommandStart + ScriptDirectory + "/logmein-hamachi stop --skip-redirect" + Command.SudoCommandEnd );
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
        
        string output = "";
        
        try
        {
            output = Retrieve ( lastInfo, "lmi account" );
        }
        catch {}
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetAccount", output );
        
        return output;
        
    }
    
    
    public static string GetClientId ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            return "090-123-456";
        }
        
        string output = "";
        
        try
        {
            output = Retrieve ( lastInfo, "client id" );
        }
        catch {}
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetClientId", output );
        
        return output;
        
    }
    
    
    public static string [] GetAddress ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            IpVersion = "Both";
            return new string [] { "5.123.456.789", "2620:9b::56d:f78e" };
        }
        
        string [] output = new string [] { "", "" };
        
        if ( Hamachi.ApiVersion >= 3 )
        {
            try
            {
                string rawOuput = Retrieve ( lastInfo, "address" );
                
                Regex regex = new Regex ( "(?<ipv4>[0-9" + Regex.Escape (".") + "]{7,15})?([ ]*)(?<ipv6>[0-9a-z" + Regex.Escape (":") + "]+)?$" );
                string IPv4 = regex.Match ( rawOuput ).Groups["ipv4"].ToString ();
                string IPv6 = regex.Match ( rawOuput ).Groups["ipv6"].ToString ();
                
                if ( ( IPv4 != "" ) &&
                     ( IPv6 != "" ) )
                {
                    IpVersion = "Both";
                }
                else if ( IPv4 != "" )
                {
                    IpVersion = "IPv4";
                }
                else if ( IPv6 != "" )
                {
                    IpVersion = "IPv6";
                }
                
                output [0] = IPv4;
                output [1] = IPv6;
            }
            catch {}
        }
        else if ( Hamachi.ApiVersion == 2 )
        {
            try
            {
                output [0] = Retrieve ( lastInfo, "address" );
            }
            catch {}
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            string filePath = ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) + "/state";
            
            try
            {
                output [0] = Retrieve ( "cat", filePath, "Identity", 11 );
            }
            catch {}
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetAddress", "IPv4: " + output [0] );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetAddress", "IPv6: " + output [1] );
        
        return output;
        
    }
    
    
    public static string GetInfo ()
    {
        
        if ( !Config.Settings.DemoMode )
        {
            lastInfo = Command.ReturnOutput ( "hamachi", "" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetInfo", lastInfo );
        }
        
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

        if ( output.Contains ( ".. failed, you are not an owner" ) )
        {
            string heading = String.Format ( TextStrings.failedDeleteNetworkHeading, network.Name );
            string message = TextStrings.failedDeleteNetworkMessage;
            
            Application.Invoke ( delegate
            {
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
            });
        }
        
    }
    
    
    public static void Leave ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "leave \"" + Utilities.CleanString ( network.Id ) + "\"" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Leave", output );

        if ( output.Contains ( ".. failed, you are an owner" ) )
        {
            string heading = String.Format ( TextStrings.failedLeaveNetworkHeading, network.Name );
            string message = TextStrings.failedLeaveNetworkMessageIsOwner;
            
            Application.Invoke ( delegate
            {
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
            });
        }
        else if ( output.Contains ( ".. failed, denied" ) )
        {
            string heading = String.Format ( TextStrings.failedLeaveNetworkHeading, network.Name );
            string message = TextStrings.failedLeaveNetworkMessageDenied;
            
            Application.Invoke ( delegate
            {
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
            });
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

        if ( output.Contains ( ".. failed, denied" ) )
        {
            string heading = String.Format ( TextStrings.failedEvictMemberHeading, member.Nick );
            string message = String.Format ( TextStrings.failedEvictMemberMessage, member.Network );
            
            Application.Invoke ( delegate
            {
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
            });
        }
        
    }
    
    
    public static string GetNick ()
    {
        
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
            return "2.1.0.17";
        }
        
        string output = "";
        
        try
        {
            output = Retrieve ( lastInfo, "version" );
            output = output.Replace ( "hamachi-lnx-", "" );
        }
        catch {}
        
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
            output += "     * " + RandomClientId () + "   scrawl                     " + RandomAddress () + "  alias: 5.255.255.255  2620:9b::753:b470    direct      UDP  170.10.240.140:12345\n";
            output += "     * " + RandomClientId () + "   Sergey                     " + RandomAddress () + "  alias: not set                             via relay\n";
            output += "     ? " + RandomClientId () + " \n";
            output += " * [" + RandomNetworkId () + "]Portal Ubuntu  capacity: 2/5, subscription type: Free, owner: Soker (092-466-858)\n";
            output += "     x 092-466-858   Soker                      " + RandomAddress () + " 2146:0d::987:a654\n";
            output += "   [" + RandomNetworkId () + "]WebUpd8  capacity: 2/20, subscription type: Multi-network, owner: account@logmein.com\n";
            output += "       094-409-761   Andrew                     " + RandomAddress () + "\n";
            output += "     * " + RandomClientId () + "   MastroPino                 " + RandomAddress () + "\n";
        }
        else
        {
            output = GetList ();
        }
        
        string [] split = output.Split ( Environment.NewLine.ToCharArray () );
        string curNetworkId = "";
        
        Regex networkRegex;
        Regex normalMemberRegex;
        Regex unapprovedMemberRegex;
        
        if ( Hamachi.ApiVersion > 1 )
        {
            networkRegex          = new Regex ( "[ ]+(?<status>.{1}) " + Regex.Escape ("[") + "(?<id>.+)" + Regex.Escape ("]") + "([ ]*)(?<name>.*?)([ ]*)(capacity: [0-9]+/(?<capacity>[0-9]+), subscription type: (?<subscription>[^,]+), owner: (?<owner>.*))?$" );
            normalMemberRegex     = new Regex ( "[ ]+(?<status>.{1}) (?<id>[0-9-]{11})([ ]+)(?<name>.*?)([ ]*)(?<ipv4>[0-9" + Regex.Escape (".") + "]{7,15})?([ ]*)(?<alias>alias: ([0-9" + Regex.Escape (".") + "]{7,15}|not set))?([ ]*)(?<ipv6>[0-9a-f" + Regex.Escape (":") + "]+" + Regex.Escape (":") + "[0-9a-f" + Regex.Escape (":") + "]+)?([ ]*)(?<connection>direct|via relay|via server)?([ a-zA-Z]+)?(?<tunnel>[0-9" + Regex.Escape (".") + "]+" + Regex.Escape (":") + "[0-9]+)?$" );
            unapprovedMemberRegex = new Regex ( "[ ]+(?<status>.{1}) (?<id>[0-9-]{11})" );
        }
        else
        {
            networkRegex          = new Regex ( "[ ]+(?<status>.{1}) " + Regex.Escape ("[") + "(?<id>.+)" + Regex.Escape ("]") );
            normalMemberRegex     = new Regex ( "[ ]+(?<status>.{1}) (?<ipv4>[0-9" + Regex.Escape (".") + "]{7,15})([ ]+)(?<name>.*?)([ ]*)(?<tunnel>[0-9" + Regex.Escape (".:") + "]+)?$" );
            unapprovedMemberRegex = new Regex ( "" );
        }
        
        foreach ( string s in split )
        {
           
            if ( s.Length > 5 ) // Check string for minimum chars
            {
            
                if ( s.IndexOf ( "[" ) == 3 ) // Line contains network
                {
                    
                    Status status = new Status ( networkRegex.Match ( s ).Groups["status"].ToString () );
                    string id     = networkRegex.Match ( s ).Groups["id"].ToString ();
                    string name   = id;
                    
                    try
                    {
                        name = networkRegex.Match ( s ).Groups["name"].ToString ().TrimEnd ();
                        
                        if ( name.Length == 0 )
                        {
                            name = id;
                        }
                    }
                    catch
                    {
                        // No name
                    }

                    Network network = new Network ( status, id, name );
                    networks.Add ( network );
                    
                    curNetworkId = id;
                    
                }
                else if ( s.IndexOf ( "?" ) == 5 ) // Line contains unapproved member
                {
                    
                    Status status = new Status ( unapprovedMemberRegex.Match ( s ).Groups["status"].ToString () );
                    string client = unapprovedMemberRegex.Match ( s ).Groups["id"].ToString ();
                    string nick   = TextStrings.unknown;
                    
                    Member member = new Member ( status, curNetworkId, "", "", nick, client, "" );
                    
                    foreach ( Network network in networks )
                    {
                        if ( network.Id == curNetworkId )
                        {
                            network.AddMember ( member );
                        }
                    }
                    
                }
                else if ( normalMemberRegex.IsMatch ( s ) ) // Line contains normal member
                {
                    
                    string connection = normalMemberRegex.Match ( s ).Groups["connection"].ToString ();
                    Status status = new Status ( normalMemberRegex.Match ( s ).Groups["status"].ToString (), connection );
                    string client;
                    
                    if ( Hamachi.ApiVersion > 1 )
                    {
                        client = normalMemberRegex.Match ( s ).Groups["id"].ToString ();
                    }
                    else
                    {
                        client = normalMemberRegex.Match ( s ).Groups["ipv4"].ToString ();
                    }
                    
                    string ipv4 = normalMemberRegex.Match ( s ).Groups["ipv4"].ToString ();
                    string ipv6 = normalMemberRegex.Match ( s ).Groups["ipv6"].ToString ();
                    string nick = normalMemberRegex.Match ( s ).Groups["name"].ToString ();
                    
                    if ( ( nick == "" ) ||
                         ( nick == "anonymous" ) )
                    {
                        nick = TextStrings.anonymous;
                    }
                    
                    string tunnel = normalMemberRegex.Match ( s ).Groups["tunnel"].ToString ();

                    Member member = new Member ( status, curNetworkId, ipv4, ipv6, nick, client, tunnel );
                    
                    foreach ( Network network in networks )
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
    
    
    public static string SetNick ( string nick )
    {
        
        string output = "";
        
        if ( ( !Config.Settings.DemoMode ) &&
             ( Controller.lastStatus >= 6 ) )
        {
            output = Command.ReturnOutput ( "hamachi", "set-nick \"" + Utilities.CleanString ( nick ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SetNick", output );
        }
        
        return output;
        
    }
    
    
    public static string SetProtocol ( string protocol )
    {
        
        string output = "";
        
        if ( !Config.Settings.DemoMode )
        {
            output = Command.ReturnOutput ( "hamachi", "set-ip-mode \"" + protocol + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.SetProtocol", output );
        }
        
        return output;
        
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
