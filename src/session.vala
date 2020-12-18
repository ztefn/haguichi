/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

[DBus (name = "com.github.ztefn.haguichi")]
public class AppSession : Object
{
    public void show () throws Error
    {
        Haguichi.window.present();
    }
    
    public void hide () throws Error
    {
        Haguichi.window.hide();
    }
    
    public void start_hamachi () throws Error
    {
        GlobalEvents.start_hamachi();
    }
    
    public void stop_hamachi () throws Error
    {
        GlobalEvents.stop_hamachi();
    }
    
    public void change_nick () throws Error
    {
        Haguichi.window.present();
        GlobalEvents.change_nick();
    }
    
    public void join_network () throws Error
    {
        Haguichi.window.present();
        GlobalEvents.join_network();
    }
    
    public void create_network () throws Error
    {
        Haguichi.window.present();
        GlobalEvents.create_network();
    }
    
    public void information () throws Error
    {
        Haguichi.window.present();
        GlobalEvents.information();
    }
    
    public void preferences () throws Error
    {
        GlobalEvents.preferences();
    }
    
    public void about () throws Error
    {
        GlobalEvents.about();
    }
    
    public void quit_app () throws Error
    {
        GlobalEvents.quit_app();
    }
    
    public string get_mode () throws Error
    {
        return Haguichi.window.mode;
    }
    
    public bool get_modality () throws Error
    {
        return (Haguichi.modal_dialog != null);
    }
    
    public bool get_visibility () throws Error
    {
        return Haguichi.window.visible;
    }
    
    public signal void mode_changed (string mode);
    public signal void modality_changed (bool modal);
    public signal void visibility_changed (bool visible);
    public signal void quitted();
}
