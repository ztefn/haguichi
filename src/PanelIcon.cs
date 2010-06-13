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
using Gdk;


public class PanelIcon : StatusIcon
{
    
    private int    animIcon = 0;    
    private string lastMode;
    
    private Pixbuf connected;
    private Pixbuf connecting1;
    private Pixbuf connecting2;
    private Pixbuf connecting3;
    private Pixbuf disconnected;
    
    
    public PanelIcon ()
    {
        
        try
        {
            connected    = new Pixbuf ( ( string ) Config.Client.Get ( Config.Settings.PixmapsPath ) + "/panel-connected.png" );
            connecting1  = new Pixbuf ( ( string ) Config.Client.Get ( Config.Settings.PixmapsPath ) + "/panel-connecting-1.png" );
            connecting2  = new Pixbuf ( ( string ) Config.Client.Get ( Config.Settings.PixmapsPath ) + "/panel-connecting-2.png" );
            connecting3  = new Pixbuf ( ( string ) Config.Client.Get ( Config.Settings.PixmapsPath ) + "/panel-connecting-3.png" );
            disconnected = new Pixbuf ( ( string ) Config.Client.Get ( Config.Settings.PixmapsPath ) + "/panel-disconnected.png" );
        }
        catch
        {
            connected    = Pixbuf.LoadFromResource ( "panel-connected" );
            connecting1  = Pixbuf.LoadFromResource ( "panel-connecting-1" );
            connecting2  = Pixbuf.LoadFromResource ( "panel-connecting-2" );
            connecting3  = Pixbuf.LoadFromResource ( "panel-connecting-3" );
            disconnected = Pixbuf.LoadFromResource ( "panel-disconnected" );
        }
        
        this.Pixbuf = disconnected;
        this.Tooltip = TextStrings.appName;
        
    }
    
    
    public void SetMode ( string mode )
    {
    
        string tipTail = " - ";
        
        this.lastMode = mode;
        
        switch ( mode )
        {
            
            case "Connecting":
            
                tipTail     += TextStrings.connecting;
                GLib.Timeout.Add ( 400, new GLib.TimeoutHandler ( update_status ) );
            
                break;
                
            case "Connected":
            
                tipTail     += TextStrings.connected;
                this.Pixbuf  = connected;
            
                break;
                
            case "Disconnected":
            
                tipTail     += TextStrings.disconnected;
                this.Pixbuf  = disconnected;
            
                break;
            
            case "Not configured":
            
                tipTail     += TextStrings.notConfigured;
                this.Pixbuf  = disconnected;
            
                break;
            
            case "Not installed":
            
                tipTail     += TextStrings.notInstalled;
                this.Pixbuf  = disconnected;
            
                break;
            
        }
        
        this.Tooltip = TextStrings.appName + tipTail;
        
    }


    private bool update_status ()
    {
        
        if ( lastMode == "Connecting" )
        {
            if ( animIcon == 0 )
            {
                this.Pixbuf = connecting1;
                animIcon    = 1;
            }
            else if ( animIcon == 1 )
            {
                this.Pixbuf = connecting2;
                animIcon    = 2;
            }
            else
            {
                this.Pixbuf = connecting3;
                animIcon    = 0;
            }
            
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
}
