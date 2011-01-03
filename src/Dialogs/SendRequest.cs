/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2011 Stephen Brandt <stephen@stephenbrandt.com>
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

    public class SendRequest : Dialogs.Base
    {

        private string Name;
        private string Password;
        
        
        public SendRequest ( string header, string message, string icon, string name, string password ) : base ( "", header, message, icon )
        {
            
            this.Name      = name;
            this.Password  = password;
            
            this.AddButton ( Stock.Cancel, ResponseType.Cancel );
            this.AddButton ( TextStrings.sendRequestLabel, ResponseType.Ok );
            
            this.SkipTaskbarHint = true;
            
            this.Response += ResponseHandler;
            this.Run ();
            this.Destroy ();
            
        }
        
        
        private void ResponseHandler ( object o, ResponseArgs args )
        {
            
            if ( args.ResponseId == ResponseType.Ok )
            {
                string output = Hamachi.SendJoinRequest ( this.Name, this.Password );
            }
            
        }

    }
    
}
