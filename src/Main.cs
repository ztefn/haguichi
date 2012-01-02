/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2012 Stephen Brandt <stephen@stephenbrandt.com>
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
using Gtk;
using Mono.Unix;


class Haguichi
{
    
    public static MainWindow mainWindow;
    public static Dialogs.About aboutDialog;
    public static Dialogs.Information informationWindow;
    public static Windows.Preferences preferencesWindow;
    
    public static Dialog modalDialog;
    
    public static Connection connection;
    
    
    static void Main ( string [] args )
    {
        
        foreach ( string s in args )
        {
            if ( ( s == "-h" ) || ( s == "--help" ) )
            {
                Console.WriteLine ( TextStrings.appHelp );
                return;
            }
            if ( ( s == "-v" ) || ( s == "--version" ) )
            {
                Console.WriteLine ( TextStrings.appName + " " + TextStrings.appVersion );
                return;
            }
            if ( s == "--license" )
            {
                Console.WriteLine ( "\n" + TextStrings.appInfo + "\n\n" + TextStrings.appLicense + "\n" );
                return;
            }
            
            if ( ( s == "-d" ) || ( s == "--debug" ) )
            {
                Config.Settings.Debugging = true;
            }
            else if ( s == "--demo" )
            {
                Config.Settings.DemoMode = true;
            }
            else
            {
                Console.WriteLine ( "Unknown option " + s + "\n" );
                Console.WriteLine ( TextStrings.appHelp );
                return;
            }
        }
        
        Debug.Log ( Debug.Domain.Info, "Main", "Greetings, I am " + TextStrings.appName + " " + TextStrings.appVersion );
        
        Platform.Init ();
        
        Debug.Log ( Debug.Domain.Environment, "Main", "Using the following path for locales: " + Config.Settings.LocalePath );
        Catalog.Init ( TextStrings.appName.ToLower (), Config.Settings.LocalePath );
        
        TextStrings.Init ();
        Config.Client.Init ();
        Config.Settings.Init ();
        Command.Init ();
        Hamachi.Init ();
        
        mainWindow          = new MainWindow ();
        aboutDialog         = new Dialogs.About ();
        informationWindow   = new Dialogs.Information ( TextStrings.informationTitle );
        preferencesWindow   = new Windows.Preferences ( TextStrings.preferencesTitle );
        
        connection          = new Connection ();
        
        Controller.Init ();
        
        Application.Run ();
        
    }
    
}
