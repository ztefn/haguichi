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

class Haguichi : Gtk.Application
{
    public static Gtk.Application app;
#if ENABLE_APPINDICATOR
    public static HaguichiIndicator indicator;
#endif
    public static HaguichiWindow window;
    public static Dialogs.Preferences preferences_dialog;
    
    public static Window modal_dialog;
    
    public static Connection connection;
    public static AppSession session;
    public static Inhibitor inhibitor;
    
    public static string current_desktop;
    
    public static bool use_app_menu;
    public static bool window_use_header_bar;
    public static bool dialog_use_header_bar;
    
    public static bool hidden;
    public static bool debugging;
    public static bool demo_mode;
    public static string demo_list_path;
    
    public static ThreadPool<Member>  member_threads;
    public static ThreadPool<Network> network_threads;
    
    private static int activate_count;
    private static uint registration_id;
    private static int64 startup_moment;
    
    public Haguichi ()
    {
        Object (application_id: "com.github.ztefn.haguichi", flags: ApplicationFlags.FLAGS_NONE);
    }
    
    public override void activate ()
    {
        activate_count ++;
        
        if (activate_count > 1)
        {
            Debug.log (Debug.domain.ENVIRONMENT, "Haguichi.activate", "Received activate signal, presenting window");
            Haguichi.window.present();
        }
    }
    
    public override bool dbus_register (DBusConnection connection, string object_path) throws Error
    {
        base.dbus_register (connection, object_path);
        
        session = new AppSession();
        registration_id = connection.register_object ("/com/github/ztefn/haguichi", session);
        
        return true;
    }
    
    public override void dbus_unregister (DBusConnection connection, string object_path)
    {
        connection.unregister_object (registration_id);
        
        base.dbus_unregister (connection, object_path);
    }
    
    public override void startup ()
    {
        base.startup();
        
        startup_moment = get_real_time();
        
        Intl.bindtextdomain (GETTEXT_PACKAGE, LOCALEDIR);
        Intl.bind_textdomain_codeset (GETTEXT_PACKAGE, "UTF-8");
        Intl.textdomain (GETTEXT_PACKAGE);
        
        Debug.log (Debug.domain.INFO, "Haguichi.startup", "Greetings, I am " + Text.app_name + " " + Text.app_version);
        
        Notify.init (Text.app_name);
        Text.init();
        Settings.init();
        
        current_desktop = Environment.get_variable ("XDG_CURRENT_DESKTOP");
        
        // Check if we should use an app menu on any GNOME (based) desktop,
        // thus also match desktops like "ubuntu:GNOME", "Budgie:GNOME" and "GNOME-Flashback"
        if ((current_desktop.contains ("GNOME")) &&
            (app.prefers_app_menu()))
        {
            use_app_menu = true;
        }
        
        // Only on specific desktops we use header bars and possibly dark theme
        if ((current_desktop.contains ("GNOME")) ||
            (current_desktop == "Deepin") ||
            (current_desktop == "KDE") ||
            (current_desktop == "Pantheon") ||
            (current_desktop == "XFCE") ||
            (current_desktop == "X-Cinnamon"))
        {
            window_use_header_bar = true;
            Gtk.Settings.get_default().get ("gtk-dialogs-use-header", ref dialog_use_header_bar);
            Gtk.Settings.get_default().set ("gtk-application-prefer-dark-theme", (bool) Settings.prefer_dark_theme.val);
            
            // Add 52 pixels offset for all GTK+ versions before 3.20
            if (Gtk.check_version (3, 20, 0) != null)
            {
            	Settings.decorator_offset = 52;
            }
        }
        
        GlobalActions.init (app);
        Command.init();
        Hamachi.init();
        
        window = new HaguichiWindow();
        add_window (window);
        
#if ENABLE_APPINDICATOR
        indicator = new HaguichiIndicator();
        indicator.active = (bool) Settings.show_indicator.val;
#endif
        
        preferences_dialog = new Dialogs.Preferences();
        
        connection = new Connection();
        inhibitor = new Inhibitor();
        
        try
        {
            member_threads  = new ThreadPool<Member>.with_owned_data  ((member)  => { member.get_long_nick_thread();        }, 2, false);
            network_threads = new ThreadPool<Network>.with_owned_data ((network) => { network.determine_ownership_thread(); }, 2, false);
        }
        catch (ThreadError e)
        {
            Debug.log (Debug.domain.ERROR, "Haguichi.startup", e.message);
        }
        
        Controller.last_status = -3;
        Controller.init();
        
        Debug.log (Debug.domain.INFO, "Haguichi.startup", "Completed startup in " + (get_real_time() - startup_moment).to_string() + " microseconds");
    }
    
    public static int main (string[] args)
    {
        hidden = false;
        debugging = false;
        demo_mode = false;
        demo_list_path = null;
        
        foreach (string s in args)
        {
            if ((s == "-h") || (s == "--help"))
            {
                stdout.printf ("%s\n", Text.app_help);
                return 0;
            }
            if ((s == "-v") || (s == "--version"))
            {
                stdout.printf ("%s %s\n", Text.app_name, Text.app_version);
                return 0;
            }
            if (s == "--license")
            {
                stdout.printf ("\n%s\n\n%s\n\n", Text.app_info, Text.app_license);
                return 0;
            }
            
            if (s == "--hidden")
            {
                hidden = true;
            }
            else if ((s == "-d") || (s == "--debug"))
            {
                debugging = true;
            }
            else if (s == "--demo")
            {
                demo_mode = true;
            }
            else if (s.has_prefix ("--list="))
            {
                demo_list_path = s.replace ("--list=", "");
            }
            else if (s.has_prefix ("-"))
            {
                stdout.printf ("Unknown option %s\n\n%s\n", s, Text.app_help);
                return 0;
            }
        }
        
        app = new Haguichi();
        return app.run();
    }
}
