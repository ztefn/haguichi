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
    public class ConfirmDialog : Object {
        public signal void confirm ();

        public ConfirmDialog (Gtk.Widget parent,
                              string heading,
                              string body,
                              string confirm_label,
                              Adw.ResponseAppearance response_appearance) {

            var dialog = new Adw.AlertDialog (heading, body);

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
                dialog.present (parent);
            }
        }
    }
}
