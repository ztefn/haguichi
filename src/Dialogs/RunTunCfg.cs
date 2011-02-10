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

    public class RunTunCfg : Dialogs.Base
    {
        
        public RunTunCfg () : base ( Haguichi.mainWindow.ReturnWindow (), "", TextStrings.runTuncfgHeading, TextStrings.runTuncfgMessage, "Question" )
        {
            
            Haguichi.modalDialog = this;
            
            this.AddButton ( Stock.Cancel, ResponseType.Cancel );
            
            Button runBut = ( Button ) this.AddButton ( TextStrings.runLabel, ResponseType.Ok );
            runBut.Image = new Image ( Stock.DialogAuthentication, IconSize.Button );
            runBut.GrabFocus ();
            
            CheckButton check = new CheckButton ( TextStrings.checkboxAskBeforeRunningTuncfg );
            check.Active = ( bool ) Config.Client.Get ( Config.Settings.AskBeforeRunningTunCfg );
            check.Toggled += ToggleAsk;
            
            this.AddContent ( check );
            
            Box.BoxChild bc = ( ( Box.BoxChild ) ( this.Contents [ check ] ) );
            bc.Expand = false;
            bc.Padding = 6;
            
            this.Modal           = true;
            this.SkipTaskbarHint = true;
            this.Response       += ResponseHandler;
            
            this.Run ();
            this.Destroy ();
            
        }
        
        
        public void ToggleAsk ( object obj, EventArgs args )
        {
            
            bool ask = !( bool ) Config.Client.Get ( Config.Settings.AskBeforeRunningTunCfg );
            Config.Client.Set ( Config.Settings.AskBeforeRunningTunCfg, ask );
            
        }
        
        
        private void ResponseHandler ( object o, ResponseArgs args )
        {
            
            Haguichi.modalDialog = null;
            
        }
        
    }

}
