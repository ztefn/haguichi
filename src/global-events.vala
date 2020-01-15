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

public class GlobalEvents
{
    public static bool search_active;
    public static bool attach_blocking;
    
    public static void set_modal_dialog (Object? dialog)
    {
        GlobalActions.preferences.set_enabled (dialog == null);
        GlobalActions.shortcuts.set_enabled (dialog == null);
        GlobalActions.about.set_enabled (dialog == null);
        
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
        new Dialogs.About();
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
        
        HaguichiWindow.sidebar.show_tab ("Info", false);
    }
    
    public static void join_network ()
    {
        new Dialogs.JoinCreateNetwork ("Join", Text.join_network_title);
    }
    
    public static void create_network ()
    {
        new Dialogs.JoinCreateNetwork ("Create", Text.create_network_title);
    }
    
    public static void attach ()
    {
        new Dialogs.Attach();
    }
    
    public static void set_attach ()
    {
        if (!attach_blocking)
        {
            set_attach_with_account (Hamachi.get_account());
        }
    }
    
    public static void set_attach_with_account (string account)
    {
        if (Haguichi.demo_mode)
        {
            Hamachi.demo_account = account;
        }
        
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
    
    public static void cancel_attach ()
    {
        set_attach_with_account ("");
        new Thread<void*> (null, cancel_attach_thread);
    }
    
    private static void* cancel_attach_thread ()
    {
        attach_blocking = true;
        
        Hamachi.cancel();
        
        Thread.usleep (2000000); // Wait two seconds to get updated info
        Hamachi.get_info();
        
        attach_blocking = false;
        
        return null;
    }
    
    public static void set_config ()
    {
        if (Utils.path_exists ("d", Hamachi.data_path))
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
        
#if GTK_3_20
        FileChooserNative chooser = new FileChooserNative (Text.config_save_title,
                                                           Haguichi.window,
                                                           FileChooserAction.SAVE,
                                                           Text.save_label,
                                                           Text.cancel_label);
#else
        FileChooserDialog chooser = new FileChooserDialog (Text.config_save_title,
                                                           Haguichi.window,
                                                           FileChooserAction.SAVE,
                                                           Text.cancel_label, ResponseType.CANCEL);
        
        Button save_but = (Button) chooser.add_button (Text.save_label, ResponseType.ACCEPT);
        save_but.get_style_context().add_class ("suggested-action");
#endif
        
        set_modal_dialog (chooser);
        
        chooser.modal = true;
        chooser.do_overwrite_confirmation = true;
        chooser.add_filter (get_file_filter());
        chooser.set_current_folder (Environment.get_home_dir());
        chooser.set_current_name ("logmein-hamachi-config_" + now.format ("%Y-%m-%d") + ".tar.gz");
        chooser.show();
        
        chooser.response.connect ((response_id) =>
        {
            if (response_id == ResponseType.ACCEPT)
            {
                Debug.log (Debug.domain.INFO, "GlobalEvents.save_config", "Saving hamachi configuration backup to " + chooser.get_filename());
                Hamachi.save_config (chooser.get_filename());
            }
            
            set_modal_dialog (null);
            chooser.destroy();
        });
    }
    
    public static void restore_config ()
    {
#if GTK_3_20
        FileChooserNative chooser = new FileChooserNative (Text.config_restore_title,
                                                           Haguichi.window,
                                                           FileChooserAction.OPEN,
                                                           Text.config_restore_button_label,
                                                           Text.cancel_label);
#else
        FileChooserDialog chooser = new FileChooserDialog (Text.config_restore_title,
                                                           Haguichi.window,
                                                           FileChooserAction.OPEN,
                                                           Text.cancel_label, ResponseType.CANCEL);
        
        Button restore_but = (Button) chooser.add_button (Text.config_restore_button_label, ResponseType.ACCEPT);
        restore_but.get_style_context().add_class ("suggested-action");
#endif
        
        set_modal_dialog (chooser);
        
        chooser.modal = true;
        chooser.add_filter (get_file_filter());
        chooser.set_current_folder (Environment.get_home_dir());
        chooser.show();
        
        chooser.response.connect ((response_id) =>
        {
            if (response_id == ResponseType.ACCEPT)
            {
                Debug.log (Debug.domain.INFO, "GlobalEvents.restore_config", "Restoring hamachi configuration backup from " + chooser.get_filename());
                Hamachi.restore_config (chooser.get_filename());
            }
            
            set_modal_dialog (null);
            chooser.destroy();
        });
    }
    
    public static void open_config ()
    {
        Command.open_uri ("file://" + Hamachi.data_path);
    }
    
    public static void help ()
    {
        Command.open_uri (Text.help_url);
    }
    
    public static void donate ()
    {
        Command.open_uri (Text.donate_url);
    }
    
    public static void shortcuts ()
    {
#if GTK_3_20
        var builder = new Builder.from_resource ("/com/github/ztefn/haguichi/ui/shortcuts-window.ui");
        var shortcuts_window = (ShortcutsWindow) builder.get_object ("shortcuts-window");
        shortcuts_window.delete_event.connect ((event) =>
        {
            set_modal_dialog (null);
            return false;
        });
        shortcuts_window.set_transient_for (Haguichi.window);
        shortcuts_window.show_all();
        
        set_modal_dialog (shortcuts_window);
#else
        Command.open_uri (Text.shortcuts_url);
#endif
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
        
        if (Haguichi.modal_dialog != null)
        {
            if (Haguichi.modal_dialog is Window)
            {
                ((Window) Haguichi.modal_dialog).destroy();
            }
            set_modal_dialog (null);
        }
        
        Haguichi.window.hide();
        Haguichi.connection.save_long_nicks();
        
        if ((bool) Settings.disconnect_on_quit.val)
        {
            if (Controller.last_status > 4)
            {
                stop_hamachi();
            }
        }
        
        Notify.uninit();
        
        Debug.log (Debug.domain.INFO, "GlobalEvents.quit_app", "Quitting...");
        Haguichi.window.destroy();
    }
}
