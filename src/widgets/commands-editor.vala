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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/commands-editor.ui")]
    public class CommandsEditor : Adw.PreferencesGroup {
        private Settings settings;
        private string[] default_commands;
        private List<CommandsEditorRow> rows;
        private CommandsEditorRow selected_row;
        private ConfirmDialog dialog;

        [GtkChild]
        unowned Gtk.Button restore_button;
        [GtkChild]
        unowned Gtk.ListBox list_box;

        construct {
            settings = new Settings (Config.APP_ID + ".commands");
            default_commands = (string[]) settings.get_default_value ("customizable");

            rows = new List<CommandsEditorRow> ();

            populate ();
            set_button_sensitivity (compose_commands_string ());
        }

        [GtkCallback]
        private void on_row_activated (Gtk.ListBoxRow row) {
            if (row.get_index () == (int) rows.length ()) {
                on_add_command ();
            } else {
                edit_command ((CommandsEditorRow) row);
            }
        }

        [GtkCallback]
        private void on_add_command () {
            new Haguichi.AddEditCommandDialog ("Add", this, "", "", "", "IPv4");
        }

        [GtkCallback]
        private void on_restore_commands () {
            dialog = new ConfirmDialog ((Gtk.Window) get_root (),
                                        _("Restore default commands?"),
                                        _("All customizations will be lost."),
                                        _("_Restore"),
                                        Adw.ResponseAppearance.DESTRUCTIVE);
            dialog.confirm.connect (() => {
                settings.set_strv ("customizable", default_commands);
                populate ();
                update_commands ();
            });
        }

        public void set_default (CommandsEditorRow default_row) {
            foreach (CommandsEditorRow row in rows) {
                row.set_default (row == default_row);
            }

            update_commands ();
        }

        public void move_command (CommandsEditorRow row, int new_index) {
            // Remove the row and insert at new index
            rows.remove (row);
            rows.insert (row, new_index);
            list_box.remove (row);
            list_box.insert (row, new_index);

            update_commands ();
        }

        public void remove_command (CommandsEditorRow row) {
            rows.remove (row);
            list_box.remove (row);

            update_commands ();
        }

        public void update_selected_command (string label, string command_ipv4, string command_ipv6, string priority) {
            selected_row.update (label, command_ipv4, command_ipv6, priority);

            update_commands ();
        }

        public void insert_command (string label, string command_ipv4, string command_ipv6, string priority) {
            var row = new CommandsEditorRow (this, true, false, label, command_ipv4, command_ipv6, priority);

            rows.append (row);
            list_box.insert (row, (int) rows.length () - 1);

            update_commands ();
        }

        public void edit_command (CommandsEditorRow row) {
            selected_row = row;
            new Haguichi.AddEditCommandDialog ("Edit", this, _(row.label), row.command_ipv4, row.command_ipv6, row.priority);
        }

        private void clear () {
            foreach (CommandsEditorRow row in rows) {
                list_box.remove (row);
            }

            rows = new List<CommandsEditorRow> ();
        }

        public void populate () {
            clear ();

            string[] commands = settings.get_strv ("customizable");

            var index = 0;
            foreach (string command in commands) {
                string[] parts = command.split (";", 6);

                if (parts.length == 6) {
                    bool is_active  = bool.parse (parts[0]);
                    bool is_default = bool.parse (parts[1]);

                    string command_ipv4 = parts[3];
                    string command_ipv6 = parts[4];
                    string priority     = parts[5];

                    command_ipv4 = command_ipv4.replace ("{COLON}", ";");
                    command_ipv6 = command_ipv6.replace ("{COLON}", ";");

                    var row = new CommandsEditorRow (this, is_active, is_default, parts[2], command_ipv4, command_ipv6, priority);
                    rows.append (row);

                    list_box.insert (row, index);
                    index ++;
                }
            }
        }

        public void update_commands () {
            var commands_string = compose_commands_string ();
            set_button_sensitivity (commands_string);
            settings.set_strv ("customizable", commands_string);

            Command.fill_custom_commands ();
        }

        private string[] compose_commands_string () {
            string[] commands_string = {};

            foreach (CommandsEditorRow row in rows) {
                commands_string += compose_command_string (row);
            }

            return commands_string;
        }

        private string compose_command_string (CommandsEditorRow row) {
            string command_ipv4 = row.command_ipv4.replace (";", "{COLON}");
            string command_ipv6 = row.command_ipv6.replace (";", "{COLON}");

            return row.is_active.to_string () + ";" +
                   row.is_default.to_string () + ";" +
                   row.label + ";" +
                   command_ipv4 + ";" +
                   command_ipv6 + ";" +
                   row.priority.to_string ();
        }

        private void set_button_sensitivity (string[] commands_string) {
            // Set restore button sensitive if the current commands are different than the default
            restore_button.sensitive = (string.joinv ("", commands_string) != string.joinv ("", default_commands));

            // Update enabled state for row actions
            var total = (int) rows.length () - 1;
            foreach (CommandsEditorRow row in rows) {
                var index = row.get_index ();
                row.action_set_enabled ("row.move-up",     index > 0);
                row.action_set_enabled ("row.move-down",   index < total);
                row.action_set_enabled ("row.set-default", !row.is_default);
            }
        }
    }
}
