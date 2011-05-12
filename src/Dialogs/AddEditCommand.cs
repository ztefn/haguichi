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
        
        private Label commandLabel;
        private Entry commandEntry;
        
        private Button cancelButton;
        private Button saveButton;
        
        
        public AddEditCommand ( string mode, CommandsEditor editor, string icon, string label, string command ) : base ()
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
            this.BorderWidth     = 6;
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
            buttonBox.BorderWidth = 3;
            
            
            labelEntry = new Entry ();
            labelEntry.ActivatesDefault = true;
            labelEntry.WidthChars = 36;
            labelEntry.Text = label;
            labelEntry.Changed += CheckEntryLengths;
            
            labelLabel = new Label ( TextStrings.labelLabel + " " );
            labelLabel.Xalign = 0;
            labelLabel.MnemonicWidget = labelEntry;
            
            
            commandEntry = new Entry ();
            commandEntry.ActivatesDefault = true;
            commandEntry.WidthChars = 36;
            commandEntry.Text = command;
            commandEntry.Changed += CheckEntryLengths;
            
            commandLabel = new Label ( TextStrings.commandLabel + " " );
            commandLabel.Xalign = 0;
            commandLabel.MnemonicWidget = commandEntry;
            
            
            string info = TextStrings.commandInfo;
            info = info.Replace ( "%N", "<b>%N</b>" );
            info = info.Replace ( "%A", "<b>%A</b>" );
            
            Label infoLabel = new Label ();
            infoLabel.Markup = String.Format ( "<span size='smaller'>{0}</span>", info );
            infoLabel.Xalign = 0;
            
            
            iconImg = new Image ();
            iconImg.SetFromIconName ( this.CommandIcon, IconSize.Dialog );
            
            iconBut = new Button ();
            iconBut.Image = iconImg;
            iconBut.TooltipText = TextStrings.chooseIconTip;
            iconBut.Clicked += ChooseIcon;
            
            
            Table table = new Table ( 3, 3, false );
            table.RowSpacing = 6;
            table.ColumnSpacing = 6;
            table.BorderWidth = 3;
            table.Attach ( labelLabel, 0, 1, 0, 1 );
            table.Attach ( labelEntry, 1, 2, 0, 1 );
            table.Attach ( commandLabel, 0, 1, 1, 2 );
            table.Attach ( commandEntry, 1, 2, 1, 2 );
            table.Attach ( iconBut, 2, 3, 0, 2, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0 );
            table.Attach ( infoLabel, 1, 3, 2, 3 );
            
            this.VBox.Add ( table );
            this.VBox.Add ( buttonBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( this.VBox [ table ] ) );
            bc1.Padding = 3;
            
            
            this.ShowAll ();
            
            saveButton.GrabDefault ();
            
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
            
            string label   = labelEntry.GetChars ( 0, -1 );
            string command = commandEntry.GetChars ( 0, -1 );
            
            if ( ( label.Length > 0 ) && ( command.Length > 0 ) )
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
        
        
        private void AddCommand ( object obj, EventArgs args )
        {
            
            Editor.InsertCommand ( this.CommandIcon, labelEntry.Text, commandEntry.Text );
            Dismiss ();
            
        }
        
        
        private void SaveCommand ( object obj, EventArgs args )
        {
            
            Editor.UpdateSelectedCommand ( this.CommandIcon, labelEntry.Text, commandEntry.Text );
            Dismiss ();
            
        }
        
    }

}
