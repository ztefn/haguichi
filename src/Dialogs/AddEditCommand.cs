/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2010 Stephen Brandt <stephen@stephenbrandt.com>
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
        
        private VBox   iconBox;
        private Image  iconImg;
        private Button iconBut;
        
        private Label labelLabel;
        private Entry labelEntry;
        private HBox  labelBox;
        
        private Label commandLabel;
        private Entry commandEntry;
        private HBox  commandBox;
        
        private Button cancelBut;
        private Button okBut;
        
        
        public AddEditCommand ( string mode, CommandsEditor editor, string icon, string label, string command ) : base ()
        {
            
            this.Mode = mode;
            this.Editor = editor;
            
            this.CommandIcon = icon;
            
            this.Modal = true;
            this.HasSeparator = false;
            this.Resizable = false;
            this.SkipTaskbarHint = true;
            this.IconList = MainWindow.appIcons;
            this.BorderWidth = 9;
            this.ActionArea.Destroy ();
            this.DeleteEvent += OnDeleteEvent;
            
            
            if ( Mode == "Add" )
            {
                this.Title = TextStrings.addCommandTitle;
                
                okBut = new Button ( Stock.Add );
                okBut.Clicked += AddCommand;
            }
            if ( Mode == "Edit" )
            {
                this.Title = TextStrings.editCommandTitle;
                
                okBut = new Button ( Stock.Save );
                okBut.Clicked += SaveCommand;
            }
            okBut.CanDefault = true;
            okBut.Realized += CheckEntryLengths;
            
            cancelBut = new Button ( Stock.Cancel );
            cancelBut.Clicked += Dismiss;
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( cancelBut );
            buttonBox.Add ( okBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            
            labelEntry = new Entry ();
            labelEntry.ActivatesDefault = true;
            labelEntry.WidthChars = 39;
            labelEntry.Text = label;
            labelEntry.Changed += CheckEntryLengths;
            
            labelLabel = new Label ( TextStrings.labelLabel + "  " );
            labelLabel.Xalign = 0;
            labelLabel.MnemonicWidget = labelEntry;
            
            labelBox = new HBox ();
            labelBox.Add ( labelLabel );
            labelBox.Add ( labelEntry );
            
            Box.BoxChild labelBoxC = ( ( Box.BoxChild ) ( labelBox [ labelEntry ] ) );
            labelBoxC.Expand = false;
            
            
            commandEntry = new Entry ();
            commandEntry.ActivatesDefault = true;
            commandEntry.WidthChars = 39;
            commandEntry.Text = command;
            commandEntry.Changed += CheckEntryLengths;
            
            commandLabel = new Label ( TextStrings.commandLabel + "  " );
            commandLabel.Xalign = 0;
            commandLabel.MnemonicWidget = commandEntry;
            
            commandBox = new HBox ();
            commandBox.Add ( commandLabel );
            commandBox.Add ( commandEntry );
            
            Box.BoxChild commandBoxC = ( ( Box.BoxChild ) ( commandBox [ commandEntry ] ) );
            commandBoxC.Expand = false;
            
            
            Label empty = new Label ();
            
            string info = TextStrings.commandInfo;
            info = info.Replace ( "%N", "<b>%N</b>" );
            info = info.Replace ( "%A", "<b>%A</b>" );
            
            Label infoLabel = new Label ();
            infoLabel.Markup = String.Format ( "<span size='smaller'>{0}</span>", info );
            infoLabel.Xalign = 0;
            infoLabel.WidthChars = 40;
            
            HBox infoBox = new HBox ();
            infoBox.Add ( empty );
            infoBox.Add ( infoLabel );
            
            Box.BoxChild infoBoxC = ( ( Box.BoxChild ) ( infoBox [ infoLabel ] ) );
            infoBoxC.Expand = false;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( labelBox );
            vbox.Add ( commandBox );
            vbox.Add ( infoBox );
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( vbox [ labelBox ] ) );
            bc8.Padding = 3;
            bc8.Expand = false;
            
            Box.BoxChild bc9 = ( ( Box.BoxChild ) ( vbox [ commandBox ] ) );
            bc9.Padding = 3;
            bc9.Expand = false;
            
            
            iconImg = new Image ();
            iconImg.SetFromIconName ( this.CommandIcon, IconSize.Dialog );
            
            iconBut = new Button ();
            iconBut.Image = iconImg;
            iconBut.TooltipText = TextStrings.chooseIconTip;
            iconBut.Clicked += ChooseIcon;
            
            iconBox = new VBox ();
            iconBox.Add ( iconBut );
            
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( iconBox [ iconBut ] ) );
            bc3.Padding = 3;
            bc3.Expand = false;
            
            
            HBox hbox = new HBox ();
            hbox.Add ( vbox );
            hbox.Add ( iconBox );

            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
            bc5.Padding = 6;
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( hbox [ iconBox ] ) );
            //bc2.Padding = 3;
            
            
            this.VBox.Add ( hbox );
            this.VBox.Add ( buttonBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( this.VBox [ hbox ] ) );
            //bc1.Padding = 3;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( this.VBox [ buttonBox ] ) );
            bc4.Padding = 3;
            bc4.Expand = false;
            
            
            this.ShowAll ();
            
            okBut.GrabDefault ();
            
        }

        
        private void ChooseIcon ( object o, EventArgs args )
        {
            Dialogs.ChooseIcon iconDialog = new Dialogs.ChooseIcon ( this, this.CommandIcon );
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
                okBut.Sensitive = true;
                okBut.GrabDefault ();
            }
            else
            {
                okBut.Sensitive = false;
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
