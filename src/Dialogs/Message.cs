/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2014 Stephen Brandt <stephen@stephenbrandt.com>
 * 
 * Haguichi is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 * 
 * Haguichi is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Haguichi; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using Gtk;


namespace Dialogs
{

    public class Message : Dialogs.Base
    {
        
        public Message ( Window parent, string header, string message, string icon, string output ) : base ( parent, "", header, message, icon )
        {
            
            Button okBut = ( Button ) this.AddButton ( Stock.Ok, ResponseType.Ok );
            okBut.GrabDefault ();
            
            if ( output != null )
            {
                TextView textview = new TextView ();
                textview.Editable = false;
                textview.LeftMargin = 3;
                textview.RightMargin = 3;
                textview.PixelsAboveLines = 3;
                textview.PixelsBelowLines = 3;
                
                TextBuffer buffer = textview.Buffer;
                TextIter iter = buffer.GetIterAtOffset ( 0 );
                buffer.Insert ( ref iter, output );
                
                ScrolledWindow sw = new ScrolledWindow ();
                sw.ShadowType = ShadowType.In;
                sw.SetPolicy ( PolicyType.Never, PolicyType.Automatic );
                sw.Add ( textview );
                
                Expander expander = new Expander ( TextStrings.hamachiOutput );
                expander.Add ( sw );
                expander.ShowAll ();
                
                this.AddContent ( expander );
            }
            
            
            this.TransientFor    = parent;
            this.Modal           = true;
            this.SkipTaskbarHint = true;
            
            this.Run ();
            this.Destroy ();
            
        }

    }
    
}
