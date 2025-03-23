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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/add-edit-command.ui")]
    public class AddEditCommandDialog : Adw.Dialog {
        private string mode;
        private CommandsEditor editor;

        [GtkChild]
        unowned Gtk.Button save_button;

        [GtkChild]
        unowned Adw.EntryRow label_entry;

        [GtkChild]
        unowned CommandEntryRow command_ipv4_entry;
        [GtkChild]
        unowned CommandEntryRow command_ipv6_entry;

        [GtkChild]
        unowned Adw.ActionRow priority;
#if ADW_1_7
        private Adw.ToggleGroup priority_group;
        private Adw.Toggle priority_ipv4;
        private Adw.Toggle priority_ipv6;
#else
        private Gtk.CheckButton priority_ipv4;
        private Gtk.CheckButton priority_ipv6;
#endif

        construct {
#if ADW_1_7
            priority_ipv4 = new Adw.Toggle () {
                label = name = "IPv4"
            };
            priority_ipv6 = new Adw.Toggle () {
                label = name = "IPv6"
            };

            priority_group = new Adw.ToggleGroup () {
                valign = Gtk.Align.CENTER
            };
            priority_group.add (priority_ipv4);
            priority_group.add (priority_ipv6);

            priority.add_suffix (priority_group);
#else
            priority_ipv4 = new Gtk.CheckButton () {
                label  = "IPv4",
                valign = Gtk.Align.CENTER
            };
            priority_ipv6 = new Gtk.CheckButton () {
                group  = priority_ipv4,
                label  = "IPv6",
                valign = Gtk.Align.CENTER
            };

            priority.add_suffix (priority_ipv4);
            priority.add_suffix (priority_ipv6);
#endif
        }

        public AddEditCommandDialog (string _mode, CommandsEditor _editor, string label, string command_ipv4, string command_ipv6, string priority) {
            mode   = _mode;
            editor = _editor;

            title = (mode == "Edit") ? _("Edit Command") : _("Add Command");

            label_entry.text = label;

            command_ipv4_entry.text = command_ipv4;
            command_ipv6_entry.text = command_ipv6;

#if ADW_1_7
            priority_group.active_name = priority;
#else
            priority_ipv4.active = (priority == "IPv4");
            priority_ipv6.active = (priority == "IPv6");
#endif

            set_state ();

            present (_editor);
        }

        [GtkCallback]
        private void save_command () {
#if ADW_1_7
            var ip_mode = priority_group.active_name;
#else
            var ip_mode = priority_ipv6.active ? "IPv6" : "IPv4";
#endif
            if (mode == "Edit") {
                editor.update_selected_command (label_entry.text, command_ipv4_entry.text, command_ipv6_entry.text, ip_mode);
            } else {
                editor.insert_command (label_entry.text, command_ipv4_entry.text, command_ipv6_entry.text, ip_mode);
            }

            close ();
        }

        [GtkCallback]
        private void set_state () {
            var ipv4 = command_ipv4_entry.text.length;
            var ipv6 = command_ipv6_entry.text.length;
#if ADW_1_7
            priority_ipv4.enabled = ipv4 > 0;
            priority_ipv6.enabled = ipv6 > 0;
#else
            priority_ipv4.sensitive = ipv4 > 0;
            priority_ipv6.sensitive = ipv6 > 0;
#endif
            if (ipv4 == 0 && ipv6 > 0) {
#if ADW_1_7
                priority_group.active_name = "IPv6";
#else
                priority_ipv6.active = true;
#endif
            } else if (ipv4 > 0 && ipv6 == 0) {
#if ADW_1_7
                priority_group.active_name = "IPv4";
#else
                priority_ipv4.active = true;
#endif
            }

            save_button.sensitive = label_entry.text.length > 0 && (ipv4 > 0 || ipv6 > 0);
        }
    }
}
