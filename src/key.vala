/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Key : Object
{
    private GLib.Settings settings;
    private Variant _value;
    
    public string parent;
    public string key_name;
    
    public Key (string _parent, string _key_name)
    {
        parent = _parent;
        key_name = _key_name;
        
        settings = new GLib.Settings (Settings.schema_base_id + "." + parent);
        settings.changed[_key_name].connect (value_changed);
        
        get_value();
    }
    
    public Variant val
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            set_value (value);
        }
    }
    
    private void set_value (Variant val)
    {
        Debug.log (Debug.domain.INFO, "Key.set_value", "Setting value for GSettings key " + key_name + "...");
        
        settings.set_value (key_name, val);
    }
    
    private void get_value ()
    {
        Debug.log (Debug.domain.INFO, "Key.get_value", "Getting value for GSettings key " + key_name + "...");
        
        _value = settings.get_value (key_name);
        
        if (key_name == "nickname")
        {
            if ((string) _value == "%USERNAME")
            {
                _value = Environment.get_user_name();
            }
            else if ((string) _value == "%REALNAME")
            {
                _value = Environment.get_real_name();
            }
            else if ((string) _value == "%HOSTNAME")
            {
                _value = Environment.get_host_name();
            }
        }
    }
    
    public Variant get_default_value ()
    {
        Debug.log (Debug.domain.INFO, "Key.get_default_value", "Getting default value for GSettings key " + key_name + "...");
        
        return settings.get_default_value (key_name);
    }
    
    private void value_changed (string key)
    {
        if (key == key_name)
        {
            Debug.log (Debug.domain.INFO, "Key.value_changed", "Value for GSettings key " + key + " has been changed");
            get_value();
            
            if ((key == Settings.network_template_small.key_name) ||
                (key == Settings.network_template_large.key_name) ||
                (key == Settings.member_template_small.key_name) ||
                (key == Settings.member_template_large.key_name))
            {
                Haguichi.window.network_view.refresh_layout();
            }
            else if (key == Settings.nickname.key_name)
            {
                GlobalEvents.set_nick ((string) _value);
            }
            else if (key == Settings.protocol.key_name)
            {
                GlobalEvents.update_protocol ((string) _value);
            }
            else if (key == Settings.update_interval.key_name)
            {
                Haguichi.preferences_dialog.interval_spin.value = (int) _value;
                Haguichi.preferences_dialog.set_interval_string();
            }
            else if (key == Settings.update_network_list.key_name)
            {
                Haguichi.preferences_dialog.update_network_list.active = (bool) _value;
            }
            else if (key == Settings.connect_on_startup.key_name)
            {
                Haguichi.preferences_dialog.connect_on_startup.active = (bool) _value;
            }
            else if (key == Settings.reconnect_on_connection_loss.key_name)
            {
                Haguichi.preferences_dialog.reconnect_on_connection_loss.active = (bool) _value;
            }
            else if (key == Settings.disconnect_on_quit.key_name)
            {
                Haguichi.preferences_dialog.disconnect_on_quit.active = (bool) _value;
            }
            else if (key == Settings.notify_on_connection_loss.key_name)
            {
                Haguichi.preferences_dialog.notify_on_connection_loss.active = (bool) _value;
                
                if ((Haguichi.demo_mode) &&
                    ((bool) _value))
                {
                    new Bubble (Text.notify_connection_lost, "");
                }
            }
            else if (key == Settings.notify_on_member_join.key_name)
            {
                Haguichi.preferences_dialog.notify_on_member_join.active = (bool) _value;
                
                if (Haguichi.demo_mode)
                {
                    Controller.notify_member_joined ("T-800", "Skynet", 0); 
                }
            }
            else if (key == Settings.notify_on_member_leave.key_name)
            {
                Haguichi.preferences_dialog.notify_on_member_leave.active = (bool) _value;
                
                if (Haguichi.demo_mode)
                {
                    Controller.notify_member_left ("T-800", "Skynet", 0);
                }
            }
            else if (key == Settings.notify_on_member_online.key_name)
            {
                Haguichi.preferences_dialog.notify_on_member_online.active = (bool) _value;
                
                if (Haguichi.demo_mode)
                {
                    Controller.notify_member_online ("T-800", "Skynet", 1);
                }
            }
            else if (key == Settings.notify_on_member_offline.key_name)
            {
                Haguichi.preferences_dialog.notify_on_member_offline.active = (bool) _value;
                
                if (Haguichi.demo_mode)
                {
                    Controller.notify_member_offline ("T-800", "Skynet", 2);   
                }
            }
            else if (key == Settings.show_offline_members.key_name)
            {
                Haguichi.window.network_view.refilter();
            }
            else if (key == Settings.sort_network_list_by.key_name)
            {
                Haguichi.window.network_view.go_sort ((string) _value);
            }
            else if (key == Settings.init_system.key_name)
            {
                Hamachi.determine_service();
            }
            else if (key == Settings.super_user.key_name)
            {
                Command.determine_sudo();
            }
            else if (key == Settings.win_maximized.key_name)
            {
                if ((bool) _value)
                {
                    Haguichi.window.maximize();
                }
                else
                {
                    Haguichi.window.unmaximize();
                }
            }
            else if (key == Settings.prefer_dark_theme.key_name)
            {
                Gtk.Settings.get_default().set ("gtk-application-prefer-dark-theme", (bool) _value);
            }
        }
    }
}
