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

namespace Dialogs
{
    public class Confirm : Dialogs.Base
    {
        public Confirm (Window parent, string heading_text, string message_text, MessageType _message_type, string button_label)
        {
            base (parent, heading_text, message_text, _message_type);
            
            add_button (Text.cancel_label, ResponseType.CANCEL);
            
            Button ok_but = (Button) add_button (button_label, ResponseType.OK);
            ok_but.get_style_context().add_class ((_message_type == MessageType.WARNING) ? "destructive-action" : "suggested-action");
            ok_but.grab_default();
            
            run();
        }
    }
}
