/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

[DBus (name = "com.github.ztefn.haguichi")]
public interface AppSession : Object
{
    public abstract void show();
    public abstract void hide();
    public abstract void start_hamachi();
    public abstract void stop_hamachi();
    public abstract void change_nick();
    public abstract void join_network();
    public abstract void create_network();
    public abstract void information();
    public abstract void preferences();
    public abstract void about();
    public abstract void quit_app();
    
    public abstract string get_mode();
    public abstract bool get_modality();
    public abstract bool get_visibility();
    
    public signal void mode_changed (string mode);
    public signal void modality_changed (bool modal);
    public signal void visibility_changed (bool visible);
    public signal void quitted ();
}

public class Haguichi.Indicator : Wingpanel.Indicator
{
    private const string icon_connected    = ICON_NAME + "-connected-symbolic";
    private const string icon_connecting1  = ICON_NAME + "-connecting-1-symbolic";
    private const string icon_connecting2  = ICON_NAME + "-connecting-2-symbolic";
    private const string icon_connecting3  = ICON_NAME + "-connecting-3-symbolic";
    private const string icon_disconnected = ICON_NAME + "-disconnected-symbolic";
    
    private bool bus_name_exists;
    private bool modal;
    private string mode;
    private int icon_num;
    
    private uint watch;
    private AppSession session;
    private GLib.Settings settings;
    
    private Grid? main_widget = null;
    private Wingpanel.Widgets.OverlayIcon? display_widget = null;
    private Granite.SwitchModelButton show_item;
    private ModelButton connecting_item;
    private ModelButton connect_item;
    private ModelButton disconnect_item;
    private ModelButton join_item;
    private ModelButton create_item;
    private ModelButton info_item;
    private ModelButton quit_item;
    
    public Indicator ()
    {
        Object (code_name: "haguichi-indicator");
        
        try
        {
            // Set up callback functions that signal when the app is available on the message bus or not
            watch = Bus.watch_name (BusType.SESSION, "com.github.ztefn.haguichi", BusNameWatcherFlags.AUTO_START, on_name_appeared, on_name_vanished);
            
            // Set up connection to the message bus
            session = Bus.get_proxy_sync (BusType.SESSION, "com.github.ztefn.haguichi", "/com/github/ztefn/haguichi");
            
            // Connect to proxy signals in order to receive state updates
            session.mode_changed.connect (set_mode);
            session.modality_changed.connect (set_window_modality);
            session.visibility_changed.connect (set_window_visibility);
        }
        catch (IOError e)
        {
            stderr.printf (e.message);
        }
        
        // Connect to settings daemon to determine if the indicator should be visible
        settings = new GLib.Settings ("com.github.ztefn.haguichi.ui");
        settings.changed["show-indicator"].connect ((key) =>
        {
            if (key == "show-indicator")
            {
                set_indicator_visibility();
            }
        });
        
        set_indicator_visibility();
    }
    
    // This method is called to get the widget that is displayed in the top bar
    public override Widget get_display_widget ()
    {
        // Check if the display widget is already created
        if (display_widget == null)
        {
            display_widget = new Wingpanel.Widgets.OverlayIcon (icon_disconnected);
            display_widget.scroll_event.connect ((event) =>
            {
                // Never hide the main window when a modal dialog is being shown
                if (!modal)
                {
                    // Show the main window when scrolling up and hide it when scrolling down
                    if (event.direction == Gdk.ScrollDirection.UP)
                    {
                        session.show();
                    }
                    else if (event.direction == Gdk.ScrollDirection.DOWN)
                    {
                        session.hide();
                    }
                }
                
                return false;
            });
        }
        
        return display_widget;
    }
    
    // This method is called to get the widget that is displayed in the popover
    public override Widget? get_widget ()
    {
        // Check if the main widget is already created
        if (main_widget == null)
        {
            main_widget = create_main_widget();
        }
        
        return main_widget;
    }
    
    // This method is called when the indicator popover opened
    public override void opened ()
    {
        set_mode (session.get_mode());
        set_window_modality (session.get_modality());
        set_window_visibility (session.get_visibility());
    }
    
    // This method is called when the indicator popover closed
    public override void closed ()
    {
        // Nothing to do here...
    }
    
