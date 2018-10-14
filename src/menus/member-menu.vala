/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2018 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

namespace Menus
{
    public class MemberMenu : Gtk.Menu
    {
        private Member member;
        private Network network;
        
        private Gtk.MenuItem copy_id;
        private Gtk.MenuItem copy_ipv4;
        private Gtk.MenuItem copy_ipv6;
        private Gtk.MenuItem approve;
        private Gtk.MenuItem reject;
        private Gtk.MenuItem evict;
        
        private SeparatorMenuItem separator1;
        private SeparatorMenuItem separator2;
        
        private List<CommandMenuItem> custom_items;
        
        public MemberMenu ()
        {
            approve   = new Gtk.MenuItem.with_mnemonic (Text.approve_label);
            reject    = new Gtk.MenuItem.with_mnemonic (Text.reject_label);
            copy_id   = new Gtk.MenuItem.with_mnemonic (Text.copy_client_id_label);
            copy_ipv4 = new Gtk.MenuItem.with_mnemonic (Text.copy_address_ipv4_label);
            copy_ipv6 = new Gtk.MenuItem.with_mnemonic (Text.copy_address_ipv6_label);
            evict     = new Gtk.MenuItem.with_mnemonic (Text.evict_label);
            
            separator1 = new SeparatorMenuItem();
            separator2 = new SeparatorMenuItem();
            
            custom_items = new List<CommandMenuItem>();
            
            add_custom_commands();
            
            add (approve);
            add (reject);
            add (separator1);
            add (copy_ipv4);
            add (copy_ipv6);
            add (copy_id);
            add (separator2);
            add (evict);
            
            show_all();
        }
        
        private void add_custom_commands ()
        {
            string[] commands = (string[]) Settings.custom_commands.val;
            
            foreach (string c in commands)
            {
                string[] cArray = c.split (";", 6);
                
                if ((cArray.length == 6) &&
                    (cArray[0] == "true"))
                {
                    string label        = cArray[2];
                    string command_ipv4 = cArray[3];
                    string command_ipv6 = cArray[4];
                    string priority     = cArray[5];
                    
                    if (Command.custom_exists (command_ipv4, command_ipv6))
                    {
                        CommandMenuItem custom = new CommandMenuItem (label, command_ipv4, command_ipv6, priority);
                        custom_items.append (custom);
                        append (custom);
                    }
                }
            }
        }
        
        private void hide_custom_commands ()
        {
            foreach (CommandMenuItem item in custom_items)
            {
                item.visible = false;
            }
        }
        
        private void show_custom_commands ()
        {
            foreach (CommandMenuItem item in custom_items)
            {
                item.visible = true;
                item.sensitive = true;
                
                if ((member.status.status_int != 1) &&
                    (item.command_ipv4.contains("%A") ||
                     item.command_ipv6.contains("%A")))
                {
                    item.sensitive = false;
                }
            }
        }
        
        public void set_member (Member _member, Network _network)
        {
            /* Remove event handlers for the previous member if present */
            if (member != null)
            {
                copy_id.activate.disconnect   (member.copy_client_id_to_clipboard);
                copy_ipv4.activate.disconnect (member.copy_ipv4_to_clipboard);
                copy_ipv6.activate.disconnect (member.copy_ipv6_to_clipboard);
                approve.activate.disconnect   (member.approve);
                reject.activate.disconnect    (member.reject);
                evict.activate.disconnect     (member.evict);
            }

            /* Set the new member */
            member  = _member;
            network = _network;
            
            /* Set menu items to show */
            if ((network.is_owner == 1) &&
                (member.status.status_int != 3))
            {
                separator2.visible = true;
                evict.visible      = true;
            }
            else
            {
                separator2.visible = false;
                evict.visible      = false;
            }
            
            copy_ipv4.visible = false;
            copy_ipv6.visible = false;
            
            if (member.status.status_int != 3)
            {
                show_custom_commands();
                
                if (member.ipv4 != null)
                {
                    copy_ipv4.visible = true;
                }
                if (member.ipv6 != null)
                {
                    copy_ipv6.visible = true;
                }
                
                approve.visible = false;
                reject.visible  = false;
            }
            else
            {
                hide_custom_commands();
                
                approve.visible = true;
                reject.visible  = true;
            }
            
            /* Add event handlers for the new member */
            copy_id.activate.connect   (member.copy_client_id_to_clipboard);
            copy_ipv4.activate.connect (member.copy_ipv4_to_clipboard);
            copy_ipv6.activate.connect (member.copy_ipv6_to_clipboard);
            approve.activate.connect   (member.approve);
            reject.activate.connect    (member.reject);
            evict.activate.connect     (member.evict);
        }
    }
}
