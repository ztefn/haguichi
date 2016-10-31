/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

public class CommandsEditor : Box
{
    private Box box;
    
    private Gtk.ListStore store;
    private TreeView tv;
    
    private CellRendererText text_cell;
    private CellRendererPixbuf default_cell;
    private CellRendererToggle toggle_cell;

    private Button add_but;
    private Button remove_but;
    private Button up_but;
    private Button down_but;
    private Button edit_but;
    private Button default_but;
    private Button revert_but;
    
    private int active_column;
    private int default_column;
    private int label_column;
    private int command_ipv4_column;
    private int command_ipv6_column;
    private int priority_column;
    private int view_column;
    
    public CommandsEditor ()
    {
        orientation         = Orientation.VERTICAL;
        active_column       = 0;
        default_column      = 1;
        label_column        = 2;
        command_ipv4_column = 3;
        command_ipv6_column = 4;
        priority_column     = 5;
        view_column         = 6;
        
        store = new Gtk.ListStore (7,               // Num column
                               typeof (bool),       // Active
                               typeof (bool),       // Default
                               typeof (string),     // Label
                               typeof (string),     // IPv4 command
                               typeof (string),     // IPv6 command
                               typeof (string),     // Priority
                               typeof (string));    // View
        
        tv = new TreeView.with_model (store);
        
        text_cell = new CellRendererText();
        
        default_cell = new CellRendererPixbuf();
        default_cell.xpad = 6;
        
        toggle_cell = new CellRendererToggle();
        toggle_cell.activatable = true;
        toggle_cell.xpad = 6;
        toggle_cell.toggled.connect (enable_command_toggled);
        
        
        TreeViewColumn column1 = new TreeViewColumn();
        TreeViewColumn column2 = new TreeViewColumn();
        
        column1.pack_start (toggle_cell, false);
        
        column2.pack_start (text_cell, true);
        column2.pack_start (default_cell, false);
        
        column2.set_cell_data_func (text_cell, text_cell_layout);
        column2.set_cell_data_func (default_cell, default_cell_layout);
        
        column1.add_attribute (toggle_cell, "active", active_column);
        column2.add_attribute (text_cell, "text", label_column);
        
        tv.append_column (column1);
        tv.append_column (column2);

        tv.headers_visible = false;
        tv.reorderable = true;
        tv.enable_search = false;
        tv.drag_end.connect (handle_drag_end);
        tv.realize.connect (set_button_sensitivity_void);
        tv.button_release_event.connect (set_button_sensitivity);
        tv.key_release_event.connect (set_button_sensitivity);
        tv.row_activated.connect (on_row_activate);
        
        
        Image add_img = new Image();
        add_img.set_from_icon_name ("list-add-symbolic", IconSize.MENU);
        
        add_but = new Button();
        add_but.image = add_img;
        add_but.clicked.connect (add_command);
        add_but.tooltip_markup = Text.add_tip;
        
        Image remove_img = new Image();
        remove_img.set_from_icon_name ("list-remove-symbolic", IconSize.MENU);
        
        remove_but = new Button();
        remove_but.image = remove_img;
        remove_but.clicked.connect (remove_command);
        remove_but.tooltip_markup = Text.remove_tip;
        
        Image up_img = new Image();
        up_img.set_from_icon_name ("go-up-symbolic", IconSize.MENU);
        
        up_but = new Button();
        up_but.image = up_img;
        up_but.clicked.connect (move_up);
        up_but.tooltip_markup = Text.move_up_tip;
        
        Image down_img = new Image();
        down_img.set_from_icon_name ("go-down-symbolic", IconSize.MENU);
        
        down_but = new Button();
        down_but.image = down_img;
        down_but.clicked.connect (move_down);
        down_but.tooltip_markup = Text.move_down_tip;
        
        Image edit_img = new Image();
        edit_img.set_from_icon_name ("system-run-symbolic", IconSize.MENU);
        
        edit_but = new Button();
        edit_but.image = edit_img;
        edit_but.clicked.connect (edit_command);
        edit_but.tooltip_markup = Text.edit_tip;
        
        Image default_img = new Image();
        default_img.set_from_icon_name ("emblem-default-symbolic", IconSize.MENU);
        
        default_but = new Button();
        default_but.image = default_img;
        default_but.clicked.connect (set_default);
        default_but.tooltip_markup = Text.default_tip;
        
        Box left_box = new Box (Orientation.HORIZONTAL, 0);
        left_box.pack_start (add_but,     false, false, 0);
        left_box.pack_start (remove_but,  false, false, 0);
        left_box.pack_start (up_but,      false, false, 0);
        left_box.pack_start (down_but,    false, false, 0);
        left_box.pack_start (edit_but,    false, false, 0);
        left_box.pack_start (default_but, false, false, 0);
        
        ToolItem left_ti = new ToolItem();
        left_ti.add (left_box);


        Image revert_img = new Image();
        revert_img.set_from_icon_name ("edit-undo-symbolic", IconSize.MENU);
        
        revert_but = new Button();
        revert_but.image = revert_img;
        revert_but.clicked.connect (revert_commands);
        revert_but.tooltip_markup = Text.revert_tip;
        
        Box right_box = new Box (Orientation.HORIZONTAL, 0);
        right_box.pack_start (revert_but, false, false, 0);
        
        ToolItem right_ti = new ToolItem();
        right_ti.add (right_box);


        SeparatorToolItem sep = new SeparatorToolItem();
        sep.draw = false;
        sep.set_expand (true);


        Toolbar tb = new Toolbar();
        tb.get_style_context().add_class ("inline-toolbar");
        tb.add (left_ti);
        tb.add (sep);
        tb.add (right_ti);
        
        
        ScrolledWindow sw = new ScrolledWindow (null, null);
        sw.shadow_type = ShadowType.ETCHED_IN;
        sw.get_style_context().set_junction_sides (JunctionSides.BOTTOM);
        sw.hscrollbar_policy = PolicyType.NEVER;
        sw.vscrollbar_policy = PolicyType.AUTOMATIC;
        sw.add (tv);
        
        
        box = new Box (Orientation.VERTICAL, 0);
        box.pack_start (sw, true,  true,  0);
        box.pack_start (tb, false, false, 0);
        
        
        pack_start (box, true, true, 0);
        border_width = 12;
        
        fill();
    }
    
