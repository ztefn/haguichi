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
using Widgets;


namespace Dialogs
{

    public class Preferences : Dialog
    {
        
        private Button closeBut;
        
        public  CommandsEditor commandsEditor;
        
        public  CheckButton showTrayIcon;
        public  CheckButton startInTray;
        
        public  CheckButton connectOnStartup;
        public  CheckButton reconnectOnConnectionLoss;
        public  CheckButton disconnectOnQuit;
        public  CheckButton updateNetworkList;
        
        public  CheckButton notifyOnConnectionLoss;
        public  CheckButton notifyOnMemberJoin;
        public  CheckButton notifyOnMemberLeave;
        public  CheckButton notifyOnMemberOnline;
        public  CheckButton notifyOnMemberOffline;
        
        private HBox ipBox;
        private GroupBox hamachiBox;
        
        private HBox intervalBox;
        
        public  ComboBox ipCombo;
        public  SpinButton intervalSpin;
        
        private Label intervalLabel;
        
        
        public Preferences ( string title ) : base ()
        {
            
            this.Title          = title;
            this.TransientFor   = Haguichi.mainWindow.ReturnWindow ();
            this.WindowPosition = WindowPosition.CenterOnParent;
            this.IconList       = MainWindow.appIcons;
            this.HasSeparator   = false;
            this.Resizable      = false;
            this.BorderWidth    = 10;
            this.DeleteEvent   += OnWinDelete;
            
            this.ActionArea.Destroy ();
            
            
            closeBut = new Button ( Stock.Close );
            closeBut.Clicked += delegate
            {
                this.Hide ();            
            };
            
            
            showTrayIcon = new CheckButton ( TextStrings.checkboxShowTrayIcon );
            showTrayIcon.Active = ( bool ) Config.Settings.ShowTrayIcon.Value;
            showTrayIcon.Toggled += delegate
            {
                bool active = showTrayIcon.Active;
                
                Config.Settings.ShowTrayIcon.Value = active;
                MainWindow.ShowTrayIcon ( active );
                
                startInTray.Sensitive = active;
            };
            
            startInTray = new CheckButton ( TextStrings.checkboxStartInTray );
            startInTray.Active = ( bool ) Config.Settings.StartInTray.Value;
            startInTray.Toggled += delegate
            {
                Config.Settings.StartInTray.Value = startInTray.Active;
            };
            
            IndentHBox hbox1 = new IndentHBox ();
            hbox1.AddWidget ( startInTray );
            
            notifyOnConnectionLoss = new CheckButton ( TextStrings.checkboxNotifyConnectionLost );
            notifyOnConnectionLoss.Active = ( bool ) Config.Settings.NotifyOnConnectionLoss.Value;
            notifyOnConnectionLoss.Toggled += delegate
            {
                Config.Settings.NotifyOnConnectionLoss.Value = notifyOnConnectionLoss.Active;
            };
            
            notifyOnMemberJoin = new CheckButton ( TextStrings.checkboxNotifyMemberJoin );
            notifyOnMemberJoin.Active = ( bool ) Config.Settings.NotifyOnMemberJoin.Value;
            notifyOnMemberJoin.Toggled += delegate
            {
                Config.Settings.NotifyOnMemberJoin.Value = notifyOnMemberJoin.Active;
            };
            
            notifyOnMemberLeave = new CheckButton ( TextStrings.checkboxNotifyMemberLeave );
            notifyOnMemberLeave.Active = ( bool ) Config.Settings.NotifyOnMemberLeave.Value;
            notifyOnMemberLeave.Toggled += delegate
            {
                Config.Settings.NotifyOnMemberLeave.Value = notifyOnMemberLeave.Active;
            };
            
            notifyOnMemberOnline = new CheckButton ( TextStrings.checkboxNotifyMemberOnline );
            notifyOnMemberOnline.Active = ( bool ) Config.Settings.NotifyOnMemberOnline.Value;
            notifyOnMemberOnline.Toggled += delegate
            {
                Config.Settings.NotifyOnMemberOnline.Value = notifyOnMemberOnline.Active;
            };
            
            notifyOnMemberOffline = new CheckButton ( TextStrings.checkboxNotifyMemberOffline );
            notifyOnMemberOffline.Active = ( bool ) Config.Settings.NotifyOnMemberOffline.Value;
            notifyOnMemberOffline.Toggled += delegate
            {
                Config.Settings.NotifyOnMemberOffline.Value = notifyOnMemberOffline.Active;
            };
            
            GroupBox trayBox = new GroupBox ( TextStrings.notifyGroup );
            trayBox.AddWidget ( showTrayIcon );
            trayBox.AddWidget ( hbox1 );
            trayBox.AddWidget ( notifyOnConnectionLoss );
            trayBox.AddWidget ( notifyOnMemberJoin );
            trayBox.AddWidget ( notifyOnMemberLeave );
            trayBox.AddWidget ( notifyOnMemberOnline );
            trayBox.AddWidget ( notifyOnMemberOffline );
            
            if ( !( bool ) Config.Settings.ShowTrayIcon.Value )
            {
                startInTray.Sensitive = false;
            }
            
            VBox appearanceBox = new VBox ();
            appearanceBox.Add ( trayBox );
            appearanceBox.Add ( new SpaceBox () );
            
            Box.BoxChild trayBoxC = ( ( Box.BoxChild ) ( appearanceBox [ trayBox ] ) );
            trayBoxC.Expand = false;
            
            
            commandsEditor = new CommandsEditor ();
            
            ipCombo = new ComboBox ( new string [] { TextStrings.protocolBoth, TextStrings.protocolIPv4, TextStrings.protocolIPv6 } );
            ipCombo.Active = ( int ) Utilities.ProtocolToInt ( ( string ) Config.Settings.Protocol.Value );
            ipCombo.Changed += delegate
            {
                Config.Settings.Protocol.Value = Utilities.ProtocolToString ( ipCombo.Active );            
            };
            
            Label ipLabel = new Label ();
            ipLabel.TextWithMnemonic = TextStrings.protocolLabel + "  ";
            ipLabel.MnemonicWidget = ipCombo;
            
            ipBox = new HBox ();
            ipBox.Add ( ipLabel );
            ipBox.Add ( ipCombo );
            
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( ipBox [ ipLabel ] ) );
            bc3.Expand = false;
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( ipBox [ ipCombo ] ) );
            bc4.Expand = false;
            
