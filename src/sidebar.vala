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

public class Sidebar : Box
{
    private string current_tab;
    
    private Member member;
    private Network network;
    
    private ScrolledWindow scrolled_window;
    private Revealer action_box_revealer;
    
    private Box scrolled_box;
    private Box action_box;
    
    private Label heading_label;
    
    private Box heading_box;
    private Box commands_box;
    private Box info_box;
    private Box network_box;
    private Box member_box;
    
    private ActionBar info_action_bar;
    private ActionBar network_action_bar;
    private ActionBar member_action_bar;
    
    private SidebarLabel version_label;
    private SidebarLabel ipv4_label;
    private SidebarLabel ipv6_label;
    private SidebarLabel id_label;
    private SidebarLabel account_label;
    private SidebarLabel nick_label;
    
    private SidebarEntry version_entry;
    private SidebarEntry ipv4_entry;
    private SidebarEntry ipv6_entry;
    private SidebarEntry id_entry;
    private SidebarEntry account_entry;
    private SidebarEntry nick_entry;
    
    private Button attach_button;
    
    private SidebarLabel network_status_label;
    private SidebarLabel network_id_label;
    private SidebarLabel network_members_label;
    private SidebarLabel network_owner_label;
    private SidebarLabel network_capacity_label;
    private SidebarLabel network_lock_label;
    private SidebarLabel network_approval_label;
    
    private SidebarEntry network_status_entry;
    private SidebarEntry network_id_entry;
    private SidebarEntry network_members_entry;
    private SidebarEntry network_owner_entry;
    private SidebarEntry network_capacity_entry;
    
    private CheckButton network_lock_check;
    private ComboBox network_approval_combo;
    private Button network_password_button;
    
    private Button delete_button;
    private Button leave_button;
    private Button online_button;
    private Button offline_button;
    
    private SidebarLabel member_status_label;
    private SidebarLabel member_id_label;
    private SidebarLabel member_ipv4_label;
    private SidebarLabel member_ipv6_label;
    private SidebarLabel member_tunnel_label;
    private SidebarLabel member_connection_label;
    
    private SidebarEntry member_status_entry;
    private SidebarEntry member_id_entry;
    private SidebarEntry member_ipv4_entry;
    private SidebarEntry member_ipv6_entry;
    private SidebarEntry member_tunnel_entry;
    private SidebarEntry member_connection_entry;
    
    private Button approve_button;
    private Button reject_button;
    private Button evict_button;
    
