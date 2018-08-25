/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2018 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

public class CommandsEditor : Box
{
    private ListBox list_box;
    
    private Button add_but;
    private Button remove_but;
    private Button up_but;
    private Button down_but;
    private Button edit_but;
    private Button default_but;
    private Button revert_but;
    
    public string[] default_icon_names = {"emblem-default-symbolic", "emblem-ok-symbolic", "emblem-checked", "checkmark"};
    
    public CommandsEditor ()
    {
        border_width = 12;
        orientation  = Orientation.VERTICAL;
        
        get_style_context().add_class ("commands-editor");
        
        
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
        edit_img.set_from_icon_name ("emblem-system-symbolic", IconSize.MENU);
        
        edit_but = new Button();
        edit_but.image = edit_img;
        edit_but.clicked.connect (edit_command);
        edit_but.tooltip_markup = Text.edit_tip;
        
        Image default_img = new Image();
        default_img.set_from_icon_name (Utils.get_available_theme_icon (default_icon_names), IconSize.MENU);
        
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
        
        
        list_box = new ListBox();
        list_box.activate_on_single_click = false;
        list_box.realize.connect (set_button_sensitivity);
        list_box.row_activated.connect (on_row_activate);
        list_box.row_selected.connect (on_row_selected);
        list_box.set_header_func (add_row_header);
        
        
        ScrolledWindow sw = new ScrolledWindow (null, null);
        sw.shadow_type = ShadowType.ETCHED_IN;
        sw.get_style_context().set_junction_sides (JunctionSides.BOTTOM);
        sw.hscrollbar_policy = PolicyType.NEVER;
        sw.vscrollbar_policy = PolicyType.AUTOMATIC;
        sw.add (list_box);
        
        
        pack_start (sw, true,  true,  0);
        pack_start (tb, false, false, 0);
    }
    
    private void move_up ()
    {
        move_row (-1);
    }
    
    private void move_down ()
    {
        move_row (1);
    }
    
    private void move_row (int offset)
    {
        var row = get_selected_row();
        var index = row.get_index();
        
        // Remove the row and insert at new index
        list_box.remove (row);
        list_box.insert (row, index + offset);
        
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
            
            clear();
            fill();
            update_commands();
        }
        
        dlg.destroy();
    }
    
    private void set_default ()
    {
        foreach (Widget widget in list_box.get_children())
        {
            var row = widget as CommandsEditorRow;
            
            if (row != null)
            {
                row.set_default (row.is_selected());
            }
        }
        
        update_commands();
    }
    
    private void remove_command ()
    {
        var row = get_selected_row();
        var index = row.get_index();
        
        list_box.remove (row);
        
        // After removing select the current row at this index if present, and otherwise select the previous row
        if (list_box.get_row_at_index (index) != null)
        {
            list_box.select_row (list_box.get_row_at_index (index));
        }
        else
        {
            list_box.select_row (list_box.get_row_at_index (index - 1));
        }
        
        update_commands();
    }
    
    public void update_selected_command (string label, string command_ipv4, string command_ipv6, string priority)
    {
        get_selected_row().update (label, command_ipv4, command_ipv6, priority);
        
        update_commands();
    }
    
    public void insert_command (string label, string command_ipv4, string command_ipv6, string priority)
    {
        var row = new CommandsEditorRow (this, true, false, label, command_ipv4, command_ipv6, priority);
        
        list_box.add (row);
        list_box.select_row (row);
        
        update_commands();
    }
    
    public void edit_command ()
    {
        var row = get_selected_row();
        
        new Dialogs.AddEditCommand ("Edit", this, _(row.label), row.command_ipv4, row.command_ipv6, row.priority);
    }
    
    private void add_command ()
    {
        new Dialogs.AddEditCommand ("Add", this, "", "", "", "IPv4");
    }
    
    public void add_row_header (ListBoxRow row, ListBoxRow? before)
    {
        row.set_header ((before == null) ? null : new Separator (Orientation.HORIZONTAL));
    }
    
    private CommandsEditorRow? get_selected_row ()
    {
        foreach (Widget widget in list_box.get_children())
        {
            var row = widget as CommandsEditorRow;
            
            if ((row != null) &&
                (row.is_selected()))
            {
                return row;
            }
        }
        
        return null;
    }
    
    private void clear ()
    {
        foreach (Widget widget in list_box.get_children())
        {
            var row = widget as CommandsEditorRow;
            
            if (row != null)
            {
                list_box.remove (row);
            }
        }
    }
    
    public void fill ()
    {
        string[] commands = (string[]) Settings.custom_commands.val;
        
        foreach (string command in commands)
        {
            string[] parts = command.split (";", 6);
            
            if (parts.length == 6)
            {
                bool is_active  = bool.parse (parts[0]);
                bool is_default = bool.parse (parts[1]);
                
                string command_ipv4 = parts[3];
                string command_ipv6 = parts[4];
                string priority     = parts[5];
                
                command_ipv4 = command_ipv4.replace ("{COLON}", ";");
                command_ipv6 = command_ipv6.replace ("{COLON}", ";");
                
                list_box.add (new CommandsEditorRow (this, is_active, is_default, parts[2], command_ipv4, command_ipv6, priority));
            }
        }
    }
    
    public void update_commands ()
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
        
        foreach (Widget widget in list_box.get_children())
        {
            var row = widget as CommandsEditorRow;
            
            if (row != null)
            {
                commands_string += compose_command_string (row);
            }
        }
        
        return commands_string;
    }
    
    private string compose_command_string (CommandsEditorRow row)
    {
        string command_ipv4 = row.command_ipv4.replace (";", "{COLON}");
        string command_ipv6 = row.command_ipv6.replace (";", "{COLON}");
        
        return row.is_active.to_string() + ";" + row.is_default.to_string() + ";" + row.label + ";" + command_ipv4 + ";" + command_ipv6 + ";" + row.priority.to_string();
    }
    
    private void set_button_sensitivity ()
    {
        remove_but.sensitive  = false;
        up_but.sensitive      = false;
        down_but.sensitive    = false;
        edit_but.sensitive    = false;
        default_but.sensitive = false;
        revert_but.sensitive  = (string.joinv ("", compose_commands_string()) != string.joinv ("", (string[]) Settings.custom_commands.get_default_value()));
        
        var row = get_selected_row();
        
        if (row != null)
        {
            remove_but.sensitive  = true;
            up_but.sensitive      = (list_box.get_row_at_index (row.get_index() - 1) != null);
            down_but.sensitive    = (list_box.get_row_at_index (row.get_index() + 1) != null);
            edit_but.sensitive    = true;
            default_but.sensitive = !row.is_default;
        }
    }
    
    private void on_row_activate ()
    {
        edit_command();
    }
    
    private void on_row_selected ()
    {
        set_button_sensitivity();
    }
}
