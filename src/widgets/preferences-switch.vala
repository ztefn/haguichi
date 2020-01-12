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

namespace Widgets
{
    public class PreferencesSwitch : Switch
    {
        public PreferencesSwitch (Key key)
        {
            active = (bool) key.val;
            margin = 3;
            
            state_set.connect ((state) =>
            {
                key.val = state;
                return false;
            });
        }
    }
}
