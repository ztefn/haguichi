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
    public class Command : Object {
        public static string spawn_wrap;

        public static string sudo;
        public static string sudo_start;

        public static string terminal;
        public static string file_manager;
        public static string remote_desktop;

        private static string flatpak_list;

        private static Menu commands_menu;
        private static CustomCommand default_command;
        private static Array<CustomCommand> active_commands;

        private static Settings settings;

        public static void init () {
            settings = new Settings (Config.APP_ID + ".commands");

            if (Command.exists ("flatpak")) {
                flatpak_list = Command.return_output ("flatpak list --app");
                debug ("flatpak_list:\n%s", flatpak_list);
            }

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
            if (command == null || command == "") {
                return false;
            }

            bool result;
            if (command.has_prefix ("flatpak run ")) {
                try {
                    // Extract application ID and check for its presence in the list
                    // Ignore any OPTIONS and ARGUMENTS from the command string and find the application ID by its reverse-DNS format
                    MatchInfo mi;
                    new Regex ("""^flatpak run.*? (?<id>[a-zA-Z0-9][a-zA-Z0-9\.\_\-]+[a-zA-Z0-9]).*?$""").match (command, 0, out mi);
                    string id = mi.fetch_named ("id");
                    result = (flatpak_list != null && flatpak_list.contains ("\t%s\t".printf (id)));
                } catch (RegexError e) {
                    critical ("exists: %s", e.message);
                    return false;
                }
            } else {
                string stdout = return_output ("bash -c \"command -v " + command.split (" ", 0)[0] + " &>/dev/null || echo 'command not found'\"");
                result = !stdout.contains ("command not found");
            }

            debug ("exists: %s %s", command, result.to_string ());
            return result;
        }

        private static void determine_sudo () {
            sudo       = settings.get_string ("super-user");
            sudo_start = "";

            if (sudo == "auto") {
                sudo = get_available ({
                    "pkexec",
                    "kdesu",
                    "sudo"
                }, "");
            }

            if (sudo == "sudo") {
                sudo_start = "-- ";
            }

            debug ("determine_sudo: %s", sudo);
        }

        private static string get_available (string[] commands, string? fallback) {
            // Check each command in the list for existence, and return immediately if it does
            foreach (string cmd in commands) {
                if (exists (cmd)) {
                    return cmd;
                }
            }

            // Return fallback if no command is available
            return fallback;
        }

        private static void determine_terminal () {
            terminal = get_available ({
                "flatpak run app.devsuite.Ptyxis",
                "flatpak run org.gnome.Console",
                "flatpak run org.kde.konsole",
                "gnome-terminal",
                "mate-terminal",
                "io.elementary.terminal",
                "kgx",
                "ptyxis",
                "ghostty",
                "tilix",
                "xfce4-terminal",
                "konsole",
                "deepin-terminal",
                "qterminal",
                "lxterminal",
                "cosmic-term",
                "uxterm",
                "xterm"
            }, "gnome-terminal");

            debug ("determine_terminal: %s", terminal);
        }

        private static void determine_file_manager () {
            file_manager = get_available ({
                "flatpak run org.gnome.Nautilus",
                "flatpak run org.kde.dolphin",
                "nautilus",
                "caja",
                "nemo",
                "io.elementary.files",
                "thunar",
                "dolphin",
                "dde-file-manager",
                "pcmanfm-qt",
                "pcmanfm",
                "cosmic-files"
            }, "nautilus");

            debug ("determine_file_manager: %s", file_manager);
        }

        private static void determine_remote_desktop () {
            remote_desktop = get_available ({
                "flatpak run org.kde.krdc",
                "gvncviewer",
                "krdc",
                "vncviewer",
                "xtightvncviewer",
                "xvnc4viewer",
                "rdesktop",
                "vinagre"
            }, "gvncviewer");

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
                    win.action_set_enabled ("win.run-command-%d".printf (number), command.enabled_for_member (member));
                    number ++;
                }
            }
        }

        public static string replace_variables (owned string command, string address, string nick, string id) {
            command = command.replace ("%A",  address);
            command = command.replace ("%N",  nick   );
            command = command.replace ("%ID", id     );

            if (command.contains ("%TERMINAL")) {
                bool use_double_dash = strv_contains ({
                    "gnome-terminal",
                    "ptyxis",
                    "flatpak run app.devsuite.Ptyxis",
                    "cosmic-term"
                }, terminal);

                bool use_quotes = !use_double_dash && terminal != "ghostty";

                string option = use_double_dash ? "--" : "-e";
                string quote  = use_quotes      ? "\"" : "";

                try {
                    // For terminals that use quotes for the command argument escape any quotes in the command itself
                    // unless already preceded by a backslash
                    if (use_quotes) {
                        Regex regex = new Regex ("(?<!\\\\)\"");
                        command = regex.replace (command, -1, 0, "\\\\\"");
                    }
                    command = new Regex ("%TERMINAL(.*)").replace (command, -1, 0, terminal + " " + option + " " + quote + "\\1" + quote);
                } catch (RegexError e) {
                    critical ("replace_variables: %s", e.message);
                }
            }
            command = command.replace ("%FILEMANAGER",   file_manager);
            command = command.replace ("%REMOTEDESKTOP", remote_desktop);
            command = command.replace ("{COLON}",        ";");

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
            if (default_command.enabled_for_member (member)) {
                var cmd = default_command.return_for_member (member);
                debug ("execute_default_command: %s", cmd);
                execute (cmd);
            }
        }

        public static void open_redirect_uri (string action) {
            open_uri ("https://haguichi.net/redirect/?action=" + action + "&version=" + Config.VERSION + "&language=" + Intl.get_language_names ()[1]);
        }

        private static void open_uri (string uri) {
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
