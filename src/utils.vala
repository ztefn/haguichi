/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2025 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 *
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

namespace Haguichi {
    public class Utils : Object {
        public static string remove_mnemonics (owned string label) {
            try {
                label = new Regex ("""\(_[a-zA-Z]\)""").replace (label, -1, 0, ""); // Japanse translations
                label = label.replace ("_", "");                                    // All other translations
            } catch (RegexError e) {
                critical ("remove_mnemonics: %s", e.message);
            }

            return label;
        }

        public static string clean_string (owned string _string) {
            _string = _string.replace ("\\", "\\\\");
            _string = _string.replace ("\"", "\\\"");

            return _string;
        }

        public static string parse_nick (string nick) {
            var parsed_nick = nick;

            if (nick == "%USERNAME") {
                parsed_nick = Environment.get_user_name ();
            } else if (nick == "%REALNAME") {
                parsed_nick = Environment.get_real_name ();
            } else if (nick == "%HOSTNAME") {
                parsed_nick = Environment.get_host_name ();
            }

            return parsed_nick;
        }

        public static unowned Gtk.IconTheme get_icon_theme () {
            return Gtk.IconTheme.get_for_display (Gdk.Display.get_default ());
        }

        public static string get_available_theme_icon (string[] icon_names) {
            var icon_theme = get_icon_theme ();

            // Check each icon name in the list for existence, and return immediately if it does
            foreach (string icon_name in icon_names) {
                if (icon_theme.has_icon (icon_name)) {
                    return icon_name;
                }
            }

            // Return the first icon name as fallback
            return icon_names[0];
        }

        public static bool path_exists (string type, string path) {
            var result = FileUtils.test (path, FileTest.EXISTS);
            debug ("path_exists: FileUtils tested %s for path %s", result.to_string (), path);

            if (result) {
                return true;
            }

            if (Xdp.Portal.running_under_flatpak ()) {
                string output = Command.return_output ("bash -c \"test -%s %s &>/dev/null || echo 'path not found'\"".printf (type, path));
                debug ("path_exists test output: %s", output);

                if (!output.contains ("path not found")) {
                    return true;
                }
            }

            debug ("path_exists: Path %s was not found", path);
            return false;
        }

        public static string get_debug_info () {
            var gtk_settings = Gtk.Settings.get_default ();

            var kernel   = Command.return_output ("uname -sri").strip ();
            var distro   = Command.return_output ("lsb_release -ds").strip ();
            var codename = Command.return_output ("lsb_release -cs").strip ();
            var initsys  = Command.return_output ("ps -p 1 -o comm=").strip ();
            var engine   = Command.return_output ("cat %s".printf (Hamachi.CONFIG_PATH));

            return """
Haguichi: %s
Distribution: %s
Codename: %s
Kernel: %s
GLib: %s.%s.%s
GTK: %s.%s.%s
Adwaita: %s
Color scheme: %s
Accent color: %s
High contrast: %s
Theme: %s
Icons: %s
Font: %s
Decoration layout: %s
Locale: %s
Desktop: %s
Session: %s
Flatpak: %s
Hamachi: %s
IP mode: %s
Service: %s
Init: %s
Sudo: %s
Terminal: %s
File manager: %s
Remote desktop: %s
Engine override:
%s""".printf (
                Config.VERSION,
                distro,
                codename,
                kernel,
                Version.MAJOR.to_string (),
                Version.MINOR.to_string (),
                Version.MICRO.to_string (),
                Gtk.MAJOR_VERSION.to_string (),
                Gtk.MINOR_VERSION.to_string (),
                Gtk.MICRO_VERSION.to_string (),
                Adw.VERSION_S,
                app.style_manager.color_scheme.to_string ().replace ("ADW_COLOR_SCHEME_", "").down (),
                app.style_manager.accent_color.to_string ().replace ("ADW_ACCENT_COLOR_", "").down (),
                app.style_manager.high_contrast.to_string (),
                gtk_settings.gtk_theme_name,
                gtk_settings.gtk_icon_theme_name,
                gtk_settings.gtk_font_name,
                gtk_settings.gtk_decoration_layout,
                Environment.get_variable ("LANG"),
                Environment.get_variable ("XDG_CURRENT_DESKTOP"),
                Environment.get_variable ("XDG_SESSION_TYPE"),
                Xdp.Portal.running_under_flatpak ().to_string (),
                Hamachi.version,
                Hamachi.ip_version,
                Hamachi.service,
                initsys,
                Command.sudo,
                Command.exists (Command.terminal)       ? Command.terminal       : null,
                Command.exists (Command.file_manager)   ? Command.file_manager   : null,
                Command.exists (Command.remote_desktop) ? Command.remote_desktop : null,
                engine
            ).chug ();
        }
    }
}