    public Sidebar ()
    {
        current_tab = "Info";
        orientation = Orientation.VERTICAL;
        
        
        heading_label = new Label (null);
        heading_label.ellipsize = Pango.EllipsizeMode.END;
        
        heading_box = new Box (Orientation.HORIZONTAL, 0);
        heading_box.halign = Align.CENTER;
        heading_box.margin_top = 15;
        heading_box.margin_bottom = 18;
        heading_box.pack_start (heading_label, false, false, 0);
        
        
        version_label = new SidebarLabel ("Hamachi");
        version_entry = new SidebarEntry();
        
        ipv4_label    = new SidebarLabel (Text.address_ipv4);
        ipv4_entry    = new SidebarEntry();
        
        ipv6_label    = new SidebarLabel (Text.address_ipv6);
        ipv6_entry    = new SidebarEntry();
        
        id_label      = new SidebarLabel (Text.client_id);
        id_entry      = new SidebarEntry();
        
        account_label = new SidebarLabel (Text.account);
        account_entry = new SidebarEntry();
        
        nick_label    = new SidebarLabel (Text.nick);
        nick_entry    = new SidebarEntry();
        
        
        Grid info_grid = new Grid();
        info_grid.row_spacing = 9;
        info_grid.column_spacing = 9;
        info_grid.halign = Align.CENTER;
        info_grid.attach (version_label,  0, 1, 1, 1);
        info_grid.attach (version_entry,  1, 1, 1, 1);
        info_grid.attach (id_label,       0, 2, 1, 1);
        info_grid.attach (id_entry,       1, 2, 1, 1);
        info_grid.attach (account_label,  0, 3, 1, 1);
        info_grid.attach (account_entry,  1, 3, 1, 1);
        info_grid.attach (nick_label,     0, 4, 1, 1);
        info_grid.attach (nick_entry,     1, 4, 1, 1);
        info_grid.attach (ipv4_label,     0, 5, 1, 1);
        info_grid.attach (ipv4_entry,     1, 5, 1, 1);
        info_grid.attach (ipv6_label,     0, 6, 1, 1);
        info_grid.attach (ipv6_entry,     1, 6, 1, 1);
        
        info_box = new Box (Orientation.VERTICAL, 0);
        info_box.pack_start (info_grid, false, false, 0);
        
        
        attach_button = new Button.with_mnemonic (Text.attach_label);
        attach_button.clicked.connect (GlobalEvents.attach);
        
        info_action_bar = new ActionBar();
        info_action_bar.set_center_widget (attach_button);
        
        Revealer info_action_bar_revealer = (Revealer) info_action_bar.get_child();
        info_action_bar_revealer.set_transition_type (RevealerTransitionType.NONE);
        
        
        
        network_status_label   = new SidebarLabel (Text.status);
        network_status_entry   = new SidebarEntry();
        
        network_id_label       = new SidebarLabel (Text.network_id);
        network_id_entry       = new SidebarEntry();
        
        network_members_label  = new SidebarLabel (Text.members);
        network_members_entry  = new SidebarEntry();
        
        network_owner_label    = new SidebarLabel (Text.owner);
        network_owner_entry    = new SidebarEntry();
        
        network_capacity_label = new SidebarLabel (Text.capacity);
        network_capacity_entry = new SidebarEntry();
        
        network_lock_label     = new SidebarLabel (Text.locked);
        
        network_lock_check = new CheckButton();
        network_lock_check.halign = Align.START;
        
        
        network_approval_label = new SidebarLabel (Text.approval);
        
        CellRendererCombo cell = new CellRendererCombo();
        cell.ellipsize = Pango.EllipsizeMode.END;
        
        Gtk.ListStore store = new Gtk.ListStore (1, typeof (string));
        TreeIter iter;
        
        store.append (out iter);
        store.set (iter, 0, Text.automatically, -1);
        
        store.append (out iter);
        store.set (iter, 0, Text.manually, -1);
        
        network_approval_combo = new ComboBox();
        network_approval_combo.halign = Align.START;
        network_approval_combo.margin_start = 1;
        network_approval_combo.pack_start (cell, false);
        network_approval_combo.add_attribute (cell, "text", 0);
        network_approval_combo.model = store;
        
        
        network_password_button = new Button.with_mnemonic (Text.change_password_label);
        network_password_button.halign = Align.CENTER;
        network_password_button.margin_top = 24;
        network_password_button.margin_bottom = 6;
        network_password_button.clicked.connect (() =>
        {
            network.change_password();
        });
        
        
        Grid network_grid = new Grid();
        network_grid.row_spacing = 9;
        network_grid.column_spacing = 9;
        network_grid.halign = Align.CENTER;
        network_grid.attach (network_status_label,    0, 1, 1, 1);
        network_grid.attach (network_status_entry,    1, 1, 1, 1);
        network_grid.attach (network_id_label,        0, 2, 1, 1);
        network_grid.attach (network_id_entry,        1, 2, 1, 1);
        network_grid.attach (network_members_label,   0, 3, 1, 1);
        network_grid.attach (network_members_entry,   1, 3, 1, 1);
        network_grid.attach (network_owner_label,     0, 4, 1, 1);
        network_grid.attach (network_owner_entry,     1, 4, 1, 1);
        network_grid.attach (network_capacity_label,  0, 5, 1, 1);
        network_grid.attach (network_capacity_entry,  1, 5, 1, 1);
        network_grid.attach (network_lock_label,      0, 6, 1, 1);
        network_grid.attach (network_lock_check,      1, 6, 1, 1);
        network_grid.attach (network_approval_label,  0, 7, 1, 1);
        network_grid.attach (network_approval_combo,  1, 7, 1, 1);
        
        network_box = new Box (Orientation.VERTICAL, 0);
        network_box.vexpand = true;
        network_box.pack_start (network_grid,            false, false, 0);
        network_box.pack_start (network_password_button, false, false, 0);
        
        
        delete_button = new Button.with_mnemonic (Text.delete_label);
        delete_button.clicked.connect (() =>
        {
            network.delete();
        });
        
        leave_button = new Button.with_mnemonic (Text.leave_label);
        leave_button.clicked.connect (() =>
        {
            network.leave();
        });
        
        online_button = new Button.with_mnemonic (Text.go_online_label);
        online_button.get_style_context().add_class ("suggested-action");
        online_button.clicked.connect (() =>
        {
            network.go_online();
        });
        
        offline_button = new Button.with_mnemonic (Text.go_offline_label);
        offline_button.clicked.connect (() =>
        {
            network.go_offline();
        });
        
        SizeGroup network_actions_size_group = new SizeGroup (SizeGroupMode.HORIZONTAL);
        network_actions_size_group.add_widget (delete_button);
        network_actions_size_group.add_widget (leave_button);
        network_actions_size_group.add_widget (online_button);
        network_actions_size_group.add_widget (offline_button);
        
        network_action_bar = new ActionBar();
        network_action_bar.pack_start (online_button);
        network_action_bar.pack_start (offline_button);
        network_action_bar.pack_end (delete_button);
        network_action_bar.pack_end (leave_button);
        
        Revealer network_action_bar_revealer = (Revealer) network_action_bar.get_child();
        network_action_bar_revealer.set_transition_type (RevealerTransitionType.NONE);
        
        
        
        member_status_label     = new SidebarLabel (Text.status);
        member_status_entry     = new SidebarEntry();
        
        member_id_label         = new SidebarLabel (Text.client_id);
        member_id_entry         = new SidebarEntry();
        
        member_ipv4_label       = new SidebarLabel (Text.address_ipv4);
        member_ipv4_entry       = new SidebarEntry();
        
        member_ipv6_label       = new SidebarLabel (Text.address_ipv6);
        member_ipv6_entry       = new SidebarEntry();
        
        member_tunnel_label     = new SidebarLabel (Text.tunnel);
        member_tunnel_entry     = new SidebarEntry();
        
        member_connection_label = new SidebarLabel (Text.connection);
        member_connection_entry = new SidebarEntry();
        
        
        Grid member_grid = new Grid();
        member_grid.row_spacing = 9;
        member_grid.column_spacing = 9;
        member_grid.halign = Align.CENTER;
        member_grid.attach (member_status_label,     0, 1, 1, 1);
        member_grid.attach (member_status_entry,     1, 1, 1, 1);
        member_grid.attach (member_id_label,         0, 2, 1, 1);
        member_grid.attach (member_id_entry,         1, 2, 1, 1);
        member_grid.attach (member_ipv4_label,       0, 3, 1, 1);
        member_grid.attach (member_ipv4_entry,       1, 3, 1, 1);
        member_grid.attach (member_ipv6_label,       0, 4, 1, 1);
        member_grid.attach (member_ipv6_entry,       1, 4, 1, 1);
        member_grid.attach (member_tunnel_label,     0, 5, 1, 1);
        member_grid.attach (member_tunnel_entry,     1, 5, 1, 1);
        member_grid.attach (member_connection_label, 0, 6, 1, 1);
        member_grid.attach (member_connection_entry, 1, 6, 1, 1);
        
        commands_box = new Box (Orientation.VERTICAL, 6);
        commands_box.margin_top = 18;
        
        member_box = new Box (Orientation.VERTICAL, 0);
        member_box.pack_start (member_grid,  false, false, 0);
        member_box.pack_start (commands_box, false, false, 0);
        
        
        approve_button = new Button.with_mnemonic (Text.approve_label);
        approve_button.clicked.connect (() =>
        {
            member.approve();
        });
        
        reject_button = new Button.with_mnemonic (Text.reject_label);
        reject_button.clicked.connect (() =>
        {
            member.reject();
        });
        
        evict_button = new Button.with_mnemonic (Text.evict_label);
        evict_button.clicked.connect (() =>
        {
            member.evict();
        });
        
        SizeGroup member_actions_size_group = new SizeGroup (SizeGroupMode.HORIZONTAL);
        member_actions_size_group.add_widget (approve_button);
        member_actions_size_group.add_widget (reject_button);
        
        member_action_bar = new ActionBar();
        member_action_bar.set_center_widget (evict_button);
        member_action_bar.pack_start (approve_button);
        member_action_bar.pack_end (reject_button);
        
        Revealer member_action_bar_revealer = (Revealer) member_action_bar.get_child();
        member_action_bar_revealer.set_transition_type (RevealerTransitionType.NONE);
        
        
        
        scrolled_box = new Box (Orientation.VERTICAL, 0);
        scrolled_box.margin = 12;
        scrolled_box.pack_start (heading_box, false, false, 0);
        scrolled_box.pack_start (info_box,    true,  true,  0);
        scrolled_box.pack_start (network_box, true,  true,  0);
        scrolled_box.pack_start (member_box,  true,  true,  0);
        
        
        scrolled_window = new ScrolledWindow (null, null);
        scrolled_window.add (scrolled_box);
        scrolled_window.get_style_context().add_class ("sidebar");
        scrolled_window.set_policy (PolicyType.NEVER, PolicyType.AUTOMATIC);
        
        
        action_box = new Box (Orientation.VERTICAL, 0);
        action_box.pack_start (info_action_bar,    true, true, 0);
        action_box.pack_start (network_action_bar, true, true, 0);
        action_box.pack_start (member_action_bar,  true, true, 0);
        
        action_box_revealer = new Revealer();
        action_box_revealer.add (action_box);
        action_box_revealer.set_transition_type (RevealerTransitionType.NONE);
        
        
        pack_start (scrolled_window,     true,  true,  0);
        pack_start (action_box_revealer, false, false, 0);
        
        
        generate_command_buttons();
    }
    
