/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2021 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class GlobalActions
{
    public static SimpleAction connect;
    public static SimpleAction disconnect;
    
    public static SimpleAction approve_reject;
    
    public static SimpleAction join_network;
    public static SimpleAction create_network;
    
    public static SimpleAction refresh;
    
    public static SimpleAction start_search;
    public static SimpleAction stop_search;
    
    public static SimpleAction copy_ipv4;
    public static SimpleAction copy_ipv6;
    public static SimpleAction copy_id;
    
    public static SimpleAction change_nick;
    
    public static SimpleAction open_config;
    public static SimpleAction save_config;
    public static SimpleAction restore_config;
    
    public static Action sort_by;
    public static Action show_offline;
    
    public static SimpleAction preferences;
    public static SimpleAction info;
    public static SimpleAction shortcuts;
    public static SimpleAction help;
    public static SimpleAction about;
    public static SimpleAction quit;
    
    public static void init (Gtk.Application app)
    {
        connect = new SimpleAction ("connect", null);
        connect.activate.connect (GlobalEvents.start_hamachi);
        
        disconnect = new SimpleAction ("disconnect", null);
        disconnect.activate.connect (GlobalEvents.stop_hamachi);
        
        approve_reject = new SimpleAction ("approve-reject", VariantType.STRING_ARRAY);
        approve_reject.activate.connect ((variant) =>
        {
            string[] args = variant.get_strv();
            
            if (args.length > 2)
            {
                string[] network_ids = {};
                
                for (int i = 2; i < args.length; i++)
                {
                    network_ids += args[i];
                }
                
                GlobalEvents.approve_or_reject (args[0], args[1], network_ids);
            }
            else
            {
                Debug.log (Debug.domain.ERROR, "GlobalActions.approve_reject", "Too few arguments, expected \"action, client_id, network_id1, network_id2, ...\"");
            }
        });
        
        join_network = new SimpleAction ("join-network", null);
        join_network.activate.connect (GlobalEvents.join_network);
        
        create_network = new SimpleAction ("create-network", null);
        create_network.activate.connect (GlobalEvents.create_network);
        
        refresh = new SimpleAction ("refresh", null);
        refresh.activate.connect (() =>
        {
            if (Controller.last_status <= 1)
            {
                Controller.init();
            }
            if (Controller.last_status >= 6)
            {
                Controller.update_connection();
            }
        });
        
        start_search = new SimpleAction ("start-search", null);
        start_search.activate.connect (GlobalEvents.start_search);
        
        stop_search = new SimpleAction ("stop-search", null);
        stop_search.activate.connect (GlobalEvents.stop_search);
        
        copy_ipv4 = new SimpleAction ("copy-ipv4", null);
        copy_ipv4.activate.connect (GlobalEvents.copy_ipv4_to_clipboard);
        
        copy_ipv6 = new SimpleAction ("copy-ipv6", null);
        copy_ipv6.activate.connect (GlobalEvents.copy_ipv6_to_clipboard);
        
        copy_id = new SimpleAction ("copy-id", null);
        copy_id.activate.connect (GlobalEvents.copy_client_id_to_clipboard);
        
        change_nick = new SimpleAction ("change-nick", null);
        change_nick.activate.connect (GlobalEvents.change_nick);
        
        open_config = new SimpleAction ("open-config", null);
        open_config.activate.connect (GlobalEvents.open_config);
        
        save_config = new SimpleAction ("save-config", null);
        save_config.activate.connect (GlobalEvents.save_config);
        
        restore_config = new SimpleAction ("restore-config", null);
        restore_config.activate.connect (GlobalEvents.restore_config);
        
        sort_by = new GLib.Settings (Settings.schema_base_id + "." + Settings.sort_network_list_by.parent).create_action (Settings.sort_network_list_by.key_name) ;
        
        show_offline = new GLib.Settings (Settings.schema_base_id + "." + Settings.show_offline_members.parent).create_action (Settings.show_offline_members.key_name) ;
        
        preferences = new SimpleAction ("preferences", null);
        preferences.activate.connect (GlobalEvents.preferences);
        
        info = new SimpleAction ("info", null);
        info.activate.connect (GlobalEvents.information);
        
        shortcuts = new SimpleAction ("shortcuts", null);
        shortcuts.activate.connect (GlobalEvents.shortcuts);
        
        help = new SimpleAction ("help", null);
        help.activate.connect (GlobalEvents.help);
        
        about = new SimpleAction ("about", null);
        about.activate.connect (GlobalEvents.about);
        
        quit = new SimpleAction ("quit", null);
        quit.activate.connect (GlobalEvents.quit_app);
        
        app.add_action (connect);
        app.add_action (disconnect);
        app.add_action (approve_reject);
        app.add_action (join_network);
        app.add_action (create_network);
        app.add_action (refresh);
        app.add_action (start_search);
        app.add_action (stop_search);
        app.add_action (copy_ipv4);
        app.add_action (copy_ipv6);
        app.add_action (copy_id);
        app.add_action (change_nick);
        app.add_action (open_config);
        app.add_action (save_config);
        app.add_action (restore_config);
        app.add_action (sort_by);
        app.add_action (show_offline);
        app.add_action (preferences);
        app.add_action (shortcuts);
        app.add_action (info);
        app.add_action (help);
        app.add_action (about);
        app.add_action (quit);
        
        app.set_accels_for_action ("app.connect", {"<Control>O"});
        app.set_accels_for_action ("app.disconnect", {"<Control>D"});
        app.set_accels_for_action ("app.join-network", {"<Control>J"});
        app.set_accels_for_action ("app.create-network", {"<Control>N"});
        app.set_accels_for_action ("app.refresh", {"F5", "<Control>R"});
        app.set_accels_for_action ("app.start-search", {"<Control>F"});
        app.set_accels_for_action ("app." + Settings.show_offline_members.key_name, {"<Control>M"});
        app.set_accels_for_action ("app.preferences", {"<Control>comma"});
        app.set_accels_for_action ("app.shortcuts", {"<Control>question"});
        app.set_accels_for_action ("app.info", {"F2"});
        app.set_accels_for_action ("app.help", {"F1"});
        app.set_accels_for_action ("app.quit", {"<Control>Q"});
    }
}