    private Grid create_main_widget ()
    {
        var grid = new Grid();
        
        show_item = new Granite.SwitchModelButton (_("_Show Haguichi").replace ("_", ""));
        show_item.toggled.connect (() =>
        {
            if (show_item.active)
            {
                session.show();
            }
            else
            {
                session.hide();
            }
        });
        
        connecting_item = new ModelButton();
        connecting_item.sensitive = false;
        connecting_item.text = _("Connecting…").replace ("…", "");
        
        connect_item = new ModelButton();
        connect_item.text = _("C_onnect").replace ("_", "");
        connect_item.clicked.connect (() =>
        {
            session.start_hamachi();
        });
        
        disconnect_item = new ModelButton();
        disconnect_item.text = _("_Disconnect").replace ("_", "");
        disconnect_item.clicked.connect (() =>
        {
            session.stop_hamachi();
        });
        
        join_item = new ModelButton();
        join_item.text = _("_Join Network…").replace ("_", "");
        join_item.clicked.connect (() =>
        {
            session.join_network();
        });
        
        create_item = new ModelButton();
        create_item.text = _("_Create Network…").replace ("_", "");
        create_item.clicked.connect (() =>
        {
            session.create_network();
        });
        
        info_item = new ModelButton();
        info_item.text = _("_Information").replace ("_", "");
        info_item.clicked.connect (() =>
        {
            session.information();
        });
        
        quit_item = new ModelButton();
        quit_item.text = _("_Quit").replace ("_", "");
        quit_item.clicked.connect (() =>
        {
            session.quit_app();
        });
        
        grid.attach (show_item,                             0,  0, 1, 1);
        grid.attach (new Separator(Orientation.HORIZONTAL), 0,  1, 1, 1);
        grid.attach (connecting_item,                       0,  2, 1, 1);
        grid.attach (connect_item,                          0,  3, 1, 1);
        grid.attach (disconnect_item,                       0,  4, 1, 1);
        grid.attach (new Separator(Orientation.HORIZONTAL), 0,  5, 1, 1);
        grid.attach (join_item,                             0,  6, 1, 1);
        grid.attach (create_item,                           0,  7, 1, 1);
        grid.attach (new Separator(Orientation.HORIZONTAL), 0,  8, 1, 1);
        grid.attach (info_item,                             0,  9, 1, 1);
        grid.attach (new Separator(Orientation.HORIZONTAL), 0, 10, 1, 1);
        grid.attach (quit_item,                             0, 11, 1, 1);
        
        return grid;
    }
    
    private void on_name_appeared (DBusConnection conn, string name)
    {
        bus_name_exists = true;
        set_indicator_visibility();
        set_mode (session.get_mode());
        set_window_modality (session.get_modality());
    }
    
    private void on_name_vanished (DBusConnection conn, string name)
    {
        bus_name_exists = false;
        set_indicator_visibility();
    }
    
    private void set_indicator_visibility ()
    {
        this.visible = ((bool) settings.get_value ("show-indicator") && bus_name_exists);
    }
    
    private void set_window_visibility (bool _visible)
    {
        show_item.active = _visible;
    }
    
    private void set_window_modality (bool _modal)
    {
        modal = _modal;
        set_mode (mode);
    }
    
    private void set_mode (string _mode)
    {
        if (display_widget != null)
        {
            set_icon_mode (_mode);
        }
        
        if (main_widget != null)
        {
            set_menu_mode (_mode);
        }
        
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
            (display_widget.get_main_icon_name().has_prefix(ICON_NAME + "-connecting")))
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
                display_widget.set_main_icon_name (icon_connected);
                break;
                
            default:
                display_widget.set_main_icon_name (icon_disconnected);
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
            display_widget.set_main_icon_name (icon_connecting1);
            icon_num = 1;
        }
        else if (icon_num == 1)
        {
            display_widget.set_main_icon_name (icon_connecting2);
            icon_num = 2;
        }
        else
        {
            display_widget.set_main_icon_name (icon_connecting3);
            icon_num = 0;
        }
        
        return true;
    }
}

public Wingpanel.Indicator? get_indicator (Module module, Wingpanel.IndicatorManager.ServerType server_type)
{
    if (server_type != Wingpanel.IndicatorManager.ServerType.SESSION)
    {
        // Only display the indicator in the "normal" session
        return null;
    }
    
    return new Haguichi.Indicator();
}
