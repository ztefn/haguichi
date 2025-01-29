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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/sidebar.ui")]
    public class Sidebar : Adw.Bin {
        private Member member;
        private Network network;

        [GtkChild]
        private unowned Gtk.Stack stack;

        [GtkChild]
        private unowned SidebarPage network_page;
        [GtkChild]
        private unowned SidebarPage member_page;

        [GtkChild]
        unowned SidebarRow account_row;
        [GtkChild]
        unowned SidebarRow client_id_row;
        [GtkChild]
        unowned SidebarRow client_ipv4_row;
        [GtkChild]
        unowned SidebarRow client_ipv6_row;
        [GtkChild]
        unowned SidebarRow hamachi_row;

        [GtkChild]
        unowned SidebarRow network_status_row;
        [GtkChild]
        unowned SidebarRow network_id_row;
        [GtkChild]
        unowned SidebarRow network_owner_row;
        [GtkChild]
        unowned SidebarRow network_members_row;
        [GtkChild]
        unowned SidebarRow network_capacity_row;

        [GtkChild]
        unowned SidebarRow member_status_row;
        [GtkChild]
        unowned SidebarRow member_id_row;
        [GtkChild]
        unowned SidebarRow member_ipv4_row;
        [GtkChild]
        unowned SidebarRow member_ipv6_row;
        [GtkChild]
        unowned SidebarRow member_tunnel_row;
        [GtkChild]
        unowned SidebarRow member_connection_row;

        [GtkChild]
        public unowned Gtk.Button attach_button;
        [GtkChild]
        public unowned Gtk.Button cancel_button;

        [GtkChild]
        public unowned Gtk.Button network_leave_button;
        [GtkChild]
        public unowned Gtk.Button network_delete_button;
        [GtkChild]
        public unowned Gtk.Button network_password_button;
        [GtkChild]
        public unowned Gtk.Button network_access_button;
        [GtkChild]
        public unowned Gtk.Button network_go_online_button;
        [GtkChild]
        public unowned Gtk.Button network_go_offline_button;

        [GtkChild]
        public unowned Gtk.Button member_approve_button;
        [GtkChild]
        public unowned Gtk.Button member_reject_button;
        [GtkChild]
        public unowned Gtk.Button member_evict_button;

        [GtkChild]
        private unowned Gtk.MenuButton member_command_menubutton;

        construct {
            // Setup member command menubutton
            member_command_menubutton.menu_model = Command.get_commands_menu ();
            Command.get_commands_menu ().items_changed.connect (() => {
                set_member_command_menubutton_visible ();
                if (member != null) {
                    Command.set_active_commands_enabled (member);
                }
            });

            // Use different leave icon for Yaru and elementary themes
            var theme_name = Utils.get_icon_theme ().theme_name;
            if (theme_name.has_prefix ("Yaru")) {
                network_leave_button.icon_name = "system-log-out-symbolic";
            } else if (theme_name == "elementary") {
                network_leave_button.icon_name = "system-log-out";
            }
        }

        [GtkCallback]
        private void on_network_leave () {
            network.leave ();
        }

        [GtkCallback]
        private void on_network_delete () {
            network.delete ();
        }

        [GtkCallback]
        private void on_network_change_password () {
            network.change_password ();
        }

        [GtkCallback]
        private void on_network_change_access () {
            network.change_access ();
        }

        [GtkCallback]
        private void on_network_go_online () {
            network.go_online ();
        }

        [GtkCallback]
        private void on_network_go_offline () {
            network.go_offline ();
        }

        [GtkCallback]
        private void on_member_approve () {
            member.approve ();
        }

        [GtkCallback]
        private void on_member_reject () {
            member.reject ();
        }

        [GtkCallback]
        private void on_member_evict () {
            member.evict ();
        }

        private void set_member_command_menubutton_visible () {
            member_command_menubutton.visible = member != null &&
                                                member.status.status_int != 3 &&
                                                Command.get_commands_menu ().get_n_items () > 0;
        }

        private void set_version () {
            string ver = Hamachi.version;

            if (ver == null) {
                ver = _("Unavailable");
            }

            hamachi_row.set_value (ver);
        }

        private void set_address () {
            string[] address = Hamachi.get_address ();

            client_ipv4_row.set_value (address[0]);
            client_ipv6_row.set_value (address[1]);
        }

        private void set_client_id () {
            client_id_row.set_value (Hamachi.get_client_id ());
        }

        public void set_account (string account) {
            account_row.title = account.has_suffix (" (pending)") ? "%s (%s)".printf (_("Account"), _("pending")) : _("Account");
            account_row.set_value ((account == "" || account == "-") ? _("Not attached") : account.replace (" (pending)", ""));
            attach_button.visible = (account == "" || account == "-");
            cancel_button.visible = account.has_suffix (" (pending)");
        }

        public void set_network (Network _network) {
            network = _network;

            int total, online;
            network.return_member_count (out total, out online);

            network_page.heading              = network.name;

            network_status_row.set_value      (network.status.status_text);
            network_id_row.set_value          (network.id);
            network_owner_row.set_value       (network.return_owner_string ());
            network_members_row.set_value     (_("{0} online, {1} total").replace ("{0}", online.to_string ()).replace ("{1}", total.to_string ()));
            network_capacity_row.set_value    (network.capacity.to_string ());

            network_owner_row.show_copy       = network.is_owner == 0;
            network_leave_button.visible      = network.is_owner == 0;
            network_delete_button.visible     = network.is_owner == 1;
            network_password_button.visible   = network.is_owner == 1;
            network_access_button.visible     = network.is_owner == 1;
            network_go_online_button.visible  = network.status.status_int == 0;
            network_go_offline_button.visible = network.status.status_int == 1;

            if (network.is_owner == 1) {
                network_access_button.remove_css_class ("success");
                network_access_button.remove_css_class ("warning");
                network_access_button.remove_css_class ("error");
                network_access_button.add_css_class (network.lock_state == "locked" ? "error" : network.approve == "manual" ? "warning" : "success");
                network_access_button.icon_name = network.lock_state == "locked" ? "changes-prevent-symbolic" : "changes-allow-symbolic";
            }

            show_page ("Network");
        }

        public void set_member (Member _member) {
            member = _member;

            member_page.heading              = member.nick;

            member_status_row.set_value      (member.status.status_text);
            member_id_row.set_value          (member.id);
            member_ipv4_row.set_value        (member.ipv4);
            member_ipv6_row.set_value        (member.ipv6);
            member_tunnel_row.set_value      (member.tunnel);
            member_connection_row.set_value  (member.status.connection_type);

            member_approve_button.visible    = member.network.is_owner == 1 && member.status.status_int == 3;
            member_reject_button.visible     = member.network.is_owner == 1 && member.status.status_int == 3;
            member_evict_button.visible      = member.network.is_owner == 1 && member.status.status_int != 3;

            set_member_command_menubutton_visible ();
            Command.set_active_commands_enabled (member);

            show_page ("Member");
        }

        public void update () {
            set_account (Hamachi.get_account ());
            set_version ();
            set_address ();
            set_client_id ();

            if (stack.visible_child_name == "Network") {
                set_network (network);
            } else if (stack.visible_child_name == "Member") {
                set_member (member);
            }
        }

        public void show_page (string page) {
            stack.visible_child_name = page;
        }
    }
}