    private void move_up ()
    {
        TreeIter selected;
        TreeModel model;
        
        if (tv.get_selection().get_selected (out model, out selected))
        {
            TreeIter prev = selected;
            
            if ((model as Gtk.ListStore).iter_previous (ref prev))
            {
                (model as Gtk.ListStore).swap (selected, prev);
            }
        }
        
        update_commands();
    }
    
    private void move_down ()
    {
        TreeIter selected;
        TreeModel model;
        
        if (tv.get_selection().get_selected (out model, out selected))
        {
            TreeIter next = selected;
            
            if ((model as Gtk.ListStore).iter_next (ref next))
            {
                (model as Gtk.ListStore).swap (selected, next);
            }
        }
        
        update_commands();
    }
    
    private void revert_commands ()
    {
        string heading = Text.revert_heading;
        string message = Text.revert_message;
            
        Dialogs.Confirm dlg = new Dialogs.Confirm (Haguichi.preferences_dialog, heading, message, MessageType.QUESTION, Text.revert_label);
        
        if (dlg.response_id == ResponseType.OK)
        {
            Settings.custom_commands.val = Settings.custom_commands.get_default_value();
            
            store.clear();
            fill();
            update_commands();
        }
        dlg.destroy();
    }
    
    private void set_default ()
    {
        TreeIter iter = TreeIter();
        
        if (store.get_iter_first (out iter))                           // First command
        {
            store.set_value (iter, default_column, false);
            
            while (store.iter_next (ref iter))                         // All other commands
            {
                store.set_value (iter, default_column, false);
            }
        }
        
        
        TreeIter selected;
        TreeModel model;
        
        if (tv.get_selection().get_selected (out model, out selected))
        {            
            store.set_value (selected, default_column, true);
        }
        
        update_commands();
    }
    
    private void remove_command ()
    {
        TreeIter selected;
        TreeModel model;
        
        if (tv.get_selection().get_selected (out model, out selected))
        {
            store.remove (selected);
        }
        
        update_commands();
    }
    
