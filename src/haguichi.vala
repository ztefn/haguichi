/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2015 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

class Haguichi : Gtk.Application
{
    public static Gtk.Application app;
    public static HaguichiWindow window;
    public static Dialogs.Preferences preferences_dialog;
    
    public static Dialog modal_dialog;
    
    public static Connection connection;
    public static AppSession session;
    
    public static bool window_use_header_bar;
    public static bool dialog_use_header_bar;
    
    public static bool hidden;
    public static bool debugging;
    public static bool demo_mode;
    public static string demo_list_path;
    
    private static int activate_count;
    private static uint registration_id;
    
    public Haguichi ()
    {
        Object (application_id: "apps.Haguichi", flags: ApplicationFlags.FLAGS_NONE);
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
        registration_id = connection.register_object ("/apps/Haguichi", session);
        
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
        
        Debug.log (Debug.domain.INFO, "Haguichi.startup", "Greetings, I am " + Text.app_name + " " + Text.app_version);
        
        if (Command.exists ("haguichi-indicator"))
        {
            Debug.log (Debug.domain.ENVIRONMENT, "Haguichi.startup", "Launching haguichi-indicator...");
            Command.execute ("haguichi-indicator");
        }
        
        Intl.textdomain (Text.app_name.down());
        Text.init();
        
        Settings.init();
        
        if ((Environment.get_variable ("XDG_CURRENT_DESKTOP") == "GNOME") ||
            (Environment.get_variable ("XDG_CURRENT_DESKTOP") == "Pantheon") ||
            (Environment.get_variable ("XDG_CURRENT_DESKTOP") == "XFCE") ||
            (Environment.get_variable ("XDG_CURRENT_DESKTOP").has_suffix (":GNOME"))) // Any GNOME based desktop, for example "Budgie:GNOME"
        {
            window_use_header_bar = true;
            Settings.decorator_offset = 52;
            Gtk.Settings.get_default().set ("gtk-application-prefer-dark-theme", (bool) Settings.prefer_dark_theme.val);
        }
        Gtk.Settings.get_default().get ("gtk-dialogs-use-header", ref dialog_use_header_bar);
        
        GlobalActions.init (app);
        Command.init();
        Hamachi.init();
        
        window = new HaguichiWindow();
        add_window (window);
        
        preferences_dialog = new Dialogs.Preferences();
        
        connection = new Connection();
        
        Controller.last_status = -3;
        Controller.init();
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
                stdout.printf ("%s\n", Text.app_name + " " + Text.app_version);
                return 0;
            }
            if (s == "--license")
            {
                stdout.printf ("%s\n", "\n" + Text.app_info + "\n\n" + Text.app_license + "\n");
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
                stdout.printf ("%s\n", "Unknown option " + s + "\n");
                stdout.printf ("%s\n", Text.app_help);
                return 0;
            }
        }
        
        app = new Haguichi();
        return app.run();
    }
}
