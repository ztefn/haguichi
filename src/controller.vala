/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2025 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

namespace Haguichi {
    public class Controller : Object {
        public  static bool   continue_update;
        public  static bool   manual_update;
        public  static bool   restore;
        public  static int    restore_countdown;
        public  static int    last_status;
        public  static int    num_update_cycles;
        private static int    num_wait_for_internet_cycles;
        private static string start_output;
        private static string old_list;

        private static Settings behavior;
        private static Settings config;
        private static Settings notifications;

        private static HashTable<string, MemberEvent> members_left_hash;
        private static HashTable<string, MemberEvent> members_online_hash;
        private static HashTable<string, MemberEvent> members_offline_hash;
        private static HashTable<string, MemberEvent> members_joined_hash;

        private static List<Network> new_networks_list;

        public static void init () {
            behavior      = new Settings (Config.APP_ID + ".behavior");
            config        = new Settings (Config.APP_ID + ".config");
            notifications = new Settings (Config.APP_ID + ".notifications");

            new_networks_list = new List<Network> ();

            new Thread<void*> (null, init_thread);
        }

        private static void* init_thread () {
            Command.init ();
            Hamachi.init ();

            Idle.add_full (Priority.HIGH_IDLE, () => {
                win.sidebar.update ();
                return false;
            });

            last_status = -2;
            status_check ();

            Idle.add_full (Priority.HIGH_IDLE, () => {
                if (last_status >= 6) {
                    restore = behavior.get_boolean ("reconnect-on-connection-loss");
                    win.set_mode ("Connected");
                    get_network_list ();
                } else if (last_status >= 5) {
                    restore = behavior.get_boolean ("reconnect-on-connection-loss");
                    go_connect ();
                } else if (last_status >= 3 && behavior.get_boolean ("connect-on-startup")) {
                    restore = true;
                    go_connect ();
                } else if (last_status >= 2 && behavior.get_boolean ("connect-on-startup")) {
                    restore = true;
                    win.set_mode ("Disconnected");
                    wait_for_internet_cycle ();
                } else if (last_status >= 2) {
                    win.set_mode ("Disconnected");
                } else if (last_status >= 1) {
                    win.set_mode ("Not configured");
                } else {
                    win.set_mode ("Not installed");
                }

                return false;
            });

            return null;
        }

        public static void status_check () {
            last_status = status_int ();
        }

        private static int status_int () {
            if (demo_mode) {
                return 6;
            }

            if (last_status > 1) {
                if (!has_internet_connection ()) {
                    // We don't want to call Hamachi if there's no Internet connection...
                    debug ("status_check: No internet connection");
                    return 2;
                }
            }

            string output;

            if (last_status == -2) {
                // Reuse last info requested by Hamachi.init when (re)initializing to increase startup speed
                output = Hamachi.last_info;
            } else {
                output = Hamachi.get_info ();
            }

            try {
                if (Hamachi.version == "") {
                    debug ("status_check: Not installed");
                    return 0;
                }

                if (Hamachi.version.has_prefix ("0.9.9.") ||
                    Hamachi.version.has_prefix ("2.0.") ||
                    Hamachi.version == "2.1.0.17" ||
                    Hamachi.version == "2.1.0.18" ||
                    Hamachi.version == "2.1.0.68" ||
                    Hamachi.version == "2.1.0.76" ||
                    Hamachi.version == "2.1.0.80" ||
                    Hamachi.version == "2.1.0.81") {
                    debug ("status_check: Obsolete version %s installed", Hamachi.version);
                    return 0;
                }

                if (output.contains ("You do not have permission to control the hamachid daemon.") || !Utils.path_exists ("f", Hamachi.CONFIG_PATH)) {
                    debug ("status_check: Not configured");
                    return 1;
                }

                if (last_status <= 1) {
                    if (!has_internet_connection ()) {
                        debug ("status_check: No internet connection");
                        return 2;
                    }
                }

                if (output.contains ("Hamachi does not seem to be running.")) {
                    debug ("status_check: Not started");
                    return 3;
                }

                if (new Regex ("status([ ]+):([ ]+)offline").match (output)) {
                    debug ("status_check: Not logged in");
                    return 4;
                }

                if (new Regex ("status([ ]+):([ ]+)logging in").match (output)) {
                    debug ("status_check: Logging in");
                    return 5;
                }

                if (new Regex ("status([ ]+):([ ]+)logged in").match (output)) {
                    debug ("status_check: Logged in");
                    return 6;
                }
            } catch (RegexError e) {
                critical ("status_check: %s", e.message);
            }

            debug ("status_check: Unknown");
            return -1;
        }

