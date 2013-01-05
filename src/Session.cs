/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2013 Stephen Brandt <stephen@stephenbrandt.com>
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
using NDesk.DBus;


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
    
}


[Interface ( Platform.indicatorBusName )]
public class IndicatorSession : MarshalByRefObject
{
    
    public void Show ( bool show )
    {}
    
    public void SetVisibility ( bool visible )
    {}
    
    public void SetModality ( bool modal )
    {}
    
    public void SetMode ( string mode )
    {}
    
    public void QuitApp ()
    {}
    
}
