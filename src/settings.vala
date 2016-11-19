/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Settings : Object
{
    public static string schema_base_id;
    
    public static int decorator_offset;
    public static int switch_layout_threshold;
    public static int switch_sidebar_threshold;
    
    public static Key connect_on_startup;
    public static Key disconnect_on_quit;
    public static Key reconnect_on_connection_loss;
    public static Key start_in_tray;
    public static Key update_network_list;
    public static Key custom_commands;
    public static Key init_system;
    public static Key super_user;
    public static Key check_internet_hostname;
    public static Key check_internet_ip;
    public static Key nickname;
    public static Key protocol;
    public static Key reconnect_interval;
    public static Key update_interval;
    public static Key collapsed_networks;
    public static Key long_nicks;
    public static Key network_list_icon_size_large;
    public static Key network_list_icon_size_small;
    public static Key member_template_large;
    public static Key member_template_small;
    public static Key network_template_large;
    public static Key network_template_small;
    public static Key show_offline_members;
    public static Key sort_network_list_by;
    public static Key notify_on_connection_loss;
    public static Key notify_on_member_join;
    public static Key notify_on_member_leave;
    public static Key notify_on_member_offline;
    public static Key notify_on_member_online;
    public static Key win_height;
    public static Key win_maximized;
    public static Key win_x;
    public static Key win_y;
    public static Key prefer_dark_theme;
    public static Key sidebar_position;
    public static Key win_width;

    public static void init ()
    {
        schema_base_id                = "org." + Text.app_name.down();
        
        decorator_offset              = 0;
        switch_layout_threshold       = 620;
        switch_sidebar_threshold      = 480;
        
        connect_on_startup            = new Key ("behavior", "connect-on-startup");
        disconnect_on_quit            = new Key ("behavior", "disconnect-on-quit");
        reconnect_on_connection_loss  = new Key ("behavior", "reconnect-on-connection-loss");
        update_network_list           = new Key ("behavior", "update-network-list");
        custom_commands               = new Key ("commands", "customizable");
        init_system                   = new Key ("commands", "init-system");
        super_user                    = new Key ("commands", "super-user");
        check_internet_hostname       = new Key ("config", "check-internet-hostname");
        check_internet_ip             = new Key ("config", "check-internet-ip");
        nickname                      = new Key ("config", "nickname");
        protocol                      = new Key ("config", "protocol");
        reconnect_interval            = new Key ("config", "reconnect-interval");
        update_interval               = new Key ("config", "update-interval");
        collapsed_networks            = new Key ("network-list", "collapsed-networks");
        long_nicks                    = new Key ("network-list", "long-nicks");
        member_template_large         = new Key ("network-list", "member-template-large");
        member_template_small         = new Key ("network-list", "member-template-small");
        network_template_large        = new Key ("network-list", "network-template-large");
        network_template_small        = new Key ("network-list", "network-template-small");
        show_offline_members          = new Key ("network-list", "show-offline-members");
        sort_network_list_by          = new Key ("network-list", "sort-by");
        notify_on_connection_loss     = new Key ("notifications", "connection-loss");
        notify_on_member_join         = new Key ("notifications", "member-join");
        notify_on_member_leave        = new Key ("notifications", "member-leave");
        notify_on_member_offline      = new Key ("notifications", "member-offline");
        notify_on_member_online       = new Key ("notifications", "member-online");
        win_height                    = new Key ("ui", "height");
        win_maximized                 = new Key ("ui", "maximized");
        win_x                         = new Key ("ui", "position-x");
        win_y                         = new Key ("ui", "position-y");
        prefer_dark_theme             = new Key ("ui", "prefer-dark-theme");
        sidebar_position              = new Key ("ui", "sidebar-position");
        win_width                     = new Key ("ui", "width");
    }
}
