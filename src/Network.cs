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
    public string Owner;
    public string Lock;
    public string Approve;
    public int Capacity;
    
    public string NameSortString;
    public string StatusSortString;
    
    private bool updating;
    
    
    public Network ()
    {
        
        // Just for creating unassigned instances
        
    }
    
    
    public Network ( Status status, string id, string name, string owner, int capacity )
    {
        
        this.Status   = status;
        this.Id       = id;
        this.Name     = name;
        this.Members  = new ArrayList();
        this.IsOwner  = -1;
        this.Owner    = owner;
        this.Lock     = "";
        this.Approve  = "";
        this.Capacity = capacity;
        
        SetSortStrings ();
        
    }
    
    
    public Network ( Status status, string id, string name, ArrayList members )
    {
        
        this.Status  = status;
        this.Id      = id;
        this.Name    = name;
        this.Members = members;
        this.IsOwner = -1;
        this.Owner   = "";
        
        SetSortStrings ();
        
    }
    
    
    private void SetSortStrings ()
    {
        
        NameSortString   = this.Name;
        StatusSortString = this.Status.statusSortable + this.Name;
        
    }
    
    
    public void Update ( Status status, string id, string name )
    {
        
        if ( !updating ) // Check this flag to prevent the background update process from overriding a very recent change
        {
            this.Status = status;
            this.Id     = id;
            this.Name   = name;
            
            SetSortStrings ();
        }
        
    }
    
    
    public void ReturnMemberCount ( out int totalCount, out int onlineCount )
    {
        
        totalCount  = 0;
        onlineCount = 0;

        foreach ( Member member in this.Members )
        {
            totalCount ++;
                
            if ( ( member.Status.statusInt > 0 ) &&
                 ( member.Status.statusInt < 3 ) )
            {
                onlineCount ++;
            }
        }
        
    }
    
    
    public string ReturnOwnerString ()
    {
        
        string owner;
        
        if ( this.IsOwner == 1 )
        {
            owner = TextStrings.you;
        }
        else if ( this.Owner != "" )
        {
            owner = this.Owner;
            
            foreach ( Member member in this.Members )
            {
                if ( member.ClientId == owner )
                {
                    owner = member.Nick;
                }
            }
        }
        else
        {
            owner = TextStrings.unknown;
        }
        
        return owner;
        
    }
    
    
    public void DetermineOwnership ()
    {
        
        Thread thread = new Thread ( DetermineOwnershipThread );
        thread.Start ();
        
    }
    
    
    private void DetermineOwnershipThread ()
    {
        
        string output = "";
        
        try
        {
            if ( Config.Settings.DemoMode )
            {
                output = @"
id       : " + this.Id + @"
name     : " + this.Name + @"
type     : Mesh";
                
                if ( this.Name == "Artwork" )
                {
                    output += @"
owner    : 090-736-821";
                }
                else if ( this.Name == "Development" )
                {
                    output += @"
owner    : 090-736-821";
                }
                else if ( this.Name == "Packaging" )
                {
                    output += @"
owner    : 094-409-761";
                }
                else if ( this.Name != "Translators" )
                {
                    output += @"
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
            
            string owner = Hamachi.Retrieve ( output, "owner" );
            if ( owner != "" )
            {
                this.Owner = owner;
            }
            
            this.Lock = Hamachi.Retrieve ( output, "status" );
            this.Approve = Hamachi.Retrieve ( output, "approve" );
            
            if ( this.Owner == "This computer" )
            {
                this.IsOwner = 1;
            }
            else
            {
                this.IsOwner = 0;
            }
            
            Application.Invoke ( delegate
            {
                MainWindow.networkView.UpdateNetwork ( this );
                
                foreach ( Member member in this.Members )
                {
                    if ( member.ClientId == this.Owner )
                    {
                        MainWindow.networkView.UpdateMember ( this, member );
                    }
                }
            });
        }
        catch {}
        
    }
    
    
    public void AddMember ( Member member )
    {
        
        this.Members.Add ( member );
        
    }
    
    
    public void RemoveMember ( Member member )
    {
        
        this.Members.Remove ( member );
        
    }
    
    
    public void GoOnline ( object o, EventArgs args )
    {
        
        updating = true;
        
        Thread thread = new Thread ( GoOnlineThread );
        thread.Start ();
        
        this.Status = new Status ( "*" );
        
        SetSortStrings ();
        
        MainWindow.networkView.UpdateNetwork ( this );
        
    }
    
    
    private void GoOnlineThread ()
    {
        
        Hamachi.GoOnline ( this );
        
        updating = false;
        
    }
    
    
    public void GoOffline ( object o, EventArgs args )
    {
        
        updating = true;
        
        Thread thread = new Thread ( GoOfflineThread );
        thread.Start ();
        
        this.Status = new Status ( " " );
        
        SetSortStrings ();
        
        MainWindow.networkView.UpdateNetwork ( this );
        
    }
    
    
    private void GoOfflineThread ()
    {
        
        Hamachi.GoOffline ( this );
        
        updating = false;
        
    }
    
    
    public void ChangePassword ( object o, EventArgs args )
    {
        
        new Dialogs.ChangePassword ( this );
        
    }
    
    
    public void SetLock ( string locked )
    {
        
        updating = true;
        
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
        
        updating = false;
        
    }
    
    
    public void SetApproval ( string approval )
    {
        
        updating = true;
        
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
        
        updating = false;
        
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
        
        Dialogs.Confirm dlg = new Dialogs.Confirm ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Question", label, null );
        
        if ( dlg.response == "Ok" )
        {
            if ( Config.Settings.DemoMode )
            {
                MainWindow.networkView.RemoveNetwork ( this );
            }
            else
            {
                Thread thread = new Thread ( LeaveThread );
                thread.Start ();
            }
        }
        
    }
    
    
    private void LeaveThread ()
    {
        
        Hamachi.Leave ( this );

        Application.Invoke ( delegate
        {
            Controller.UpdateConnection (); // Update list
        });
        
    }
    
    
    public void Delete ( object o, EventArgs args )
    {
        
        string label   = Stock.Delete;
        string heading = String.Format ( TextStrings.confirmDeleteNetworkHeading, this.Name );
        string message = String.Format ( TextStrings.confirmDeleteNetworkMessage, this.Name );
        
        Dialogs.Confirm dlg = new Dialogs.Confirm ( Haguichi.mainWindow.ReturnWindow (), heading, message, "Warning", label, Stock.Delete );
        
        if ( dlg.response == "Ok" )
        {
            if ( Config.Settings.DemoMode )
            {
                MainWindow.networkView.RemoveNetwork ( this );
            }
            else
            {
                Thread thread = new Thread ( DeleteThread );
                thread.Start ();
            }
        }
        
    }
    
    
    private void DeleteThread ()
    {
        
        Hamachi.Delete ( this );

        Application.Invoke ( delegate
        {
            Controller.UpdateConnection (); // Update list
        });
        
    }
    
}
