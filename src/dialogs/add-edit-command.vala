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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/add-edit-command.ui")]
#if ADW_1_6
    public class AddEditCommandDialog : Adw.Dialog {
#else
    public class AddEditCommandDialog : Adw.Window {
#endif
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
        unowned Gtk.CheckButton priority_ipv4;
        [GtkChild]
        unowned Gtk.CheckButton priority_ipv6;

        public AddEditCommandDialog (string _mode, CommandsEditor _editor, string label, string command_ipv4, string command_ipv6, string priority) {
            mode   = _mode;
            editor = _editor;

            title = (mode == "Edit") ? _("Edit Command") : _("Add Command");

            label_entry.text = label;

            command_ipv4_entry.text = command_ipv4;
            command_ipv6_entry.text = command_ipv6;

            priority_ipv4.active = (priority == "IPv4");
            priority_ipv6.active = (priority == "IPv6");

            set_state ();
#if ADW_1_6
            content_width = 440;
            present (_editor);
#else
            default_width = 440;
            modal = true;
            resizable = false;
            transient_for = (Gtk.Window) _editor.get_root ();
            present ();
#endif
        }

        [GtkCallback]
        private void save_command () {
            if (mode == "Edit") {
                editor.update_selected_command (label_entry.text, command_ipv4_entry.text, command_ipv6_entry.text, (priority_ipv6.active) ? "IPv6" : "IPv4");
            } else {
                editor.insert_command (label_entry.text, command_ipv4_entry.text, command_ipv6_entry.text, (priority_ipv6.active) ? "IPv6" : "IPv4");
            }

            close ();
        }

        [GtkCallback]
        private void set_state () {
            var ipv4 = command_ipv4_entry.text.length;
            var ipv6 = command_ipv6_entry.text.length;

            priority_ipv4.sensitive = ipv4 > 0;
            priority_ipv6.sensitive = ipv6 > 0;

            if (ipv4 == 0 && ipv6 > 0) {
                priority_ipv6.active = true;
            } else if (ipv4 > 0 && ipv6 == 0) {
                priority_ipv4.active = true;
            }

            save_button.sensitive = label_entry.text.length > 0 && (ipv4 > 0 || ipv6 > 0);
        }
    }
}