    private void network_lock_changed ()
    {
        if (network_lock_check.active)
        {
            network.set_lock ("locked");
        }
        else
        {
            network.set_lock ("unlocked");
        }
    }
    
    private void network_approval_changed ()
    {
        if (network_approval_combo.active == 1)
        {
            network.set_approval ("manual");
        }
        else
        {
            network.set_approval ("auto");
        }
    }
    
    public void update ()
    {
        set_version();
        set_address();
        set_client_id();
    }
    
    public void generate_command_buttons ()
    {
        string[] commands = (string[]) Settings.custom_commands.val;
        
        foreach (Widget cb in commands_box.get_children())
        {
            cb.destroy();
        }
        
        foreach (string c in commands)
        {
            string[] cArray = c.split (";", 6);
            
            if ((cArray.length == 6) &&
                (cArray[0] == "true"))
            {
                string label        = cArray[2];
                string command_ipv4 = cArray[3];
                string command_ipv6 = cArray[4];
                string priority     = cArray[5];
                
                if (Command.custom_exists (command_ipv4, command_ipv6))
                {
                    CommandButton cb = new CommandButton (label, command_ipv4, command_ipv6, priority);
                    commands_box.pack_start (cb, false, false, 3);
                }
            }
        }
    }
    
    private void set_version ()
    {
        string version = Hamachi.version;
        
        if (version == null)
        {
            version = "<i>" + Text.unavailable + "</i>";
        }
        
        version_entry.set_markup (version);
    }
    
