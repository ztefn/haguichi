/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2015 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

public class Debug : Object
{
    public static void log (domain _domain, string reporter, string? output)
    {
        if (Haguichi.debugging)
        {
            DateTime now = new DateTime.now_local();
            
            stdout.printf ("\x1b[%sm[%02d:%02d:%02d.%06d %s] [%s]\x1b[0m %s\n", // http://ascii-table.com/ansi-escape-sequences.php
                           domain_color (_domain),
                           now.get_hour(),
                           now.get_minute(),
                           now.get_second(),
                           now.get_microsecond(),
                           domain_name (_domain).up(),
                           reporter,
                           output);
        }
    }
    
    public enum domain
    {
        ERROR, INFO, HAMACHI, ENVIRONMENT, GUI;
    }
    
    private static string domain_color (domain _domain)
    {
        switch (_domain)
        {
            case domain.ERROR:          return "00;31"; // Red
            case domain.INFO:           return "00;34"; // Blue
            case domain.HAMACHI:        return "00;35"; // Magenta
            case domain.ENVIRONMENT:    return "00;36"; // Cyan
            case domain.GUI:            return "00;32"; // Green
            
            default:                    return "0";
        }
    }
    
    private static string domain_name (domain _domain)
    {
        switch (_domain)
        {
            case domain.ERROR:          return "Error";
            case domain.INFO:           return "Info";
            case domain.HAMACHI:        return "Hamachi";
            case domain.ENVIRONMENT:    return "Environment";
            case domain.GUI:            return "Gui";
            
            default:                    return "Unknown";
        }
    }
}
