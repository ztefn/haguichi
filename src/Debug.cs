/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2013 Stephen Brandt <stephen@stephenbrandt.com>
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


public class Debug
{
    
    public static void Log ( Domain domain, string reporter, string output )
    {
        
        if ( Config.Settings.Debugging )
        {
            
            string domainString = Enum.GetName ( typeof ( Domain ), domain );
            
            Console.ForegroundColor = ( ConsoleColor ) domain;
            DateTime datetime = DateTime.Now;
            Console.Write ( "[{0} {1}] [{2}]", datetime.TimeOfDay, domainString.ToUpper (), reporter );
            Console.Write ( " " );
            Console.ResetColor ();
            Console.WriteLine ( output );
            
        }
        
    }
    
    
    public enum Domain
    {
        
        Error       = ConsoleColor.DarkRed,
        Info        = ConsoleColor.DarkBlue,
        Hamachi     = ConsoleColor.DarkGray,
        Environment = ConsoleColor.DarkCyan,
        Gui         = ConsoleColor.DarkGreen,
        
    }
    
}