    public void update_selected_command (string label, string command_ipv4, string command_ipv6, string priority)
    {
        TreeIter selected;
        TreeModel model;
        
        if (tv.get_selection().get_selected (out model, out selected))
        {
            store.set (selected,
                       2, label,
                       3, command_ipv4,
                       4, command_ipv6,
                       5, priority,
                       6, _(label),
                       -1);
        }
        
        update_commands();
    }
    
    public void insert_command (string label, string command_ipv4, string command_ipv6, string priority)
    {
        TreeIter iter;
        store.append (out iter);
        store.set (iter,
                   0, true,
                   1, false,
                   2, label,
                   3, command_ipv4,
                   4, command_ipv6,
                   5, priority,
                   6, _(label),
                   -1);
        
        update_commands();
    }
    
    public void edit_command ()
    {
        TreeIter selected;
        TreeModel model;
        
        if (tv.get_selection().get_selected (out model, out selected))
        {
            Value label;           store.get_value (selected, view_column, out label);
            Value command_ipv4;    store.get_value (selected, command_ipv4_column, out command_ipv4);
            Value command_ipv6;    store.get_value (selected, command_ipv6_column, out command_ipv6);
            Value priority;        store.get_value (selected, priority_column, out priority);
            
            new Dialogs.AddEditCommand ("Edit", this, (string) label, (string) command_ipv4, (string) command_ipv6, (string) priority);
        }
    }
    
    private void add_command ()
    {
        new Dialogs.AddEditCommand ("Add", this, "", "", "", "IPv4");
    }
    
    private void handle_drag_end ()
    {
        update_commands();
    }
    
    public void fill ()
    {
        string[] commands = (string[]) Settings.custom_commands.val;
        
        foreach (string command in commands)
        {
            string[] parts = command.split (";", 6);
            
            if (parts.length == 6)
            {
                bool is_active = false;
                if (parts[0] == "true")
                {
                    is_active = true;
                }
                
                bool is_default = false;
                if (parts[1] == "true")
                {
                    is_default = true;
                }
                
                string command_ipv4 = parts[3];
                string command_ipv6 = parts[4];
                string priority     = parts[5];
                
                command_ipv4 = command_ipv4.replace ("{COLON}", ";");
                command_ipv6 = command_ipv6.replace ("{COLON}", ";");
                
                TreeIter iter;
                store.append (out iter);
                store.set (iter,
                           0, is_active,
                           1, is_default,
                           2, parts[2],
                           3, command_ipv4,
                           4, command_ipv6,
                           5, priority,
                           6,_(parts[2]),
                           -1);
            }
        }
    }
    
    private void enable_command_toggled (string path)
    {
        TreeIter iter;

        if (store.get_iter (out iter, new TreePath.from_string (path)))
        {
            Value old;
            store.get_value (iter, active_column, out old);
            store.set_value (iter, active_column, !(bool) old);
            
            update_commands();
        }
    }
    
    private void update_commands ()
    {
        set_button_sensitivity();
        
        Settings.custom_commands.val = compose_commands_string();
        
        Haguichi.window.network_view.generate_popup_menus();
        HaguichiWindow.sidebar.generate_command_buttons();
        HaguichiWindow.sidebar.refresh_tab();
    }
    
    private string[] compose_commands_string ()
    {
        string[] commands_string = {};
        TreeIter iter = TreeIter();
        
        if (store.get_iter_first (out iter))                           // First command
        {
            commands_string += compose_command_string (iter);
            
            while (store.iter_next (ref iter))                         // All other commands
            {
                commands_string += compose_command_string (iter);
            }
        }
        
        return commands_string;
    }
    
    private string compose_command_string (TreeIter iter)
    {
        Value active_val;          store.get_value (iter, active_column, out active_val);
        Value default_val;         store.get_value (iter, default_column, out default_val);
        Value label_val;           store.get_value (iter, label_column, out label_val);
        Value command_ipv4_val;    store.get_value (iter, command_ipv4_column, out command_ipv4_val);
        Value command_ipv6_val;    store.get_value (iter, command_ipv6_column, out command_ipv6_val);
        Value priority_val;        store.get_value (iter, priority_column, out priority_val);
        
        string is_active  = (bool) active_val  ? "true" : "false";
        string is_default = (bool) default_val ? "true" : "false";
        
        string command_ipv4 = ((string) command_ipv4_val).replace (";", "{COLON}");
        string command_ipv6 = ((string) command_ipv6_val).replace (";", "{COLON}");
        
        return is_active + ";" + is_default + ";" + (string) label_val + ";" + command_ipv4 + ";" + command_ipv6 + ";" + (string) priority_val;
    }
    
