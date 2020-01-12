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

public class SidebarButton : Button
{
    public SidebarButton (string _label)
    {
        label = _label;
        use_underline = true;
        Utils.get_label_widget(this).ellipsize = Pango.EllipsizeMode.MIDDLE;
    }
}
