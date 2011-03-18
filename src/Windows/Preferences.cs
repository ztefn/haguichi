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
        
        public  CheckButton notifyOnConnectionLoss;
        public  CheckButton notifyOnMemberJoin;
        public  CheckButton notifyOnMemberLeave;
        public  CheckButton notifyOnMemberOnline;
        public  CheckButton notifyOnMemberOffline;
        
        public  FileChooserButton pathButton;
        public  SpinButton intervalSpin;
        
        private Label intervalLabel;
        private Label intervalLabel2;
        
        
        public Preferences ( string title ) : base ( title )
        {
            
            this.TransientFor   = Haguichi.mainWindow.ReturnWindow ();
            this.WindowPosition = WindowPosition.CenterOnParent;
            this.IconList       = MainWindow.appIcons;
            this.DefaultWidth   = 440;
            this.DefaultHeight  = 360;
            this.BorderWidth    = 12;
            this.DeleteEvent   += OnWinDelete;
            
            
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
            
            notifyOnConnectionLoss = new CheckButton ( TextStrings.checkboxNotifyConnectionLost );
            notifyOnConnectionLoss.Active = ( bool ) Config.Client.Get ( Config.Settings.NotifyOnConnectionLoss );
            notifyOnConnectionLoss.Toggled += delegate
            {
                Config.Client.Set ( Config.Settings.NotifyOnConnectionLoss, notifyOnConnectionLoss.Active );
            };
            
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
            trayBox.AddWidget ( notifyOnConnectionLoss );
            trayBox.AddWidget ( notifyOnMemberJoin );
            trayBox.AddWidget ( notifyOnMemberLeave );
            trayBox.AddWidget ( notifyOnMemberOnline );
            trayBox.AddWidget ( notifyOnMemberOffline );
            
            if ( !( bool ) Config.Client.Get ( Config.Settings.ShowTrayIcon ) )
            {
                startInTray.Sensitive = false;
            }
            
            GroupBox spaceBox1 = new GroupBox ( "" );
            
            VBox appearanceBox = new VBox ();
            appearanceBox.Add ( trayBox );
            appearanceBox.Add ( spaceBox1 );
            
            Box.BoxChild trayBoxC = ( ( Box.BoxChild ) ( appearanceBox [ trayBox ] ) );
            trayBoxC.Expand = false;
            
            
            commandsEditor = new CommandsEditor ();
            
            pathButton = new FileChooserButton ( TextStrings.chooseFolderTitle, FileChooserAction.SelectFolder );
            pathButton.SelectionChanged += delegate
            {
                Config.Client.Set ( Config.Settings.HamachiDataPath, pathButton.Filename );            
            };
            pathButton.SetCurrentFolder ( ( string ) Config.Client.Get ( Config.Settings.HamachiDataPath ) );
            pathButton.WidthRequest = 150;
            
            Label pathLabel = new Label ();
            pathLabel.TextWithMnemonic = TextStrings.dataPathLabel + "  ";
            pathLabel.MnemonicWidget = pathButton;
            
            HBox pathBox = new HBox ();
            pathBox.Add ( pathLabel );
            pathBox.Add ( pathButton );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( pathBox [ pathLabel ] ) );
            bc5.Expand = false;
            
            GroupBox hamachiBox = new GroupBox ( "Hamachi" );
            hamachiBox.AddWidget ( pathBox );
            
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
            
            intervalSpin = new SpinButton ( 0, 999, 1 );
            intervalSpin.Value = ( int ) ( ( double ) Config.Client.Get ( Config.Settings.UpdateInterval ) );
            intervalSpin.ValueChanged += delegate
            {
                Config.Client.Set ( Config.Settings.UpdateInterval, ( double ) intervalSpin.Value );
            };
            intervalSpin.MaxLength = 3;
            
            intervalLabel = new Label ();
            intervalLabel.MnemonicWidget = intervalSpin;
            intervalLabel.Xalign = 0;
            
            intervalLabel2 = new Label ();
            intervalLabel2.Xalign = 0;
            
            HBox intervalBox = new HBox ();
            intervalBox.Add ( intervalLabel );
            intervalBox.Add ( intervalSpin );
            intervalBox.Add ( intervalLabel2 );
            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( intervalBox [ intervalLabel ] ) );
            bc7.Expand = false;
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( intervalBox [ intervalSpin ] ) );
            bc8.Expand = false;
            
            GroupBox behaviorBox = new GroupBox ( TextStrings.behaviorGroup );
            behaviorBox.AddWidget ( connectOnStartup );
            behaviorBox.AddWidget ( reconnectOnConnectionLoss );
            behaviorBox.AddWidget ( disconnectOnQuit );
            behaviorBox.AddWidget ( askTunCfg );
            behaviorBox.AddWidget ( intervalBox );
            
            GroupBox spaceBox2 = new GroupBox ( "" );
            
            VBox systemBox = new VBox ();
            systemBox.Add ( hamachiBox );
            systemBox.Add ( behaviorBox );
            systemBox.Add ( spaceBox2 );
            
            Box.BoxChild hamachiBoxC = ( ( Box.BoxChild ) ( systemBox [ hamachiBox ] ) );
            hamachiBoxC.Expand = false; 
                        
            Box.BoxChild behaviorBoxC = ( ( Box.BoxChild ) ( systemBox [ behaviorBox ] ) );
            behaviorBoxC.Expand = false;
            
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
            
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc2.Expand = false;

            
            this.Add ( vbox );
            vbox.ShowAll ();
            
            if ( Hamachi.ApiVersion != 1 )
            {
                hamachiBox.Hide ();
                askTunCfg.Hide ();
            }
            
        }
        
        
        public void SetIntervalString ()
        {
            
            string [] intervalString = TextStrings.updateNetworkListInterval ( ( int ) intervalSpin.Value ).Split ( new string [] { "%S" }, StringSplitOptions.None );
            
            intervalLabel.TextWithMnemonic = intervalString [0];
            intervalLabel2.Text = intervalString [1];
            
        }
        
        
        public void Open ()
        {
            
            this.SetIntervalString ();
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
