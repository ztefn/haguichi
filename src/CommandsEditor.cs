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
using System.Collections;
using Gtk;
using Mono.Unix;

    
public class CommandsEditor : VBox
{
    
    private VBox vbox;
    
    private ListStore store;
    private TreeIter iter;
    private TreeView tv;
    
    private CellRendererPixbuf iconCell;
    private CellRendererText textCell;
    private CellRendererToggle toggleCell;
    
    private Button revertBut;
    private Button defaultBut;
    private Button addBut;
    private Button editBut;
    private Button removeBut;
    private Button upBut;
    private Button downBut;
    
    private int activeColumn;
    private int defaultColumn;
    private int iconPixColumn;
    private int iconStringColumn;
    private int labelColumn;
    private int commandColumn;
    private int viewColumn;
    
    
    public CommandsEditor ()
    {
        
        activeColumn     = 0;
        defaultColumn    = 1;
        iconPixColumn    = 2;
        iconStringColumn = 3;
        labelColumn      = 4;
        commandColumn    = 5;
        viewColumn       = 6;
        
        store = new ListStore ( typeof ( bool       ),     // Active
                                typeof ( bool       ),     // Default
                                typeof ( Gdk.Pixbuf ),     // Icon (pixbuf)
                                typeof ( string     ),     // Icon (string)
                                typeof ( string     ),     // Label
                                typeof ( string     ),     // Command
                                typeof ( string     ) );   // View

        tv = new TreeView (store);

        iconCell = new CellRendererPixbuf ();
        
        textCell = new CellRendererText ();

        toggleCell = new CellRendererToggle ();
        toggleCell.Activatable = true;
        toggleCell.Xpad = 6;
        //toggleCell.Ypad = 6;
        toggleCell.Toggled += EnableCommandToggled;
        
        
        TreeViewColumn column1 = new TreeViewColumn ();
        column1.Title = TextStrings.available;
        
        TreeViewColumn column2 = new TreeViewColumn ();
        column2.Title = TextStrings.command;
        
        column1.PackStart ( toggleCell, false );
        
        column2.PackStart ( iconCell, false );
        column2.PackStart ( textCell, true );
        
        column2.SetCellDataFunc ( textCell, new CellLayoutDataFunc ( TextCellLayout ) );
        column1.SetCellDataFunc ( toggleCell, new CellLayoutDataFunc ( ToggleCellLayout ) );
        
        column1.AddAttribute ( toggleCell, "active", activeColumn );
        
        column2.AddAttribute ( iconCell, "pixbuf", iconPixColumn );
        column2.AddAttribute ( textCell, "text", labelColumn );
 
        Label label = new Label ( TextStrings.customizeCommands + "  " );
        label.Xalign = 0;
        label.Xpad = 3;
        label.Ypad = 6;
        label.MnemonicWidget = tv;
        
        tv.AppendColumn ( column1 );
        tv.AppendColumn ( column2 );

        tv.HeadersVisible = false;
        tv.RulesHint = true;
        tv.Reorderable = true;
        tv.EnableSearch = false;
        tv.DragEnd += HandleDragEnd;
        tv.Realized += SetButtonSensitivity;
        tv.ButtonReleaseEvent += SetButtonSensitivity;
        tv.KeyReleaseEvent += SetButtonSensitivity;
        tv.RowActivated += OnRowActivate;
        
        ScrolledWindow sw = new ScrolledWindow ();
           
        //sw.BorderWidth = 6;
        sw.ShadowType = ShadowType.Out;
        sw.Add (tv);
        sw.Show ();
        
        
        revertBut = new Button ( Stock.RevertToSaved ); 
        revertBut.TooltipText = TextStrings.revertTip;
        revertBut.Clicked += RevertCommands;
        
        defaultBut = new Button ( TextStrings.defaultLabel );
        defaultBut.Clicked += SetDefault;
        
        addBut = new Button ( Stock.Add );
        addBut.Clicked += AddCommand;
        
        editBut = new Button ( Stock.Edit );
        editBut.Clicked += EditCommand;
        
        removeBut  = new Button ( Stock.Remove );
        removeBut.Clicked += RemoveCommand;
        
        upBut = new Button ( new Image ( Stock.GoUp, IconSize.Button ) );
        upBut.TooltipText = TextStrings.moveUpTip;
        upBut.Clicked += MoveUp;
        
        downBut = new Button ( new Image ( Stock.GoDown, IconSize.Button ) );
        downBut.TooltipText = TextStrings.moveDownTip;
        downBut.Clicked += MoveDown;
        
        HButtonBox buttonBox = new HButtonBox ();
        //buttonBox.Add ( defaultBut );
        buttonBox.Add ( revertBut );
        buttonBox.Layout = ButtonBoxStyle.Start;
        buttonBox.Spacing = 6;
        
        vbox = new VBox ();
        //vbox.Add ( label );
        vbox.Add ( sw );
        vbox.Add ( buttonBox );
        
        Box.BoxChild bc1 = ( ( Box.BoxChild ) ( vbox [ sw ] ) );
        //bc1.Padding = 6;
        
        Box.BoxChild bc2 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
        bc2.Padding = 6;
        bc2.Expand = false;
        
        
        HBox updown = new HBox ();
        updown.Add ( upBut );
        updown.Add ( downBut );
        updown.Spacing = 6;
        
        VButtonBox updownBox = new VButtonBox ();
        updownBox.Add ( addBut );
        updownBox.Add ( editBut );
        updownBox.Add ( removeBut );
        updownBox.Add ( defaultBut );
        updownBox.Add ( updown );
        updownBox.Layout = ButtonBoxStyle.Start;
        updownBox.Spacing = 6;
        
        HBox hbox = new HBox ();
        
        hbox.Add ( vbox );
        hbox.Add ( updownBox );
        
        Box.BoxChild bc5 = ( ( Box.BoxChild ) ( hbox [ vbox ] ) );
        bc5.Padding = 3;
        
        this.Add ( label );
        this.Add ( hbox );
        
        Box.BoxChild bc3 = ( ( Box.BoxChild ) ( this [ label ] ) );
        bc3.Expand = false;
        
        Box.BoxChild bc4 = ( ( Box.BoxChild ) ( hbox [ updownBox ] ) );
        bc4.Padding = 3;
        bc4.Expand = false;
        
        this.BorderWidth = 9;
        
        Fill ();

    }
    
    
    private void MoveUp ( object o, EventArgs e )
    {
        
        /*
         *  Shamelessly taken from F-Spot (FSpot.UI.Dialog.SelectionRatioDialog.cs)
         */
        
        TreeIter selected;
        TreeModel model;
        
        if ( tv.Selection.GetSelected ( out model, out selected ) )
        {
            //no IterPrev :(
            TreeIter prev;
            TreePath path = model.GetPath ( selected );
            
            if ( path.Prev () )
            {
                if ( model.GetIter ( out prev, path ) )
                {
                    ( model as ListStore ).Swap ( prev, selected );
                }
            }
            
        }
        
        UpdateCommands ();
        
    }
    
    
    private void MoveDown ( object o, EventArgs e )
    {
        
        /*
         *  Shamelessly taken from F-Spot (FSpot.UI.Dialog.SelectionRatioDialog.cs)
         */
        
        TreeIter selected;
        TreeModel model;
        
        if ( tv.Selection.GetSelected ( out model, out selected ) )
        {
            TreeIter next = selected;
            
            if ( ( model as ListStore ).IterNext ( ref next ) )
            {
                ( model as ListStore ).Swap ( selected, next );
            }
        }
        
        UpdateCommands ();
        
    }
    
    
    private void RevertCommands ( object o, EventArgs args )
    {
        
        string label   = Stock.RevertToSaved;
        string heading = String.Format ( TextStrings.revertHeading );
        string message = String.Format ( TextStrings.revertMessage );
            
        Dialogs.Confirm dlg = new Dialogs.Confirm ( heading, message, "Question", label, null );
        
        if ( dlg.response == "Ok" )
        {
            Config.Client.Set ( Config.Settings.CustomCommands, Config.Settings.DefaultCommands );
            
            /*
             * Giving GConf some time to update before we refill the list
             */
            GLib.Timeout.Add ( 100, new GLib.TimeoutHandler ( FillAfterTimeout ) );
        }
        
    }
    
    
    private bool FillAfterTimeout ()
    {
        
        store.Clear ();
        Fill ();
        UpdateCommands ();
        
        return false;
        
    }
    
    
    private void SetDefault ( object o, EventArgs args )
    {
        
        TreeIter iter = new TreeIter ();
        
        if ( store.GetIterFirst ( out iter ) )                         // First command
        {
            store.SetValue ( iter, defaultColumn , false );
            
            while ( store.IterNext ( ref iter ) )                      // All other commands
            {
                store.SetValue ( iter, defaultColumn , false );
            }
        }
        
        
        TreeIter selected;
        TreeModel model;
        
        if ( tv.Selection.GetSelected ( out model, out selected ) )
        {            
            store.SetValue ( selected, defaultColumn , true );
        }
        
        UpdateCommands ();
        
    }
    
    
    private void RemoveCommand  ( object o, EventArgs args )
    {
        
        TreeIter selected;
        TreeModel model;
        
        if ( tv.Selection.GetSelected ( out model, out selected ) )
        {
            store.Remove ( ref selected );
        }
        
        UpdateCommands ();
        
    }
    
    
    public void UpdateSelectedCommand ( string icon, string label, string command )
    {
        
        TreeIter selected;
        TreeModel model;
        
        if ( tv.Selection.GetSelected ( out model, out selected ) )
        {
            Gdk.Pixbuf pix = GetIconPixbuf ( icon );
            
            store.SetValue ( selected, iconPixColumn,    pix     );
            store.SetValue ( selected, iconStringColumn, icon    );
            store.SetValue ( selected, labelColumn,      label   );
            store.SetValue ( selected, commandColumn,    command );
            store.SetValue ( selected, viewColumn,       label   );
        }
        
        UpdateCommands ();
        
    }
    
    
    public void InsertCommand ( string icon, string label, string command )
    {
        
        iter = store.AppendValues ( true, false, GetIconPixbuf ( icon ), icon, label, command, Catalog.GetString ( label ) );
        
        UpdateCommands ();
        
    }
    
    
    private Gdk.Pixbuf GetIconPixbuf ( string iconName )
    {
     
        Gdk.Pixbuf iconPix = Gnome.IconTheme.Default.LoadIcon ( "system-run", 24, IconLookupFlags.GenericFallback );
        
        try { iconPix = Gnome.IconTheme.Default.LoadIcon ( "gnome-run", 24, IconLookupFlags.GenericFallback ); }
        catch {}
        
        try { iconPix = Gnome.IconTheme.Default.LoadIcon ( iconName, 24, IconLookupFlags.GenericFallback ); }
        catch {}
           
        return iconPix;
    }
    
    
    private void EditCommand ( object o, EventArgs args )
    {
        
        EditCommand ();
        
    }
    
    
    private void EditCommand ()
    {
        
        TreeIter selected;
        TreeModel model;
        
        if ( tv.Selection.GetSelected ( out model, out selected ) )
        {
            string icon    = ( string ) store.GetValue ( selected, iconStringColumn );
            string label   = ( string ) store.GetValue ( selected, viewColumn );
            string command = ( string ) store.GetValue ( selected, commandColumn );
            
            Dialogs.AddEditCommand edit = new Dialogs.AddEditCommand ( "Edit", this, icon, label, command );
        }
        
    }
    
    
    private void AddCommand ( object o, EventArgs args )
    {
        
        Dialogs.AddEditCommand add = new Dialogs.AddEditCommand ( "Add", this, "none", "", "" );   
        
    }
    
    
    private void HandleDragEnd ( object o, DragEndArgs args )
    {
        
        UpdateCommands ();
        
    }
    
    
    public void Fill ()
    {
        
        string [] commands = ( string [] ) Config.Client.Get ( Config.Settings.CustomCommands );
        
        if ( Utilities.AsString ( commands ) == Utilities.AsString ( Config.Settings.DefaultCommands ) )
        {
            commands = Config.Settings.SessionDefaultCommands;
        }
        
        foreach ( string c in commands )
        {
            
            string [] cArray = c.Split ( new char [] { ';' }, 5 );
            
            if ( cArray.GetLength ( 0 ) == 5 )
            {
                
                bool isActive = false;
                if ( cArray [ 0 ] == "true" )
                {
                    isActive = true;
                }
                
                bool isDefault = false;
                if ( cArray [ 1 ] == "true" )
                {
                    isDefault = true;
                }
                
                iter = store.AppendValues ( isActive, isDefault, GetIconPixbuf ( cArray [ 2 ] ), cArray [ 2 ], cArray [ 3 ], cArray [ 4 ], Catalog.GetString ( cArray [ 3 ] ) );
            }

        }
        
    }
    
    
    private void EnableCommandToggled ( object o, ToggledArgs args )
    {
        
        TreeIter iter;

        if ( store.GetIter ( out iter, new TreePath ( args.Path ) ) )
        {
            bool old = ( bool ) store.GetValue ( iter, activeColumn );
            
            store.SetValue ( iter, activeColumn, !old );
            
            UpdateCommands ();
        }
        
    }
    
    
    private void UpdateCommands ()
    {
        
        SetButtonSensitivity ();
        
        string [] commands = ComposeCommandsString ();
        
        if ( Utilities.AsString ( commands ) == Utilities.AsString ( Config.Settings.SessionDefaultCommands ) )
        {
            commands = Config.Settings.DefaultCommands;
        }
        
        Config.Client.Set ( Config.Settings.CustomCommands, commands );
        
        /*
         * Giving GConf some time to update before we change the menu
         */
        GLib.Timeout.Add ( 100, new GLib.TimeoutHandler ( UpdatePopupMenu ) );
        
    }
    
    
    private static bool UpdatePopupMenu ()
    {
        
        MainWindow.networkView.GeneratePopupMenus ();
        
        return false;
        
    }
    
    
    private string [] ComposeCommandsString ()
    {
        
        TreeIter cmdIter = new TreeIter ();
        
        ArrayList arrayList = new ArrayList ();
        
        if ( store.GetIterFirst ( out cmdIter ) )                         // First command
        {
            arrayList.Add ( ComposeCommandString ( cmdIter ) );
            
            while ( store.IterNext ( ref cmdIter ) )                      // All other commands
            {
                arrayList.Add ( ComposeCommandString ( cmdIter ) );
            }
        }
        
        return arrayList.ToArray ( typeof ( string ) ) as string [];
        
    }
    
    
    private string ComposeCommandString ( TreeIter iter )
    {
     
        string isActive = "false";
        if ( ( bool ) store.GetValue ( iter, activeColumn ) )
        {
            isActive = "true";
        }
        
        string isDefault = "false";
        if ( ( bool ) store.GetValue ( iter, defaultColumn ) )
        {
            isDefault = "true";
        }
        
        string icon    = ( string ) store.GetValue ( iter, iconStringColumn );
        string label   = ( string ) store.GetValue ( iter, labelColumn );
        string command = ( string ) store.GetValue ( iter, commandColumn );
        
        return isActive + ";" + isDefault + ";" + icon + ";" + label + ";" + command;
        
    }
    
    
    private void IconCellLayout ( CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter )
    {
        
    }

    
    private void ToggleCellLayout ( CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter )
    {
        
    }
    
    
    private void TextCellLayout ( CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter )
    {
    
        CellRendererText textCell = ( cell as CellRendererText );
        
        Gdk.Color darkTxtColor = this.Style.TextColors [ (int) StateType.Normal ];
        Gdk.Color lightTxtColor =  this.Style.TextColors [ (int) StateType.Insensitive ];
        
        string isDefault = "";
        if ( ( bool ) model.GetValue ( iter, defaultColumn ) )
        {
            isDefault = String.Format ( " <span size=\"smaller\" style=\"italic\">({0})</span>", TextStrings.isDefault );
        }
        
        if ( ( bool ) model.GetValue ( iter, activeColumn ) )
        {
            textCell.ForegroundGdk = darkTxtColor;
        }
        else
        {
            textCell.ForegroundGdk = lightTxtColor;
        }
        
        string title   = ( string ) model.GetValue ( iter, viewColumn );
        title = title.Replace ( "_", "" );
        
        string command = ( string ) model.GetValue ( iter, commandColumn );
        command = command.Replace ( "%N", "Nick" );
        command = command.Replace ( "%A", "123.45.67.89" );
        
        textCell.Markup = String.Format ( "<b>{0}</b>{1}\n<span size=\"smaller\">{2}</span>", title, isDefault, command );
        
    }
    
    
    private void SetButtonSensitivity ( object o, EventArgs args )
    {
        
        SetButtonSensitivity ();
        
    }
    
    
    private void SetButtonSensitivity ()
    {
        
        if ( String.Join ( "", ComposeCommandsString () ) == String.Join ( "", Config.Settings.SessionDefaultCommands ) )
        {
            revertBut.Sensitive = false;
        }
        else
        {
            revertBut.Sensitive = true;
        }
        
        if ( tv.Selection.CountSelectedRows () > 0 )
        {
            defaultBut.Sensitive = true;
            editBut.Sensitive    = true;
            removeBut.Sensitive  = true;

            TreeIter selected;
            TreeModel model;
            
            if ( tv.Selection.GetSelected ( out model, out selected ) )
            {
                
                bool isDefault = ( bool ) store.GetValue ( selected, defaultColumn );
                
                if ( isDefault )
                {
                    defaultBut.Sensitive = false;
                }
                else
                {
                    defaultBut.Sensitive = true;
                }
                
                TreePath path = model.GetPath ( selected );
                
                if ( path.Prev () )
                {
                    upBut.Sensitive = true;
                }
                else
                {
                    upBut.Sensitive = false;
                }
                
                TreeIter next = selected;
                
                if ( ( model as ListStore ).IterNext ( ref next ) )
                {
                    downBut.Sensitive = true;
                }
                else
                {
                    downBut.Sensitive = false;
                }
            }
        }
        else
        {
            defaultBut.Sensitive = false;
            editBut.Sensitive    = false;
            removeBut.Sensitive  = false;
            upBut.Sensitive      = false;
            downBut.Sensitive    = false;
        }
        
    }
    
    
    private void OnRowActivate ( object o, RowActivatedArgs args )
    {
        
        EditCommand ();
        
    }
    
}
