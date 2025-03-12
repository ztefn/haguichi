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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/command-entry-row.ui")]
    public class CommandEntryRow : Adw.EntryRow {
        [GtkChild]
        public unowned Gtk.MenuButton menu_button;

        [GtkChild]
        public unowned CommandVariableButton address_var;
        [GtkChild]
        public unowned CommandVariableButton nickname_var;
        [GtkChild]
        public unowned CommandVariableButton client_id_var;
        [GtkChild]
        public unowned CommandVariableButton terminal_var;
        [GtkChild]
        public unowned CommandVariableButton file_manager_var;
        [GtkChild]
        public unowned CommandVariableButton remote_desktop_var;
    }
}
