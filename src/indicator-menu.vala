/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
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
        show_item = new Gtk.CheckMenuItem.with_mnemonic (_("_Show Haguichi"));
        show_item.toggled.connect (() =>
        {
            if (show_item.active)
            {
                Haguichi.session.show();
            }
            else
            {
                Haguichi.session.hide();
            }
        });
        
        connecting_item = new Gtk.MenuItem.with_label (_("Connecting…").replace ("…", ""));
        connecting_item.sensitive = false;
        
        connect_item = new Gtk.MenuItem.with_mnemonic (_("C_onnect"));
        connect_item.activate.connect (() =>
        {
            Haguichi.session.start_hamachi();
        });
        
        disconnect_item = new Gtk.MenuItem.with_mnemonic (_("_Disconnect"));
        disconnect_item.activate.connect (() =>
        {
            Haguichi.session.stop_hamachi();
        });
        
        join_item = new Gtk.MenuItem.with_mnemonic (_("_Join Network…"));
        join_item.activate.connect (() =>
        {
            Haguichi.session.join_network();
        });
        
        create_item = new Gtk.MenuItem.with_mnemonic (_("_Create Network…"));
        create_item.activate.connect (() =>
        {
            Haguichi.session.create_network();
        });
        
        info_item = new Gtk.MenuItem.with_mnemonic (_("_Information"));
        info_item.activate.connect (() =>
        {
            Haguichi.session.information();
        });
        
        quit_item = new Gtk.MenuItem.with_mnemonic (_("_Quit"));
        quit_item.activate.connect (() =>
        {
            Haguichi.session.quit_app();
        });
        
        
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
            Haguichi.app.quit();
        });
    }
    
    public void set_visibility (bool visible)
    {
        show_item.active = visible;
    }
    
    public void set_modality (bool _modal)
    {
        modal = _modal;
        
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
            info_item.sensitive       = true;
            
            set_mode (mode);
        }
    }
    
    private bool switch_icon ()
    {
        if (mode == "Connecting")
        {
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
        else
        {
            return false;
        }
    }
    
    public void set_mode (string _mode)
    {
        mode = _mode;
        icon_num = 0;
        
        switch (mode)
        {
            case "Connecting":
                connect_item.hide();
                connecting_item.show();
                
                GLib.Timeout.add (400, switch_icon);
                break;
                
            case "Connected":
                connecting_item.hide();
                connect_item.hide();
                disconnect_item.show();
                disconnect_item.sensitive = true;
                join_item.sensitive       = true;
                create_item.sensitive     = true;
                info_item.sensitive       = true;
                
                Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_connected);
                break;
                
            case "Disconnected":
                connecting_item.hide();
                connect_item.show();
                connect_item.sensitive    = true;
                disconnect_item.hide();
                join_item.sensitive       = false;
                create_item.sensitive     = false;
                info_item.sensitive       = true;
                
                Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_disconnected);
                break;
            
            case "Not configured":
                connecting_item.hide();
                connect_item.sensitive    = false;
                disconnect_item.hide();
                join_item.sensitive       = false;
                create_item.sensitive     = false;
                info_item.sensitive       = false;
                
                Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_disconnected);
                break;
            
            case "Not installed":
                connecting_item.hide();
                connect_item.sensitive    = false;
                disconnect_item.hide();
                join_item.sensitive       = false;
                create_item.sensitive     = false;
                info_item.sensitive       = false;
                
                Haguichi.indicator.set_icon_name (Haguichi.indicator.icon_disconnected);
                break;
        }
    }
}
