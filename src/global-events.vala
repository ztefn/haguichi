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

public class GlobalEvents
{
    public static bool search_active;
    
    public static void set_modal_dialog (Dialog? dialog)
    {
        Haguichi.modal_dialog = dialog;
        Haguichi.session.modality_changed (dialog != null);
    }
    
    public static void start_hamachi ()
    {
        Controller.restore_countdown = 0;
        Controller.go_connect();
    }
    
    public static void stop_hamachi ()
    {
        new Thread<void*> (null, stop_hamachi_thread);
        
        Controller.restore = false;
        
        connection_stopped();
    }
    
    private static void* stop_hamachi_thread ()
    {
        if (Haguichi.demo_mode)
        {
            // Do nothing
        }
        else
        {
            Hamachi.logout();
        }
        
        return null;
    }
    
    public static void connection_established ()
    {
        Haguichi.window.set_mode ("Connected");
        
        connection_updated();
        
        string protocol = (string) Settings.protocol.val;
        
        if (Hamachi.ip_version.down() != protocol)
        {
            update_protocol (protocol);
        }
        
        new Thread<void*> (null, set_nick_after_login_thread);
        
        Controller.restore = (bool) Settings.reconnect_on_connection_loss.val;
        Controller.num_update_cycles ++;
        Controller.update_cycle();
    }
    
    private static void* set_nick_after_login_thread ()
    {
        Thread.usleep (2000000);
        Hamachi.set_nick ((string) Settings.nickname.val);
        
        return null;
    }
    
    public static void connection_updated ()
    {
        set_attach();
        Haguichi.window.update();
    }
    
    public static void connection_stopped ()
    {
        Haguichi.window.set_mode ("Disconnected");
        
        Controller.continue_update = false; // Stop update interval
        
        if (Controller.restore)
        {
            if (Controller.last_status == 2)
            {
                Controller.wait_for_internet_cycle();
            }
            else
            {
                Controller.restore_connection_cycle();
            }
            return;
        }
        
        if (!Haguichi.demo_mode)
        {
            Haguichi.connection.clear_networks();
        }
        Controller.last_status = 4;
        
        set_attach();
    }
    
    public static void about ()
    {
        Gtk.show_about_dialog (null,
                               "transient-for", Haguichi.window,
                               "program-name", Text.app_name,
                               "logo-icon-name", Text.app_name.down(),
                               "comments", Text.app_comments,
                               "version", Text.app_version,
                               "license", Text.app_license,
                               "copyright", Text.app_copyright,
                               "website", Text.app_website,
                               "website-label", Text.app_website_label,
                               "authors", Text.app_authors,
                               "translator-credits", Text.app_translator_credits,
                               "artists", Text.app_artists);
    }
    
    public static void preferences ()
    {
        Haguichi.preferences_dialog.open();
    }
    
    public static void start_search ()
    {
        HaguichiWindow.header_bar.search_but.active = true;
        HaguichiWindow.search_bar_revealer.set_reveal_child (true);
        HaguichiWindow.search_entry.grab_focus();
        
        search_active = true;
    }
    
    public static void stop_search ()
    {
        HaguichiWindow.header_bar.search_but.active = false;
        HaguichiWindow.search_bar_revealer.set_reveal_child (false);
        HaguichiWindow.search_entry.text = "";
        
        search_active = false;
    }
    
    public static void toggle_search ()
    {
        if (search_active) 
        {
            stop_search();
        }
        else
        {
            start_search();
        }
    }
    
    public static void set_nick (string nick)
    {
        Haguichi.window.set_nick (nick);
        HaguichiWindow.sidebar.set_nick (nick);
    }
    
    public static void update_nick ()
    {
        if (Haguichi.demo_mode)
        {
            set_nick ("Joe Demo");
        }
        else
        {
            set_nick ((string) Settings.nickname.val);
        }
    }
    
    public static void change_nick ()
    {
        Haguichi.window.present();
        new Dialogs.ChangeNick();
    }
    
    public static void update_protocol (string protocol)
    {
        if (Controller.last_status >= 6)
        {
            Haguichi.preferences_dialog.ip_combo.active = (int) Utils.protocol_to_int (protocol);
            new Thread<void*> (null, update_protocol_thread);
        }
    }
    
    private static void* update_protocol_thread ()
    {
        Hamachi.set_protocol ((string) Settings.protocol.val);
        return null;
    }
    
    public static void information ()
    {
        int width, height;
        
        Haguichi.window.get_size (out width, out height);
        
        width  -= Settings.decorator_offset;
        height -= Settings.decorator_offset;
        
        if (width <= Settings.switch_sidebar_threshold + 1)
        {
            width = Settings.switch_sidebar_threshold + 1;
            Haguichi.window.resize (width + Settings.decorator_offset, height + Settings.decorator_offset);
        }
        
        if (Haguichi.window.content_box.position > (width - (Settings.decorator_offset + Settings.switch_sidebar_threshold) / 2))
        {
            Haguichi.window.content_box.position = width - (Settings.decorator_offset + Settings.switch_sidebar_threshold) / 2;
        }
        
        HaguichiWindow.sidebar.show_tab ("Info");
    }
    
