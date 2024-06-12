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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/command-entry-row.ui")]
    public class CommandEntryRow : Adw.EntryRow {
        [GtkChild]
        public unowned CommandVariableBox address_var;
        [GtkChild]
        public unowned CommandVariableBox nickname_var;
        [GtkChild]
        public unowned CommandVariableBox client_id_var;
        [GtkChild]
        public unowned CommandVariableBox terminal_var;
        [GtkChild]
        public unowned CommandVariableBox file_manager_var;
        [GtkChild]
        public unowned CommandVariableBox remote_desktop_var;
    }
}
