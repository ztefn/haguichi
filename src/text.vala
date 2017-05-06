/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Text : Object
{
    public const  string app_name                  = "Haguichi";
    public const  string app_version               = "1.3.8";
    public const  string app_website               = "https://www.haguichi.net/";
    public const  string app_website_label         = "www.haguichi.net";
    public static string app_comments;
    public static string app_description;
    public const  string app_copyright             = "Copyright © 2007–2017 Stephen Brandt";
    public const  string app_license               =
@"Haguichi is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published
by the Free Software Foundation, either version 3 of the License,
or (at your option) any later version.

Haguichi is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Haguichi. If not, see <http://www.gnu.org/licenses/>.";
    
    public const  string app_info                  =
@"Haguichi, a graphical frontend for Hamachi.
Copyright © 2007–2017 Stephen Brandt <stephen@stephenbrandt.com>";
            
    public const  string app_help                  =
@"Usage:
  haguichi [options]

Options:
  -h, --help              Show this help and exit
  -v, --version           Show version number and exit
  --license               Show license and exit
  -d, --debug             Print debug messages
  --hidden                Start with hidden window
  --demo                  Run in demo mode
  --list=FILE             Use a text file as list in demo mode

" + app_info + "\n";
    
    public static string[] app_authors;
    public const  string   app_translator_credits  =
@"Asturianu (ast)
    Tornes Llume https://launchpad.net/~l-lumex03-tornes

Български (bg)
    Dimitar Dimitrov https://launchpad.net/~dimitrov

Čeština (cs)
    Jan Bažant https://launchpad.net/~brbla
    Zdeněk Kopš https://launchpad.net/~zdenekkops

Deutsch (de)
    Jannik Heller https://launchpad.net/~scrawl

Español (es)
    Adolfo Jayme https://launchpad.net/~fitoschido
    Eduardo Parra https://launchpad.net/~soker

Français (fr)
    Gabriel U. https://launchpad.net/~gabriel-ull
    Emilien Klein https://launchpad.net/~emilien-klein

Bahasa Indonesia (id)
    Fattah Rizki https://launchpad.net/~galamarv
    Hertatijanto Hartono https://launchpad.net/~dvertx

Italiano (it)
    Enrico Grassi https://launchpad.net/~enricog

日本語 (ja)
    Satoru Matsumoto https://launchpad.net/~helios-reds

Қазақ (kk)
    jmb_kz https://launchpad.net/~jmb-kz

lietuvių kalba (lt)
    Moo https://launchpad.net/~mooo

Nederlands (nl)
    Stephen Brandt https://launchpad.net/~ztefn

Polski (pl)
    Antoni Sperka https://launchpad.net/~antek1004-gmail
    Piotr Drąg https://launchpad.net/~raven46

Português do Brasil (pt-BR)
    Rodrigo de Avila https://launchpad.net/~rodrigo-avila

Русский (ru)
    jmb_kz https://launchpad.net/~jmb-kz
    Sergey Korolev https://launchpad.net/~slipped-on-blade
    Sergey Sedov https://launchpad.net/~serg-sedov

Slovenčina (sk)
    Zdeněk Kopš https://launchpad.net/~zdenekkops

Svenska (sv)
    Daniel Holm https://launchpad.net/~danielholm
    Daniel Nylander https://launchpad.net/~yeager
    David Bengtsson https://launchpad.net/~justfaking

Türkçe (tr)
    Aytunç Yeni https://launchpad.net/~aytuncyeni
    Kudret Emre https://launchpad.net/~kudretemre

Українська (uk)
    Fedir Zinchuk https://launchpad.net/~fedikw
";
    
    public const  string help_url                  = "https://www.haguichi.net/redirect/?version=" + app_version + "&action=help";
    public const  string get_hamachi_url           = "https://www.haguichi.net/redirect/?version=" + app_version + "&action=get-hamachi";
    public const  string donate_url                = "https://www.haguichi.net/redirect/?version=" + app_version + "&action=donate";
    
    public static string ok_label;
    public static string cancel_label;
    public static string save_label;
    public static string refresh_label;
    public static string revert_label;
    public static string delete_label;
    public static string information_label;
    public static string donate_label;
    public static string preferences_label;
    public static string help_label;
    public static string about_label;
    public static string quit_label;
    
    public static string show_app;
    
    public static string yes;
    public static string no;
    
    public static string hamachi_output;
    
    public static string address_ipv4;
    public static string address_ipv6;
    public static string client_id;
    public static string status;
    public static string tunnel;
    public static string connection;
    public static string direct;
    public static string relayed;
    public static string anonymous;
    public static string network_id_label;
    public static string network_id;
    public static string password_label;
    public static string nick_label;
    public static string nick;
    
    public static string offline;
    public static string online;
    public static string unreachable;
    public static string mismatch;
    public static string conflict;
    public static string connect_label;
    public static string disconnect_label;
    public static string reconnect_label;
    public static string connected;
    public static string disconnected;
    public static string connecting;
    public static string logging_in;
    public static string updating;
    
    public static string not_configured_heading;
    public static string not_configured_message;
    public static string configure_label;
    
    public static string not_installed_heading;
    public static string not_installed_message;
    public static string obsolete_heading;
    public static string obsolete_message;
    public static string download_label;
    
    public static string empty_list_heading;
    public static string empty_list_message;
    
    public static string members;
    public static string member_count;
    public static string owner;
    public static string you;
    public static string unknown;
    public static string unavailable;
    public static string unapproved;
    public static string locked;
    public static string approval;
    public static string manually;
    public static string automatically;
    public static string capacity;
    
    public static string go_online_label;
    public static string go_offline_label;
    public static string leave_label;
    public static string locked_label;
    public static string approval_label;
    public static string auto_label;
    public static string manual_label;
    public static string copy_network_id_label;
    
    public static string browse_label;
    public static string ping_label;
    public static string vnc_label;
    public static string approve_label;
    public static string reject_label;
    public static string evict_label;
    public static string copy_address_ipv4_label;
    public static string copy_address_ipv6_label;
    public static string copy_client_id_label;
    
    public static string change_nick_label;
    public static string change_nick_title;
    public static string change_label;
    
    public static string change_password_label;
    public static string change_password_title;
    public static string new_password_label;
    
    public static string account_label;
    public static string account;
    public static string pending;
    
    public static string attach_label;
    public static string attach_title;
    public static string attach_with_networks_checkbox;
    public static string attach_button_label;
    public static string attach_error_account_not_found;
    
    public static string config_label;
    public static string config_folder_label;
    public static string config_save_label;
    public static string config_save_title;
    public static string config_restore_label;
    public static string config_restore_title;
    public static string config_restore_button_label;
    public static string config_restore_error_title;
    public static string config_restore_error_message;
    public static string config_file_filter_title;
    
    public static string join_network_label;
    public static string join_network_title;
    public static string join_label;
    
    public static string create_network_label;
    public static string create_network_title;
    public static string create_label;
    
    public static string error_network_not_found;
    public static string error_invalid_password;
    public static string error_network_full;
    public static string error_network_locked;
    public static string error_network_already_joined;
    public static string error_network_id_taken;
    public static string error_unknown;
    
    public static string request_sent_message;
    
    public static string preferences_title;
    public static string information_title;
    
    public static string confirm_delete_network_heading;
    public static string confirm_delete_network_message;
    
    public static string confirm_leave_network_heading;
    public static string confirm_leave_network_message;
    
    public static string confirm_evict_member_heading;
    public static string confirm_evict_member_message;
    
    public static string failed_delete_network_heading;
    public static string failed_leave_network_heading;
    public static string failed_evict_member_heading;
    public static string failed_go_online_heading;
    public static string failed_go_offline_heading;
    
    public static string see_output;
    
    public static string general_tab;
    public static string commands_tab;
    public static string desktop_tab;
    
    public static string notify_group;
    public static string behavior_group;
    
    public static string protocol_label;
    public static string protocol_both;
    public static string protocol_ipv4;
    public static string protocol_ipv6;
    
    public static string checkbox_show_offline_members;
    public static string sort_by_name;
    public static string sort_by_status;
    
    public static string connect_on_startup;
    public static string reconnect_on_connection_loss;
    public static string disconnect_on_quit;
    
    public static string checkbox_notify_connection_lost;
    public static string checkbox_notify_member_join;
    public static string checkbox_notify_member_leave;
    public static string checkbox_notify_member_online;
    public static string checkbox_notify_member_offline;
    
    public static string connect_error_heading;
    public static string connect_error_login_failed;
    public static string connect_error_no_internet_connection;
    
    public static string notify_connection_lost;
    public static string notify_member_online_heading;
    public static string notify_member_online_message;
    public static string notify_member_offline_heading;
    public static string notify_member_offline_message;
    public static string notify_member_joined_heading;
    public static string notify_member_joined_message;
    public static string notify_member_left_heading;
    public static string notify_member_left_message;
    
    public static string add_tip;
    public static string add_command_title;
    public static string add_label;
    public static string edit_tip;
    public static string edit_command_title;
    public static string label_label;
    public static string command_ipv4_label;
    public static string command_ipv6_label;
    public static string command_hint;
    public static string priority_label;
    
    public static string remove_tip;
    public static string move_up_tip;
    public static string move_down_tip;
    public static string default_tip;
    
    public static string revert_tip;
    public static string revert_heading;
    public static string revert_message;
    
    public static void init ()
    {
        app_comments                               = _("A graphical frontend for Hamachi.");
        app_description                            = _("Join and create local networks over the Internet");
        
        app_authors                                = {"Stephen Brandt https://launchpad.net/~ztefn", ""};
        
        ok_label                                   = _("_OK");
        cancel_label                               = _("_Cancel");
        save_label                                 = _("_Save");
        refresh_label                              = _("_Refresh");
        revert_label                               = _("_Revert");
        delete_label                               = _("_Delete");
        information_label                          = _("_Information");
        donate_label                               = _("_Donate");
        preferences_label                          = _("_Preferences");
        help_label                                 = _("_Help");
        about_label                                = _("_About");
        quit_label                                 = _("_Quit");
        
        show_app                                   = _("_Show Haguichi"); // This string is only used in indicators
        
        yes                                        = _("Yes");
        no                                         = _("No");
        
        hamachi_output                             = _("_Hamachi output");
        
        address_ipv4                               = _("IPv4 address:");
        address_ipv6                               = _("IPv6 address:");
        client_id                                  = _("Client ID:");
        status                                     = _("Status:");
        tunnel                                     = _("Tunnel:");
        connection                                 = _("Connection:");
        direct                                     = _("Direct");
        relayed                                    = _("Relayed");
        anonymous                                  = _("Anonymous");
        network_id_label                           = _("_Network ID:");
        network_id                                 = Utils.remove_mnemonics (network_id_label); // "Network ID:"
        password_label                             = _("_Password:");
        nick_label                                 = _("_Nickname:");
        nick                                       = Utils.remove_mnemonics (nick_label); // "Nickname:"
        
        offline                                    = _("Offline");
        online                                     = _("Online");
        unreachable                                = _("Unreachable");
        mismatch                                   = _("Protocol mismatch");
        conflict                                   = _("Conflicting address");
        connect_label                              = _("C_onnect"); // This string is only used in indicators
        disconnect_label                           = _("_Disconnect"); // This string is only used in indicators
        reconnect_label                            = _("Reconnect");
        connected                                  = _("Connected");
        disconnected                               = _("Disconnected");
        connecting                                 = _("Connecting…");
        logging_in                                 = _("Logging in…");
        updating                                   = _("Updating…");
        
        not_configured_heading                     = _("Hamachi is not configured");
        not_configured_message                     = _("You need to configure Hamachi before you can connect.");
        configure_label                            = _("C_onfigure");

        not_installed_heading                      = _("Hamachi is not installed");
        not_installed_message                      = _("Please download Hamachi and follow the installation instructions.");
        obsolete_heading                           = _("Hamachi version {0} is obsolete");
        obsolete_message                           = _("Please download and install the latest Hamachi version.");
        download_label                             = _("_Download");
        
        empty_list_heading                         = _("You are connected!");
        empty_list_message                         = _("Click on the + button to add a network.");
        
        members                                    = _("Members:");
        member_count                               = _("{0} online, {1} total");
        owner                                      = _("Owner:");
        you                                        = _("You");
        unknown                                    = _("Unknown");
        unavailable                                = _("Unavailable");
        unapproved                                 = _("Awaiting approval");
        locked                                     = _("Locked:");
        approval                                   = _("Approval:");
        manually                                   = _("Manually");
        automatically                              = _("Automatically");
        capacity                                   = _("Capacity:");
        
        go_online_label                            = _("_Go Online");
        go_offline_label                           = _("_Go Offline");
        leave_label                                = _("_Leave");
        locked_label                               = _("_Locked");
        approval_label                             = _("_New Member Approval");
        auto_label                                 = _("_Automatically");
        manual_label                               = _("_Manually");
        copy_network_id_label                      = _("_Copy Network ID");
        
        browse_label                               = _("_Browse Shares");
        ping_label                                 = _("_Ping");
        vnc_label                                  = _("_View Remote Desktop");
        approve_label                              = _("_Approve");
        reject_label                               = _("_Reject");
        evict_label                                = _("_Evict");
        copy_address_ipv4_label                    = _("Copy IPv_4 Address");
        copy_address_ipv6_label                    = _("Copy IPv_6 Address");
        copy_client_id_label                       = _("_Copy Client ID");
        
        change_nick_label                          = _("Change _Nickname…");
        change_nick_title                          = _("Change Nickname");
        change_label                               = _("C_hange");
        
        change_password_label                      = _("Change _Password…");
        change_password_title                      = _("Change Password");
        new_password_label                         = _("_New password:");
        
        account_label                              = _("_Account:");
        account                                    = Utils.remove_mnemonics (account_label); // "Account:"
        pending                                    = _("Pending");
        
        attach_label                               = _("_Attach to Account…");
        attach_title                               = _("Attach to Account");
        attach_with_networks_checkbox              = _("_Include all networks created by this client");
        attach_button_label                        = _("_Attach");
        attach_error_account_not_found             = _("Account not found");
        
        config_label                               = _("Config_uration");
        config_folder_label                        = _("_Open Folder");
        config_save_label                          = _("_Save a Backup…");
        config_save_title                          = _("Save a Backup");
        config_restore_label                       = _("_Restore from Backup…");
        config_restore_title                       = _("Restore from Backup");
        config_restore_button_label                = _("_Restore");
        config_restore_error_title                 = _("Could not restore configuration");
        config_restore_error_message               = _("Archive does not contain expected files.");
        config_file_filter_title                   = _("All supported archives");
        
        join_network_label                         = _("_Join Network…");
        join_network_title                         = _("Join Network");
        join_label                                 = _("_Join");
        
        create_network_label                       = _("_Create Network…");
        create_network_title                       = _("Create Network");
        create_label                               = _("C_reate");
        
        error_network_not_found                    = _("Network not found");
        error_invalid_password                     = _("Invalid password");
        error_network_full                         = _("Network is full");
        error_network_locked                       = _("Network is locked");
        error_network_already_joined               = _("Network already joined");
        error_network_id_taken                     = _("Network ID is already taken");
        error_unknown                              = _("Unknown error");
        
        request_sent_message                       = _("Join request sent");
        
        preferences_title                          = _("Preferences");
        information_title                          = _("Information");
        
        confirm_delete_network_heading             = _("Are you sure you want to delete the network “{0}”?");
        confirm_delete_network_message             = _("If you delete a network, it will be permanently lost.");
        
        confirm_leave_network_heading              = _("Are you sure you want to leave the network “{0}”?");
        confirm_leave_network_message              = _("If admitted, you can rejoin the network at any later time.");
        
        confirm_evict_member_heading               = _("Are you sure you want to evict member “{0}” from network “{1}”?");
        confirm_evict_member_message               = _("If admitted, evicted members can rejoin the network at any later time.");
        
        failed_delete_network_heading              = _("Could not delete network “{0}”");
        failed_leave_network_heading               = _("Could not leave network “{0}”");
        failed_evict_member_heading                = _("Could not evict member “{0}”");
        failed_go_online_heading                   = _("Could not go online in the network “{0}”");
        failed_go_offline_heading                  = _("Could not go offline in the network “{0}”");
        
        see_output                                 = _("See output for details.");
        
        general_tab                                = _("General");
        desktop_tab                                = _("Desktop");
        commands_tab                               = _("Commands");
        
        notify_group                               = _("Notifications");
        behavior_group                             = _("Behavior");
        
        protocol_label                             = _("_Protocol:");
        protocol_both                              = _("Both IPv4 and IPv6");
        protocol_ipv4                              = _("IPv4 only");
        protocol_ipv6                              = _("IPv6 only");
        
        checkbox_show_offline_members              = _("Show _Offline Members");
        sort_by_name                               = _("Sort by _Name");
        sort_by_status                             = _("Sort by _Status");
        
        connect_on_startup                         = _("C_onnect automatically on startup");
        reconnect_on_connection_loss               = _("_Reconnect automatically when the connection is lost");
        disconnect_on_quit                         = _("_Disconnect on quit");
        
        checkbox_notify_connection_lost            = _("Display notification when the connection is l_ost");
        checkbox_notify_member_join                = _("Display notification when a member _joins");
        checkbox_notify_member_leave               = _("Display notification when a member _leaves");
        checkbox_notify_member_online              = _("Display notification when a member comes o_nline");
        checkbox_notify_member_offline             = _("Display notification when a member goes o_ffline");
        
        connect_error_heading                      = _("Error connecting");
        connect_error_login_failed                 = _("Hamachi login failed");
        connect_error_no_internet_connection       = _("No internet connection");
        
        notify_connection_lost                     = _("Hamachi lost connection");
        notify_member_online_heading               = _("Member online");
        notify_member_online_message               = _("{0} came online in the network {1}");
        notify_member_offline_heading              = _("Member offline");
        notify_member_offline_message              = _("{0} went offline in the network {1}");
        notify_member_joined_heading               = _("Member joined");
        notify_member_joined_message               = _("{0} joined the network {1}");
        notify_member_left_heading                 = _("Member left");
        notify_member_left_message                 = _("{0} left the network {1}");
        
        add_tip                                    = _("Add");
        add_command_title                          = _("Add Command");
        add_label                                  = _("_Add");
        edit_tip                                   = _("Edit");
        edit_command_title                         = _("Edit Command");
        label_label                                = _("_Label:");
        command_ipv4_label                         = _("IPv_4 command:");
        command_ipv6_label                         = _("IPv_6 command:");
        command_hint                               = _("Use %A for address and %N for nickname");
        priority_label                             = _("_Priority:");
        
        remove_tip                                 = _("Remove");
        move_up_tip                                = _("Move up");
        move_down_tip                              = _("Move down");
        default_tip                                = _("Set as default action");
        
        revert_tip                                 = _("Revert all changes");
        revert_heading                             = _("Are you sure you want to revert all changes?");
        revert_message                             = _("If you revert all changes, the default commands will be restored.");
    }
    
    public static string reconnecting (int count)
    {
        return ngettext ("Reconnecting in {0} second", "Reconnecting in {0} seconds", count);
    }
    
    public static string update_network_list_interval (int count)
    {
        return ngettext ("_Update the network list every %S _second", "_Update the network list every %S _seconds", count);
    }
    
    public static string notify_member_online_message_plural (int count)
    {
        return ngettext ("{0} came online in the network {1} and {2} other network", "{0} came online in the network {1} and {2} other networks", count);
    }
    
    public static string notify_member_offline_message_plural (int count)
    {
        return ngettext ("{0} went offline in the network {1} and {2} other network", "{0} went offline in the network {1} and {2} other networks", count);
    }
    
    public static string notify_member_joined_message_plural (int count)
    {
        return ngettext ("{0} joined the network {1} and {2} other network", "{0} joined the network {1} and {2} other networks", count);
    }
    
    public static string notify_member_left_message_plural (int count)
    {
        return ngettext ("{0} left the network {1} and {2} other network", "{0} left the network {1} and {2} other networks", count);
    }
}
