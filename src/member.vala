/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Member : Object
{
    public Status status;
    public string network_id;
    public string ipv4;
    public string ipv6;
    public string nick;
    public string client_id;
    public string tunnel;
    
    public string name_sort_string;
    public string status_sort_string;
    
    public bool is_evicted;
    
    public Member (Status _status, string _network_id, string? _ipv4, string? _ipv6, string? _nick, string _client_id, string? _tunnel)
    {
        status     = _status;
        network_id = _network_id;
        ipv4       = _ipv4;
        ipv6       = _ipv6;
        nick       = _nick;
        client_id  = _client_id;
        tunnel     = _tunnel;
        
        set_sort_strings ();
        
        is_evicted = false;
    }
    
    public void init ()
    {
        get_long_nick (nick);
    }
    
    public void update (Status _status, string _network_id, string? _ipv4, string? _ipv6, string? _nick, string _client_id, string? _tunnel)
    {
        status     = _status;
        network_id = _network_id;
        ipv4       = _ipv4;
        ipv6       = _ipv6;
        client_id  = _client_id;
        tunnel     = _tunnel;
        
        get_long_nick (_nick);
    }
    
    private void set_sort_strings ()
    {
        name_sort_string = nick + client_id;
        status_sort_string = status.status_sortable + nick + client_id;
    }
    
    public void get_long_nick (string _nick)
    {
        if ((_nick.length >= 25) &&
            (nick.length > 25) &&
            (nick.has_prefix (_nick)))
        {
            // Long nick has already been retreived and is probably not altered, since the first 25 characters are identical
        }
        else if ((_nick.length >= 25) ||
                 (_nick.has_suffix ("�")))
        {
            // Retrieve long nick
            new Thread<void*> (null, get_long_nick_thread);
        }
        else
        {
            // Save passed nick
            nick = _nick;
            set_sort_strings ();
        }
    }
    
    private void* get_long_nick_thread ()
    {
        if (Haguichi.demo_mode)
        {
            Debug.log (Debug.domain.HAMACHI, "Member.get_long_nick_thread", "Demo mode, keeping nick " + nick);
        }
        else
        {
            string output = Command.return_output ("hamachi peer " + client_id);
            Debug.log (Debug.domain.HAMACHI, "Member.GetLongNickThread", output);
            
            nick = Hamachi.retrieve (output, "nickname");
            set_sort_strings ();
            
            Idle.add_full (Priority.DEFAULT_IDLE, () =>
            {
                Haguichi.window.network_view.update_member (this);
                return false;
            });
        }
        
        return null;
    }
    
    public void copy_ipv4_to_clipboard ()
    {
        Gtk.Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (ipv4, -1);
    }
    
    public void copy_ipv6_to_clipboard ()
    {
        Gtk.Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (ipv6, -1);
    }
    
    public void copy_client_id_to_clipboard ()
    {
        Gtk.Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (client_id, -1);
    }
    
    public void approve ()
    {
        if (Haguichi.demo_mode)
        {
            nick    = "Nick";
            ipv4    = "192.168.155.23";
            status  = new Status ("*");
        
            set_sort_strings ();
            
            Haguichi.window.network_view.update_member (this);
            HaguichiWindow.sidebar.refresh_tab();
        }
        else
        {
            new Thread<void*> (null, approve_thread);
        }
    }
    
    private void* approve_thread ()
    {
        Hamachi.approve (this);
        
        Thread.usleep (1000000); // Wait a second to get an updated list
        
        Idle.add_full (Priority.HIGH_IDLE, () =>
        {
            Controller.update_connection(); // Update list
            return false;
        });
        return null;
    }
    
    public void reject ()
    {
        if (Haguichi.demo_mode)
        {
            Haguichi.window.network_view.remove_member (Haguichi.window.network_view.return_network_by_id (network_id), this);
        }
        else
        {
            new Thread<void*> (null, reject_thread);
        }
    }
    
    private void* reject_thread ()
    {
        Hamachi.reject (this);
        
        Thread.usleep (1000000); // Wait a second to get an updated list
        
        Idle.add_full (Priority.HIGH_IDLE, () =>
        {
            Controller.update_connection(); // Update list
            return false;
        });
        return null;
    }
    
    public void evict ()
    {
        Network network = Haguichi.window.network_view.return_network_by_id (network_id);
        
        string heading = Utils.format (Text.confirm_evict_member_heading, nick, network.name, null);
        string message = Text.confirm_evict_member_message;
            
        Dialogs.Confirm dlg = new Dialogs.Confirm (Haguichi.window, heading, message, Gtk.MessageType.QUESTION, Text.evict_label);
        
        if (dlg.response_id == Gtk.ResponseType.OK)
        {
            is_evicted = true;
            new Thread<void*> (null, evict_thread);
        }
        dlg.destroy();
    }
    
    private void* evict_thread ()
    {
        bool success = Hamachi.evict (this);
        
        if (success)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                Network network = Haguichi.window.network_view.return_network_by_id (network_id);
                
                Haguichi.window.network_view.remove_member (network, this);
                network.remove_member (this);
                return false;
            });
        }
        
        Thread.usleep (1000000); // Wait a second to get an updated list
        
        Idle.add_full (Priority.HIGH_IDLE, () =>
        {
            Controller.update_connection(); // Update list
            return false;
        });
        return null;
    }
}
