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

public class Status : Object {
    public int    status_int;
    public string status_text;
    public string status_sortable;
    public string message;
    public string connection_type;

    public Status (string status) {
        message = "";
        connection_type = "";
        set_status (status);
    }

    public Status.complete (string status, string? connection, string? _message) {
        message = _message;
        set_connection_type (connection);
        set_status (status);
    }

    public void set_status (string status) {
        if (status == " ") {
            status_int = 0;
            status_text = _("Offline");
            status_sortable = "f";
        } else if (status == "*") {
            status_int = 1;
            status_text = _("Online");

            if (connection_type == _("Relayed")) {
                status_sortable = "b";
            } else {
                status_sortable = "a";
            }
        } else if (status == "x") {
            status_int = 2;
            status_text = _("Unreachable");
            status_sortable = "d";
        } else if (status == "?") {
            status_int = 3;
            status_text = _("Awaiting approval");
            status_sortable = "c";
        } else if (status == "!") {
            status_int = 4;
            status_text = _("Unknown error");
            status_sortable = "e";

            if (message == "IP protocol mismatch between you and peer") {
                status_text = _("Protocol mismatch");
            } else if (message == "This address is also used by another peer") {
                status_text = _("Conflicting address");
            }
        }
    }

    private void set_connection_type (string? connection) {
        connection_type = "";

        if (connection == null) {
            return;
        } else if (connection == "direct") {
            connection_type = _("Direct");
        } else if (connection.contains ("via ")) {
            connection_type = _("Relayed");
        }
    }

    public string get_css_classes () {
        string icon_name = "";

        if (status_int == 0) {
            icon_name = "network-node-offline";
        } if (status_int == 1) {
            if (connection_type == _("Relayed")) {
                icon_name = "network-node-online-relayed";
            } else {
                icon_name = "network-node-online";
            }
        } else if (status_int == 2) {
            icon_name = "network-node-unreachable";
        } else if (status_int == 3) {
            icon_name = "network-node-unapproved";
        } else if (status_int == 4) {
            icon_name = "network-node-error";
        }

        return icon_name;
    }
}
