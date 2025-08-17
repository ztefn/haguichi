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

using Gtk;

namespace Haguichi {
    [GtkTemplate (ui = "/com/github/ztefn/haguichi/ui/network-list.ui")]
    public class NetworkList : Adw.Bin {
        [GtkChild]
        private unowned ScrolledWindow scrolled_window;
        [GtkChild]
        private unowned ListView list_view;

        private GLib.Settings   settings;
        private GLib.ListStore  store;
        private Sorter          sorter;
        private Filter          filter;
        private TreeListModel   tree_list_model;
        private SortListModel   sort_model;
        private FilterListModel filter_model;
        private SingleSelection selection_model;
        private PopoverMenu     list_view_popover_menu;
        private PopoverMenu     network_popover_menu;
        private PopoverMenu     member_popover_menu;
        private Menu            member_menu_model;

        private bool   commands_menu_present = false;
        private bool   skip_save_collapsed_networks;
        private bool   show_offline_members;
        private string sort_by;

        public string  network_label_template;
        public string  member_label_template;
        public string  restore_search_text;
        public string  select_member_id;
        public string  select_network_id;

        construct {
            install_action ("row.context-menu",        null, (WidgetActionActivateFunc) on_context_menu);
            install_action ("row.collapse",            null, (WidgetActionActivateFunc) on_collapse_row);
            install_action ("row.expand",              null, (WidgetActionActivateFunc) on_expand_row);
            install_action ("row.delete",              null, (WidgetActionActivateFunc) on_delete_row);

            install_action ("network.go-online",       null, (WidgetActionActivateFunc) on_go_online);
            install_action ("network.go-offline",      null, (WidgetActionActivateFunc) on_go_offline);
            install_action ("network.leave",           null, (WidgetActionActivateFunc) on_delete_row);
            install_action ("network.delete",          null, (WidgetActionActivateFunc) on_delete_row);
            install_action ("network.change-access",   null, (WidgetActionActivateFunc) on_change_access);
            install_action ("network.change-password", null, (WidgetActionActivateFunc) on_change_password);
            install_action ("network.copy-id",         null, (WidgetActionActivateFunc) on_copy_id);
            install_action ("network.details",         null, (WidgetActionActivateFunc) on_show_details);

            install_action ("member.approve",          null, (WidgetActionActivateFunc) on_approve_row);
            install_action ("member.reject",           null, (WidgetActionActivateFunc) on_reject_row);
            install_action ("member.evict",            null, (WidgetActionActivateFunc) on_delete_row);
            install_action ("member.copy-id",          null, (WidgetActionActivateFunc) on_copy_id);
            install_action ("member.copy-ipv4",        null, (WidgetActionActivateFunc) on_copy_ipv4);
            install_action ("member.copy-ipv6",        null, (WidgetActionActivateFunc) on_copy_ipv6);
            install_action ("member.details",          null, (WidgetActionActivateFunc) on_show_details);

            settings = new GLib.Settings (Config.APP_ID + ".network-list");

            show_offline_members   = settings.get_boolean ("show-offline-members");
            sort_by                = settings.get_string  ("sort-by");
            network_label_template = settings.get_string  ("network-template");
            member_label_template  = settings.get_string  ("member-template");

            settings.changed.connect ((key) => {
                if (key == "sort-by") {
                    sort_by = settings.get_string ("sort-by");
                    sort ();
                } else if (key == "show-offline-members") {
                    autoselect (true);
                    show_offline_members = settings.get_boolean ("show-offline-members");
                    refilter ();
                }
            });

            store = new GLib.ListStore (typeof (Network));

            tree_list_model = new TreeListModel (store, false, true, create_model);

            sorter = new CustomSorter((a, b) => {
                var itemA = ((TreeListRow) a).get_item ();
                var itemB = ((TreeListRow) b).get_item ();

                return sort_by_string (get_item_sort_string (itemA), get_item_sort_string (itemB));
            });

            sort_model = new SortListModel (tree_list_model, sorter);

            filter = new CustomFilter ((row) => {
                var item = ((TreeListRow) row).get_item ();
                return filter_tree_row (item, win.search_entry.text);
            });

            filter_model = new FilterListModel (sort_model, filter);
            filter_model.items_changed.connect (() => {
                if (Controller.last_status >= 6) {
                    // When items are added or removed from the model no selection-changed signal will be emitted
                    // so we need to listen to items-changed as well and manually call selection_changed()
                    Idle.add_full (Priority.HIGH_IDLE, () => {
                        selection_changed ();
                        return false;
                    });
                }
            });

            selection_model = new SingleSelection (filter_model);
            selection_model.autoselect = false;
            selection_model.can_unselect = true;
            selection_model.selection_changed.connect (() => {
                selection_changed ();
            });

            list_view.model = selection_model;

            var network_list_menu_builder = new Builder.from_resource("/com/github/ztefn/haguichi/ui/menus/network-list-menu.ui");
            list_view_popover_menu = (PopoverMenu) network_list_menu_builder.get_object ("popover_menu");
            list_view_popover_menu.set_parent (this);

            var network_menu_builder = new Builder.from_resource("/com/github/ztefn/haguichi/ui/menus/network-menu.ui");
            network_popover_menu = (PopoverMenu) network_menu_builder.get_object ("popover_menu");
            network_popover_menu.set_parent (this);

            var member_menu_builder = new Builder.from_resource("/com/github/ztefn/haguichi/ui/menus/member-menu.ui");
            member_popover_menu = (PopoverMenu) member_menu_builder.get_object ("popover_menu");
            member_popover_menu.set_parent (this);

            member_menu_model = (Menu) member_menu_builder.get_object ("menu_model");

            var secondary_click_gesture = new GestureClick () {
                button = Gdk.BUTTON_SECONDARY
            };
            secondary_click_gesture.pressed.connect ((n_press, x, y) => {
                var widget = pick (x, y, PickFlags.DEFAULT);
                if (widget is ListView) {
                    show_context_menu (null, (int) x, (int) y);
                } else {
                    //print ("type: %s\n", widget.get_type ().name ());
                    TreeExpander tree_expander;
                    if (widget is TreeExpander) {
                        // Widget is tree expander itself
                        tree_expander = (TreeExpander) widget;
                    } else if (widget.get_first_child () is TreeExpander) {
                        // Get child of list item widget
                        tree_expander = (TreeExpander) widget.get_first_child ();
                    } else {
                        // Find tree expander in ancestor
                        tree_expander = (TreeExpander) widget.get_ancestor (typeof (TreeExpander));
                    }

                    if (tree_expander != null) {
                        var item = tree_expander.item;
                        if (item is Network) {
                            var network = (Network) item;
                            selection_model.selected = find_selection_model_position (network);
                            show_context_menu (item, (int) x, (int) y);
                        } else if (item is Member) {
                            var member = (Member) item;
                            selection_model.selected = find_selection_model_position (member);
                            show_context_menu (item, (int) x, (int) y);
                        }
                    }
                }
            });
            add_controller (secondary_click_gesture);
        }

