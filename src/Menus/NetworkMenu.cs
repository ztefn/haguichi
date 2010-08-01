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
using Gtk;


namespace Menus
{

    public class NetworkMenu : Menu
    {
        
        private Network network;
        
        private AccelGroup ag;
        
        private ImageMenuItem goOnline;
        private ImageMenuItem goOffline;
        private ImageMenuItem leave;
        private ImageMenuItem delete;
        
        private SeparatorMenuItem separator1;
        
        private CheckMenuItem locked;
        private ImageMenuItem approve;
        private ImageMenuItem password;
        
        private Menu approveMenu;
        private RadioMenuItem approveGroup;
        private RadioMenuItem auto;
        private RadioMenuItem manual;
        
        private SeparatorMenuItem separator2;
        
        private ImageMenuItem copy;
        
        
        public NetworkMenu ()
        {
            
            ag = new AccelGroup ();
            
            goOnline = new ImageMenuItem ( TextStrings.goOnlineLabel );
            goOffline = new ImageMenuItem ( TextStrings.goOfflineLabel );
            
            copy = new ImageMenuItem ( TextStrings.copyNetworkIdLabel );
            copy.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            leave = new ImageMenuItem ( TextStrings.leaveLabel );
            delete = new ImageMenuItem ( Stock.Delete, ag );
            
            separator1 = new SeparatorMenuItem ();
            
            locked = new CheckMenuItem ( TextStrings.lockedLabel );
            
            approveGroup = new RadioMenuItem ( "approve" );
            
            auto = new RadioMenuItem ( approveGroup, TextStrings.autoLabel );
            manual = new RadioMenuItem ( approveGroup, TextStrings.manualLabel );
            
            approveMenu = new Menu ();
            approveMenu.Add ( auto );
            approveMenu.Add ( manual );
            
            approve = new ImageMenuItem ( TextStrings.approvalLabel );
            approve.Submenu = approveMenu;
            
            password = new ImageMenuItem ( TextStrings.changePasswordLabel );
            
            separator2 = new SeparatorMenuItem ();
            
            this.Add ( goOnline );
            this.Add ( goOffline );
            this.Add ( leave );
            this.Add ( delete );
            this.Add ( separator1 );
            this.Add ( locked );
            this.Add ( approve );
            this.Add ( password );
            this.Add ( separator2 );
            this.Add ( copy );
            
            this.ShowAll ();
            
            if ( Hamachi.apiVersion == 1 )
            {
                separator1.Visible = false;
                copy.Visible = false;
            }
            
        }
        
        
        private void ChangeLock ( object o, EventArgs args )
        {
            
            if ( locked.Active )
            {
                this.network.SetLock ( "locked" );
            }
            else
            {
                this.network.SetLock ( "unlocked" );
            }
            
        }
        
        
        private void ChangeApproval ( object o, EventArgs args )
        {
            
            if ( manual.Active )
            {
                this.network.SetApproval ( "manual" );
            }
            else
            {
                this.network.SetApproval ( "auto" );
            }
            
        }
        
        
        public void SetNetwork ( Network netw )
        {
            
            /* Remove event handlers from the previous network */
            goOnline.Activated   -= new EventHandler ( network.GoOnline );
            goOffline.Activated  -= new EventHandler ( network.GoOffline );
            copy.Activated       -= new EventHandler ( network.CopyIdToClipboard );
            leave.Activated      -= new EventHandler ( network.Leave );
            delete.Activated     -= new EventHandler ( network.Delete );
            locked.Toggled       -= new EventHandler ( ChangeLock );
            auto.Toggled         -= new EventHandler ( ChangeApproval );
            password.Activated   -= new EventHandler ( network.ChangePassword );

            
            /* Set the new network */
            this.network = netw;
            
            
            /* Set menu items to show */
            if ( network.Status.statusInt == 0 )
            {
                goOnline.Visible  = true;
                goOffline.Visible = false;
            }
            else
            {
                goOnline.Visible  = false;
                goOffline.Visible = true;
            }
            
            if ( network.IsOwner == -1 )
            {
                leave.Visible     = false;
                delete.Visible    = false;
                password.Visible  = false;
            }
            else if ( network.IsOwner == 1 )
            {
                leave.Visible     = false;
                delete.Visible    = true;
                
                if ( Hamachi.apiVersion > 1 )
                {
                    password.Visible  = true;
                }
                else
                {
                    password.Visible  = false;
                }
            }
            else if ( network.IsOwner == 0 )
            {
                leave.Visible     = true;
                delete.Visible    = false;
                password.Visible  = false;
            }
            
            if ( ( network.Lock != "" ) ||
                 ( network.Approve != "" ) )
            {
                separator2.Visible = true;
            }
            else
            {
                separator2.Visible = false;
            }
            
            if ( network.Lock != "" )
            {
                locked.Visible    = true;
                
                if ( network.Lock == "locked" )
                {
                    locked.Active = true;
                }
                else
                {
                    locked.Active = false;
                }
            }
            else
            {
                locked.Visible    = false;
            }
            
            if ( network.Approve != "" )
            {
                approve.Visible   = true;
                
                if ( network.Approve == "manual" )
                {
                    manual.Active = true;
                }
                else
                {
                    auto.Active   = true;
                }
            }
            else
            {
                approve.Visible   = false;
            }
            
            /* Add event handlers for the new network */
            goOnline.Activated   += new EventHandler ( network.GoOnline );
            goOffline.Activated  += new EventHandler ( network.GoOffline );
            copy.Activated       += new EventHandler ( network.CopyIdToClipboard );
            leave.Activated      += new EventHandler ( network.Leave );
            delete.Activated     += new EventHandler ( network.Delete );
            locked.Toggled       += new EventHandler ( ChangeLock );
            auto.Toggled         += new EventHandler ( ChangeApproval );
            password.Activated   += new EventHandler ( network.ChangePassword );
            
        }
        
    }
    
}
