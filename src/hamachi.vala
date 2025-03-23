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
    public class Hamachi : Object {
        public  const  string DATA_PATH   = "/var/lib/logmein-hamachi";
        public  const  string CONFIG_PATH = DATA_PATH + "/h2-engine-override.cfg";

        public  static string version;
        public  static string ip_version;
        public  static string demo_account;
        public  static string last_info;
        public  static string last_list;

        private static string service;

        public static void init () {
            ip_version   = "";
            demo_account = "-";

            get_info ();
            determine_version ();
            determine_service ();
        }

        public static void determine_version () {
            version = "";

            if (demo_mode) {
                version = "2.1.0.203";
                return;
            }

            if (!Command.exists ("hamachi")) {
                return;
            }

            version = retrieve (last_info, "version");

            if (version.has_prefix ("hamachi-lnx-")) {
                version = version.replace ("hamachi-lnx-", "");
            }

            if (version != "") {
                debug ("determine_version: Hamachi %s detected", version);
                return;
            }

            string output = Command.return_output ("hamachi -h");

            if (output == "") {
                output = Command.return_output ("stdbuf -o0 hamachi -h"); // Adjust stdout stream buffering
            }

            if (output == "") {
                debug ("determine_version: No output");
                return;
            }

            debug ("determine_version: %s", output);

            if (output.contains ("Hamachi, a zero-config virtual private networking utility, ver ")) {
                try {
                    MatchInfo mi;
                    new Regex ("Hamachi, a zero-config virtual private networking utility, ver (.+)").match (output, 0, out mi);

                    version = mi.fetch (1);
                } catch (RegexError e) {
                    critical ("determine_version: %s", e.message);
                }
            }

            if (version != "") {
                debug ("determine_version: Hamachi %s detected", version);
                return;
            }

            debug ("determine_version: Unknown version");
        }

        public static void determine_service () {
            var settings = new Settings (Config.APP_ID + ".commands");
            string init_system = settings.get_string ("init-system");

            if (init_system == "systemctl" ||
                (init_system == "auto" &&
                 Utils.path_exists ("d", "/run/systemd/system") &&
                 Command.exists ("systemctl") &&
                 systemctl_unit_found ())) {
                // Systemd
                service = "systemctl %s logmein-hamachi.service";
            }
            else if (init_system == "init.d" ||
                     (init_system == "auto" &&
                      Utils.path_exists ("f", "/etc/init.d/logmein-hamachi"))) {
                // Sysvinit
                service = "/etc/init.d/logmein-hamachi %s";
            }
            else if (init_system == "rc.d" ||
                     (init_system == "auto" &&
                      Utils.path_exists ("f", "/etc/rc.d/logmein-hamachi"))) {
                // BSD style init
                service = "/etc/rc.d/logmein-hamachi %s";
            }

            debug ("determine_service %s: %s", init_system, service);
        }

        public static bool systemctl_unit_found () {
            bool found = true;

            string output = Command.return_output ("systemctl status logmein-hamachi.service");
            debug ("systemctl_unit_found output: %s", output);

            if (output.contains ("Loaded: not-found") ||
                output.contains ("Unit logmein-hamachi.service could not be found.")) {
                found = false;
            }

            debug ("systemctl_unit_found result: %s", found.to_string ());
            return found;
        }

        public static string retrieve (string? output, string nfo) {
            if (output == null) {
                return "";
            }

            string retrieved = null;

            try {
                MatchInfo mi;
                new Regex (nfo + "[ ]*:[ ]+(.+)").match (output, 0, out mi);

                retrieved = mi.fetch (1);
            } catch (RegexError e) {
                critical ("retrieve: %s", e.message);
            }

            if (retrieved == null) {
                return "";
            }

            return retrieved;
        }

        public static void configure () {
            new Thread<void*> (null, () => {
                string output = Command.return_output (Command.sudo + " " + Command.sudo_start + "bash -c \"" +
                                                       service.printf ("start") + "; " +
                                                       service.printf ("stop") + "; " +
                                                       "killall -9 hamachid &> /dev/null; " +
                                                       "echo \'Ipc.User      " + Environment.get_user_name () +
                                                       "\' >> " + CONFIG_PATH + "; " +
                                                       service.printf ("start") + "\"");
                debug ("configure: %s", output);

                // Wait a second to let hamachi settle
                Thread.usleep (1000000);

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    Controller.init ();
                    return false;
                });

                return null;
            });
        }

        public static string start () {
            string output = Command.return_output (Command.sudo + " " + Command.sudo_start + service.printf ("restart"));
            debug ("start: %s", output);

            // Wait a second to let hamachi settle
            Thread.usleep (1000000);

            return output;
        }

        public static string login () {
            string output = Command.return_output ("hamachi login");
            debug ("login: %s", output);

            return output;
        }

        public static string logout () {
            string output = Command.return_output ("hamachi logout");

            debug ("logout: %s", output);
            return output;
        }

        public static string get_account () {
            if (demo_mode) {
                return demo_account;
            }

            string output = retrieve (last_info, "lmi account");

            debug ("get_account: %s", output);
            return output;
        }

        public static string get_client_id () {
            if (demo_mode) {
                return "090-123-456";
            }

            string output = retrieve (last_info, "client id");

            debug ("get_client_id: %s", output);
            return output;
        }

        public static string[] get_address () {
            if (demo_mode) {
                ip_version = "Both";
                return new string[] {"25.123.456.78", "2620:9b::56d:f78e"};
            }

            string ipv4 = null;
            string ipv6 = null;

            string ouput = retrieve (last_info, "address");

            try {
                MatchInfo mi;
                new Regex ("""^(?<ipv4>[0-9\.]{7,15})?[ ]*(?<ipv6>[0-9a-f\:]{6,39})?$""").match (ouput, 0, out mi);

                ipv4 = mi.fetch_named ("ipv4");
                ipv6 = mi.fetch_named ("ipv6");

                debug ("get_address ipv4: %s", ipv4);
                debug ("get_address ipv6: %s", ipv6);
            } catch (RegexError e) {
                critical ("get_address: %s", e.message);
            }

            if (ipv4 == "") {
                ipv4 = null;
            }

            if (ipv6 == "") {
                ipv6 = null;
            }

            if ((ipv4 != null) &&
                (ipv6 != null)) {
                ip_version = "Both";
            } else if (ipv4 != null) {
                ip_version = "IPv4";
            } else if (ipv6 != null) {
                ip_version = "IPv6";
            }

            debug ("get_address: IP version: %s", ip_version);

            return new string[] {ipv4, ipv6};
        }

        public static string get_info () {
            if (!demo_mode && Command.exists ("hamachi")) {
                last_info = Command.return_output ("hamachi");
                debug ("get_info: %s", last_info);
            }

            return last_info;
        }

        public static bool go_online (Network network) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi go-online \"%s\"".printf (Utils.clean_string (network.id)));
                debug ("go_online: %s", output);

                if (!output.contains (".. ok")) {
                    success = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.show_toast (output.strip ());
                        return false;
                    });
                }
            }

            return success;
        }

        public static bool go_offline (Network network) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi go-offline \"%s\"".printf (Utils.clean_string (network.id)));
                debug ("go_offline: %s", output);

                if (!output.contains (".. ok")) {
                    success = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.show_toast (output.strip ());
                        return false;
                    });
                }
            }

            return success;
        }

        public static bool delete (Network network) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi delete \"%s\"".printf (Utils.clean_string (network.id)));
                debug ("delete: %s", output);

                if (!output.contains (".. ok")) {
                    success = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.show_toast (output.strip ());
                        return false;
                    });
                }
            }

            return success;
        }

        public static bool leave (Network network) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi leave \"%s\"".printf (Utils.clean_string (network.id)));
                debug ("leave: %s", output);

                if (!output.contains (".. ok")) {
                    success = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.show_toast (output.strip ());
                        return false;
                    });
                }
            }

            return success;
        }

        public static bool approve (Member member) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi approve \"%s\" %s".printf (Utils.clean_string (member.network.id), member.id));
                debug ("approve: %s", output);

                if (output.contains (".. failed")) {
                    success = false;
                }
            }

            return success;
        }

        public static bool reject (Member member) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi reject \"%s\" %s".printf (Utils.clean_string (member.network.id), member.id));
                debug ("reject: %s", output);

                if (output.contains (".. failed")) {
                    success = false;
                }
            }

            return success;
        }

        public static bool evict (Member member) {
            bool success = true;

            if (!demo_mode) {
                string output = Command.return_output ("hamachi evict \"%s\" %s".printf (Utils.clean_string (member.network.id), member.id));
                debug ("evict: %s", output);

                if (!output.contains (".. ok")) {
                    success = false;

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.show_toast (output.strip ());
                        return false;
                    });
                }
            }

            return success;
        }

        public static string random_address () {
            string address  = "25.";
                   address += Random.int_range (1, 255).to_string ();
                   address += ".";
                   address += Random.int_range (1, 255).to_string ();
                   address += ".";
                   address += Random.int_range (1, 255).to_string ();

            return address;
        }

        public static string random_client_id () {
            string id  = "0";
                   id += Random.int_range (80, 99).to_string ();
                   id += "-";
                   id += Random.int_range (100, 999).to_string ();
                   id += "-";
                   id += Random.int_range (100, 999).to_string ();

            return id;
        }

        public static string random_network_id () {
            string id  = "0";
                   id += Random.int_range (40, 45).to_string ();
                   id += "-";
                   id += Random.int_range (100, 999).to_string ();
                   id += "-";
                   id += Random.int_range (100, 999).to_string ();

            return id;
        }

        public static string get_list () {
            string output = "";

            if (demo_mode) {
                if (last_list != null) {
                    return last_list;
                } else if (demo_list_path != null) {
                    output = Command.return_output ("cat " + demo_list_path);
                } else {
                    output += " * [Artwork]  capacity: 3/5, subscription type: Free, owner: ztefn (090-736-821)\n";
                    output += "       " + random_client_id () + "   Lapo                       " + random_address () + "  alias: not set                             direct\n";
                    output += "     * 090-736-821   ztefn                                     " + random_address () + "  alias: not set        2146:0d::987:a654    direct\n";
                    output += "   [Bug Hunters]  capacity: 4/5,   [192.168.155.24/24]  subscription type: Free, owner: This computer\n";
                    output += "     x " + random_client_id () + "   Eduardo                    192.168.155.21\n";
                    output += "     * " + random_client_id () + "   freijon                    192.168.155.22  alias: not set                             direct\n";
                    output += "     ? 094-139-744 \n";
                    output += "       You are approaching your member limit and may soon have to upgrade your network.\n";
                    output += " * [" + random_network_id () + "]  Development  capacity: 2/32, subscription type: Standard, owner: ztefn (090-736-821)\n";
                    output += "     * 090-736-821   ztefn                                     " + random_address () + "  alias: not set        2146:0d::987:a654    direct\n";
                    output += "   [" + random_network_id () + "]Packaging  capacity: 4/256, subscription type: Premium, owner: Andrew (094-409-761)\n";
                    output += "     * 094-409-761   Andrew                                    " + random_address () + "  alias: not set                             via relay\n";
                    output += "     * " + random_client_id () + "   carstene1ns                " + random_address () + "  alias: not set                             direct\n";
                    output += "       " + random_client_id () + "   etamPL                     " + random_address () + "\n";
                    output += " * [" + random_network_id () + "]Translators  capacity: 18/256, subscription type: Multi-network, owner: translators@haguichi.net\n";
                    output += "     x " + random_client_id () + "   Aytunç                     " + random_address () + "\n";
                    output += "     * " + random_client_id () + "   Brbla                      " + random_address () + "  alias: not set                             via relay\n";
                    output += "       " + random_client_id () + "   Daniel                     " + random_address () + "\n";
                    output += "     ! " + random_client_id () + "   dimitrov                   " + random_address () + "  alias: not set                             IP protocol mismatch between you and peer\n";
                    output += "     * " + random_client_id () + "   enolp                      " + random_address () + "  alias: not set                             direct\n";
                    output += "       " + random_client_id () + "   enricog                    " + random_address () + "\n";
                    output += "     * " + random_client_id () + "   fitojb                     " + random_address () + "\n";
                    output += "     * " + random_client_id () + "   Fedik                      " + random_address () + "  alias: not set                             direct\n";
                    output += "     * " + random_client_id () + "   galamarv                   " + random_address () + "  alias: not set                             via relay\n";
                    output += "       " + random_client_id () + "   ryonakano                  " + random_address () + "\n";
                    output += "     * " + random_client_id () + "   Jean-Marc                  " + random_address () + "  alias: not set                             direct\n";
                    output += "     ! " + random_client_id () + "   jmb_kz                     " + random_address () + "  alias: not set                             direct\n";
                    output += "     x " + random_client_id () + "   Ḷḷumex03                   " + random_address () + "\n";
                    output += "     * " + random_client_id () + "   Moo                        " + random_address () + "  alias: not set                             direct\n";
                    output += "     * " + random_client_id () + "   piotrdrag                  " + random_address () + "  alias: not set                             direct\n";
                    output += "     * " + random_client_id () + "   Rodrigo                    " + random_address () + "  alias: not set                             via relay\n";
                    output += "     ! " + random_client_id () + "   scrawl                     " + random_address () + "  alias: 25.353.432.28  2620:9b::753:b470    direct      UDP  170.45.240.141:43667  This address is also used by another peer\n";
                    output += "       " + random_client_id () + "   Sergey                     " + random_address () + "\n";
                    output += "     x " + random_client_id () + "   Soker                      " + random_address () + "\n";
                    output += "     * " + random_client_id () + "   Zdeněk                     " + random_address () + "  alias: not set                             direct\n";
                    output += "     * " + random_client_id () + "   ztefn                      " + random_address () + "  alias: not set        2146:0d::987:a654    direct\n";
                }
            } else {
                output = Command.return_output ("hamachi list");
                debug ("get_list:\n%s", output);
            }

            last_list = output;
            return last_list;
        }

        public static List<Network> return_networks () {
            List<Network> networks = new List<Network> ();

            string[] split = last_list.split ("\n");
            Network cur_network = null;

            try {
                int64 start_time = get_real_time ();

                Regex network_regex_part1     = new Regex ("""^ (?<status>.{1}) \[(?<id>[0-9-]{11}|.+)\][ ]*(?<name>.*?)[ ]*$""");
                Regex network_regex_part2     = new Regex ("""^(capacity: [0-9]{1,3}/(?<capacity>[0-9]{1,3}),)?[ ]*(\[(?<subnet>[0-9\./]{9,19})\])?[ ]*( subscription type: (?<subscription>[^,]+),)?( owner: (?<owner>.*))?$""");
                Regex normal_member_regex     = new Regex ("""^     (?<status>.{1}) (?<id>[0-9-]{11})[ ]+(?<nick>.*?)[ ]*(?<ipv4>[0-9\.]{7,15})?[ ]*(alias: (?<alias>not set|[0-9\.]{7,15}))?[ ]*(?<ipv6>[0-9a-f\:]{6,39})?[ ]*(?<connection>direct|via relay|via server)?[ ]*(?<transport>UDP|TCP)?[ ]*(?<tunnel>[0-9\.]{7,15}\:[0-9]{1,5})?[ ]*(?<message>[ a-zA-Z]+)?$""");
                Regex unapproved_member_regex = new Regex ("""^     \? (?<id>[0-9-]{11})[ ]*$""");

                foreach (string s in split) {
                    // Check string for minimum chars
                    if (s.length > 5) {
                        // Line contains network
                        if (s.index_of ("[") == 3) {
                            int index = s.last_index_of ("capacity: ");

                            string part1 = s.substring (0, index);
                            string part2 = s.substring (index, -1);

                            MatchInfo mi1; network_regex_part1.match (part1, RegexMatchFlags.NOTEMPTY, out mi1);
                            MatchInfo mi2; network_regex_part2.match (part2, RegexMatchFlags.NOTEMPTY, out mi2);

                            string id       = mi1.fetch_named ("id");
                            string name     = mi1.fetch_named ("name");
                            string owner    = mi2.fetch_named ("owner");
                            string capacity = mi2.fetch_named ("capacity");
                            Status status   = new Status (mi1.fetch_named ("status"));

                            if (name == "") {
                                name = id;
                            }

                            int capacity_int = 0;
                            if (capacity != null) {
                                capacity_int = int.parse (capacity);
                            }

                            Network network = new Network (status, id, name, owner, capacity_int);
                            networks.append (network);

                            cur_network = network;
                        } else if (s.index_of ("?") == 5) {
                            // Line contains unapproved member
                            MatchInfo mi;
                            unapproved_member_regex.match (s, RegexMatchFlags.NOTEMPTY, out mi);

                            string id     = mi.fetch_named ("id");
                            string nick   = _("Unknown");
                            Status status = new Status ("?");

                            Member member = new Member (status, cur_network, null, null, nick, id, null);

                            cur_network.add_member (member);
                        } else if (s.index_of ("-") == 10) {
                            // Line contains normal member

                            // UTF-8 multibyte characters in long nicknames may get cut off in the network list.
                            // Therefore by calling the make_valid string function, bytes that could not be
                            // interpreted as valid Unicode are replaced with the Unicode replacement character (U+FFFD).
                            s = s.make_valid ();

                            MatchInfo mi;
                            normal_member_regex.match (s, RegexMatchFlags.NOTEMPTY, out mi);

                            string id         = mi.fetch_named ("id");
                            string nick       = mi.fetch_named ("nick");
                            string ipv4       = mi.fetch_named ("ipv4");
                            string ipv6       = mi.fetch_named ("ipv6");
                            string alias      = mi.fetch_named ("alias");
                            string tunnel     = mi.fetch_named ("tunnel");
                            string connection = mi.fetch_named ("connection");
                            string message    = mi.fetch_named ("message");
                            Status status     = new Status.complete (mi.fetch_named ("status"), connection, message);

                            if (nick == "" || nick == "anonymous") {
                                nick = _("Anonymous");
                            }

                            if (ipv4 == "") {
                                ipv4 = null;
                            }

                            if (ipv6 == "") {
                                ipv6 = null;
                            }

                            if (alias != null && alias.contains (".")) {
                                ipv4 = alias;
                                ipv6 = null; // IPv6 address doesn't work when the alias is set, therefore clearing it
                            }

                            if (tunnel == "") {
                                tunnel = null;
                            }

                            Member member = new Member (status, cur_network, ipv4, ipv6, nick, id, tunnel);

                            cur_network.add_member (member);
                        }
                    }
                }

                debug ("return_networks: Parsed network list in %s microseconds".printf ((get_real_time () - start_time).to_string ()));
            } catch (RegexError e) {
                critical ("return_networks: %s", e.message);
            }

            return networks;
        }

        public static string set_nick (string nick) {
            string output = "";

            if (!demo_mode) {
                output = Command.return_output ("hamachi set-nick \"%s\"".printf (Utils.clean_string (nick)));
                debug ("set_nick: %s", output);
            }

            return output;
        }

        public static string set_protocol (string protocol) {
            string output = "";

            if (!demo_mode) {
                output = Command.return_output ("hamachi set-ip-mode \"%s\"".printf (protocol));
                debug ("set_protocol: %s", output);
            }

            return output;
        }

        public static string attach (string account_id, bool with_networks) {
            string output  = "";
            string command = "attach";

            if (with_networks) {
                command += "-net";
            }

            output = Command.return_output ("hamachi %s \"%s\"".printf (command, Utils.clean_string (account_id)));
            debug ("attach: %s", output);

            return output;
        }

        public static string cancel () {
            string output = "";

            if (demo_mode) {
                demo_account = "-";
            } else {
                output = Command.return_output ("hamachi cancel");
                debug ("cancel: %s", output);
            }

            return output;
        }

        public static string join_network (string name, string password) {
            string output = Command.return_output ("hamachi do-join \"%s\" \"%s\"".printf (Utils.clean_string (name), Utils.clean_string (password)));
            debug ("join_network: %s", output);

            return output;
        }

        public static string set_access (string network_id, string locking, string approve) {
            string output = "";

            if (!demo_mode) {
                output = Command.return_output ("hamachi set-access \"%s\" %s %s".printf (Utils.clean_string (network_id), locking, approve));
                debug ("set_access: %s", output);

                if (!output.contains (".. ok")) {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.show_toast (output.strip ());
                        return false;
                    });
                }
            }

            return output;
        }

        public static string set_password (string network_id, string password) {
            string output = "";

            if (!demo_mode) {
                // Call with timeout because this command frequently hangs
                output = Command.return_output_with_timeout (1, "hamachi set-pass \"%s\" \"%s\"".printf (Utils.clean_string (network_id), Utils.clean_string (password)));
                debug ("set_password: %s", output);

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    if (output.has_suffix (" ..")) {
                        warning ("set_password: command hanged, restarting hamachi...");
                        win.network_list.save_state ();
                        win.show_toast (_("Password changed"));
                        Controller.restart_hamachi ();
                    } else if (output.contains (".. ok")) {
                        win.show_toast (_("Password changed"));
                    } else {
                        win.show_toast (output.strip ());
                    }

                    return false;
                });
            }

            return output;
        }

        public static string create_network (string name, string password) {
            string output = Command.return_output ("hamachi create \"%s\" \"%s\"".printf (Utils.clean_string (name), Utils.clean_string (password)));
            debug ("create_network: %s", output);

            return output;
        }

        public static void save_config (string path, Adw.PreferencesDialog dialog) {
            new Thread<void*> (null, () => {
                var save_toast = new Adw.Toast (_("Saving backup…")) {
                    timeout = 0
                };

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    dialog.add_toast (save_toast);
                    return false;
                });

                // Wait a second to show toast for minimum amount of time
                Thread.usleep (1000000);

                string output = Command.return_output ("tar -cavPf '%s' %s".printf (path, DATA_PATH));
                debug ("save_config: %s", output);

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    save_toast.dismiss ();
                    dialog.add_toast (new Adw.Toast (output.contains (DATA_PATH)? _("Backup saved") : _("Failed to save backup")));
                    return false;
                });

                return null;
            });
        }

        public static void restore_config (string path, Adw.PreferencesDialog dialog) {
            new Thread<void*> (null, () => {
                string output = Command.return_output ("tar -tvf '%s'".printf (path));
                debug ("restore_config: Listing archive contents...\n%s", output);

                if (output.contains (DATA_PATH)) {
                    var restore_toast = new Adw.Toast (_("Restoring backup…")) {
                        timeout = 0
                    };

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        dialog.add_toast (restore_toast);
                        Controller.stop_hamachi ();
                        return false;
                    });

                    string working_path = path;

                    // When running inside Flatpak sanbox we create temporary copy of the file within the home directory so it's accessible as super user,
                    // otherwise we get a permission error when trying to access /run/user/1000/doc/xxxxxxxx/etc
                    if (Xdp.Portal.running_under_flatpak ()) {
                        working_path = "/home/%s/.hamachi-config-restore".printf (Environment.get_user_name ());

                        debug ("restore_config: Creating temporary file at %s...", working_path);
                        Command.return_output ("cp " + path + " " + working_path);
                    }

                    output = Command.return_output (Command.sudo + " " + Command.sudo_start + "bash -c \"" +
                                                    service.printf ("start") + "; " +
                                                    service.printf ("stop") + "; " +
                                                    "killall -9 hamachid &> /dev/null; rm " + DATA_PATH +
                                                    "/*; tar -xavf '" + working_path + "' -C /; " +
                                                    service.printf ("start") + "\"");
                    debug ("restore_config: %s", output);

                    // Remove temporary copy of the file
                    if (Xdp.Portal.running_under_flatpak ()) {
                        debug ("restore_config: Removing temporary file at %s...", working_path);
                        Command.return_output ("rm " + working_path);
                    }

                    // Wait a second to let hamachi settle
                    Thread.usleep (1000000);

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        Controller.init ();
                        restore_toast.dismiss ();
                        dialog.add_toast (new Adw.Toast (output.contains (DATA_PATH) ? _("Backup restored") : _("Failed to restore backup")));
                        return false;
                    });
                } else {
                    debug ("restore_config: Archive doesn't contain %s", DATA_PATH);

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        dialog.add_toast (new Adw.Toast (_("Archive does not contain expected files")));
                        return false;
                    });
                }

                return null;
            });
        }
    }
}
