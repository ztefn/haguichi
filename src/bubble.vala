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
    public class Bubble : Object {
        private Notification notification;

        public Bubble (string summary, string? body) {
            notification = new Notification (summary);

            if (body != null) {
                notification.set_body (body);
            }

            // On elementary OS set_icon is only used to set a badge icon,
            // the app icon itself is automatically set based on application ID
            // https://docs.elementary.io/develop/apis/notifications#badge-icons
            if (Haguichi.current_desktop != "Pantheon") {
                notification.set_icon (new ThemedIcon (Config.APP_ID));
            }
        }

        public void show () {
            Haguichi.app.send_notification (null, notification);
        }

        public bool server_supports_actions () {
            // Unity desktop running NotifyOSD server doesn't support actions
            // https://wiki.ubuntu.com/NotifyOSD
            return Haguichi.current_desktop.has_prefix ("Unity") ? false : true;
        }

        public void add_reconnect_action () {
            if (server_supports_actions ()) {
                notification.add_button (_("Reconnect"), "app.connect");
            }
        }

        public void add_approve_reject_actions (string client_id, string[] network_ids) {
            if (server_supports_actions ()) {
                string[] approve_args = {"approve", client_id};
                string[] reject_args  = {"reject",  client_id};

                foreach (string network_id in network_ids) {
                    approve_args += network_id;
                    reject_args  += network_id;
                }

                notification.add_button_with_target_value (Utils.remove_mnemonics (_("_Approve")), "app.approve-reject", approve_args);
                notification.add_button_with_target_value (Utils.remove_mnemonics (_("_Reject")),  "app.approve-reject", reject_args);
            }
        }
    }
}
