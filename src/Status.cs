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
using Gdk;


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
    
    
    public Pixbuf GetPixbuf ( int size )
    {
        
        Pixbuf statusPix = GetPixbufOffline ( size );
        
        if ( this.statusInt == 1 )
        {
            statusPix = GetPixbufOnline ( size );
        }
        if ( this.statusInt == 2 )
        {
            statusPix = GetPixbufUnreachable ( size );
        }
        if ( this.statusInt == 3 )
        {
            statusPix = GetPixbufUnapproved ( size );
        }
        
        return statusPix;
        
    }
    
    
    public static Pixbuf GetPixbuf ( int size, int statusInt )
    {
        
        Pixbuf statusPix = GetPixbufOffline ( size );
        
        if ( statusInt == 1 )
        {
            statusPix = GetPixbufOnline ( size );
        }
        if ( statusInt == 2 )
        {
            statusPix = GetPixbufUnreachable ( size );
        }
        if ( statusInt == 3 )
        {
            statusPix = GetPixbufUnapproved ( size );
        }
        
        return statusPix;
        
    }
    
    
    public static Pixbuf GetPixbufOnline ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-online", size, IconLookupFlags.UseBuiltin );
        
    }
    
    
    public static Pixbuf GetPixbufOffline ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-offline", size, IconLookupFlags.UseBuiltin );
        
    }
    
    
    public static Pixbuf GetPixbufUnreachable ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-unreachable", size, IconLookupFlags.UseBuiltin );
        
    }
    
    
    public static Pixbuf GetPixbufUnapproved ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-unapproved", size, IconLookupFlags.UseBuiltin );
        
    }
    
}
