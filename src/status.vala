/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2026 Stephen Brandt <stephen@stephenbrandt.com>
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
        switch (status) {
            case " ":
                status_int = 0;
                status_text = _("Offline");
                status_sortable = "f";
                break;
            case "*":
                status_int = 1;
                status_text = _("Online");
                status_sortable = connection_type == _("Relayed") ? "b" : "a";
                break;
            case "x":
                status_int = 2;
                status_text = _("Unreachable");
                status_sortable = "d";
                break;
            case "?":
                status_int = 3;
                status_text = _("Awaiting approval");
                status_sortable = "c";
                break;
            case "!":
                status_int = 4;
                status_text = message == "IP protocol mismatch between you and peer" ? _("Protocol mismatch") :
                              message == "This address is also used by another peer" ? _("Conflicting address") :
                              _("Unknown error");
                status_sortable = "e";
                break;
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
        string icon_name = "network-node-";

        switch (status_int) {
            case 0:
                return icon_name + "offline";
            case 1:
                return icon_name + "online" + (connection_type == _("Relayed") ? "-relayed" : "");
            case 2:
                return icon_name + "unreachable";
            case 3:
                return icon_name + "unapproved";
            default:
                return icon_name + "error";
        }
    }
}
