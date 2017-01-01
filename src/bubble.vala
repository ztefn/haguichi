/*
 * This file is part of Haguichi, a graphical frontend for Hamachi.
 * Copyright (C) 2007-2017 Stephen Brandt <stephen@stephenbrandt.com>
 *
 * Haguichi is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 */

using Notify;

public class Bubble : Object
{
    public Bubble (string summary, string body)
    {
        Notify.init ("Haguichi");
        
        try
        {
            new Notify.Notification (summary, body, "haguichi").show();
        }
        catch (Error e)
        {
            Debug.log (Debug.domain.ERROR, "Bubble", e.message);
        }
    }
}
