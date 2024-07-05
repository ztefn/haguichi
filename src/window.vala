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

using Config;

namespace Haguichi {
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/window.ui")]
    public class Window : Adw.ApplicationWindow {
        [GtkChild]
        private unowned Adw.ToastOverlay toast_overlay;

        [GtkChild]
        public unowned Adw.OverlaySplitView split_view;
        [GtkChild]
        public unowned Adw.WindowTitle window_title;
        [GtkChild]
        public unowned Adw.StatusPage not_installed_status_page;
        [GtkChild]
        public unowned Sidebar sidebar;
        [GtkChild]
        public unowned NetworkList network_list;
        [GtkChild]
        private unowned Gtk.Button refresh_button;
        [GtkChild]
        public unowned Gtk.ToggleButton connect_button;
        [GtkChild]
        public unowned Gtk.ToggleButton disconnect_button;
        [GtkChild]
        public unowned Gtk.ToggleButton search_button;
        [GtkChild]
        public unowned Gtk.SearchBar search_bar;
        [GtkChild]
        public unowned Gtk.SearchEntry search_entry;
        [GtkChild]
        public unowned Gtk.Stack stack;
        [GtkChild]
        public unowned Gtk.Stack connected_stack;

        [GtkChild]
        public unowned Gtk.Spinner spinner;

        [GtkChild]
        private unowned Gtk.Button configure_button;

        [GtkChild]
        private unowned Gtk.Button overlay_refresh_button;
        [GtkChild]
        private unowned Gtk.MenuButton overlay_add_network_button;

        private Xdp.Portal portal = new Xdp.Portal ();

        public Gtk.Window modal_dialog;

        public Window (Application app) {
            Object (application: app, title: APP_NAME);

            // KDE places the window icon in the headerbar, so only set it for other desktops
            if (current_desktop != "KDE") {
                icon_name = APP_ID;
            }

            var config = new Settings (APP_ID + ".config");
            var list   = new Settings (APP_ID + ".network-list");
            var ui     = new Settings (APP_ID + ".ui");

            set_nick (demo_mode ? "Joe Demo" : Utils.parse_nick (config.get_string ("nickname")));

            var sort_by = list.create_action ("sort-by");
            add_action (sort_by);

            var show_offline_members = list.create_action ("show-offline-members");
            add_action (show_offline_members);

            app.set_color_scheme ((int) ui.get_enum ("color-scheme"));

            if (strv_contains (Intl.get_language_names (), "ru")) {
                add_css_class ("custom");
            }

            default_width  = ui.get_int ("width");
            default_height = ui.get_int ("height");

            ui.bind ("width",  this, "default-width",  DEFAULT);
            ui.bind ("height", this, "default-height", DEFAULT);

            indicator = new Indicator ();
            indicator.active = ui.get_boolean ("show-indicator");

            search_entry.search_changed.connect (()=> {
                network_list.refilter ();
            });

            hide.connect (() => {
                session.visibility_changed (false);
                update_indicator_status ();
            });
            show.connect (() => {
                session.visibility_changed (true);
                update_indicator_status ();
            });
        }

        construct {
            install_action ("win.connect",               null, (Gtk.WidgetActionActivateFunc) connect_action);
            install_action ("win.disconnect",            null, (Gtk.WidgetActionActivateFunc) disconnect_action);
            install_action ("win.join-network",          null, (Gtk.WidgetActionActivateFunc) join_network_action);
            install_action ("win.create-network",        null, (Gtk.WidgetActionActivateFunc) create_network_action);
            install_action ("win.attach",                null, (Gtk.WidgetActionActivateFunc) attach_action);
            install_action ("win.attach-cancel",         null, (Gtk.WidgetActionActivateFunc) attach_cancel_action);
            install_action ("win.start-search",          null, (Gtk.WidgetActionActivateFunc) start_search_action);
            install_action ("win.refresh",               null, (Gtk.WidgetActionActivateFunc) refresh_action);
            install_action ("win.toggle-sidebar",        null, (Gtk.WidgetActionActivateFunc) toggle_sidebar_action);
            install_action ("win.cycle-mode",            null, (Gtk.WidgetActionActivateFunc) cycle_mode_action);
            install_action ("win.expand-all-networks",   null, (Gtk.WidgetActionActivateFunc) expand_all_networks_action);
            install_action ("win.collapse-all-networks", null, (Gtk.WidgetActionActivateFunc) collapse_all_networks_action);
            install_action ("win.preferences",           null, (Gtk.WidgetActionActivateFunc) preferences_action);
            install_action ("win.shortcuts",             null, (Gtk.WidgetActionActivateFunc) shortcuts_action);
            install_action ("win.help",                  null, (Gtk.WidgetActionActivateFunc) help_action);
            install_action ("win.info",                  null, (Gtk.WidgetActionActivateFunc) info_action);
            install_action ("win.about",                 null, (Gtk.WidgetActionActivateFunc) about_action);
            install_action ("win.quit",                  null, (Gtk.WidgetActionActivateFunc) quit_action);
        }

