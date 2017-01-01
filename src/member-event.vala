/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class MemberEvent
{
    public string nick;
    private string[] networks;
    
    public MemberEvent (string _nick)
    {
        nick = _nick;
        networks = {};
    }
    
    public void add_network (string network)
    {
        networks += network;
    }
    
    public string first_network
    {
        get
        {
            return networks[0];
        }
    }
    
    public int networks_length
    {
        get
        {
            return networks.length;
        }
    }
}
