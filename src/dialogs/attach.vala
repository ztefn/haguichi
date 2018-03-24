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
using Widgets;

namespace Dialogs
{
    public class Attach : Dialog
    {
        private MessageBar message_bar;
        
        private Label account_label;
        private Entry account_entry;
        private Box   account_box;
        
        private Button cancel_but;
        private Button attach_but;
        private CheckButton with_networks;
        
        public Attach ()
        {
            Object (title: Text.attach_title,
                    transient_for: Haguichi.window,
                    modal: true,
                    resizable: false,
                    use_header_bar: (int) Haguichi.dialog_use_header_bar);
            
            GlobalEvents.set_modal_dialog (this);
            
            
            message_bar = new MessageBar();
            
            cancel_but = (Button) add_button (Text.cancel_label, ResponseType.CANCEL);
            
            attach_but = (Button) add_button (Text.attach_button_label, ResponseType.OK);
            attach_but.can_default = true;
            attach_but.sensitive = false;
            
            account_entry = new Entry();
            account_entry.activates_default = true;
            account_entry.width_chars = 30;
            account_entry.changed.connect (check_entry_length);
            account_entry.changed.connect (message_bar.hide_message);
            
            account_label = new Label.with_mnemonic (Utils.remove_colons (Text.account_label) + "  ");
            account_label.halign = Align.END;
            account_label.mnemonic_widget = account_entry;

            
            account_box = new Box (Orientation.HORIZONTAL, 0);
            account_box.pack_start (account_label, false, false, 0);
            account_box.pack_start (account_entry, true, true, 0);
            
            with_networks = new CheckButton.with_mnemonic (Text.attach_with_networks_checkbox);
            with_networks.active = true;
            
            
            Box vbox = new Box (Orientation.VERTICAL, 0);
            vbox.pack_start (account_box, false, false, 0);
            vbox.pack_start (with_networks, false, false, 6);
            vbox.margin = 12;
            vbox.margin_bottom = 6;
            
            
            Box container = new Box (Orientation.VERTICAL, 0);
            container.pack_start (message_bar, true, true, 0);
            container.pack_start (vbox, true, true, 0);
            container.show_all();
            
            
            get_content_area().border_width = 0;
            get_content_area().add (container);
            
            get_action_area().margin = 6;
            get_action_area().margin_top = 0;
            
            account_entry.grab_focus();
            
            show();
            
            response.connect ((response_id) =>
            {
                if (response_id == ResponseType.OK)
                {
                    set_mode ("Attaching");
                    new Thread<void*> (null, go_attach_thread);
                }
                else
                {
                    dismiss();
                }
            });
        }
        
        private void* go_attach_thread ()
        {
            string account = account_entry.get_chars (0, -1);
            string output;
            
            GlobalEvents.attach_blocking = true;
            
            if (Haguichi.demo_mode)
            {
                if (account.contains ("@"))
                {
                    output = ".. ok";
                }
                else
                {
                    output = ".. failed, not found";
                }
            }
            else
            {
                output = Hamachi.attach (account, with_networks.active);
            }
            
            if (output.index_of (".. ok") != -1)
            {
                Idle.add_full (Priority.HIGH_IDLE, () =>
                {
                    GlobalEvents.set_attach_with_account (account + " (pending)");
                    dismiss();
                    return false;
                });
                
                Thread.usleep (2000000); // Wait two seconds to get updated info
                Hamachi.get_info();
            }
            else if ((output.index_of (".. failed, not found") != -1) ||
                     (output.index_of (".. failed, [248]") != -1))
            {
                show_message (Text.attach_error_account_not_found);
            }
            else if (output.index_of (".. failed") != -1)
            {
                show_message (Text.error_unknown);
            }
            else
            {
                // Unknown output
            }
            
            GlobalEvents.attach_blocking = false;
            
            return null;
        }
        
        private void check_entry_length ()
        {
            string account = account_entry.get_chars (0, -1);
            
            if (account.length > 0)
            {
                attach_but.sensitive = true;
                attach_but.get_style_context().add_class ("suggested-action");
                attach_but.grab_default();
            }
            else
            {
                attach_but.sensitive = false;
                attach_but.get_style_context().remove_class ("suggested-action");
            }
        }
        
        private void show_message (string message)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                set_mode ("Normal");
                
                account_entry.grab_focus();
                
                message_bar.set_message (message, null, MessageType.ERROR);
                return false;
            });
        }
        
        private void dismiss ()
        {
            GlobalEvents.set_modal_dialog (null);
            destroy();
        }
        
        private void set_mode (string mode)
        {
            switch (mode)
            {
                case "Attaching":
                    account_entry.sensitive = false;
                    with_networks.sensitive = false;
                    cancel_but.sensitive    = false;
                    attach_but.sensitive    = false;
                    break;
                    
                case "Normal":
                    account_entry.sensitive = true;
                    with_networks.sensitive = true;
                    cancel_but.sensitive    = true;
                    attach_but.sensitive    = true;
                    break;
            }
        }
    }
}
