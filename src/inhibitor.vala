/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

[DBus (name = "org.freedesktop.login1.Manager")]
public interface LogindManager : DBusProxy
{
    public signal void prepare_for_sleep (bool before);
    public abstract UnixInputStream inhibit (string what, string who, string why, string mode) throws Error;
}

public class Inhibitor : Object
{
    private LogindManager manager;
    private UnixInputStream lock_file = null;

    public Inhibitor ()
    {
        try
        {
            // Connect to the logind manager on the system bus: https://www.freedesktop.org/wiki/Software/systemd/logind/
            manager = Bus.get_proxy_sync (BusType.SYSTEM, "org.freedesktop.login1", "/org/freedesktop/login1");
        }
        catch (IOError e)
        {
            Debug.log (Debug.domain.ERROR, "Inhibitor.manager", e.message);
        }
        
        manager.prepare_for_sleep.connect ((before) =>
        {
            if (before)
            {
                Debug.log (Debug.domain.ENVIRONMENT, "Inhibitor.manager", "Preparing for sleep...");
                
                // Only restore connection after wake up if currently connected or already marked to do so
                if ((Controller.restore) ||
                    (Controller.last_status >= 6))
                {
                    // Abort any pending connection restore cycle and make sure we don't trigger a new one either
                    Controller.restore_countdown = 0;
                    Controller.restore = false;
                    
                    // It's now safe to call the following method
                    GlobalEvents.connection_stopped();
                    
                    // And don't forget the most important bit!
                    Hamachi.logout();
                    
                    // Finally, mark the connection to be restored again after wake up
                    Controller.restore = true;
                }
                release();
            }
            else
            {
                Debug.log (Debug.domain.ENVIRONMENT, "Inhibitor.manager", "Waking up...");
                
                if (Controller.restore)
                {
                    // Restore the connection when internet is available again
                    Controller.wait_for_internet_cycle();
                }
                aquire();
            }
        });
        
        aquire();
    }

    public void release ()
    {
        Debug.log (Debug.domain.ENVIRONMENT, "Inhibitor.release", "Releasing inhibit lock.");
        
        if (lock_file != null)
        {
            try
            {
                lock_file.close();
            }
            catch (IOError e)
            {
                Debug.log (Debug.domain.ERROR, "Inhibitor.release", e.message);
            }
            finally
            {
                lock_file = null;
            }
        }
    }

    public void aquire ()
    {
        Debug.log (Debug.domain.ENVIRONMENT, "Inhibitor.aquire", "Acquiring inhibit lock.");
        
        try
        {
            lock_file = manager.inhibit ("sleep", "Haguichi", "Properly disconnect", "delay");
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "Inhibitor.aquire", e.message);
        }
    }
}
