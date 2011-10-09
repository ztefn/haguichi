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

    public class AddEditCommand : Dialog
    {
        
        private string Mode;
        private CommandsEditor Editor;
        
        private string CommandIcon;
        
        private Image  iconImg;
        private Button iconBut;
        
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
            
            
            iconImg = new Image ();
            iconImg.SetFromIconName ( this.CommandIcon, IconSize.Dialog );
            
            iconBut = new Button ();
            iconBut.Image = iconImg;
            iconBut.TooltipText = TextStrings.chooseIconTip;
            iconBut.Clicked += ChooseIcon;
            
            
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
            table.Attach ( iconBut,          2, 3, 0, 2, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0 );
            
            
            this.VBox.Add ( table );
            this.VBox.Add ( buttonBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( this.VBox [ table ] ) );
            bc1.Padding = 3;
            
            
            this.ShowAll ();
            
            saveButton.GrabDefault ();
            
            if ( Hamachi.ApiVersion <= 2 )
            {
                commandIPv4Label.TextWithMnemonic = TextStrings.commandLabel;
                
                commandIPv6Label.Hide ();
                commandIPv6Entry.Hide ();
                
                priorityLabel.Hide ();
                priorityBox.Hide ();
            }
            
        }

        
        private void ChooseIcon ( object o, EventArgs args )
        {
            
            new Dialogs.ChooseIcon ( this, this.CommandIcon );
            
        }
        
        
        public void SetIcon ( string icon )
        {
            
            this.CommandIcon = icon;
            
            iconImg.SetFromIconName ( this.CommandIcon, IconSize.Dialog );
            iconBut.Image = iconImg;
            
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
        
        
        private void OnDeleteEvent (object obj, DeleteEventArgs args )
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
