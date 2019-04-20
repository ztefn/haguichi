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

class CellRendererNode : CellRendererPixbuf
{
    public override void render (Cairo.Context cr, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
    {
        Gdk.Rectangle aligned_area = get_aligned_area (widget, flags, cell_area);

        int x      = aligned_area.x + 4;
        int y      = aligned_area.y + 4;
        int width  = aligned_area.width  - 7;
        int height = aligned_area.height - 7;

        var context = widget.get_style_context();
        context.add_class ("network-node");
        context.add_class (icon_name);
        context.render_background (cr, x, y, width, height);
    }
}