        public static bool has_internet_connection () {
            bool success = false;

            string stdout;
            string stderr;
            int    status;

            debug ("has_internet_connection: Trying to ping %s...", config.get_string ("check-internet-ip"));

            try {
                Process.spawn_command_line_sync (Command.spawn_wrap + "ping -c1 -W1 " + config.get_string ("check-internet-ip"), out stdout, out stderr, out status);

                success = (status == 0);

                string output = (stderr != "") ? stderr : stdout;
                if (!success) {
                    warning ("has_internet_connection: %s", output);
                } else {
                    debug ("has_internet_connection: %s", output);
                }
            } catch (SpawnError e) {
                critical ("has_internet_connection: %s", e.message);
            }

            if (!success) {
                debug ("has_internet_connection: Ping failed. Trying to resolve hostname %s using dig...", config.get_string ("check-internet-hostname"));

                try {
                    Process.spawn_command_line_sync (Command.spawn_wrap + "dig +short +tries=1 +time=1 " + config.get_string ("check-internet-hostname"), out stdout, out stderr, out status);

                    success = (status == 0 && stdout != "");

                    string output = (stderr != "") ? stderr : stdout;
                    if (!success) {
                        warning ("has_internet_connection: %s", output);
                    } else {
                        debug ("has_internet_connection: %s", output);
                    }
                } catch (SpawnError e) {
                    critical ("has_internet_connection: %s", e.message);
                }
            }

            if (!success && !Command.exists ("dig")) {
                debug ("has_internet_connection: Dig not available - trying to connect to %s on port 53... ", config.get_string ("check-internet-ip"));

                try {
                    SocketClient client = new SocketClient ();
                    client.timeout = 1;
                    client.connect_to_host (config.get_string ("check-internet-ip"), 53);

                    success = true;
                } catch (Error e) {
                    critical ("has_internet_connection: %s", e.message);
                }
            }

            debug ("has_internet_connection: %s", success ? "Success!" : "No success");
            return success;
        }

        public static void start_hamachi () {
            restore_countdown = 0;
            go_connect ();
        }

