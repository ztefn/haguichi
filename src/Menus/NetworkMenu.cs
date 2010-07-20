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
        
        private ImageMenuItem copy;
        private ImageMenuItem goOnline;
        private ImageMenuItem goOffline;
        private ImageMenuItem leave;
        private ImageMenuItem delete;
        
        private SeparatorMenuItem separator;
        
        
        public NetworkMenu ()
        {
            
            ag = new AccelGroup ();
            
            goOnline = new ImageMenuItem ( TextStrings.goOnlineLabel );
            goOffline = new ImageMenuItem ( TextStrings.goOfflineLabel );
            
            copy = new ImageMenuItem ( TextStrings.copyNetworkIdLabel );
            copy.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            separator = new SeparatorMenuItem ();
            
            leave = new ImageMenuItem ( TextStrings.leaveLabel );
            delete = new ImageMenuItem ( Stock.Delete, ag );
            
            this.Add ( goOnline );
            this.Add ( goOffline );
            this.Add ( leave );
            this.Add ( delete );
            this.Add ( separator );
            this.Add ( copy );
            
            this.ShowAll ();
            
            if ( Hamachi.apiVersion == 1 )
            {
                separator.Visible = false;
                copy.Visible = false;
            }
            
        }
        
        
        public void SetNetwork ( Network netw )
        {
            
            /*
             * Remove event handlers from the previous network
             */
            goOnline.Activated   -= new EventHandler ( network.GoOnline );
            goOffline.Activated  -= new EventHandler ( network.GoOffline );
            copy.Activated       -= new EventHandler ( network.CopyIdToClipboard );
            leave.Activated      -= new EventHandler ( network.Leave );
            delete.Activated     -= new EventHandler ( network.Delete );

            /*
             * Set the new network
             */
            this.network = netw;
            
            goOnline.Activated   += new EventHandler ( network.GoOnline );
            goOffline.Activated  += new EventHandler ( network.GoOffline );
            copy.Activated       += new EventHandler ( network.CopyIdToClipboard );
            leave.Activated      += new EventHandler ( network.Leave );
            delete.Activated     += new EventHandler ( network.Delete );
            
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
            }
            else if ( network.IsOwner == 1 )
            {
                leave.Visible     = false;
                delete.Visible    = true;
            }
            else if ( network.IsOwner == 0 )
            {
                leave.Visible     = true;
                delete.Visible    = false;
            }
            
        }
        
    }
    
}
