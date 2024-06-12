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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/widgets/command-variable-box.ui")]
    public class CommandVariableBox : Gtk.Box {
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
    }
}
