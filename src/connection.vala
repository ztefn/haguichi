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
    private HashTable<string, string> long_nicks_hash;
    
    public Connection ()
    {
        networks = new List<Network>();
        long_nicks_hash = new HashTable<string, string>(str_hash, str_equal);
        
        // Retreive saved long nicks from GSettings
        string[] long_nicks = (string[]) Settings.long_nicks.val;
        
        // Add saved long nicks to the hash table
        foreach (string long_nick in long_nicks)
        {
            string[] parts = long_nick.split (";", 2);
            
            if (parts.length == 2)
            {
                add_long_nick (parts[0], parts[1]);
            }
        }
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
    
    public bool has_long_nick (string client_id)
    {
        return long_nicks_hash.contains (client_id);
    }
    
    public string get_long_nick (string client_id)
    {
        return long_nicks_hash.lookup (client_id);
    }
    
    public void add_long_nick (string client_id, string long_nick)
    {
        // Lock hash table while inserting because this function can be called from asynchronous Member.get_long_nick threads
        lock (long_nicks_hash)
        {
            long_nicks_hash.insert (client_id, long_nick);
        }
    }
    
    public void save_long_nicks ()
    {
        string[] long_nicks = {};
        
        long_nicks_hash.foreach ((client_id, long_nick) =>
        {
            long_nicks += client_id + ";" + long_nick;
        });
        
        Settings.long_nicks.val = long_nicks;
    }
}
