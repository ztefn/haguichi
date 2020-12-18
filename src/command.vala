/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Command : Object
{
    public static string spawn_wrap;
    
    public static string sudo;
    public static string sudo_args;
    public static string sudo_start;
    
    public static string terminal;
    public static string file_manager;
    public static string remote_desktop;
    
    public static void init ()
    {
        determine_sudo();
        determine_terminal();
        determine_file_manager();
        determine_remote_desktop();
    }
    
    public static void execute (string command)
    {
        if (command == "")
        {
            return;
        }
        
        try
        {
            GLib.Process.spawn_command_line_async (spawn_wrap + command);
        }
        catch (SpawnError e)
        {
            Debug.log (Debug.domain.ERROR, "Command.execute", e.message);
        }
    }
    
    public static string return_output (string command)
    {
        string output;
        string error;
        int    exit_status;
        
        try
        {
            GLib.Process.spawn_command_line_sync (spawn_wrap + command, out output, out error, out exit_status);
            
            if (error != "")
            {
                Debug.log (Debug.domain.ERROR, "Command.return_output", "stderr: " + error);
            }
        }
        catch (SpawnError e)
        {
            error = e.message;
            Debug.log (Debug.domain.ERROR, "Command.return_output", error);
        }
        
        // We don't like NULL strings
        if (output == null)
        {
            output = "";
        }
        
        // When hamachi is busy try again after a little while
        if (output.contains (".. failed, busy"))
        {
            Debug.log (Debug.domain.HAMACHI, "Command.return_output", "Hamachi is busy, waiting to try again...");
            Thread.usleep (100000);
            
            output = return_output (command);
        }
        
        // When there's no regular output we'd want to return the error if available
        if ((output == "") && (error != ""))
        {
            return error;
        }
        else
        {
            return output;
        }
    }
    
    public static bool exists (string? command)
    {
        if (command == null)
        {
            return false;
        }
        
        string output = return_output ("bash -c \"command -v " + command + " &>/dev/null || echo 'command not found'\"");
        
        if (output.contains ("command not found"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    public static bool custom_exists (string command_ipv4, string command_ipv6)
    {
        if ((exists (replace_variables (command_ipv4, "", "", "").split (" ", 0)[0])) ||
            (exists (replace_variables (command_ipv6, "", "", "").split (" ", 0)[0])))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public static void determine_sudo ()
    {
        sudo       = (string) Settings.super_user.val;
        sudo_args  = "";
        sudo_start = "-- ";
        
        if (sudo == "auto")
        {
            sudo = get_available ({
                "pkexec",
                "gksudo",
                "gksu",
                "gnomesu",
                "kdesudo",
                "kdesu",
                "sudo"
            });
        }
        
        if (sudo == "pkexec")
        {
            sudo_start = "";
        }
        else if (sudo.has_prefix ("gksu"))
        {
            sudo_args = "--sudo-mode -D \"" + Text.app_name + "\" ";
        }
        
        Debug.log (Debug.domain.ENVIRONMENT, "Command.determine_sudo", sudo);
    }
    
    private static string get_available (string[] commands)
    {
        // Check each command in the list for existence, and return immediately if it does
        foreach (string cmd in commands)
        {
            if (exists (cmd))
            {
                return cmd;
            }
        }
        
        // Return the first command as fallback
        return commands[0];
    }
    
    public static void determine_terminal ()
    {
        terminal = get_available ({
            "gnome-terminal",
            "mate-terminal",
            "pantheon-terminal",
            "io.elementary.terminal",
            "tilix",
            "xfce4-terminal",
            "konsole",
            "deepin-terminal",
            "qterminal",
            "lxterminal",
            "uxterm",
            "xterm"
        });
        
        Debug.log (Debug.domain.ENVIRONMENT, "Command.determine_terminal", terminal);
    }
    
    public static void determine_file_manager ()
    {
        file_manager = get_available ({
            "nautilus",
            "caja",
            "nemo",
            "pantheon-files",
            "io.elementary.files",
            "thunar",
            "dolphin",
            "dde-file-manager",
            "pcmanfm-qt",
            "pcmanfm"
        });
        
        Debug.log (Debug.domain.ENVIRONMENT, "Command.determine_file_manager", file_manager);
    }
    
    public static void determine_remote_desktop ()
    {
        remote_desktop = get_available ({
            "vinagre",
            "gvncviewer",
            "krdc",
            "vncviewer",
            "xtightvncviewer",
            "xvnc4viewer",
            "rdesktop"
        });
        
        Debug.log (Debug.domain.ENVIRONMENT, "Command.determine_remote_desktop", remote_desktop);
    }
    
    public static string return_custom (Member? member, string command_ipv4, string command_ipv6, string priority)
    {
        string command = "";
        string address = "";
        
        if (Hamachi.ip_version == "Both")
        {
            if (priority == "IPv4")
            {
                if (member.ipv4 != null)
                {
                    command = command_ipv4;
                    address = member.ipv4;
                }
                else
                {
                    command = command_ipv6;
                    address = member.ipv6;
                }
            }
            if (priority == "IPv6")
            {
                if (member.ipv6 != null)
                {
                    command = command_ipv6;
                    address = member.ipv6;
                }
                else
                {
                    command = command_ipv4;
                    address = member.ipv4;
                }
            }
        }
        else if (Hamachi.ip_version == "IPv4")
        {
            command = command_ipv4;
            address = member.ipv4;
        }
        else if (Hamachi.ip_version == "IPv6")
        {
            command = command_ipv6;
            address = member.ipv6;
        }
        
        return replace_variables (command, address, member.nick, member.client_id);
    }
    
    public static string replace_variables (owned string command, string address, string nick, string id)
    {
        try
        {
            command = command.replace ("%A",  address);
            command = command.replace ("%N",  nick   );
            command = command.replace ("%ID", id     );
            
            string execute = (terminal == "gnome-terminal") ? "--" : "-e";
            string quote   = (terminal == "gnome-terminal") ? ""   : "\"";
            
            command = new Regex ("%TERMINAL (.*)").replace (command, -1, 0, terminal + " " + execute + " " + quote + "\\1" + quote);
            command = command.replace ("%FILEMANAGER", file_manager);
            command = command.replace ("%REMOTEDESKTOP", remote_desktop);
            command = command.replace ("{COLON}", ";");
        }
        catch (RegexError e)
        {
            Debug.log (Debug.domain.ERROR, "Command.replace_variables", e.message);
        }
        
        return command;
    }
    
    public static string[] return_default ()
    {
        string[] command  = new string[] {""};
        string[] commands = (string[]) Settings.custom_commands.val;
        
        foreach (string _string in commands)
        {
            string[] _array = _string.split (";", 6);
            
            if ((_array.length == 6) &&
                (_array[1] == "true") &&
                (custom_exists (_array[3], _array[4])))
            {
                command = _array;
            }
        }
        
        return command;
    }
    
    public static string[] return_by_number (int number)
    {
        string[] command  = new string[] {""};
        string[] commands = (string[]) Settings.custom_commands.val;
        
        int count = 0;
        
        foreach (string _string in commands)
        {
            string[] _array = _string.split (";", 6);
            
            if ((_array.length == 6) &&
                (_array[0] == "true") &&
                (custom_exists (_array[3], _array[4])))
            {
                count ++;
                
                if (count == number)
                {
                    command = _array;
                }
            }
        }
        
        return command;
    }
    
    public static void open_uri (string uri)
    {
        try
        {
#if GTK_3_22
            Gtk.show_uri_on_window (Haguichi.window, uri, Gdk.CURRENT_TIME);
#else
            Gtk.show_uri (null, uri, Gdk.CURRENT_TIME);
#endif
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "Command.open_uri", e.message);
            
            if (exists ("xdg-open"))
            {
                Debug.log (Debug.domain.ENVIRONMENT, "Command.open_uri", "Falling back to xdg-open");
                execute ("xdg-open " + uri);
            }
        }
    }
}
