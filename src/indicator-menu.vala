/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class IndicatorMenu : Gtk.Menu
{
    public string mode;
    public bool modal;
    
    private int icon_num;
    
    private Gtk.CheckMenuItem show_item;
    private Gtk.MenuItem connecting_item;
    private Gtk.MenuItem connect_item;
    private Gtk.MenuItem disconnect_item;
    private Gtk.MenuItem join_item;
    private Gtk.MenuItem create_item;
    private Gtk.MenuItem info_item;
    private Gtk.MenuItem quit_item;
    
    public IndicatorMenu()
    {
        show_item = new Gtk.CheckMenuItem.with_mnemonic (Text.show_app);
        show_item.active = get_visibility();
        show_item.toggled.connect (() =>
        {
            if (show_item.active)
            {
                show_window();
            }
            else
            {
                hide_window();
            }
        });
        
        connecting_item = new Gtk.MenuItem.with_label (Text.connecting.replace ("…", ""));
        connecting_item.sensitive = false;
        
        connect_item = new Gtk.MenuItem.with_mnemonic (Text.connect_label);
        connect_item.activate.connect (start_hamachi);
        
        disconnect_item = new Gtk.MenuItem.with_mnemonic (Text.disconnect_label);
        disconnect_item.activate.connect (stop_hamachi);
        
        join_item = new Gtk.MenuItem.with_mnemonic (Text.join_network_label);
        join_item.activate.connect (join_network);
        
        create_item = new Gtk.MenuItem.with_mnemonic (Text.create_network_label);
        create_item.activate.connect (create_network);
        
        info_item = new Gtk.MenuItem.with_mnemonic (Text.information_label);
        info_item.activate.connect (information);
        
        quit_item = new Gtk.MenuItem.with_mnemonic (Text.quit_label);
        quit_item.activate.connect (quit_app);
        
        
        add (show_item);
        add (new Gtk.SeparatorMenuItem());
        add (connecting_item);
        add (connect_item);
        add (disconnect_item);
        add (new Gtk.SeparatorMenuItem());
        add (join_item);
        add (create_item);
        add (new Gtk.SeparatorMenuItem());
        add (info_item);
        add (new Gtk.SeparatorMenuItem());
        add (quit_item);
        
        show_all();
        
        Haguichi.session.mode_changed.connect (set_mode);
        Haguichi.session.modality_changed.connect (set_modality);
        Haguichi.session.visibility_changed.connect (set_visibility);
        Haguichi.session.quitted.connect (() =>
        {
            Haguichi.session.mode_changed.disconnect (set_mode);
            Haguichi.session.modality_changed.disconnect (set_modality);
            Haguichi.session.visibility_changed.disconnect (set_visibility);
        });
    }
    
    public void show_window ()
    {
        try
        {
            Haguichi.session.show();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.show_window", e.message);
        }
    }
    
    public void hide_window ()
    {
        try
        {
            Haguichi.session.hide();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.hide_window", e.message);
        }
    }
    
    private void start_hamachi ()
    {
        try
        {
            Haguichi.session.start_hamachi();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.start_hamachi", e.message);
        }
    }
    
    private void stop_hamachi ()
    {
        try
        {
            Haguichi.session.stop_hamachi();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.stop_hamachi", e.message);
        }
    }
    
    private void join_network ()
    {
        try
        {
            Haguichi.session.join_network();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.join_network", e.message);
        }
    }
    
    private void create_network ()
    {
        try
        {
            Haguichi.session.create_network();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.create_network", e.message);
        }
    }
    
    private void information ()
    {
        try
        {
            Haguichi.session.information();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.information", e.message);
        }
    }
    
    private void quit_app ()
    {
        try
        {
            Haguichi.session.quit_app();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.quit_app", e.message);
        }
    }
    
    private bool get_visibility ()
    {
        try
        {
            return Haguichi.session.get_visibility();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "IndicatorMenu.get_visibility", e.message);
            return false;
        }
    }
    
    private void set_visibility (bool _visible)
    {
        show_item.active = _visible;
    }
    
    private void set_modality (bool _modal)
    {
        modal = _modal;
        set_mode (mode);
    }
    
    private void set_mode (string _mode)
    {
        set_icon_mode (_mode);
        set_menu_mode (_mode);
        
        mode = _mode;
    }
    
    private void set_menu_mode (string _mode)
    {
        switch (_mode)
        {
            case "Connecting":
                connecting_item.show();
                connect_item.hide();
                disconnect_item.hide();
                join_item.sensitive       = false;
                create_item.sensitive     = false;
                info_item.sensitive       = true;
                break;
                
            case "Connected":
                connecting_item.hide();
                connect_item.hide();
                disconnect_item.sensitive = true;
                disconnect_item.show();
                join_item.sensitive       = true;
                create_item.sensitive     = true;
                info_item.sensitive       = true;
                break;
                
            case "Disconnected":
                connecting_item.hide();
                connect_item.sensitive    = true;
                connect_item.show();
                disconnect_item.hide();
                join_item.sensitive       = false;
                create_item.sensitive     = false;
                info_item.sensitive       = true;
                break;
                
            default:
                connecting_item.hide();
                connect_item.sensitive    = false;
                connect_item.show();
                disconnect_item.hide();
                join_item.sensitive       = false;
                create_item.sensitive     = false;
                info_item.sensitive       = false;
                break;
        }
        
        if (modal)
        {
            show_item.sensitive       = false;
            connect_item.sensitive    = false;
            disconnect_item.sensitive = false;
            join_item.sensitive       = false;
            create_item.sensitive     = false;
            info_item.sensitive       = false;
        }
        else
        {
            show_item.sensitive       = true;
        }
    }
    
    private void set_icon_mode (string _mode)
    {
        // Check if there isn't already an animation going on when connecting
        if ((_mode == "Connecting") &&
            (Haguichi.indicator.get_icon_name().has_prefix(ICON_NAME + "-connecting")))
        {
            return;
        }
        
        icon_num = 0;
        
        switch (_mode)
        {
            case "Connecting":
                Timeout.add (400, switch_icon);
                break;
                
            case "Connected":
                Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_connected);
                break;
                
            default:
                Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_disconnected);
                break;
        }
    }
    
    private bool switch_icon ()
    {
        if (mode != "Connecting")
        {
            return false;
        }
        
        if (icon_num == 0)
        {
            Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_connecting1);
            icon_num = 1;
        }
        else if (icon_num == 1)
        {
            Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_connecting2);
            icon_num = 2;
        }
        else
        {
            Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_connecting3);
            icon_num = 0;
        }
        
        return true;
    }
}
