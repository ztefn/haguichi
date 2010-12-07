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
using Gtk;


public class Status
{

    public int    statusInt;
    public string statusString;
    public string statusSortable;

    
    public Status ( string status )
    {
        
        if ( status == " " )
        {
            statusInt = 0;
            statusString = TextStrings.offline;
            statusSortable = "d";
        }
        if ( status == "*" )
        {
            statusInt = 1;
            statusString = TextStrings.online;
            statusSortable = "a";
        }
        if ( status == "x" )
        {
            statusInt = 2;
            statusString = TextStrings.unreachable;
            statusSortable = "b";
        }
        if ( status == "?" )
        {
            statusInt = 3;
            statusString = TextStrings.unapproved;
            statusSortable = "c";
        }
        
    }
    
    
    public static Gdk.Pixbuf GetPixbuf ( Status status )
    {
        
        Gdk.Pixbuf statusPix = GetPixbufOffline ();
        
        if ( status.statusInt == 1 )
        {
            statusPix = GetPixbufOnline ();
        }
        if ( status.statusInt == 2 )
        {
            statusPix = GetPixbufUnreachable ();
        }
        if ( status.statusInt == 3 )
        {
            statusPix = GetPixbufUnapproved ();
        }
        
        return statusPix;
        
    }
    
    
    public static Gdk.Pixbuf GetPixbufOnline ()
    {
        
        IconTheme.AddBuiltinIcon ( "haguichi-node-online", 10, Gdk.Pixbuf.LoadFromResource ( "node-online" ) );
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-online", 10, IconLookupFlags.UseBuiltin );
        
    }
    
    
    public static Gdk.Pixbuf GetPixbufOffline ()
    {
        
        IconTheme.AddBuiltinIcon ( "haguichi-node-offline", 10, Gdk.Pixbuf.LoadFromResource ( "node-offline" ) );
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-offline", 10, IconLookupFlags.UseBuiltin );
        
    }
    
    
    public static Gdk.Pixbuf GetPixbufUnreachable ()
    {
        
        IconTheme.AddBuiltinIcon ( "haguichi-node-unreachable", 10, Gdk.Pixbuf.LoadFromResource ( "node-unreachable" ) );
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-unreachable", 10, IconLookupFlags.UseBuiltin );
        
    }
    
    
    public static Gdk.Pixbuf GetPixbufUnapproved ()
    {
        
        IconTheme.AddBuiltinIcon ( "haguichi-node-unapproved", 10, Gdk.Pixbuf.LoadFromResource ( "node-unapproved" ) );
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-unapproved", 10, IconLookupFlags.UseBuiltin );
        
    }
    
}
