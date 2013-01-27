/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2013 Stephen Brandt <stephen@stephenbrandt.com>
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
    
    public  static int MajorVersion;
    public  static string Version;
    public  static bool VpnAliasCapable = false;
    public  static bool IpModeCapable = false;
    public  static string IpVersion = "IPv4";
    public  static string lastInfo;
    private static string ScriptDirectory;
    private static Random random;
    
    
    public static void Init ()
    {
        
        GetInfo ();
        GetVersion ();
        MajorVersion = DetermineVersionAndCapabilities ();
        ScriptDirectory = DetermineScriptDirectory ();
        
    }
    
    
    public static int DetermineVersionAndCapabilities ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            VpnAliasCapable = true;
            IpModeCapable = true;
            return 2;
        }
        
        string output = Command.ReturnOutput ( "hamachi", "-h" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.DetermineVersionAndCapabilities", output );
        
        if ( ( output == "timeout" ) ||
             ( output == "error" ) ) // 'bash: hamachi: command not found' causes exception
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineVersionAndCapabilities", "Timeout or error" );
            return 0;
        }
        
        if ( output.StartsWith ( "LogMeIn Hamachi, a zero-config virtual private networking utility" ) )
        {
            if ( output.Contains ( "vpn-alias" ) ) // Since version 2.1.0.68
            {
                VpnAliasCapable = true;
                Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineVersionAndCapabilities", "VPN alias capable" );
            }
            if ( output.Contains ( "set-ip-mode" ) ) // Since version 2.1.0.17
            {
                IpModeCapable = true;
                Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineVersionAndCapabilities", "IP mode capable" );
            }
            
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineVersionAndCapabilities", "LogMeIn Hamachi detected" );
            return 2;
        }
        
        if ( ( output.StartsWith ( "Hamachi, a zero-config virtual private networking utility" ) ) ||
             ( output == "" ) )
        {
            Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineVersionAndCapabilities", "Hamachi legacy detected" );
            return 1;
        }
        
        Debug.Log ( Debug.Domain.Info, "Hamachi.DetermineVersionAndCapabilities", "Unknown" );
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
        
        string output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), Command.SudoArgs + Command.SudoStart + "bash -c \"echo \'Ipc.User      " + System.Environment.UserName + "\' >> /var/lib/logmein-hamachi/h2-engine-override.cfg; " + ScriptDirectory + "/logmein-hamachi restart\"" + Command.SudoEnd );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Configure", output );
        
        Controller.Init ();
        
    }
    
    
    public static string Start ()
    {
        
        string output = Command.ReturnOutput ( ( string ) Config.Client.Get ( Config.Settings.CommandForSuperUser ), Command.SudoArgs + Command.SudoStart + ScriptDirectory + "/logmein-hamachi start" + Command.SudoEnd );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Start", output );
        
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
            return new string [] { "25.123.456.78", "2620:9b::56d:f78e" };
        }
        
        string [] output = new string [] { "", "" };
        
        if ( Hamachi.IpModeCapable )
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
        else
        {
            try
            {
                output [0] = Retrieve ( lastInfo, "address" );
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
    
    
    public static bool GoOnline ( Network network )
    {
        
        bool success = true;
        
        if ( !Config.Settings.DemoMode )
        {
            string output = Command.ReturnOutput ( "hamachi", "go-online \"" + Utilities.CleanString ( network.Id ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOnline", output );
            
            if ( ( output.Contains ( ".. failed" ) ) &&
                 ( !output.Contains ( ".. failed, already online" ) ) )
            {
                success = false;
                
                string heading = String.Format ( TextStrings.failedGoOnlineHeading, network.Name );
                string message = TextStrings.seeOutput;
                
                Application.Invoke ( delegate
                {
                    new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
                });
            }
        }
        
        return success;
        
    }
    
    
    public static bool GoOffline ( Network network )
    {
        
        bool success = true;
        
        if ( !Config.Settings.DemoMode )
        {
            string output = Command.ReturnOutput ( "hamachi", "go-offline \"" + Utilities.CleanString ( network.Id ) + "\"" );
            Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GoOffline", output );
            
            if ( ( output.Contains ( ".. failed" ) ) &&
                 ( !output.Contains ( ".. failed, already offline" ) ) )
            {
                success = false;
                
                string heading = String.Format ( TextStrings.failedGoOfflineHeading, network.Name );
                string message = TextStrings.seeOutput;
                
                Application.Invoke ( delegate
                {
                    new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
                });
            }
        }
        
        return success;
        
    }
    
    
    public static void Delete ( Network network )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "delete \"" + Utilities.CleanString ( network.Id ) + "\"" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.Delete", output );

        if ( output.Contains ( ".. failed" ) )
        {
            string heading = String.Format ( TextStrings.failedDeleteNetworkHeading, network.Name );
            string message = TextStrings.seeOutput;
            
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
        
        if ( output.Contains ( ".. failed" ) )
        {
            string heading = String.Format ( TextStrings.failedLeaveNetworkHeading, network.Name );
            string message = TextStrings.seeOutput;
            
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

        if ( output.Contains ( ".. failed" ) )
        {
            string heading = String.Format ( TextStrings.failedEvictMemberHeading, member.Nick );
            string message = String.Format ( TextStrings.seeOutput, member.Network );
            
            Application.Invoke ( delegate
            {
                new Dialogs.Message ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Error", output );
            });
        }
        
    }
    
    
    public static string GetNick ()
    {
        
        string nick = Retrieve ( "hamachi", "", "nickname" );
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetNick", nick );
        
        return nick;
        
    }
    
    
    private static void GetVersion ()
    {
        
        if ( Config.Settings.DemoMode )
        {
            Version = "2.1.0.84";
        }
        else if ( MajorVersion == 1 )
        {
            Version = "0.9.9.9-20";
        }
        else
        {
            try
            {
                Version = Retrieve ( lastInfo, "version" );
            }
            catch {}
        }
        
        Debug.Log ( Debug.Domain.Hamachi, "Hamachi.GetVersion", Version );
        
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
        
        string address  = "25.";
               address += random.Next ( 0, 255 );
               address += ".";
               address += random.Next ( 0, 255 );
               address += ".";
               address += random.Next ( 0, 255 );
        
        return address;
        
    }
    
    
    public static string RandomClientId ()
    {
        
        string id  = "0";
               id += random.Next ( 80, 99 );
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
            
            output += " * [Artwork]\n";
            output += "       " + RandomClientId () + "   Lapo                       " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     * 090-736-821   ztefn                      " + RandomAddress () + "  alias: not set        2146:0d::987:a654    direct\n";
            output += "   [Bug Hunters]  capacity: 4/5,   [192.168.155.24/24]  subscription type: Free, owner: This computer\n";
            output += "     * " + RandomClientId () + "   Conrad                     192.168.155.20  alias: not set                             via relay\n";
            output += "     * " + RandomClientId () + "   war59312                   192.168.155.22  alias: not set                             direct\n";
            output += "     ? " + RandomClientId () + " \n";
            output += " * [" + RandomNetworkId () + "]  Development  capacity: 2/32, subscription type: Standard, owner: ztefn (090-736-821)\n";
            output += "     * 090-736-821   ztefn                      " + RandomAddress () + "  alias: not set        2146:0d::987:a654    direct\n";
            output += "   [" + RandomNetworkId () + "]Packaging  capacity: 4/256, subscription type: Premium, owner: Andrew (094-409-761)\n";
            output += "     * " + RandomClientId () + "   AndreasBWagner             " + RandomAddress () + "  alias: not set                             via relay\n";
            output += "     * 094-409-761   Andrew                     " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     * " + RandomClientId () + "   etamPL                     " + RandomAddress () + "  alias: not set                             direct\n";
            output += " * [" + RandomNetworkId () + "]Translators  capacity: 15/256, subscription type: Multi-network, owner: translators@haguichi.net\n";
            output += "     x " + RandomClientId () + "   Aytunç                     " + RandomAddress () + "\n";
            output += "     * " + RandomClientId () + "   Brbla                      " + RandomAddress () + "  alias: not set                             via relay\n";
            output += "       " + RandomClientId () + "   Daniel                     " + RandomAddress () + "\n";
            output += "     ! " + RandomClientId () + "   dimitrov                   " + RandomAddress () + "  alias: not set                             IP protocol mismatch between you and peer\n";
            output += "     * " + RandomClientId () + "   enricog                    " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     * " + RandomClientId () + "   galamarv                   " + RandomAddress () + "  alias: not set                             via relay\n";
            output += "     * " + RandomClientId () + "   HeliosReds                 " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     ! " + RandomClientId () + "   jmb_kz                     " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     * " + RandomClientId () + "   Raven46                    " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     * " + RandomClientId () + "   Rodrigo                    " + RandomAddress () + "  alias: not set                             direct\n";
            output += "     ! " + RandomClientId () + "   scrawl                     " + RandomAddress () + "  alias: 25.353.432.28  2620:9b::753:b470    direct      UDP  170.45.240.141:43667  This address is also used by another peer\n";
            output += "       " + RandomClientId () + "   Sergey                     " + RandomAddress () + "\n";
            output += "     x " + RandomClientId () + "   Soker                      " + RandomAddress () + "\n";
            output += "     * " + RandomClientId () + "   ztefn                      " + RandomAddress () + "  alias: not set        2146:0d::987:a654    direct\n";
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
        
        networkRegex          = new Regex ( "[ ]+(?<status>.{1}) " + Regex.Escape ("[") + "(?<id>.+?)" + Regex.Escape ("]") + "([ ]*)(?<name>.*?)([ ]*)(capacity: [0-9]+/(?<capacity>[0-9]+),)?([ ]*)(" + Regex.Escape ("[") + "(?<subnet>[0-9" + Regex.Escape (".") + "/]{9,19})" + Regex.Escape ("]") + ")?([ ]*)( subscription type: (?<subscription>[^,]+),)?( owner: (?<owner>.*))?$" );
        normalMemberRegex     = new Regex ( "[ ]+(?<status>.{1}) (?<id>[0-9-]{11})([ ]+)(?<name>.*?)([ ]*)(?<ipv4>[0-9" + Regex.Escape (".") + "]{7,15})?([ ]*)(alias: (?<alias>[0-9" + Regex.Escape (".") + "]{7,15}|not set))?([ ]*)(?<ipv6>[0-9a-f" + Regex.Escape (":") + "]+" + Regex.Escape (":") + "[0-9a-f" + Regex.Escape (":") + "]+)?([ ]*)(?<connection>direct|via relay|via server)?([ ]*)(?<transport>UDP|TCP)?([ ]*)(?<tunnel>[0-9" + Regex.Escape (".") + "]+" + Regex.Escape (":") + "[0-9]+)?([ ]*)(?<message>[ a-zA-Z]+)?$" );
        unapprovedMemberRegex = new Regex ( "[ ]+(?<status>.{1}) (?<id>[0-9-]{11})" );
        
        foreach ( string s in split )
        {
           
            if ( s.Length > 5 ) // Check string for minimum chars
            {
            
                if ( s.IndexOf ( "[" ) == 3 ) // Line contains network
                {
                    
                    Status status = new Status ( networkRegex.Match ( s ).Groups["status"].ToString () );
                    string id     = networkRegex.Match ( s ).Groups["id"].ToString ();
                    string name   = id;
                    string owner  = networkRegex.Match ( s ).Groups["owner"].ToString ();
                    
                    int capacity;
                    Int32.TryParse ( networkRegex.Match ( s ).Groups["capacity"].ToString (), out capacity );
                    
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

                    Network network = new Network ( status, id, name, owner, capacity );
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
                    string client     = normalMemberRegex.Match ( s ).Groups["id"].ToString ();
                    string ipv4       = normalMemberRegex.Match ( s ).Groups["ipv4"].ToString ();
                    string ipv6       = normalMemberRegex.Match ( s ).Groups["ipv6"].ToString ();
                    string nick       = normalMemberRegex.Match ( s ).Groups["name"].ToString ();
                    string alias      = normalMemberRegex.Match ( s ).Groups["alias"].ToString ();
                    string tunnel     = normalMemberRegex.Match ( s ).Groups["tunnel"].ToString ();
                    string message    = normalMemberRegex.Match ( s ).Groups["message"].ToString ();
                    
                    Status status = new Status ( normalMemberRegex.Match ( s ).Groups["status"].ToString (), connection, message );
                    
                    if ( ( nick == "" ) ||
                         ( nick == "anonymous" ) )
                    {
                        nick = TextStrings.anonymous;
                    }
                    
                    if ( alias.Contains ( "." ) )
                    {
                        ipv4 = alias;
                        ipv6 = ""; // IPv6 address doesn't work when the alias is set, therefore clearing it
                    }
                    
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
    
    
    public static string JoinNetwork ( string name, string password )
    {
        
        string output = Command.ReturnOutput ( "hamachi", "do-join \"" + Utilities.CleanString ( name ) + "\" \"" + Utilities.CleanString ( password ) + "\"" );
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
