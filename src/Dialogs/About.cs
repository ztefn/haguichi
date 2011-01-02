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
    
    public class About : AboutDialog
    {
        
        public About ()
        {
            
            AboutDialog.SetUrlHook   ( new AboutDialogActivateLinkFunc ( OpenUrl   ) );
            AboutDialog.SetEmailHook ( new AboutDialogActivateLinkFunc ( OpenEmail ) );
            
            this.IconList            = MainWindow.appIcons;
            
            this.ProgramName         = TextStrings.appName;
            this.Comments            = TextStrings.appComments;
            this.Version             = TextStrings.appVersion;
            this.License             = TextStrings.appLicense;
            this.Copyright           = TextStrings.appCopyright;
            this.Website             = TextStrings.appWebsite;
            this.WebsiteLabel        = TextStrings.appWebsiteLabel;
            this.Authors             = TextStrings.appAuthors;
            this.TranslatorCredits   = TextStrings.appTranslatorCredits;
            this.Artists             = TextStrings.appArtists;
            
            this.Logo                = MainWindow.appIcons[4];
            
            this.DeleteEvent        += OnDeleteEvent;
            this.Response           += ResponseHandler;
            
        }
        
        
        public void Open ()
        {
            
            this.Show ();
            this.Present ();
            
        }
        
        
        private void OpenUrl ( AboutDialog dialog, string url )
        {
            
            Command.OpenURL ( url );
            
        }
        
        
        private void OpenEmail ( AboutDialog dialog, string email )
        {
            
            Command.OpenURL ( "mailto:" + email );
            
        }
        
        
        private void Dismiss ()
        {
            
            this.Hide ();
            
        }
        
        
        private void ResponseHandler ( object obj, ResponseArgs args )
        {
            
            Dismiss ();
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
    
    }

}
