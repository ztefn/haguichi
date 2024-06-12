/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2024 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

namespace Haguichi {
    [DBus (name = "com.github.ztefn.haguichi")]
    public class Session : Object {
        public void show () throws Error {
            win.present ();
        }

        public void hide () throws Error {
            win.hide ();
        }

        public void start_hamachi () throws Error {
            win.connect_action ();
        }

        public void stop_hamachi () throws Error {
            win.disconnect_action ();
        }

        public void join_network () throws Error {
            win.present ();
            win.join_network_action ();
        }

        public void create_network () throws Error {
            win.present ();
            win.create_network_action ();
        }

        public void information () throws Error {
            win.present ();
            win.info_action ();
        }

        public void preferences () throws Error {
            win.present ();
            win.preferences_action ();
        }

        public void about () throws Error {
            win.present ();
            win.about_action ();
        }

        public void quit_app () throws Error {
            win.quit_action ();
        }

        public string get_mode () throws Error {
            return win.get_mode ();
        }

        public bool get_modality () throws Error {
            return (win.modal_dialog != null);
        }

        public bool get_visibility () throws Error {
            return win.visible;
        }

        public signal void mode_changed (string mode);
        public signal void modality_changed (bool modal);
        public signal void visibility_changed (bool visible);
        public signal void quitted ();
    }
}
