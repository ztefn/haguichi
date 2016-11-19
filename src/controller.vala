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

public class Controller : Object
{
    public  static bool continue_update;
    public  static bool manual_update;
    public  static bool restore;
    public  static int restore_countdown;
    public  static int last_status;
    public  static int num_update_cycles;
    private static int num_wait_for_internet_cycles;
    private static string start_output;
    private static string old_list;
    
    private static HashTable<string, MemberEvent> members_left_hash;
    private static HashTable<string, MemberEvent> members_online_hash;
    private static HashTable<string, MemberEvent> members_offline_hash;
    private static HashTable<string, MemberEvent> members_joined_hash;
    
    private static List<Network> new_networks_list;
    
    public static void init ()
    {
        HaguichiWindow.message_bar.hide();
        GlobalEvents.set_config();
        
        new_networks_list = new List<Network>();
        
        if (last_status != -3)
        {
            Hamachi.init();
        }
        
        GlobalEvents.update_nick();
        GlobalEvents.set_attach();
        
        HaguichiWindow.sidebar.update();
        
        last_status = -2;
        status_check();
        
        if (last_status >= 6)
        {
            restore = (bool) Settings.reconnect_on_connection_loss.val;
            
            Haguichi.window.set_mode ("Connected");
            
            get_network_list();
        }
        else if (last_status >= 5)
        {
            restore = (bool) Settings.reconnect_on_connection_loss.val;
            
            go_connect();
        }
        else if ((last_status >= 3) &&
                 ((bool) Settings.connect_on_startup.val))
        {
            restore = true;
            
            go_connect();
        }
        else if ((last_status >= 2) &&
                 ((bool) Settings.connect_on_startup.val))
        {
            restore = true;
            
            Haguichi.window.set_mode ("Disconnected");
            wait_for_internet_cycle();
        }
        else if (last_status >= 2)
        {
            Haguichi.window.set_mode ("Disconnected");
        }
        else if (last_status >= 1)
        {
            Haguichi.window.set_mode ("Not configured");
            
            Button configure_button = new Button.with_mnemonic (Text.configure_label);
            configure_button.get_style_context().add_class ("suggested-action");
            configure_button.clicked.connect (() =>
            {
                Hamachi.configure();
                Controller.init();
            });
            
            HaguichiWindow.message_box.set_message (Text.not_configured_heading, Text.not_configured_message);
            HaguichiWindow.message_box.add_button (configure_button);
            
            configure_button.can_default = true;
            configure_button.grab_default();
            configure_button.grab_focus();
        }
        else
        {
            Haguichi.window.set_mode ("Not installed");
            
            Button download_button = new Button.with_mnemonic (Text.download_label);
            download_button.get_style_context().add_class ("suggested-action");
            download_button.clicked.connect (() =>
            {
                Command.open_uri (Text.get_hamachi_url);
            });
            
            if (Hamachi.version != "")
            {
                HaguichiWindow.message_box.set_message (Utils.format (Text.obsolete_heading, Hamachi.version, null, null), Text.obsolete_message);
            }
            else
            {
                HaguichiWindow.message_box.set_message (Text.not_installed_heading, Text.not_installed_message);
            }
            
            HaguichiWindow.message_box.add_button (download_button);
            
            download_button.can_default = true;
            download_button.grab_default();
            download_button.grab_focus();
        }
    }
    
    public static void status_check ()
    {
        last_status = status_int();
    }
    
