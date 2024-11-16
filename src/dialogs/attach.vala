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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/attach.ui")]
#if ADW_1_6
    public class AttachDialog : Adw.Dialog {
#else
    public class AttachDialog : Adw.Window {
#endif
        [GtkChild]
        unowned Gtk.Revealer revealer;
        [GtkChild]
        unowned Gtk.Label message_label;

        [GtkChild]
        unowned Gtk.Button attach_button;

        [GtkChild]
        unowned Adw.EntryRow account_entry;
        [GtkChild]
        unowned Adw.SwitchRow include_networks_switch;

        public AttachDialog () {
#if !ADW_1_6
            resizable = false;
#endif
        }

        [GtkCallback]
        private void attach () {
            set_buttons_sensitivity (false);

            new Thread<void*> (null, () => {
                string account = account_entry.text;
                string output;

                if (demo_mode) {
                    if (account.contains ("@")) {
                        output = ".. ok";
                        Hamachi.demo_account = account + " (pending)";
                    } else if (account.contains ("#")) {
                        output = ".. failed";
                    } else if (account.contains ("!")) {
                        output = "";
                    } else {
                        output = ".. failed, not found";
                    }
                } else {
                    output = Hamachi.attach (account, include_networks_switch.active);
                }

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    if (output.contains (".. ok")) {
                        win.sidebar.set_account (account + " (pending)");
                        win.show_toast (_("Attach request sent"));
                        close ();
                    } else {
                        set_buttons_sensitivity (true);

                        if (output.contains (".. failed, not found") ||
                            output.contains (".. failed, [248]")) {
                            account_entry.add_css_class ("error");
                            account_entry.grab_focus_without_selecting ();
                            show_message (_("Account not found"));
                        } else {
                            show_message (output.strip ());
                        }
                    }

                    return false;
                });

                return null;
            });
        }

        [GtkCallback]
        private void entry_changed () {
            dismiss_message ();
            account_entry.remove_css_class ("error");
            attach_button.sensitive = account_entry.text.length > 0;
        }

        private void set_buttons_sensitivity (bool sensitive) {
            attach_button.sensitive = sensitive;
        }

        private void show_message (string message) {
            if (message != "") {
                revealer.reveal_child = true;
                message_label.label = message;
            }
        }

        private void dismiss_message () {
            revealer.reveal_child = false;
        }
    }
}
