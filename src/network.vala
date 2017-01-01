/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Network : Object
{
    public Status status;
    public string id;
    public string name;
    public List<Member> members;
    public int is_owner;
    public string owner;
    public string lock_state;
    public string approve;
    public int capacity;
    
    public string name_sort_string;
    public string status_sort_string;
    
    private bool updating;
    
    public Network.empty ()
    {
        // Just for creating unassigned instances
    }
    
    public Network (Status _status, string _id, string? _name, string? _owner, int? _capacity)
    {
        status     = _status;
        id         = _id;
        name       = _name;
        members    = new List<Member>();
        is_owner   = -1;
        owner      = _owner;
        lock_state = "";
        approve    = "";
        capacity   = _capacity;
        
        set_sort_strings();
    }
    
    public void init ()
    {
        determine_ownership();
    }
    
    private void set_sort_strings ()
    {
        name_sort_string   = name;
        status_sort_string = status.status_sortable + name;
    }
    
    public void update (Status _status, string _id, string? _name)
    {
        if (!updating) // Check this flag to prevent the background update process from overriding a very recent change
        {
            status = _status;
            id     = _id;
            name   = _name;
            
            set_sort_strings();
        }
    }
    
    public void return_member_count (out int total_count, out int online_count)
    {
        total_count  = 0;
        online_count = 0;

        foreach (Member member in members)
        {
            total_count ++;
                
            if ((member.status.status_int > 0) &&
                (member.status.status_int < 3))
            {
                online_count ++;
            }
        }
    }
    
    public string return_owner_string ()
    {
        string _owner;
        
        if (is_owner == 1)
        {
            _owner = Text.you;
        }
        else if (owner != null)
        {
            _owner = owner;
            
            foreach (Member member in members)
            {
                if (member.client_id == _owner)
                {
                    _owner = member.nick;
                }
            }
        }
        else
        {
            _owner = Text.unknown;
        }
        
        return _owner;
    }
    
    public void determine_ownership ()
    {
        new Thread<void*> (null, determine_ownership_thread);
    }
    
    private void* determine_ownership_thread ()
    {
        if (owner == "This computer")
        {
            is_owner = 1;
            
            string output;
            
            if (Haguichi.demo_mode)
            {
                output = @"
id       : " + id + @"
name     : " + name + @"
type     : Mesh
owner    : This computer
status   : unlocked
approve  : manual";
            }
            else
            {
                output = Command.return_output ("hamachi network \"" + Utils.clean_string (id) + "\"");
            }
            Debug.log (Debug.domain.HAMACHI, "Network.determine_ownership_thread", output);
            
            lock_state = Hamachi.retrieve (output, "status");
            approve = Hamachi.retrieve (output, "approve");
        }
        else
        {
            is_owner = 0;
            
            try
            {
                MatchInfo mi;
                new Regex ("""^.*? \((?<id>[0-9-]{11})\)$""").match (owner, 0, out mi);
                
                string id = mi.fetch_named ("id");
                
                if (id != null)
                {
                    owner = id;
                }
            }
            catch (RegexError e)
            {
                Debug.log (Debug.domain.ERROR, "Network.determine_ownership_thread", e.message);
            }
        }
        Debug.log (Debug.domain.HAMACHI, "Network.determine_ownership_thread", "Owner for network " + id + ": " + owner);
        
        Idle.add_full (Priority.DEFAULT_IDLE, () =>
        {
            Haguichi.window.network_view.update_network (this);
            
            foreach (Member member in members)
            {
                if (member.client_id == owner)
                {
                    Haguichi.window.network_view.update_member_with_network (this, member);
                }
            }
            return false;
        });
        
        return null;
    }
    
    public void add_member (Member member)
    {
        members.append (member);
    }
    
    public void remove_member (Member member)
    {
        members.remove (member);
    }
    
    public void go_online ()
    {
        updating = true;
        
        new Thread<void*> (null, go_online_thread);
        
        status = new Status ("*");
        
        Haguichi.window.network_view.update_network (this);
        HaguichiWindow.sidebar.refresh_tab();
    }
    
    private void* go_online_thread ()
    {
        bool success = Hamachi.go_online (this);
        
        if (!success)
        {
            status = new Status (" ");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                Haguichi.window.network_view.update_network (this);
                HaguichiWindow.sidebar.refresh_tab();
                return false;
            });
        }
        
        Thread.usleep (1000000); // Wait a second to get an updated list
        
        Idle.add_full (Priority.HIGH_IDLE, () =>
        {
            Controller.update_connection(); // Update list
            return false;
        });
        
        updating = false;
        return null;
    }
    
    public void go_offline ()
    {
        updating = true;
        
        new Thread<void*> (null, go_offline_thread);
        
        status = new Status (" ");
        
        Haguichi.window.network_view.update_network (this);
        HaguichiWindow.sidebar.refresh_tab();
    }
    
    private void* go_offline_thread ()
    {
        bool success = Hamachi.go_offline (this);
        
        if (!success)
        {
            status = new Status ("*");
            
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                Haguichi.window.network_view.update_network (this);
                HaguichiWindow.sidebar.refresh_tab();
                return false;
            });
        }
        
        Thread.usleep (1000000); // Wait a second to get an updated list
        
        Idle.add_full (Priority.HIGH_IDLE, () =>
        {
            Controller.update_connection(); // Update list
            return false;
        });
        
        updating = false;
        return null;
    }
    
    public void change_password ()
    {
        new Dialogs.ChangePassword (this);
    }
    
    public void set_lock (string locked)
    {
        updating = true;
        
        lock_state = locked;
        
        HaguichiWindow.sidebar.refresh_tab();
        
        new Thread<void*> (null, set_lock_thread);
    }
    
    private void* set_lock_thread ()
    {
        string locked = "unlock";
        
        if (lock_state == "locked")
        {
            locked = "lock";
        }
        
        Hamachi.set_access (id, locked, approve);
        
        updating = false;
        return null;
    }
    
    public void set_approval (string approval)
    {
        updating = true;
        
        approve = approval;
        
        HaguichiWindow.sidebar.refresh_tab();
        
        new Thread<void*> (null, set_approval_thread);
    }
    
    private void* set_approval_thread ()
    {
        string locked = "unlock";
        
        if (lock_state == "locked")
        {
            locked = "lock";
        }
        
        Hamachi.set_access (id, locked, approve);
        
        updating = false;
        return null;
    }
    
    public void copy_id_to_clipboard ()
    {
        Gtk.Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (id, -1);
    }
    
    public void leave ()
    {
        string heading = Utils.format (Text.confirm_leave_network_heading, name, null, null);
        string message = Utils.format (Text.confirm_leave_network_message, name, null, null);
        
        Dialogs.Confirm dlg = new Dialogs.Confirm (Haguichi.window, heading, message, Gtk.MessageType.QUESTION, Text.leave_label);
        
        if (dlg.response_id == Gtk.ResponseType.OK)
        {
            new Thread<void*> (null, leave_thread);
        }
        dlg.destroy();
    }
    
    private void* leave_thread ()
    {
        bool success = Hamachi.leave (this);
        
        if (success)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                Haguichi.connection.remove_network (this);
                Haguichi.window.network_view.remove_network (this);
                Haguichi.window.update();
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
    
    public void delete ()
    {
        string heading = Utils.format (Text.confirm_delete_network_heading, name, null, null);
        string message = Utils.format (Text.confirm_delete_network_message, name, null, null);
        
        Dialogs.Confirm dlg = new Dialogs.Confirm (Haguichi.window, heading, message, Gtk.MessageType.WARNING, Text.delete_label);
        
        if (dlg.response_id == Gtk.ResponseType.OK)
        {
            new Thread<void*> (null, delete_thread);
        }
        dlg.destroy();
    }
    
    private void* delete_thread ()
    {
        bool success = Hamachi.delete (this);
        
        if (success)
        {
            Idle.add_full (Priority.HIGH_IDLE, () =>
            {
                Haguichi.connection.remove_network (this);
                Haguichi.window.network_view.remove_network (this);
                Haguichi.window.update();
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
