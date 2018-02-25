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

public class CommandsEditorRow : ListBoxRow
{
    public bool   is_active;
    public bool   is_default;
    public string label;
    public string command_ipv4;
    public string command_ipv6;
    public string priority;
    
    private CommandsEditor editor;
    private Label title;
    private Label preview;
    private Image img;
    private Switch swh;
    
    public CommandsEditorRow (CommandsEditor _editor, bool _is_active, bool _is_default, string _label, string _command_ipv4, string _command_ipv6, string _priority)
    {
        editor       = _editor;
        is_active    = _is_active;
        is_default   = _is_default;
        
        activatable  = true;
        selectable   = true;
        
        
        title = new Label (null);
        title.xalign = 0.0f;
        
        preview = new Label (null);
        preview.xalign = 0.0f;
        preview.get_style_context().add_class ("dim-label");
        
        Box label_box = new Box (Orientation.VERTICAL, 0);
        label_box.pack_start (title,   false, false, 0);
        label_box.pack_end   (preview, false, false, 0);
        label_box.valign = Align.CENTER;
        
        img = new Image();
        
        swh = new Switch();
        swh.active = is_active;
        swh.valign = Align.CENTER;
        swh.state_set.connect ((state) =>
        {
            is_active = state;
            editor.update_commands();
            return false;
        });
        
        Box box = new Box (Orientation.HORIZONTAL, 0);
        box.border_width = 6;
        box.pack_start (label_box, false, false, 6);
        box.pack_start (img,       false, false, 6);
        box.pack_end   (swh,       false, false, 6);
        
        add (box);
        
        
        update (_label, _command_ipv4, _command_ipv6, _priority);
        set_default (is_default);
        
        show_all();
    }
    
    public void update (string _label, string _command_ipv4, string _command_ipv6, string _priority)
    {
        label        = _label;
        command_ipv4 = _command_ipv4;
        command_ipv6 = _command_ipv6;
        priority     = _priority;
        
        string command = (priority == "IPv6") ? command_ipv6 : command_ipv4;
        string address = (priority == "IPv6") ? "2620:9b::56d:f78e" : "25.123.456.78";
        
        title.label   = Utils.remove_mnemonics (label);
        preview.label = Command.replace_variables (command, address, "Nick", "090-123-456");
    }
    
    public void set_default (bool _is_default)
    {
        is_default = _is_default;
        
        if (is_default)
        {
            img.set_from_icon_name (Utils.get_available_theme_icon (editor.default_icon_names), IconSize.MENU);
        }
        else
        {
            img.clear();
        }
    }
}
