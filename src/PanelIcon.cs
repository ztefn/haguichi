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
    
    private string connected    = "haguichi-connected";
    private string connecting1  = "haguichi-connecting-1";
    private string connecting2  = "haguichi-connecting-2";
    private string connecting3  = "haguichi-connecting-3";
    private string disconnected = "haguichi-disconnected";
    
    
    public PanelIcon ()
    {
        
        IconTheme.AddBuiltinIcon ( "haguichi-connected",    22, Gdk.Pixbuf.LoadFromResource ( "connected"    ) );
        IconTheme.AddBuiltinIcon ( "haguichi-connecting-1", 22, Gdk.Pixbuf.LoadFromResource ( "connecting-1" ) );
        IconTheme.AddBuiltinIcon ( "haguichi-connecting-2", 22, Gdk.Pixbuf.LoadFromResource ( "connecting-2" ) );
        IconTheme.AddBuiltinIcon ( "haguichi-connecting-3", 22, Gdk.Pixbuf.LoadFromResource ( "connecting-3" ) );
        IconTheme.AddBuiltinIcon ( "haguichi-disconnected", 22, Gdk.Pixbuf.LoadFromResource ( "disconnected" ) );
        
        this.IconName = disconnected;
        this.Tooltip  = TextStrings.appName;
        
    }
    
    
    public void SetMode ( string mode )
    {
    
        string tipTail = " - ";
        
        this.lastMode = mode;
        
        switch ( mode )
        {
            
            case "Connecting":
            
                tipTail      += TextStrings.connecting;
                GLib.Timeout.Add ( 400, new GLib.TimeoutHandler ( updateStatus ) );
            
                break;
                
            case "Connected":
            
                tipTail      += TextStrings.connected;
                this.IconName = connected;
            
                break;
                
            case "Disconnected":
            
                tipTail      += TextStrings.disconnected;
                this.IconName = disconnected;
            
                break;
            
            case "Not configured":
            
                tipTail      += TextStrings.notConfigured;
                this.IconName = disconnected;
            
                break;
            
            case "Not installed":
            
                tipTail      += TextStrings.notInstalled;
                this.IconName = disconnected;
            
                break;
            
        }
        
        this.Tooltip = TextStrings.appName + tipTail;
        
    }


    private bool updateStatus ()
    {
        
        if ( lastMode == "Connecting" )
        {
            if ( animIcon == 0 )
            {
                this.IconName = connecting1;
                animIcon      = 1;
            }
            else if ( animIcon == 1 )
            {
                this.IconName = connecting2;
                animIcon      = 2;
            }
            else
            {
                this.IconName = connecting3;
                animIcon      = 0;
            }
            
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
}
