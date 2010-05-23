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
using System.Text;
using System.Runtime.InteropServices;
using NDesk.DBus;


public static class Platform
{

    private static string sessionName = "org." + TextStrings.appName;
    
    
    [ DllImport ( "libc" ) ] // Linux
    private static extern int prctl ( int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5 );

    
    [ DllImport ( "libc" ) ] // BSD
    private static extern void setproctitle ( byte [] fmt, byte [] str_arg );

    
    public static void SetProcessName ()
    {
        
        if ( Environment.OSVersion.Platform != PlatformID.Unix )
        {
            return;
        }

        try
        {
            if ( prctl ( 15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes ( TextStrings.appName.ToLower () + "\0" ), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero ) != 0 ) 
            {
                throw new ApplicationException ("Error setting process name: " + Mono.Unix.Native.Stdlib.GetLastError () );
            }
        }
        catch ( EntryPointNotFoundException )
        {
            setproctitle ( Encoding.ASCII.GetBytes ( "%s\0" ), Encoding.ASCII.GetBytes ( TextStrings.appName.ToLower () + "\0" ) );
        }
        
    }
    
    
    public static bool ActiveProcess ()
    {
        
        if ( Bus.Session.NameHasOwner ( sessionName ) )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
    
    public static void RegisterProcess ()
    {
        
        Bus.Session.RequestName ( sessionName );
        
    }
    
    
    public static void UnregisterProcess ()
    {
        
        Bus.Session.ReleaseName ( sessionName );
        
    }
    
}
