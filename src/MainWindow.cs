/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2013 Stephen Brandt <stephen@stephenbrandt.com>
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
using GLib;


public class MainWindow
{
    
    public  static Gdk.Pixbuf [] appIcons = new Gdk.Pixbuf [6];
    
    public  static AccelGroup accelGroup;
    public  static PanelIcon panelIcon;
    public  static Statusbar statusBar;
    
    public  static Menus.Quick quickMenu;
    public  static Menus.Menubar menuBar;
    
    public  static Widgets.MessageBar messageBar;
    
    private static Window window;
    private static int x, y, width, height;
    
    public  static NetworkView networkView;

    private static Label nameLabel;
    private static Button nameButton;
    private static Button connectButton;
    public  static CheckButton autoconnectCheckbox;
    
    private static VBox connectedBox;
    private static HBox disconnectedBox;
    
    
    public MainWindow ()
    {
    
        Application.Init ();
        
        IconTheme.AddBuiltinIcon ( "haguichi", 16, Gdk.Pixbuf.LoadFromResource ( "16x16.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 22, Gdk.Pixbuf.LoadFromResource ( "22x22.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 24, Gdk.Pixbuf.LoadFromResource ( "24x24.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 32, Gdk.Pixbuf.LoadFromResource ( "32x32.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 48, Gdk.Pixbuf.LoadFromResource ( "48x48.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 64, Gdk.Pixbuf.LoadFromResource ( "64x64.haguichi" ) );
        
        IconTheme.AddBuiltinIcon ( "haguichi-node-online",         12, Gdk.Pixbuf.LoadFromResource ( "12x12.node-online"         ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-online-relayed", 12, Gdk.Pixbuf.LoadFromResource ( "12x12.node-online-relayed" ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-offline",        12, Gdk.Pixbuf.LoadFromResource ( "12x12.node-offline"        ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-unreachable",    12, Gdk.Pixbuf.LoadFromResource ( "12x12.node-unreachable"    ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-unapproved",     12, Gdk.Pixbuf.LoadFromResource ( "12x12.node-unapproved"     ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-error",          12, Gdk.Pixbuf.LoadFromResource ( "12x12.node-error"          ) );
        
        IconTheme.AddBuiltinIcon ( "haguichi-node-online",         18, Gdk.Pixbuf.LoadFromResource ( "18x18.node-online"         ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-online-relayed", 18, Gdk.Pixbuf.LoadFromResource ( "18x18.node-online-relayed" ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-offline",        18, Gdk.Pixbuf.LoadFromResource ( "18x18.node-offline"        ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-unreachable",    18, Gdk.Pixbuf.LoadFromResource ( "18x18.node-unreachable"    ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-unapproved",     18, Gdk.Pixbuf.LoadFromResource ( "18x18.node-unapproved"     ) );
        IconTheme.AddBuiltinIcon ( "haguichi-node-error",          18, Gdk.Pixbuf.LoadFromResource ( "18x18.node-error"          ) );
        
        appIcons [0] = IconTheme.Default.LoadIcon ( "haguichi", 16, IconLookupFlags.UseBuiltin );
        appIcons [1] = IconTheme.Default.LoadIcon ( "haguichi", 22, IconLookupFlags.UseBuiltin );
        appIcons [2] = IconTheme.Default.LoadIcon ( "haguichi", 24, IconLookupFlags.UseBuiltin );
        appIcons [3] = IconTheme.Default.LoadIcon ( "haguichi", 32, IconLookupFlags.UseBuiltin );
        appIcons [4] = IconTheme.Default.LoadIcon ( "haguichi", 48, IconLookupFlags.UseBuiltin );
        appIcons [5] = IconTheme.Default.LoadIcon ( "haguichi", 64, IconLookupFlags.UseBuiltin );
        
        panelIcon = new PanelIcon ();
        panelIcon.Activate += ToggleMainWindow; 
        panelIcon.PopupMenu += StatusIconPopupHandler;
        
        accelGroup = new AccelGroup ();
        
        statusBar = new Statusbar ();
        statusBar.HasResizeGrip = true;
        
        quickMenu = new Menus.Quick ();
        menuBar = new Menus.Menubar ();
        
        messageBar = new Widgets.MessageBar ();
        
        
        
        /* Connected Box */
        
        networkView = new NetworkView ();
        
        ScrolledWindow scrolledWindow = new ScrolledWindow ();
        scrolledWindow.Add ( networkView );
        scrolledWindow.SetPolicy ( PolicyType.Never, PolicyType.Automatic );

        connectedBox = new VBox ();
        connectedBox.Add ( scrolledWindow );
        
        
        
        /* Disconnected Box */
        
        Image logoImg = new Image ( appIcons [5] );
        
        nameLabel = new Label ();
        
        nameButton = new Button ( nameLabel );
        nameButton.TooltipText = TextStrings.changeNicknameTooltip;
        nameButton.Clicked += GlobalEvents.ChangeNick;
        nameButton.Relief = ReliefStyle.None;
        
        connectButton = new Button ( Stock.Connect );
        connectButton.Clicked += GlobalEvents.StartHamachi;
        
        autoconnectCheckbox = new CheckButton ( TextStrings.connectAutomatically );
        autoconnectCheckbox.Active = ( bool ) Config.Settings.ConnectOnStartup.Value;
        autoconnectCheckbox.Toggled += delegate
        {
            Config.Settings.ConnectOnStartup.Value = autoconnectCheckbox.Active;       
        };
        
        
        VBox vbDisonnected = new VBox ( false, 0 );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( logoImg );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( nameButton );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( connectButton );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( autoconnectCheckbox );
        vbDisonnected.Add ( new VBox () );
        vbDisonnected.Add ( new VBox () );
        
        disconnectedBox = new HBox ();
        disconnectedBox.Add ( new VBox () );
        disconnectedBox.Add ( vbDisonnected );
        disconnectedBox.Add ( new VBox () );
        
        
        
        /* Main VBox */
        
        VBox mainBox = new VBox ( false, 0 );
        mainBox.Add ( menuBar );
        mainBox.Add ( messageBar );
        mainBox.Add ( disconnectedBox );
        mainBox.Add ( connectedBox );
        mainBox.Add ( statusBar );
        
        
        Box.BoxChild bc1 = ( ( Box.BoxChild ) ( mainBox [ menuBar ] ) );
        bc1.Expand = false;
        
        Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbDisonnected [ connectButton ] ) );
        bc4.Expand = false;
        
        Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbDisonnected [ autoconnectCheckbox ] ) );
        bc6.Expand = false;
        
        //Box.BoxChild bc7 = ( ( Box.BoxChild ) ( vbDisonnected [ nameButton ] ) );
        //bc7.Expand = false;
        
        Box.BoxChild bc3 = ( ( Box.BoxChild ) ( mainBox [ statusBar ] ) );
        bc3.Expand = false;
        
        Box.BoxChild bc8 = ( ( Box.BoxChild ) ( mainBox [ messageBar ] ) );
        bc8.Expand = false;
        
        
        window = new Window ( TextStrings.appName );
        window.AddAccelGroup( accelGroup );

        window.WindowStateEvent += OnStateChanged;
        window.ConfigureEvent += OnMoveResize;
        window.DeleteEvent += OnWinDelete;

        window.SetDefaultSize ( ( int ) Config.Settings.WinWidth.Value, ( int ) Config.Settings.WinHeight.Value );
        window.Move ( ( int ) Config.Settings.WinX.Value, ( int ) Config.Settings.WinY.Value );
        window.AllowShrink = true;
        window.IconList = appIcons;
        window.Add ( mainBox );
        window.ShowAll ();
        
        messageBar.Hide ();
        
        connectButton.GrabFocus ();
        
        statusBar.Visible = ( bool ) Config.Settings.ShowStatusbar.Value;
        
        x = ( int ) Config.Settings.WinX.Value;
        y = ( int ) Config.Settings.WinY.Value;
        
        if ( ( bool ) Config.Settings.ShowTrayIcon.Value )
        {
            ShowTrayIcon ( true );
            
            if ( ( bool ) Config.Settings.StartInTray.Value )
            {
                window.Hide ();
                
                quickMenu.SetVisibility ( false );
                if ( Platform.IndicatorSession != null )
                {
                    Platform.IndicatorSession.SetVisibility ( false );    
                }
            }
            else
            {
                window.Show ();
                
                quickMenu.SetVisibility ( true );
                if ( Platform.IndicatorSession != null )
                {
                    Platform.IndicatorSession.SetVisibility ( true );    
                }
            }
        }
        else
        {
            ShowTrayIcon ( false );
        }
        
    }
    
    
    public Window ReturnWindow ()
    {
        
        return window;
        
    }
    
    
    [ConnectBefore]
    private void OnMoveResize ( object o, ConfigureEventArgs args )
    {
        
        // Getting window size and position values when changed (only in normal window state)
        
        if ( window.GdkWindow.State == 0 )
        {
            window.GetSize ( out width, out height );
            window.GetPosition ( out x, out y );
        }
        
    }
    
    
    public static void SaveGeometry ()
    {
        
        Config.Settings.WinX.Value = x;
        Config.Settings.WinY.Value = y;
        Config.Settings.WinWidth.Value = width;
        Config.Settings.WinHeight.Value = height;
        
    }
    
    
    private void OnStateChanged ( object o, WindowStateEventArgs args )
    {
        
        Gdk.WindowState ws = args.Event.NewWindowState;
        
        Debug.Log ( Debug.Domain.Gui, "MainWindow", "State changed: " + ws.ToString () );    
        
        if ( ws.ToString().IndexOf( "Iconified" ) == -1 )
        {
            Config.Settings.WinMinimized = false;
        }
        else
        {
            Config.Settings.WinMinimized = true;
        }
        
        if ( ws.ToString().IndexOf( "Maximized" ) == -1 )
        {
            Config.Settings.WinMaximized = false;
        }
        else
        {
            Config.Settings.WinMaximized = true;
        }        
        
    }

    
    private void OnWinDelete ( object o, DeleteEventArgs args )
    {
        
        if ( !( bool ) Config.Settings.ShowTrayIcon.Value )
        {
            GlobalEvents.QuitApp ( o, args );
        }
        else
        {
            Hide ();
            args.RetVal = true;
        }
        
    }
     
    
    public static void ShowOfflineMembers ( bool show )
    {
        
        networkView.GoFilterOfflineMembers ( !show );
        
    }
    
    
    public static void ShowStatusbar ( bool show )
    {
        
        statusBar.Visible = show;
        
    }
    
    
    public static void ShowTrayIcon ( bool show )
    {
        
        if ( Platform.IndicatorSession != null )
        {
            Platform.IndicatorSession.Show ( show );
            panelIcon.Visible = false;
        }
        else
        {
            panelIcon.Visible = show;
        }
        
    }
    
    
    private void StatusIconPopupHandler ( object o, PopupMenuArgs args )
    {
        
        quickMenu.Popup ( null, null, PositionMenu, 0, Gtk.Global.CurrentEventTime );
        
    }
    
    
    public void PositionMenu ( Menu menu, out int x, out int y, out bool push_in )
    {
        
        StatusIcon.PositionMenu ( menu, out x, out y, out push_in, panelIcon.Handle );
        
    }
    
    
    public void ToggleMainWindow ( object obj, EventArgs args )
    {
        
        if ( Haguichi.modalDialog == null )
        {
            if ( Config.Settings.WinMinimized || !window.Visible )
            {
                Show ();
            }
            else
            {
                Hide ();
            }
        }
        else
        {
            Haguichi.modalDialog.Present ();
        }
        
    }
    
    
    public static void Show ( object o, EventArgs args )
    {
        
        Show ();
        
    }    
    
    
    public static void Show ()
    {
        
        window.Present ();
        
        if ( Config.Settings.WinMinimized )
        {
            window.Deiconify ();
            Config.Settings.WinMinimized = false;
        }
        
        quickMenu.SetVisibility ( true );
        if ( Platform.IndicatorSession != null )
        {
            Platform.IndicatorSession.SetVisibility ( true );    
        }
        
        
        // Move window to the current desktop and correct for any desktop compositor deviation
        
        while ( x < 0 )
        {
            x += window.Screen.Width;
        }
        while ( x > window.Screen.Width )
        {
            x -= window.Screen.Width;
        }
        
        while ( y < 0 )
        {
            y += window.Screen.Height;
        }
        while ( y > window.Screen.Height )
        {
            y -= window.Screen.Height;
        }
        
        window.Move ( x, y );
        
    }    
    
    
    public static void Hide ( object o, EventArgs args )
    {
        
        Hide ();
        
    }    
    
    
    public static void Hide ()
    {
        
        SaveGeometry ();
        
        window.Hide ();
        
        quickMenu.SetVisibility ( false );
        if ( Platform.IndicatorSession != null )
        {
            Platform.IndicatorSession.SetVisibility ( false );    
        }
        
    }


    public void SetNick ( string nick )
    {
        
        if ( nick == "" )
        {
            nick = TextStrings.anonymous;
        }
        
        nameLabel.Markup = String.Format ( "<span size=\"larger\" weight=\"bold\">{0}</span>", Markup.EscapeText ( nick ) );
        
    }
    
    
    public static void SetMode ( string mode )
    {
        
        if ( Platform.IndicatorSession != null )
        {
            Platform.IndicatorSession.SetMode ( mode );    
        }
        
        switch ( mode )
        {
            
            case "Countdown":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                connectButton.Sensitive = true;
                connectButton.Label = TextStrings.connectCountdown.Replace ( "%S", Controller.restoreCountdown.ToString () );
                connectButton.Image = new Image ( Stock.Connect, IconSize.Button );
                
                panelIcon.SetMode ( "Disconnected" );
                statusBar.Push ( 0, TextStrings.disconnected );
                
                menuBar.SetMode ( "Disconnected" );
                quickMenu.SetMode ( "Disconnected" );
                
                break;
                
            case "Connecting":
                
                SetMode ( "Disconnected" );
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                connectButton.Sensitive = false;
                connectButton.Label = TextStrings.connecting;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.connecting );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
            case "Connected":
                
                disconnectedBox.Hide ();
                connectedBox.Show ();
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.connected );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
            case "Disconnected":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                connectButton.Sensitive = true;
                connectButton.Label = Stock.Connect;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.disconnected );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
            case "Not configured":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                connectButton.Sensitive = false;
                connectButton.Label = Stock.Connect;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.disconnected );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
            case "Not installed":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                connectButton.Sensitive = false;
                connectButton.Label = Stock.Connect;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.disconnected );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
        }
        
    }
    
}