            hamachiBox = new GroupBox ( "Hamachi" );
            hamachiBox.AddWidget ( ipBox );
            
            connectOnStartup = new CheckButton ( TextStrings.connectOnStartup );
            connectOnStartup.Active = ( bool ) Config.Settings.ConnectOnStartup.Value;
            connectOnStartup.Toggled += delegate
            {
                Config.Settings.ConnectOnStartup.Value = connectOnStartup.Active;       
            };
            
            reconnectOnConnectionLoss = new CheckButton ( TextStrings.reconnectOnConnectionLoss );
            reconnectOnConnectionLoss.Active = ( bool ) Config.Settings.ReconnectOnConnectionLoss.Value;
            reconnectOnConnectionLoss.Toggled += delegate
            {
                Config.Settings.ReconnectOnConnectionLoss.Value = reconnectOnConnectionLoss.Active;       
            };
            
            disconnectOnQuit = new CheckButton ( TextStrings.disconnectOnQuit );
            disconnectOnQuit.Active = ( bool ) Config.Settings.DisconnectOnQuit.Value;
            disconnectOnQuit.Toggled += delegate
            {
                Config.Settings.DisconnectOnQuit.Value = disconnectOnQuit.Active;
            };
            
            intervalSpin = new SpinButton ( 0, 999, 1 );
            intervalSpin.Sensitive = ( bool ) Config.Settings.UpdateNetworkList.Value;
            intervalSpin.Value = ( double ) Config.Settings.UpdateInterval.Value;
            intervalSpin.ValueChanged += delegate
            {
                Config.Settings.UpdateInterval.Value = ( double ) intervalSpin.Value;
            };
            intervalSpin.MaxLength = 3;
            
            intervalLabel = new Label ();
            intervalLabel.MnemonicWidget = intervalSpin;
            intervalLabel.Xalign = 0;
            
            intervalBox = new HBox ();
            intervalBox.Add ( intervalSpin );
            intervalBox.Add ( intervalLabel );
            
            Box.BoxChild bc7 = ( ( Box.BoxChild ) ( intervalBox [ intervalLabel ] ) );
            bc7.Expand = false;
            
            Box.BoxChild bc8 = ( ( Box.BoxChild ) ( intervalBox [ intervalSpin ] ) );
            bc8.Expand = false;
            
            GroupBox behaviorBox = new GroupBox ( TextStrings.behaviorGroup );
            behaviorBox.AddWidget ( connectOnStartup );
            behaviorBox.AddWidget ( reconnectOnConnectionLoss );
            behaviorBox.AddWidget ( disconnectOnQuit );
            behaviorBox.AddWidget ( intervalBox );
            
            VBox systemBox = new VBox ();
            systemBox.Add ( hamachiBox );
            systemBox.Add ( behaviorBox );
            systemBox.Add ( new SpaceBox () );
            
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
            vbox.Spacing = 10;
            
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( vbox [ buttonBox ] ) );
            bc2.Expand = false;

            
            this.SetIntervalString ();
            this.VBox.Add ( vbox );
            vbox.ShowAll ();
            
        }
        
        
        public void Update ()
        {
            
            if ( Hamachi.IpModeCapable )
            {
                ipBox.Sensitive = true;
            }
            else
            {
                ipBox.Sensitive = false;
            }
            
        }
        
        
        public void SetIntervalString ()
        {
            
            string [] intervalString = TextStrings.updateNetworkListInterval ( ( int ) intervalSpin.Value ).Split ( new string [] { "%S" }, StringSplitOptions.None );
            
            if ( updateNetworkList != null )
            {
                intervalBox.Remove ( updateNetworkList );
            }
            
            updateNetworkList = new CheckButton ( intervalString [0] + " " );
            updateNetworkList.Active = ( bool ) Config.Settings.UpdateNetworkList.Value;
            updateNetworkList.Toggled += delegate
            {
                Config.Settings.UpdateNetworkList.Value = updateNetworkList.Active;
                intervalSpin.Sensitive = updateNetworkList.Active;
            };
            
            intervalBox.PackStart ( updateNetworkList, false, false, 0 );
            intervalBox.ReorderChild ( updateNetworkList, 0 );
            intervalBox.ShowAll ();
            
            intervalLabel.TextWithMnemonic = " " + intervalString [1];
            
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
