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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/change-access.ui")]
    public class ChangeAccessDialog : Adw.Window {
        private Network network;

        [GtkChild]
        unowned Adw.SwitchRow locked;
        [GtkChild]
        unowned Adw.ComboRow approval;

        public ChangeAccessDialog (Network _network) {
            network = _network;

            locked.active      = network.lock_state == "locked";
            approval.sensitive = network.lock_state == "unlocked";
            approval.selected  = network.approve == "manual" ? 1 : 0;
        }

        [GtkCallback]
        private void change_access () {
            network.updating = true;
            network.lock_state = locked.active ? "locked" : "unlocked";
            network.approve = approval.selected == 1 ? "manual" : "auto";

            win.sidebar.set_network (network);

            new Thread<void*> (null, () => {
                Hamachi.set_access (network.id, locked.active ? "lock" : "unlock", approval.selected == 1 ? "manual" : "auto");
                network.updating = false;

                return null;
            });

            close ();
        }

        [GtkCallback]
        private void lock_changed () {
            approval.sensitive = !locked.active;
        }
    }
}
