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

public class SidebarLabel : Label
{
    public SidebarLabel (string _label)
    {
        label = Utils.remove_colons (_label);
        // Fix error "undefined symbol: gtk_label_set_xalign" when build with valac >= 0.28 (which asumes Gtk+ 3.16) and running Gtk+ 3.14
        // xalign = 1;
        ((Gtk.Misc) this).xalign = 1.0f;
        width_chars = 11;
        ellipsize = Pango.EllipsizeMode.START;
        get_style_context().add_class ("dim-label");
    }
}
