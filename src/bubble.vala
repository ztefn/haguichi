/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Notify;

public class Bubble : Object
{
    private Notify.Notification notification;
    
    private string   client_id;
    private string[] network_ids;
    
    public Bubble (string summary, string body)
    {
        notification = new Notify.Notification (summary, body, "haguichi");
    }
    
    public void show ()
    {
        try
        {
            notification.show();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "Bubble.show", e.message);
        }
    }
    
    public void close ()
    {
        try
        {
            notification.close();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "Bubble.close", e.message);
        }
    }
    
    public void add_reconnect_action ()
    {
        // Check if notification server is capable of showing actions
        if (Notify.get_server_caps().find_custom ("actions", strcmp) != null)
        {
            notification.add_action ("reconnect", Text.reconnect_label, reconnect_action_callback);
        }
    }
    
    public void add_approve_reject_actions (string _client_id, owned string[] _network_ids)
    {
        client_id   = _client_id;
        network_ids = _network_ids;
        
        // Check if notification server is capable of showing actions
        if (Notify.get_server_caps().find_custom ("actions", strcmp) != null)
        {
            notification.add_action ("approve", Utils.remove_mnemonics (Text.approve_label), approve_reject_action_callback);
            notification.add_action ("reject",  Utils.remove_mnemonics (Text.reject_label),  approve_reject_action_callback);
        }
    }
    
    public void reconnect_action_callback (Notify.Notification notification, string action)
    {
        // Connect only if this action is still enabled
        if (GlobalActions.connect.get_enabled())
        {
            GlobalActions.connect.activate (null);
        }
        // Close the notification as the action may not be executed without this call
        close();
    }
    
    public void approve_reject_action_callback (Notify.Notification notification, string action)
    {
        // Approve or reject member only if we are still connected
        if (Controller.last_status >= 6)
        {
            foreach (string network_id in network_ids)
            {
                Network network = Haguichi.window.network_view.return_network_by_id (network_id);
                
                foreach (Member member in network.members)
                {
                    if ((member.client_id == client_id) &&
                        (member.status.status_int == 3))
                    {
                        if (action == "approve")
                        {
                            member.approve();
                        }
                        else if (action == "reject")
                        {
                            member.reject();
                        }
                    }
                }
            }
        }
        // Close the notification as the action may not be executed without this call
        close();
    }
}
