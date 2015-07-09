/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2015 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

public class SidebarEntry : Label
{
    public SidebarEntry ()
    {
        xalign = 0;
        width_chars = 15;
        ellipsize = Pango.EllipsizeMode.END;
        selectable = true;
    }
}
