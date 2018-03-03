/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2018 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;
using Config;

public class HaguichiWindow : Gtk.ApplicationWindow
{
    public static List<Gdk.Pixbuf> app_icons = new List<Gdk.Pixbuf>();
    
    public static AccelGroup accel_group;
    public static Headerbar header_bar;
    public static Widgets.MessageBar message_bar;
    public static Widgets.MessageBox message_box;
    public static Sidebar sidebar;
    
    public static SearchEntry search_entry;
    public static Toolbar search_bar;
    public static Revealer search_bar_revealer;
    
    public static int minimum_width;
    
    private int x;
    private int y;
    private int width;
    private int height;
    private int sidebar_pos;
    
    private bool minimized;
    private bool maximized;
    
    public string mode;
    
    public Paned content_box;
    public NetworkView network_view;
    
    private ScrolledWindow scrolled_window;
    private EventBox empty_box;
    private Box connected_box;
    private Box disconnected_box;
    
    private Menus.JoinCreateMenu join_create_menu;
    
    private Spinner spinner;
    
    private CssProvider provider;
    
    public HaguichiWindow ()
    {
        Object (application: Haguichi.app, title: Text.app_name);
        
        try
        {
            app_icons.append (IconTheme.get_default().load_icon (ICON_NAME,  16, IconLookupFlags.FORCE_SIZE));
            app_icons.append (IconTheme.get_default().load_icon (ICON_NAME,  24, IconLookupFlags.FORCE_SIZE));
            app_icons.append (IconTheme.get_default().load_icon (ICON_NAME,  32, IconLookupFlags.FORCE_SIZE));
            app_icons.append (IconTheme.get_default().load_icon (ICON_NAME,  48, IconLookupFlags.FORCE_SIZE));
            app_icons.append (IconTheme.get_default().load_icon (ICON_NAME, 256, IconLookupFlags.FORCE_SIZE));
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "HaguichiWindow.app_icons", e.message);
        }
        set_icon_list (app_icons);
        
        accel_group = new AccelGroup();
        header_bar  = new Headerbar();
        message_bar = new Widgets.MessageBar();
        message_box = new Widgets.MessageBox();
        sidebar     = new Sidebar();
        
        search_entry = new SearchEntry();
        search_entry.key_release_event.connect (search_entry_on_key_release);
        search_entry.search_changed.connect (() =>
        {
            network_view.refilter();
        });
        
        ToolItem search_item = new ToolItem();
        search_item.add (search_entry);
        search_item.set_expand (true);
        
        search_bar = new Toolbar();
        search_bar.add (search_item);
        search_bar.get_style_context().add_class ("search-bar");
        
        search_bar_revealer = new Revealer();
        search_bar_revealer.add (search_bar);
        
        
        // Connected Box
        
        network_view = new NetworkView();
        
        scrolled_window = new ScrolledWindow (null, null);
        scrolled_window.add (network_view);
        scrolled_window.set_policy (PolicyType.NEVER, PolicyType.AUTOMATIC);
        
        join_create_menu = new Menus.JoinCreateMenu();
        
        Widgets.MessageBox empty_message_box = new Widgets.MessageBox();
        empty_message_box.set_message (Text.empty_list_heading, Text.empty_list_message);
        
        empty_box = new EventBox();
        empty_box.add (empty_message_box);
        empty_box.button_press_event.connect ((event) =>
        {
            if (event.button == 3)
            {
#if GTK_3_22
                join_create_menu.popup_at_pointer (event);
#else
                join_create_menu.popup (null, null, null, 0, get_current_event_time());
#endif
                return true;
            }
            
            return false;
        });
        
        connected_box = new Box (Orientation.VERTICAL, 0);
        connected_box.pack_start (scrolled_window, true, true, 0);
        connected_box.pack_start (empty_box, true, true, 0);
        connected_box.get_style_context().add_class ("connected-box");
        
        
        // Disconnected Box
        