    private void set_address ()
    {
        string[] address = Hamachi.get_address();
        string ipv4 = address[0];
        string ipv6 = address[1];
        
        if ((ipv4 == null) ||
            (ipv4 == ""))
        {
            ipv4_label.hide();
            ipv4_entry.hide();
        }
        else
        {
            ipv4_entry.set_text (ipv4);
            
            ipv4_label.show();
            ipv4_entry.show();
        }
        
        if (ipv6 == null)
        {
            ipv6_label.hide();
            ipv6_entry.hide();
        }
        else
        {
            ipv6_entry.set_text (ipv6);
            
            ipv6_label.show();
            ipv6_entry.show();
        }
    }
    
    private void set_client_id ()
    {
        string id = Hamachi.get_client_id();
        
        if (id == "")
        {
            id_label.hide();
            id_entry.hide();
        }
        else
        {
            id_entry.set_text (id);
            
            id_label.show();
            id_entry.show();
        }
    }
    
    public void set_account (string? account)
    {
        if ((account == "") ||
            (account == "-"))
        {
            account_label.hide();
            account_entry.hide();
        }
        else
        {
            account_entry.set_markup (account.replace (" (pending)", "\n<small><i>(pending)</i></small>"));
            
            account_label.show();
            account_entry.show();
        }
    }
    
    public void set_nick (string _nick)
    {
        string nick = Markup.escape_text (_nick);
        
        if (nick == "")
        {
            nick = "<i>" + Text.anonymous + "</i>";
        }
        
        nick_entry.set_markup (nick);
    }
    
    public void set_attach (bool visible, bool sensitive)
    {
        attach_button.visible   = visible;
        attach_button.sensitive = sensitive;
        
        if (current_tab == "Info")
        {
            refresh_tab();
        }
    }
    
    public void set_member (Member? _member)
    {
        member = _member;
    }
    
    public void set_network (Network _network)
    {
        network = _network;
    }
    
    public void refresh_tab ()
    {
        show_tab (current_tab, true);
    }
    