        public static void restart_hamachi () {
            stop_hamachi ();
            win.set_mode ("Connecting");

            new Thread<void*> (null, () => {
                // Wait a second before starting hamachi again
                Thread.usleep (1000000);

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    start_hamachi ();
                    return false;
                });

                return null;
            });
        }

        public static void stop_hamachi () {
            new Thread<void*> (null, () => {
                if (!demo_mode) {
                    Hamachi.logout ();
                }

                return null;
            });

            restore = false;
            connection_stopped ();
        }

        public static void go_connect () {
            new Thread<void*> (null, () => {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    win.set_mode ("Connecting");
                    return false;
                });

                status_check ();

                if (demo_mode) {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        connection_established ();
                        return false;
                    });
                } else if (last_status == 2) {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        connection_stopped ();
                        win.show_toast (_("No internet connection"));
                        return false;
                    });
                } else if (last_status >= 4) {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.window_title.subtitle = _("Logging in");
                        return false;
                    });
                    go_login_thread ();
                } else if (last_status >= 3) {
                    debug ("go_connect: Not yet started, go start");

                    // Wait half a second to finish updating GUI before showing blocking sudo dialog
                    Thread.usleep (500000);

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        go_start ();
                        return false;
                    });
                }

                return null;
            });
        }

        private static void go_start () {
            start_output = Hamachi.start ();

            new Thread<void*> (null, () => {
                string output = start_output;

                debug ("go_start: Hamachi should be started now, let's check...");
                status_check ();

                if (last_status >= 4) {
                    debug ("go_start: Hamachi is succesfully started, now go login");

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.window_title.subtitle = _("Logging in");
                        return false;
                    });
                    go_login_thread ();
                } else if (last_status == 1) {
                    debug ("go_start: Hamachi is succesfully started, but not configured");

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        connection_stopped ();
                        init ();
                        return false;
                    });
                } else if (output != "" && !output.contains ("Request dismissed")) {
                    debug ("go_start: Failed to start Hamachi, showing output");
                    restore = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        connection_stopped ();
                        win.show_toast (output.strip ());
                        return false;
                    });
                } else {
                    debug ("go_start: Failed to start Hamachi, no output to show - user might have cancelled sudo dialog");
                    restore = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        connection_stopped ();
                        win.show_toast (_("Error connecting"));
                        return false;
                    });
                }

                start_output = null;
                return null;
            });
        }

        private static void* go_login_thread () {
            string output = Hamachi.login ();

            if (output.contains (".. ok") || output.contains ("Already logged in")) { // Ok, logged in
                debug ("go_login_thread: Connected!");
                last_status = 6;

                Thread.usleep (2000000); // Wait two seconds to get updated info and list

                Hamachi.get_info ();

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    get_network_list ();
                    return false;
                });
            } else {
                debug ("go_login_thread: Error connecting");

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    connection_stopped ();
                    win.show_toast (_("Hamachi login failed"));
                    return false;
                });
            }

            return null;
        }

        public static void connection_established () {
            win.set_mode ("Connected");
            win.sidebar.update ();

            // Update protocol if needed
            var protocol = config.get_string ("protocol");
            if (Hamachi.ip_version.down () != protocol) {
                new Thread<void*> (null, () => {
                    Hamachi.set_protocol (protocol);
                    return null;
                });
            }

            // Set nick after login
            new Thread<void*> (null, () => {
                Thread.usleep (2000000);
                Hamachi.set_nick (Utils.parse_nick (config.get_string ("nickname")));
                return null;
            });

            restore = behavior.get_boolean ("reconnect-on-connection-loss");
            num_update_cycles ++;
            update_cycle ();
        }

        public static void connection_stopped () {
            if (restore) {
                win.network_list.save_state ();
            }
            win.set_disconnected_stack_page ("empty");
            win.set_mode ("Disconnected");

            // Stop update interval
            continue_update = false;

            if (restore) {
                if (last_status == 2) {
                    wait_for_internet_cycle ();
                } else {
                    restore_connection_cycle ();
                }
            }

            if (!demo_mode) {
                connection.clear_networks ();
            }

            last_status = 4;
        }

        private static void get_network_list () {
            Hamachi.get_list ();
            connection.networks = Hamachi.return_networks ();
            win.network_list.fill_tree ();
            connection_established ();
        }

        public static void update_connection () {
            manual_update = true;
            update_connection_timeout ();
        }

        private static bool update_connection_timeout () {
            debug ("update_connection_timeout: Number of active update cycles: %s", num_update_cycles.to_string ());

            if (!manual_update && num_update_cycles > 1) {
                num_update_cycles --;
                return false;
            }

            if (continue_update) {
                debug ("update_connection: Retrieving connection status...");

                win.window_title.subtitle = _("Updating");

                new Thread<void*> (null, update_connection_thread);
            } else {
                num_update_cycles --;
            }

            return false;
        }

        private static void* update_connection_thread () {
            status_check ();

            if (last_status >= 6) {
                old_list = Hamachi.last_list;
                Hamachi.get_list ();

                if (old_list != Hamachi.last_list) {
                    new_networks_list = Hamachi.return_networks ();
                }
            }

            if (continue_update) {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    update_list ();
                    win.sidebar.update ();
                    return false;
                });
            } else {
                num_update_cycles --;
            }

            return null;
        }

        private static void update_list () {
            if (last_status == 2) {
                debug ("update_list: Internet connection lost");
                connection_stopped ();
                num_update_cycles --;
            } else if (last_status < 6) {
                debug ("update_list: Hamachi connection lost");
                // Display connection lost notification only if window is not currently focused
                if (!(win.visible && win.is_active)) {
                    notify_connection_lost ();
                }
                connection_stopped ();
                num_update_cycles --;
            } else if (last_status >= 6) {
                // We are still connected
                if (demo_mode) {
                    debug ("update_list: Demo mode, not really updating list");
                    win.network_list.sort ();
                    win.network_list.refilter ();
                } else if (old_list == Hamachi.last_list) {
                    debug ("update_list: Connected, list not changed");
                } else {
                    debug ("update_list: Connected, updating list");

                    members_left_hash    = new HashTable<string, MemberEvent>(str_hash, str_equal);
                    members_online_hash  = new HashTable<string, MemberEvent>(str_hash, str_equal);
                    members_offline_hash = new HashTable<string, MemberEvent>(str_hash, str_equal);
                    members_joined_hash  = new HashTable<string, MemberEvent>(str_hash, str_equal);

                    HashTable<string, Network> old_networks_hash = new HashTable<string, Network>(str_hash, str_equal);
                    foreach (Network old_network in connection.networks) {
                        old_networks_hash.insert (old_network.id, old_network);
                    }

                    HashTable<string, Network> new_networks_hash = new HashTable<string, Network>(str_hash, str_equal);
                    foreach (Network new_network in new_networks_list) {
                        new_networks_hash.insert (new_network.id, new_network);
                    }

                    old_networks_hash.foreach ((old_network_id, old_network) => {
                        if (!new_networks_hash.contains (old_network.id)) {
                            // Network not in new list, removing...
                            connection.remove_network (old_network);
                        }
                    });

                    new_networks_hash.foreach ((new_network_id, new_network) => {
                        if (old_networks_hash.contains (new_network.id)) {
                            // Network in new and old list, updating...
                            Network old_network = (Network) old_networks_hash.get (new_network.id);
                            old_network.update (new_network.status, new_network.name);

                            // Check all network members
                            HashTable<string, Member> old_members_hash = new HashTable<string, Member>(str_hash, str_equal);

                            foreach (Member old_member in old_network.members) {
                                old_members_hash.insert (old_member.id, old_member);
                            }

                            HashTable<string, Member> new_members_hash = new HashTable<string, Member>(str_hash, str_equal);

                            foreach (Member new_member in new_network.members) {
                                new_members_hash.insert (new_member.id, new_member);
                            }

                            old_members_hash.foreach ((old_member_id, old_member) => {
                                if (!new_members_hash.contains (old_member.id)) {
                                    // Member not in new list, removing...
                                    old_network.remove_member (old_member);

                                    if ((old_member.status.status_int < 3 ) &&
                                        (!old_member.is_evicted)) {
                                        add_member_to_hash (members_left_hash, old_member, old_network);
                                    }
                                }
                            });

                            new_members_hash.foreach ((new_member_id, new_member) => {
                                if (old_members_hash.contains (new_member.id)) {
                                    // Member in old and new list, updating...
                                    Member old_member = (Member) old_members_hash [new_member.id];

                                    if ((old_member.status.status_int == 0) &&
                                        (new_member.status.status_int == 1)) {
                                        add_member_to_hash (members_online_hash, old_member, old_network);
                                    }

                                    if ((old_member.status.status_int == 1) &&
                                        (new_member.status.status_int == 0)) {
                                        add_member_to_hash (members_offline_hash, old_member, old_network);
                                    }

                                    old_member.update (new_member.status, new_member.ipv4, new_member.ipv6, new_member.nick, new_member.tunnel);
                                } else {
                                    // Member not in old list, adding...
                                    old_network.add_member (new_member);
                                    new_member.network = old_network;
                                    new_member.init ();

                                    add_member_to_hash (members_joined_hash, new_member, old_network);
                                }
                            });

                            old_network.set_label_markup ();
                        } else {
                            // Network not in old list, adding...
                            connection.add_network (new_network);
                        }
                    });

                    win.network_list.sort ();
                    win.network_list.refilter ();

                    notify_members_joined ();
                    notify_members_left ();
                    notify_members_online ();
                    notify_members_offline ();
                }

                if (manual_update) {
                    manual_update = false;
                    num_update_cycles ++;
                }

                // Continue update interval

                update_cycle ();

                win.window_title.subtitle = _("Connected");
            }
        }

        private static void add_member_to_hash (HashTable<string, MemberEvent> hash, Member member, Network network) {
            MemberEvent member_event = new MemberEvent (member.id, member.known_name);

            if (hash.contains (member.id)) {
                member_event = (MemberEvent) hash.get (member.id);
            }

            member_event.add_network (network.name);

            if (member.status.status_int == 3) {
                member_event.add_network_approval (network.id);
            }

            hash.replace (member.id, member_event);
        }

        public static void wait_for_internet_cycle () {
            num_wait_for_internet_cycles ++;

            new Thread<void*> (null, () => {
                Thread.usleep (2000000);

                if (num_wait_for_internet_cycles > 1) {
                    // Do nothing
                } else if (!restore) {
                    // Do nothing
                } else if (has_internet_connection ()) {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        start_hamachi ();
                        return false;
                    });
                } else {
                    debug ("wait_for_internet_cycle: Waiting for internet connection...");

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        wait_for_internet_cycle ();
                        return false;
                    });
                }

                num_wait_for_internet_cycles --;

                return null;
            });
        }

        public static void restore_connection_cycle () {
            debug ("restore_connection_cycle: Trying to reconnect...");

            restore_countdown = config.get_int ("reconnect-interval");
            win.set_restore_countdown (restore_countdown);
            win.set_disconnected_stack_page ("restore");
            Timeout.add (1000, restore_connection);
        }

        private static bool restore_connection () {
            if (restore && restore_countdown > 0) {
                restore_countdown --;

                if (restore_countdown == 0) {
                    start_hamachi ();
                } else {
                    win.set_restore_countdown (restore_countdown);
                    return true;
                }
            }

            return false;
        }

        public static bool update_cycle () {
            continue_update = true;

            uint interval = (uint) (1000 * config.get_int ("update-interval"));
            if (interval > 0) {
                Timeout.add (interval, update_connection_timeout);
            } else {
                Timeout.add (1000, update_cycle);
            }

            return false;
        }

        public static void notify_connection_lost () {
            if (notifications.get_boolean ("connection-loss")) {
                Bubble bubble = new Bubble (_("Hamachi Lost Connection"), null);
                bubble.add_reconnect_action ();
                bubble.show ();
            }
        }

        public static void notify_members_joined () {
            members_joined_hash.foreach ((member_id, member_event) => {
                notify_member_joined (member_event.name,
                                      member_event.first_network_name,
                                      member_event.networks_length - 1,
                                      member_event.id,
                                      member_event.get_network_approval_ids ());
            });
        }

        public static void notify_member_joined (string nick, string network, int more, string client_id, string[] network_approval_ids) {
            if (notifications.get_boolean ("member-join")) {
                var bubble = compose_bubble (
                    _("Member Joined"),
                    // Notification bubble. For example: "T-800 joined the network Skynet".
                    _("{0} joined the network {1}"),
                    // Notification bubble. For example: "T-800 joined the network Skynet and 1 other network".
                    ngettext ("{0} joined the network {1} and {2} other network",
                              "{0} joined the network {1} and {2} other networks",
                              more),
                    nick,
                    network,
                    more);

                if (network_approval_ids.length > 0) {
                    bubble.add_approve_reject_actions (client_id, network_approval_ids);
                }

                bubble.show ();
            }
        }

        public static void notify_members_left () {
            members_left_hash.foreach ((member_id, member_event) => {
                notify_member_left (member_event.name,
                                    member_event.first_network_name,
                                    member_event.networks_length - 1);
            });
        }

        public static void notify_member_left (string nick, string network, int more) {
            if (notifications.get_boolean ("member-leave")) {
                var bubble = compose_bubble (
                    _("Member Left"),
                    // Notification bubble. For example: "T-800 left the network Skynet".
                    _("{0} left the network {1}"),
                    // Notification bubble. For example: "T-800 left the network Skynet and 1 other network".
                    ngettext ("{0} left the network {1} and {2} other network",
                              "{0} left the network {1} and {2} other networks",
                              more),
                    nick,
                    network,
                    more);

                bubble.show ();
            }
        }

        public static void notify_members_online () {
            members_online_hash.foreach ((member_id, member_event) => {
                notify_member_online (member_event.name,
                                      member_event.first_network_name,
                                      member_event.networks_length - 1);
            });
        }

        public static void notify_member_online (string nick, string network, int more) {
            if (notifications.get_boolean ("member-online")) {
                var bubble = compose_bubble (
                    _("Member Online"),
                    // Notification bubble. For example: "T-800 came online in the network Skynet".
                    _("{0} came online in the network {1}"),
                    // Notification bubble. For example: "T-800 came online in the network Skynet and 1 other network".
                    ngettext ("{0} came online in the network {1} and {2} other network",
                              "{0} came online in the network {1} and {2} other networks",
                              more),
                    nick,
                    network,
                    more);

                bubble.show ();
            }
        }

        public static void notify_members_offline () {
            members_offline_hash.foreach ((member_id, member_event) => {
                notify_member_offline (member_event.name,
                                       member_event.first_network_name,
                                       member_event.networks_length - 1);
            });
        }

        public static void notify_member_offline (string nick, string network, int more) {
            if (notifications.get_boolean ("member-offline")) {
                var bubble = compose_bubble (
                    _("Member Offline"),
                    // Notification bubble. For example: "T-800 went offline in the network Skynet".
                    _("{0} went offline in the network {1}"),
                    // Notification bubble. For example: "T-800 went offline in the network Skynet and 1 other network".
                    ngettext ("{0} went offline in the network {1} and {2} other network",
                              "{0} went offline in the network {1} and {2} other networks",
                              more),
                    nick,
                    network,
                    more);

                bubble.show ();
            }
        }

        public static Bubble compose_bubble (
            string heading,
            string message_one,
            string message_more,
            string nick,
            string network,
            int more) {

            var message = more > 0 ? message_more : message_one;
                message = message.replace ("{0}", nick);
                message = message.replace ("{1}", network);
                message = message.replace ("{2}", more.to_string ());

            return new Bubble (heading, message);
        }

        public static void quit () {
            if (last_status > 4 && behavior.get_boolean ("disconnect-on-quit")) {
                stop_hamachi ();
            }
        }
    }
}
