/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2018 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using AppIndicator;

class HaguichiIndicator
{
    public string icon_connected    = "haguichi-connected";
    public string icon_connecting1  = "haguichi-connecting-1";
    public string icon_connecting2  = "haguichi-connecting-2";
    public string icon_connecting3  = "haguichi-connecting-3";
    public string icon_disconnected = "haguichi-disconnected";
    
    private IndicatorMenu menu;
    private Indicator indicator;
    
    public HaguichiIndicator ()
    {
        // Only on specific desktops we use symbolic icons
        if ((Haguichi.current_desktop.contains ("GNOME")) ||
            (Haguichi.current_desktop == "Pantheon"))
        {
            string postfix = "-symbolic";
            
            icon_connected    += postfix;
            icon_connecting1  += postfix;
            icon_connecting2  += postfix;
            icon_connecting3  += postfix;
            icon_disconnected += postfix;
        }
        
        menu = new IndicatorMenu();
        
        indicator = new Indicator ("haguichi", icon_disconnected, IndicatorCategory.APPLICATION_STATUS);
        indicator.set_title ("Haguichi");
        indicator.set_menu (menu);
        indicator.set_status (IndicatorStatus.ACTIVE);
        indicator.scroll_event.connect ((ind, steps, direction) =>
        {
            // Never hide the main window when a modal dialog is being shown
            if (menu.modal == true)
            {
                return;
            }
            
            // Show the main window when scrolling up and hide it when scrolling down
            if (direction == Gdk.ScrollDirection.UP)
            {
                Haguichi.session.show();
            }
            else if (direction == Gdk.ScrollDirection.DOWN)
            {
                Haguichi.session.hide();
            }
        });
    }
    
    public void set_icon_name (string name)
    {
        indicator.icon_name = name;
    }
}
