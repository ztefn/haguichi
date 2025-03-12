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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/command-variable-button.ui")]
    public class CommandVariableButton : Gtk.Button {
        [GtkChild]
        unowned Gtk.Label title_label;
        [GtkChild]
        unowned Gtk.Label variable_label;

        public string title {
            get {
                return title_label.label;
            }
            set {
                title_label.label = value;
            }
        }

        public string variable {
            get {
                return variable_label.label;
            }
            set {
                variable_label.label = value;
            }
        }

        [GtkCallback]
        private void on_insert_value () {
            // Find menu button and close popover
            var menu_button = (Gtk.MenuButton) get_ancestor (typeof (Gtk.MenuButton));
            menu_button.popdown ();

            // Find entry and insert variable at cursor position
            var entry = (Adw.EntryRow) get_ancestor (typeof (Adw.EntryRow));
            var position = entry.cursor_position;
            entry.do_insert_text (variable, -1, ref position);
        }
    }
}
