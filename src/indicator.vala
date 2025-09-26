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

using Config;

namespace Haguichi {
    public class Indicator {
        private Menu   menu;
        private int    icon_num;
        private string icon_postfix;

        public StatusNotifierItem item;

        public Indicator () {
            // Only on specific desktops we use symbolic icons
            if (current_desktop.contains ("GNOME") ||
                current_desktop == "COSMIC" ||
                current_desktop == "MATE" ||
                current_desktop == "Pantheon" ||
                current_desktop == "X-Cinnamon") {
                icon_postfix = "-symbolic";
            }

            item = new StatusNotifierItem () {
                id        = APP_ID + ".indicator",
                category  = "ApplicationStatus",
                title     = APP_NAME,
                icon_name = APP_ID + "-disconnected" + icon_postfix,
                status    = "Active",
                is_menu   = true,
            };
            item.scroll.connect ((delta, orientation) => {
                debug ("scroll delta %d, orientation %s", delta, orientation);
                if (orientation.down () != "vertical") return;

                // Show window when scrolling up and hide it when scrolling down
                if (delta < 0) {
                    win.present ();
                } else {
                    win.hide ();
                }
            });
            item.notify["host-registered"].connect (() => {
                if (item.host_registered) {
                    item.register ();
                }
            });

            item.init ();
        }

        public bool active {
            get {
                return item.status == "Active";
            }
            set {
                if (value) {
                    item.register ();
                }
                item.status = (value == true) ? "Active" : "Passive";
            }
        }

        private string icon_name {
            get {
                return item.icon_name;
            }
            set {
                item.icon_name = APP_ID + "-" + value + icon_postfix;
            }
        }

        public void update_status (string mode, bool modal, bool visible) {
            set_icon (mode);

            var status_text = (mode == "Connected") ? _("Connected") : (mode == "Connecting") ? _("Connecting") : _("Disconnected");
            item.tool_tip = { APP_ID, {}, APP_NAME, status_text };

            // For available properties see:
            // https://github.com/AyatanaIndicators/libdbusmenu/blob/master/libdbusmenu-glib/dbus-menu.xml

            menu = new Menu ();

            var show = new MenuItem (_("_Show Haguichi"), "app.toggle-window");
            show.set_attribute_value ("toggle-type", "checkmark");
            show.set_attribute_value ("toggle-state", (int) visible);
            show.set_attribute_value ("enabled", !modal);
            menu.append_item (show);

            var separator = new MenuItem (null, null);
            separator.set_attribute_value ("type", "separator");
            menu.append_item (separator);

            if (mode.down ().contains ("connect")) {
                var connect = new MenuItem ((mode == "Connected") ? _("_Disconnect") : _("C_onnect"),
                                            (mode == "Connected") ? "app.disconnect" : "app.connect");
                connect.set_attribute_value ("enabled", (!modal && mode != "Connecting"));
                set_menu_item_icon_name (connect, "network-" + (mode == "Connected" ? "disconnect" : "connect"));
                menu.append_item (connect);

                menu.append_item (separator);

                var join = new MenuItem (_("_Join Network…"), "app.join-network");
                join.set_attribute_value ("enabled", !modal && mode == "Connected");
                set_menu_item_icon_name (join, "list-add");
                menu.append_item (join);

                var create = new MenuItem (_("_Create Network…"), "app.create-network");
                create.set_attribute_value ("enabled", !modal && mode == "Connected");
                set_menu_item_icon_name (create, "list-add");
                menu.append_item (create);

                menu.append_item (separator);

                var info = new MenuItem (_("_Information"), "app.info");
                info.set_attribute_value ("enabled", !modal);
                set_menu_item_icon_name (info, "documentinfo");
                menu.append_item (info);

                menu.append_item (separator);
            }

            var quit = new MenuItem (_("_Quit"), "app.quit");
            set_menu_item_icon_name (quit, "application-exit");
            menu.append_item (quit);

            item.menu_model = menu;
        }

        private void set_menu_item_icon_name (MenuItem menu_item, string icon_name) {
            // Only on KDE icons seems to be commonly used with menu items
            if (current_desktop == "KDE") {
                menu_item.set_attribute_value ("icon-name", icon_name);
            }
        }

        private void set_icon (string mode) {
            // Check if there isn't already an animation going on when connecting
            if (mode == "Connecting" && icon_name.contains ("-connecting-")) {
                return;
            }

            icon_num = 0;

            switch (mode) {
                case "Connecting":
                    Timeout.add (400, switch_icon);
                    break;
                case "Connected":
                    icon_name = "connected";
                    break;
                default:
                    icon_name = "disconnected";
                    break;
            }
        }

        private bool switch_icon () {
            // Stop animation when not connecting anymore
            if (win.get_mode () != "Connecting") {
                return false;
            }

            switch (icon_num) {
                case 0:
                    icon_name = "connecting-1";
                    icon_num = 1;
                    break;
                case 1:
                    icon_name = "connecting-2";
                    icon_num = 2;
                    break;
                default:
                    icon_name = "connecting-3";
                    icon_num = 0;
                    break;
            }

            return true;
        }
    }
}
