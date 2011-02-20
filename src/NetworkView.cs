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
using System.Collections;
using Gtk;
using GLib;


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
    private string currentLayout;
    
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
        
        this.HeadersVisible    = false;
        this.RulesHint         = ( bool ) Config.Client.Get ( Config.Settings.ShowAlternatingRowColors  );
        
        currentLayout = ( string ) Config.Client.Get ( Config.Settings.NetworkListLayout );
        if ( currentLayout == "large" )
        {
            iconCell.Width = ( int ) Config.Client.Get ( Config.Settings.NetworkListIconSizeLarge ) + 4;
            
            networkTemplate = ( string ) Config.Client.Get ( Config.Settings.NetworkTemplateLarge );
            memberTemplate  = ( string ) Config.Client.Get ( Config.Settings.MemberTemplateLarge  );
        }
        else
        {
            iconCell.Width = ( int ) Config.Client.Get ( Config.Settings.NetworkListIconSizeSmall ) + 4;
            
            networkTemplate = ( string ) Config.Client.Get ( Config.Settings.NetworkTemplateSmall );
            memberTemplate  = ( string ) Config.Client.Get ( Config.Settings.MemberTemplateSmall  );
        }
        
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
                
                string idString      = "";
                string ownerString;
                string countString   = String.Format ( TextStrings.memberCount, memberOnlineCount, memberCount );
                string memberString  = String.Format ( "\n{0} <i>{1}</i>", TextStrings.members, countString );
                string lockString    = "";
                string approveString = "";
                string statusString  = String.Format ( "\n{0} <i>{1}</i>", TextStrings.status, network.Status.statusString );
                
                if ( Hamachi.ApiVersion > 1 )
                {
                    idString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.networkId, network.Id );
                }
                
                string ownerNick = network.ReturnOwnerNick ();
                
                if ( network.IsOwner == 1 )
                {
                    ownerString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.owner, TextStrings.you );
                }
                else if ( ( network.OwnerId != "" ) && ( ownerNick != "" ) )
                {
                    ownerString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.owner, Markup.EscapeText ( ownerNick ) );
                }
                else if ( network.OwnerId != "" )
                {
                    ownerString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.owner, network.OwnerId );
                }
                else
                {
                    ownerString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.owner, TextStrings.unknown );
                }
                
                if ( network.Lock == "locked" )
                {
                    lockString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.locked, TextStrings.yes );
                }
                
                if ( network.Lock == "unlocked" )
                {
                    lockString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.locked, TextStrings.no );
                }
                
                if ( network.Approve == "manual" )
                {
                    approveString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.approval, TextStrings.manually );
                }
                
                if ( network.Approve == "auto" )
                {
                    approveString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.approval, TextStrings.automatically );
                }
                
                tipLabel.Markup = String.Format ( "<span size=\"larger\" weight=\"bold\">{0}</span><span size=\"smaller\">{1}{2}{3}{4}{5}{6}</span>", Markup.EscapeText ( network.Name ), idString, memberString, ownerString, approveString, lockString, statusString );
                tipLabel.Xpad   = 6;
                tipLabel.Ypad   = 3;
                
                tipIcon = new Image ();
                if ( IconTheme.Default.HasIcon ( "network-workgroup" ) )
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
                
                string clientString  = "";
                string addressString = "";
                string tunnelString  = "";
                string statusString  = String.Format ( "\n{0} <i>{1}</i>", TextStrings.status, member.Status.statusString );
                
                if ( member.ClientId != member.Address )
                {
                    clientString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.clientId, member.ClientId );
                }
                
                if ( member.Address != "" )
                {
                    addressString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.address, member.Address );
                }
                
                if ( member.Tunnel != "" )
                {
                    tunnelString = String.Format ( "\n{0} <i>{1}</i>", TextStrings.tunnel, member.Tunnel );
                }
                
                tipLabel.Markup = String.Format ( "<span size=\"larger\" weight=\"bold\">{0}</span><span size=\"smaller\">{1}{2}{3}{4}</span>", Markup.EscapeText ( member.Nick ), clientString, addressString, tunnelString, statusString );
                tipLabel.Xpad   = 6;
                tipLabel.Ypad   = 3;
                
                tipIcon = new Image ();
                if ( IconTheme.Default.HasIcon ( "stock_person" ) )
                {
                    tipIcon.SetFromIconName ( "stock_person", IconSize.Dialog );
                }
                else if ( IconTheme.Default.HasIcon ( "avatar-default" ) )
                {
                    tipIcon.SetFromIconName ( "avatar-default", IconSize.Dialog );
                }
                else if ( IconTheme.Default.HasIcon ( "user-info" ) )
                {
                    tipIcon.SetFromIconName ( "user-info", IconSize.Dialog );
                }
                else if ( IconTheme.Default.HasIcon ( "user-identity" ) )
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
    
    
    private int GetNetworkListIconSize ()
    {
        
        if ( currentLayout == "large" )
        {
            return ( int ) Config.Client.Get ( Config.Settings.NetworkListIconSizeLarge );
        }
        else
        {
            return ( int ) Config.Client.Get ( Config.Settings.NetworkListIconSizeSmall );
        }
        
    }
    
    
    public void AddNetwork ( Network network )
    {
        
        int memberCount;
        int memberOnlineCount;

        network.ReturnMemberCount ( out memberCount, out memberOnlineCount );

        iter = store.AppendValues ( network.Name, network.Status.GetPixbuf ( GetNetworkListIconSize () ), network.Status.statusInt, network, null, network.NameSortString, network.StatusSortString );
        
        foreach ( Member member in network.Members )
        {
            AddMember ( network, member );
        }
        
        network.DetermineOwnership ();
        
        CollapseOrExpandNetwork ( network );
        
    }
    
    
    public void UpdateNetwork ( Network network )
    {
        
        TreeIter iter = ReturnNetworkIter ( network );
        
        int memberCount;
        int memberOnlineCount;

        network.ReturnMemberCount ( out memberCount, out memberOnlineCount );
        
        store.SetValues ( iter, network.Name, network.Status.GetPixbuf ( GetNetworkListIconSize () ), network.Status.statusInt, network, null, network.NameSortString, network.StatusSortString );
        
    }
    
    
    public TreeIter ReturnNetworkIter ( Network network )
    {
        
        return ReturnNetworkIter ( network.Id );
        
    }
    
    
    public Network ReturnNetworkById ( string id )
    {
        
        Network returnNetwork = new Network ();
        TreeIter networkIter = new TreeIter ();
        
        if ( store.GetIterFirst ( out networkIter ) )                         // First network
        {
            Network netw = ( Network ) store.GetValue ( networkIter, networkColumn );
            if ( netw.Id == id )
            {
                returnNetwork = netw;
            }
            while ( store.IterNext ( ref networkIter ) )                      // All other networks
            {
                netw = ( Network ) store.GetValue ( networkIter, networkColumn );
                
                if ( netw.Id == id )
                {
                    returnNetwork = netw;
                }
            }
        }
        
        return returnNetwork;
    }
    
    
    private TreeIter ReturnNetworkIter ( string network )
    {
        
        TreeIter returnIter = new TreeIter ();
        TreeIter networkIter = new TreeIter ();
        
        if ( store.GetIterFirst ( out networkIter ) )                         // First network
        {
            Network netw = ( Network ) store.GetValue ( networkIter, networkColumn );
            if ( netw.Id == network )
            {
                returnIter = networkIter;
            }
            while ( store.IterNext ( ref networkIter ) )                      // All other networks
            {
                netw = ( Network ) store.GetValue ( networkIter, networkColumn );
                
                if ( netw.Id == network )
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
        
        Member memb = ( Member ) store.GetValue ( memberIter, memberColumn );
        if ( memb.ClientId == member.ClientId )
        {
            returnIter = memberIter;
        }      
        while ( store.IterNext ( ref memberIter ) )                     // All other members
        {
            memb = ( Member ) store.GetValue ( memberIter, memberColumn );
            
            if ( memb.ClientId == member.ClientId )
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
        store.AppendValues ( iter, member.Nick, member.Status.GetPixbuf ( GetNetworkListIconSize () ), member.Status.statusInt, network, member, member.NameSortString, member.StatusSortString );
        
    }
    
    
    public void UpdateMember ( Network network, Member member )
    {
        
        TreeIter iter = ReturnMemberIter ( network, member );
        store.SetValues ( iter, member.Nick, member.Status.GetPixbuf ( GetNetworkListIconSize () ), member.Status.statusInt, network, member, member.NameSortString, member.StatusSortString );
        
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

        if ( IsNetwork ( model, iter ) )
        {
        
            string name = Markup.EscapeText ( network.Name );
            name = name.Replace ( "\n", "" );
            name = name.Replace ( "\r", "" );
            name = name.Replace ( "\t", "" );
            name = name.Replace ( "\b", "" );
            
            int memberCount;
            int memberOnlineCount;
            network.ReturnMemberCount ( out memberCount, out memberOnlineCount );
            
            string template = networkTemplate;
            template = template.Replace ( "%ID",  "{0}" );
            template = template.Replace ( "%N",   "{1}" );
            template = template.Replace ( "%S",   "{2}" );
            template = template.Replace ( "%T",   "{3}" );
            template = template.Replace ( "%O",   "{4}" );
            template = template.Replace ( "<br>", "{5}" );
            
            if ( network.IsOwner == 1 )
            {
                template = template.Replace ( "%*",  "✩"  );
                template = template.Replace ( "%_*", " ✩" );
                template = template.Replace ( "%*_", "✩ " );
            }
            else
            {
                template = template.Replace ( "%*",  "" );
                template = template.Replace ( "%_*", "" );
                template = template.Replace ( "%*_", "" );
            }
            
            textCell.Markup = String.Format ( template, network.Id, name, network.Status.statusString, memberCount.ToString(), memberOnlineCount.ToString(), "\n" );
            
            if ( network.Status.statusInt == 0 )
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
            
            string name = Markup.EscapeText ( member.Nick );
            name = name.Replace ( "\n", "" );
            name = name.Replace ( "\r", "" );
            name = name.Replace ( "\t", "" );
            name = name.Replace ( "\b", "" );
            
            string template = memberTemplate;
            template = template.Replace ( "%ID",  "{0}" );
            template = template.Replace ( "%N",   "{1}" );
            template = template.Replace ( "%A",   "{2}" );
            template = template.Replace ( "%S",   "{3}" );
            template = template.Replace ( "<br>", "{4}" );
            
            if ( network.OwnerId == member.ClientId )
            {
                template = template.Replace ( "%*",  "✩"  );
                template = template.Replace ( "%_*", " ✩" );
                template = template.Replace ( "%*_", "✩ " );
            }
            else
            {
                template = template.Replace ( "%*",  "" );
                template = template.Replace ( "%_*", "" );
                template = template.Replace ( "%*_", "" );
            }
            
            textCell.Markup = String.Format ( template, member.ClientId, name, member.Address, member.Status.statusString, "\n" );
            
            if ( member.Status.statusInt == 0 )
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
        
        if ( this.Selection.GetSelected ( out iter ) )
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
            if ( lastMember.Status.statusInt != 3 )
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
                
                if ( !arrayList.Contains ( network.Id ) )
                {
                    arrayList.Add ( network.Id );
                    
                    string [] newNetworks = arrayList.ToArray ( typeof ( string ) ) as string [];
                    
                    Config.Client.Set ( Config.Settings.CollapsedNetworks, newNetworks );
                }
                
            }
            
            if ( mode == "Expanded" )
            {
                
                if ( arrayList.Contains ( network.Id ) )
                {
                    arrayList.Remove ( network.Id );
                    
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
                this.CollapseRow ( sortedStore.GetPath ( networkIter ) );
            }
            else
            {
                this.ExpandRow ( sortedStore.GetPath ( networkIter ), false );
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
        
        TreeIter iter = ReturnNetworkIter ( network );
        
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
            
            if ( s == network.Id )
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
    
    
    private void SetNetworkListIconSize ( int size )
    {
        
        this.iconCell.Width = size;
        this.ColumnsAutosize ();
        
        store.Foreach( ( model, path, iter ) =>
        {
            int statusInt = ( int ) store.GetValue ( iter, statusColumn );
            store.SetValue ( iter, iconColumn, Status.GetPixbuf ( size, statusInt ) );
            
            return false; // Continue
        });
        
    }
    
    
    public void SetLayout ()
    {
        
        SetLayout ( currentLayout );
        
    }
    
    
    public void SetLayout ( string layout )
    {
        
        if ( layout == "large" )
        {
            currentLayout = "large";
            
            Config.Client.Set ( Config.Settings.NetworkListLayout, "large" );
            
            networkTemplate = ( string ) Config.Client.Get ( Config.Settings.NetworkTemplateLarge );
            memberTemplate  = ( string ) Config.Client.Get ( Config.Settings.MemberTemplateLarge  );
            
            SetNetworkListIconSize ( ( int ) Config.Client.Get ( Config.Settings.NetworkListIconSizeLarge ) );
        }
        else
        {
            currentLayout = "normal";
            
            Config.Client.Set ( Config.Settings.NetworkListLayout, "normal" );
            
            networkTemplate = ( string ) Config.Client.Get ( Config.Settings.NetworkTemplateSmall );
            memberTemplate  = ( string ) Config.Client.Get ( Config.Settings.MemberTemplateSmall  );
            
            SetNetworkListIconSize ( ( int ) Config.Client.Get ( Config.Settings.NetworkListIconSizeSmall ) );
        }
        
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
