/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2011 Stephen Brandt <stephen@stephenbrandt.com>
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
    
    public const string busName = "org." + TextStrings.appName;
    
    private static Bus bus;
    private static ObjectPath path;
    private static Session session;
    
    
    [DllImport ( "libc" )] // Linux
    private static extern int prctl ( int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5 );

    
    [DllImport ( "libc" )] // BSD
    private static extern void setproctitle ( byte [] fmt, byte [] str_arg );

    
    public static void Init ()
    {
        
        BusG.Init ();
        
        bus  = Bus.Session;
        path = new ObjectPath ( "/org/" + TextStrings.appName );
        
        if ( Platform.ActiveSession () )
        {
            Debug.Log ( Debug.Domain.Environment, "Main", "There is already an active session, will try to show it and close this session" );
            
            session = bus.GetObject <Session> ( busName, path );
            session.Present ();
            
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
        
        if ( bus.NameHasOwner ( busName ) )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
    
    public static void RegisterSession ()
    {
        
        session = new Session ();
        
        bus.RequestName ( busName );
        bus.Register ( path, session );
        
    }
    
    
    public static void UnregisterSession ()
    {
        
        bus.ReleaseName ( busName );
        
    }
    
}