        [GtkCallback]
        private void download_hamachi () {
            Command.open_redirect_uri ("get-hamachi");
        }

        [GtkCallback]
        private void configure_hamachi () {
            window_title.subtitle = _("Configuring");
            configure_button.sensitive = false;
            action_set_enabled ("win.refresh", false);
            if (!demo_mode) {
                Hamachi.configure ();
            }
        }

        [GtkCallback]
        private void go_back_button_clicked () {
            split_view.show_sidebar = false;
        }

        public void connect_action () {
            Controller.start_hamachi ();
        }

        public void disconnect_action () {
            Controller.stop_hamachi ();
        }

        public void join_network_action () {
            add_network_action ("Join");
        }

        public void create_network_action () {
            add_network_action ("Create");
        }

        private void add_network_action (string mode) {
            var add_network = new JoinCreateNetworkDialog (mode);
            show_dialog (add_network);
        }

        private void attach_action () {
            var attach = new AttachDialog ();
            show_dialog (attach);
        }

        private void attach_cancel_action () {
            new Thread<void*> (null, () => {
                Hamachi.cancel ();
                return null;
            });

            sidebar.set_account ("-");
        }

        private void start_search_action () {
            search_bar.search_mode_enabled = true;
        }

        private void refresh_action () {
            if (Controller.last_status <= 1) {
                Controller.init ();
            } else if (Controller.last_status >= 6) {
                Controller.update_connection ();
            }
        }

        private void toggle_sidebar_action () {
            if (split_view.collapsed == false) return;

            toggle_sidebar ();
        }

        private void cycle_mode_action () {
            if (!demo_mode) return;

            var cur_mode = get_mode ();
            var new_mode = "";
            switch (cur_mode) {
                case "Initializing":
                    new_mode = "Disconnected";
                    break;
                case "Disconnected":
                    new_mode = "Connecting";
                    break;
                case "Connecting":
                    new_mode = "Connected";
                    break;
                case "Connected":
                    new_mode = "Not installed";
                    break;
                case "Not installed":
                    new_mode = "Not configured";
                    break;
                case "Not configured":
                    new_mode = "Initializing";
                    break;
            }

            debug ("new mode: %s", new_mode);
            set_mode (new_mode);
        }

        public void expand_all_networks_action () {
            network_list.set_all_rows_expanded (true);
        }

        public void collapse_all_networks_action () {
            network_list.set_all_rows_expanded (false);
        }

        public void preferences_action () {
            var prefs = new Preferences () {
              application = app
            };
            show_dialog (prefs);
        }

        private void shortcuts_action () {
            var builder = new Gtk.Builder.from_resource ("/com/github/ztefn/haguichi/ui/shortcuts.ui");
            var shortcuts = (Gtk.ShortcutsWindow) builder.get_object ("shortcuts");
            show_dialog (shortcuts);
        }

        private void help_action () {
            Command.open_redirect_uri ("help");
        }

        public void info_action () {
            show_info ();
        }

        public void about_action () {
            var developer_name = "Stephen Brandt";
            var about = new Adw.AboutWindow () {
                application_name   = APP_NAME,
                application_icon   = APP_ID,
                developer_name     = developer_name,
                // Translator credits. Put one translator per line, in the form of "NAME URL".
                translator_credits = _("translator-credits"),
                version            = VERSION,
                website            = "https://haguichi.net",
                issue_url          = "https://github.com/ztefn/haguichi/issues",
                copyright          = "© 2007-2024 " + developer_name,
                license_type       = Gtk.License.GPL_3_0,
            };
            show_dialog (about);
        }

        public void quit_action () {
            Controller.quit ();

            if (modal_dialog != null) {
                modal_dialog.destroy ();
            }

            hide ();

            connection.save_long_nicks ();
            session.quitted ();

            debug ("quit_action: Bye!");
            app.quit ();
        }

        public void show_dialog (Gtk.Window dialog) {
            dialog.transient_for = this;
            dialog.close_request.connect (() => {
                modal_dialog = null;
                session.modality_changed (false);
                update_indicator_status ();
                return false;
            });
            dialog.present ();

            modal_dialog = dialog;
            session.modality_changed (true);
            update_indicator_status ();
        }

        public void show_toast (string title, uint timeout = 5) {
            toast_overlay.add_toast (new Adw.Toast (title) {
                timeout = timeout
            });
        }

        public void show_copied_to_clipboard_toast () {
            show_toast (_("Copied to clipboard"), 2);
        }

