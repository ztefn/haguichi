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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/change-password.ui")]
#if ADW_1_6
    public class ChangePasswordDialog : Adw.Dialog {
#else
    public class ChangePasswordDialog : Adw.Window {
#endif
        private Network network;

        [GtkChild]
        unowned Adw.EntryRow password_entry;

        public ChangePasswordDialog (Network _network) {
            network = _network;
#if ADW_1_6
            content_width = 400;
#else
            height_request = 100;
            resizable = false;
#endif
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
