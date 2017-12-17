/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

public class NetworkView : TreeView
{
    private static TreeStore store;
    private static TreeModelSort sorted_store;
    private static TreeModelFilter filter;
    
    private TreePath last_path;
    
    public Network last_network;
    public Member last_member;
    
    private TreeViewColumn column;
    
    private int icon_column;
    private int status_column;
    private int network_column;
    private int member_column;
    private int name_sort_column;
    private int status_sort_column;
    
    private CellRendererText text_cell;
    private CellRendererPixbuf icon_cell;
    
    public  string network_template;
    public  string member_template;
    private string current_layout;
    
    private Menus.NetworkMenu network_menu;
    private Menus.MemberMenu member_menu;
    private Menus.JoinCreateMenu join_create_menu;
    
    private bool skip_update_collapsed_networks;

    public NetworkView ()
    {
        search_column      = 0;
        icon_column        = 1;
        status_column      = 2;
        network_column     = 3;
        member_column      = 4;
        name_sort_column   = 5;
        status_sort_column = 6;
        
        text_cell = new CellRendererText();
        text_cell.ellipsize = Pango.EllipsizeMode.END;
        
        icon_cell = new CellRendererPixbuf();
        
        column = new TreeViewColumn();
        
        column.pack_start (icon_cell, false);
        column.pack_start (text_cell, true);
        
        column.set_cell_data_func (text_cell, text_cell_layout);
        
        column.add_attribute (icon_cell, "icon-name", icon_column);
        column.add_attribute (text_cell, "text", search_column);
        
        append_column (column);
        
        row_activated.connect  (on_row_activate);
        row_collapsed.connect  (on_row_collapsed);
        row_expanded.connect   (on_row_expanded);
        popup_menu.connect     (handle_popup_menu);
        has_tooltip            = true;
        query_tooltip.connect  (on_query_tooltip);
        level_indentation      = 0;
        cursor_changed.connect (row_handler);
        headers_visible        = false;
        set_search_entry       (HaguichiWindow.search_entry);
        
        init_store();
        set_layout_from_string ("small");
        generate_popup_menus();
    }
    
    private void init_store ()
    {
        store = new TreeStore (7,                    // Num columns
                               typeof (string),      // Search string (name)
                               typeof (string),      // Status icon name
                               typeof (int),         // Status integer for filtering offline members
                               typeof (Network),     // Network instance
                               typeof (Member),      // Member instance (null when network row)
                               typeof (string),      // Sortable name string
                               typeof (string));     // Sortable status
        
        filter = new TreeModelFilter (store, null);
        filter.set_visible_func (filter_tree);
        
        sorted_store = new TreeModelSort.with_model (filter);
        
        model = sorted_store;
    }
    
    public void generate_popup_menus ()
    {
        network_menu     = new Menus.NetworkMenu();
        member_menu      = new Menus.MemberMenu();
        join_create_menu = new Menus.JoinCreateMenu();
    }
    
    private bool is_network (TreeModel _model, TreeIter _iter)
    {
        return !is_member (_model, _iter);
    }
    
