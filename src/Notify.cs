/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2015 Stephen Brandt <stephen@stephenbrandt.com>
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
using Notifications;


public class Notify
{
    
    private Notification notification;
    
    
    public Notify ( string summary, string body )
    {
        
        notification = new Notification ( summary, body, MainWindow.appIcons [4] );
        notification.Show ();
        
    }
    
}
