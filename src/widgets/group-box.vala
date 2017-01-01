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

namespace Widgets
{
    public class GroupBox : Box
    {
        private Box grp_box;
        private Label grp_label;
        
        public GroupBox (string label)
        {
            orientation = Orientation.VERTICAL;
            border_width = 6;
            
            if (label != "")
            {
                grp_label = new Label(null);
                grp_label.halign = Align.START;
                grp_label.margin = 6;
                grp_label.set_markup (Utils.format ("<b>{0}</b>", label, null, null));
                
                add (grp_label);
            }
            
            Box empty_box = new Box (Orientation.VERTICAL, 0);
            
            grp_box = new Box (Orientation.VERTICAL, 0);
            grp_box.spacing = 6;
            
            Box hbox = new Box (Orientation.HORIZONTAL, 0);
            hbox.pack_start (empty_box, false, false, 6);
            hbox.pack_start (grp_box, true, true, 6);
            
            add (hbox);
        }
        
        public void add_widget (Widget widget)
        {
            grp_box.pack_start (widget, false, false, 0);
        }
    }
}
