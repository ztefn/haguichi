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
                parsed_nick = Environment.get_user_name();
            } else if (nick == "%REALNAME") {
                parsed_nick = Environment.get_real_name();
            } else if (nick == "%HOSTNAME") {
                parsed_nick = Environment.get_host_name();
            }

            return parsed_nick;
        }

        public static unowned Gtk.IconTheme get_icon_theme () {
            return Gtk.IconTheme.get_for_display (Gdk.Display.get_default ());
        }

        public static string get_available_theme_icon (string[] icon_names) {
            // Check each icon name in the list for existence, and return immediately if it does
            foreach (string icon_name in icon_names) {
                if (get_icon_theme ().has_icon (icon_name)) {
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
    }
}
