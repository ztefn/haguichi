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
    public class ChangeNick : Dialog
    {
        private Label nick_label;
        private Entry nick_entry;
        private Box   nick_box;
        
        private Button cancel_but;
        private Button change_but;
        
        public ChangeNick ()
        {
            Object (title: Text.change_nick_title,
                    transient_for: Haguichi.window,
                    modal: true,
                    use_header_bar: (int) Haguichi.dialog_use_header_bar);
            
            GlobalEvents.set_modal_dialog (this);
            
            
            cancel_but = (Button) add_button (Text.cancel_label, ResponseType.CANCEL);
            
            change_but = (Button) add_button (Text.change_label, ResponseType.OK);
            change_but.can_default = true;
            change_but.get_style_context().add_class ("suggested-action");
            
            nick_entry = new Entry();
            nick_entry.text = Haguichi.demo_mode ? "Joe Demo" : (string) Settings.nickname.val;
            nick_entry.activates_default = true;
            nick_entry.max_length = 64;
            nick_entry.width_chars = 25;
            
            nick_label = new Label.with_mnemonic (Utils.remove_colons (Text.nick_label) + "  ");
            nick_label.halign = Align.END;
            nick_label.mnemonic_widget = nick_entry;
            
            nick_box = new Box (Orientation.HORIZONTAL, 0);
            nick_box.pack_start (nick_label, false, false, 0);
            nick_box.pack_start (nick_entry, true, true, 0);
            nick_box.margin = 12;
            
            
            get_content_area().border_width = 0;
            get_content_area().add (nick_box);
            
            get_action_area().margin = 6;
            get_action_area().margin_top = 0;
            
            nick_entry.grab_focus();
            change_but.grab_default();
            
            show_all();
            resizable = false;
            response.connect ((response_id) =>
            {
                if (response_id == ResponseType.OK)
                {
                    string nick = nick_entry.get_chars (0, -1);
                    
                    if (Haguichi.demo_mode)
                    {
                        GlobalEvents.set_nick (nick);
                    }
                    else
                    {
                        Settings.nickname.val = nick;
                        GlobalEvents.set_nick ((string) Settings.nickname.val);
                    }
                    
                    new Thread<void*> (null, set_nick_thread);
                }
                
                GlobalEvents.set_modal_dialog (null);
                destroy();
            });
        }
        
        private void* set_nick_thread ()
        {
            Hamachi.set_nick ((string) Settings.nickname.val);
            return null;
        }
    }
}