    private static int status_int ()
    {
        if (Haguichi.demo_mode)
        {
            return 6;
        }
        
        if (last_status > 1)
        {
            if (!has_internet_connection())
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "No internet connection."); // We don't want to call Hamachi if there's no Internet connection...
                return 2;
            }
        }
        
        string output;
        
        if (last_status == -2)
        {
            output = Hamachi.last_info; // Reuse last info requested by Hamachi.init when (re)initializing to increase startup speed
        }
        else
        {
            output = Hamachi.get_info();
        }
        
        try
        {
            if (Hamachi.version == "")
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Not installed.");
                return 0;
            }
            
            if ((Hamachi.version.has_prefix ("0.9.9.")) ||
                (Hamachi.version.has_prefix ("2.0.")) ||
                (Hamachi.version == "2.1.0.17") ||
                (Hamachi.version == "2.1.0.18") ||
                (Hamachi.version == "2.1.0.68") ||
                (Hamachi.version == "2.1.0.76") ||
                (Hamachi.version == "2.1.0.80") ||
                (Hamachi.version == "2.1.0.81"))
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Obsolete version " + Hamachi.version + " installed.");
                return 0;
            }
            
            if ((output.contains ("You do not have permission to control the hamachid daemon.")) ||
                (!FileUtils.test (Hamachi.config_path, GLib.FileTest.EXISTS)))
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Not configured.");
                return 1;
            }
            
            if (last_status <= 1)
            {
                if (!has_internet_connection())
                {
                    Debug.log (Debug.domain.INFO, "Controller.status_check", "No internet connection.");
                    return 2;
                }
            }
            
            if (output.contains ("Hamachi does not seem to be running."))
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Not started.");
                return 3;
            }
            
            if (new Regex ("status([ ]+):([ ]+)offline").match (output))
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Not logged in.");
                return 4;
            }
            
            if (new Regex ("status([ ]+):([ ]+)logging in").match (output))
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Logging in.");
                return 5;
            }
            
            if (new Regex ("status([ ]+):([ ]+)logged in").match (output))
            {
                Debug.log (Debug.domain.INFO, "Controller.status_check", "Logged in.");
                return 6;
            }
        }
        catch (RegexError e)
        {
            Debug.log (Debug.domain.ERROR, "Controller.status_check", e.message);
        }
        
        Debug.log (Debug.domain.INFO, "Controller.status_check", "Unknown.");
        return -1;
    }
    
    public static bool has_internet_connection ()
    {
        bool success = false;
        
        string stdout;
        string stderr;
        int status;
        
        Debug.log (Debug.domain.ENVIRONMENT, "Controller.has_internet_connection", "Trying to ping " + (string) Settings.check_internet_ip.val + "...");
        
        try
        {
            GLib.Process.spawn_command_line_sync ("ping -c1 -W1 " + (string) Settings.check_internet_ip.val, out stdout, out stderr, out status);
            
            success = (status == 0);
            
            Debug.log (success ? Debug.domain.ENVIRONMENT : Debug.domain.ERROR, "Controller.has_internet_connection", (stderr != "") ? stderr : stdout);
        }
        catch (SpawnError e)
        {
            Debug.log (Debug.domain.ERROR, "Controller.has_internet_connection", e.message);
        }
        
        if (!success)
        {
            Debug.log (Debug.domain.ENVIRONMENT, "Controller.has_internet_connection", "Ping failed. Trying to resolve hostname " + (string) Settings.check_internet_hostname.val + " using dig...");
            
            try
            {
                GLib.Process.spawn_command_line_sync ("dig +short +tries=1 +time=1 " + (string) Settings.check_internet_hostname.val, out stdout, out stderr, out status);
                
                success = ((status == 0) && (stdout != ""));
                
                Debug.log (success ? Debug.domain.ENVIRONMENT : Debug.domain.ERROR, "Controller.has_internet_connection", (stderr != "") ? stderr : stdout);
            }
            catch (SpawnError e)
            {
                Debug.log (Debug.domain.ERROR, "Controller.has_internet_connection", e.message);
            }
        }
        
        if (!success && !Command.exists ("dig"))
        {
            Debug.log (Debug.domain.ENVIRONMENT, "Controller.has_internet_connection", "Dig not available. Trying to connect to " + (string) Settings.check_internet_ip.val + " on port 53...");
            
            try
            {
                SocketClient client = new SocketClient();
                client.timeout = 1;
                client.connect_to_host ((string) Settings.check_internet_ip.val, 53);
                
                success = true;
            }
            catch (Error e)
            {
                Debug.log (Debug.domain.ERROR, "Controller.has_internet_connection", e.message);
            }
        }
        
        Debug.log (Debug.domain.ENVIRONMENT, "Controller.has_internet_connection", success ? "Success!" : "No success.");
        
        return success;
    }
    
    public static void* go_start_thread ()
    {
        string output = start_output;
        
        Debug.log (Debug.domain.INFO, "Controller.go_start_thread", "Hamachi should be started now, let's check...");
        
        status_check();
        
        if (last_status >= 4)
        {
            Debug.log (Debug.domain.INFO, "Controller.go_start_thread", "Hamachi is succesfully started, now go login.");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                HaguichiWindow.header_bar.set_subtitle (Text.logging_in);
                return false;
            });
            go_login_thread();
        }
        else if (last_status == 1)
        {
            Debug.log (Debug.domain.INFO, "Controller.go_start_thread", "Hamachi is succesfully started, but not configured.");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.connection_stopped();
                Controller.init();
                return false;
            });
        }
        else if (output != "")
        {
            Debug.log (Debug.domain.INFO, "Controller.go_start_thread", "Failed to start Hamachi, showing output.");
            
            restore = false;
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.connection_stopped();
                new Dialogs.Message (Haguichi.window,
                                     Text.connect_error_heading,
                                     Text.see_output,
                                     MessageType.ERROR,
                                     output);
                return false;
            });
        }
        else
        {
            Debug.log (Debug.domain.INFO, "Controller.go_start_thread", "Failed to start Hamachi, no output to show. User might have cancelled sudo dialog.");
            
            restore = false;
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.connection_stopped();
                HaguichiWindow.message_bar.set_message (Text.connect_error_heading, null, MessageType.ERROR);
                return false;
            });
        }
        
        start_output = null;
        return null;
    }
    
    public static void* go_connect_thread ()
    {
        Idle.add_full (Priority.HIGH_IDLE, () =>
        {
            Haguichi.window.set_mode ("Connecting");
            HaguichiWindow.message_bar.hide();
            return false;
        });
        
        status_check();
        
        if (Haguichi.demo_mode)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.connection_established();
                return false;
            });
        }
        else if (last_status == 2)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.connection_stopped();
                HaguichiWindow.message_bar.set_message (Text.connect_error_no_internet_connection, null, MessageType.WARNING);
                return false;
            });
        }
        else if (last_status >= 4)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                HaguichiWindow.header_bar.set_subtitle (Text.logging_in);
                return false;
            });
            go_login_thread();
        }
        else if (last_status >= 3)
        {
            Debug.log (Debug.domain.INFO, "Controller.go_connect_thread", "Not yet started, go start.");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                go_start();
                return false;
            });
        }
        
        return null;
    }
    
    private static void go_start ()
    {
        start_output = Hamachi.start();
        
        new Thread<void*> (null, go_start_thread);
    }
    
    public static void go_connect ()
    {
        new Thread<void*> (null, go_connect_thread);
    }
    
    private static void* go_login_thread ()
    {
        string output = Hamachi.login();
        
        if ((output.contains (".. ok")) ||
            (output.contains ("Already logged in")) ) // Ok, logged in
        {
            Debug.log (Debug.domain.INFO, "Controller.go_login_thread", "Connected!");
            
            last_status = 6;
            
            Thread.usleep (2000000); // Wait two seconds to get updated info and list
            
            Hamachi.get_info();
             
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                get_network_list();
                return false;
            });
        }
        else
        {
            Debug.log (Debug.domain.INFO, "Controller.go_login_thread", "Error connecting.");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.connection_stopped();
                HaguichiWindow.message_bar.set_message (Text.connect_error_login_failed, null, MessageType.ERROR);
                return false;
            });
        }
        
        return null;
    }
    
    private static void get_network_list ()
    {
        Hamachi.get_list();
        Haguichi.connection.networks = Hamachi.return_networks();
        Haguichi.window.network_view.fill_tree();
        GlobalEvents.connection_established();
    }
    
    public static void update_connection ()
    {
        manual_update = true;
        update_connection_timeout();
    }
    
    private static bool update_connection_timeout ()
    {
        Debug.log (Debug.domain.INFO, "Controller.update_connection_timeout", "Number of active update cycles: " + num_update_cycles.to_string());
        
        if (!manual_update && num_update_cycles > 1)
        {
            num_update_cycles --;
            return false;
        }
        
        if (continue_update)
        {
            Debug.log (Debug.domain.INFO, "Controller.update_connection", "Retrieving connection status...");
            
            HaguichiWindow.header_bar.set_subtitle (Text.updating);
            
            new Thread<void*> (null, update_connection_thread);
        }
        else
        {
            num_update_cycles --;
        }
        
        return false;
    }
    
    private static void* update_connection_thread ()
    {
        status_check();
    
        if (last_status >= 6)
        {
            old_list = Hamachi.last_list;
            Hamachi.get_list();
            
            if (old_list != Hamachi.last_list)
            {
                new_networks_list = Hamachi.return_networks();
            }
        }
        
        if (continue_update)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                update_list();
                GlobalEvents.connection_updated();
                return false;
            });
        }
        else
        {
            num_update_cycles --;
        }
        
        return null;
    }
    
    private static void update_list ()
    {
        if (last_status == 2)
        {
            Debug.log (Debug.domain.INFO, "Controller.update_list", "Internet connection lost.");
            
            GlobalEvents.connection_stopped();
            num_update_cycles --;
        }
        else if (last_status < 6)
        {
            Debug.log (Debug.domain.INFO, "Controller.update_list", "Hamachi connection lost.");
            
            if ((bool) Settings.notify_on_connection_loss.val)
            {
                new Bubble (Text.notify_connection_lost, "");
            }
            
            GlobalEvents.connection_stopped();
            num_update_cycles --;
        }
        else if (last_status >= 6) // We're connected allright
        {
            if (Haguichi.demo_mode)
            {
                Debug.log (Debug.domain.INFO, "Controller.update_list", "Demo mode, not really updating list.");
            }
            else if (old_list == Hamachi.last_list)
            {
                Debug.log (Debug.domain.INFO, "Controller.update_list", "Connected, list not changed. Skipping update.");
            }
            else
            {
                Debug.log (Debug.domain.INFO, "Controller.update_list", "Connected, updating list.");
                
                members_left_hash    = new HashTable<string, MemberEvent>(str_hash, str_equal);
                members_online_hash  = new HashTable<string, MemberEvent>(str_hash, str_equal);
                members_offline_hash = new HashTable<string, MemberEvent>(str_hash, str_equal);
                members_joined_hash  = new HashTable<string, MemberEvent>(str_hash, str_equal);
                
                
                HashTable<string, Network> old_networks_hash = new HashTable<string, Network>(str_hash, str_equal);
                
                foreach (Network old_network in Haguichi.connection.networks)
                {
                    old_networks_hash.insert (old_network.id, old_network);
                }
                
                HashTable<string, Network> new_networks_hash = new HashTable<string, Network>(str_hash, str_equal);
                
                foreach (Network new_network in new_networks_list)
                {
                    new_networks_hash.insert (new_network.id, new_network);
                }
                
                
                foreach (Network old_network in Haguichi.connection.networks)
                {
                    if (!new_networks_hash.contains (old_network.id))
                    {
                        // Network not in new list, removing...
                        
                        Haguichi.window.network_view.remove_network (old_network);
                        Haguichi.connection.remove_network (old_network);
                    }
                }
                
                foreach (Network new_network in new_networks_list)
                {
                    if (old_networks_hash.contains (new_network.id))
                    {
                        // Network in new and old list, updating...
                        
                        Network old_network = (Network) old_networks_hash.get (new_network.id);
                        
                        old_network.update (new_network.status, new_network.id, new_network.name);
                        Haguichi.window.network_view.update_network (old_network);
                        
                        
                        // Check all network members
                        
                        HashTable<string, Member> old_members_hash = new HashTable<string, Member>(str_hash, str_equal);
                        
                        foreach (Member old_member in old_network.members)
                        {
                            old_members_hash.insert (old_member.client_id, old_member);
                        }
                        
                        
                        HashTable<string, Member> new_members_hash = new HashTable<string, Member>(str_hash, str_equal);
                        
                        foreach (Member new_member in new_network.members)
                        {
                            new_members_hash.insert (new_member.client_id, new_member);
                        }
                        
                        
                        foreach (Member old_member in old_network.members)
                        {
                            if (!new_members_hash.contains (old_member.client_id))
                            {
                                // Member not in new list, removing...
                                
                                Haguichi.window.network_view.remove_member (old_network, old_member);
                                old_network.remove_member (old_member);
                                
                                if ((old_member.status.status_int < 3 ) &&
                                    (!old_member.is_evicted))
                                {
                                    add_member_to_hash (members_left_hash, old_member, old_network);
                                }
                            }
                        }
                        
                        foreach (Member new_member in new_network.members)
                        {
                            if (old_members_hash.contains (new_member.client_id))
                            {
                                // Member in old and new list, updating...

                                Member old_member = (Member) old_members_hash [new_member.client_id];
                                
                                if ((old_member.status.status_int == 0) &&
                                     (new_member.status.status_int == 1))
                                {
                                    add_member_to_hash (members_online_hash, old_member, old_network);
                                }
                                if ((old_member.status.status_int == 1) &&
                                    (new_member.status.status_int == 0))
                                {
                                    add_member_to_hash (members_offline_hash, old_member, old_network);
                                }
                                
                                old_member.update (new_member.status, new_member.network_id, new_member.ipv4, new_member.ipv6, new_member.nick, new_member.client_id, new_member.tunnel);
                                Haguichi.window.network_view.update_member_with_network (old_network, old_member);
                            }
                            else
                            {
                                // Member not in old list, adding...
                                
                                old_network.add_member (new_member);
                                Haguichi.window.network_view.add_member (old_network, new_member);
                                
                                add_member_to_hash (members_joined_hash, new_member, old_network);
                            }
                        }
                    }
                    else
                    {
                        // Network not in old list, adding...
                        
                        Haguichi.connection.add_network (new_network);
                        Haguichi.window.network_view.add_network (new_network);
                    }
                }
                
                Haguichi.window.network_view.collapse_or_expand_networks();
                HaguichiWindow.sidebar.refresh_tab();
                
                notify_members_joined();
                notify_members_left();
                notify_members_online();
                notify_members_offline();
            }
            
            if (manual_update)
            {
                manual_update = false;
                num_update_cycles ++;
            }
            
            update_cycle(); // Continue update interval
            
            HaguichiWindow.header_bar.set_subtitle (Text.connected);
        }
    }
    
    private static void add_member_to_hash (HashTable<string, MemberEvent> hash, Member member, Network network)
    {
        MemberEvent member_event = new MemberEvent(member.nick);
        
        if (hash.contains (member.client_id))
        {
            member_event = (MemberEvent) hash.get (member.client_id);
        }
        member_event.add_network (network.name);
        
        hash.replace (member.client_id, member_event);
    }
    
    public static void wait_for_internet_cycle ()
    {
        num_wait_for_internet_cycles ++;

        new Thread<void*> (null, wait_for_internet_cycle_thread);
    }
    
    private static void* wait_for_internet_cycle_thread ()
    {
        Thread.usleep (2000000);
        
        if (num_wait_for_internet_cycles > 1)
        {
            // Do nothing
        }
        else if (!restore)
        {
            // Do nothing
        }
        else if (has_internet_connection())
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                GlobalEvents.start_hamachi();
                return false;
            });
        }
        else
        {
            Debug.log (Debug.domain.INFO, "Controller.wait_for_internet_cycle", "Waiting for internet connection...");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                wait_for_internet_cycle();
                return false;
            });
        }
        
        num_wait_for_internet_cycles --;
        return null;
    }
    
    public static void restore_connection_cycle ()
    {
        Debug.log (Debug.domain.INFO, "Controller.restore_connection_cycle", "Trying to reconnect...");
        
        restore_countdown = (int) Settings.reconnect_interval.val;
        Haguichi.window.set_mode ("Countdown");
        
        GLib.Timeout.add (1000, restore_connection);
    }
    
    private static bool restore_connection ()
    {
        if (restore && restore_countdown > 0)
        {
            restore_countdown --;
            
            if (restore_countdown == 0)
            {
                GlobalEvents.start_hamachi();
            }
            else
            {
                Haguichi.window.set_mode ("Countdown");
                return true;   
            }
        }
        
        return false;
    }
    
    public static bool update_cycle ()
    {
        continue_update = true;
        
        uint interval = (uint) (1000 * (int) Settings.update_interval.val);
        
        if (((bool) Settings.update_network_list.val) &&
            (interval > 0))
        {
            GLib.Timeout.add (interval, update_connection_timeout);
        }
        else
        {
            GLib.Timeout.add (1000, update_cycle);
        }
        
        return false;
    }
    
    public static void notify_members_joined ()
    {
        members_joined_hash.foreach ((member_id, member_event) =>
        {
            notify_member_joined (member_event.nick, member_event.first_network, (member_event.networks_length - 1));
        });
    }
    
    public static void notify_member_joined (string nick, string network, int more)
    {
        if ((bool) Settings.notify_on_member_join.val)
        {
            string message = Text.notify_member_joined_message;
            if (more > 0)
            {
                message = Text.notify_member_joined_message_plural (more);
            }
            
            string body = Utils.format (message, nick, network, more.to_string());
            new Bubble (Text.notify_member_joined_heading, body);
        }
    }
    
    public static void notify_members_left ()
    {
        members_left_hash.foreach ((member_id, member_event) =>
        {
            notify_member_left (member_event.nick, member_event.first_network, (member_event.networks_length - 1));
        });
    }
    
    public static void notify_member_left (string nick, string network, int more)
    {
        if ((bool) Settings.notify_on_member_leave.val)
        {
            string message = Text.notify_member_left_message;
            if (more > 0)
            {
                message = Text.notify_member_left_message_plural (more);
            }
            
            string body = Utils.format (message, nick, network, more.to_string());
            new Bubble (Text.notify_member_left_heading, body);
        }
    }
    
    public static void notify_members_online ()
    {
        members_online_hash.foreach ((member_id, member_event) =>
        {
            notify_member_online (member_event.nick, member_event.first_network, (member_event.networks_length - 1));
        });
    }
    
    public static void notify_member_online (string nick, string network, int more)
    {
        if ((bool) Settings.notify_on_member_online.val)
        {
            string message = Text.notify_member_online_message;
            if (more > 0)
            {
                message = Text.notify_member_online_message_plural (more);
            }
            
            string body = Utils.format (message, nick, network, more.to_string());
            new Bubble (Text.notify_member_online_heading, body);
        }
    }
    
    public static void notify_members_offline ()
    {
        members_offline_hash.foreach ((member_id, member_event) =>
        {
            notify_member_offline (member_event.nick, member_event.first_network, (member_event.networks_length - 1));
        });
    }
    
    public static void notify_member_offline (string nick, string network, int more)
    {
        if ((bool) Settings.notify_on_member_offline.val)
        {
            string message = Text.notify_member_offline_message;
            if (more > 0)
            {
                message = Text.notify_member_offline_message_plural (more);
            }
            
            string body = Utils.format (message, nick, network, more.to_string());
            new Bubble (Text.notify_member_offline_heading, body);
        }
    }
}
