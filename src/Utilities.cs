/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2015 Stephen Brandt <stephen@stephenbrandt.com>
 * 
 * Haguichi is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 * 
 * Haguichi is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Haguichi; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Text.RegularExpressions;


public static class Utilities
{
    
    public static string RemoveMnemonics ( string label )
    {
        
        label = Regex.Replace ( label, @" ?\(_[a-zA-Z]\)", "" );  // For Japanse translations
        label = label.Replace ( "_", "" );                        // For all other translations
        
        return label;
        
    }
    
    
    public static string RemoveColons ( string label )
    {
        
        label = label.Replace ( " :", "" );
        label = label.Replace ( ":",  "" );
        
        return label;
        
    }
    
    
    public static string AsString ( string [] commands )
    {
        
        string output = "";
        
        foreach ( string c in commands )
        {
            output += c;
        }
        
        return output;
        
    }
    
    
    public static string CleanString ( string inString )
    {
        
        inString = inString.Replace ( "\\", "\\\\" );
        inString = inString.Replace ( "\"", "\\\"" );
        
        return inString;
        
    }
    
    
    public static string ColorSchemeToString ( int scheme )
    {
        
        string output = "";
        
        switch ( scheme )
        {
            case 0:
                output = "System";
                break;
            
            case 1:
                output = "Dark";
                break;
            
            case 2:
                output = "Light";
                break;
        }
        
        return output.ToLower ();
        
    }
    
    
    public static int ColorSchemeToInt ( string scheme )
    {
        
        int output = 0;
        
        switch ( scheme.ToLower () )
        {
            case "system":
                output = 0;
                break;
            
            case "dark":
                output = 1;
                break;
            
            case "light":
                output = 2;
                break;
        }
        
        return output;
        
    }
    
    
    public static string ProtocolToString ( int protocol )
    {
        
        string output = "";
        
        switch ( protocol )
        {
            case 0:
                output = "Both";
                break;
            
            case 1:
                output = "IPv4";
                break;
            
            case 2:
                output = "IPv6";
                break;
        }
        
        return output.ToLower ();
        
    }
    
    
    public static int ProtocolToInt ( string protocol )
    {
        
        int output = 0;
        
        switch ( protocol.ToLower () )
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
    
}
