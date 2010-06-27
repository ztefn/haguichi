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
using System.Threading;
using Gtk;


public class NetworkView : TreeView
{

    private static TreeStore store;
    private static TreeModelSort sortedStore;
    private static TreeModelFilter filter;
    private static TreeIter iter;
    
    private TreePath lastPath;
    
    public Network lastNetwork;
    public Member lastMember;
    
    private TreeViewColumn column;
    
    private int searchColumn;
    private int iconColumn;
    private int statusColumn;
    private int networkColumn;
    private int memberColumn;
    private int nameSortColumn;
    private int statusSortColumn;
    
    private CellRendererText textCell;
    private CellRendererPixbuf iconCell;
    
    public  string networkTemplate;
    public  string memberTemplate;
    
    private Menus.NetworkMenu networkMenu;
    private Menus.MemberMenu memberMenu;
    

    public NetworkView ()
    {
        
        searchColumn     = 0;
        iconColumn       = 1;
        statusColumn     = 2;
        networkColumn    = 3;
        memberColumn     = 4;
        nameSortColumn   = 5;
        statusSortColumn = 6;
        
        textCell = new CellRendererText ();
        
        iconCell = new CellRendererPixbuf ();
        iconCell.Width = 14;
        
        column = new TreeViewColumn ();
        
        column.PackStart ( iconCell, false );
        column.PackStart ( textCell, true );
        
        column.SetCellDataFunc ( textCell, new CellLayoutDataFunc ( TextCellLayout ) );
        column.SetCellDataFunc ( iconCell, new CellLayoutDataFunc ( IconCellLayout ) );
        
        column.AddAttribute ( iconCell, "pixbuf", iconColumn );
        column.AddAttribute ( textCell, "text", searchColumn );
 
        this.AppendColumn ( column );
        
        this.RowActivated     += OnRowActivate;
        this.RowCollapsed     += OnRowCollapsed;
        this.RowExpanded      += OnRowExpanded;
        this.PopupMenu        += HandlePopupMenu;
        this.HasTooltip        = true;
        this.QueryTooltip     += OnQueryTooltip;
        this.LevelIndentation  = 0;
        this.CursorChanged    += RowHandler;
        
        this.HeadersVisible = false;
        
        networkTemplate = ( string ) Config.Client.Get ( Config.Settings.NetworkTemplate );
        memberTemplate  = ( string ) Config.Client.Get ( Config.Settings.MemberTemplate  );
        
        InitStore ();
        GeneratePopupMenus ();
        
    }
    
    
    private void InitStore ()
    {
        
        store = new TreeStore ( typeof ( string ),      // Search string (name)
                                typeof ( Gdk.Pixbuf ),  // Status indicator icon
                                typeof ( int ),         // Status integer for filtering offline members
                                typeof ( Network ),     // Network instance
                                typeof ( Member ),      // Member instance (null when network row)
                                typeof ( string ),      // Sortable name string
                                typeof ( string ) );    // Sortable status
        
        sortedStore = new Gtk.TreeModelSort ( store );
        
        filter = new TreeModelFilter ( sortedStore , null );
        filter.VisibleFunc = new Gtk.TreeModelFilterVisibleFunc ( FilterOfflineMembers );
        
        if ( ( bool ) Config.Client.Get( Config.Settings.ShowOfflineMembers ) )
        {
            this.Model = sortedStore;
        }
        else
        {
            this.Model = filter;
        }
        
    }
    
    
    public void GeneratePopupMenus ()
    {
        
        networkMenu = new Menus.NetworkMenu ();
        memberMenu  = new Menus.MemberMenu ();   
        
    }
    
    
    private bool IsNetwork ( TreeModel model, TreeIter iter )
    {
        
        Member member = ( Member ) model.GetValue ( iter, memberColumn );
        
        if ( member == null )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
    
    private bool IsNetwork ( TreeStore store, TreeIter iter )
    {
        
        Member member = ( Member ) store.GetValue ( iter, memberColumn );
        
        if ( member == null )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
    
    private void OnQueryTooltip ( object sender, QueryTooltipArgs args )
    {

        TreePath path;
        
        if ( this.GetPathAtPos ( System.Convert.ToInt16 ( args.X ), System.Convert.ToInt16 ( args.Y ), out path ) )
        {
            sortedStore.GetIter (out iter, path);
            
            Network network;
            Member member;
            
            HBox  tipBox   = new HBox ();
            Label tipLabel = new Label ();
            Image tipIcon;
            
            if ( IsNetwork ( sortedStore, iter ) )
            {
                network = ( Network ) sortedStore.GetValue ( iter, networkColumn );
                
                int memberCount;
                int memberOnlineCount;

                network.ReturnMemberCount ( out memberCount, out memberOnlineCount );
        
                string statusString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.status, network.Status.statusString );
                string countString  = String.Format ( TextStrings.memberCount, memberOnlineCount, memberCount );
                string memberString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.members, countString );
                string ownerString;
                
                if ( network.IsOwner == 1 )
                {
                    ownerString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.owner, TextStrings.you );
                }
                else
                {
                    ownerString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.owner, TextStrings.unknown );
                }
                
                tipLabel.Markup = String.Format ( "<span size=\"larger\" weight=\"bold\">{0}</span><span size=\"smaller\">{1}{2}{3}</span>", network.Name, statusString, memberString, ownerString );
                tipLabel.Xpad   = 6;
                tipLabel.Ypad   = 3;
                
                tipIcon = new Image ();
                if ( Gnome.IconTheme.Default.HasIcon ( "network-workgroup" ) )
                {
                    tipIcon.SetFromIconName ( "network-workgroup", IconSize.Dialog );
                }
                tipIcon.Yalign  = 0;
                tipIcon.Xpad    = 3;
                
                tipBox.Add ( tipIcon );
                tipBox.Add ( tipLabel );
                tipBox.ShowAll ();
                tipBox.BorderWidth = 3;
                
                args.Tooltip.Custom = tipBox;
                args.RetVal = true;
            }
            else
            {
                member = ( Member ) sortedStore.GetValue ( iter, memberColumn );
                
                string statusString  = String.Format ( "\n{0} <i>{1}</i>", TextStrings.status, member.Status.statusString );
                string addressString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.address, member.Address );
                string tunnelString  = "";
                
