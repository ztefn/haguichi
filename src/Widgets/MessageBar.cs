/*
 * Haguichi, a graphical frontend for Hamachi.
 * Copyright © 2007-2014 Stephen Brandt <stephen@stephenbrandt.com>
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


namespace Widgets
{
    
    public class MessageBar : VBox
    {
        
        private Widget parent;
        private MessageType messageType;
        private uint timerID;
        
        private Gdk.Color errorColor;
        private Gdk.Color infoColor;
        private Gdk.Color otherColor;
        private Gdk.Color questionColor;
        private Gdk.Color warningColor;
        
        private HBox topBox;
        private HBox bottomBox;
        private Image image;
        private WrapLabel label;
        private Button closeButton;
        
        private ButtonBox buttonBox;
        
        
        public MessageBar () : base ()
        {
            
            image = new Image ();
            image.Yalign = 0;
            
            VBox closeBox = new VBox ();
            
            closeButton = new Button ();
            closeButton.Image = new Image ( Stock.Close, IconSize.Button );
            closeButton.Relief = ReliefStyle.None;
            closeButton.Clicked += delegate
            {
                this.Hide ();
            };
            
            closeBox.Add ( closeButton );
            
            Box.BoxChild bc0 = ( ( Box.BoxChild ) ( closeBox [ closeButton ] ) );
            bc0.Expand = false;
            
            
            label = new WrapLabel ();
            
            VBox labelBox = new VBox ();
            labelBox.Add ( new HBox () );
            labelBox.Add ( label );
            labelBox.Add ( new HBox () );
            
            
            topBox = new HBox ();
            topBox.Add ( image );
            topBox.Add ( labelBox );
            topBox.Add ( closeBox );
            
            Box.BoxChild bc1 = ( ( Box.BoxChild ) ( topBox [ image ] ) );
            bc1.Expand = false;
            
            Box.BoxChild bc2 = ( ( Box.BoxChild ) ( topBox [ labelBox ] ) );
            bc2.Padding = 6;
            
            Box.BoxChild bc3 = ( ( Box.BoxChild ) ( topBox [ closeBox ] ) );
            bc3.Expand = false;
            
            
            buttonBox = new HButtonBox ();
            buttonBox.Layout = ButtonBoxStyle.End;
            buttonBox.Spacing = 6;
            
            
            bottomBox = new HBox ();
            bottomBox.Add ( buttonBox );
            
            Box.BoxChild bc4 = ( ( Box.BoxChild ) ( bottomBox [ buttonBox ] ) );
            bc4.Padding = 3;
            
            
            this.Add ( topBox );
            this.Add ( bottomBox );
            
            Box.BoxChild bc5 = ( ( Box.BoxChild ) ( this [ bottomBox ] ) );
            bc5.Padding = 3;
            
            
            this.BorderWidth = 6;
            
            this.ShowAll ();
            
            Window win = new Window ( WindowType.Popup );
            win.HeightRequest = 1;
            win.WidthRequest = 1;
            //win.Name = "gtk-tooltips"; /* Uncomment this line if you want to use tooltip colors */
            win.Show ();
            
            otherColor = win.Style.Backgrounds [(int) StateType.Normal];
            
            win.Hide ();
            
            errorColor    = new Gdk.Color ( 240,  56,  56 );
            infoColor     = new Gdk.Color ( 255, 255, 191 );
            questionColor = new Gdk.Color ( 140, 176, 215 );
            warningColor  = new Gdk.Color ( 252, 175,  62 );
            
        }
        
        
        public void SetMessage ( string header, string message, MessageType messageType )
        {
            
            foreach ( var child in buttonBox.Children )
            {
                buttonBox.Remove (child);
                child.Destroy ();
            }
            
            string markup = "";
            
            if ( header != null )
            {
                markup += "<b>{0}</b>";
            }
            if ( message != null )
            {
                markup += "\n<span size=\"smaller\">{1}</span>";
            }
            
            label.Markup = String.Format ( markup, header, message );
            closeButton.Visible = false;
            
            SetMessageType ( messageType );
            
            bottomBox.Hide ();
            
            parent = this.Parent;
            parent.SizeAllocated -= HandleParentSizeAllocated;
            parent.SizeAllocated += HandleParentSizeAllocated;
            
            this.Show ();
            
        }
        
        
        public void SetMessage ( string header, string message, MessageType messageType, bool showClose )
        {
            
            SetMessage ( header, message, messageType );
            
            closeButton.Visible = showClose;
            
        }
        
        
        public void SetMessage ( string header, string message, MessageType messageType, int timeout )
        {
            
            SetMessage ( header, message, messageType );
            
            SetTimeout ( timeout );
            
        }
        
        
        public void SetTimeout ( int timeout )
        {
            
            if ( timerID > 0 )
            {
                GLib.Source.Remove( timerID );
                timerID = 0;
            }
            
            timerID = GLib.Timeout.Add ( ( uint ) timeout, () =>
            {
                this.Hide ();
                timerID = 0;
                
                return false;
            });
            
        }
        
        
        public void AddButton ( Button button )
        {
            
            ButtonBox.Add ( button );
            button.Show ();
            bottomBox.Show ();
            
        }
        
        
        public ButtonBox ButtonBox
        {
            
            get
            {
                return buttonBox;
            }
            set
            {
                buttonBox = value;
            }
            
        }
        
        
        public MessageType MessageType
        {
            
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
                SetMessageType ( messageType );
            }
            
        }
        
        
        private void SetMessageType ( MessageType messageType )
        {
            
            this.messageType = messageType;
            
            switch ( messageType )
            {
                
                case MessageType.Error:
                    
                    this.ModifyBg ( StateType.Normal, errorColor );
                    
                    image.SetFromStock ( Stock.DialogError, IconSize.LargeToolbar );
                    
                    break;
                    
                case MessageType.Info:
                    
                    this.ModifyBg ( StateType.Normal, infoColor );
                    
                    image.SetFromStock ( Stock.DialogInfo, IconSize.LargeToolbar );
                    
                    break;
                    
                case MessageType.Other:
                    
                    this.ModifyBg ( StateType.Normal, otherColor );
                    
                    image.Clear ();
                    
                    break;
                    
                case MessageType.Question:
                    
                    this.ModifyBg ( StateType.Normal, questionColor );
                    
                    image.SetFromStock ( Stock.DialogQuestion, IconSize.LargeToolbar );
                    
                    break;
                    
                case MessageType.Warning:
                    
                    this.ModifyBg ( StateType.Normal, warningColor );
                    
                    image.SetFromStock ( Stock.DialogWarning, IconSize.LargeToolbar );
                    
                    break;
                    
            }
            
        }
        
        
        private void HandleParentSizeAllocated ( object o, SizeAllocatedArgs args )
        {
            
            parent.QueueDrawArea ( parent.Allocation.X, parent.Allocation.Y, parent.Allocation.Width, parent.Allocation.Height );
            
        }
        
        
        protected override bool OnExposeEvent ( Gdk.EventExpose evnt )
        {
            
            Style.PaintFlatBox ( Style,
                                 evnt.Window,
                                 StateType.Normal,
                                 ShadowType.Out,
                                 evnt.Area,
                                 this,
                                 "tooltip", 
                                 Allocation.X,
                                 Allocation.Y,
                                 Allocation.Width,
                                 Allocation.Height );
            
            return base.OnExposeEvent ( evnt );
            
        }
        
    }
    
}
