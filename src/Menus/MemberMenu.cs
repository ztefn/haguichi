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


namespace Menus
{

    public class MemberMenu : Menu
    {
        
        private Member member;
        private Network network;
        
        private ImageMenuItem copy;
        private ImageMenuItem approve;
        private ImageMenuItem reject;
        private ImageMenuItem evict;
        private SeparatorMenuItem separator;
        
        private ArrayList customItems;
        
        
        public MemberMenu ()
        {
            
            copy = new ImageMenuItem ( TextStrings.copyLabel );
            copy.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            separator = new SeparatorMenuItem ();
            
            approve = new ImageMenuItem ( TextStrings.approveLabel );
            reject  = new ImageMenuItem ( TextStrings.rejectLabel );
            evict   = new ImageMenuItem ( TextStrings.evictLabel );
            
            customItems = new ArrayList ();
            
            AddCustomCommands ();
            
            this.Append ( copy );
            this.Add ( separator );
            this.Append ( approve );
            this.Append ( reject );
            this.Append ( evict );
            
            this.ShowAll ();
            
        }
        
        
        private void AddCustomCommands ()
        {
         
            string [] commands = ( string [] ) Config.Client.Get ( Config.Settings.CustomCommands );
            
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
            
            /*
             * Remove event handlers from the previous member
             */
            copy.Activated    -= new EventHandler ( member.CopyAddressToClipboard );
            approve.Activated -= new EventHandler ( member.Approve );
            reject.Activated  -= new EventHandler ( member.Reject );
            evict.Activated   -= new EventHandler ( member.Evict );

            /*
             * Set the new member
             */
            this.member = memb;
            this.network = netw;
            
            copy.Activated    += new EventHandler ( member.CopyAddressToClipboard );
            approve.Activated += new EventHandler ( member.Approve );
            reject.Activated  += new EventHandler ( member.Reject );
            evict.Activated   += new EventHandler ( member.Evict );
            
            if ( ( network.IsOwner == 1 ) && ( member.Status.statusString != "Unapproved" ) )
            {
                separator.Visible = true;
                evict.Visible = true;
            }
            else
            {
                separator.Visible = false;
                evict.Visible = false;
            }
            
            if ( member.Status.statusString != "Unapproved" )
            {
                ShowCustomCommands ();
                copy.Visible    = true;
                approve.Visible = false;
                reject.Visible  = false;
            }
            else
            {
                HideCustomCommands ();
                copy.Visible    = false;
                approve.Visible = true;
                reject.Visible  = true;
            }
            
        }
        
    }
    
}
