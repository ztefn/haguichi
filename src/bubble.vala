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
            notification.add_action ("reconnect", Text.reconnect_label, (notification, action) =>
            {
                // Connect only if this action is still enabled
                if (GlobalActions.connect.get_enabled())
                {
                    GlobalActions.connect.activate (null);
                }
                // Close the notification as the action may not be executed without this call
                close();
            });
        }
    }
    
    public void add_approve_reject_actions (string client_id, owned string[] network_ids)
    {
        // Check if notification server is capable of showing actions
        if (Notify.get_server_caps().find_custom ("actions", strcmp) != null)
        {
            notification.add_action ("approve", Utils.remove_mnemonics (Text.approve_label), (notification, action) =>
            {
                // Approve member only if we are still connected
                if (Controller.last_status >= 6)
                {
                    foreach (string network_id in network_ids)
                    {
                        Network network = Haguichi.window.network_view.return_network_by_id(network_id);
                        
                        foreach (Member member in network.members)
                        {
                            if ((member.client_id == client_id) &&
                                (member.status.status_int == 3))
                            {
                                member.approve();
                            }
                        }
                    }
                }
                // Close the notification as the action may not be executed without this call
                close();
            });
            
            notification.add_action ("reject", Utils.remove_mnemonics (Text.reject_label), (notification, action) =>
            {
                // Reject member only if we are still connected
                if (Controller.last_status >= 6)
                {
                    foreach (string network_id in network_ids)
                    {
                        Network network = Haguichi.window.network_view.return_network_by_id(network_id);
                        
                        foreach (Member member in network.members)
                        {
                            if ((member.client_id == client_id) &&
                                (member.status.status_int == 3))
                            {
                                member.reject();
                            }
                        }
                    }
                }
                // Close the notification as the action may not be executed without this call
                close();
            });
        }
    }
}
