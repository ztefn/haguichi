/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Utils : Object
{
    public static string format (owned string text, string? param1, string? param2, string? param3)
    {
        if (param1 == null) {text = text.replace ("{0}", "");}
        if (param2 == null) {text = text.replace ("{1}", "");}
        if (param3 == null) {text = text.replace ("{2}", "");}
        
        if (param1 != null) {text = text.replace ("{0}", param1);}
        if (param2 != null) {text = text.replace ("{1}", param2);}
        if (param3 != null) {text = text.replace ("{2}", param3);}
        
        return text;
    }
    
    public static string remove_mnemonics (owned string label)
    {
        try
        {
            label = new Regex (""" ?\(_[a-zA-Z]\)""").replace (label, -1, 0, ""); // Japanse translations
            label = label.replace ("_", "");                                      // All other translations
        }
        catch (RegexError e)
        {
            Debug.log (Debug.domain.ERROR, "Utils.remove_mnemonics", e.message);
        }
        
        return label;
    }
    
    public static string remove_colons (owned string label)
    {
        label = label.replace (" :", ""); // French translations
        label = label.replace (":",  ""); // All other translations
        
        return label;
    }
    
    public static string as_string (string[] commands)
    {
        string output = "";
        
        foreach (string c in commands)
        {
            output += c;
        }
        
        return output;
    }
    
    public static string clean_string (owned string _string)
    {
        _string = _string.replace ("\\", "\\\\");
        _string = _string.replace ("\"", "\\\"");
        
        return _string;
    }
    
    public static string protocol_to_string (int protocol)
    {
        string output = "";
        
        switch (protocol)
        {
            case 0:
                output = "both";
                break;
            
            case 1:
                output = "ipv4";
                break;
            
            case 2:
                output = "ipv6";
                break;
        }
        
        return output;
    }
    
    public static int protocol_to_int (string protocol)
    {
        int output = 0;
        
        switch (protocol)
        {
            case "both":
                output = 0;
                break;
            
            case "ipv4":
                output = 1;
                break;
            
            case "ipv6":
                output = 2;
                break;
        }
        
        return output;
    }
    
    public static string get_symbolic_icon_name (owned string icon_name)
    {
        if (Gtk.IconTheme.get_default().has_icon (icon_name + "-symbolic") )
        {
            icon_name += "-symbolic";
        }
        
        return icon_name;
    }
    
    public static string get_information_icon_name (bool symbolic)
    {
        string icon_name = "";
        
        if (Gtk.IconTheme.get_default().has_icon ("dialog-information-symbolic") && symbolic)
        {
            icon_name = "dialog-information-symbolic";
        }
        else if (Gtk.IconTheme.get_default().has_icon ("dialog-information"))
        {
            icon_name = "dialog-information";
        }
        
        return icon_name;
    }
    
    public static string get_network_icon_name (bool symbolic)
    {
        string icon_name = "";
        
        if (Gtk.IconTheme.get_default().has_icon ("network-workgroup-symbolic") && symbolic)
        {
            icon_name = "network-workgroup-symbolic";
        }
        else if (Gtk.IconTheme.get_default().has_icon ("network-workgroup"))
        {
            icon_name = "network-workgroup";
        }
        
        return icon_name;
    }
    
    public static string get_member_icon_name (bool symbolic)
    {
        string icon_name = "";
        
        if (Gtk.IconTheme.get_default().has_icon ("avatar-default-symbolic") && symbolic)
        {
            icon_name = "avatar-default-symbolic";
        }
        else if (Gtk.IconTheme.get_default().has_icon ("stock_person"))
        {
            icon_name = "stock_person";
        }
        else if (Gtk.IconTheme.get_default().has_icon ("avatar-default"))
        {
            icon_name = "avatar-default";
        }
        else if (Gtk.IconTheme.get_default().has_icon ("user-info"))
        {
            icon_name = "user-info";
        }
        else if (Gtk.IconTheme.get_default().has_icon ("user-identity"))
        {
            icon_name = "user-identity";
        }
        
        return icon_name;
    }
}
