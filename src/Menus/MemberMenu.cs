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
using System.Collections;
using Gtk;


namespace Menus
{

    public class MemberMenu : Menu
    {
        
        private Member member;
        private Network network;
        
        private ImageMenuItem copyId;
        private ImageMenuItem copyIPv4;
        private ImageMenuItem copyIPv6;
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
            
            copyIPv4 = new ImageMenuItem ( TextStrings.copyAddressIPv4Label );
            copyIPv4.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            copyIPv6 = new ImageMenuItem ( TextStrings.copyAddressIPv6Label );
            copyIPv6.Image = new Image ( Stock.Copy, IconSize.Menu );
            
            evict = new ImageMenuItem ( TextStrings.evictLabel );
            
            customItems = new ArrayList ();
            
            AddCustomCommands ();
            
            this.Add ( approve     );
            this.Add ( reject      );
            this.Add ( separator1  );
            this.Add ( copyIPv4    );
            this.Add ( copyIPv6    );
            this.Add ( copyId      );
            this.Add ( separator2  );
            this.Add ( evict       );
            
            this.ShowAll ();
            
        }
        
        
        private void AddCustomCommands ()
        {
         
            string [] commands = ( string [] ) Config.Client.Get ( Config.Settings.CustomCommands );
            
            foreach ( string c in commands )
            {
                
                string [] cArray = c.Split ( new char [] { ';' }, 7 );
                
                if ( ( cArray.GetLength ( 0 ) == 7 ) &&
                     ( cArray [0] == "true" ) )
                {
                    string icon        = cArray [2];
                    string label       = cArray [3];
                    string commandIPv4 = cArray [4];
                    string commandIPv6 = cArray [5];
                    string priority    = cArray [6];
                    
                    CommandMenuItem custom = new Menus.CommandMenuItem ( icon, label, commandIPv4, commandIPv6, priority );
                    
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
                item.Sensitive = true;
                
                if ( member.Status.statusInt != 1 )
                {
                    item.Sensitive = false;
                }
            }
            
        }
        
        
        public void SetMember ( Member memb, Network netw )
        {
            
            /* Remove event handlers for the previous member if present */
            if ( member != null )
            {
                copyId.Activated      -= new EventHandler ( member.CopyClientIdToClipboard );
                copyIPv4.Activated    -= new EventHandler ( member.CopyIPv4ToClipboard     );
                copyIPv6.Activated    -= new EventHandler ( member.CopyIPv6ToClipboard     );
                approve.Activated     -= new EventHandler ( member.Approve                 );
                reject.Activated      -= new EventHandler ( member.Reject                  );
                evict.Activated       -= new EventHandler ( member.Evict                   );
            }

            /* Set the new member */
            member  = memb;
            network = netw;
            
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
            
            copyIPv4.Visible = false;
            copyIPv6.Visible = false;
            
            if ( member.Status.statusInt != 3 )
            {
                ShowCustomCommands ();
                
                if ( member.IPv4 != "" )
                {
                    copyIPv4.Visible = true;
                }
                if ( member.IPv6 != "" )
                {
                    copyIPv6.Visible = true;
                }
                
                approve.Visible = false;
                reject.Visible  = false;
            }
            else
            {
                HideCustomCommands ();
                
                approve.Visible     = true;
                reject.Visible      = true;
            }
            
            /* Add event handlers for the new member */
            copyId.Activated      += new EventHandler ( member.CopyClientIdToClipboard );
            copyIPv4.Activated    += new EventHandler ( member.CopyIPv4ToClipboard     );
            copyIPv6.Activated    += new EventHandler ( member.CopyIPv6ToClipboard     );
            approve.Activated     += new EventHandler ( member.Approve                 );
            reject.Activated      += new EventHandler ( member.Reject                  );
            evict.Activated       += new EventHandler ( member.Evict                   );
            
        }
        
    }
    
}
