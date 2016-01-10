/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2016 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

[DBus (name = "apps.Haguichi")]
public class AppSession : Object
{
    public void show ()
    {
        Haguichi.window.present();
    }
    
    public void hide ()
    {
        Haguichi.window.hide();
    }
    
    public void start_hamachi ()
    {
        GlobalEvents.start_hamachi();
    }
    
    public void stop_hamachi ()
    {
        GlobalEvents.stop_hamachi();
    }
    
    public void change_nick ()
    {
        GlobalEvents.change_nick();
    }
    
    public void join_network ()
    {
        GlobalEvents.join_network();
    }
    
    public void create_network ()
    {
        GlobalEvents.create_network();
    }
    
    public void information ()
    {
        GlobalEvents.information();
    }
    
    public void preferences ()
    {
        GlobalEvents.preferences();
    }
    
    public void about ()
    {
        GlobalEvents.about();
    }
    
    public void quit_app ()
    {
        GlobalEvents.quit_app();
    }
    
    public signal void mode_changed (string mode);
    public signal void modality_changed (bool modal);
    public signal void visibility_changed (bool visible);
    public signal void quitted ();
}
