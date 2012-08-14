/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2012 Stephen Brandt <stephen@stephenbrandt.com>
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

    public class AddEditCommand : Dialog
    {
        
        private string Mode;
        private CommandsEditor Editor;
        
        private string CommandIcon;
        
        private Menu IconMenu;
        private Image IconImg;
        private ToggleButton IconBut;
        
        private Label labelLabel;
        private Entry labelEntry;
        
        private Label commandIPv4Label;
        private Entry commandIPv4Entry;
        
        private Label commandIPv6Label;
        private Entry commandIPv6Entry;
        
        private Label priorityLabel;
        private RadioButton priorityGroup;
        private RadioButton priorityIPv4;
        private RadioButton priorityIPv6;
        
        private Button cancelButton;
        private Button saveButton;
        
        
        public AddEditCommand ( string mode, CommandsEditor editor, string icon, string label, string commandIPv4, string commandIPv6, string priority ) : base ()
        {
            
            this.Mode            = mode;
            this.Editor          = editor;
            this.CommandIcon     = icon;
            
            this.TransientFor    = Haguichi.preferencesWindow;
            this.IconList        = MainWindow.appIcons;
            this.Modal           = true;
            this.HasSeparator    = false;
            this.Resizable       = false;
            this.SkipTaskbarHint = true;
            this.BorderWidth     = 8;
            this.DeleteEvent    += OnDeleteEvent;
            
            this.ActionArea.Destroy ();
            
            
            if ( Mode == "Add" )
            {
                this.Title = TextStrings.addCommandTitle;
                
                saveButton = new Button ( Stock.Add );
                saveButton.Clicked += AddCommand;
            }
            if ( Mode == "Edit" )
            {
                this.Title = TextStrings.editCommandTitle;
                
                saveButton = new Button ( Stock.Save );
                saveButton.Clicked += SaveCommand;
            }
            saveButton.CanDefault = true;
            saveButton.Realized += CheckEntryLengths;
            
            cancelButton = new Button ( Stock.Cancel );
            cancelButton.Clicked += Dismiss;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( cancelButton );
            buttonBox.Add ( saveButton );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            buttonBox.BorderWidth = 2;
            
            
            labelEntry = new Entry ();
            labelEntry.ActivatesDefault = true;
            labelEntry.WidthChars = 32;
            labelEntry.Text = label;
            labelEntry.Changed += CheckEntryLengths;
            
            labelLabel = new Label ( TextStrings.labelLabel + " " );
            labelLabel.Xalign = 0;
            labelLabel.MnemonicWidget = labelEntry;
            
            
            string info = TextStrings.commandInfo;
            info = info.Replace ( "%N", "%N" );
            info = info.Replace ( "%A", "%A" );
            
            
            commandIPv4Entry = new Entry ();
            commandIPv4Entry.ActivatesDefault = true;
            commandIPv4Entry.WidthChars = 32;
            commandIPv4Entry.Text = commandIPv4;
            commandIPv4Entry.TooltipMarkup = info;
            commandIPv4Entry.Changed += CheckEntryLengths;
            
            commandIPv4Label = new Label ( TextStrings.commandIPv4Label + " " );
            commandIPv4Label.Xalign = 0;
            commandIPv4Label.MnemonicWidget = commandIPv4Entry;
            
            
            commandIPv6Entry = new Entry ();
            commandIPv6Entry.ActivatesDefault = true;
            commandIPv6Entry.WidthChars = 32;
            commandIPv6Entry.Text = commandIPv6;
            commandIPv6Entry.TooltipMarkup = info;
            commandIPv6Entry.Changed += CheckEntryLengths;
            
            commandIPv6Label = new Label ( TextStrings.commandIPv6Label + " " );
            commandIPv6Label.Xalign = 0;
            commandIPv6Label.MnemonicWidget = commandIPv6Entry;
            
            
            priorityGroup = new RadioButton ( "priority" );
            
            priorityIPv4 = new RadioButton ( priorityGroup, "IPv4" );
            priorityIPv6 = new RadioButton ( priorityGroup, "IPv6" );
            if ( priority == "IPv6" )
            {
                priorityIPv6.Active = true;
            }
            else
            {
                priorityIPv4.Active = true;
            }
            
            HBox priorityBox = new HBox ();
            priorityBox.Add ( priorityIPv4 );
            priorityBox.Add ( priorityIPv6 );
            
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( priorityBox [ priorityIPv4 ] ) );
            bc3.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( priorityBox [ priorityIPv6 ] ) );
            bc4.Expand = false;
            bc4.Padding = 9;
            
            priorityLabel = new Label ( TextStrings.priorityLabel + " " );
            priorityLabel.Xalign = 0;
            priorityLabel.MnemonicWidget = priorityGroup;
            
            
            IconMenu = new Menu ();
            IconMenu.Deactivated += delegate
            {
                IconBut.Active = false;  
                IconBut.HasTooltip = true;
            };
            
            IconImg = new Image ();
            IconImg.SetFromIconName ( this.CommandIcon, IconSize.Dialog );
            
            IconBut = new ToggleButton ();
            IconBut.Image = IconImg;
            IconBut.TooltipText = TextStrings.chooseIconTip;
            IconBut.Clicked += Popup;
            
            Fill ();
            
            
            Table table = new Table ( 4, 3, false );
            table.RowSpacing = 6;
            table.ColumnSpacing = 6;
            table.BorderWidth = 2;
            table.Attach ( labelLabel,       0, 1, 0, 1 );
            table.Attach ( labelEntry,       1, 2, 0, 1 );
            table.Attach ( commandIPv4Label, 0, 1, 1, 2 );
            table.Attach ( commandIPv4Entry, 1, 2, 1, 2 );
            table.Attach ( commandIPv6Label, 0, 1, 2, 3 );
            table.Attach ( commandIPv6Entry, 1, 2, 2, 3 );
            table.Attach ( priorityLabel,    0, 1, 3, 4 );
            table.Attach ( priorityBox,      1, 2, 3, 4 );
            table.Attach ( IconBut,          2, 3, 0, 2, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0 );
            
            
            this.VBox.Add ( table );
            this.VBox.Add ( buttonBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( this.VBox [ table ] ) );
            bc1.Padding = 3;
            
            
            this.ShowAll ();
            
            saveButton.GrabDefault ();
            
        }
        
        
        private void Fill ()
        {
            
            uint columns     = 6;
            uint countLeft   = 0;
            uint countRight  = 1;
            uint countTop    = 0;
            uint countBottom = 1;
            
            string [] iconNames = { "folder-remote",
                                    "network-workgroup",
                                    "network-server",
                                    "printer",
                                    "preferences-desktop-remote-desktop",
                                    "application-x-remote-connection",
                                    "applications-internet",
                                    "computer",
                                    "go-home",
                                    "user-home",
                                    "folder-open",
                                    "utilities-terminal",
                                    "utilities-system-monitor",
                                    "dialog-password",
                                    "system-search",
                                    "audio-x-generic",
                                    "video-x-generic",
                                    "x-office-address-book",
                                    "package-x-generic",
                                    "input-gaming",
                                    "system-users",
                                    "drive-harddisk",
                                    "drive-optical",
                                    "drive-removable-media",
                                    "camera-web",
                                    "audio-input-microphone",
                                    "network-wired",
                                    "network-wireless",
                                    "modem",
                                    "call-start",
                                    "call-stop",
                                    "document-send",
                                    "face-cool",
                                    "face-devilish",
                                    "system-shutdown" };
            
            foreach ( string iconName in iconNames )
            {
                //Console.WriteLine ( "left: " + countLeft + "  right: " + countRight + "  top: " + countTop + "  bottom: " + countBottom );
                
                Image img = new Image ();
                img.SetFromIconName ( iconName, IconSize.LargeToolbar );
                
                MenuItem icon = new MenuItem ();
                icon.Add ( img );
                icon.TooltipText = iconName;
                icon.HasTooltip = false;
                icon.Activated += delegate
                {
                    SetIcon ( icon.TooltipText );
                };
                
                IconMenu.Attach ( icon, countLeft, countRight, countTop, countBottom );
                
                if ( countLeft < columns )
                {
                    countLeft   += 1;
                    countRight  += 1;
                }
                else
                {
                    if ( countTop < columns )
                    {
                        countTop    += 1;
                        countBottom += 1;
                    }
                    else
                    {
                        countTop     = 0;
                        countBottom  = 1;
                    }
                    
                    countLeft    = 0;
                    countRight   = 1;
                }
            }
            
            MenuItem none = new MenuItem ( TextStrings.noIconLabel );
            none.Activated += delegate
            {
                SetIcon ( "none" );
            };
            
            IconMenu.Attach ( new SeparatorMenuItem (), 0, 7, 5, 6 );
            IconMenu.Attach ( none, 0, 7, 6, 7 );
            IconMenu.ShowAll ();
            
        }
        
        
        private void Popup ( object obj, EventArgs args )
        {
            
            IconMenu.Popup ( null, null, PositionMenu, 0, Gtk.Global.CurrentEventTime );
            IconBut.HasTooltip = false;
            
        }
        
        
        public void PositionMenu ( Menu menu, out int x, out int y, out bool push_in )
        {
            
            this.GdkWindow.GetPosition ( out x, out y );
            x += IconBut.Allocation.X;
            y += IconBut.Allocation.Bottom;
            
            push_in = true;
            
        }
        
        
        public void SetIcon ( string icon )
        {
            
            this.CommandIcon = icon;
            
            IconImg.SetFromIconName ( this.CommandIcon, IconSize.Dialog );
            IconBut.Image = IconImg;
            
        }
        
        
        private void CheckEntryLengths ( object obj, EventArgs args )
        {
            
            string label       = labelEntry.GetChars ( 0, -1 );
            string commandIPv4 = commandIPv4Entry.GetChars ( 0, -1 );
            string commandIPv6 = commandIPv6Entry.GetChars ( 0, -1 );
            
            if ( ( label.Length > 0 ) &&
                 ( ( commandIPv4.Length > 0 ) ||
                   ( commandIPv6.Length > 0 ) ) )
            {
                saveButton.Sensitive = true;
                saveButton.GrabDefault ();
            }
            else
            {
                saveButton.Sensitive = false;
            }
            
        }
        
        
        private void OnDeleteEvent ( object obj, DeleteEventArgs args )
        {
            
            Dismiss ();
            args.RetVal = true;
            
        }
        

        private void Dismiss ()
        {
            
            this.Destroy ();
            
        }
        
        
        private void Dismiss ( object obj, EventArgs args )
        {
            
            Dismiss ();
            
        }
        
        
        private string GetPriority ()
        {
            
            if ( priorityIPv6.Active )
            {
                return "IPv6";
            }
            else
            {
                return "IPv4";
            }
            
        }
        
        
        private void AddCommand ( object obj, EventArgs args )
        {
            
            Editor.InsertCommand ( this.CommandIcon, labelEntry.Text, commandIPv4Entry.Text, commandIPv6Entry.Text, GetPriority () );
            Dismiss ();
            
        }
        
        
        private void SaveCommand ( object obj, EventArgs args )
        {
            
            Editor.UpdateSelectedCommand ( this.CommandIcon, labelEntry.Text, commandIPv4Entry.Text , commandIPv6Entry.Text, GetPriority () );
            Dismiss ();
            
        }
        
    }

}
