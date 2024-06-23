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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/sidebar-row.ui")]
    public class SidebarRow : Adw.ActionRow {
        [GtkChild]
        unowned Gtk.Button copy_button;

        construct {
            install_action ("row.copy-value", null, (Gtk.WidgetActionActivateFunc) on_copy_value);
        }

        public void set_value (string? value) {
            subtitle = value;
            visible = (value != null && value != "");
        }

        public bool show_copy {
            get {
                return copy_button.visible;
            }
            set {
                copy_button.visible = value;
            }
        }

        public void on_copy_value () {
            win.get_clipboard ().set_text (subtitle);
            win.show_copied_to_clipboard_toast ();
        }
    }
}
