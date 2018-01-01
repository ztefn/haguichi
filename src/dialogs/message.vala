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
    public class Message : Dialogs.Base
    {
        public Message (Window parent, string header_text, string message_text, MessageType _message_type, string? output)
        {
            base (parent, header_text, message_text, _message_type);
            
            Button ok_but = (Button) add_button (Text.ok_label, ResponseType.OK);
            ok_but.grab_default();
            
            if (output != null)
            {
                TextView textview = new TextView();
                textview.editable = false;
                textview.left_margin = 3;
                textview.right_margin = 3;
                textview.pixels_above_lines = 3;
                textview.pixels_below_lines = 3;
                
                TextBuffer buffer = textview.buffer;
                TextIter iter;
                buffer.get_iter_at_offset (out iter, 0);
                buffer.insert (ref iter, output.strip(), -1);
                
                ScrolledWindow sw = new ScrolledWindow (null, null);
                sw.hscrollbar_policy = PolicyType.NEVER;
                sw.vscrollbar_policy = PolicyType.AUTOMATIC;
                sw.min_content_height = 60;
                sw.shadow_type = ShadowType.IN;
                sw.add (textview);
                
                Expander expander = new Expander.with_mnemonic (Text.hamachi_output);
                expander.add (sw);
                expander.show_all();
                
                add_content (expander);
            }
            
            run();
        }
    }
}
