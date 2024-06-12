 /*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2024 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

namespace Haguichi {
    public class Command : Object {
        public static string spawn_wrap;

        public static string sudo;
        public static string sudo_start;

        public static string terminal;
        public static string file_manager;
        public static string remote_desktop;

        private static Menu commands_menu;
        private static CustomCommand default_command;
        private static Array<CustomCommand> active_commands;

        private static Settings settings;

        public static void init () {
            settings = new Settings (Config.APP_ID + ".commands");

            determine_sudo ();
            determine_terminal ();
            determine_file_manager ();
            determine_remote_desktop ();

            Idle.add_full (Priority.HIGH_IDLE, () => {
                fill_custom_commands ();
                return false;
            });
        }

        public static void execute (string command) {
            if (command == "") {
                return;
            }

            try {
                Process.spawn_command_line_async (spawn_wrap + command);
            } catch (SpawnError e) {
                critical ("execute: %s", e.message);
            }
        }

        public static string return_output (string command) {
            string stdout;
            string stderr;
            int    exit_status;

            try {
                Process.spawn_command_line_sync (spawn_wrap + command, out stdout, out stderr, out exit_status);

                if (stderr != "") {
                    warning ("return_output stderr: %s", stderr);
                }
            } catch (SpawnError e) {
                critical ("return_output: %s", e.message);
            }

            // We don't like NULL strings
            if (stdout == null) {
                stdout = "";
            }

            // When hamachi is busy try again after a little while
            if (stdout.contains (".. failed, busy")) {
                debug ("Hamachi is busy, waiting to try again...");
                Thread.usleep (100000);

                stdout = return_output (command);
            }

            // When there's no regular output we'd want to return the error if available
            return (stdout == "" && stderr != "") ? stderr : stdout;
        }

        public static string return_output_with_timeout (int timeout, string command) {
            return return_output ("timeout %d %s".printf (timeout, command));
        }

        public static bool exists (string? command) {
            if (command == null) {
                return false;
            }

            string stdout = return_output ("bash -c \"command -v " + command + " &>/dev/null || echo 'command not found'\"");
            return !stdout.contains ("command not found");
        }

        public static void determine_sudo () {
            sudo       = settings.get_string ("super-user");
            sudo_start = "-- ";

            if (sudo == "auto") {
                sudo = get_available ({
                    "pkexec",
                    "sudo"
                });
            }

            if (sudo == "pkexec") {
                sudo_start = "";
            }

            debug ("determine_sudo: %s", sudo);
        }

        private static string get_available (string[] commands) {
            // Check each command in the list for existence, and return immediately if it does
            foreach (string cmd in commands) {
                if (exists (cmd)) {
                    return cmd;
                }
            }

            // Return the first command as fallback
            return commands[0];
        }

        public static void determine_terminal () {
            terminal = get_available ({
                "gnome-terminal",
                "mate-terminal",
                "io.elementary.terminal",
                "kgx",
                "tilix",
                "xfce4-terminal",
                "konsole",
                "deepin-terminal",
                "qterminal",
                "lxterminal",
                "uxterm",
                "xterm"
            });

            debug ("determine_terminal: %s", terminal);
        }

        public static void determine_file_manager () {
            file_manager = get_available ({
                "nautilus",
                "caja",
                "nemo",
                "io.elementary.files",
                "thunar",
                "dolphin",
                "dde-file-manager",
                "pcmanfm-qt",
                "pcmanfm"
            });

            debug ("determine_file_manager: %s", file_manager);
        }

        public static void determine_remote_desktop () {
            remote_desktop = get_available ({
                "gvncviewer",
                "krdc",
                "vncviewer",
                "xtightvncviewer",
                "xvnc4viewer",
                "rdesktop",
                "vinagre"
            });

            debug ("determine_remote_desktop: %s", remote_desktop);
        }

        public static Menu get_commands_menu () {
            if (commands_menu == null) {
                commands_menu = new Menu ();
            }

            return commands_menu;
        }

        public static void fill_custom_commands () {
            new Thread<void*> (null, () => {
                // Initiate or clear existing commands
                default_command = null;
                active_commands = new Array<CustomCommand> ();
                get_commands_menu ().remove_all ();

                int number = 1;
                string[] commands = settings.get_strv ("customizable");
                foreach (string command_str in commands) {
                    string[] command = command_str.split (";", 6);

                    if (command.length == 6) {
                        string is_active  = command[0];
                        string is_default = command[1];
                        string label      = command[2];
                        string cmd_ipv4   = command[3].replace ("{COLON}", ";");
                        string cmd_ipv6   = command[4].replace ("{COLON}", ";");
                        string priority   = command[5];

                        var custom_command = new CustomCommand (is_active, is_default, label, cmd_ipv4, cmd_ipv6, priority);
                        if (custom_command.exists ()) {
                            if (command[0] == "true") {
                                win.install_action ("win.run-command-%d".printf (number), null, (Gtk.WidgetActionActivateFunc) run_command_action);
                                active_commands.append_val (custom_command);
                                commands_menu.append (_(label), "win.run-command-%d".printf (number));
                                number ++;
                            }
                            if (command[1] == "true") {
                                default_command = custom_command;
                            }
                        }
                    }
                }

                return null;
            });
        }

        public static void set_active_commands_enabled (Member member) {
            if (active_commands != null) {
                var number = 1;
                foreach (CustomCommand command in active_commands) {
                    // Enable if member is online or command doesn't use address variable
                    var enabled = (member.status.status_int == 1 || (!command.cmd_ipv4.contains ("%A") && !command.cmd_ipv6.contains ("%A")));
                    win.action_set_enabled ("win.run-command-%d".printf (number), enabled);
                    number ++;
                }
            }
        }

        public static string replace_variables (owned string command, string address, string nick, string id) {
            try {
                command = command.replace ("%A",  address);
                command = command.replace ("%N",  nick   );
                command = command.replace ("%ID", id     );

                string execute = (terminal == "gnome-terminal") ? "--" : "-e";
                string quote   = (terminal == "gnome-terminal") ? ""   : "\"";

                command = new Regex ("%TERMINAL (.*)").replace (command, -1, 0, terminal + " " + execute + " " + quote + "\\1" + quote);
                command = command.replace ("%FILEMANAGER",   file_manager);
                command = command.replace ("%REMOTEDESKTOP", remote_desktop);
                command = command.replace ("{COLON}",        ";");
            } catch (RegexError e) {
                critical ("replace_variables: %s", e.message);
            }

            return (command != null) ? command : "";
        }

        private void run_command_action (string action_name, Variant? parameter) {
            var number = int.parse (action_name.replace ("win.run-command-", ""));
            var index  = number - 1;
            var member = win.network_list.get_selected_member ();

            if (member != null && index < active_commands.length) {
                var command = active_commands.index (index);
                var cmd = command.return_for_member (member);
                debug ("run_command_action: %s", cmd);
                execute (cmd);
            }
        }

        public static void execute_default_command (Member member) {
            var cmd = default_command.return_for_member (member);
            debug ("execute_default_command: %s", cmd);
            execute (cmd);
        }

        public static void open_uri (string uri) {
            debug ("open_uri: %s", uri);
            new Gtk.UriLauncher (uri).launch.begin (win, null);
        }

        public static void open_file (string path) {
            debug ("open_file: %s", path);
            File file = File.new_for_path (path);
            new Gtk.FileLauncher (file).launch.begin (win, null);
        }
    }
}
