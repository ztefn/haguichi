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

    
public class Member
{

    public Status Status;
    public string Network;
    public string Address;
    public string Nick;
    public string ClientId;
    public string Tunnel;
    
    public string NameSortString;
    public string StatusSortString;
    
    public bool IsEvicted;
    
    
    public Member ( Status status, string network, string address, string nick, string client, string tunnel )
    {
        
        this.Status   = status;
        this.Network  = network;
        this.Address  = address;
        this.Nick     = nick;
        this.ClientId = client;
        this.Tunnel   = tunnel;
        
        SetSortStrings ();
        
        this.IsEvicted = false;
        
    }
    
    
    public void Update ( Status status, string network, string address, string nick, string client, string tunnel )
    {
        
        this.Status   = status;
        this.Network  = network;
        this.Address  = address;
        this.ClientId = client;
        this.Tunnel   = tunnel;
        
        GetLongNick ( nick );
        SetSortStrings ();
        
    }
    
    
    private void SetSortStrings ()
    {
        
        NameSortString = this.Nick + this.ClientId;
        StatusSortString = this.Status.statusSortable + this.Nick + this.ClientId;
        
    }
    
    
    public void GetLongNick ( string nick )
    {
        
        if ( ( Hamachi.ApiVersion > 1 ) &&
             ( ( this.Nick.Length >= 25 ) ||
               ( this.Nick.EndsWith ( "�" ) ) ) )
        {
            Thread thread = new Thread ( GetLongNickThread );
            thread.Start ();
        }
        else
        {
            this.Nick = nick;
        }
        
    }
    
    
    private void GetLongNickThread ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "peer " + this.ClientId );
        Debug.Log ( Debug.Domain.Hamachi, "Network.GetLongNickThread", output );
        
        this.Nick = Hamachi.Retrieve ( output, "nickname" );
        
    }
    
    
    public void CopyAddressToClipboard ( object o, EventArgs args )
    {
        
        Clipboard clip = Clipboard.Get ( Gdk.Atom.Intern( "CLIPBOARD", true ) );
        clip.Text = this.Address;
        
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
            this.Address = "5.092.112.049";
            this.Status  = new Status ( "*" );
        
            SetSortStrings ();
            
            MainWindow.networkView.UpdateMember ( MainWindow.networkView.ReturnNetworkById ( this.Network ), this );
        }
        else
        {
            Hamachi.Approve ( this );
            Controller.UpdateConnection (); // Update list
        }
        
    }
    
    
    public void Reject ( object o, EventArgs args )
    {
        
        if ( Config.Settings.DemoMode )
        {
            MainWindow.networkView.RemoveMember ( MainWindow.networkView.ReturnNetworkById ( this.Network ), this );
        }
        else
        {
            Hamachi.Reject ( this );
            Controller.UpdateConnection (); // Update list
        }
        
    }
    
    
    public void Evict ( object o, EventArgs args )
    {
        
        Network network = MainWindow.networkView.ReturnNetworkById ( this.Network );
        
        string label   = TextStrings.evictLabel;
        string heading = String.Format ( TextStrings.confirmEvictMemberHeading, this.Nick );
        string message = String.Format ( TextStrings.confirmEvictMemberMessage, this.Nick, network.Name );
            
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
                
                Hamachi.Evict ( this );
                Controller.UpdateConnection (); // Update list
            }
        }
        
    }
    
}
