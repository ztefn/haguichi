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
    
    private BackgroundWorker worker;
    
    
    public Member ( Status status, string network, string address, string nick, string client, string tunnel )
    {
        
        this.Status   = status;
        this.Network  = network;
        this.Address  = address;
        this.Nick     = nick;
        this.ClientId = client;
        this.Tunnel   = tunnel;
        
        SetSortStrings ();
        
    }
    
    
    public void Update ( Status status, string network, string address, string nick, string client, string tunnel )
    {
    
        this.Status   = status;
        this.Network  = network;
        this.Address  = address;
        this.Nick     = nick;
        this.ClientId = client;
        this.Tunnel   = tunnel;
        
        SetSortStrings ();
    
    }
    
    
    private void SetSortStrings ()
    {
    
        NameSortString = this.Nick + this.ClientId;
        StatusSortString = this.Status.statusSortable + this.Nick + this.ClientId;
    
    }
    
    
    public void GetLongNick ()
    {
        
        worker = new BackgroundWorker {};

        worker.DoWork += GetLongNickThread;
        worker.RunWorkerAsync ();
        
    }
    
    
    private void GetLongNickThread ( object sender, DoWorkEventArgs e )
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
        
        Hamachi.Approve ( this );
        Controller.UpdateConnection (); // Update list
    
    }
    
    
    public void Reject ( object o, EventArgs args )
    {
        
        Hamachi.Reject ( this );
        Controller.UpdateConnection (); // Update list
    
    }
    
    
    public void Evict ( object o, EventArgs args )
    {
        
        string label   = TextStrings.evictLabel;
        string heading = String.Format ( TextStrings.confirmEvictMemberHeading, this.Nick );
        string message = String.Format ( TextStrings.confirmEvictMemberMessage, this.Nick, this.Network );
            
        Dialogs.Confirm dlg = new Dialogs.Confirm ( heading, message, "Warning", label, null );
        
        if ( dlg.response == "Ok" )
        {
            Hamachi.Evict ( this );
            Controller.UpdateConnection (); // Update list
        }
    
    }
    
}
