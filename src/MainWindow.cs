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


public class MainWindow
{
    
    public  static Gdk.Pixbuf [] appIcons = new Gdk.Pixbuf [6];
    
    public  static AccelGroup accelGroup;
    public  static PanelIcon panelIcon;
    public  static Statusbar statusBar;
    
    public  static Menus.Quick quickMenu;
    public  static Menus.Menubar menuBar;
    
    private static Window window;
    
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
        
        IconTheme.AddBuiltinIcon ( "haguichi", 16,  Gdk.Pixbuf.LoadFromResource ( "16x16.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 22,  Gdk.Pixbuf.LoadFromResource ( "22x22.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 24,  Gdk.Pixbuf.LoadFromResource ( "24x24.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 32,  Gdk.Pixbuf.LoadFromResource ( "32x32.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 48,  Gdk.Pixbuf.LoadFromResource ( "48x48.haguichi" ) );
        IconTheme.AddBuiltinIcon ( "haguichi", 64,  Gdk.Pixbuf.LoadFromResource ( "64x64.haguichi" ) );
        
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
        autoconnectCheckbox.Active = ( bool ) Config.Client.Get ( Config.Settings.ConnectOnStartup );
        autoconnectCheckbox.Toggled += delegate
        {
            Config.Client.Set ( Config.Settings.ConnectOnStartup, autoconnectCheckbox.Active );       
        };
        
        
        VBox vbTop = new VBox ();
        VBox vbMid1 = new VBox ();
        VBox vbMid2 = new VBox ();
        VBox vbMid3 = new VBox ();
        VBox vbBottom = new VBox ();

        VBox vbDisonnected = new VBox ( false, 1 );
        vbDisonnected.Add ( vbTop );
        vbDisonnected.Add ( logoImg );
        vbDisonnected.Add ( vbMid1 );
        vbDisonnected.Add ( nameButton );
        //vbDisonnected.Add ( vbMid2 );
        vbDisonnected.Add ( connectButton );
        vbDisonnected.Add ( vbMid3 );
        vbDisonnected.Add ( autoconnectCheckbox );
        vbDisonnected.Add ( vbBottom );

        connectButton.GrabFocus ();
        
        VBox vbLeft = new VBox ();
        VBox vbRight = new VBox ();;
        
        disconnectedBox = new HBox ();
        disconnectedBox.Add ( vbLeft );
        disconnectedBox.Add ( vbDisonnected );
        disconnectedBox.Add ( vbRight );
        
        
        Box.BoxChild bc5 = ( ( Box.BoxChild ) ( disconnectedBox [ vbDisonnected ] ) );
        bc5.Expand = false;

        
        /* Main VBox */

        VBox mainBox = new VBox ( false, 1 );
        mainBox.Add ( menuBar );
        mainBox.Add ( disconnectedBox );
        mainBox.Add ( connectedBox );
        mainBox.Add ( statusBar );
        
        
        Box.BoxChild bc1 = ( ( Box.BoxChild ) ( mainBox [ menuBar ] ) );
        bc1.Expand = false;
        
        Box.BoxChild bc4 = ( ( Box.BoxChild ) ( vbDisonnected [ connectButton ] ) );
        bc4.Expand = false;
        
        Box.BoxChild bc6 = ( ( Box.BoxChild ) ( vbDisonnected [ autoconnectCheckbox ] ) );
        bc6.Expand = false;
        
        Box.BoxChild bc7 = ( ( Box.BoxChild ) ( vbDisonnected [ nameButton ] ) );
        //bc7.Expand = false;
        
        Box.BoxChild bc3 = ( ( Box.BoxChild ) ( mainBox [ statusBar ] ) );
        bc3.Expand = false;


        window = new Window ( TextStrings.appName);
        window.AddAccelGroup( accelGroup );

        window.WindowStateEvent += OnStateChanged;
        window.ConfigureEvent += OnMoveResize;
        window.DeleteEvent += OnWinDelete;

        window.SetDefaultSize ( ( int ) Config.Client.Get ( Config.Settings.WinWidth ), ( int ) Config.Client.Get ( Config.Settings.WinHeight ) );
        window.Move ( ( int ) Config.Client.Get ( Config.Settings.WinX ), ( int ) Config.Client.Get ( Config.Settings.WinY ) );
        window.AllowShrink = true;
        window.IconList = appIcons;
        window.Add ( mainBox );
        window.ShowAll ();
        
        statusBar.Visible = ( bool ) Config.Client.Get ( Config.Settings.ShowStatusbar );
        
        if ( ( bool ) Config.Client.Get ( Config.Settings.ShowTrayIcon ) )
        {
            if ( ( bool ) Config.Client.Get ( Config.Settings.StartInTray ) )
            {
                this.Hide ();
            }
            else
            {
                this.Show ();
            }
        }
        else
        {
            panelIcon.Visible = false;
        }
        
    }
    
    
    
    [GLib.ConnectBefore]
    private void OnMoveResize ( object o, ConfigureEventArgs args )
    {
        // Getting and setting window size and position values when changed (only in normal window state)
        
        if ( window.GdkWindow.State == 0 ) 
        {    
            int x, y, width, height;
            
            window.GetSize ( out width, out height );
            window.GetPosition ( out x, out y );
            
            Config.Client.Set ( Config.Settings.WinX, x );
            Config.Client.Set ( Config.Settings.WinY, y );
            Config.Client.Set ( Config.Settings.WinWidth, width );
            Config.Client.Set ( Config.Settings.WinHeight, height );
        }
    }
    
    private void OnStateChanged ( object sender, WindowStateEventArgs a ) {
        
        Gdk.EventWindowState ews = a.Event;
        Gdk.WindowState ws = ews.NewWindowState;
        
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

    
    private void OnWinDelete ( object obj, DeleteEventArgs args )
    {
        if ( !( bool ) Config.Client.Get ( Config.Settings.ShowTrayIcon ) )
        {
            GlobalEvents.QuitApp ( obj, args );
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
        panelIcon.Visible = show;
    }
    
    
    public void ToggleMainWindow ( object obj, EventArgs args )
    {
        
        if ( Config.Settings.WinMinimized )
        {
            window.Deiconify ();
        }

        if ( Config.Settings.ShowMainWindow )
        {
            window.Hide ();
        }
        else
        {
            /*
             * Correcting window decorator deviation by requiring the last position ourself before showing, and then move the window.
             */
            int x = ( int ) Config.Client.Get ( Config.Settings.WinX );
            int y = ( int ) Config.Client.Get ( Config.Settings.WinY );
            
            window.Show ();
            
            /*
             * Move window to the current desktop.
             */
            
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
        
        Config.Settings.ShowMainWindow = !Config.Settings.ShowMainWindow;
        
    }
    

    
    void StatusIconPopupHandler ( object o, PopupMenuArgs args )
    {
        quickMenu.Popup ();
    }
    
    
    public void Show ()
    {
        window.Visible = true;
        Config.Settings.ShowMainWindow = true;
    }    
    
    
    public void Hide ()
    {
        window.Visible = false;
        Config.Settings.ShowMainWindow = false;
    }


    public void SetNick ( string nick )
    {
        if ( nick == "" )
        {
            nick = TextStrings.unavailable;
        }
        
        nameLabel.Markup = String.Format ( "<span size=\"larger\" weight=\"bold\">{0}</span>", nick );
    }
    
    
    public static void SetMode ( string mode )
    {
          
        switch ( mode )
        {
            
            case "Countdown":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                autoconnectCheckbox.Sensitive = true;
                connectButton.Sensitive = true;
                connectButton.Label = TextStrings.connectCountdown.Replace ( "%S", Controller.restoreCountdown.ToString () );
                connectButton.Image = new Image ( Stock.Connect, IconSize.Button );
                nameButton.Sensitive = true;
                
                panelIcon.SetMode ( "Disconnected" );
                statusBar.Push ( 0, TextStrings.disconnected );
                
                menuBar.SetMode ( "Disconnected" );
                quickMenu.SetMode ( "Disconnected" );
                
                break;
                
            case "Connecting":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                autoconnectCheckbox.Sensitive = false;
                connectButton.Sensitive = false;
                connectButton.Label = TextStrings.connecting;
                nameButton.Sensitive = true;
                
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
                
                autoconnectCheckbox.Sensitive = true;
                connectButton.Sensitive = true;
                connectButton.Label = Stock.Connect;
                nameButton.Sensitive = true;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.disconnected );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
            case "Not configured":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                autoconnectCheckbox.Sensitive = true;
                connectButton.Sensitive = false;
                connectButton.Label = Stock.Connect;
                nameButton.Sensitive = false;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.notConfigured );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
            case "Not installed":
                
                disconnectedBox.Show ();
                connectedBox.Hide ();
                
                autoconnectCheckbox.Sensitive = true;
                connectButton.Sensitive = false;
                connectButton.Label = Stock.Connect;
                nameButton.Sensitive = false;
                
                panelIcon.SetMode ( mode );
                statusBar.Push ( 0, TextStrings.notInstalled );
                
                menuBar.SetMode ( mode );
                quickMenu.SetMode ( mode );
                
                break;
                
        }
        
    }
    
}
