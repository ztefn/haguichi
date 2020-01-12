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
    public class Base : Granite.MessageDialog
#else
    public class Base : MessageDialog
#endif
    {
        public int response_id;
        
        public Base (Window parent, string header_text, string message_text, MessageType _message_type)
        {
#if FOR_ELEMENTARY
            base.with_image_from_icon_name (header_text, message_text, Utils.get_message_type_icon_name (_message_type), ButtonsType.NONE);
#else
            text           = header_text;
            secondary_text = message_text;
            message_type   = _message_type;
#endif
            transient_for  = parent;
            
            if (transient_for == Haguichi.window)
            {
                GlobalEvents.set_modal_dialog (this);
            }
            
            response.connect (on_response);
        }
        
        public void add_content (Widget widget)
        {
#if FOR_ELEMENTARY
            custom_bin.add (widget);
#else
            ((Box) message_area).pack_end (widget, false, false, 0);   
#endif
            widget.show();
        }
        
        public void on_response (Dialog dialog, int _response_id)
        {
            response_id = _response_id;
            
            if (transient_for == Haguichi.window)
            {
                GlobalEvents.set_modal_dialog (null);
            }
        }
    }
}
