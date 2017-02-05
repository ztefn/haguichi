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
    public  string   client_id;
    public  string   name;
    private string[] network_approval_ids;
    private string[] network_names;
    
    public MemberEvent (string _client_id, string _name)
    {
        client_id            = _client_id;
        name                 = _name;
        network_approval_ids = {};
        network_names        = {};
    }
    
    public void add_network (string network_name)
    {
        network_names += network_name;
    }
    
    public void add_network_approval (string network_id)
    {
        network_approval_ids += network_id;
    }
    
    public string[] get_network_approval_ids ()
    {
        return network_approval_ids;
    }
    
    public string first_network_name
    {
        get
        {
            return network_names[0];
        }
    }
    
    public int networks_length
    {
        get
        {
            return network_names.length;
        }
    }
}
