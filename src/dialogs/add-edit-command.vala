/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

namespace Dialogs
{
#if FOR_ELEMENTARY
    public class AddEditCommand : Granite.Dialog
#else
    public class AddEditCommand : Dialog
#endif
    {
        private string mode;
        
        private CommandsEditor editor;
        
        private Label label_label;
        private Entry label_entry;
        
        private Label command_ipv4_label;
        private Entry command_ipv4_entry;
        
        private Label command_ipv6_label;
        private Entry command_ipv6_entry;
        
        private Label priority_label;
        private RadioButton priority_group;
        private RadioButton priority_ipv4;
        private RadioButton priority_ipv6;
        
        private Button cancel_but;
        private Button save_but;
        
        public AddEditCommand (string _mode, CommandsEditor _editor, string label, string command_ipv4, string command_ipv6, string priority)
        {
            Object (transient_for: Haguichi.preferences_dialog,
                    modal: true,
                    resizable: false,
                    use_header_bar: (int) Haguichi.dialog_use_header_bar);
            
            mode   = _mode;
            editor = _editor;
            
            
            cancel_but = (Button) add_button (Text.cancel_label, ResponseType.CANCEL);
            
            if (mode == "Add")
            {
                title = Text.add_command_title;
                save_but = (Button) add_button (Text.add_label, ResponseType.OK);
            }
            if (mode == "Edit")
            {
                title = Text.edit_command_title;
                save_but = (Button) add_button (Text.save_label, ResponseType.OK);
            }
            save_but.can_default = true;
            save_but.get_style_context().add_class ("suggested-action");
            save_but.realize.connect (check_entry_lengths);
            
            
            label_entry = new Entry();
            label_entry.activates_default = true;
            label_entry.width_chars = 36;
            label_entry.text = label;
            label_entry.changed.connect (check_entry_lengths);
            
            label_label = new Label.with_mnemonic (Utils.remove_colons (Text.label_label) + " ");
            label_label.halign = Align.END;
            label_label.mnemonic_widget = label_entry;
            
            
            command_ipv4_entry = new Entry();
            command_ipv4_entry.activates_default = true;
            command_ipv4_entry.width_chars = 36;
            command_ipv4_entry.text = command_ipv4;
            command_ipv4_entry.changed.connect (check_entry_lengths);
            
            command_ipv4_label = new Label.with_mnemonic (Utils.remove_colons (Text.command_ipv4_label) + " ");
            command_ipv4_label.halign = Align.END;
            command_ipv4_label.mnemonic_widget = command_ipv4_entry;
            
            
            command_ipv6_entry = new Entry();
            command_ipv6_entry.activates_default = true;
            command_ipv6_entry.width_chars = 36;
            command_ipv6_entry.text = command_ipv6;
            command_ipv6_entry.changed.connect (check_entry_lengths);
            
            command_ipv6_label = new Label.with_mnemonic (Utils.remove_colons (Text.command_ipv6_label) + " ");
            command_ipv6_label.halign = Align.END;
            command_ipv6_label.mnemonic_widget = command_ipv6_entry;
            
            
            Label hint_label = new Label (Text.command_hint);
            hint_label.halign = Align.START;
            hint_label.get_style_context().add_class ("dim-label");
            
            
            priority_ipv4 = new RadioButton.with_label (null, "IPv4");
            priority_ipv6 = new RadioButton.with_label_from_widget (priority_ipv4, "IPv6");
            if (priority == "IPv6")
            {
                priority_ipv6.active = true;
            }
            else
            {
                priority_ipv4.active = true;
            }
            
            Box priority_box = new Box (Orientation.HORIZONTAL, 0);
            priority_box.pack_start (priority_ipv4, false, false, 0);
            priority_box.pack_start (priority_ipv6, false, false, 9);
            
            priority_label = new Label.with_mnemonic (Utils.remove_colons (Text.priority_label) + " ");
            priority_label.halign = Align.END;
            priority_label.mnemonic_widget = priority_group;
            
            
            Grid grid = new Grid();
            grid.row_spacing = 6;
            grid.column_spacing = 6;
            grid.attach (label_label,        0, 1, 1, 1);
            grid.attach (label_entry,        1, 1, 1, 1);
            grid.attach (command_ipv4_label, 0, 2, 1, 1);
            grid.attach (command_ipv4_entry, 1, 2, 1, 1);
            grid.attach (command_ipv6_label, 0, 3, 1, 1);
            grid.attach (command_ipv6_entry, 1, 3, 1, 1);
            grid.attach (hint_label,         1, 4, 1, 1);
            grid.attach (priority_label,     0, 5, 1, 1);
            grid.attach (priority_box,       1, 5, 1, 1);
            grid.margin = 12;
            
            
            get_content_area().border_width = 0;
            get_content_area().add (grid);
            
            Utils.set_action_area_margins (cancel_but.parent);
            
            save_but.grab_default();
            
            show_all();
            
            response.connect ((response_id) =>
            {
                if (response_id == ResponseType.OK)
                {
                    if (mode == "Add")
                    {
                        editor.insert_command (label_entry.text, command_ipv4_entry.text, command_ipv6_entry.text, get_priority());
                    }
                    if (mode == "Edit")
                    {
                        editor.update_selected_command (label_entry.text, command_ipv4_entry.text, command_ipv6_entry.text, get_priority());
                    }
                }
                
                dismiss();
            });
        }
        
        private void check_entry_lengths ()
        {
            string label        = label_entry.get_chars (0, -1);
            string command_ipv4 = command_ipv4_entry.get_chars (0, -1);
            string command_ipv6 = command_ipv6_entry.get_chars (0, -1);
            
            if ((label.length > 0) &&
                ((command_ipv4.length > 0) ||
                 (command_ipv6.length > 0)))
            {
                save_but.sensitive = true;
                save_but.grab_default();
            }
            else
            {
                save_but.sensitive = false;
            }
        }
        
        private void dismiss ()
        {
            destroy();
        }
        
        private string get_priority ()
        {
            if (priority_ipv6.active)
            {
                return "IPv6";
            }
            else
            {
                return "IPv4";
            }
        }
    }
}
