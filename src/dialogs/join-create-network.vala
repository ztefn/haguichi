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
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/dialogs/join-create-network.ui")]
    public class JoinCreateNetworkDialog : Adw.Window {
        private string mode;
        private Adw.Toast toast;

        [GtkChild]
        unowned Adw.ToastOverlay toast_overlay;

        [GtkChild]
        unowned Gtk.Button cancel_button;
        [GtkChild]
        unowned Gtk.Button add_button;

        [GtkChild]
        unowned Adw.EntryRow id_entry;
        [GtkChild]
        unowned Adw.PasswordEntryRow password_entry;

        public JoinCreateNetworkDialog (string _mode) {
            mode  = _mode;
            title = (mode == "Join") ? _("Join Network") : _("Create Network");

            add_button.label = (mode == "Join") ? _("_Join") : _("C_reate");
        }

        [GtkCallback]
        private void add_network () {
            set_buttons_sensitivity (false);
            if (mode == "Join") {
                new Thread<void*> (null, go_join_thread);
            } else {
                new Thread<void*> (null, go_create_thread);
            }
        }

        private void* go_join_thread () {
            string id       = id_entry.text;
            string password = password_entry.text;
            string output;

            if (demo_mode) {
                if (id == "not found") {
                    output = ".. failed, network not found";
                } else if (id == "invalid password") {
                    output = ".. failed, invalid password";
                } else if (id == "full") {
                    output = ".. failed, the network is full";
                } else if (id == "joined") {
                    output = ".. failed, you are already a member";
                } else if (id == "failed") {
                    output = ".. failed";
                } else  {
                    output = ".. ok, request sent";
                }
            } else {
                output = Hamachi.join_network (id, password);
            }

            if (output.contains (".. ok, request sent")) {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    win.show_toast (_("Join request sent"));
                    close ();
                    return false;
                });
            } else if (output.contains (".. ok")) {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    close ();
                    return false;
                });

                // Wait a second to get an updated list
                Thread.usleep (1000000);

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    win.network_list.select_network_id = id;
                    Controller.update_connection ();
                    return false;
                });
            } else {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    set_buttons_sensitivity (true);

                    if (output.contains (".. failed, network not found")) {
                        id_entry.add_css_class ("error");
                        id_entry.grab_focus_without_selecting ();
                        show_toast (_("Network not found"));
                    } else if (output.contains (".. failed, invalid password")) {
                        password_entry.add_css_class ("error");
                        password_entry.grab_focus_without_selecting ();
                        show_toast (_("Invalid password"));
                    } else if (output.contains (".. failed, the network is full")) {
                        show_toast (_("Network is full"));
                    } else if (output.contains (".. failed, network is locked")) {
                        show_toast (_("Network is locked"));
                    } else if (output.contains (".. failed, you are already a member") ||
                               output.contains (".. failed, you are an owner")) {
                        show_toast (_("Network already joined"));
                    } else {
                        show_toast (output.strip ());
                    }

                    return false;
                });
            }

            return null;
        }

        private void* go_create_thread () {
            string id       = id_entry.text;
            string password = password_entry.text;
            string output;

            if (demo_mode) {
                if (id == "taken") {
                    output = ".. failed, network name is already taken";
                } else if (id == "failed") {
                    output = ".. failed";
                } else  {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        close ();

                        Network new_network = new Network (new Status ("*"), id, id, "This computer", 5);

                        win.network_list.select_network_id = id;
                        connection.add_network (new_network);
                        Controller.update_connection ();

                        return false;
                    });

                    return null;
                }
            } else {
                output = Hamachi.create_network (id, password);
            }

            if (output.contains (".. ok")) {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    close ();
                    return false;
                });

                // Wait a second to get an updated list
                Thread.usleep (1000000);

                Idle.add_full (Priority.HIGH_IDLE, () => {
                    win.network_list.select_network_id = id;
                    Controller.update_connection ();
                    return false;
                });
            } else {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    set_buttons_sensitivity (true);

                    if (output.contains (".. failed, network name is already taken")) {
                        id_entry.add_css_class ("error");
                        id_entry.grab_focus_without_selecting ();
                        show_toast (_("Network ID is already taken"));
                    } else {
                        show_toast (output.strip ());
                    }

                    return false;
                });
            }

            return null;
        }

        [GtkCallback]
        private void entry_changed () {
            dismiss_toast ();

            id_entry.remove_css_class ("error");
            password_entry.remove_css_class ("error");

            // Network name must be between 4 and 64 characters long
            add_button.sensitive = (id_entry.text.length >= 4 && id_entry.text.length <= 64);
        }

        private void set_buttons_sensitivity (bool sensitive) {
            cancel_button.sensitive = sensitive;
            add_button.sensitive = sensitive;
        }

        private void show_toast (string title) {
            dismiss_toast ();
            toast = new Adw.Toast (title);
            toast_overlay.add_toast (toast);
        }

        private void dismiss_toast () {
            if (toast != null) {
                toast.dismiss ();
            }
        }
    }
}
