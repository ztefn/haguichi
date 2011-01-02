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
using Gtk;


namespace Menus
{

    public class MemberMenu : Menu
    {
        
        private Member member;
        private Network network;
        
        private ImageMenuItem copyId;
        private ImageMenuItem copyAddress;
        private ImageMenuItem approve;
        private ImageMenuItem reject;
        private ImageMenuItem evict;
        
        private SeparatorMenuItem separator1;
        private SeparatorMenuItem separator2;
        
        private ArrayList customItems;
        
        
        public MemberMenu ()
        {
            
            separator1 = new SeparatorMenuItem ();
            separator2 = new SeparatorMenuItem ();
            
            
            approve = new ImageMenuItem ( TextStrings.approveLabel );
            reject = new ImageMenuItem ( TextStrings.rejectLabel );
            
            copyId = new ImageMenuItem ( TextStrings.copyClientIdLabel );
            copyId.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            copyAddress = new ImageMenuItem ( TextStrings.copyAddressLabel );
            copyAddress.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            evict = new ImageMenuItem ( TextStrings.evictLabel );
            
            customItems = new ArrayList ();
            
            AddCustomCommands ();
            
            this.Add ( approve );
            this.Add ( reject );
            this.Add ( separator1 );
            this.Add ( copyAddress );
            this.Add ( copyId );
            this.Add ( separator2 );
            this.Add ( evict );
            
            this.ShowAll ();
            
            if ( Hamachi.ApiVersion == 1 )
            {
                copyId.Visible = false;
            }
            
        }
        
        
        private void AddCustomCommands ()
        {
         
            string [] commands = ( string [] ) Config.Client.Get ( Config.Settings.CustomCommands );
            
            if ( Utilities.AsString ( commands ) == Utilities.AsString ( Config.Settings.DefaultCommands ) )
            {
                commands = Config.Settings.SessionDefaultCommands;
            }
            
            foreach ( string c in commands )
            {
                
                string[] cArray = c.Split ( new char [] { ';' }, 5 );
                
                if ( ( cArray.GetLength ( 0 ) == 5 ) && ( cArray [ 0 ] == "true" ) )
                {
                    string icon    = cArray [ 2 ];
                    string label   = cArray [ 3 ];
                    string command = cArray [ 4 ];
                    
                    CommandMenuItem custom = new Menus.CommandMenuItem ( icon, label, command );
                    
                    customItems.Add ( custom );
                    
                    this.Append ( custom );
                }
                
            }
            
        }
        
        
        private void HideCustomCommands ()
        {
         
            foreach ( CommandMenuItem item in customItems )
            {
                item.Visible = false;
            }
            
        }
        
        
        private void ShowCustomCommands ()
        {
         
            foreach ( CommandMenuItem item in customItems )
            {
                item.Visible = true;
            }
            
        }
        
        
        public void SetMember ( Member memb, Network netw )
        {
            
            /* Remove event handlers from the previous member */
            copyId.Activated      -= new EventHandler ( member.CopyClientIdToClipboard );
            copyAddress.Activated -= new EventHandler ( member.CopyAddressToClipboard );
            approve.Activated     -= new EventHandler ( member.Approve );
            reject.Activated      -= new EventHandler ( member.Reject );
            evict.Activated       -= new EventHandler ( member.Evict );

            /* Set the new member */
            this.member = memb;
            this.network = netw;
            
            /* Set menu items to show */
            if ( ( network.IsOwner == 1 ) &&
                 ( member.Status.statusInt != 3 ) )
            {
                separator2.Visible = true;
                evict.Visible      = true;
            }
            else
            {
                separator2.Visible = false;
                evict.Visible      = false;
            }
            
            if ( member.Status.statusInt != 3 )
            {
                ShowCustomCommands ();
                
                copyAddress.Visible = true;
                approve.Visible     = false;
                reject.Visible      = false;
            }
            else
            {
                HideCustomCommands ();
                
                copyAddress.Visible = false;
                approve.Visible     = true;
                reject.Visible      = true;
            }
            
            /* Add event handlers for the new member */
            copyId.Activated      += new EventHandler ( member.CopyClientIdToClipboard );
            copyAddress.Activated += new EventHandler ( member.CopyAddressToClipboard );
            approve.Activated     += new EventHandler ( member.Approve );
            reject.Activated      += new EventHandler ( member.Reject );
            evict.Activated       += new EventHandler ( member.Evict );
            
        }
        
    }
    
}