        [GtkCallback]
        public void on_activate () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;

                if (network.status.status_int == 1) {
                    network.go_offline ();
                } else {
                    network.go_online ();
                }
            } else if (item is Member) {
                var member = (Member) item;
                Command.execute_default_command (member);
            }
        }

        private void on_collapse_row () {
            var item = get_selected_item ();

            if (item is Network) {
                set_row_expanded ((Network) item, false);
            }
        }

        private void on_expand_row () {
            var item = get_selected_item ();

            if (item is Network) {
                set_row_expanded ((Network) item, true);
            }
        }

        private void on_delete_row () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;

                if (network.is_owner == 1) {
                    network.delete ();
                } else if (network.is_owner == 0) {
                    network.leave ();
                }
            } else if (item is Member) {
                var member = (Member) item;
                if (member.network.is_owner == 1 && member.status.status_int != 3) {
                    member.evict ();
                }
            }
        }

        private void on_context_menu () {
            // Position context menu based on focused row in list view
            Graphene.Rect out_bounds;
            var focus_widget = list_view.get_focus_child ();
            focus_widget.compute_bounds (list_view, out out_bounds);
            show_context_menu (get_selected_item (), 30, (int) out_bounds.get_y () + focus_widget.get_height () + 12);
        }

        private void show_context_menu (Object? item, int x, int y) {
            var rect = Gdk.Rectangle () {
                x = x,
                y = y
            };

            if (item is Network) {
                var network = (Network) item;

                action_set_enabled ("network.go-online",       network.status.status_int == 0);
                action_set_enabled ("network.go-offline",      network.status.status_int == 1);
                action_set_enabled ("network.leave",           network.is_owner == 0);
                action_set_enabled ("network.delete",          network.is_owner == 1);
                action_set_enabled ("network.change-access",   network.is_owner == 1);
                action_set_enabled ("network.change-password", network.is_owner == 1);
                action_set_enabled ("network.details",         !win.split_view.show_sidebar);

                network_popover_menu.pointing_to = rect;
                network_popover_menu.popup ();
            } else if (item is Member) {
                var member = (Member) item;

                action_set_enabled ("member.approve",          member.network.is_owner == 1 && member.status.status_int == 3);
                action_set_enabled ("member.reject",           member.network.is_owner == 1 && member.status.status_int == 3);
                action_set_enabled ("member.evict",            member.network.is_owner == 1 && member.status.status_int != 3);
                action_set_enabled ("member.copy-ipv4",        member.ipv4 != null          && member.status.status_int != 3);
                action_set_enabled ("member.copy-ipv6",        member.ipv6 != null          && member.status.status_int != 3);
                action_set_enabled ("member.details",          !win.split_view.show_sidebar);

                if (member.status.status_int == 3 && commands_menu_present) {
                    member_menu_model.remove (0);
                    commands_menu_present = false;
                } else if (member.status.status_int != 3 && !commands_menu_present) {
                    member_menu_model.insert_section (0, null, Command.get_commands_menu ());
                    commands_menu_present = true;
                }

                member_popover_menu.pointing_to = rect;
                member_popover_menu.popup ();
            } else {
                list_view_popover_menu.pointing_to = rect;
                list_view_popover_menu.popup ();
            }
        }

        private void on_go_online () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;
                network.go_online ();
            }
        }

        private void on_go_offline () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;
                network.go_offline ();
            }
        }

        private void on_change_access () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;
                network.change_access ();
            }
        }

        private void on_change_password () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;
                network.change_password ();
            }
        }

        private void on_approve_row () {
            var item = get_selected_item ();

            if (item is Member) {
                var member = (Member) item;
                member.approve ();
            }
        }

        private void on_reject_row () {
            var item = get_selected_item ();

            if (item is Member) {
                var member = (Member) item;
                member.reject ();
            }
        }

        private void on_copy_id () {
            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;
                copy_to_clipboard (network.id);
            } else if (item is Member) {
                var member = (Member) item;
                copy_to_clipboard (member.id);
            }
        }

        private void on_copy_ipv4 () {
            var item = get_selected_item ();

            if (item is Member) {
                var member = (Member) item;
                copy_to_clipboard (member.ipv4);
            }
        }

        private void on_copy_ipv6 () {
            var item = get_selected_item ();

            if (item is Member) {
                var member = (Member) item;
                copy_to_clipboard (member.ipv6);
            }
        }

        private void copy_to_clipboard (string text) {
            win.get_clipboard ().set_text (text);
            win.show_copied_to_clipboard_toast ();
        }

        private void on_show_details () {
            win.toggle_sidebar ();
        }

        private ListModel? create_model (Object item) {
            if (item is Network) {
                return item.store;
            }

            return null;
        }

        public void fill_tree () {
            remove_all ();

            foreach (Network network in connection.networks) {
                add_network (network, false);
            }

            if (store.n_items > 0) {
                // Make sure the scrollbar is at top most position
                list_view.scroll_to (0, ListScrollFlags.NONE, null);

                // Select specified item
                select_item ();
            }

            if (restore_search_text != null) {
                win.search_bar.search_mode_enabled = true;
                win.search_entry.text = restore_search_text;
            }

            clear_state ();
        }

        public void add_network (Network network, bool select) {
            store.append (network);

            network.init ();
            foreach (Member member in network.members) {
                member.init ();
            }

            // Show network list when the first network has been added
            if (store.n_items == 1) {
                set_connected_stack_page ();
            }

            // Set row expanded state
            if (is_collapsed (network)) {
                set_row_expanded (network, false);
            }

            // Connect to expanded signal on row to save collapsed networks
            tree_list_model.get_row (find_tree_model_position (network)).notify["expanded"].connect (() => {
                save_collapsed_networks ();
            });

            if (select) {
                select_item ();
            }
        }

        public void remove_network (Network network) {
            autoselect (true);

            uint position;
            store.find (network, out position);
            store.remove (position);

            selection_changed ();

            // Show empty status page when the last network has been removed
            if (store.n_items == 0) {
                set_connected_stack_page ();
            }
        }

        public void remove_all () {
            skip_save_collapsed_networks = true;
            store.remove_all ();
            skip_save_collapsed_networks = false;
        }

        public void sort () {
            sorter.changed (SorterChange.DIFFERENT);
        }

        public void refilter () {
            filter.changed (FilterChange.DIFFERENT);
            selection_changed ();
            set_connected_stack_page ();
        }

        public Object? get_selected_item () {
            var row = ((TreeListRow) selection_model.get_selected_item ());
            if (row is TreeListRow) {
                return row.get_item ();
            }

            return null;
        }

        public Member? get_selected_member () {
            var item = get_selected_item ();

            if (item is Member) {
                return (Member) item;
            }

            return null;
        }

        private void select_item () {
            if (select_network_id != null) {
                foreach (Network network in connection.networks) {
                    if (select_network_id == network.id) {
                        uint pos = -1;
                        if (select_member_id != null) {
                            Member member = network.return_member_by_id (select_member_id);
                            if (member != null) {
                                pos = find_selection_model_position (member);
                            }
                        } else {
                            pos = find_selection_model_position (network);
                        }

                        // Only continue if selection is visible
                        if (pos < selection_model.n_items) {
                            list_view.scroll_to (pos, ListScrollFlags.FOCUS, null);
                            selection_model.selected = pos;
                        }
                    }
                }
            }
        }

        public void unselect () {
            var item = get_selected_item ();

            if (item != null) {
                selection_model.unselect_item (find_selection_model_position (item));
            } else {
                selection_changed ();
            }
        }

        public void autoselect (bool autoselect) {
            selection_model.autoselect = autoselect;
        }

        public void save_state () {
            if (win.search_bar.search_mode_enabled) {
                restore_search_text = win.search_entry.text;
            }

            var item = get_selected_item ();

            if (item is Network) {
                select_network_id = item.id;
            } else if (item is Member) {
                select_member_id  = item.id;
                select_network_id = item.network.id;
            }
        }

        public void clear_state () {
            restore_search_text = null;
            select_member_id    = null;
            select_network_id   = null;
        }

        public void selection_changed () {
            autoselect (false);

            var item = get_selected_item ();

            if (item is Network) {
                var network = (Network) item;
                win.sidebar.set_network (network);
            } else if (item is Member) {
                var member = (Member) item;
                win.sidebar.set_member (member);
            } else {
                win.sidebar.show_page ("Information");
            }
        }

        private void set_row_expanded (Network network, bool expanded) {
            tree_list_model.get_row (find_tree_model_position (network)).expanded = expanded;
        }

        public void set_all_rows_expanded (bool expanded) {
            uint64 start = get_real_time ();

            var network_rows = new List<TreeListRow> ();
            for (uint pos = 0; pos < tree_list_model.n_items; pos++) {
                var row = tree_list_model.get_row (pos);
                var item = row.get_item ();
                if (item is Network) {
                    network_rows.append (row);
                }
            }

            // Calculate scrolled percentage to determine optimal direction
            double scrolled = (scrolled_window.vadjustment.value / scrolled_window.vadjustment.upper);
            if (expanded && scrolled > 0.6 || !expanded && scrolled < 0.66) {
                // Go backwards through list
                network_rows.reverse ();
            }

            skip_save_collapsed_networks = true;
            foreach (TreeListRow network_row in network_rows) {
                network_row.expanded = expanded;
            }
            skip_save_collapsed_networks = false;

            save_collapsed_networks ();

            debug ("set_all_rows_expanded: %s all in %s microseconds at %f scrolled\n",
                   expanded ? "expanded" : "collapsed",
                   (get_real_time () - start).to_string (), scrolled);
        }

        private bool is_collapsed (Network network) {
            string[] collapsed = settings.get_strv ("collapsed-networks");

            return (network.id in collapsed);
        }

        public void save_collapsed_networks () {
            if (skip_save_collapsed_networks) {
                return;
            }

            string[] collapsed = {};
            for (uint pos = 0; pos < tree_list_model.n_items; pos++) {
                var row = tree_list_model.get_row (pos);
                if (!row.expanded) {
                    var item = row.get_item ();
                    if (item is Network) {
                        collapsed += ((Network) item).id;
                    }
                }
            }

            settings.set_strv ("collapsed-networks", collapsed);
        }

        private int sort_by_string (string a, string b) {
            return strcmp (a.collate_key (), b.collate_key ());
        }

        private uint? find_selection_model_position (Object item) {
            for (uint pos = 0; pos < selection_model.n_items; pos++) {
                if (((TreeListRow) selection_model.model.get_item (pos)).get_item () == item) {
                    return pos;
                }
            }

            return -1;
        }

        private uint? find_tree_model_position (Object item) {
            for (uint pos = 0; pos < tree_list_model.n_items; pos++) {
                if (((TreeListRow) tree_list_model.get_row (pos)).get_item () == item) {
                    return pos;
                }
            }

            return -1;
        }

        private string get_item_sort_string (Object item) {
            var sort_string = "";
            if (item is Network) {
                Network network = (Network) item;
                sort_string = (sort_by == "status") ? network.status_sort_string : network.name_sort_string;
            } else if (item is Member) {
                Member member = (Member) item;
                sort_string = (sort_by == "status") ? member.status_sort_string : member.name_sort_string;
            }

            return sort_string;
        }

        private bool filter_tree_row (Object item, string search_text) {
            string match_text = "";

            if (item is Network) {
                Network network = (Network) item;

                // If there is no search text then network is shown
                if (search_text == "") return true;

                // Compose text to match with search text
                match_text = "%s %s %s".printf (network.name, network.id, network.owner);
                foreach (Member member in network.members) {
                    if (show_offline_members || member.status.status_int != 0) {
                        match_text += " %s %s %s %s".printf (member.nick, member.id, member.ipv4, member.ipv6);
                    }
                }
            } else if (item is Member) {
                Member member = (Member) item;

                // If offline members should be hidden then check status first
                if (!show_offline_members && member.status.status_int == 0) return false;

                // If there is no search text then member is shown
                if (search_text == "") return true;

                // Compose text to match with search text
                match_text = "%s %s %s %s %s %s %s".printf (member.network.name, member.network.id, member.network.owner,
                                                            member.nick, member.id, member.ipv4, member.ipv6);
            }

            // Split search text into multiple terms and match all
            string[] terms = search_text.casefold ().split_set (" +");
            foreach (string term in terms) {
                // If term contains comma match any of multiple words, otherwise match full term
                if (term.contains (",")) {
                    bool match = false;
                    string[] words = term.split (",");
                    foreach (string word in words) {
                        if (word != "" && match_text.casefold ().contains (word)) {
                            match = true;
                            break;
                        }
                    }
                    if (!match) {
                        return false;
                    }
                } else if (!match_text.casefold ().contains (term)) {
                    return false;
                }
            }

            return true;
        }

        private void set_connected_stack_page () {
            win.set_connected_stack_page (win.search_entry.text.length > 0 && filter_model.n_items == 0 ? "no-results" : store.n_items == 0 ? "empty" : "network-list");
        }
    }
}
