/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Connection : Object
{
    public List<Network> networks;
    
    public Connection ()
    {
        networks = new List<Network>();
    }
    
    public void add_network (Network network)
    {
        networks.append (network);
    }
    
    public void remove_network (Network network)
    {
        networks.remove (network);
    }
    
    public bool has_network (Network network)
    {
        return (networks.index (network) > -1);
    }
    
    public void clear_networks ()
    {
        networks = new List<Network>();
    }
}
