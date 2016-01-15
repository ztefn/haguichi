/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

public class Headerbar : HeaderBar
{
    private string mode;
    
    public ToggleButton connect_but;
    public MenuButton   network_but;
    public MenuButton   client_but;
    public ToggleButton search_but;
    public Button       refresh_but;
    public MenuButton   gear_but;
    
    public Box box;
    
    private int minimum_width;
    private int last_allocated;
    
    private int connect_but_width;
    private int client_but_width;
    private int box_width;
    private int gear_but_width;
    private int search_but_width;
    
    public Headerbar ()
    {
        show_close_button = Haguichi.window_use_header_bar;
        
        
        connect_but = new ToggleButton();
        connect_but.image = new Image.from_icon_name ("system-shutdown-symbolic", IconSize.MENU);
        connect_but.set_action_name ("app.connect");
        connect_but.get_style_context().add_class ("suggested-action");
        
        
        network_but = new MenuButton();
        
        var join   = new GLib.MenuItem (Text.join_network_label,   "app.join-network");
        var create = new GLib.MenuItem (Text.create_network_label, "app.create-network");
        
        var network_menu = new GLib.Menu();
        network_menu.append_item (join);
        network_menu.append_item (create);
        
        network_but.image = new Image.from_icon_name ("list-add-symbolic", IconSize.MENU);
        network_but.menu_model = network_menu;
        
        
        var copy_ipv4   = new GLib.MenuItem (Text.copy_address_ipv4_label, "app.copy-ipv4");
        var copy_ipv6   = new GLib.MenuItem (Text.copy_address_ipv6_label, "app.copy-ipv6");
        var copy_id     = new GLib.MenuItem (Text.copy_client_id_label,    "app.copy-id");
        var change_nick = new GLib.MenuItem (Text.change_nick_label,       "app.change-nick");
        var info        = new GLib.MenuItem (Text.information_label,       "app.info");
        
        var info_section = new GLib.Menu ();
        info_section.append_item (info);
        
        var nick_section = new GLib.Menu ();
        nick_section.append_item (change_nick);
        
        var copy_section = new GLib.Menu ();
        copy_section.append_item (copy_ipv4);
        copy_section.append_item (copy_ipv6);
        copy_section.append_item (copy_id);
        
        var client_menu = new GLib.Menu();
        client_menu.append_section (null, info_section);
        client_menu.append_section (null, nick_section);
        client_menu.append_section (null, copy_section);
        
        client_but = new MenuButton();
        client_but.image = new Image.from_icon_name ("avatar-default-symbolic", IconSize.MENU);
        client_but.menu_model = client_menu;
        client_but.toggled.connect (() =>
        {
            if (client_but.active)
            {
                string[] address = Hamachi.get_address();
                string ipv4 = address[0];
                string ipv6 = address[1];
                
                copy_section.remove_all();
                
                if ((ipv4 != null) &&
                    (ipv4 != ""))
                {
                    copy_section.append_item (copy_ipv4);
                }
                if (ipv6 != null)
                {
                    copy_section.append_item (copy_ipv6);
                }
                if (Hamachi.get_client_id() != "")
                {
                    copy_section.append_item (copy_id);
                }
            }
        });
        
        
        search_but = new ToggleButton();
        search_but.image = new Image.from_icon_name ("edit-find-symbolic", IconSize.MENU);
        search_but.set_action_name ("app.toggle-search");
        
        
        refresh_but = new Button();
        refresh_but.image = new Image.from_icon_name ("view-refresh-symbolic", IconSize.MENU);
        refresh_but.set_action_name ("app.refresh");
        
        
        var gear_menu = new GLib.Menu ();
        
        var open_config    = new GLib.MenuItem (Text.config_folder_label,  "app.open-config");
        var save_config    = new GLib.MenuItem (Text.config_save_label,    "app.save-config");
        var restore_config = new GLib.MenuItem (Text.config_restore_label, "app.restore-config");
        
        var sort_by_name = new GLib.MenuItem (Text.sort_by_name, "app.sort-by");
        sort_by_name.set_attribute_value ("target", "name");
        
        var sort_by_status = new GLib.MenuItem (Text.sort_by_status, "app.sort-by");
        sort_by_status.set_attribute_value ("target", "status");
        
        var show_offline = new GLib.MenuItem (Text.checkbox_show_offline_members, "app.show-offline-members");
        var preferences  = new GLib.MenuItem (Text.preferences_label,             "app.preferences");
        var donate       = new GLib.MenuItem (Text.donate_label,                  "app.donate");
        var help         = new GLib.MenuItem (Text.help_label,                    "app.help");
        var about        = new GLib.MenuItem (Text.about_label,                   "app.about");
        var quit         = new GLib.MenuItem (Text.quit_label,                    "app.quit");
        
        var backup_section = new GLib.Menu ();
        backup_section.append_item (save_config);
        backup_section.append_item (restore_config);
        
        var config_submenu = new GLib.Menu ();
        config_submenu.append_item (open_config);
        config_submenu.append_section (null, backup_section);
        
        var sort_section = new GLib.Menu ();
        sort_section.append_item (sort_by_name);
        sort_section.append_item (sort_by_status);
        
        var filter_section = new GLib.Menu ();
        filter_section.append_item (show_offline);
        
        var preferences_section = new GLib.Menu ();
        preferences_section.append_item (preferences);
        
        var donate_section = new GLib.Menu ();
        donate_section.append_item (donate);
        
        var meta_section = new GLib.Menu ();
        meta_section.append_item (help);
        meta_section.append_item (about);
        meta_section.append_item (quit);
        
        gear_menu.append_submenu (Text.config_label, config_submenu);
        gear_menu.append_section (null, sort_section);
        gear_menu.append_section (null, filter_section);
        
        if (!Haguichi.app.prefers_app_menu())
        {
            gear_menu.append_section (null, preferences_section);
            gear_menu.append_section (null, donate_section);
            gear_menu.append_section (null, meta_section);
        }
        
        gear_but = new MenuButton();
        gear_but.image = new Image.from_icon_name ("open-menu-symbolic", IconSize.MENU);
        gear_but.menu_model = gear_menu;
        gear_but.add_accelerator ("clicked", HaguichiWindow.accel_group, Gdk.Key.F10, 0, AccelFlags.VISIBLE);
        
        
        box = new Box (Orientation.HORIZONTAL, 0);
        box.get_style_context().add_class ("linked");
        box.add (network_but);
        box.add (refresh_but);
        
        
        pack_start (connect_but);
        pack_start (client_but);
        pack_start (box);
        pack_end (gear_but);
        pack_end (search_but);
        
        show_all();
        
        connect_but.get_preferred_width (out connect_but_width, null);
        client_but.get_preferred_width (out client_but_width, null);
        box.get_preferred_width (out box_width, null);
        search_but.get_preferred_width (out search_but_width, null);
        gear_but.get_preferred_width (out gear_but_width, null);
        
        hide_all_buttons();
        
        get_preferred_width (out minimum_width, null);
    }
    
