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

using System;
using System.Threading;
using System.Text.RegularExpressions;
using Gtk;

    
public class Member
{

    public Status Status;
    public string Network;
    public string IPv4;
    public string IPv6;
    public string Nick;
    public string ClientId;
    public string Tunnel;
    
    public string NameSortString;
    public string StatusSortString;
    
    public bool IsEvicted;
    
    private bool HasCompleteAddresses;
    
    
    public Member ( Status status, string network, string ipv4, string ipv6, string nick, string client, string tunnel )
    {
        
        this.Status   = status;
        this.Network  = network;
        this.IPv4     = ipv4;
        this.IPv6     = ipv6;
        this.Nick     = nick;
        this.ClientId = client;
        this.Tunnel   = tunnel;
        
        SetSortStrings ();
        
        this.IsEvicted = false;
        
    }
    
    
    public void Init ()
    {
        
        GetLongNick ( this.Nick );
        GetCompleteAddresses ( this.IPv4, this.IPv6 );
        
    }
    
    
    public void Update ( Status status, string network, string ipv4, string ipv6, string nick, string client, string tunnel )
    {
        
        this.Status   = status;
        this.Network  = network;
        this.ClientId = client;
        this.Tunnel   = tunnel;
        
        GetLongNick ( nick );
        GetCompleteAddresses ( ipv4, ipv6 );
        
    }
    
    
    private void SetSortStrings ()
    {
        
        this.NameSortString = this.Nick + this.ClientId;
        this.StatusSortString = this.Status.statusSortable + this.Nick + this.ClientId;
        
    }
    
    
    public void GetLongNick ( string nick )
    {
        
        if ( ( nick.Length >= 25 ) ||
             ( nick.EndsWith ( "�" ) ) )
        {
            Thread thread = new Thread ( GetLongNickThread );
            thread.Start ();
        }
        else
        {
            this.Nick = nick;
            SetSortStrings ();
        }
        
    }
    
    
    private void GetLongNickThread ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "peer " + this.ClientId );
        Debug.Log ( Debug.Domain.Hamachi, "Member.GetLongNickThread", output );
        
        this.Nick = Hamachi.Retrieve ( output, "nickname" );
        SetSortStrings ();
        
        Application.Invoke ( delegate
        {
            MainWindow.networkView.UpdateMember ( this.Network, this );
        });
        
    }
    
    
    public void GetCompleteAddresses ( string ipv4, string ipv6 )
    {
        
        if ( ( Hamachi.Version == "2.0.0.11" ) ||
             ( Hamachi.Version == "2.0.0.12" ) ||
             ( Hamachi.Version == "2.0.1.13" ) ||
             ( Hamachi.Version == "2.0.1.15" ) ||
             ( Hamachi.Version == "2.1.0.17" ) ||
             ( Hamachi.Version == "2.1.0.18" ) ||
             ( Hamachi.Version == "2.1.0.68" ) ||
             ( Hamachi.Version == "2.1.0.76" ) ||
             ( Hamachi.Version == "2.1.0.80" ) ||
             ( Hamachi.Version == "2.1.0.81" ) )
        {
            if ( ( !this.IPv4.StartsWith ( ipv4 ) ) ||
                 ( !this.IPv6.StartsWith ( ipv6 ) ) )
            {
                this.HasCompleteAddresses = false;
            }
            
            if ( this.HasCompleteAddresses == true )
            {
                if ( ipv4 == "" )
                {
                    this.IPv4 = ipv4;
                }
                if ( ipv6 == "" )
                {
                    this.IPv6 = ipv6;
                }
            }
            else if ( this.Status.statusInt == 1 )
            {
                Thread thread = new Thread ( GetCompleteAddressesThread );
                thread.Start ();
            }
            else
            {
                this.IPv4 = ipv4;
                this.IPv6 = ipv6;
            }
        }
        else
        {
            this.IPv4 = ipv4;
            this.IPv6 = ipv6;
        }
        
    }
    
    
    private void GetCompleteAddressesThread ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "peer " + this.ClientId );
        Debug.Log ( Debug.Domain.Hamachi, "Member.GetCompleteAddressesThread", output );
        
        string alias     = Hamachi.Retrieve ( output, "VPN alias"   );
        string addresses = Hamachi.Retrieve ( output, "VIP address" );
        
        Regex regex = new Regex ( "(?<ipv4>[0-9" + Regex.Escape (".") + "]{7,15})?([ ]*)(?<ipv6>[0-9a-z" + Regex.Escape (":") + "]+)?$" );
        string ipv4 = regex.Match ( addresses ).Groups["ipv4"].ToString ();
        string ipv6 = regex.Match ( addresses ).Groups["ipv6"].ToString ();
        
        if ( alias.Contains ( "." ) )
        {
            this.IPv4 = alias;
            this.IPv6 = ""; // IPv6 address doesn't work when the alias is set, therefore clearing it
        }
        else
        {
            this.IPv4 = ipv4;
            this.IPv6 = ipv6;
        }
        
        this.HasCompleteAddresses = true;
        
        Application.Invoke ( delegate
        {
            MainWindow.networkView.UpdateMember ( this.Network, this );
        });
        
    }
    
    
    public void CopyIPv4ToClipboard ( object o, EventArgs args )
    {
        
        Clipboard clip = Clipboard.Get ( Gdk.Atom.Intern( "CLIPBOARD", true ) );
        clip.Text = this.IPv4;
        
    }
    
    
    public void CopyIPv6ToClipboard ( object o, EventArgs args )
    {
        
        Clipboard clip = Clipboard.Get ( Gdk.Atom.Intern( "CLIPBOARD", true ) );
        clip.Text = this.IPv6;
        
    }
    
    
    public void CopyClientIdToClipboard ( object o, EventArgs args )
    {
        
        Clipboard clip = Clipboard.Get ( Gdk.Atom.Intern( "CLIPBOARD", true ) );
        clip.Text = this.ClientId;
        
    }
    
    
    public void Approve ( object o, EventArgs args )
    {
        
        if ( Config.Settings.DemoMode )
        {
            this.Nick    = "Nick";
            this.IPv4    = "5.092.112.049";
            this.Status  = new Status ( "*" );
        
            SetSortStrings ();
            
            MainWindow.networkView.UpdateMember ( MainWindow.networkView.ReturnNetworkById ( this.Network ), this );
        }
        else
        {
            Thread thread = new Thread ( ApproveThread );
            thread.Start ();
        }
        
    }
    
    
    private void ApproveThread ()
    {
        
        Hamachi.Approve ( this );
        
        Application.Invoke ( delegate
        {
            Controller.UpdateConnection (); // Update list
        });
        
    }
    
    
    public void Reject ( object o, EventArgs args )
    {
        
        if ( Config.Settings.DemoMode )
        {
            MainWindow.networkView.RemoveMember ( MainWindow.networkView.ReturnNetworkById ( this.Network ), this );
        }
        else
        {
            Thread thread = new Thread ( RejectThread );
            thread.Start ();
        }
        
    }
    
    
    private void RejectThread ()
    {
        
        Hamachi.Reject ( this );
        
        Application.Invoke ( delegate
        {
            Controller.UpdateConnection (); // Update list
        });
        
    }
    
    
    public void Evict ( object o, EventArgs args )
    {
        
        Network network = MainWindow.networkView.ReturnNetworkById ( this.Network );
        
        string label   = TextStrings.evictLabel;
        string heading = String.Format ( TextStrings.confirmEvictMemberHeading, this.Nick, network.Name  );
        string message = String.Format ( TextStrings.confirmEvictMemberMessage );
            
        Dialogs.Confirm dlg = new Dialogs.Confirm ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Warning", label, null );
        
        if ( dlg.response == "Ok" )
        {
            if ( Config.Settings.DemoMode )
            {
                MainWindow.networkView.RemoveMember ( network, this );
            }
            else
            {
                this.IsEvicted = true;
                
                Thread thread = new Thread ( EvictThread );
                thread.Start ();
            }
        }
        
    }
    
    
    private void EvictThread ()
    {
        
        Hamachi.Evict ( this );
        
        Application.Invoke ( delegate
        {
            Controller.UpdateConnection (); // Update list
        });
        
    }
    
}
