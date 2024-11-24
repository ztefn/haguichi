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
using DBus;
using org.freedesktop.DBus;


[Interface ( Platform.appBusName )]
public class ApplicationSession : MarshalByRefObject
{
    
    public void Show ()
    {
        
        MainWindow.Show ();
        
    }
    
    
    public void Hide ()
    {
        
        MainWindow.Hide ();
        
    }
    
    
    public void StartHamachi ()
    {
        
        GlobalEvents.StartHamachi ();
        
    }
    
    
    public void StopHamachi ()
    {
        
        GlobalEvents.StopHamachi ();
        
    }
    
    
    public void ChangeNick ()
    {
        
        GlobalEvents.ChangeNick ();
        
    }
    
    
    public void JoinNetwork ()
    {
        
        GlobalEvents.JoinNetwork ();
        
    }
    
    
    public void CreateNetwork ()
    {
        
        GlobalEvents.CreateNetwork ();
        
    }
    
    
    public void Information ()
    {
        
        GlobalEvents.Information ();
        
    }
    
    
    public void Preferences ()
    {
        
        GlobalEvents.Preferences ();
        
    }
    
    
    public void About ()
    {
        
        GlobalEvents.About ();
        
    }
    
    
    public void QuitApp ()
    {
        
        GlobalEvents.QuitApp ();
        
    }
    
    
    public string GetMode ()
    {
        
        return MainWindow.lastMode;
        
    }
    
    
    public bool GetModality ()
    {
        
        return Haguichi.modalDialog != null;
        
    }
    
    
    public bool GetVisibility ()
    {
        
        return MainWindow.Visible;
        
    }
    
    
    public delegate void ModeChangedEvent (string mode);
    public event ModeChangedEvent ModeChanged;
    internal void FireModeChanged (string mode)
    {
        
        ModeChanged?.Invoke (mode);
        
    }
    
    
    public delegate void ModalityChangedEvent (bool modal);
    public event ModalityChangedEvent ModalityChanged;
    internal void FireModalityChanged (bool modal)
    {
        
        ModalityChanged?.Invoke (modal);
        
    }
    
    
    public delegate void VisibilityChangedEvent (bool visible);
    public event VisibilityChangedEvent VisibilityChanged;
    internal void FireVisibilityChanged (bool visible)
    {
        
        VisibilityChanged?.Invoke (visible);
        
    }
    
    
    public delegate void QuitEvent ();
    public event QuitEvent Quitted;
    internal void FireQuitted ()
    {
        
        Quitted?.Invoke ();
        
    }
    
}


public delegate void SettingChangedHandler ( string namesp, string key, object val );

[Interface ( "org.freedesktop.portal.Settings" )]
public interface DesktopSession : Introspectable
{
    
    object ReadOne ( string namesp, string val );
    
    event SettingChangedHandler SettingChanged;
    
}

