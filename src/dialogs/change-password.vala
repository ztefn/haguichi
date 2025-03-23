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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/change-password.ui")]
    public class ChangePasswordDialog : Adw.Dialog {
        private Network network;

        [GtkChild]
        unowned Adw.EntryRow password_entry;

        public ChangePasswordDialog (Network _network) {
            network = _network;
        }

        [GtkCallback]
        private void change_password () {
            new Thread<void*> (null, () => {
                Hamachi.set_password (network.id, password_entry.text);

                return null;
            });

            close ();
        }
    }
}
