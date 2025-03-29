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

using Config;

namespace Haguichi {
    public Application app;
    public Window      win;
    public Indicator   indicator;

    public static Connection connection;
    public static Session    session;
    public static Inhibitor  inhibitor;

    public static bool   run_in_background;
    public static bool   demo_mode;
    public static string demo_list_path;
    public static string current_desktop;

    public static ThreadPool<Member>  member_threads;
    public static ThreadPool<Network> network_threads;

    private static uint registration_id;

    public class Application : Adw.Application {
        public Application () {
            Object (application_id: APP_ID, flags: ApplicationFlags.DEFAULT_FLAGS);

            Intl.bindtextdomain          (GETTEXT_PACKAGE, LOCALEDIR);
            Intl.bind_textdomain_codeset (GETTEXT_PACKAGE, "UTF-8");
            Intl.textdomain              (GETTEXT_PACKAGE);
        }

        construct {
            add_main_option_entries ({
                { "version",    'v', 0, OptionArg.NONE,   null,                  "Show version number",                  null   },
                { "debug",      'd', 0, OptionArg.NONE,   null,                  "Print debug messages",                 null   },
                { "background", 'b', 0, OptionArg.NONE,   out run_in_background, "Run in background",                    null   },
                { "demo",         0, 0, OptionArg.NONE,   out demo_mode,         "Run in demo mode",                     null   },
                { "list",         0, 0, OptionArg.STRING, out demo_list_path,    "Use a text file as list in demo mode", "FILE" },
                { null }
            });

            ActionEntry[] action_entries = {
                { "toggle-window",  toggle_window_action,       },
                { "connect",        connect_action              },
                { "disconnect",     disconnect_action           },
                { "join-network",   join_network_action         },
                { "create-network", create_network_action       },
                { "approve-reject", approve_reject_action, "as" },
                { "info",           info_action                 },
                { "quit",           quit_action                 },
            };
            add_action_entries (action_entries, this);
        }

        public static int main (string[] args) {
            if ("-v" in args || "--version" in args) {
                stdout.printf ("%s %s\n", APP_NAME, VERSION);
                return 0;
            }

            if ("-d" in args || "--debug" in args) {
                Environment.set_variable ("G_MESSAGES_DEBUG", APP_NAME, true);
            }

#if FOR_ELEMENTARY
            Granite.init ();
#endif

            app = new Application ();
            return app.run (args);
        }

        public override void startup () {
            base.startup ();

            uint64 startup_moment = get_real_time ();

            debug ("Greetings, I am %s %s", APP_NAME, VERSION);

            debug ("running_under_flatpak: %s", Xdp.Portal.running_under_flatpak ().to_string ());
            Command.spawn_wrap = Xdp.Portal.running_under_flatpak () ? "flatpak-spawn --host " : "";

            current_desktop = Environment.get_variable ("XDG_CURRENT_DESKTOP");
            debug ("current_desktop: %s", current_desktop);

#if FOR_ELEMENTARY
            Gtk.Settings.get_default().set ("gtk-theme-name", "io.elementary.stylesheet.strawberry");
            Gtk.Settings.get_default().set ("gtk-icon-theme-name", "elementary");
#endif

            connection = new Connection ();
            inhibitor  = new Inhibitor ();

            try {
                member_threads = new ThreadPool<Member>.with_owned_data ((member) => {
                    member.get_long_nick_thread ();
                }, 2, false);
                network_threads = new ThreadPool<Network>.with_owned_data ((network) => {
                    network.determine_ownership_thread ();
                }, 2, false);
            } catch (ThreadError e) {
                critical ("threading: %s", e.message);
            }

            Controller.init ();

            debug ("startup: Completed startup in %s microseconds", (get_real_time () - startup_moment).to_string ());
        }

        public override void activate () {
            base.activate ();

            if (win == null) {
                win = new Window (this);

                if (!run_in_background) {
                    win.present ();
                    win.set_focus (null);
                }
            } else {
                win.present ();
            }
        }

        public override bool dbus_register (DBusConnection connection, string object_path) throws Error {
            base.dbus_register (connection, object_path);

            session = new Session ();
            registration_id = connection.register_object ("/com/github/ztefn/haguichi", session);

            return true;
        }

        public override void dbus_unregister (DBusConnection connection, string object_path) {
            connection.unregister_object (registration_id);

            base.dbus_unregister (connection, object_path);
        }

        public void set_color_scheme (int scheme) {
            var color_scheme = scheme == 1 ? Adw.ColorScheme.FORCE_LIGHT :
                               scheme == 2 ? Adw.ColorScheme.FORCE_DARK  :
                                             Adw.ColorScheme.DEFAULT;

            debug ("set_color_scheme: setting color scheme to %s", color_scheme.to_string ());
            style_manager.set_color_scheme (color_scheme);
        }

        public void toggle_window_action () {
            if (win.visible) {
                win.hide ();
            } else {
                win.present ();
            }
        }

        private void connect_action () {
            win.trigger_action ("connect");
        }

        private void disconnect_action () {
            win.trigger_action ("disconnect");
        }

        public void join_network_action () {
            win.present ();
            win.trigger_action ("join-network");
        }

        public void create_network_action () {
            win.present ();
            win.trigger_action ("create-network");
        }

        private void info_action () {
            win.present ();
            win.trigger_action ("info");
        }

        private void quit_action () {
            win.trigger_action ("quit");
        }

        private void approve_reject_action (SimpleAction action, Variant? param) {
            string[] args = param.get_strv ();

            // We expect at least three arguments (operation, client ID and one or more network IDs)
            if (args.length > 2) {
                var operation = args[0];
                var client_id = args[1];

                // Collect the network IDs
                string[] network_ids = {};
                for (int i = 2; i < args.length; i++) {
                    network_ids += args[i];
                }

                debug ("approve_or_reject: Go %s join request for client %s in network(s) %s\n", operation, client_id, string.joinv (", ", network_ids));

                // Approve or reject member only if we are still connected
                if (Controller.last_status >= 6) {
                    // Iterate all supplied network IDs
                    foreach (string network_id in network_ids) {
                        // Find actual network that matches the ID
                        foreach (Network network in connection.networks) {
                            if (network.id == network_id) {
                                // Find our member in the network
                                foreach (Member member in network.members) {
                                    if (member.id == client_id) {
                                        // Check if member still requires approval
                                        if (member.status.status_int == 3) {
                                            // Finally execute the desired operation
                                            if (operation == "approve") {
                                                member.approve ();
                                            } else if (operation == "reject") {
                                                member.reject ();
                                            }
                                        }
                                        // Stop iteration of members
                                        break;
                                    }
                                }
                                // Stop iteration of networks
                                break;
                            }
                        }
                    }
                } else {
                    debug ("approve_reject: Not connected");
                }
            } else {
                warning ("approve_reject: Too few arguments, expected \"operation, client_id, network_id1, network_id2, ...\"");
            }
        }
    }
}