                if ( member.Tunnel != "")
                {
                    tunnelString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.tunnel, member.Tunnel );
                }
                
                tipLabel.Markup = String.Format ( "<span size=\"larger\" weight=\"bold\">{0}</span><span size=\"smaller\">{1}{2}{3}</span>", member.Nick, statusString, addressString, tunnelString );
                tipLabel.Xpad   = 6;
                tipLabel.Ypad   = 3;
                
                tipIcon = new Image ();
                if ( Gnome.IconTheme.Default.HasIcon ( "stock_person" ) )
                {
                    tipIcon.SetFromIconName ( "stock_person", IconSize.Dialog );
                }
                else if ( Gnome.IconTheme.Default.HasIcon ( "user-identity" ) )
                {
                    tipIcon.SetFromIconName ( "user-identity", IconSize.Dialog );
                }
                tipIcon.Yalign  = 0;
                tipIcon.Xpad    = 3;
                
                tipBox.Add ( tipIcon );
                tipBox.Add ( tipLabel );
                tipBox.ShowAll ();
                tipBox.BorderWidth = 3;
                
                args.Tooltip.Custom = tipBox;
                args.RetVal = true;
            }
            
            /* Redraw tooltip for every row separately */
            this.SetTooltipRow ( args.Tooltip, path );
        
        } else {
        
            args.RetVal = false;
        
        }
    }
    
    
    public void FillTree ()
    {
        
        InitStore ();
    
        foreach ( Network network in Haguichi.connection.Networks )
        {
            AddNetwork ( network );
        }
        
        /* Go sort tree. Doing this after the tree is filled, because otherwise things get messy... */
        GoSort ( ( string ) Config.Client.Get ( Config.Settings.SortNetworkListBy ) );
        
    }
    
    
    public void AddNetwork ( Network network )
    {
        
        int memberCount;
        int memberOnlineCount;

        network.ReturnMemberCount ( out memberCount, out memberOnlineCount );

        iter = store.AppendValues ( network.Name, Status.GetPixbuf ( network.Status ), network.Status.statusInt, network, null, network.NameSortString, network.StatusSortString );
        
        foreach ( Member member in network.Members )
        {
            AddMember ( network, member );
        }
        
        Thread updateThread = new Thread ( network.DetermineOwnership );
        updateThread.Start();
        
        CollapseOrExpandNetwork ( network );
        
    }
    
    
    public void UpdateNetwork ( Network network )
    {
        
        TreeIter iter = ReturnNetworkIter ( network );
        
        int memberCount;
        int memberOnlineCount;

        network.ReturnMemberCount ( out memberCount, out memberOnlineCount );
        
        store.SetValues ( iter, network.Name, Status.GetPixbuf ( network.Status ), network.Status.statusInt, network, null, network.NameSortString, network.StatusSortString );
        
    }
    
    
    private TreeIter ReturnNetworkIter ( Network network )
    {
        
        return ReturnNetworkIter ( network.Name );
        
    }
    
    
    private TreeIter ReturnNetworkIter ( string network )
    {
        
        TreeIter returnIter = new TreeIter ();
        TreeIter networkIter = new TreeIter ();
        
        if ( store.GetIterFirst ( out networkIter ) )                         // First network
        {
            Network netw = ( Network ) store.GetValue ( networkIter, networkColumn );
            if ( netw.Name == network )
            {
                returnIter = networkIter;
            }
            while ( store.IterNext ( ref networkIter ) )                      // All other networks
            {
                netw = ( Network ) store.GetValue ( networkIter, networkColumn );
                
                if ( netw.Name == network )
                {
                    returnIter = networkIter;
                }
            }
        }
        
        return returnIter;
    }
    
    
    private TreeIter ReturnMemberIter ( Network network, Member member )
    {
        
        TreeIter returnIter = new TreeIter ();
    
        TreeIter networkIter = ReturnNetworkIter ( network );
        TreeIter memberIter = new TreeIter ();
        store.IterChildren ( out memberIter, networkIter );             // First member
        
        Member pr = ( Member ) store.GetValue ( memberIter, memberColumn );
        if ( pr.Address == member.Address )
        {
            returnIter = memberIter;
        }      
        while ( store.IterNext ( ref memberIter ) )                     // All other members
        {
            pr = ( Member ) store.GetValue ( memberIter, memberColumn );
            
            if ( pr.Address == member.Address )
            {
                returnIter = memberIter;
            }
        }
        
        return returnIter;
    }
    
    
    public void RemoveNetwork ( Network network )
    {
        
        TreeIter iter = ReturnNetworkIter ( network );
        store.Remove ( ref iter );
    }
    
    
    public void AddMember ( Network network, Member member )
    {
        
        TreeIter iter = ReturnNetworkIter ( network );
        store.AppendValues ( iter, member.Nick, Status.GetPixbuf ( member.Status ), member.Status.statusInt, network, member, member.NameSortString, member.StatusSortString );
        
    }
    
    
    public void UpdateMember ( Network network, Member member )
    {
        
        TreeIter iter = ReturnMemberIter ( network, member );
        store.SetValues ( iter, member.Nick, Status.GetPixbuf ( member.Status ), member.Status.statusInt, network, member, member.NameSortString, member.StatusSortString );
        
    }
    
    
    public void RemoveMember ( Network network, Member member )
    {
        
        TreeIter iter = ReturnMemberIter ( network, member );
        store.Remove ( ref iter );
        
    }
    
    
    public void GoFilterOfflineMembers ( bool boolean )
    {
        
        if ( boolean == true )
        {
            this.Model = filter;
        }
        else
        {
            this.Model = sortedStore;
        }
        
        filter.Refilter ();
        
        CollapseOrExpandNetworks ();
        
    }
    
    
    public void GoSort ( string sortBy )
    {
        
        if ( sortBy == "status" )
        {
            sortedStore.SetSortColumnId ( statusSortColumn, SortType.Ascending );
        }
        else
        {
            sortedStore.SetSortColumnId ( nameSortColumn, SortType.Ascending );
        }
        
    }
    
    
    private bool FilterOfflineMembers ( Gtk.TreeModel model, Gtk.TreeIter iter )
    {

        int status = ( int ) model.GetValue ( iter, statusColumn );
        
        if ( IsNetwork ( model, iter ) )
        {
            return true;
        }
        else
        {
            if ( status == 0 )
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
    }
    
    
    private void TextCellLayout ( CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter )
    {
    
        CellRendererText textCell = ( cell as CellRendererText );
        
        Gdk.Color darkTxtColor = this.Style.TextColors [ (int) StateType.Normal ];
        Gdk.Color lightTxtColor =  this.Style.TextColors [ (int) StateType.Insensitive ];
        
        Network network = ( Network ) model.GetValue ( iter, networkColumn );
        Member member = ( Member ) model.GetValue ( iter, memberColumn );
        
        int memberCount;
        int memberOnlineCount;
        network.ReturnMemberCount ( out memberCount, out memberOnlineCount );
        
        string name;
        string address = string.Empty;
        string statusText;
        int    statusInt;

        if ( IsNetwork ( model, iter ) )
        {
        
            name = GLib.Markup.EscapeText ( network.Name );
            name = name.Replace ( "\n", "" );
            name = name.Replace ( "\r", "" );
            name = name.Replace ( "\t", "" );
            name = name.Replace ( "\b", "" );
            
            statusText = network.Status.statusString;
            statusInt  = network.Status.statusInt;
            
            string template = networkTemplate;
            template = template.Replace ( "%N", "{0}" );
            template = template.Replace ( "%S", "{1}" );
            template = template.Replace ( "%T", "{2}" );
            template = template.Replace ( "%O", "{3}" );
            template = template.Replace ( "<br>", "{4}" );
            
            textCell.Markup = String.Format ( template, name, statusText, memberCount.ToString(), memberOnlineCount.ToString(), "\n" );
            
            if ( statusInt == 0 )
            {
                textCell.ForegroundGdk = lightTxtColor;
            }
            else
            {
                textCell.ForegroundGdk = darkTxtColor;
            }
            
        }
        else
        {
        
            name = GLib.Markup.EscapeText ( member.Nick );
            name = name.Replace ( "\n", "" );
            name = name.Replace ( "\r", "" );
            name = name.Replace ( "\t", "" );
            name = name.Replace ( "\b", "" );
            
            address    = member.Address;
            statusText = member.Status.statusString;
            statusInt  = member.Status.statusInt;
            
            string template = memberTemplate;
            template = template.Replace ( "%N", "{0}" );
            template = template.Replace ( "%A", "{1}" );
            template = template.Replace ( "%S", "{2}" );
            template = template.Replace ( "<br>", "{3}" );
            
            textCell.Markup = String.Format ( template, name, address, statusText, "\n" );

            if ( statusInt == 0 )
            {
                textCell.ForegroundGdk = lightTxtColor;
            }
            else
            {
                textCell.ForegroundGdk = darkTxtColor;
            }
            
        } 
        
    }
    
    
    private void IconCellLayout ( CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter )
    {
        
        // Nothing
        
    }
    
    
    private void RowHandler ( object o, EventArgs args )
    {
    
        TreeIter iter;
        TreeModel model;
        
        if ( this.Selection.GetSelected ( out model, out iter ) )
        {
            
            lastPath = sortedStore.GetPath ( iter );
            lastNetwork = ( Network ) sortedStore.GetValue ( iter, networkColumn );
            
            if ( !IsNetwork ( sortedStore, iter ) )
            {
                lastMember = ( Member ) sortedStore.GetValue ( iter, memberColumn );
            }

        }
        
    }
    
    
    private void OnRowActivate ( object o, RowActivatedArgs args )
    {
        
        sortedStore.GetIter ( out iter, args.Path );
        
        if ( IsNetwork ( sortedStore, iter ) )
        {
            Network network = ( Network ) sortedStore.GetValue ( iter, networkColumn );
            int statusInt = network.Status.statusInt;
            
            if ( statusInt == 1 )
            {
                network.GoOffline ( o, args );
            }
            else
            {
                network.GoOnline ( o, args );
            }
        }
        else
        {
            string command = Command.ReturnDefault();
            
            if ( command != "" )
            {
                command = command.Replace ( "%N", lastMember.Nick );
                command = command.Replace ( "%A", lastMember.Address );
                
                Command.Execute ( command );
            }

        }
          
    }
    
    
    private void OnRowExpanded (object o, RowExpandedArgs args)
    {
        
        sortedStore.GetIter ( out iter, args.Path );
        UpdateCollapsedNetworks ( iter, "Expanded" );
        
    }

    
    private void OnRowCollapsed (object o, RowCollapsedArgs args)
    {
        
        sortedStore.GetIter ( out iter, args.Path );
        UpdateCollapsedNetworks ( iter, "Collapsed" );
        
    }
    
    
    private void UpdateCollapsedNetworks ( TreeIter iter, string mode )
    {
        
        if ( IsNetwork ( sortedStore, iter ) )
        {
            
            Network network = ( Network ) sortedStore.GetValue ( iter, networkColumn );
            
            string [] curNetworks = ( string [] ) Config.Client.Get ( Config.Settings.CollapsedNetworks );
            
            ArrayList arrayList = new ArrayList ();
            foreach ( string s in curNetworks )
            {
                arrayList.Add ( s );
            }
            
            
            if ( mode == "Collapsed" )
            {
                
                if ( !arrayList.Contains ( network.Name ) )
                {
                    arrayList.Add ( network.Name );
                    
                    string [] newNetworks = arrayList.ToArray ( typeof ( string ) ) as string [];
                    
                    Config.Client.Set ( Config.Settings.CollapsedNetworks, newNetworks );
                }
                
            }
            
            if ( mode == "Expanded" )
            {
                
                if ( arrayList.Contains ( network.Name ) )
                {
                    arrayList.Remove ( network.Name );
                    
                    string [] newNetworks = arrayList.ToArray ( typeof ( string ) ) as string [];
                                                     
                    Config.Client.Set ( Config.Settings.CollapsedNetworks, newNetworks );
                }
                
            }
            
        }
        
    }
    
    
    private void CollapseOrExpandNetworks ()
    {
        
        TreeIter networkIter = new TreeIter ();
        
        if ( sortedStore.GetIterFirst ( out networkIter ) )                         // First network
        {
            
            Network network = ( Network ) sortedStore.GetValue ( networkIter, networkColumn );
            
            if ( IsCollapsed ( network ) )
            {
                this.CollapseRow ( sortedStore.GetPath ( iter ) );
            }
            else
            {
                this.ExpandRow ( sortedStore.GetPath ( iter ), false );
            }
            
            while ( sortedStore.IterNext ( ref networkIter ) )                      // All other networks
            {
                
                network = ( Network ) sortedStore.GetValue ( networkIter, networkColumn );
                
                if ( IsCollapsed ( network ) )
                {
                    this.CollapseRow ( sortedStore.GetPath ( networkIter ) );
                }
                else
                {
                    this.ExpandRow ( sortedStore.GetPath ( networkIter ), false );
                }
                
            }
            
        }
        
    }
    
    
    private void CollapseOrExpandNetwork ( Network network )
    {
        
        TreeIter iter =  ReturnNetworkIter ( network );
        
        if ( IsCollapsed ( network ) )
        {
            this.CollapseRow ( store.GetPath ( iter ) );
        }
        else
        {
            this.ExpandRow ( store.GetPath ( iter ), false );
        }
        
    }
    
    
    private bool IsCollapsed ( Network network )
    {
        
        string [] collapsed = ( string [] ) Config.Client.Get ( Config.Settings.CollapsedNetworks );
            
        foreach ( string s in collapsed )
        {
            
            if ( s == network.Name )
            {
                return true;
            }
            
        }   
        
        return false;
    }
    
    
    private void ShowPopupMenu ()
    {
        
        sortedStore.GetIter ( out iter, lastPath );
        
        Network lastNetwork = ( Network ) sortedStore.GetValue ( iter, networkColumn );
            
        if ( IsNetwork ( sortedStore, iter ) )
        {
            networkMenu.SetNetwork ( lastNetwork );
            networkMenu.Popup ();
        }
        else
        {
            Member lastMember = ( Member ) sortedStore.GetValue ( iter, memberColumn );
            
            memberMenu.SetMember ( lastMember, lastNetwork );
            memberMenu.Popup ();
        }
        
    }
    
    
    private void HandlePopupMenu ( object o, PopupMenuArgs args )
    {
        
        if ( this.Selection.PathIsSelected ( lastPath ) )
        {
            ShowPopupMenu ();
        }
        
         args.RetVal = true;
    }
    
    
    protected override bool OnButtonPressEvent ( Gdk.EventButton evnt )
    {

        if ( evnt.Button == 3 )
        {
            if ( GetPathAtPos ( System.Convert.ToInt16 ( evnt.X ), System.Convert.ToInt16 ( evnt.Y ), out lastPath ) )
            {
                ShowPopupMenu ();
            }
        }
        
        return base.OnButtonPressEvent ( evnt );
    }
    
}
