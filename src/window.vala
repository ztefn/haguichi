/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

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
    
    public Paned content_box;
    public NetworkView network_view;
    
    private ScrolledWindow scrolled_window;
    private Widgets.MessageBox empty_box;
    private Box connected_box;
    private Box disconnected_box;
    
    private Spinner spinner;
    
    
    public HaguichiWindow ()
    {
        Object (application: Haguichi.app, title: Text.app_name);
        
        try
        {
            app_icons.append (IconTheme.get_default().load_icon ("haguichi",  16, IconLookupFlags.NO_SVG));
            app_icons.append (IconTheme.get_default().load_icon ("haguichi",  24, IconLookupFlags.NO_SVG));
            app_icons.append (IconTheme.get_default().load_icon ("haguichi",  32, IconLookupFlags.NO_SVG));
            app_icons.append (IconTheme.get_default().load_icon ("haguichi",  48, IconLookupFlags.NO_SVG));
            app_icons.append (IconTheme.get_default().load_icon ("haguichi", 256, IconLookupFlags.NO_SVG));
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
        
        empty_box = new Widgets.MessageBox();
        empty_box.set_message (Text.empty_list_heading, Text.empty_list_message);
        
        connected_box = new Box (Orientation.VERTICAL, 0);
        connected_box.pack_start (scrolled_window, true, true, 0);
        connected_box.pack_start (empty_box, true, true, 0);
        
        
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
        
        
        // Content Box
        
        Box left_box = new Box (Orientation.VERTICAL, 0);
        left_box.pack_start (message_bar,         false, false, 0);
        left_box.pack_start (search_bar_revealer, false, false, 0);
        left_box.pack_start (disconnected_box,    true,  true,  0);
        left_box.pack_start (connected_box,       true,  true,  0);
        
        content_box = new Paned (Orientation.HORIZONTAL);
        content_box.pack1 (left_box, false, false);
        content_box.pack2 (sidebar, false, true);
        content_box.position = (int) Settings.sidebar_position.val;
        content_box.size_allocate.connect (() =>
        {
            sidebar_pos = content_box.position;
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
        
        Gtk.Settings.get_default().notify["gtk-theme-name"].connect ((sender, property) =>
        {
            set_colors();
        });
        Gtk.Settings.get_default().notify["gtk-application-prefer-dark-theme"].connect ((sender, property) =>
        {
            set_colors();
        });
        set_colors();
        
        add_accel_group(accel_group);
        
        window_state_event.connect (on_state_changed);
        configure_event.connect (on_configure);
        delete_event.connect (on_win_delete);
        key_press_event.connect (on_key_press);
        
        maximized = (bool) Settings.win_maximized.val;
        
        if (maximized)
        {
            maximize();
        }
        
        width  = (int) Settings.win_width.val + Settings.decorator_offset;
        height = (int) Settings.win_height.val + Settings.decorator_offset;
        
        x = (int) Settings.win_x.val;
        y = (int) Settings.win_y.val;
        
        add (main_box);
        set_default_size (width, height);
        move (x, y);
        
        message_bar.hide();
        
        if (Haguichi.hidden == false)
        {
            show();
        }
    }
    
    private void set_colors ()
    {
        // Extracting normal background color from treeview for connected box
        TreeView tree_view = new TreeView();
        StyleContext tree_context = tree_view.get_style_context();
        tree_context.save();
        tree_context.set_state (StateFlags.NORMAL);
        connected_box.override_background_color (StateFlags.NORMAL, tree_context.get_background_color(tree_context.get_state()));
        tree_context.restore();
        
        // Extracting insensitive background color from textview for disconnected box
        TextView text_view = new TextView();
        StyleContext text_context = text_view.get_style_context();
        text_context.save();
        text_context.set_state (StateFlags.INSENSITIVE);
        disconnected_box.override_background_color (StateFlags.NORMAL, text_context.get_background_color(text_context.get_state()));
        text_context.restore();
    }
    
    private bool on_configure (Gdk.EventConfigure event)
    {
        move_resize();
        return false;
    }
    
    private void move_resize ()
    {
        Gdk.WindowState ws = get_window().get_state();
        
        if ((Gdk.WindowState.WITHDRAWN in ws) ||
            (Gdk.WindowState.ICONIFIED in ws) ||
            (Gdk.WindowState.MAXIMIZED in ws) ||
            (Gdk.WindowState.TILED in ws))
        {
            return; // Return when not in normal window state, otherwise we'll receive wrong positions
        }
        
        get_position (out x, out y);
        get_size (out width, out height);
        
        width  -= Settings.decorator_offset;
        height -= Settings.decorator_offset;
        
        //print ("x: %d  y: %d  w: %d  h: %d\n", x, y, width, height);
        
        if (width > Settings.switch_layout_threshold)
        {
            network_view.set_layout_from_string ("large");
        }
        else
        {
            network_view.set_layout_from_string ("small");
        }
        
        if (width > Settings.switch_sidebar_threshold)
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
            
            if (Gtk.check_version(3, 18, 0) == null)
            {
                minimum_width += 50;
            }
        }
        header_bar.show_hide_buttons (width, minimum_width);
    }
    
    public void save_geometry ()
    {
        Settings.win_x.val = x;
        Settings.win_y.val = y;
        Settings.win_width.val = width;
        Settings.win_height.val = height;
        Settings.win_maximized.val = maximized;
        Settings.sidebar_position.val = sidebar_pos;
    }
    
    private bool on_state_changed (Gdk.EventWindowState event)
    {
        minimized = (bool) (Gdk.WindowState.ICONIFIED in event.new_window_state);
        Debug.log (Debug.domain.GUI, "Window.on_state_changed", "Minimized: " + minimized.to_string());
        
        maximized = (bool) (Gdk.WindowState.MAXIMIZED in event.new_window_state);
        Debug.log (Debug.domain.GUI, "Window.on_state_changed", "Maximized: " + maximized.to_string());
        
        if (maximized)
        {
            network_view.set_layout_from_string ("large");
            sidebar.show();
            header_bar.show_all_buttons();
        }
        else
        {
            move_resize();
        }
        
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
            if (event.keyval == Gdk.Key.bracketleft)
            {
                network_view.expand_all();
            }
            if (event.keyval == Gdk.Key.bracketright)
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
                if (!GlobalEvents.search_active)
                {
                     GlobalEvents.start_search();
                }
                else if (!search_entry.has_focus)
                {
                    search_entry.grab_focus();
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
        base.show();
        
        Haguichi.session.visibility_changed (true);
        
        // Move window to the current desktop and correct for any desktop compositor deviation
        
        while (x < 0)
        {
            x += Gdk.Screen.width();
        }
        while (x > Gdk.Screen.width())
        {
            x -= Gdk.Screen.width();
        }
        
        while (y < 0)
        {
            y += Gdk.Screen.height();
        }
        while (y > Gdk.Screen.height())
        {
            y -= Gdk.Screen.height();
        }
        
        move (x, y);
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
    
    public void set_mode (string mode)
    {
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
                
                sidebar.show_tab ("Info");
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
