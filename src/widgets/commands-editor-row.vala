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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/commands-editor-row.ui")]
    public class CommandsEditorRow : Adw.ActionRow {
        private CommandsEditor editor;

        private double drag_x = 0;
        private double drag_y = 0;

        public bool   is_active;
        public bool   is_default;
        public string label;
        public string command_ipv4;
        public string command_ipv6;
        public string priority;

        [GtkChild]
        unowned Gtk.Image drag_handle;
        [GtkChild]
        unowned Gtk.Image default_emblem;
        [GtkChild]
        unowned Gtk.Switch command_switch;

        construct {
            install_action ("row.edit",        null, (Gtk.WidgetActionActivateFunc) on_edit);
            install_action ("row.duplicate",   null, (Gtk.WidgetActionActivateFunc) on_duplicate);
            install_action ("row.move-up",     null, (Gtk.WidgetActionActivateFunc) on_move_up);
            install_action ("row.move-down",   null, (Gtk.WidgetActionActivateFunc) on_move_down);
            install_action ("row.set-default", null, (Gtk.WidgetActionActivateFunc) on_set_deault);
            install_action ("row.remove",      null, (Gtk.WidgetActionActivateFunc) on_remove);

            drag_handle.icon_name = Utils.get_available_theme_icon ({
                "list-drag-handle-symbolic",
                "drag-handle-symbolic"
            });

            default_emblem.icon_name = Utils.get_available_theme_icon ({
                "object-select-symbolic",
                "emblem-default-symbolic",
                "emblem-ok-symbolic",
                "selection-mode-symbolic"
            });
        }

        public CommandsEditorRow (CommandsEditor _editor, bool _is_active, bool _is_default, string _label, string _command_ipv4, string _command_ipv6, string _priority) {
            editor     = _editor;
            is_active  = _is_active;
            is_default = _is_default;

            command_switch.active = is_active;
            command_switch.state_set.connect ((state) => {
                is_active = state;
                editor.update_commands ();
                return false;
            });

            update (_label, _command_ipv4, _command_ipv6, _priority);
            set_default (is_default);
        }

        public void on_edit () {
            editor.edit_command (this);
        }

        public void on_duplicate () {
            editor.duplicate_command (this);
        }

        public void on_move_up () {
            editor.move_command (this, get_index () - 1);
        }

        public void on_move_down () {
            editor.move_command (this, get_index () + 1);
        }

        public void on_set_deault () {
            editor.set_default (this);
        }

        public void on_remove () {
            editor.remove_command (this);
        }

        [GtkCallback]
        public Gdk.ContentProvider on_drag_prepare (double x, double y) {
            drag_x = x;
            drag_y = y;

            return new Gdk.ContentProvider.for_value (this);
        }

        [GtkCallback]
        public void on_drag_begin (Gtk.DragSource source, Gdk.Drag drag) {
            drag.set_hotspot ((int) drag_x, (int) drag_y);

            var drag_row = new CommandsEditorRow (editor, is_active, is_default, label, command_ipv4, command_ipv6, priority);
            // Property 'use_underline' with value 'true' triggers a segmentation fault here,
            // so turn it off and remove underscores manually
            drag_row.use_underline = false;
            drag_row.title = _(label).replace ("_", "");

            var drag_widget = new Gtk.ListBox ();
            drag_widget.set_size_request (get_width (), get_height ());
#if FOR_ELEMENTARY
            // Add "boxed-list" class to get some styling on our dragged row
            drag_widget.add_css_class ("boxed-list");
#endif
            drag_widget.append (drag_row);
            drag_widget.drag_highlight_row (drag_row);

            var icon = Gtk.DragIcon.get_for_drag (drag) as Gtk.DragIcon;
            icon.child = drag_widget;
        }

        [GtkCallback]
        public bool on_drop (Gtk.DropTarget target, GLib.Value val, double x, double y) {
            var row = (CommandsEditorRow) val;

            if (row == this) {
                // Do nothing if dropped onto itself
                return false;
            } else {
                // Move dropped row to index of this
                editor.move_command (row, get_index ());
                return true;
            }
        }

        public void update (string _label, string _command_ipv4, string _command_ipv6, string _priority) {
            label        = _label;
            command_ipv4 = _command_ipv4;
            command_ipv6 = _command_ipv6;
            priority     = _priority;

            string command = (priority == "IPv6") ? command_ipv6 : command_ipv4;
            string address = (priority == "IPv6") ? "2620:9b::56d:f78e" : "25.123.456.78";

            title    = _(_label);
            subtitle = Command.replace_variables (command, address, "Nick", "090-123-456");
        }

        public void set_default (bool _is_default) {
            is_default = _is_default;
            default_emblem.visible = is_default;
        }
    }
}