    public static void join_network ()
    {
        Haguichi.window.present();
        new Dialogs.JoinCreateNetwork ("Join", Text.join_network_title);
    }
    
    public static void create_network ()
    {
        Haguichi.window.present();
        new Dialogs.JoinCreateNetwork ("Create", Text.create_network_title);
    }
    
    public static void attach ()
    {
        Haguichi.window.present();
        new Dialogs.Attach();
    }
    
    public static void set_attach ()
    {
        string account = Hamachi.get_account();
        
        HaguichiWindow.sidebar.set_account (account);
        
        if (((account == "") ||
             (account == "-")) &&
            (Controller.last_status >= 6))
        {
            HaguichiWindow.sidebar.set_attach (true, true);
        }
        else if ((account == "") ||
                 (account == "-"))
        {
            HaguichiWindow.sidebar.set_attach (true, false);
        }
        else
        {
            HaguichiWindow.sidebar.set_attach (false, false);
        }
    }
    
    public static void set_config ()
    {
        if (FileUtils.test (Hamachi.data_path, GLib.FileTest.EXISTS))
        {
            GlobalActions.open_config.set_enabled (true);
            GlobalActions.save_config.set_enabled (true);
        }
        else
        {
            GlobalActions.open_config.set_enabled (false);
            GlobalActions.save_config.set_enabled (false);
        }
    }
    
    private static FileFilter get_file_filter ()
    {
        FileFilter tar = new FileFilter();
        tar.set_filter_name (Text.config_file_filter_title);
        tar.add_mime_type ("application/x-tar");
        tar.add_mime_type ("application/x-compressed-tar");
        tar.add_mime_type ("application/x-bzip-compressed-tar");
        tar.add_mime_type ("application/x-lzma-compressed-tar");
        tar.add_mime_type ("application/x-xz-compressed-tar");
        
        return tar;
    }
    
    public static void save_config ()
    {
        DateTime now = new DateTime.now_local();
        
        FileChooserDialog chooser = new FileChooserDialog (Text.config_save_title, Haguichi.window, FileChooserAction.SAVE, Text.cancel_label, ResponseType.CANCEL);
        
        Button ok_but = (Button) chooser.add_button (Text.save_label, ResponseType.OK);
        ok_but.get_style_context().add_class ("suggested-action");
        
        chooser.modal = true;
        chooser.do_overwrite_confirmation = true;
        chooser.add_filter (get_file_filter());
        chooser.set_current_folder (Environment.get_home_dir());
        chooser.set_current_name ("logmein-hamachi-config_" + now.format ("%Y-%m-%d") + ".tar.gz");
        chooser.show_all();
        
        chooser.response.connect ((response_id) =>
        {
            if (response_id == ResponseType.OK)
            {
                Hamachi.save_config (chooser.get_filename());
            }
            
            chooser.destroy();
        });
    }
    
    public static void restore_config ()
    {
        FileChooserDialog chooser = new FileChooserDialog (Text.config_restore_title, Haguichi.window, FileChooserAction.OPEN, Text.cancel_label, ResponseType.CANCEL);
        
        Button ok_but = (Button) chooser.add_button (Text.config_restore_button_label, ResponseType.OK);
        ok_but.get_style_context().add_class ("suggested-action");
        
        chooser.modal = true;
        chooser.add_filter (get_file_filter());
        chooser.set_current_folder (Environment.get_home_dir());
        chooser.show_all();
        
        chooser.response.connect ((response_id) =>
        {
            chooser.hide();
            
            if (response_id == ResponseType.OK)
            {
                Hamachi.restore_config (chooser.get_filename());
            }
            
            chooser.destroy();
        });
    }
    
    public static void open_config ()
    {
        Command.open_uri ("file://"+Hamachi.data_path);
    }
    
    public static void help ()
    {
        Command.open_uri (Text.help_url);
    }
    
    public static void donate ()
    {
        Command.open_uri (Text.donate_url);
    }
    
    public static void copy_ipv4_to_clipboard ()
    {
        string ipv4 = Hamachi.get_address()[0];
        Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (ipv4, -1);
    }
    
    public static void copy_ipv6_to_clipboard ()
    {
        string ipv6 = Hamachi.get_address()[1];
        Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (ipv6, -1);
    }
    
    public static void copy_client_id_to_clipboard ()
    {
        string id = Hamachi.get_client_id();
        Clipboard.get_for_display (Gdk.Display.get_default(), Gdk.SELECTION_CLIPBOARD).set_text (id, -1);
    }
    
    public static void quit_app ()
    {
        Haguichi.session.quitted();
        Haguichi.window.hide();
        
        if ((bool) Settings.disconnect_on_quit.val)
        {
            if (Controller.last_status > 4)
            {
                stop_hamachi();
            }
        }
        
        Debug.log (Debug.domain.INFO, "GlobalEvents.quit_app", "Quitting...");
        Haguichi.window.destroy();
    }
}
