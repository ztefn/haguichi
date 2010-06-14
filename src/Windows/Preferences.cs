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
using Widgets;


namespace Windows
{

    public class Preferences : Window
    {
        
        private Button closeBut;
        
        public  CommandsEditor commandsEditor;
        
        public  CheckButton showTrayIcon;
        public  CheckButton startInTray;
        
        public  CheckButton connectOnStartup;
        public  CheckButton reconnectOnConnectionLoss;
        public  CheckButton disconnectOnQuit;
        public  CheckButton askTunCfg;
        
        public  CheckButton notifyOnMemberJoin;
        public  CheckButton notifyOnMemberLeave;
        public  CheckButton notifyOnMemberOnline;
        public  CheckButton notifyOnMemberOffline;
        
        public  FileChooserButton pathButton;
        public  SpinButton intervalSpin;
        public  SpinButton timeoutSpin;
        
        
        public Preferences ( string title ) : base ( title )
        {
            
            this.WindowPosition = Gtk.WindowPosition.Center;
            this.IconList = MainWindow.appIcons;
            this.BorderWidth = 12;
            this.DeleteEvent += OnWinDelete;
            
            
            closeBut = new Button ( Stock.Close );
            closeBut.Clicked += delegate
            {
                this.Hide ();            
            };
            
            
            showTrayIcon = new CheckButton ( TextStrings.checkboxShowTrayIcon );
            showTrayIcon.Active = ( bool ) Config.Client.Get ( Config.Settings.ShowTrayIcon );
            showTrayIcon.Toggled += delegate
            {
                bool active = showTrayIcon.Active;
                
                Config.Client.Set ( Config.Settings.ShowTrayIcon, active );
                MainWindow.ShowTrayIcon ( active );
                
                startInTray.Sensitive = active;
            };
            
            startInTray = new CheckButton ( TextStrings.checkboxStartInTray );
            startInTray.Active = ( bool ) Config.Client.Get ( Config.Settings.StartInTray );
            startInTray.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.StartInTray, startInTray.Active );
            };
            
            IndentHBox hbox1 = new IndentHBox ();
            hbox1.AddWidget ( startInTray );
                        
