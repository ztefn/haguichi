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
    public class ConfirmDialog : Object {
        public signal void confirm ();

        public ConfirmDialog (Gtk.Window parent,
                              string heading,
                              string body,
                              string confirm_label,
                              Adw.ResponseAppearance response_appearance) {
#if ADW_1_6
            var dialog = new Adw.AlertDialog (heading, body);
#else
            var dialog = new Adw.MessageDialog (parent, heading, body);
#endif
            dialog.add_response ("cancel", _("_Cancel"));
            dialog.add_response ("confirm", confirm_label);

            dialog.set_response_appearance ("confirm", response_appearance);

            dialog.default_response = "confirm";
            dialog.close_response   = "cancel";

            dialog.response.connect ((dialog, response) => {
                if (response == "confirm") {
                    confirm ();
                }
            });

            if (parent == win) {
                win.show_dialog (dialog);
            } else {
#if ADW_1_6
                dialog.present (parent);
#else
                dialog.present ();
#endif
            }
        }
    }
}
