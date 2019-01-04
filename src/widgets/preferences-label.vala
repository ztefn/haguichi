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
    public class PreferencesLabel : Label
    {
        public PreferencesLabel (string? text, Widget widget)
        {
            halign          = Align.START;
            label           = Utils.remove_colons (text);
            mnemonic_widget = widget;
            use_underline   = true;
        }
    }
}