            notifyOnMemberJoin = new CheckButton ( TextStrings.checkboxNotifyMemberJoin );
            notifyOnMemberJoin.Active = ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberJoin );
            notifyOnMemberJoin.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.NotifyOnMemberJoin, notifyOnMemberJoin.Active );
            };
            
            notifyOnMemberLeave = new CheckButton ( TextStrings.checkboxNotifyMemberLeave );
            notifyOnMemberLeave.Active = ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberLeave );
            notifyOnMemberLeave.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.NotifyOnMemberLeave, notifyOnMemberLeave.Active );
            };
            
            notifyOnMemberOnline = new CheckButton ( TextStrings.checkboxNotifyMemberOnline );
            notifyOnMemberOnline.Active = ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberOnline );
            notifyOnMemberOnline.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.NotifyOnMemberOnline, notifyOnMemberOnline.Active );
            };

            notifyOnMemberOffline = new CheckButton ( TextStrings.checkboxNotifyMemberOffline );
            notifyOnMemberOffline.Active = ( bool ) Config.Client.Get ( Config.Settings.NotifyOnMemberOffline );
            notifyOnMemberOffline.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.NotifyOnMemberOffline, notifyOnMemberOffline.Active );
            };
            
            GroupBox trayBox = new GroupBox ( TextStrings.notifyGroup );
            trayBox.AddWidget ( showTrayIcon );
            trayBox.AddWidget ( hbox1 );
            trayBox.AddWidget ( notifyOnMemberJoin );
            trayBox.AddWidget ( notifyOnMemberLeave );
            trayBox.AddWidget ( notifyOnMemberOnline );
            trayBox.AddWidget ( notifyOnMemberOffline );
            
            if ( !( bool ) Config.Client.Get ( Config.Settings.ShowTrayIcon ) )
            {
                startInTray.Sensitive = false;
            }
            
            VBox appearanceBox = new VBox ();
            appearanceBox.Add ( trayBox );
            
            Box.BoxChild trayBoxC = ( ( Box.BoxChild ) ( appearanceBox [ trayBox ] ) );
            trayBoxC.Expand = false;
            
            
            commandsEditor = new CommandsEditor ();
            
            pathButton = new Gtk.FileChooserButton ( TextStrings.chooseFolderTitle, FileChooserAction.SelectFolder );
            pathButton.SelectionChanged += delegate
            {
                Config.Client.Set ( Config.Settings.HamachiDataPath, pathButton.Filename );            
            };
            pathButton.SetCurrentFolder ( ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) );
            pathButton.WidthRequest = 150;
            
            Label pathLabel = new Label ();
            pathLabel.TextWithMnemonic = TextStrings.dataPathLabel + ":  ";
            pathLabel.MnemonicWidget = pathButton;
            
            HBox pathBox = new HBox ();
            pathBox.Add ( pathLabel );
            pathBox.Add ( pathButton );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( pathBox [ pathLabel ] ) );
            bc5.Expand = false;
            
            Box.BoxChild bc6 = ( ( Box.BoxChild ) ( pathBox [ pathButton ] ) );
            bc6.Expand = false;
            bc6.PackType = PackType.End;
            
            intervalSpin = new SpinButton ( 5, 600, 1 );
            intervalSpin.Value = ( int ) ( ( double ) Config.Client.Get ( Config.Settings.UpdateInterval ) );
            intervalSpin.ValueChanged += delegate
            {
                Config.Client.Set ( Config.Settings.UpdateInterval, ( double ) intervalSpin.Value );
            };
            intervalSpin.MaxLength = 3;
            intervalSpin.WidthRequest = 150;

            Label intervalLabel = new Label ();
            intervalLabel.TextWithMnemonic = TextStrings.intervalLabel + ":  ";
            intervalLabel.MnemonicWidget = intervalSpin;
            intervalLabel.Xalign = 0;
            
            HBox intervalBox = new HBox ();
            intervalBox.Add ( intervalLabel );
            intervalBox.Add ( intervalSpin );
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( intervalBox [ intervalSpin ] ) );
            bc8.Expand = false;
            
            timeoutSpin = new SpinButton ( 5, 60, 1 );
            timeoutSpin.Value = ( int ) ( ( double ) Config.Client.Get ( Config.Settings.CommandTimeout ) );
            timeoutSpin.ValueChanged += delegate
            {
                Config.Client.Set ( Config.Settings.CommandTimeout, ( double ) timeoutSpin.Value );    
            };
            timeoutSpin.MaxLength = 2;
            timeoutSpin.WidthRequest = 150;

            Label timeoutLabel = new Label ();
            timeoutLabel.TextWithMnemonic = TextStrings.timeoutLabel + ":  ";
            timeoutLabel.MnemonicWidget = timeoutSpin;
            timeoutLabel.Xalign = 0;
            
            HBox timeoutBox = new HBox ();
            timeoutBox.Add ( timeoutLabel );
            timeoutBox.Add ( timeoutSpin );
            
            Box.BoxChild bc10 = ( ( Box.BoxChild ) ( timeoutBox [ timeoutSpin ] ) );
            bc10.Expand = false;
            
            GroupBox hamachiBox = new GroupBox ( "Hamachi" );
            hamachiBox.AddWidget ( pathBox );
            hamachiBox.AddWidget ( intervalBox );
            hamachiBox.AddWidget ( timeoutBox );
            
            connectOnStartup = new CheckButton ( TextStrings.connectOnStartup );
            connectOnStartup.Active = ( bool ) Config.Client.Get ( Config.Settings.ConnectOnStartup );
            connectOnStartup.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.ConnectOnStartup, connectOnStartup.Active );       
            };
            
            reconnectOnConnectionLoss = new CheckButton ( TextStrings.reconnectOnConnectionLoss );
            reconnectOnConnectionLoss.Active = ( bool ) Config.Client.Get ( Config.Settings.ReconnectOnConnectionLoss );
            reconnectOnConnectionLoss.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.ReconnectOnConnectionLoss, reconnectOnConnectionLoss.Active );       
            };
            
            disconnectOnQuit = new CheckButton ( TextStrings.disconnectOnQuit );
            disconnectOnQuit.Active = ( bool ) Config.Client.Get ( Config.Settings.DisconnectOnQuit );
            disconnectOnQuit.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.DisconnectOnQuit, disconnectOnQuit.Active );
            };
            
            askTunCfg =  new CheckButton ( TextStrings.checkboxAskBeforeRunningTuncfg2 );
            askTunCfg.Active = ( bool ) Config.Client.Get ( Config.Settings.AskBeforeRunningTunCfg );
            askTunCfg.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.AskBeforeRunningTunCfg, askTunCfg.Active );
            };
            
            GroupBox connectBox = new GroupBox ( TextStrings.behaviorGroup );
            connectBox.AddWidget ( connectOnStartup );
            connectBox.AddWidget ( reconnectOnConnectionLoss );
            connectBox.AddWidget ( disconnectOnQuit );
            connectBox.AddWidget ( askTunCfg );
            
            GroupBox spaceBox = new GroupBox ( "" );
            
            VBox systemBox = new VBox ();
            systemBox.Add ( hamachiBox );
            systemBox.Add ( connectBox );
            systemBox.Add ( spaceBox );
            
            Box.BoxChild hamachiBoxC = ( ( Box.BoxChild ) ( systemBox [ hamachiBox ] ) );
            hamachiBoxC.Expand = false; 
                        
            Box.BoxChild connectBoxC = ( ( Box.BoxChild ) ( systemBox [ connectBox ] ) );
            connectBoxC.Expand = false;
            
            Notebook notebook = new Notebook ();
            notebook.AppendPage ( systemBox, new Label ( TextStrings.generalTab ) );
            notebook.AppendPage ( commandsEditor, new Label ( TextStrings.commandsTab ) );
            notebook.AppendPage ( appearanceBox, new Label ( TextStrings.desktopTab ) );
            
            
            HButtonBox buttonBox = new HButtonBox ();
            buttonBox.Add ( closeBut );
            buttonBox.Layout = ButtonBoxStyle.End;
            
            
            VBox vbox = new VBox ();
            vbox.Add ( notebook );
            vbox.Add ( buttonBox );
            vbox.Spacing = 12;
            
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( vbox [ notebook ] ) );
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc2.Expand = false;

            
            this.Add ( vbox );
            
            this.ShowAll ();
            this.Hide ();
            
        }
        
        
        public void Open ()
        {
            
            this.Show ();
            this.Present ();
            
        }
        
        
        private void OnWinDelete ( object obj, DeleteEventArgs args )
        {
            
            this.Hide ();
            args.RetVal = true;
            
        }
        
    }

}
