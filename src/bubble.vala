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
    public Notify.Notification notification;
    
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
}
