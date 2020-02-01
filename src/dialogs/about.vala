/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2020 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Gtk;

namespace Dialogs
{
    public class About : AboutDialog
    {
        public About ()
        {
            Object (transient_for:      Haguichi.window,
                    modal:              true,
                    program_name:       Text.app_name,
                    logo_icon_name:     Config.ICON_NAME,
                    comments:           Text.app_comments,
                    version:            Text.app_version,
                    license:            Text.app_license,
                    copyright:          Text.app_copyright,
                    website:            Text.app_website,
                    website_label:      Text.app_website_label,
                    translator_credits: Text.app_translator_credits);
            
            GlobalEvents.set_modal_dialog (this);
            
            show();
            
            response.connect ((response_id) =>
            {
                GlobalEvents.set_modal_dialog (null);
                destroy();
            });
        }
    }
}
