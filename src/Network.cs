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
using Gtk;


public class Network
{

    public Status Status;
    public string Name;
    public ArrayList Members;
    public int IsOwner;
    
    public string NameSortString;
    public string StatusSortString;
    
    
    public Network ( Status status, string name )
    {
        
        this.Status  = status;
        this.Name    = name;
        this.Members = new ArrayList();
        this.IsOwner = -1;
        
        SetSortStrings ();
        
    }
    
    
    public Network ( Status status, string name, ArrayList members )
    {
        
        this.Status  = status;
        this.Name    = name;
        this.Members = members;
        this.IsOwner = -1;
        
        SetSortStrings ();
        
    }
    
    
    private void SetSortStrings ()
    {
        
        NameSortString   = this.Name;
        StatusSortString = this.Status.statusSortable + this.Name;
        
    }
    
    
    public void Update ( Status status, string name )
    {
        
        this.Status = status;
        this.Name   = name;
        
        SetSortStrings ();
        
    }
    
    
    public void ReturnMemberCount ( out int totalCount, out int onlineCount )
    {
        
        totalCount  = 0;
        onlineCount = 0;

        foreach ( Member member in Members )
        {
            totalCount ++;
                
            if ( member.Status.statusInt != 0 )
            {
                onlineCount ++;
            }
        }
        
    }
    
    
    public void DetermineOwnership ()
    {
        
        string output = Command.ReturnOutput ( "hamachi", "evict '" + this.Name + "' 0.0.0.0" );
        Debug.Log ( Debug.Domain.Hamachi, "Network.DetermineOwnership", output );

        if ( output.IndexOf ( ".. ok" ) != -1 )
        {
            this.IsOwner = 1;
        }
        else
        {
            this.IsOwner = 0;
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
        
        Hamachi.GoOnline ( this );
        Controller.UpdateConnection (); // Update list
        
    }
    
    
    public void GoOffline ( object o, EventArgs args )
    {
        
        Hamachi.GoOffline ( this );
        Controller.UpdateConnection (); // Update list
        
    }
    
    
    public void Leave ( object o, EventArgs args )
    {
    
        string label   = TextStrings.leaveLabel;
        string heading = String.Format ( TextStrings.confirmLeaveNetworkHeading, this.Name );
        string message = String.Format ( TextStrings.confirmLeaveNetworkMessage, this.Name );
        
        Dialogs.Confirm dlg = new Dialogs.Confirm ( heading, message, "Warning", label, null );
        
        if ( dlg.response == "Ok" )
        {
            Hamachi.Leave ( this );
            Controller.UpdateConnection (); // Update list
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
            Hamachi.Delete ( this );
            Controller.UpdateConnection (); // Update list
        }
        
    }

}
