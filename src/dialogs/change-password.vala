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

namespace Dialogs
{
    public class ChangePassword : Dialog
    {
        private string password;
        
        private Network network;
        
        private Label password_label;
        private Entry password_entry;
        private Box   password_box;
        
        private Button cancel_but;
        private Button change_but;
        
        public ChangePassword (Network _network)
        {
            Object (title: Text.change_password_title,
                    transient_for: Haguichi.window,
                    modal: true,
                    use_header_bar: (int) Haguichi.dialog_use_header_bar);
            
            GlobalEvents.set_modal_dialog (this);
            
            network = _network;
            
            
            cancel_but = (Button) add_button (Text.cancel_label, ResponseType.CANCEL);
            
            change_but = (Button) add_button (Text.change_label, ResponseType.OK);
            change_but.can_default = true;
            change_but.get_style_context().add_class ("suggested-action");
            
            password_entry = new Entry();
            password_entry.activates_default = true;
            password_entry.visibility = false;
            password_entry.width_chars = 25;
            
            password_label = new Label.with_mnemonic (Utils.remove_colons (Text.new_password_label) + "  ");
            password_label.halign = Align.END;
            password_label.mnemonic_widget = password_entry;
            
            password_box = new Box (Orientation.HORIZONTAL, 0);
            password_box.pack_start (password_label, false, false, 0);
            password_box.pack_start (password_entry, true, true, 0);
            password_box.margin = 12;
            
            
            get_content_area().border_width = 0;
            get_content_area().add (password_box);
            
            get_action_area().margin = 6;
            get_action_area().margin_top = 0;
            
            password_entry.grab_focus();
            change_but.grab_default();
            
            show_all();
            resizable = false;
            response.connect ((response_id) =>
            {
                if (response_id == ResponseType.OK)
                {
                    password = password_entry.get_chars (0, -1);
                    new Thread<void*> (null, set_password_thread);
                }
                
                GlobalEvents.set_modal_dialog (null);
                destroy();
            });
        }
        
        public void* set_password_thread ()
        {
            Hamachi.set_password (network.id, password);
            return null;
        }
    }
}
