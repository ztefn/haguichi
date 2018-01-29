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
using Widgets;

namespace Dialogs
{
    public class Preferences : Dialog
    {
        public  Switch connect_on_startup_switch;
        public  Switch reconnect_on_connection_loss_switch;
        public  Switch disconnect_on_quit_switch;
        public  Switch update_network_list_switch;
        
        public  Switch prefer_dark_theme_switch;
        public  Switch show_indicator_switch;
        public  Switch notify_connection_loss_switch;
        public  Switch notify_member_join_switch;
        public  Switch notify_member_leave_switch;
        public  Switch notify_member_online_switch;
        public  Switch notify_member_offline_switch;
        
        public  ComboBoxText ip_combo;
        public  SpinButton interval_spin;
        
        private Label connect_on_startup_label;
        private Label reconnect_on_connection_loss_label;
        private Label disconnect_on_quit_label;
        private Label update_network_list_label;
        private Label interval_label;
        
        private Label prefer_dark_theme_label;
        private Label show_indicator_label;
        private Label notify_connection_loss_label;
        private Label notify_member_join_label;
        private Label notify_member_leave_label;
        private Label notify_member_online_label;
        private Label notify_member_offline_label;
        
        private PreferencesBox appearance_box;
        
        public Preferences ()
        {
            Object (title: Text.preferences_title,
                    transient_for: Haguichi.window,
#if FOR_ELEMENTARY
                    deletable: false,
#endif
                    modal: false,
                    resizable: false,
                    use_header_bar: (int) Haguichi.dialog_use_header_bar);
            
            delete_event.connect (on_delete);
            
            
            prefer_dark_theme_switch      = new PreferencesSwitch (Settings.prefer_dark_theme);
            show_indicator_switch         = new PreferencesSwitch (Settings.show_indicator);
            notify_connection_loss_switch = new PreferencesSwitch (Settings.notify_on_connection_loss);
            notify_member_join_switch     = new PreferencesSwitch (Settings.notify_on_member_join);
            notify_member_leave_switch    = new PreferencesSwitch (Settings.notify_on_member_leave);
            notify_member_online_switch   = new PreferencesSwitch (Settings.notify_on_member_online);
            notify_member_offline_switch  = new PreferencesSwitch (Settings.notify_on_member_offline);
            
            prefer_dark_theme_label       = new PreferencesLabel (Text.prefer_dark_theme, prefer_dark_theme_switch);
            show_indicator_label          = new PreferencesLabel (Text.show_indicator, show_indicator_switch);
            notify_connection_loss_label  = new PreferencesLabel (Text.notify_on_connection_lost, notify_connection_loss_switch);
            notify_member_join_label      = new PreferencesLabel (Text.notify_on_member_join, notify_member_join_switch);
            notify_member_leave_label     = new PreferencesLabel (Text.notify_on_member_leave, notify_member_leave_switch);
            notify_member_online_label    = new PreferencesLabel (Text.notify_on_member_online, notify_member_online_switch);
            notify_member_offline_label   = new PreferencesLabel (Text.notify_on_member_offline, notify_member_offline_switch);
            
            appearance_box = new PreferencesBox (Text.appearance_group);
            if (Haguichi.window_use_header_bar)
            {
                appearance_box.add_row (prefer_dark_theme_label, prefer_dark_theme_switch, 1);
            }
#if ENABLE_APPINDICATOR
            appearance_box.add_row (show_indicator_label, show_indicator_switch, 2);
#endif
            
            var notify_box = new PreferencesBox (Text.notify_group);
            notify_box.add_row (notify_connection_loss_label, notify_connection_loss_switch, 1);
            notify_box.add_row (notify_member_join_label,     notify_member_join_switch,     2);
            notify_box.add_row (notify_member_leave_label,    notify_member_leave_switch,    3);
            notify_box.add_row (notify_member_online_label,   notify_member_online_switch,   4);
            notify_box.add_row (notify_member_offline_label,  notify_member_offline_switch,  5);
            
            var desktop_box = new Box (Orientation.VERTICAL, 0);
            desktop_box.margin_bottom = 3;
            desktop_box.pack_start (appearance_box, false, false, 0);
            desktop_box.pack_start (notify_box,     false, false, 0);
            
            
            ip_combo = new ComboBoxText();
            ip_combo.append_text (Text.protocol_both);
            ip_combo.append_text (Text.protocol_ipv4);
            ip_combo.append_text (Text.protocol_ipv6);
            ip_combo.active = (int) Utils.protocol_to_int ((string) Settings.protocol.val);
            ip_combo.changed.connect (() =>
            {
                Settings.protocol.val = Utils.protocol_to_string (ip_combo.active);
            });
            
            var ip_label = new PreferencesLabel (Text.protocol_label, ip_combo);
            
            var hamachi_box = new PreferencesBox ("Hamachi");
            hamachi_box.add_row (ip_label, ip_combo, 1);
            
            connect_on_startup_switch           = new PreferencesSwitch (Settings.connect_on_startup);
            reconnect_on_connection_loss_switch = new PreferencesSwitch (Settings.reconnect_on_connection_loss);
            disconnect_on_quit_switch           = new PreferencesSwitch (Settings.disconnect_on_quit);
            update_network_list_switch          = new PreferencesSwitch (Settings.update_network_list);
            update_network_list_switch.state_set.connect ((state) =>
            {
                interval_spin.sensitive = state;
                return false;
            });
            
            interval_spin = new SpinButton.with_range (0, 999, 1);
            interval_spin.sensitive = (bool) Settings.update_network_list.val;
            interval_spin.value = (int) Settings.update_interval.val;
            interval_spin.value_changed.connect (() =>
            {
                Settings.update_interval.val = (int) interval_spin.value;
            });
            interval_spin.max_length = 3;
            
            connect_on_startup_label           = new PreferencesLabel (Text.connect_on_startup, connect_on_startup_switch);
            reconnect_on_connection_loss_label = new PreferencesLabel (Text.reconnect_on_connection_loss, reconnect_on_connection_loss_switch);
            disconnect_on_quit_label           = new PreferencesLabel (Text.disconnect_on_quit, disconnect_on_quit_switch);
            update_network_list_label          = new PreferencesLabel (null, update_network_list_switch);
            interval_label                     = new PreferencesLabel (null, interval_spin);
            
            var interval_box = new Box (Orientation.HORIZONTAL, 0);
            interval_box.pack_start (update_network_list_label, false, false, 0);
            interval_box.pack_start (interval_spin,             false, false, 0);
            interval_box.pack_start (interval_label,            false, false, 0);
            
            var behavior_box = new PreferencesBox (Text.behavior_group);
            behavior_box.add_row (connect_on_startup_label,           connect_on_startup_switch,           1);
            behavior_box.add_row (reconnect_on_connection_loss_label, reconnect_on_connection_loss_switch, 2);
            behavior_box.add_row (disconnect_on_quit_label,           disconnect_on_quit_switch,           3);
            behavior_box.add_row (interval_box,                       update_network_list_switch,          4);
            
            var general_box = new Box (Orientation.VERTICAL, 0);
            general_box.margin_bottom = 3;
            general_box.pack_start (hamachi_box,  false, false, 0);
            general_box.pack_start (behavior_box, false, false, 0);
            
            
            var container = new Stack();
            container.expand = true;
            container.add_titled (general_box,          "general",  Text.general_tab);
            container.add_titled (new CommandsEditor(), "commands", Text.commands_tab);
            container.add_titled (desktop_box,          "desktop",  Text.desktop_tab);
            
            var switcher = new StackSwitcher();
            switcher.stack = container;
            
            if (Haguichi.dialog_use_header_bar)
            {
                var titlebar = new HeaderBar();
                titlebar.custom_title = switcher;
                titlebar.show_close_button = true;
                titlebar.show_all();
                
                set_titlebar (titlebar);
                
                container.transition_type = StackTransitionType.SLIDE_LEFT_RIGHT;
            }
            else
            {
                switcher.halign = Align.CENTER;
#if !FOR_ELEMENTARY
                switcher.margin_top = 12;
#endif
                get_content_area().add (switcher);
                
                get_action_area().margin = 6;
                get_action_area().margin_top = 0;
                
                add_button (Text.close_label, ResponseType.CLOSE);
                
                response.connect ((response_id) =>
                {
                    if (response_id == ResponseType.CLOSE)
                    {
                        hide();
                    }
                });
            }
            
            get_content_area().add (container);
            get_content_area().show_all();
            
            // Only show appearance box if there are any rows present
            appearance_box.visible = (appearance_box.num_rows > 0);
            
            get_style_context().add_class ("preferences");
            
            set_interval_string();
        }
        
        public void set_interval_string ()
        {
            string[] interval_string = Text.update_network_list_interval ((int) interval_spin.value).split ("%S", 2);
            
            update_network_list_label.label = interval_string[0] + " ";
            interval_label.label            = " " + interval_string[1];
        }
        
        public void open ()
        {
            show();
            present();
        }
        
        private bool on_delete ()
        {
            hide();
            return true;
        }
    }
}
