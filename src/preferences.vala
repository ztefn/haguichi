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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/preferences.ui")]
    public class Preferences : Adw.PreferencesWindow {
        private Settings config;
        private Settings behavior;
        private Settings notifications;
        private Settings ui;

        private enum UpdateInterval {
            15_SECONDS = 15,
            30_SECONDS = 30,
             1_MINUTE  = 60,
             5_MINUTES = 300,
            10_MINUTES = 600,
            15_MINUTES = 900,
            NEVER      = 0,
        }

        [GtkChild]
        unowned Adw.PreferencesPage general_page;
        [GtkChild]
        unowned Adw.PreferencesPage commands_page;
        [GtkChild]
        unowned Adw.PreferencesPage desktop_page;

        [GtkChild]
        unowned Adw.ActionRow configuration;
        [GtkChild]
        unowned Adw.EntryRow nickname;
        [GtkChild]
        unowned Adw.ComboRow protocol;

        [GtkChild]
        unowned Adw.ComboRow update_interval;
        [GtkChild]
        unowned Adw.SwitchRow connect_on_startup;
        [GtkChild]
        unowned Adw.SwitchRow reconnect_on_connection_loss;
        [GtkChild]
        unowned Adw.SwitchRow disconnect_on_quit;

        [GtkChild]
        public unowned CommandsEditor commands_editor;

        [GtkChild]
        public unowned Gtk.CheckButton system;
        [GtkChild]
        public unowned Gtk.CheckButton light;
        [GtkChild]
        public unowned Gtk.CheckButton dark;
        [GtkChild]
        public unowned Adw.SwitchRow show_indicator;

        [GtkChild]
        unowned Adw.SwitchRow notify_on_connection_loss;
        [GtkChild]
        unowned Adw.SwitchRow notify_on_member_join;
        [GtkChild]
        unowned Adw.SwitchRow notify_on_member_leave;
        [GtkChild]
        unowned Adw.SwitchRow notify_on_member_online;
        [GtkChild]
        unowned Adw.SwitchRow notify_on_member_offline;

        construct {
            general_page.icon_name = Utils.get_available_theme_icon ({
                "applications-system-symbolic",
                "emblem-system-symbolic",
                "preferences-system-symbolic"
            });
            commands_page.icon_name = Utils.get_available_theme_icon ({
                "utilities-terminal-symbolic",
                "system-run-symbolic"
            });
            desktop_page.icon_name = Utils.get_available_theme_icon ({
                "preferences-desktop-appearance-symbolic",
                "user-desktop-symbolic"
            });

            config        = new Settings (Config.APP_ID + ".config");
            behavior      = new Settings (Config.APP_ID + ".behavior");
            notifications = new Settings (Config.APP_ID + ".notifications");
            ui            = new Settings (Config.APP_ID + ".ui");

            install_action ("config.open",    null, (Gtk.WidgetActionActivateFunc) open_config);
            install_action ("config.save",    null, (Gtk.WidgetActionActivateFunc) save_config);
            install_action ("config.restore", null, (Gtk.WidgetActionActivateFunc) restore_config);

            var config_exists = Utils.path_exists ("d", Hamachi.DATA_PATH);

            configuration.subtitle = config_exists ? Hamachi.DATA_PATH : _("Not present");
            action_set_enabled ("config.open", config_exists);
            action_set_enabled ("config.save", config_exists);

            nickname.text = Utils.parse_nick (config.get_string ("nickname"));
            nickname.apply.connect (() => {
                var nick = nickname.text;
                var parsed_nick = Utils.parse_nick (nick);
                debug ("nickname changed to: %s (parsed: %s)", nick, parsed_nick);

                nickname.editable  = false;
                nickname.sensitive = false;
                nickname.text      = parsed_nick;
                nickname.sensitive = true;
                nickname.editable  = true;

                config.set_string ("nickname", nick);
                win.set_nick (parsed_nick);

                // Run hamachi command if connected
                if (Controller.last_status >= 6) {
                    new Thread<void*> (null, () => {
                        Hamachi.set_nick (parsed_nick);
                    });
                }
            });

            protocol.selected = (int) config.get_enum ("protocol");
            protocol.notify["selected-item"].connect ((sender, property) => {
                var i = (int) protocol.selected;
                config.set_enum ("protocol", i);

                // Run hamachi command if connected
                if (Controller.last_status >= 6) {
                    new Thread<void*> (null, () => {
                        var protocol = "both";
                        if (i == 1) {
                            protocol = "ipv4";
                        } else if (i == 2) {
                            protocol = "ipv6";
                        }
                        Hamachi.set_protocol (protocol);
                    });
                }
            });

            var model      = new Adw.EnumListModel (typeof (UpdateInterval));
            var expression = new Gtk.CClosureExpression (typeof (string), null, {}, (Callback) update_interval_name, null, null);
            update_interval.expression = expression;
            update_interval.model      = model;
            update_interval.selected   = get_update_interval_row_index ();
            update_interval.notify["selected-item"].connect ((sender, property) => {
                var item = (Adw.EnumListItem) update_interval.selected_item;
                var val  = (UpdateInterval) item.get_value ();
                config.set_int ("update-interval", val);
            });

            behavior.bind ("connect-on-startup",           connect_on_startup,           "active", DEFAULT);
            behavior.bind ("reconnect-on-connection-loss", reconnect_on_connection_loss, "active", DEFAULT);
            behavior.bind ("disconnect-on-quit",           disconnect_on_quit,           "active", DEFAULT);

            var scheme = (int) ui.get_enum ("color-scheme");
            if (scheme == 1) {
                light.active = true;
            } else if (scheme == 2) {
                dark.active = true;
            } else {
                system.active = true;
            }

            ui.bind ("show-indicator", show_indicator, "active", DEFAULT);

            show_indicator.visible = indicator.item.host_registered;
            indicator.item.notify["host-registered"].connect (() => {
                show_indicator.visible = indicator.item.host_registered;
            });
            show_indicator.notify["active"].connect ((sender, property) => {
                indicator.active = ui.get_boolean ("show-indicator");
                win.update_indicator_status ();
            });

            notifications.bind ("connection-loss", notify_on_connection_loss, "active", DEFAULT);
            notifications.bind ("member-join",     notify_on_member_join,     "active", DEFAULT);
            notifications.bind ("member-leave",    notify_on_member_leave,    "active", DEFAULT);
            notifications.bind ("member-online",   notify_on_member_online,   "active", DEFAULT);
            notifications.bind ("member-offline",  notify_on_member_offline,  "active", DEFAULT);

            if (demo_mode) {
                notify_on_connection_loss.notify["active"].connect ((sender, property) => {
                    Controller.notify_connection_lost ();
                });
                notify_on_member_join.notify["active"].connect ((sender, property) => {
                    Controller.notify_member_joined  ("T-800", "Skynet", 0, "000-000-000", {"000-000-000"});
                });
                notify_on_member_leave.notify["active"].connect ((sender, property) => {
                    Controller.notify_member_left    ("T-800", "Skynet", 0);
                });
                notify_on_member_online.notify["active"].connect ((sender, property) => {
                    Controller.notify_member_online  ("T-800", "Skynet", 1);
                });
                notify_on_member_offline.notify["active"].connect ((sender, property) => {
                    Controller.notify_member_offline ("T-800", "Skynet", 2);
                });
            }
        }

        [GtkCallback]
        private void on_style_selection () {
            int scheme = 0;
            if (light.active) {
                scheme = 1;
            } else if (dark.active) {
                scheme = 2;
            }
            ui.set_enum ("color-scheme", scheme);
            app.set_color_scheme (scheme);
        }

        private void open_config () {
            Command.open_file (Hamachi.DATA_PATH);
        }

        private ListStore get_file_filters () {
            var tar = new Gtk.FileFilter ();
            tar.set_filter_name (_("All supported archives"));
            tar.add_mime_type ("application/x-tar");
            tar.add_mime_type ("application/x-compressed-tar");
            tar.add_mime_type ("application/x-bzip-compressed-tar");
            tar.add_mime_type ("application/x-lzma-compressed-tar");
            tar.add_mime_type ("application/x-xz-compressed-tar");
            tar.add_mime_type ("application/x-zstd-compressed-tar");

            var filters = new ListStore (typeof (Gtk.FileFilter));
            filters.append (tar);

            return filters;
        }

        private void save_config () {
            debug ("config.save action activated");

            var now = new DateTime.now_local ();

            var dialog = new Gtk.FileDialog () {
                title        = _("Save a Backup"),
                initial_name = "logmein-hamachi-config_" + now.format ("%Y-%m-%d") + ".tar.gz",
                modal        = true,
                filters      = get_file_filters ()
            };

            dialog.save.begin (this, null, (obj, res) => {
                try {
                    var file = dialog.save.end (res);
                    var path = file.get_path ();
                    debug ("save_config: Selected path %s: ", path);
                    Hamachi.save_config (path, this);
                } catch (Error e) {
                    critical ("save_config %s: ", e.message);
                }
            });
        }

        private void restore_config () {
            debug ("config.restore action activated");

            var dialog = new Gtk.FileDialog () {
                title        = _("Restore from Backup"),
                accept_label = _("_Restore"),
                modal        = true,
                filters      = get_file_filters ()
            };

            dialog.open.begin (this, null, (obj, res) => {
                try {
                    var file = dialog.open.end (res);
                    var path = file.get_path ();
                    debug ("restore_config: Selected path %s: ", path);
                    Hamachi.restore_config (path, this);
                } catch (Error e) {
                    critical ("restore_config: %s", e.message);
                }
            });
        }

        private int get_update_interval_row_index () {
            var interval = config.get_int ("update-interval");

            if (interval == 0) {
              return 6; // Never
            } else if (interval < 30) {
              return 0; // 15 seconds
            } else if (interval < 60) {
              return 1; // 30 seconds
            } else if (interval < 300) {
              return 2; // 1 minute
            } else if (interval < 600) {
              return 3; // 5 minutes
            } else if (interval < 900) {
              return 4; // 10 minutes
            } else {
              return 5; // 15 minutes
            }
        }

        private static string update_interval_name (Adw.EnumListItem item) {
            switch (item.get_value ()) {
                case UpdateInterval.NEVER:
                    return _("Never");
                case UpdateInterval.15_SECONDS:
                    return get_seconds_interval_label (15);
                case UpdateInterval.30_SECONDS:
                    return get_seconds_interval_label (30);
                case UpdateInterval.1_MINUTE:
                    return get_minutes_interval_label (1);
                case UpdateInterval.5_MINUTES:
                    return get_minutes_interval_label (5);
                case UpdateInterval.10_MINUTES:
                    return get_minutes_interval_label (10);
                case UpdateInterval.15_MINUTES:
                    return get_minutes_interval_label (15);
                default:
                    return "";
            }
        }

        private static string get_seconds_interval_label (int interval) {
            return ngettext ("%S second", "%S seconds", interval).replace ("%S", interval.to_string ());
        }

        private static string get_minutes_interval_label (int interval) {
            return ngettext ("%S minute", "%S minutes", interval).replace ("%S", interval.to_string ());
        }
    }
}
