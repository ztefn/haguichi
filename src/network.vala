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
    public class Network : Object {
        private ConfirmDialog dialog;

        public List<Member> members;
        public ListStore    store;

        public Status   status;
        public string   id;
        public string   name;
        public int      is_owner;
        public string   owner;
        public string   lock_state;
        public string   approve;
        public int      capacity;

        public bool     hide_expander     { get; private set; }
        public string   label             { get; private set; }
        public string[] node_css_classes  { get; private set; }
        public string[] label_css_classes { get; private set; }

        public string   name_sort_string;
        public string   status_sort_string;

        public bool     updating;

        public Network (Status _status, string _id, string? _name, string? _owner, int? _capacity) {
            status     = _status;
            id         = _id;
            name       = _name;
            members    = new List<Member> ();
            store      = new ListStore (typeof (Member));
            is_owner   = -1;
            owner      = _owner;
            lock_state = "";
            approve    = "";
            capacity   = _capacity;

            show_hide_expander ();
            set_label_markup ();
            set_row_css_classes ();
            set_sort_strings ();
        }

        public void init () {
            determine_ownership ();
        }

        public void update (Status _status, string? _name) {
            // Check updating flag to prevent the background update process from overriding a very recent change
            if (!updating) {
                status = _status;
                name   = _name;

                set_row_css_classes ();
                set_sort_strings ();
            }
        }

        private void show_hide_expander () {
            hide_expander = store.n_items == 0;
        }

        public void set_label_markup () {
            string id   = Markup.escape_text (id).replace   ("%", "{PERCENTSIGN}");
            string name = Markup.escape_text (name).replace ("%", "{PERCENTSIGN}");

            string template = win.network_list.network_label_template;
            template = template.replace ("%ID",  id);
            template = template.replace ("%N",   name);
            template = template.replace ("%S",   status.status_text);
            template = template.replace ("%CAP", capacity.to_string ());
            template = template.replace ("<br>", "\n");

            if (template.contains ("%T") || template.contains ("%O")) {
                int member_count;
                int member_online_count;
                return_member_count (out member_count, out member_online_count);

                template = template.replace ("%T", member_count.to_string ());
                template = template.replace ("%O", member_online_count.to_string ());
            }

            if (is_owner == 1) {
                template = template.replace ("%*",  "★");
                template = template.replace ("%_*", " ★");
                template = template.replace ("%*_", "★ ");
            } else {
                template = template.replace ("%*",  "");
                template = template.replace ("%_*", "");
                template = template.replace ("%*_", "");
            }

            lock (label) {
                label = template.replace ("{PERCENTSIGN}", "%");
            }
        }

        public void set_row_css_classes () {
            node_css_classes  = new string[] {"network-node", status.get_css_classes ()};
            label_css_classes = new string[] {status.status_int == 0 ? "dim-label" : ""};
        }

        public void set_sort_strings () {
            name_sort_string   = name;
            status_sort_string = status.status_sortable + name;
        }

        public void return_member_count (out int total_count, out int online_count) {
            total_count  = 1; // Include client itself
            online_count = status.status_int; // Include client itself if online

            foreach (Member member in members) {
                total_count ++;

                if (member.status.status_int > 0 && member.status.status_int < 3) {
                    online_count ++;
                }
            }
        }

        public Member? return_member_by_id (string id) {
            foreach (Member member in members) {
                if (member.id == id) {
                    return member;
                }
            }

            return null;
        }

        public string return_owner_string () {
            string _owner;

            if (is_owner == 1) {
                _owner = _("You");
            } else if (owner != null) {
                _owner = owner;

                foreach (Member member in members) {
                    if (member.id == _owner) {
                        _owner = member.nick;
                    }
                }
            } else {
                _owner = _("Unknown");
            }

            return _owner;
        }

        private void determine_ownership () {
            try {
                network_threads.add (this);
            } catch (ThreadError e) {
                critical ("determine_ownership: %s", e.message);
            }
        }

        public void determine_ownership_thread () {
            if (owner == "This computer") {
                is_owner = 1;

                string output;

                if (demo_mode) {
                    output = """
id       : %s
name     : %s
type     : Mesh
owner    : This computer
status   : unlocked
approve  : auto""".printf (id, name);
                } else {
                    output = Command.return_output ("hamachi network \"%s\"".printf (Utils.clean_string (id)));
                }
                debug ("determine_ownership_thread: %s", output);

                lock_state = Hamachi.retrieve (output, "status");
                approve = Hamachi.retrieve (output, "approve");
            } else {
                is_owner = 0;

                try {
                    MatchInfo mi;
                    new Regex ("""^.*? \((?<id>[0-9-]{11})\)$""").match (owner, 0, out mi);

                    string id = mi.fetch_named ("id");

                    if (id != null) {
                        owner = id;
                        foreach (Member member in members) {
                            if (member.id == owner) {
                                Idle.add_full (Priority.HIGH_IDLE, () => {
                                    member.is_owner = true;
                                    member.set_label_markup ();
                                    return false;
                                });
                            }
                        }
                    }
                } catch (RegexError e) {
                    critical ("determine_ownership_thread: %s", e.message);
                }
            }
            debug ("determine_ownership_thread: Owner for network %s: %s", id, owner);

            set_label_markup ();

            // Update sidebar with new ownership info when selected
            if (win.network_list.get_selected_item () == this) {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    win.sidebar.set_network (this);
                    return false;
                });
            }
        }

        public void add_member (Member member) {
            members.append (member);
            store.append (member);

            show_hide_expander ();
        }

        public void remove_member (Member member) {
            members.remove (member);

            uint position;
            store.find (member, out position);
            store.remove (position);

            show_hide_expander ();
            win.network_list.selection_changed ();
        }

        public void go_online () {
            updating = true;

            status = new Status ("*");
            set_row_css_classes ();

            win.sidebar.set_network (this);

            new Thread<void*> (null, () => {
                bool success = Hamachi.go_online (this);

                if (success) {
                    // Wait a second to get an updated list
                    Thread.usleep (1000000);

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        if (demo_mode) {
                            set_sort_strings ();
                            foreach (Member member in members) {
                                member.set_sort_strings ();
                            }
                            set_label_markup ();
                        }

                        Controller.update_connection ();
                        return false;
                    });
                } else {
                    status = new Status (" ");
                    set_row_css_classes ();

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.sidebar.set_network (this);
                        return false;
                    });
                }

                updating = false;
                return null;
            });
        }

        public void go_offline () {
            updating = true;

            status = new Status (" ");
            set_row_css_classes ();

            win.sidebar.set_network (this);

            new Thread<void*> (null, () => {
                bool success = Hamachi.go_offline (this);

                if (success) {
                    // Wait a second to get an updated list
                    Thread.usleep (1000000);

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        if (demo_mode) {
                            set_sort_strings ();
                            foreach (Member member in members) {
                                member.set_sort_strings ();
                            }
                            set_label_markup ();
                        }

                        Controller.update_connection ();
                        return false;
                    });
                } else {
                    status = new Status ("*");
                    set_row_css_classes ();

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        win.sidebar.set_network (this);
                        return false;
                    });
                }

                updating = false;
                return null;
            });
        }

        public void change_password () {
            var dialog = new ChangePasswordDialog (this);
            win.show_dialog (dialog);
        }

        public void change_access () {
            var dialog = new ChangeAccessDialog (this);
            win.show_dialog (dialog);
        }

        public void leave () {
            var heading = _("Are you sure you want to leave the network “{0}”?").replace ("{0}", name);
            var body    = _("If admitted, you can rejoin the network at any later time.");

            dialog = new ConfirmDialog (win, heading, body, _("_Leave"), Adw.ResponseAppearance.SUGGESTED);
            dialog.confirm.connect (() => {
                new Thread<void*> (null, () => {
                    bool success = Hamachi.leave (this);

                    if (success) {
                        Idle.add_full (Priority.HIGH_IDLE, () => {
                            connection.remove_network (this);
                            return false;
                        });

                        // Wait a second to get an updated list
                        Thread.usleep (1000000);

                        Idle.add_full (Priority.HIGH_IDLE, () => {
                            Controller.update_connection ();
                            return false;
                        });
                    }

                    return null;
                });
            });
        }

        public void delete () {
            var heading = _("Are you sure you want to delete the network “{0}”?").replace ("{0}", name);
            var body    = _("If you delete a network, it will be permanently lost.");

            dialog = new ConfirmDialog (win, heading, body, _("_Delete"), Adw.ResponseAppearance.DESTRUCTIVE);
            dialog.confirm.connect (() => {
                new Thread<void*> (null, () => {
                    bool success = Hamachi.delete (this);

                    if (success) {
                        Idle.add_full (Priority.HIGH_IDLE, () => {
                            connection.remove_network (this);
                            return false;
                        });

                        // Wait a second to get an updated list
                        Thread.usleep (1000000);

                        Idle.add_full (Priority.HIGH_IDLE, () => {
                            Controller.update_connection ();
                            return false;
                        });
                    }

                    return null;
                });
            });
        }
    }
}
