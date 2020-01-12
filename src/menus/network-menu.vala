/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

namespace Menus
{
    public class NetworkMenu : Gtk.Menu
    {
        private Network network;
        
        private Gtk.MenuItem go_online_item;
        private Gtk.MenuItem go_offline_item;
        private Gtk.MenuItem leave_item;
        private Gtk.MenuItem delete_item;
        
        private SeparatorMenuItem separator1;
        
        private CheckMenuItem locked_item;
        private Gtk.MenuItem approve_item;
        private Gtk.MenuItem password_item;
        
        private Gtk.Menu approve_menu;
        private RadioMenuItem auto_item;
        private RadioMenuItem manual_item;
        
        private SeparatorMenuItem separator2;
        
        private Gtk.MenuItem copy_item;
        
        
        public NetworkMenu ()
        {
            go_online_item = new Gtk.MenuItem.with_mnemonic (Text.go_online_label);
            go_offline_item = new Gtk.MenuItem.with_mnemonic (Text.go_offline_label);
            
            copy_item = new Gtk.MenuItem.with_mnemonic (Text.copy_network_id_label);
            
            leave_item = new Gtk.MenuItem.with_mnemonic (Text.leave_label);
            delete_item = new Gtk.MenuItem.with_mnemonic (Text.delete_label);
            
            locked_item = new CheckMenuItem.with_mnemonic (Text.locked_label);
            
            auto_item = new RadioMenuItem.with_mnemonic (null, Text.auto_label);
            manual_item = new RadioMenuItem.with_mnemonic_from_widget (auto_item, Text.manual_label);
            
            approve_menu = new Gtk.Menu();
            approve_menu.add (auto_item);
            approve_menu.add (manual_item);
            
            approve_item = new Gtk.MenuItem.with_mnemonic (Text.approval_label);
            approve_item.submenu = approve_menu;
            
            password_item = new Gtk.MenuItem.with_mnemonic (Text.change_password_label);
            
            separator1 = new SeparatorMenuItem();
            separator2 = new SeparatorMenuItem();
            
            add (go_online_item);
            add (go_offline_item);
            add (leave_item);
            add (delete_item);
            add (separator1);
            add (locked_item);
            add (approve_item);
            add (password_item);
            add (separator2);
            add (copy_item);
            
            show_all();
        }
        
        private void change_lock ()
        {
            if (locked_item.active)
            {
                network.set_lock ("locked");
            }
            else
            {
                network.set_lock ("unlocked");
            }
        }
        
        private void change_approval ()
        {
            if (manual_item.active)
            {
                network.set_approval ("manual");
            }
            else
            {
                network.set_approval ("auto");
            }
        }
        
        public void set_network (Network _network)
        {
            /* Remove event handlers for the previous network if present */
            if (network != null)
            {
                go_online_item.activate.disconnect  (network.go_online);
                go_offline_item.activate.disconnect (network.go_offline);
                copy_item.activate.disconnect       (network.copy_id_to_clipboard);
                leave_item.activate.disconnect      (network.leave);
                delete_item.activate.disconnect     (network.delete);
                locked_item.toggled.disconnect      (change_lock);
                auto_item.toggled.disconnect        (change_approval);
                password_item.activate.disconnect   (network.change_password);
            }
            
            /* Set the new network */
            network = _network;
            
            /* Set menu items to show */
            if (network.status.status_int == 0)
            {
                go_online_item.visible  = true;
                go_offline_item.visible = false;
            }
            else
            {
                go_online_item.visible  = false;
                go_offline_item.visible = true;
            }
            
            if (network.is_owner == -1)
            {
                leave_item.visible     = false;
                delete_item.visible    = false;
                password_item.visible  = false;
            }
            else if (network.is_owner == 1)
            {
                leave_item.visible     = false;
                delete_item.visible    = true;
                password_item.visible  = true;
            }
            else if (network.is_owner == 0)
            {
                leave_item.visible     = true;
                delete_item.visible    = false;
                password_item.visible  = false;
            }
            
            if ((network.lock_state != "") ||
                (network.approve != ""))
            {
                separator2.visible = true;
            }
            else
            {
                separator2.visible = false;
            }
            
            if (network.lock_state != "")
            {
                locked_item.visible = true;
                
                if (network.lock_state == "locked")
                {
                    locked_item.active = true;
                }
                else
                {
                    locked_item.active = false;
                }
            }
            else
            {
                locked_item.visible = false;
            }
            
            if (network.approve != "")
            {
                approve_item.visible = true;
                
                if (network.approve == "manual")
                {
                    manual_item.active = true;
                }
                else
                {
                    auto_item.active = true;
                }
            }
            else
            {
                approve_item.visible = false;
            }
            
            /* Add event handlers for the new network */
            go_online_item.activate.connect  (network.go_online);
            go_offline_item.activate.connect (network.go_offline);
            copy_item.activate.connect       (network.copy_id_to_clipboard);
            leave_item.activate.connect      (network.leave);
            delete_item.activate.connect     (network.delete);
            locked_item.toggled.connect      (change_lock);
            auto_item.toggled.connect        (change_approval);
            password_item.activate.connect   (network.change_password);
        }
    }
}
