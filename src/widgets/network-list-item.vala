/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2026 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

namespace Haguichi {
    public class NetworkListItem : Object {
        public string   label             { get; set; }
        public string   accessible_label  { get; set; }
        public string[] node_css_classes  { get; set; }
        public string[] label_css_classes { get; set; }
    }
}
