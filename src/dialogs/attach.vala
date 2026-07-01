/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2026 Stephen Brandt <stephen@stephenbrandt.com>
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
    public class AttachDialog : Adw.Dialog {
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

        [GtkCallback]
        private void attach () {
            set_button_sensitivity (false);

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
                        close ();
                        win.show_toast (_("Attach request sent"));
                    } else {
                        set_button_sensitivity (true);

                        if (output.contains (".. failed, not found") ||
                            output.contains (".. failed, [248]")) {
                            set_entry_error (account_entry, true);
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
            set_entry_error (account_entry, false);
            set_button_sensitivity (account_entry.text.length > 0);
        }

        private void set_button_sensitivity (bool sensitive) {
            attach_button.sensitive = sensitive;
        }

        public static void set_entry_error (Adw.EntryRow entry, bool has_error) {
            entry.update_state (Gtk.AccessibleState.INVALID, has_error);

            if (has_error) {
                entry.add_css_class ("error");
                entry.grab_focus_without_selecting ();
            } else {
                entry.remove_css_class ("error");
            }
        }

        private void show_message (string message) {
            if (message != "") {
                revealer.reveal_child = true;
                message_label.label = message;
                announce (message, Gtk.AccessibleAnnouncementPriority.HIGH);
            }
        }

        private void dismiss_message () {
            revealer.reveal_child = false;
        }
    }
}