    public void show_all_buttons ()
    {
        connect_but.show();
        client_but.show();
        box.show();
        search_but.show();
        gear_but.show();
    }
    
    public void hide_all_buttons ()
    {
        connect_but.hide();
        client_but.hide();
        box.hide();
        search_but.hide();
        gear_but.hide();
    }
    
    public void show_hide_buttons (int allocated, int minimum)
    {
        if (allocated == last_allocated)
        {
            return;
        }
        last_allocated = allocated;
        
        hide_all_buttons();
        
        int current = minimum + connect_but_width;
        connect_but.show();
        
        current += gear_but_width;
        if (allocated > current)
        {
            gear_but.show();
        }
        
        current += client_but_width;
        if (allocated > current)
        {
            client_but.show();
        }
        
        current += search_but_width;
        if (allocated > current)
        {
            search_but.show();
        }
        
        current += box_width;
        if (allocated > current)
        {
            box.show();
        }
    }
    
    public override void get_preferred_width (out int new_minimum_width, out int new_natural_width)
    {
        if ((Haguichi.window_use_header_bar == false) &&
            (minimum_width > 0))
        {
            // Keep existing minimum width
        }
        else if (HaguichiWindow.minimum_width > 0)
        {
            minimum_width = HaguichiWindow.minimum_width;
        }
        else
        {
            int base_minimum_width;
            int base_natural_width;
            
            base.get_preferred_width (out base_minimum_width, out base_natural_width);
            
            minimum_width = base_minimum_width;
        }
        
        new_minimum_width = minimum_width;
        new_natural_width = minimum_width;
    }
    
    public void set_mode (string _mode)
    {
        mode = _mode;
        
        GlobalActions.connect.set_enabled (false);
        GlobalActions.disconnect.set_enabled (false);
        
        GlobalActions.info.set_enabled (false);
        
        GlobalActions.join_network.set_enabled (false);
        GlobalActions.create_network.set_enabled (false);
        
        GlobalActions.refresh.set_enabled (false);
        
        GlobalActions.start_search.set_enabled (false);
        GlobalActions.toggle_search.set_enabled (false);
        
        network_but.sensitive = false;
        
        switch (mode)
        {
            case "Countdown":
                GlobalActions.connect.set_enabled (true);
                
                set_subtitle (Utils.format (Text.reconnecting (Controller.restore_countdown), Controller.restore_countdown.to_string(), null, null));
                break;
                
            case "Connecting":
                connect_but.active = true;
                
                set_subtitle (Text.connecting);
                break;
                
            case "Connected":
                connect_but.set_action_name ("app.disconnect");
                connect_but.active = true;
                GlobalActions.disconnect.set_enabled (true);
                
                GlobalActions.info.set_enabled (true);
                
                GlobalActions.join_network.set_enabled (true);
                GlobalActions.create_network.set_enabled (true);
                
                GlobalActions.refresh.set_enabled (true);
                
                GlobalActions.start_search.set_enabled (true);
                GlobalActions.toggle_search.set_enabled (true);
                
                network_but.sensitive = true;
                
                set_subtitle (Text.connected);
                break;
                
            case "Disconnected":
                connect_but.set_action_name ("app.connect");
                connect_but.active = false;
                GlobalActions.connect.set_enabled (true);
                
                GlobalActions.info.set_enabled (true);
                
                GlobalEvents.stop_search ();
                
                set_subtitle (Text.disconnected);
                break;
                
            case "Not configured":
                connect_but.active = false;
                
                GlobalActions.refresh.set_enabled (true);
                
                set_subtitle (Text.disconnected);
                break;
                
            case "Not installed":
                connect_but.active = false;
                
                GlobalActions.refresh.set_enabled (true);
                
                set_subtitle (Text.disconnected);
                break;
        }
    }
}