    private void default_cell_layout (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
    {
        CellRendererPixbuf default_cell = (cell as CellRendererPixbuf);
        default_cell.follow_state = true;
        
        Value default_val;
        model.get_value (iter, default_column, out default_val);
        
        if ((bool) default_val)
        {
            default_cell.icon_name = "emblem-default-symbolic";
        }
        else
        {
            default_cell.icon_name = "";
        }
        
        Value active_val;
        model.get_value (iter, active_column, out active_val);
        
        if ((bool) active_val)
        {
            default_cell.sensitive = true;
        }
        else
        {
            default_cell.sensitive = false;
        }
    }
    
    private void text_cell_layout (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
    {
        CellRendererText text_cell = (cell as CellRendererText);
        
        StyleContext context = tv.get_style_context();
        context.save();
        
        context.set_state (StateFlags.NORMAL);
        Gdk.RGBA active_txt_color = context.get_color (context.get_state());
        
        context.set_state (StateFlags.INSENSITIVE);
        Gdk.RGBA inactive_txt_color = context.get_color (context.get_state());
        
        context.restore();
        
        Value active_val;
        model.get_value (iter, active_column, out active_val);
        
        if ((bool) active_val)
        {
            text_cell.foreground_rgba = active_txt_color;
        }
        else
        {
            text_cell.foreground_rgba = inactive_txt_color;
        }
        
        Value title_val;
        model.get_value (iter, view_column, out title_val);
        
        Value priority_val;
        model.get_value (iter, priority_column, out priority_val);
        
        string title    = ((string) title_val).replace ("_", "");
        string command  = "";
        string address  = "";
        string priority = (string) priority_val;
        
        if (priority == "IPv4")
        {
            Value command_val;
            model.get_value (iter, command_ipv4_column, out command_val);
            command = (string) command_val;
            address = "25.123.456.78";
        }
        if (priority == "IPv6")
        {
            Value command_val;
            model.get_value (iter, command_ipv6_column, out command_val);
            command = (string) command_val;
            address = "2620:9b::56d:f78e";
        }
        
        command = Command.replace_variables (command, address, "Nick", "090-123-456");
        
        text_cell.markup = Utils.format ("<b>{0}</b>\n{1}", Markup.escape_text (title), Markup.escape_text (command), null);
    }
    
    private void set_button_sensitivity_void ()
    {
        set_button_sensitivity();
    }
    
    private bool set_button_sensitivity ()
    {
        if (string.joinv ("", compose_commands_string()) == string.joinv ("", (string[]) Settings.custom_commands.get_default_value()))
        {
            revert_but.sensitive = false;
        }
        else
        {
            revert_but.sensitive = true;
        }
        
        if (tv.get_selection().count_selected_rows() > 0)
        {
            default_but.sensitive = true;
            edit_but.sensitive    = true;
            remove_but.sensitive  = true;

            TreeIter selected;
            TreeModel model;
            
            if (tv.get_selection().get_selected (out model, out selected))
            {
                Value default_val;
                store.get_value (selected, default_column, out default_val);
                
                if ((bool) default_val)
                {
                    default_but.sensitive = false;
                }
                else
                {
                    default_but.sensitive = true;
                }
                
                TreePath path = model.get_path (selected);
                
                if (path.prev())
                {
                    up_but.sensitive = true;
                }
                else
                {
                    up_but.sensitive = false;
                }
                
                TreeIter next = selected;
                
                if ((model as Gtk.ListStore).iter_next (ref next))
                {
                    down_but.sensitive = true;
                }
                else
                {
                    down_but.sensitive = false;
                }
            }
        }
        else
        {
            default_but.sensitive = false;
            edit_but.sensitive    = false;
            remove_but.sensitive  = false;
            up_but.sensitive      = false;
            down_but.sensitive    = false;
        }
        
        return false;
    }
    
    private void on_row_activate ()
    {
        edit_command();
    }
}