    private bool is_member (TreeModel _model, TreeIter _iter)
    {
        Value member_val;
        _model.get_value (_iter, member_column, out member_val);
        
        if ((Member) member_val != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private bool on_query_tooltip (int x, int y, bool keyboard_tooltip, Tooltip tooltip)
    {
        bool retval = false;
        
        Box   tip_box;
        Label tip_label;
        Image tip_icon;
        
        TreePath path;
        TreeViewColumn column;
        int cell_x;
        int cell_y;
        
        if (get_path_at_pos (x, y, out path, out column, out cell_x, out cell_y))
        {
            TreeIter _iter;
            sorted_store.get_iter (out _iter, path);
            
            tip_box = new Box (Orientation.HORIZONTAL, 0);
            tip_box.border_width = 3;
            
            tip_label = new Label (null);
            tip_label.margin_start = 6;
            tip_label.margin_end = 6;
            tip_label.margin_bottom = 3;
            
            tip_icon = new Image();
            tip_icon.valign = Align.START;
            tip_icon.margin_start = 3;
            tip_icon.margin_end = 3;
            
            if (is_network (sorted_store, _iter))
            {
                Value network_val;
                sorted_store.get_value (_iter, network_column, out network_val);
                Network network = (Network) network_val;
                
                int member_count;
                int member_online_count;
                
                network.return_member_count (out member_count, out member_online_count);
                
                string status_string   = Utils.format ("\n{0} <i>{1}</i>", Text.status, network.status.status_text, null);
                string id_string       = Utils.format ("\n{0} <i>{1}</i>", Text.network_id, Markup.escape_text (network.id), null);
                string count_string    = Utils.format (Text.member_count, member_online_count.to_string(), member_count.to_string(), null);
                string member_string   = Utils.format ("\n{0} <i>{1}</i>", Text.members, count_string, null);
                string capacity_string = "";
                string owner_string    = Utils.format ("\n{0} <i>{1}</i>", Text.owner, Markup.escape_text (network.return_owner_string()), null);
                string lock_string     = "";
                string approve_string  = "";
                
                if (network.capacity > 0)
                {
                    capacity_string = Utils.format ("\n{0} <i>{1}</i>", Text.capacity, network.capacity.to_string(), null);
                }
                
                if (network.lock_state == "locked")
                {
                    lock_string = Utils.format ("\n{0} <i>{1}</i>", Text.locked, Text.yes, null);
                }
                
                if (network.lock_state == "unlocked")
                {
                    lock_string = Utils.format ("\n{0} <i>{1}</i>", Text.locked, Text.no, null);
                }
                
                if (network.approve == "manual")
                {
                    approve_string = Utils.format ("\n{0} <i>{1}</i>", Text.approval, Text.manually, null);
                }
                
                if (network.approve == "auto")
                {
                    approve_string = Utils.format ("\n{0} <i>{1}</i>", Text.approval, Text.automatically, null);
                }
                
                tip_label.set_markup ("<big><b>" + Markup.escape_text (network.name) + "</b></big><small>" + status_string + id_string + member_string + owner_string + capacity_string + lock_string + approve_string + "</small>");
                
                tip_icon.set_from_icon_name (Utils.get_network_icon_name (false), IconSize.DIALOG);
                
                tip_box.add (tip_icon);
                tip_box.add (tip_label);
                tip_box.show_all();
                
                tooltip.set_custom (tip_box);
                retval = true;
            }
            else
            {
                Value member_val;
                sorted_store.get_value (_iter, member_column, out member_val);
                Member member = (Member) member_val;
                
                string status_string     = Utils.format ("\n{0} <i>{1}</i>", Text.status, member.status.status_text, null);
                string client_string     = Utils.format ("\n{0} <i>{1}</i>", Text.client_id, member.client_id, null);
                string address_string    = "";
                string tunnel_string     = "";
                string connection_string = "";
                
                if (member.ipv4 != null)
                {
                    address_string += Utils.format ("\n{0} <i>{1}</i>", Text.address_ipv4, member.ipv4, null);
                }
                
                if (member.ipv6 != null)
                {
                    address_string += Utils.format ("\n{0} <i>{1}</i>", Text.address_ipv6, member.ipv6, null);
                }
                
                if (member.tunnel != null)
                {
                    tunnel_string = Utils.format ("\n{0} <i>{1}</i>", Text.tunnel, member.tunnel, null);
                }
                
                if (member.status.connection_type != "")
                {
                    connection_string = Utils.format ("\n{0} <i>{1}</i>", Text.connection, member.status.connection_type, null);
                }
                
                tip_label.set_markup ("<big><b>" + Markup.escape_text (member.nick) + "</b></big><small>" + status_string + client_string + address_string + tunnel_string + connection_string + "</small>");
                
                tip_icon.set_from_icon_name (Utils.get_member_icon_name (false), IconSize.DIALOG);
                
                tip_box.add (tip_icon);
                tip_box.add (tip_label);
                tip_box.show_all();
                
                tooltip.set_custom (tip_box);
                retval = true;
            }
            
            set_tooltip_row (tooltip, path); // Redraw tooltip for every row separately
        }
        
        return retval;
    }
    
    public void fill_tree ()
    {
        init_store();
        
        foreach (Network network in Haguichi.connection.networks)
        {
            add_network (network);
        }
        
        refilter();
    }
    
    public void add_network (Network network)
    {
        int member_count;
        int member_online_count;
        
        network.return_member_count (out member_count, out member_online_count);
        
        TreeIter network_iter;
        store.append (out network_iter, null);
        store.set (network_iter,
                   0, network.name,
                   1, network.status.get_icon_name(),
                   2, network.status.status_int,
                   3, network,
                   4, null,
                   5, network.name_sort_string,
                   6, network.status_sort_string,
                   -1);
        
        foreach (Member member in network.members)
        {
            add_member (network, member);
        }
        
        network.init();
        
        collapse_or_expand_network (network);
    }
    
    public void update_network (Network network)
    {
        TreeIter network_iter = return_network_iter (network);
        store.set (network_iter,
                   0, network.name,
                   1, network.status.get_icon_name(),
                   2, network.status.status_int,
                   3, network,
                   4, null,
                   5, network.name_sort_string,
                   6, network.status_sort_string,
                   -1);
    }
    
    public Network return_network_by_id (string id)
    {
        Network return_network = new Network.empty();
        TreeIter network_iter = TreeIter();
        
        if (store.get_iter_first (out network_iter))                        // First network
        {
            Value network_val;
            store.get_value (network_iter, network_column, out network_val);
            Network _network = (Network) network_val;
            
            if (_network.id == id)
            {
                return_network = _network;
            }
            while (store.iter_next (ref network_iter))                      // All other networks
            {
                store.get_value (network_iter, network_column, out network_val);
                _network = (Network) network_val;
                
                if (_network.id == id)
                {
                    return_network = _network;
                }
            }
        }
        
        return return_network;
    }
    
    private TreeIter return_network_iter (Network network)
    {
        TreeIter return_iter = TreeIter();
        TreeIter network_iter = TreeIter();
        
        if (store.get_iter_first (out network_iter))                        // First network
        {
            Value network_val;
            store.get_value (network_iter, network_column, out network_val);
            Network _network = (Network) network_val;
            
            if (_network.id == network.id)
            {
                return_iter = network_iter;
            }
            while (store.iter_next (ref network_iter))                      // All other networks
            {
                store.get_value (network_iter, network_column, out network_val);
                _network = (Network) network_val;
                
                if (_network.id == network.id)
                {
                    return_iter = network_iter;
                }
            }
        }
        
        return return_iter;
    }
    
    private TreeIter return_member_iter (Network network, Member member)
    {
        TreeIter _iter = TreeIter();
    
        TreeIter network_iter = return_network_iter (network);
        TreeIter member_iter = TreeIter();
        store.iter_children (out member_iter, network_iter);                 // First member
        
        Value member_val;
        store.get_value (member_iter, member_column, out member_val);
        Member _member = (Member) member_val;
        
        if (_member.client_id == member.client_id)
        {
            _iter = member_iter;
        }      
        while (store.iter_next (ref member_iter))                           // All other members
        {
            store.get_value (member_iter, member_column, out member_val);
            _member = (Member) member_val;
            
            if (_member.client_id == member.client_id)
            {
                _iter = member_iter;
            }
        }
        
        return _iter;
    }
    
    public void remove_network (Network network)
    {
        TreeIter _iter = return_network_iter (network);
        store.remove (ref _iter);
    }
    
    public void add_member (Network network, Member member)
    {
        TreeIter parent_iter = return_network_iter (network);
        TreeIter member_iter;
        store.append (out member_iter, parent_iter);
        store.set (member_iter,
                   0, member.nick,
                   1, member.status.get_icon_name(),
                   2, member.status.status_int,
                   3, network,
                   4, member,
                   5, member.name_sort_string,
                   6, member.status_sort_string,
                   -1);
        
        member.init();
    }
    
    public void update_member (Member member)
    {
        update_member_with_network (return_network_by_id (member.network_id), member);
    }
    
    public void update_member_with_network (Network network, Member member)
    {
        TreeIter member_iter = return_member_iter (network, member);
        store.set (member_iter,
                   0, member.nick,
                   1, member.status.get_icon_name(),
                   2, member.status.status_int,
                   3, network,
                   4, member,
                   5, member.name_sort_string,
                   6, member.status_sort_string,
                   -1);
    }
    
    public void remove_member (Network network, Member member)
    {
        TreeIter _iter = return_member_iter (network, member);
        store.remove (ref _iter);
    }
    
    public void refilter ()
    {
        go_sort ((string) Settings.sort_network_list_by.val);
        
        filter.refilter();
        
        collapse_or_expand_networks();
    }
    
    public void go_sort (string sort_by)
    {
        if (sort_by == "status")
        {
            sorted_store.set_sort_column_id (status_sort_column, SortType.ASCENDING);
        }
        else
        {
            sorted_store.set_sort_column_id (name_sort_column, SortType.ASCENDING);
        }
    }
    
    private bool filter_tree (TreeModel _model, TreeIter _iter)
    {
        bool show_offline_members = (bool) Settings.show_offline_members.val;
        
        if (!show_offline_members)
        {
            Value status_val;
            _model.get_value (_iter, status_column, out status_val);
            int status = (int) status_val;
            
            if ((!is_network (_model, _iter)) &&
                (status == 0))
            {
                return false;
            }
        }
        
        string filter_text = HaguichiWindow.search_entry.text.down();
        
        if (filter_text != "")
        {
            Value network_val;
            _model.get_value (_iter, network_column, out network_val);
            Network network = (Network) network_val;
            
            if (network != null)
            {
                string match_text = network.name + " " + network.id;
                
                if (is_network (_model, _iter))
                {
                    foreach (Member member in network.members)
                    {
                        if ((show_offline_members) ||
                            (member.status.status_int != 0))
                        {
                            match_text += " " + member.nick + " " + member.client_id + " " + member.ipv4 + " " + member.ipv6;
                        }
                    }
                }
                else
                {
                    Value member_val;
                    _model.get_value (_iter, member_column, out member_val);
                    Member member = (Member) member_val;
                    
                    if (member != null)
                    {
                        match_text += " " + member.nick + " " + member.client_id + " " + member.ipv4 + " " + member.ipv6;
                    }
                }
                
                if (!match_text.down().contains (filter_text))
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    private void text_cell_layout (CellLayout layout, CellRenderer cell, TreeModel _model, TreeIter _iter)
    {
        CellRendererText text_cell = (cell as CellRendererText);
        
        StyleContext context = get_style_context();
        context.save();
        
        context.set_state (StateFlags.NORMAL);
        Gdk.RGBA online_txt_color = context.get_color (context.get_state());
        
        context.set_state (StateFlags.INSENSITIVE);
        Gdk.RGBA offline_txt_color = context.get_color (context.get_state());
        
        context.restore();
        
        Value network_val;
        _model.get_value (_iter, network_column, out network_val);
        Network network = (Network) network_val;
        
        Value member_val;
        _model.get_value (_iter, member_column, out member_val);
        Member member = (Member) member_val;
        
        if (is_network (_model, _iter))
        {
            string id   = Markup.escape_text (network.id).replace   ("%", "{PERCENTSIGN}");
            string name = Markup.escape_text (network.name).replace ("%", "{PERCENTSIGN}");
            
            int member_count;
            int member_online_count;
            network.return_member_count (out member_count, out member_online_count);
            
            string template = network_template;
            template = template.replace ("%ID",  id);
            template = template.replace ("%N",   name);
            template = template.replace ("%S",   network.status.status_text);
            template = template.replace ("%T",   member_count.to_string());
            template = template.replace ("%O",   member_online_count.to_string());
            template = template.replace ("%CAP", network.capacity.to_string());
            template = template.replace ("<br>", "\n");
            
            if (network.is_owner == 1)
            {
                template = template.replace ("%*",  "✩");
                template = template.replace ("%_*", " ✩");
                template = template.replace ("%*_", "✩ ");
            }
            else
            {
                template = template.replace ("%*",  "");
                template = template.replace ("%_*", "");
                template = template.replace ("%*_", "");
            }
            
            text_cell.markup = template.replace ("{PERCENTSIGN}", "%");
            
            if (network.status.status_int == 0)
            {
                text_cell.foreground_rgba = offline_txt_color;
            }
            else
            {
                text_cell.foreground_rgba = online_txt_color;
            }
        }
        else
        {
            string name = Markup.escape_text (member.known_name).replace ("%", "{PERCENTSIGN}");
            
            string address = "";
            if ((member.ipv4 != null) &&
                (member.ipv6 != null))
            {
                address = member.ipv4 + " / " + member.ipv6;
            }
            else if (member.ipv4 != null)
            {
                address = member.ipv4;
            }
            else if (member.ipv6 != null)
            {
                address = member.ipv6;
            }
            
            string template = member_template;
            template = template.replace ("%ID",  member.client_id);
            template = template.replace ("%N",   name);
            template = template.replace ("%A",   address);
            template = template.replace ("%IP4", (member.ipv4   == null) ? "" : member.ipv4);
            template = template.replace ("%IP6", (member.ipv6   == null) ? "" : member.ipv6);
            template = template.replace ("%TUN", (member.tunnel == null) ? "" : member.tunnel);
            template = template.replace ("%S",   member.status.status_text);
            template = template.replace ("%CX",  member.status.connection_type);
            template = template.replace ("<br>", "\n");
            
            if (network.owner == member.client_id)
            {
                template = template.replace ("%*",   "✩");
                template = template.replace ("%_*", " ✩");
                template = template.replace ("%*_", "✩ ");
            }
            else
            {
                template = template.replace ("%*",  "");
                template = template.replace ("%_*", "");
                template = template.replace ("%*_", "");
            }
            
            text_cell.markup = template.replace ("{PERCENTSIGN}", "%");
            
            if (member.status.status_int == 0)
            {
                text_cell.foreground_rgba = offline_txt_color;
            }
            else
            {
                text_cell.foreground_rgba = online_txt_color;
            }
        }
    }
    
    private void row_handler ()
    {
        TreeModel _model;
        TreeIter _iter;
        
        if (get_selection().get_selected (out _model, out _iter))
        {
            last_path = sorted_store.get_path (_iter);
            
            Value network_val;
            sorted_store.get_value (_iter, network_column, out network_val);
            last_network = (Network) network_val;
            
            HaguichiWindow.sidebar.set_network (last_network);
            
            if (is_network (sorted_store, _iter))
            {
                HaguichiWindow.sidebar.set_member (null);
                HaguichiWindow.sidebar.show_tab ("Network", true);
            }
            else
            {
                Value member_val;
                sorted_store.get_value (_iter, member_column, out member_val);
                last_member = (Member) member_val;
                
                HaguichiWindow.sidebar.set_member (last_member);
                HaguichiWindow.sidebar.show_tab ("Member", true);
            }
        }
        else
        {
            HaguichiWindow.sidebar.show_tab ("Info", false);
        }
    }
    
    public void activate_selected_row ()
    {
        TreeIter _iter;
        TreeModel _model;
        
        if (get_selection().get_selected (out _model, out _iter))
        {
            last_path = sorted_store.get_path (_iter);
            row_activated (last_path, column);
        }
    }
    
    private void on_row_activate (TreePath path, TreeViewColumn column)
    {
        TreeIter _iter;
        sorted_store.get_iter (out _iter, path);
        
        if (is_network (sorted_store, _iter))
        {
            Value network_val;
            sorted_store.get_value (_iter, network_column, out network_val);
            Network network = (Network) network_val;
            
            int status_int = network.status.status_int;
            
            if (status_int == 1)
            {
                network.go_offline();
            }
            else
            {
                network.go_online();
            }
        }
        else
        {
            if (last_member.status.status_int == 1)
            {
                string[] command = Command.return_default();
                
                if (command.length == 6)
                {
                    Command.execute (Command.return_custom (last_member, command[3], command[4], command[5]));
                }
            }
        }
    }
    
    private void on_row_expanded (TreeIter _iter, TreePath path)
    {
        update_collapsed_networks (_iter, "expanded");
    }

    private void on_row_collapsed (TreeIter _iter, TreePath path)
    {
        update_collapsed_networks (_iter, "collapsed");
    }
    
    private void update_collapsed_networks (TreeIter _iter, string mode)
    {
        if (skip_update_collapsed_networks)
        {
            return;
        }
        
        if (is_network (sorted_store, _iter))
        {
            Value network_val;
            sorted_store.get_value (_iter, network_column, out network_val);
            Network network = (Network) network_val;
            
            string[] cur_networks = (string[]) Settings.collapsed_networks.val;
            string[] new_networks = new string[] {};
            
            foreach (string network_id in cur_networks)
            {
                if (network_id != network.id)
                {
                    new_networks += network_id;
                }
            }
            
            if (mode == "collapsed")
            {
                new_networks += network.id;
            }
            
            Settings.collapsed_networks.val = new_networks;
        }
    }
    
    public void collapse_or_expand_networks ()
    {
        TreeIter network_iter = TreeIter();
        
        if (sorted_store.get_iter_first (out network_iter))                         // First network
        {
            Value network_val;
            sorted_store.get_value (network_iter, network_column, out network_val);
            Network network = (Network) network_val;
            
            skip_update_collapsed_networks = true;
            
            if (is_collapsed (network))
            {
                collapse_row (sorted_store.get_path (network_iter));
            }
            else
            {
                expand_row (sorted_store.get_path (network_iter), false);
            }
            
            while (sorted_store.iter_next (ref network_iter))                      // All other networks
            {
                sorted_store.get_value (network_iter, network_column, out network_val);
                network = (Network) network_val;
                
                if (is_collapsed (network))
                {
                    collapse_row (sorted_store.get_path (network_iter));
                }
                else
                {
                    expand_row (sorted_store.get_path (network_iter), false);
                }
            }
            
            skip_update_collapsed_networks = false;
        }
    }
    
    private void collapse_or_expand_network (Network network)
    {
        TreeIter _iter = return_network_iter (network);
        
        skip_update_collapsed_networks = true;
        
        if (is_collapsed (network))
        {
            collapse_row (store.get_path (_iter));
        }
        else
        {
            expand_row (store.get_path (_iter), false);
        }
        
        skip_update_collapsed_networks = false;
    }
    
    private bool is_collapsed (Network network)
    {
        string[] collapsed = (string[]) Settings.collapsed_networks.val;
            
        foreach (string network_id in collapsed)
        {
            if (network_id == network.id)
            {
                return true;
            }
        }   
        
        return false;
    }
    
    private void show_popup_menu ()
    {
        TreeIter _iter;
        sorted_store.get_iter (out _iter, last_path);
        
        Value network_val;
        sorted_store.get_value (_iter, network_column, out network_val);
        Network last_network = (Network) network_val;
        
        if (is_network (sorted_store, _iter))
        {
            network_menu.set_network (last_network);
            network_menu.popup (null, null, null, 0, get_current_event_time());
        }
        else
        {
            Value member_val;
            sorted_store.get_value (_iter, member_column, out member_val);
            Member last_member = (Member) member_val;
            
            member_menu.set_member (last_member, last_network);
            member_menu.popup (null, null, null, 0, get_current_event_time());
        }
    }
    
    private bool handle_popup_menu ()
    {
        if (get_selection().path_is_selected (last_path))
        {
            show_popup_menu();
            return true;
        }
        
        return false;
    }
    
    public void activate_command_by_number (int number)
    {
        TreeModel _model;
        TreeIter _iter;
        
        if (get_selection().get_selected (out _model, out _iter))
        {
            if (is_member (sorted_store, _iter))
            {
                Value member_val;
                sorted_store.get_value (_iter, member_column, out member_val);
                Member member = (Member) member_val;
                
                if (member.status.status_int == 1)
                {
                    string[] command = Command.return_by_number (number);
                    
                    if (command.length == 6)
                    {
                        Command.execute (Command.return_custom (member, command[3], command[4], command[5]));
                    }
                }
            }
        }
    }
    
    private void set_network_list_icon_size (IconSize size)
    {
        icon_cell.stock_size = size;
        columns_autosize();
    }
    
    public void refresh_layout ()
    {
        string layout = current_layout;
        current_layout = "";
        set_layout_from_string (layout);
    }
    
    public void set_layout_from_string (string layout)
    {
        if (layout == current_layout)
        {
            // Layout already set
        }
        else if (layout == "large")
        {
            current_layout = "large";
            
            network_template = (string) Settings.network_template_large.val;
            member_template  = (string) Settings.member_template_large.val;
            
            set_network_list_icon_size (IconSize.LARGE_TOOLBAR);
        }
        else
        {
            current_layout = "small";
            
            network_template = (string) Settings.network_template_small.val;
            member_template  = (string) Settings.member_template_small.val;
            
            set_network_list_icon_size (IconSize.SMALL_TOOLBAR);
        }
        
    }
    
    public override bool button_press_event (Gdk.EventButton event)
    {
        if (event.button == 3)
        {
            TreeViewColumn column;
            int cell_x;
            int cell_y;
            
            if (get_path_at_pos ((int) event.x, (int) event.y, out last_path, out column, out cell_x, out cell_y))
            {
                show_popup_menu();
            }
            else
            {
                join_create_menu.popup (null, null, null, 0, get_current_event_time());
            }
        }
        
        return base.button_press_event(event);
    }
}