    public void show_tab (string tab, bool refresh)
    {
        if ((tab == current_tab) &&
            (refresh == false))
        {
            return;
        }
        
        current_tab = tab;
        
        info_box.hide();
        info_action_bar.hide();
        
        network_box.hide();
        network_action_bar.hide();
        
        member_box.hide();
        member_action_bar.hide();
        
        switch (current_tab)
        {
            case "Info":
                info_box.show();
                info_action_bar.show();
                
                if (action_box_revealer.get_reveal_child() != attach_button.visible)
                {
                    action_box_revealer.set_reveal_child (attach_button.visible);
                }
                
                if (action_box_revealer.get_transition_type() == RevealerTransitionType.NONE)
                {
                    action_box_revealer.set_transition_type (RevealerTransitionType.SLIDE_UP);
                }
                
                heading_label.set_markup ("<span size=\"large\"><b>" + Text.information_title + "</b></span>");
                heading_label.selectable = false;
                
                break;
                
            case "Network":
                if (!Haguichi.connection.has_network (network))
                {
                    show_tab ("Info", false);
                    return;
                }
                
                network_box.show_all();
                network_action_bar.show_all();
                
                action_box_revealer.set_reveal_child (true);
                
                int member_count, member_online_count;
                network.return_member_count (out member_count, out member_online_count);
                
                network_status_entry.set_text   (network.status.status_text);
                network_id_entry.set_text       (network.id);
                network_members_entry.set_text  (Utils.format (Text.member_count, member_online_count.to_string(), member_count.to_string(), null));
                network_owner_entry.set_text    (Markup.escape_text (network.return_owner_string()));
                network_capacity_entry.set_text (network.capacity.to_string());
                
                
                if (network.status.status_int == 1)
                {
                    online_button.hide();
                }
                else
                {
                    offline_button.hide();
                }
                
                if (network.capacity == 0)
                {
                    network_capacity_label.hide();
                    network_capacity_entry.hide();
                }
                
                if (network.is_owner == 1)
                {
                    network_lock_check.toggled.disconnect (network_lock_changed);
                    network_approval_combo.changed.disconnect (network_approval_changed);
                    
                    if (network.lock_state == "unlocked")
                    {
                        network_lock_check.active = false;
                    }
                    if (network.lock_state == "locked")
                    {
                        network_lock_check.active = true;
                    }
                    
                    if (network.approve == "auto")
                    {
                        network_approval_combo.active = 0;
                    }
                    if (network.approve == "manual")
                    {
                        network_approval_combo.active = 1;
                    }
                    
                    network_lock_check.toggled.connect (network_lock_changed);
                    network_approval_combo.changed.connect (network_approval_changed);
                    
                    leave_button.hide();
                }
                else
                {
                    network_approval_label.hide();
                    network_approval_combo.hide();
                    network_lock_label.hide();
                    network_lock_check.hide();
                    network_password_button.hide();
                    
                    delete_button.hide();
                }
                
                heading_label.set_markup ("<span size=\"large\"><b>" + Markup.escape_text (network.name) + "</b></span>");
                heading_label.selectable = true;
                
                break;
                
            case "Member":
                member_box.show_all();
                member_action_bar.show_all();
                
                action_box_revealer.set_reveal_child (true);
                
                member_status_entry.set_text     (member.status.status_text);
                member_id_entry.set_text         (member.client_id);
                member_ipv4_entry.set_text       (member.ipv4);
                member_ipv6_entry.set_text       (member.ipv6);
                member_tunnel_entry.set_text     (member.tunnel);
                member_connection_entry.set_text (member.status.connection_type);
                
                if (member.ipv4 == null)
                {
                    member_ipv4_label.hide();
                    member_ipv4_entry.hide();
                }
                if (member.ipv6 == null)
                {
                    member_ipv6_label.hide();
                    member_ipv6_entry.hide();
                }
                if (member.tunnel == null)
                {
                    member_tunnel_label.hide();
                    member_tunnel_entry.hide();
                }
                if (member.status.connection_type == "")
                {
                    member_connection_label.hide();
                    member_connection_entry.hide();
                }
                
                heading_label.set_markup ("<span size=\"large\"><b>" + Markup.escape_text (member.nick) + "</b></span>");
                heading_label.selectable = true;
                
                if (member.status.status_int != 3)
                {
                    approve_button.hide();
                    reject_button.hide();
                    
                    commands_box.show_all();
                    
                    foreach (Widget cb in commands_box.get_children())
                    {
                        cb.sensitive = true;
                        
                        if (member.status.status_int != 1)
                        {
                            cb.sensitive = false;
                        }
                    }
                }
                else
                {
                    commands_box.hide();
                }
                
                if ((network.is_owner == 0) ||
                    (member.status.status_int == 3))
                {
                    evict_button.hide();
                }
                
                if ((network.is_owner == 0) &&
                    (member.status.status_int != 3))
                {
                    action_box_revealer.set_reveal_child (false);
                }
                
                break;
        }
    }
}
