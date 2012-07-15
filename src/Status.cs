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
    public string ConnectionType;
    
    
    public Status ( string status )
    {
        
        SetStatus ( status );
        ConnectionType = "";
        
    }
    
    
    public Status ( string status, string connection )
    {
        
        SetStatus ( status );
        SetConnectionType ( connection );
        
    }
    
    
    public void SetStatus ( string status )
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
    
    
    private void SetConnectionType ( string connection )
    {
        
        ConnectionType = "";
        
        if ( connection == "direct" )
        {
            ConnectionType = TextStrings.direct;
        }
        else if ( connection.Contains ( "via " ) )
        {
            ConnectionType = TextStrings.relayed;
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
    
    
    private Pixbuf GetPixbufOnline ( int size )
    {
        
        if ( ConnectionType == TextStrings.relayed )
        {
            return IconTheme.Default.LoadIcon ( "haguichi-node-online-relayed", size, IconLookupFlags.UseBuiltin );
        }
        else
        {
            return IconTheme.Default.LoadIcon ( "haguichi-node-online", size, IconLookupFlags.UseBuiltin );
        }
        
    }
    
    
    private Pixbuf GetPixbufOffline ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-offline", size, IconLookupFlags.UseBuiltin );
        
    }
    
    
    private Pixbuf GetPixbufUnreachable ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-unreachable", size, IconLookupFlags.UseBuiltin );
        
    }
    
    
    private Pixbuf GetPixbufUnapproved ( int size )
    {
        
        return IconTheme.Default.LoadIcon ( "haguichi-node-unapproved", size, IconLookupFlags.UseBuiltin );
        
    }
    
}
