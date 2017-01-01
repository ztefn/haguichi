/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

namespace Dialogs
{
    public class Base : MessageDialog
    {
        public int response_id;
        public Box contents;
        
        public Base (Window parent, string header_text, string message_text, MessageType _message_type)
        {
            GlobalEvents.set_modal_dialog (this);
            
            text           = header_text;
            secondary_text = message_text;
            message_type   = _message_type;
            transient_for  = parent;
            
            response.connect (on_response);
        }
        
        public void add_content (Widget widget)
        {
            ((Box) message_area).pack_end (widget, false, false, 0);   
            widget.show();
        }
        
        public void on_response (Dialog dialog, int _response_id)
        {
            response_id = _response_id;
            GlobalEvents.set_modal_dialog (null);
        }
    }
}
