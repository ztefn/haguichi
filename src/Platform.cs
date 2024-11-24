/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2024 Stephen Brandt <stephen@stephenbrandt.com>
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
using DBus;


public static class Platform
{
    
    public const string appBusName = "com.github.ztefn.haguichi";
    public const string desktopBusName = "org.freedesktop.portal.Desktop";
    
    private static Bus bus;
    private static ObjectPath path;
    public static ApplicationSession appSession;
    public static DesktopSession desktopSession;
    
    
    [DllImport ( "libc" )] // Linux
    private static extern int prctl ( int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5 );

    
    [DllImport ( "libc" )] // BSD
    private static extern void setproctitle ( byte [] fmt, byte [] str_arg );

    
    public static void Init ()
    {
        
        BusG.Init ();
        
        bus  = Bus.Session;
        path = new ObjectPath ( "/com/github/ztefn/haguichi" );
        
        if ( Platform.ActiveSession () )
        {
            Debug.Log ( Debug.Domain.Environment, "Main", "There is already an active session, will try to show it and close this session" );
            
            appSession = bus.GetObject <ApplicationSession> ( appBusName, path );
            appSession.Show ();
            
            Environment.Exit ( 0 );
        }
        else
        {
            Debug.Log ( Debug.Domain.Environment, "Main", "Registering session" );
            
            RegisterSession ();
            
            SetProcessName ();
        }
        
    }
    
    
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
                throw new ApplicationException ( "Error setting process name: " + Mono.Unix.Native.Stdlib.GetLastError () );
            }
        }
        catch ( EntryPointNotFoundException )
        {
            setproctitle ( Encoding.ASCII.GetBytes ( "%s\0" ), Encoding.ASCII.GetBytes ( TextStrings.appName.ToLower () + "\0" ) );
        }
        
    }
    
    
    public static bool ActiveSession ()
    {
        
        if ( bus.NameHasOwner ( appBusName ) )
        {
            return true;
        }
        
        return false;
        
    }
    
    
    public static void RegisterSession ()
    {
        
        appSession = new ApplicationSession ();
        
        bus.RequestName ( appBusName );
        bus.Register ( path, appSession );
        
        desktopSession = Bus.Session.GetObject <DesktopSession> ( desktopBusName, new ObjectPath ( "/org/freedesktop/portal/desktop" ) );
        
    }
    
    
    public static void UnregisterSession ()
    {
        
        bus.ReleaseName ( appBusName );
        
    }
    
}
