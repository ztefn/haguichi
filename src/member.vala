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
    public class Member : Object {
        private ConfirmDialog dialog;
        private string new_nick;

        public Status   status;
        public Network  network;
        public string   ipv4;
        public string   ipv6;
        public string   nick;
        public string   id;
        public string   tunnel;
 
        public string   label             { get; private set; }
        public string[] node_css_classes  { get; private set; }
        public string[] label_css_classes { get; private set; }
 
        public string   name_sort_string;
        public string   status_sort_string;
 
        public bool     is_owner;
        public bool     is_evicted;

        public Member (Status _status, Network _network, string? _ipv4, string? _ipv6, string? _nick, string _id, string? _tunnel) {
            status  = _status;
            network = _network;
            ipv4    = _ipv4;
            ipv6    = _ipv6;
            nick    = _nick;
            id      = _id;
            tunnel  = _tunnel;

            set_label_markup ();
            set_row_css_classes ();
            set_sort_strings ();

            is_evicted = false;
        }

        public void init () {
            get_long_nick (nick, true);
        }

        public void update (Status _status, string? _ipv4, string? _ipv6, string? _nick, string? _tunnel) {
            status = _status;
            ipv4   = _ipv4;
            ipv6   = _ipv6;
            tunnel = _tunnel;

            set_row_css_classes ();

            get_long_nick (_nick, false);
        }

        public string known_name {
            get {
                return (nick == _("Unknown")) ? id : nick;
            }
        }

        public void set_label_markup () {
            string name = Markup.escape_text (known_name).replace ("%", "{PERCENTSIGN}");

            string template = win.network_list.member_label_template;
            template = template.replace ("%ID",  id);
            template = template.replace ("%N",   name);
            template = template.replace ("%A",   (ipv4   != null && ipv6 != null) ? ipv4 + " / " + ipv6 : (ipv4 != null) ? ipv4 : (ipv6 != null) ? ipv6 : "");
            template = template.replace ("%IP4", (ipv4   != null) ? ipv4   : "");
            template = template.replace ("%IP6", (ipv6   != null) ? ipv6   : "");
            template = template.replace ("%TUN", (tunnel != null) ? tunnel : "");
            template = template.replace ("%S",   status.status_text);
            template = template.replace ("%CX",  status.connection_type);
            template = template.replace ("<br>", "\n");

            if (is_owner) {
                template = template.replace ("%*",   "★");
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
            label_css_classes = new string[] {"member-label", status.status_int == 0 ? "dim-label" : ""};
        }

        public void set_sort_strings () {
            name_sort_string   = network.name_sort_string   + "0" + known_name + id;
            status_sort_string = network.status_sort_string + "0" + status.status_sortable + nick + id;
        }

        private void get_long_nick (string _nick, bool _init) {
            new_nick = _nick;

            if (_init == false && new_nick.length >= 25 && nick.length >= 25 && nick.has_prefix (new_nick.replace ("�", ""))) {
                // Long nick has already been retreived and is probably not altered, since the first 25 characters are identical
            }
            else if (new_nick.length >= 25) {
                string cached_nick = get_long_nick_from_cache ();

                if (cached_nick != null) {
                    // If we got a long nick from cache then use it
                    nick = cached_nick;
                } else {
                    // If not then get long nick from hamachi
                    try {
                        member_threads.add (this);
                    } catch (ThreadError e) {
                        critical ("get_long_nick: %s", e.message);
                    }
                }
            } else {
                // Save passed nick
                nick = new_nick;
            }

            set_label_markup ();
            set_sort_strings ();
        }

        public void get_long_nick_thread () {
            // First try the cache again, because an other thread might have retreived it already for the same member in a different network
            string cached_nick = get_long_nick_from_cache ();

            if (cached_nick != null) {
                nick = cached_nick;
            } else {
                // Okay, really retreive it from hamachi now
                string output = Command.return_output ("hamachi peer %s".printf (id));
                debug ("get_long_nick_thread: %s", output);

                string _nick = Hamachi.retrieve (output, "nickname");
                if (_nick != "") {
                    nick = _nick;
                }

                // Add retreived long nick to the cache
                connection.add_long_nick (id, nick);
            }

            set_label_markup ();
            set_sort_strings ();

            // Update sidebar with new nick when selected
            if (win.network_list.get_selected_item () == this) {
                Idle.add_full (Priority.HIGH_IDLE, () => {
                    win.sidebar.set_member (this);
                    return false;
                });
            }
        }

        private string get_long_nick_from_cache () {
            string _nick = null;

            if (connection.has_long_nick (id) && connection.get_long_nick (id).has_prefix (new_nick.replace ("�", ""))) {
                _nick = connection.get_long_nick (id);
                debug ("get_long_nick_from_cache: Retrieved long nick for client %s from cache: %s", id, _nick);
            }

            return _nick;
        }

        public void approve () {
            new Thread<void*> (null, () => {
                bool success = Hamachi.approve (this);

                if (success) {
                    // Wait a second to get an updated list
                    Thread.usleep (1000000);

                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        if (demo_mode) {
                            update (new Status ("*"), "192.168.155.23", null, "Nick", null);
                            win.sidebar.set_member (this);
                        }

                        Controller.update_connection ();
                        return false;
                    });
                }

                return null;
            });
        }

        public void reject () {
            new Thread<void*> (null, () => {
                bool success = Hamachi.reject (this);

                if (success) {
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        network.remove_member (this);
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
        }

        public void evict () {
            var heading = _("Are you sure you want to evict member “{0}” from network “{1}”?").replace ("{0}", nick).replace ("{1}", network.name);
            var body    = _("If admitted, evicted members can rejoin the network at any later time.");

            dialog = new ConfirmDialog (win, heading, body, _("_Evict"), Adw.ResponseAppearance.DESTRUCTIVE);
            dialog.confirm.connect (() => {
                is_evicted = true;

                new Thread<void*> (null, () => {
                    bool success = Hamachi.evict (this);

                    if (success) {
                        Idle.add_full (Priority.HIGH_IDLE, () => {
                            network.remove_member (this);
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
