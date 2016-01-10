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

namespace Widgets
{
    public class MessageBar : InfoBar
    {
        private uint timer_id;
        
        private Label label;
        private ButtonBox button_box;
        
        public MessageBar ()
        {
            label = new Label (null);
            label.wrap = true;
            label.margin_start = 3;
            label.halign = Align.START;
            
            button_box = new ButtonBox (Orientation.HORIZONTAL);
            button_box.margin = 6;
            button_box.margin_end = 9;
            button_box.spacing = 6;
            
            get_content_area().add (label);
            get_action_area().destroy();
            add (button_box);
            
            show_all();
        }
        
        public void set_message (string? header, string? message, MessageType _message_type)
        {
            foreach (var child in button_box.get_children())
            {
                button_box.remove (child);
            }
            
            string markup = "";
            
            if (header != null)
            {
                markup += "<b>{0}</b>";
            }
            if (message != null)
            {
                markup += "\n<span size=\"smaller\">{1}</span>";
            }
            
            label.set_markup (Utils.format (markup, header, message, null));
            
            set_message_type (_message_type);
            
            button_box.hide();
            
            show();
        }
        
        public void set_message_with_timeout (string? header, string? message, MessageType _message_type, int timeout)
        {
            set_message (header, message, _message_type);
            set_timeout (timeout);
        }
        
        public void set_timeout (int timeout)
        {
            if (timer_id > 0)
            {
                GLib.Source.remove (timer_id);
                timer_id = 0;
            }
            
            timer_id = GLib.Timeout.add ((uint) timeout, () =>
            {
                hide();
                timer_id = 0;
                
                return false;
            });
        }
        
        public new void add_button (Button button)
        {
            button_box.add (button);
            button.show();
            button_box.show();
        }
    }
}
