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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/sidebar-row.ui")]
    public class SidebarRow : Adw.ActionRow, Gtk.Buildable {
        [GtkChild]
        unowned Gtk.Button copy_button;

#if FOR_ELEMENTARY
        private Gtk.Widget action_button;
#endif

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

        [GtkCallback]
        private void on_copy_value () {
            win.get_clipboard ().set_text (subtitle);
            win.show_copied_to_clipboard_toast ();
        }

#if FOR_ELEMENTARY
        // Intercept added action buttons to replace the icons with non-symbolic variant
        public void add_child (Gtk.Builder builder, Object child, string? type) {
            if (child is Gtk.Button || child is Gtk.MenuButton) {
                action_button = (Gtk.Widget) child;
                icon_updated ();
            }

            base.add_child (builder, child, type);
        }

        private void icon_updated () {
            // Disconnect existing signal first to prevent an infinite update loop
            action_button.notify["icon-name"].disconnect (icon_updated);

            // Unfortunately Gtk.MenuButton is not subclassed from Gtk.Button
            // so we end up with this duplicate code for both classes...
            if (action_button is Gtk.Button) {
                var button = (Gtk.Button) action_button;
                if (button.icon_name != null) {
                    button.icon_name = button.icon_name.replace ("-symbolic", "");
                }
            } else if (action_button is Gtk.MenuButton) {
                var button = (Gtk.MenuButton) action_button;
                if (button.icon_name != null) {
                    button.icon_name = button.icon_name.replace ("-symbolic", "");
                }
            }

            // Connect signal (again) to listen for icon name changes
            action_button.notify["icon-name"].connect (icon_updated);
        }
#endif
    }
}
