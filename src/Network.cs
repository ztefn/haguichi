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
using System.Threading;
using Gtk;


public class Network
{

    public Status Status;
    public string Id;
    public string Name;
    public ArrayList Members;
    public int IsOwner;
    public string OwnerId;
    public string Lock;
    public string Approve;
    
    public string NameSortString;
    public string StatusSortString;
    
    public DateTime lastUpdate;
    
    
    public Network ()
    {
        // Just for creating unassigned instances
    }
    
    
    public Network ( Status status, string id, string name )
    {
        
        this.Status  = status;
        this.Id      = id;
        this.Name    = name;
        this.Members = new ArrayList();
        this.IsOwner = -1;
        this.OwnerId = "";
        this.Lock    = "";
        this.Approve = "";
        
        this.lastUpdate = DateTime.Now;
        SetSortStrings ();
        
    }
    
    
    public Network ( Status status, string id, string name, ArrayList members )
    {
        
        this.Status  = status;
        this.Id      = id;
        this.Name    = name;
        this.Members = members;
        this.IsOwner = -1;
        this.OwnerId = "";
        
        this.lastUpdate = DateTime.Now;
        SetSortStrings ();
        
    }
    
    
    private void SetSortStrings ()
    {
        
        NameSortString   = this.Name;
        StatusSortString = this.Status.statusSortable + this.Name;
        
    }
    
    
    public void Update ( Status status, string id, string name )
    {
        
        /* 
         * Make sure the last update wasn't in the GetListWaitTime timespan to prevent the background
         * update process from overriding a more recent update triggered by some other event
         */
        int seconds = ( int ) ( ( double ) Config.Client.Get ( Config.Settings.GetListWaitTime ) );
        DateTime offset = DateTime.Now.Subtract ( new System.TimeSpan ( 0, 0, seconds ) );
        
        if ( offset > this.lastUpdate)
        {
            this.Status = status;
            this.Id     = id;
            this.Name   = name;
            
            this.lastUpdate = DateTime.Now;
            SetSortStrings ();
        }
        
    }
    
    
    public void ReturnMemberCount ( out int totalCount, out int onlineCount )
    {
        
        totalCount  = 0;
        onlineCount = 0;

        foreach ( Member member in Members )
        {
            totalCount ++;
                
            if ( ( member.Status.statusInt > 0 ) &&
                 ( member.Status.statusInt < 3 ) )
            {
                onlineCount ++;
            }
        }
        
    }
    
    
    public string ReturnOwnerNick ()
    {

        string nick = "";
        
        foreach ( Member member in Members )
        {
            if ( member.ClientId == this.OwnerId )
            {
                nick = member.Nick;
            }
        }
        
        return nick;
        
    }
    
    
    public void DetermineOwnership ()
    {
        
        Thread thread = new Thread ( DetermineOwnershipThread );
        thread.Start ();
        
    }
    
    
    private void DetermineOwnershipThread ()
    {
        
        string output = "";
        
        if ( Hamachi.ApiVersion > 1 )
        {
            try
            {
                if ( Config.Settings.DemoMode )
                {
                    output = "hamachi network " + this.Id + @"
    id       : " + this.Id + @"
    name     : " + this.Name;
                    
                    if ( this.Name == "Portal Ubuntu" )
                    {
                        output += @"
    type     : Mesh
    owner    : 092-466-858";
                    }
                    else if ( this.Name == "WebUpd8" )
                    {
                        output += @"
    type     : Mesh
    owner    : 094-409-761";
                    }
                    else
                    {
                        output += @"
    type     : Mesh
    owner    : This computer
    status   : unlocked
    approve  : manual";
                    }
                }
                else
                {
                    output = Command.ReturnOutput ( "hamachi", "network \"" + Utilities.CleanString ( this.Id ) + "\"" );
                }
                Debug.Log ( Debug.Domain.Hamachi, "Network.DetermineOwnership", output );
                
                this.OwnerId = Hamachi.Retrieve ( output, "owner" );
                this.Lock = Hamachi.Retrieve ( output, "status" );
                this.Approve = Hamachi.Retrieve ( output, "approve" );
                
                if ( this.OwnerId == "This computer" )
                {
                    this.IsOwner = 1;
                }
                else
                {
                    this.IsOwner = 0;
                }
            }
            catch {}
        }
        else if ( Hamachi.ApiVersion == 1 )
        {
            output = Command.ReturnOutput ( "hamachi", "evict \"" + Utilities.CleanString ( this.Id ) + "\" 5.5.5.5" );
            Debug.Log ( Debug.Domain.Hamachi, "Network.DetermineOwnership", output );
    
            if ( ( output.IndexOf ( ".. ok" ) != -1 ) ||
                 ( output.IndexOf ( ".. failed, [16]" ) != -1 ) )
            {
                this.IsOwner = 1;
            }
            else
            {
                this.IsOwner = 0;
            }
        }
        
    }
    
    
    public void AddMember ( Member member )
    {
        
        Members.Add ( member );
        
    }
    
    
    public void RemoveMember ( Member member )
    {
        
        Members.Remove ( member );
        
    }
    
    
    public void GoOnline ( object o, EventArgs args )
    {
        
        Thread thread = new Thread ( GoOnlineThread );
        thread.Start ();
        
        this.Status = new Status ( "*" );
        
        this.lastUpdate = DateTime.Now;
        SetSortStrings ();
        
        MainWindow.networkView.UpdateNetwork ( this );
        
    }
    
    
    private void GoOnlineThread ()
    {
        
        Hamachi.GoOnline ( this );
        
    }
    
    
    public void GoOffline ( object o, EventArgs args )
    {
        
        Thread thread = new Thread ( GoOfflineThread );
        thread.Start ();
        
        this.Status = new Status ( " " );
        
        this.lastUpdate = DateTime.Now;
        SetSortStrings ();
        
        MainWindow.networkView.UpdateNetwork ( this );
        
    }
    
    
    private void GoOfflineThread ()
    {
        
        Hamachi.GoOffline ( this );
        
    }
    
    
    public void ChangePassword ( object o, EventArgs args )
    {
        
        Dialogs.ChangePassword passwordWindow = new Dialogs.ChangePassword ( this );
        
    }
    
    
    public void SetLock ( string locked )
    {
        
        this.Lock = locked;
        
        Thread thread = new Thread ( SetLockThread );
        thread.Start ();
        
    }
    
    
    private void SetLockThread ()
    {
        
        string locked = "unlock";
        
        if ( this.Lock == "locked" )
        {
            locked = "lock";
        }
        
        Hamachi.SetAccess ( this.Id, locked, this.Approve );
        
    }
    
    
    public void SetApproval ( string approval )
    {
        
        this.Approve = approval;
        
        Thread thread = new Thread ( SetApprovalThread );
        thread.Start ();
        
    }
    
    
    private void SetApprovalThread ()
    {
        
        string locked = "unlock";
        
        if ( this.Lock == "locked" )
        {
            locked = "lock";
        }
        
        Hamachi.SetAccess ( this.Id, locked, this.Approve );
        
    }
    
    
    public void CopyIdToClipboard ( object o, EventArgs args )
    {
    
        Clipboard clip = Clipboard.Get ( Gdk.Atom.Intern( "CLIPBOARD", true ) );
        clip.Text = this.Id;
    
    }
    
    
    public void Leave ( object o, EventArgs args )
    {
        
        string label   = TextStrings.leaveLabel;
        string heading = String.Format ( TextStrings.confirmLeaveNetworkHeading, this.Name );
        string message = String.Format ( TextStrings.confirmLeaveNetworkMessage, this.Name );
        
        Dialogs.Confirm dlg = new Dialogs.Confirm ( heading, message, "Warning", label, null );
        
        if ( dlg.response == "Ok" )
        {
            if ( Config.Settings.DemoMode )
            {
                MainWindow.networkView.RemoveNetwork ( this );
            }
            else
            {
                Hamachi.Leave ( this );
                Controller.UpdateConnection (); // Update list
            }
        }
        
    }
    
    
    public void Delete ( object o, EventArgs args )
    {
        
        string label   = Stock.Delete;
        string heading = String.Format ( TextStrings.confirmDeleteNetworkHeading, this.Name );
        string message = String.Format ( TextStrings.confirmDeleteNetworkMessage, this.Name );
        
        Dialogs.Confirm dlg = new Dialogs.Confirm ( heading, message, "Warning", label, Stock.Delete );
        
        if ( dlg.response == "Ok" )
        {
            if ( Config.Settings.DemoMode )
            {
                MainWindow.networkView.RemoveNetwork ( this );
            }
            else
            {
                Hamachi.Delete ( this );
                Controller.UpdateConnection (); // Update list
            }
        }
        
    }

}