        public void show_info () {
            network_list.unselect ();
            split_view.show_sidebar = true;
        }

        public void toggle_sidebar () {
            split_view.show_sidebar = !split_view.show_sidebar;
        }

        public void show_sidebar () {
            split_view.show_sidebar = (default_width > 520);
            split_view.pin_sidebar = false;
        }

        public void hide_sidebar () {
            split_view.show_sidebar = false;
            split_view.pin_sidebar = true;
        }

        public void set_nick (string nick) {
            window_title.title = (nick == "") ? _("Anonymous") : nick;
        }

        public void set_connected_stack_page (string page) {
            connected_stack.visible_child_name = page;
        }

        public string get_mode () {
            return stack.visible_child_name;
        }

        public void set_mode (string mode) {
            session.mode_changed (mode);

            stack.visible_child_name = mode;

            bool is_configured = mode.down ().contains ("connect");

            action_set_enabled ("win.connect",              mode == "Disconnected");
            action_set_enabled ("win.disconnect",           mode == "Connected");
            action_set_enabled ("win.join-network",         mode == "Connected");
            action_set_enabled ("win.create-network",       mode == "Connected");
            action_set_enabled ("win.start-search",         mode == "Connected");
            action_set_enabled ("win.refresh",              true);
            action_set_enabled ("win.toggle-sidebar",       is_configured);
            action_set_enabled ("win.info",                 is_configured);
            action_set_enabled ("win.sort-by",              is_configured);
            action_set_enabled ("win.show-offline-members", is_configured);

            refresh_button.visible = (mode == "Not installed" || mode == "Not configured");

            if (mode == "Not installed") {
                if (demo_mode || Hamachi.version == "") {
                    not_installed_status_page.title = _("Hamachi Is Not Installed");
                } else {
                    not_installed_status_page.title = _("Hamachi Version %s Is Obsolete").printf (Hamachi.version);
                }
            }

            connect_button.visible = (mode == "Disconnected");
            disconnect_button.visible = (mode == "Connecting" || mode == "Connected");

            search_button.visible = is_configured;
            search_button.sensitive = (mode == "Connected");
            search_bar.sensitive = (mode == "Connected");
            if (mode != "Connected") {
                search_bar.search_mode_enabled = false;
            }

            if (is_configured) {
                sidebar.attach_button.sensitive = (mode == "Connected");
                sidebar.cancel_button.sensitive = (mode == "Connected");
                show_sidebar ();
            } else {
                hide_sidebar ();
            }

            spinner.spinning = (mode == "Connecting");

            configure_button.sensitive = (mode == "Not configured");

            overlay_refresh_button.visible = (mode == "Connected");
            overlay_add_network_button.visible = (mode == "Connected");

            if (mode == "Initializing") {
                window_title.subtitle = _("Initializing");
            } else if (mode == "Connecting") {
                window_title.subtitle = _("Connecting");
            } else if (mode == "Connected") {
                window_title.subtitle = _("Connected");
            } else {
                window_title.subtitle = _("Disconnected");
                sidebar.show_page ("Information");
            }

            // Only sandboxed applications can set background status
            if (Xdp.Portal.running_under_flatpak ()) {
                set_background_status.begin ();
            }

            update_indicator_status ();
        }

        public async void set_background_status () {
            try {
                bool success = yield portal.set_background_status (window_title.subtitle, null);
                if (!success) {
                    warning ("set_background_status: Updating background status failed");
                }
            } catch (Error e) {
                critical ("set_background_status: %s", e.message);
            }
        }

        public void update_indicator_status () {
            if (indicator != null) {
                indicator.update_status (get_mode (), modal_dialog != null);
            }
        }

        public override void hide () {
            if (modal_dialog == null) {
                base.hide ();
            }
        }

        public override bool close_request () {
            // If connecting or connected request to run in background
            var mode = get_mode ();
            if (mode == "Connecting" || mode == "Connected") {
                request_background.begin ((obj, res) => {
                    if (request_background.end (res)) {
                        hide ();
                    } else {
                        destroy ();
                    }
                });
            } else {
                destroy ();
            }

            return true;
        }

        public async bool request_background () {
            var parent = Xdp.parent_new_gtk (this);

            var command = new GenericArray<weak string> ();
            command.add (APP_ID);
            command.add ("--background");

            try {
                return yield portal.request_background (
                    parent,
                    _("Haguichi will continue to run when its window is closed so that it can monitor the connection and send notifications."),
                    command,
                    Xdp.BackgroundFlags.NONE,
                    null
                );
            } catch (Error e) {
                critical ("request_background: %s", e.message);
                // If background portal is not available then return true to hide window
                return true;
            }
        }
    }
}