        spinner = new Spinner();
        spinner.height_request = 20;
        spinner.width_request  = 20;
        
        disconnected_box = new Box (Orientation.VERTICAL, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (spinner, false, false, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.pack_start (new Box (Orientation.VERTICAL, 0), true, true, 0);
        disconnected_box.get_style_context().add_class ("disconnected-box");
        
        
        // Content Box
        
        Box left_box = new Box (Orientation.VERTICAL, 0);
        left_box.pack_start (message_bar,         false, false, 0);
        left_box.pack_start (search_bar_revealer, false, false, 0);
        left_box.pack_start (disconnected_box,    true,  true,  0);
        left_box.pack_start (connected_box,       true,  true,  0);
        
        sidebar_pos = (int) Settings.sidebar_position.val;
        
        content_box = new Paned (Orientation.HORIZONTAL);
        content_box.pack1 (left_box, false, false);
        content_box.pack2 (sidebar, false, true);
        content_box.position = sidebar_pos;
        content_box.set_size_request (-1, 180);
        content_box.size_allocate.connect (() =>
        {
            Gdk.Window w = get_window();
            
            // Only update position when in normal window state
            if (w != null && is_state_normal (w.get_state()))
            {
                sidebar_pos = content_box.position;
            }
        });
        
        
        // Main VBox
        
        Box main_box = new Box (Orientation.VERTICAL, 0);
        
        if (Haguichi.window_use_header_bar)
        {
            set_titlebar (header_bar);
        }
        else
        {
            main_box.pack_start (header_bar, false, false, 0);
        }
        
        main_box.pack_start (message_box, true, true, 0);
        main_box.pack_start (content_box, true, true, 0);
        main_box.show_all();
        
        provider = new CssProvider();
        StyleContext.add_provider_for_screen (this.get_screen(), provider, Gtk.STYLE_PROVIDER_PRIORITY_APPLICATION);
        
        Gtk.Settings.get_default().notify["gtk-theme-name"].connect ((sender, property) =>
        {
            set_styles();
        });
        Gtk.Settings.get_default().notify["gtk-application-prefer-dark-theme"].connect ((sender, property) =>
        {
            set_styles();
        });
        set_styles();
        
        add_accel_group(accel_group);
        
        window_state_event.connect (on_state_changed);
        configure_event.connect (on_configure);
        delete_event.connect (on_win_delete);
        key_press_event.connect (on_key_press);
        
        width  = (int) Settings.width.val  + Settings.decorator_offset;
        height = (int) Settings.height.val + Settings.decorator_offset;
        
        x = (int) Settings.position_x.val;
        y = (int) Settings.position_y.val;
        
        add (main_box);
        set_default_size (width, height);
        move (x, y);
        
        if (Haguichi.hidden == false)
        {
            show();
        }
    }
    
    private void set_styles ()
    {
        // Request stylesheet for current theme
        string theme_name = Gtk.Settings.get_default().gtk_theme_name.down();
        
        // Don't use sidebar class with elementary theme because it looks hideous
        if (theme_name == "elementary")
        {
            sidebar.remove_style_class ("sidebar");
        }
        else
        {
            sidebar.add_style_class ("sidebar");
        }
        
        if ((theme_name == "elementary") ||
            (theme_name == "adwaita"))
        {
            if (Gtk.Settings.get_default().gtk_application_prefer_dark_theme)
            {
                theme_name += "-dark";
            }
        }
        else
        {
            theme_name = "default";
        }
        
        provider.load_from_resource ("/com/github/ztefn/haguichi/stylesheets/" + theme_name + ".css");
    }
    
    private bool is_state_normal (Gdk.WindowState ws)
    {
        if ((Gdk.WindowState.FULLSCREEN in ws) ||
            (Gdk.WindowState.MAXIMIZED in ws) ||
            (Gdk.WindowState.TILED in ws))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    private bool on_configure (Gdk.EventConfigure event)
    {
        move_resize();
        return false;
    }
    
    private void move_resize ()
    {
        Gdk.WindowState ws = get_window().get_state();
        
        // Return when not window is not visible, otherwise we might receive wrong position and size
        if ((Gdk.WindowState.WITHDRAWN in ws) ||
            (Gdk.WindowState.ICONIFIED in ws))
        {
            return;
        }
        
        int new_x, new_y, new_width, new_height;
        
        get_position (out new_x, out new_y);
        get_size (out new_width, out new_height);
        
        new_width  -= Settings.decorator_offset;
        new_height -= Settings.decorator_offset;
        
        //print ("x: %d  y: %d  w: %d  h: %d\n", new_x, new_y, new_width, new_height);
        
        // Only update position and size when in normal window state
        if (is_state_normal (ws))
        {
            x = new_x;
            y = new_y;
            
            width  = new_width;
            height = new_height;
        }
        
        if (new_width > Settings.switch_layout_threshold)
        {
            network_view.set_layout_from_string ("large");
        }
        else
        {
            network_view.set_layout_from_string ("small");
        }
        
        if (new_width > Settings.switch_sidebar_threshold)
        {
            sidebar.show();
        }
        else
        {
            sidebar.hide();
        }
        
        if (minimum_width == 0)
        {
            get_preferred_width (out minimum_width, null);
            minimum_width -= Settings.decorator_offset;
            
            // Add 50 pixels just for GTK+ version 3.18
            if ((Gtk.check_version (3, 18, 0) == null) &&
                (Gtk.check_version (3, 20, 0) != null))
            {
                minimum_width += 50;
            }
        }
        header_bar.show_hide_buttons (new_width, minimum_width);
    }
    
    public void save_geometry ()
    {
        Settings.position_x.val = x;
        Settings.position_y.val = y;
        Settings.width.val = width;
        Settings.height.val = height;
        Settings.sidebar_position.val = sidebar_pos;
    }
    
    private bool on_state_changed (Gdk.EventWindowState event)
    {
        minimized = (bool) (Gdk.WindowState.ICONIFIED in event.new_window_state);
        Debug.log (Debug.domain.GUI, "Window.on_state_changed", "Minimized: " + minimized.to_string());
        
        maximized = (bool) (Gdk.WindowState.MAXIMIZED in event.new_window_state);
        Debug.log (Debug.domain.GUI, "Window.on_state_changed", "Maximized: " + maximized.to_string());
        
        return false;
    }
    
    private bool on_win_delete ()
    {
        hide();
        return true;
    }
    
    private bool search_entry_on_key_release (Gdk.EventKey event)
    {
        if ((event.keyval == Gdk.Key.Return) ||
            (event.keyval == Gdk.Key.ISO_Enter) ||
            (event.keyval == Gdk.Key.3270_Enter) ||
            (event.keyval == Gdk.Key.KP_Enter))
        {
            network_view.activate_selected_row();
        }
        return false;
    }
    
    private bool on_key_press (Gdk.EventKey event)
    {
        if (Gdk.ModifierType.CONTROL_MASK in event.state)
        {
            if (((event.keyval >= 49) &&         // "1"
                 (event.keyval <= 57)) ||        // "9"
                ((event.keyval >= 65457) &&      // "1" - NumPad
                 (event.keyval <= 65465)))       // "9" - NumPad
            {
                int number = (int) event.keyval;
                
                if (number > 65456)
                {
                    number -= 65456;
                }
                else if (number > 48)
                {
                    number -= 48;
                }
                network_view.activate_command_by_number (number);
            }
            else if (event.keyval == Gdk.Key.bracketleft)
            {
                network_view.expand_all();
            }
            else if (event.keyval == Gdk.Key.bracketright)
            {
                network_view.collapse_all();
            }
        }
        else if (!((Gdk.ModifierType.SUPER_MASK in event.state) ||
                   (Gdk.ModifierType.MOD1_MASK in event.state)))
        {
            if (((event.keyval >= 48) &&         // "0"
                 (event.keyval <= 122)) ||       // "z"
                ((event.keyval >= 65456) &&      // "0" - NumPad
                 (event.keyval <= 65465)))       // "9" - NumPad
            {
                if (GlobalActions.start_search.enabled)
                {
                    if (!GlobalEvents.search_active)
                    {
                        GlobalEvents.start_search();
                    }
                    else if (!search_entry.has_focus)
                    {
                        search_entry.grab_focus();
                    }
                }
            }
            else if (event.keyval == Gdk.Key.Escape)
            {
                GlobalEvents.stop_search();
            }
        }
        
        return false;
    }
    
    public void toggle_main_window ()
    {
        if (Haguichi.modal_dialog == null)
        {
            if (minimized || !visible)
            {
                show();
            }
            else
            {
                hide();
            }
        }
        else
        {
            Haguichi.modal_dialog.present();
        }
    }
    
    public override void show ()
    {
        // Move window to the current desktop and correct for any desktop compositor deviation
        
        var window = get_window();
        
        if (window != null)
        {
#if GTK_3_22
            var display  = get_display();
            var monitor  = display.get_monitor_at_window (window);
            var geometry = monitor.get_geometry();
            
            int screen_width  = geometry.width;
            int screen_height = geometry.height;
#else
            int screen_width  = Gdk.Screen.width();
            int screen_height = Gdk.Screen.height();
#endif
            
            while (x < 0)
            {
                x += screen_width;
            }
            while (x > screen_width)
            {
                x -= screen_width;
            }
            
            while (y < 0)
            {
                y += screen_height;
            }
            while (y > screen_height)
            {
                y -= screen_height;
            }
            
            move (x, y);
        }
        
        base.show();
        
        Haguichi.session.visibility_changed (true);
    }    
    
    public override void hide ()
    {
        save_geometry();
        
        base.hide();
        
        Haguichi.session.visibility_changed (false);
    }
    
    public void set_nick (string nick)
    {
        if (nick == "")
        {
            nick = Text.anonymous;
        }
        
        header_bar.title = nick;
    }
    
    public void update ()
    {
        if (Haguichi.connection.networks.length() == 0)
        {
            empty_box.show();
            scrolled_window.hide();
        }
        else
        {
            empty_box.hide();
            scrolled_window.show();
        }
        
        sidebar.update();
    }
    
    public void set_mode (string _mode)
    {
        mode = _mode;
        
        message_box.hide();
        content_box.hide();
        
        switch (mode)
        {
            case "Countdown":
                content_box.show();
                
                disconnected_box.show();
                connected_box.hide();
                
                Haguichi.session.mode_changed ("Disconnected");
                header_bar.set_mode (mode);
                break;
                
            case "Connecting":
                set_mode ("Disconnected");
                mode = _mode;
                
                spinner.sensitive = true;
                spinner.start();
                
                disconnected_box.show();
                connected_box.hide();
                
                Haguichi.session.mode_changed (mode);
                header_bar.set_mode (mode);
                break;
                
            case "Connected":
                content_box.show();
                
                spinner.hide();
                
                disconnected_box.hide();
                connected_box.show();
                
                Haguichi.session.mode_changed (mode);
                header_bar.set_mode (mode);
                sidebar.set_mode (mode);
                break;
                
            case "Disconnected":
                content_box.show();
                
                spinner.sensitive = false;
                spinner.stop();
                spinner.show();
                
                disconnected_box.show();
                connected_box.hide();
                
                Haguichi.session.mode_changed (mode);
                header_bar.set_mode (mode);
                sidebar.set_mode (mode);
                break;
                
            case "Not configured":
                message_box.show();
                
                Haguichi.session.mode_changed (mode);
                header_bar.set_mode  (mode);
                break;
                
            case "Not installed":
                message_box.show();
                
                Haguichi.session.mode_changed (mode);
                header_bar.set_mode (mode);
                break;
        }
    }
}
