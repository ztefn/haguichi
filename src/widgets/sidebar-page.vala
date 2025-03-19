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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/sidebar-page.ui")]
    public class SidebarPage : Gtk.Box, Gtk.Buildable {
        [GtkChild]
        unowned Gtk.Label label;
        [GtkChild]
        unowned Gtk.ListBox list_box;

#if FOR_ELEMENTARY
        construct {
            label.margin_start = 12;
            label.margin_end = 12;

            list_box.add_css_class ("rich-list");
        }
#endif

        public string heading {
            get {
                return label.label;
            }
            set {
                label.label = value;
            }
        }

        public bool heading_selectable {
            get {
                return label.selectable;
            }
            set {
                label.selectable = value;
            }
        }

        public void add_child (Gtk.Builder builder, Object child, string? type) {
            if (type == "row") {
                list_box.add_child (builder, child, type);
                return;
            }

            base.add_child (builder, child, type);
        }
    }
}
