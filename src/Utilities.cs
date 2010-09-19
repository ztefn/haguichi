/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2010 Stephen Brandt <stephen@stephenbrandt.com>
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
        
        label = Regex.Replace ( label, @" \(_[a-zA-Z]\)", "" );   // For Japanse translations
        label = label.Replace ( "_", "" );                        // For all other translations
        
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

}
