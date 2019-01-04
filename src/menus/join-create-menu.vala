/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2019 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

namespace Menus
{
    public class JoinCreateMenu : Gtk.Menu
    {
        private Gtk.MenuItem join;
        private Gtk.MenuItem create;
        
        
        public JoinCreateMenu ()
        {
            join = new Gtk.MenuItem.with_mnemonic (Text.join_network_label);
            join.activate.connect (GlobalEvents.join_network);
            
            create = new Gtk.MenuItem.with_mnemonic (Text.create_network_label);
            create.activate.connect (GlobalEvents.create_network);
            
            add (join);
            add (create);
            
            show_all();
        }
    }
}
