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
    public class CustomCommand : Object {
        public string is_active;
        public string is_default;
        public string label;
        public string cmd_ipv4;
        public string cmd_ipv6;
        public string priority;

        public CustomCommand (string _is_active, string _is_default, string _label, string _cmd_ipv4, string _cmd_ipv6, string _priority) {
            is_active  = _is_active;
            is_default = _is_default;
            label      = _label;
            cmd_ipv4   = _cmd_ipv4;
            cmd_ipv6   = _cmd_ipv6;
            priority   = _priority;
        }

        public bool exists () {
            return Command.exists (Command.replace_variables (cmd_ipv4, "", "", "")) ||
                   Command.exists (Command.replace_variables (cmd_ipv6, "", "", ""));
        }

        public bool enabled_for_member (Member member) {
            // Enabled if member is online or command doesn't use address variable
            return member.status.status_int == 1 || (!cmd_ipv4.contains ("%A") && !cmd_ipv6.contains ("%A"));
        }

        public string return_for_member (Member member) {
            string command = "";
            string address = "";

            if (Hamachi.ip_version == "Both") {
                if (priority == "IPv4") {
                    command = (member.ipv4 != null) ? cmd_ipv4    : cmd_ipv6;
                    address = (member.ipv4 != null) ? member.ipv4 : member.ipv6;
                }
                if (priority == "IPv6") {
                    command = (member.ipv6 != null) ? cmd_ipv6    : cmd_ipv4;
                    address = (member.ipv6 != null) ? member.ipv6 : member.ipv4;
                }
            } else if (Hamachi.ip_version == "IPv4") {
                command = cmd_ipv4;
                address = member.ipv4;
            } else if (Hamachi.ip_version == "IPv6") {
                command = cmd_ipv6;
                address = member.ipv6;
            }

            return Command.replace_variables (command, address, member.nick, member.id);
        }
    }
}
