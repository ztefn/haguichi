/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2019 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

namespace Widgets
{
    public class PreferencesBox : Box
    {
        private Grid grid;
        
        public PreferencesBox (string text)
        {
            orientation  = Orientation.VERTICAL;
            border_width = 12;
            
            var label = new Label (text);
            label.get_style_context().add_class ("h4");
            label.halign = Align.START;
            
            add (label);
            
            grid = new Grid();
            grid.column_spacing  = 9;
            grid.row_homogeneous = true;
            grid.margin_start    = 12;
            
            add (grid);
        }
        
        public void add_row (Widget label, Widget widget, int position)
        {
            widget.valign  = Align.CENTER;
            widget.halign  = Align.END;
            widget.hexpand = true;
            
            grid.attach (label,  0, position, 1, 1);
            grid.attach (widget, 1, position, 1, 1);
        }
        
        public void remove_row (int position)
        {
            grid.remove_row (position);
        }
        
        public uint num_rows
        {
            get
            {
                return grid.get_children().length();
            }
        }
    }
}
