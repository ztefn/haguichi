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
    public class MessageBox : Box
    {
        private Label heading;
        private Label message;
        private ButtonBox button_box;
        
        public MessageBox ()
        {
            orientation = Orientation.VERTICAL;
            margin = 10;
            
            heading = new Label (null);
            heading.wrap = true;
            heading.halign = Align.CENTER;
            heading.set_justify (Justification.CENTER);
            
            message = new Label (null);
            message.wrap = true;
            message.get_style_context().add_class ("dim-label");
            message.halign = Align.CENTER;
            message.set_justify (Justification.CENTER);
            
            button_box = new ButtonBox (Orientation.HORIZONTAL);
            button_box.set_layout (ButtonBoxStyle.CENTER);
            button_box.spacing = 6;
            button_box.margin_top = 20;
            
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            pack_start (heading, false, false, 0);
            pack_start (message, false, false, 0);
            pack_start (button_box, false, false, 0);
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
            
            show_all();
        }
        
        public void set_message (string? _heading, string? _message)
        {
            foreach (var child in button_box.get_children())
            {
                button_box.remove (child);
            }
            
            heading.hide();
            message.hide();
            
            heading.set_markup ("<span size=\"larger\">" + _heading + "</span>");
            message.set_markup (_message);
            
            if (_heading != null)
            {
                heading.show();
            }
            if (_message != null)
            {
                message.show();
            }
            
            button_box.hide();
            
            show();
        }
        
        public void add_button (Button button)
        {
            button_box.add (button);
            button.show();
            button_box.show();
        }
    }
}
