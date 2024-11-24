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
using Gtk;
using Gdk;

#if ENABLE_APPINDICATOR
using AppIndicator;
#endif

public class PanelIcon : StatusIcon
{
    
    private int    animIcon = 0;
    private string lastMode;
    
    private string connected    = "haguichi-connected";
    private string connecting1  = "haguichi-connecting-1";
    private string connecting2  = "haguichi-connecting-2";
    private string connecting3  = "haguichi-connecting-3";
    private string disconnected = "haguichi-disconnected";
    
#if ENABLE_APPINDICATOR
    private string icon_prefix  = "com.github.ztefn.";
    private static ApplicationIndicator indicator;
#endif
    
    
    public PanelIcon ()
    {
        
        IconTheme.AddBuiltinIcon ( connected,    16, Gdk.Pixbuf.LoadFromResource ( "16x16.connected"    ) );
        IconTheme.AddBuiltinIcon ( connecting1,  16, Gdk.Pixbuf.LoadFromResource ( "16x16.connecting-1" ) );
        IconTheme.AddBuiltinIcon ( connecting2,  16, Gdk.Pixbuf.LoadFromResource ( "16x16.connecting-2" ) );
        IconTheme.AddBuiltinIcon ( connecting3,  16, Gdk.Pixbuf.LoadFromResource ( "16x16.connecting-3" ) );
        IconTheme.AddBuiltinIcon ( disconnected, 16, Gdk.Pixbuf.LoadFromResource ( "16x16.disconnected" ) );
        
        IconTheme.AddBuiltinIcon ( connected,    22, Gdk.Pixbuf.LoadFromResource ( "22x22.connected"    ) );
        IconTheme.AddBuiltinIcon ( connecting1,  22, Gdk.Pixbuf.LoadFromResource ( "22x22.connecting-1" ) );
        IconTheme.AddBuiltinIcon ( connecting2,  22, Gdk.Pixbuf.LoadFromResource ( "22x22.connecting-2" ) );
        IconTheme.AddBuiltinIcon ( connecting3,  22, Gdk.Pixbuf.LoadFromResource ( "22x22.connecting-3" ) );
        IconTheme.AddBuiltinIcon ( disconnected, 22, Gdk.Pixbuf.LoadFromResource ( "22x22.disconnected" ) );
        
        IconTheme.AddBuiltinIcon ( connected,    24, Gdk.Pixbuf.LoadFromResource ( "24x24.connected"    ) );
        IconTheme.AddBuiltinIcon ( connecting1,  24, Gdk.Pixbuf.LoadFromResource ( "24x24.connecting-1" ) );
        IconTheme.AddBuiltinIcon ( connecting2,  24, Gdk.Pixbuf.LoadFromResource ( "24x24.connecting-2" ) );
        IconTheme.AddBuiltinIcon ( connecting3,  24, Gdk.Pixbuf.LoadFromResource ( "24x24.connecting-3" ) );
        IconTheme.AddBuiltinIcon ( disconnected, 24, Gdk.Pixbuf.LoadFromResource ( "24x24.disconnected" ) );
        
        IconTheme.AddBuiltinIcon ( connected,    48, Gdk.Pixbuf.LoadFromResource ( "48x48.connected"    ) );
        IconTheme.AddBuiltinIcon ( connecting1,  48, Gdk.Pixbuf.LoadFromResource ( "48x48.connecting-1" ) );
        IconTheme.AddBuiltinIcon ( connecting2,  48, Gdk.Pixbuf.LoadFromResource ( "48x48.connecting-2" ) );
        IconTheme.AddBuiltinIcon ( connecting3,  48, Gdk.Pixbuf.LoadFromResource ( "48x48.connecting-3" ) );
        IconTheme.AddBuiltinIcon ( disconnected, 48, Gdk.Pixbuf.LoadFromResource ( "48x48.disconnected" ) );
        
        this.IconName = disconnected;
        
#if ENABLE_APPINDICATOR
        indicator = new ApplicationIndicator ( "haguichi",
                                               "haguichi-disconnected",
                                               Category.ApplicationStatus );

        indicator.Menu = MainWindow.quickMenu;
        indicator.ScrollEvent += delegate ( object o, AppIndicator.ScrollEventArgs args )
        {
            // Never hide the main window when a modal dialog is being shown
            if ( Haguichi.modalDialog != null )
            {
                return;
            }
            
            // Show the main window when scrolling up and hide it when scrolling down
            if ( args.Direction == ScrollDirection.Up )
            {
                MainWindow.Show ();
            }
            else if ( args.Direction == ScrollDirection.Down )
            {
                MainWindow.Hide ();
            }
        };
#endif
        
    }
    
    
    public void SetMode ( string mode )
    {
        
        this.lastMode = mode;
        
        switch ( mode )
        {
            
            case "Connecting":
            
                GLib.Timeout.Add ( 400, new GLib.TimeoutHandler ( updateStatus ) );
            
                break;
                
            case "Connected":
            
                this.IconName = connected;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + connected;
#endif
            
                break;
                
            case "Disconnected":
            
                this.IconName = disconnected;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + disconnected;
#endif
            
                break;
            
            case "Not configured":
            
                this.IconName = disconnected;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + disconnected;
#endif
            
                break;
            
            case "Not installed":
            
                this.IconName = disconnected;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + disconnected;
#endif
            
                break;
            
        }
        
    }
    
    
    public void SetVisibility ( bool visible )
    {
        
        Visible = visible;
        indicator.Status = visible ? AppIndicator.Status.Active : AppIndicator.Status.Passive;
        
    }


    private bool updateStatus ()
    {
        
        if ( lastMode == "Connecting" )
        {
            if ( animIcon == 0 )
            {
                this.IconName = connecting1;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + connecting1;
#endif
                animIcon      = 1;
            }
            else if ( animIcon == 1 )
            {
                this.IconName = connecting2;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + connecting2;
#endif
                animIcon      = 2;
            }
            else
            {
                this.IconName = connecting3;
#if ENABLE_APPINDICATOR
                indicator.IconName = icon_prefix + connecting3;
#endif
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
