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
using Widgets;

namespace Dialogs
{
    public class JoinCreateNetwork : Dialog
    {
        private string mode;
        
        private string network_id;
        private string network_password;
        
        private MessageBar message_bar;
        
        private Label heading;
        
        private Label id_label;
        private Entry id_entry;
        
        private Label password_label;
        private Entry password_entry;
        
        private Button cancel_but;
        private Button ok_but;
        
        public JoinCreateNetwork (string _mode, string _title)
        {
            Object (title: _title,
                    transient_for: Haguichi.window,
                    modal: true,
                    resizable: false,
                    use_header_bar: (int) Haguichi.dialog_use_header_bar);
            
            GlobalEvents.set_modal_dialog (this);
            
            mode = _mode;
            
            
            heading = new Label (null);
            heading.halign = Align.START;
            
            message_bar = new MessageBar();
            
            
            cancel_but = (Button) add_button (Text.cancel_label, ResponseType.CANCEL);
            
            if (mode == "Join")
            {
                ok_but = (Button) add_button (Text.join_label, ResponseType.OK);
            }
            if (mode == "Create")
            {
                ok_but = (Button) add_button (Text.create_label, ResponseType.OK);
            }
            ok_but.can_default = true;
            
            
            id_entry = new Entry();
            id_entry.changed.connect (check_id_length);
            id_entry.changed.connect (message_bar.hide_message);
            id_entry.activates_default = true;
            id_entry.width_chars = 30;
            id_entry.max_length = 64;
            id_label = new Label.with_mnemonic (Utils.remove_colons (Text.network_id_label) + " ");
            id_label.halign = Align.END;
            id_label.mnemonic_widget = id_entry;

            password_entry = new Entry();
            password_entry.changed.connect (message_bar.hide_message);
            password_entry.activates_default = true;
            password_entry.visibility = false;
            password_entry.width_chars = 30;
            password_label = new Label.with_mnemonic (Utils.remove_colons (Text.password_label) + " ");
            password_label.halign = Align.END;
            password_label.mnemonic_widget = password_entry;
            
            Grid grid = new Grid();
            grid.row_spacing = 6;
            grid.column_spacing = 6;
            grid.attach (id_label,       0, 1, 1, 1);
            grid.attach (id_entry,       1, 1, 1, 1);
            grid.attach (password_label, 0, 2, 1, 1);
            grid.attach (password_entry, 1, 2, 1, 1);
            grid.margin = 12;
            
            
            Box container = new Box (Orientation.VERTICAL, 0);
            container.pack_start (message_bar, true, true, 0);
            container.pack_start (grid, true, true, 0);
            container.show_all();
            
            
            get_content_area().border_width = 0;
            get_content_area().add (container);
            
            get_action_area().margin = 6;
            get_action_area().margin_top = 0;
            
            set_mode ("Normal");
            id_entry.grab_focus();
            
            show();
            
            response.connect ((response_id) =>
            {
                if (response_id == ResponseType.OK)
                {
                    set_mode ("Working");
                    
                    if (mode == "Join")
                    {
                        new Thread<void*> (null, go_join_thread);
                    }
                    if (mode == "Create")
                    {
                        new Thread<void*> (null, go_create_thread);
                    }
                }
                else
                {
                    dismiss();
                }
            });
        }
        
        private void dismiss ()
        {
            GlobalEvents.set_modal_dialog (null);
            destroy();
        }
        
        private void* go_join_thread ()
        {
            network_id       = id_entry.get_chars (0, -1);
            network_password = password_entry.get_chars (0, -1);
            
            string output = "";
            
            if (Haguichi.demo_mode)
            {
                output = ".. ok, request sent";
            }
            else
            {
                output = Hamachi.join_network (network_id, network_password);
            }
            
            if (output.contains (".. ok, request sent"))
            {
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    dismiss();
                    HaguichiWindow.message_bar.set_message_with_timeout (Text.request_sent_message, null, MessageType.INFO, 3000);
                    return false;
                });
            }
            else if (output.contains (".. ok"))
            {
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    dismiss();
                    return false;
                });
                
                Thread.usleep (1000000); // Wait a second to get an updated list
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    Controller.update_connection(); // Update list
                    return false;
                });
                
                Thread.usleep (2000000);
                Hamachi.set_nick ((string) Settings.nickname.val); // Set nick to make sure any clients in this network will see it
            }
            else if (output.contains (".. failed, network not found"))
            {
                show_message (output, Text.error_network_not_found);
            }
            else if (output.contains (".. failed, invalid password"))
            {
                show_message (output, Text.error_invalid_password);
            }
            else if (output.contains (".. failed, the network is full"))
            {
                show_message (output, Text.error_network_full);
            }
            else if (output.contains (".. failed, network is locked"))
            {
                show_message (output, Text.error_network_locked);
            }
            else if (output.contains (".. failed, you are already a member") ||
                     output.contains (".. failed, you are an owner"))
            {
                show_message (output, Text.error_network_already_joined);
            }
            else
            {
                show_message (output, Text.error_unknown);
            }
            
            return null;
        }
        
        private void* go_create_thread ()
        {
            network_id       = id_entry.get_chars (0, -1);
            network_password = password_entry.get_chars (0, -1);
            
            if (Haguichi.demo_mode)
            {
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    dismiss();
                    
                    Network new_network = new Network (new Status ("*"), Hamachi.random_network_id(), network_id, "This computer", 5);
                    
                    Haguichi.connection.add_network (new_network);
                    Haguichi.window.network_view.add_network (new_network);
                    Controller.update_connection(); // Update list
                    return false;
                });
                return null;
            }
            
            string output = Hamachi.create_network (network_id, network_password);
            
            if (output.contains (".. ok"))
            {
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    dismiss();
                    return false;
                });
                
                Thread.usleep (1000000); // Wait a second to get an updated list
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    Controller.update_connection(); // Update list
                    return false;
                });
                
                Thread.usleep (2000000);
                Hamachi.set_nick ((string) Settings.nickname.val); // Set nick to make sure any clients in this network will see it
            }
            else if (output.contains (".. failed, network name is already taken"))
            {
                show_message (output, Text.error_network_id_taken);
            }
            else
            {
                show_message (output, Text.error_unknown);
            }
            
            return null;
        }
        
        private void show_message (string output, string message)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                set_mode ("Normal");
                
                if (output.contains ("password"))
                {
                    password_entry.grab_focus();
                }
                else
                {
                    id_entry.grab_focus();
                }
                
                message_bar.set_message (message, null, MessageType.ERROR);
                return false;
            });
        }
        
        private void check_id_length ()
        {
            string id = id_entry.get_chars (0, -1);
            
            if (id.length >= 4)
            {
                ok_but.sensitive = true;
                ok_but.get_style_context().add_class ("suggested-action");
                ok_but.grab_default();
            }
            else
            {
                ok_but.sensitive = false;
                ok_but.get_style_context().remove_class ("suggested-action");
            }
        }
        
        private void set_mode (string _mode)
        {
            switch (_mode)
            {
                case "Working":
                    cancel_but.sensitive     = false;
                    ok_but.sensitive         = false;
                    
                    id_entry.sensitive       = false;
                    password_entry.sensitive = false;
                    break;
                    
                case "Normal":
                    cancel_but.sensitive     = true;
                    ok_but.sensitive         = false;
                    
                    id_entry.sensitive       = true;
                    password_entry.sensitive = true;
                    
                    check_id_length();
                    break;
            }
        }
    }
}
