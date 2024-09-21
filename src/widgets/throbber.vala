/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2024 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

#if ADW_1_6
using Adw;
#else
using Gtk;
#endif

namespace Haguichi {
    public class Throbber : Adw.Bin {
        public Spinner spinner;

        construct {
            spinner = new Spinner () {
                width_request = 20,
                height_request = 20
            };
            set_child (spinner);
        }

        public bool spinning {
#if ADW_1_6
            get; set;
#else
            get {
                return spinner.spinning;
            }
            set {
                spinner.spinning = value;
            }
#endif
        }
    }
}
