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
using Mono.Unix;


public static class TextStrings
{
    
    public static string appName                = "Haguichi";
    public static string appVersion             = "1.0.4";
    public static string appWebsite             = "http://www.haguichi.net/";
    public static string appWebsiteLabel;
    public static string appComments;
    public static string appDescription;
    public static string appCopyright           = "Copyright © 2007-2010 Stephen Brandt";
    public static string appLicense             =
@"Haguichi is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published
by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

Haguichi is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Haguichi; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.";
    
    public static string appInfo                =
@"Haguichi, a graphical frontend for Hamachi.
Copyright © 2007-2010 Stephen Brandt <stephen@stephenbrandt.com>";
            
    public static string appHelp                =
@"Usage:
  haguichi [options]

Options:
  -h, --help              Show this help
  -d, --debug             Show debug information
  -v, --version           Show version information
  --license               Show license information
  --demo                  Run in demo mode

" + appInfo + "\n";
    
    public static string[] appAuthors           = new string [] { "Stephen Brandt <stephen@stephenbrandt.com>" };
    public static string[] appArtists           = new string [] { "Stephen Brandt <stephen@stephenbrandt.com>", "Lapo Calamandrei <calamandrei@gmail.com>" };
    public static string[] appDocumenters       = new string [] { "" };
    public static string   appTranslatorCredits =
@"Deutsch (de)
    Jannik Heller

Español (es)
    Eduardo Parra
    David Fernández

Français (fr)
    Gabriel U.
    Emilien Klein

Italiano (it)
    Enrico Grassi

日本語 (ja)
    Satoru Matsumoto

Қазақ (kk)
    jmb_kz

Nederlands (nl)
    Stephen Brandt

Русский (ru)
    Sergey Korolev
    Sergey Sedov

Svenska (sv)
    Daniel Holm
    Daniel Nylander
    David Bengtsson
";
    
    public static string helpURL                = "http://www.haguichi.net/redirect/?version=" + appVersion + "&action=help";
    public static string getHamachiURL          = "http://www.haguichi.net/redirect/?version=" + appVersion + "&action=get-hamachi";
    
    public static string yes;
    public static string no;
    
    public static string address;
    public static string id;
    public static string clientId;
    public static string networkId;
    public static string status;
    public static string tunnel;
    public static string anonymous;
    public static string nickLabel;
    public static string nick;
    public static string nameLabel;
    public static string passwordLabel;
    public static string version;
    
    public static string offline;
    public static string online;
    public static string unreachable;
    public static string connected;
    public static string disconnected;
    public static string connecting;
    public static string starting;
    public static string loggingIn;
    public static string runningTuncfg;
    public static string gettingNicks;
    public static string updating;
    
    public static string connectAutomatically;
    
    public static string downloadLabel;
    public static string configureLabel;
    
    public static string notConfigured;
    public static string notConfiguredHeading;
    public static string notConfiguredMessage;
    
    public static string notInstalled;
    public static string notInstalledHeading;
    public static string notInstalledMessage;
    
    public static string members;
    public static string memberCount;
    public static string owner;
    public static string you;
    public static string unknown;
    public static string unavailable;
    public static string unapproved;
    public static string locked;
    public static string approval;
    public static string manually;
    public static string automatically;
    
    public static string clientLabel;
    public static string editLabel;
    public static string viewLabel;
    public static string helpLabel;
    
    public static string onlineHelpLabel;
    public static string createNetworkLabel;
    public static string joinNetworkLabel;
    
    public static string goOnlineLabel;
    public static string goOfflineLabel;
    public static string leaveLabel;
    public static string lockedLabel;
    public static string approvalLabel;
    public static string autoLabel;
    public static string manualLabel;
    public static string copyNetworkIdLabel;
    
    public static string browseLabel;
    public static string pingLabel;
    public static string vncLabel;
    public static string approveLabel;
    public static string rejectLabel;
    public static string evictLabel;
    public static string copyAddressLabel;
    public static string copyClientIdLabel;
    
    public static string changeLabel;
    public static string changingLabel;
    
    public static string changeNickLabel;
    public static string changeNickTitle;
    public static string changeNickMessage;
    public static string changeNicknameTooltip;
    
    public static string changePasswordLabel;
    public static string changePasswordTitle;
    public static string changePasswordMessage;
    
    public static string accountLabel;
    public static string account;
    
    public static string attachButtonLabel;
    public static string attachMenuLabel;
    public static string attachTitle;
    public static string attachMessage;
    public static string attachWithNetworksCheckbox;
    public static string attachErrorHeading;
    public static string attachErrorAccountNotFound;
    
    public static string joinLabel;
    public static string joiningLabel;
    public static string joinNetworkTitle;
    public static string joinNetworkMessage;
    
    public static string createLabel;
    public static string creatingLabel;
    public static string createNetworkTitle;
    public static string createNetworkMessage;
    
    public static string checkboxGoOnlineInNewNetwork;

    public static string errorNetworkNotFound;
    public static string errorInvalidPassword;
    public static string errorNetworkFull;
    public static string errorNetworkLocked;
    public static string errorNetworkAlreadyJoined;
    public static string errorUnknown;
    public static string errorNetworkNameTooShort;
    public static string errorNetworkNameTaken;
    
    public static string sendRequestLabel;
    public static string sendRequestTitle;
    public static string sendRequestMessage;
    
    public static string preferencesTitle;
    public static string informationTitle;
    
    public static string confirmDeleteNetworkHeading;
    public static string confirmDeleteNetworkMessage;
    
    public static string confirmLeaveNetworkHeading;
    public static string confirmLeaveNetworkMessage;
    
    public static string confirmEvictMemberHeading;
    public static string confirmEvictMemberMessage;
    
    public static string failedDeleteNetworkHeading;
    public static string failedDeleteNetworkMessage;
    
    public static string failedLeaveNetworkHeading;
    public static string failedLeaveNetworkMessageIsOwner;
    public static string failedLeaveNetworkMessageDenied;
    
    public static string failedEvictMemberHeading;
    public static string failedEvictMemberMessage;
    
    public static string generalTab;
    public static string commandsTab;
    public static string desktopTab;
    
    public static string notifyGroup;
    public static string behaviorGroup;
    
    public static string dataPathLabel;
    public static string chooseFolderTitle;
    
    public static string checkboxShowOfflineMembers;
    public static string sortByName;
    public static string sortByStatus;
    public static string checkboxShowStatusbar;
    
    public static string checkboxShowTrayIcon;
    public static string checkboxStartInTray;
    
    public static string connectOnStartup;
    public static string reconnectOnConnectionLoss;
    public static string disconnectOnQuit;
    
    public static string checkboxNotifyMemberJoin;
    public static string checkboxNotifyMemberLeave;
    public static string checkboxNotifyMemberOnline;
    public static string checkboxNotifyMemberOffline;

    public static string runTuncfgHeading;
    public static string runTuncfgMessage;
    public static string runLabel;
    public static string checkboxAskBeforeRunningTuncfg;
    public static string checkboxAskBeforeRunningTuncfg2;

    public static string connectErrorHeading;
    public static string connectErrorLoginFailed;
    public static string connectErrorConnectionRefused;
    public static string connectErrorNoInternetConnection;

    public static string configErrorHeading;
    public static string configErrorMessage;

    public static string notifyMemberOnlineHeading;
    public static string notifyMemberOnlineMessage;
    public static string notifyMemberOfflineHeading;
    public static string notifyMemberOfflineMessage;
    public static string notifyMemberJoinedHeading;
    public static string notifyMemberJoinedMessage;
    public static string notifyMemberLeftHeading;
    public static string notifyMemberLeftMessage;
    
    public static string customizeCommands;
    public static string command;
    public static string available;
    
    public static string addCommandTitle;
    public static string editCommandTitle;
    public static string commandLabel;
    public static string labelLabel;
    public static string commandInfo;
    
    public static string chooseIconTip;
    public static string chooseIconTitle;
    public static string noIconLabel;
    
    public static string isDefault;
    public static string defaultLabel;
    
    public static string revertTip;
    public static string revertHeading;
    public static string revertMessage;
    
    public static string moveUpTip;
    public static string moveDownTip;
    
    
    public static void Init ()
    {

        appWebsiteLabel                     = Catalog.GetString ( "Haguichi Website" );
        appComments                         = Catalog.GetString ( "A graphical frontend for Hamachi." );
        appDescription                      = Catalog.GetString ( "Join and create local networks over the Internet" );
        
        yes                                 = Catalog.GetString ( "Yes" );
        no                                  = Catalog.GetString ( "No" );
        
        address                             = Catalog.GetString ( "Address:" );
        id                                  = Catalog.GetString ( "ID:" );
        clientId                            = Catalog.GetString ( "Client ID:" );
        networkId                           = Catalog.GetString ( "Network ID:" );
        status                              = Catalog.GetString ( "Status:" );
        tunnel                              = Catalog.GetString ( "Tunnel:" );
        anonymous                           = Catalog.GetString ( "Anonymous" );
        nickLabel                           = Catalog.GetString ( "_Nickname:" );
        nick                                = Utilities.RemoveMnemonics ( nickLabel ); // "Nickname:"
        nameLabel                           = Catalog.GetString ( "_Name:" );
        passwordLabel                       = Catalog.GetString ( "_Password:" );
        version                             = Catalog.GetString ( "Version:" );
        
        offline                             = Catalog.GetString ( "Offline" );
        online                              = Catalog.GetString ( "Online" );
        unreachable                         = Catalog.GetString ( "Unreachable" );
        connected                           = Catalog.GetString ( "Connected" );
        disconnected                        = Catalog.GetString ( "Disconnected" );
        connecting                          = Catalog.GetString ( "Connecting..." );
        starting                            = Catalog.GetString ( "Starting Hamachi..." );
        loggingIn                           = Catalog.GetString ( "Logging in..." );
        runningTuncfg                       = Catalog.GetString ( "Running tuncfg..." );
        gettingNicks                        = Catalog.GetString ( "Getting nicknames..." );
        updating                            = Catalog.GetString ( "Updating..." );
        
        connectAutomatically                = Catalog.GetString ( "Connect a_utomatically" );
        
        downloadLabel                       = Catalog.GetString ( "_Download" );
        configureLabel                      = Catalog.GetString ( "C_onfigure" );
        
        notConfigured                       = Catalog.GetString ( "Not configured" );
        notConfiguredHeading                = Catalog.GetString ( "Hamachi is not configured" );
        notConfiguredMessage                = Catalog.GetString ( "You need to configure Hamachi before you can connect." );

        notInstalled                        = Catalog.GetString ( "Not installed" );
        notInstalledHeading                 = Catalog.GetString ( "Hamachi is not installed" );
        notInstalledMessage                 = Catalog.GetString ( "Please download Hamachi and follow the installation instructions." );
        
        members                             = Catalog.GetString ( "Members:" );
        memberCount                         = Catalog.GetString ( "{0} online, {1} total" );
        owner                               = Catalog.GetString ( "Owner:" );
        you                                 = Catalog.GetString ( "You" );
        unknown                             = Catalog.GetString ( "Unknown" );
        unavailable                         = Catalog.GetString ( "Unavailable" );
        unapproved                          = Catalog.GetString ( "Awaiting approval" );
        locked                              = Catalog.GetString ( "Locked:" );
        approval                            = Catalog.GetString ( "Approval:" );
        manually                            = Catalog.GetString ( "Manually" );
        automatically                       = Catalog.GetString ( "Automatically" );
        
        clientLabel                         = Catalog.GetString ( "_Client" );
        editLabel                           = Catalog.GetString ( "_Edit" );
        viewLabel                           = Catalog.GetString ( "_View" );
        helpLabel                           = Catalog.GetString ( "_Help" );
        
        onlineHelpLabel                     = Catalog.GetString ( "_Online Help" );
        createNetworkLabel                  = Catalog.GetString ( "_Create Network..." );
        joinNetworkLabel                    = Catalog.GetString ( "_Join Network..." );
        
        goOnlineLabel                       = Catalog.GetString ( "_Go Online" );
        goOfflineLabel                      = Catalog.GetString ( "_Go Offline" );
        leaveLabel                          = Catalog.GetString ( "_Leave" );
        lockedLabel                         = Catalog.GetString ( "_Locked" );
        approvalLabel                       = Catalog.GetString ( "_New Member Approval" );
        autoLabel                           = Catalog.GetString ( "_Automatically" );
        manualLabel                         = Catalog.GetString ( "_Manually" );
        copyNetworkIdLabel                  = Catalog.GetString ( "_Copy Network ID" );
        
        browseLabel                         = Catalog.GetString ( "_Browse Shares" );
        pingLabel                           = Catalog.GetString ( "_Ping" );
        vncLabel                            = Catalog.GetString ( "_View Remote Desktop" );
        approveLabel                        = Catalog.GetString ( "_Approve" );
        rejectLabel                         = Catalog.GetString ( "_Reject" );
        evictLabel                          = Catalog.GetString ( "_Evict" );
        copyAddressLabel                    = Catalog.GetString ( "_Copy Address" );
        copyClientIdLabel                   = Catalog.GetString ( "_Copy Client ID" );
        
        changeLabel                         = Catalog.GetString ( "C_hange" );
        changingLabel                       = Catalog.GetString ( "Changing..." );
        
        changeNickLabel                     = Catalog.GetString ( "Change _Nickname..." );
        changeNickTitle                     = Catalog.GetString ( "Change Nickname" );
        changeNickMessage                   = Catalog.GetString ( "Please enter the nickname you want to use." );
        changeNicknameTooltip               = Catalog.GetString ( "Click to change your nickname" );
        
        changePasswordLabel                 = Catalog.GetString ( "Change _Password..." );
        changePasswordTitle                 = Catalog.GetString ( "Change Password" );
        changePasswordMessage               = Catalog.GetString ( "Please enter the new password for this network." );
        
        accountLabel                        = Catalog.GetString ( "_Account:" );
        account                             = Utilities.RemoveMnemonics ( accountLabel ); // "Account:"
        
        attachButtonLabel                   = Catalog.GetString ( "_Attach" );
        attachMenuLabel                     = Catalog.GetString ( "_Attach to Account..." );
        attachTitle                         = Catalog.GetString ( "Attach to Account" );
        attachMessage                       = Catalog.GetString ( "Please enter the account you want to attach this client to." );
        attachWithNetworksCheckbox          = Catalog.GetString ( "_Include all networks created by this client" );
        attachErrorHeading                  = Catalog.GetString ( "Error attaching" );
        attachErrorAccountNotFound          = Catalog.GetString ( "Account not found." );
        
        joinLabel                           = Catalog.GetString ( "_Join" );
        joiningLabel                        = Catalog.GetString ( "Joining..." );
        joinNetworkTitle                    = Catalog.GetString ( "Join Network" );
        joinNetworkMessage                  = Catalog.GetString ( "Please enter the name and password for the\nnetwork you want to join." );
                
        createLabel                         = Catalog.GetString ( "C_reate" );
        creatingLabel                       = Catalog.GetString ( "Creating..." );
        createNetworkTitle                  = Catalog.GetString ( "Create Network" );
        createNetworkMessage                = Catalog.GetString ( "Please enter the name and password for the\nnetwork you want to create." );
        
        checkboxGoOnlineInNewNetwork        = Catalog.GetString ( "_Go directly online in the network" );
        
        errorNetworkNotFound                = Catalog.GetString ( "Network not found" );
        errorInvalidPassword                = Catalog.GetString ( "Invalid password" );
        errorNetworkFull                    = Catalog.GetString ( "Network is full" );
        errorNetworkLocked                  = Catalog.GetString ( "Network is locked" );
        errorNetworkAlreadyJoined           = Catalog.GetString ( "Network already joined" );
        errorUnknown                        = Catalog.GetString ( "Unknown error" );
        errorNetworkNameTooShort            = Catalog.GetString ( "Network name too short" );
        errorNetworkNameTaken               = Catalog.GetString ( "Network name is already taken" );
        
        sendRequestLabel                    = Catalog.GetString ( "_Send Request" );
        sendRequestTitle                    = Catalog.GetString ( "Send join request?" );
        sendRequestMessage                  = Catalog.GetString ( "This network requires manual approval of new members by the owner.\nWould you like to send a join request?" );
        
        preferencesTitle                    = Catalog.GetString ( "Preferences" );
        informationTitle                    = Catalog.GetString ( "Information" );
        
        confirmDeleteNetworkHeading         = Catalog.GetString ( "Delete network '{0}'?" );
        confirmDeleteNetworkMessage         = Catalog.GetString ( "Are you sure you want to delete network '{0}'?" );
        
        confirmLeaveNetworkHeading          = Catalog.GetString ( "Leave network '{0}'?" );
        confirmLeaveNetworkMessage          = Catalog.GetString ( "Are you sure you want to leave network '{0}'?" );
        
        confirmEvictMemberHeading           = Catalog.GetString ( "Evict member '{0}'?" );
        confirmEvictMemberMessage           = Catalog.GetString ( "Are you sure you want to evict member '{0}'\nfrom network '{1}'?" );
        
        failedDeleteNetworkHeading          = Catalog.GetString ( "Failed deleting network '{0}'" );
        failedDeleteNetworkMessage          = Catalog.GetString ( "You are not the owner of this network." );
        
        failedLeaveNetworkHeading           = Catalog.GetString ( "Failed leaving network '{0}'" );
        failedLeaveNetworkMessageIsOwner    = Catalog.GetString ( "You are the owner of this network." );
        failedLeaveNetworkMessageDenied     = Catalog.GetString ( "You don't have permission to leave this network." );
        
        failedEvictMemberHeading            = Catalog.GetString ( "Failed evicting member '{0}'" );
        failedEvictMemberMessage            = Catalog.GetString ( "You are not the owner of network '{0}'." );
        
        generalTab                          = Catalog.GetString ( "General" );
        desktopTab                          = Catalog.GetString ( "Desktop" );
        commandsTab                         = Catalog.GetString ( "Commands" );
        
        notifyGroup                         = Catalog.GetString ( "Notification Area" );
        behaviorGroup                       = Catalog.GetString ( "Behavior" );

        dataPathLabel                       = Catalog.GetString ( "_Account data folder:" );
        chooseFolderTitle                   = Catalog.GetString ( "Choose Folder" );
        
        checkboxShowOfflineMembers          = Catalog.GetString ( "_Offline Members" );
        sortByName                          = Catalog.GetString ( "Sort by _Name" );
        sortByStatus                        = Catalog.GetString ( "Sort by _Status" );        
        checkboxShowStatusbar               = Catalog.GetString ( "S_tatusbar" );
        
        checkboxShowTrayIcon                = Catalog.GetString ( "_Show Haguichi in the notification area" );
        checkboxStartInTray                 = Catalog.GetString ( "Start Haguichi _minimized in the notification area" );
        
        connectOnStartup                    = Catalog.GetString ( "C_onnect automatically on startup" );
        reconnectOnConnectionLoss           = Catalog.GetString ( "_Reconnect automatically when connection is lost" );
        disconnectOnQuit                    = Catalog.GetString ( "_Disconnect on quit" );
        
        checkboxNotifyMemberJoin            = Catalog.GetString ( "Display notification when a member _joins" );
        checkboxNotifyMemberLeave           = Catalog.GetString ( "Display notification when a member _leaves" );
        checkboxNotifyMemberOnline          = Catalog.GetString ( "Display notification when a member comes o_nline" );
        checkboxNotifyMemberOffline         = Catalog.GetString ( "Display notification when a member goes o_ffline" );

        runTuncfgHeading                    = Catalog.GetString ( "Do you want to run tuncfg?" );
        runTuncfgMessage                    = Catalog.GetString ( "Hamachi needs to run tuncfg to be able to connect.\nRunning tuncfg requires administrative privileges." );
        runLabel                            = Catalog.GetString ( "_Run" );
        checkboxAskBeforeRunningTuncfg      = Catalog.GetString ( "_Always ask before running tuncfg" );
        checkboxAskBeforeRunningTuncfg2     = Catalog.GetString ( "A_sk confirmation before running tuncfg" );
        
        connectErrorHeading                 = Catalog.GetString ( "Error connecting" );
        connectErrorLoginFailed             = Catalog.GetString ( "Hamachi login failed." );
        connectErrorConnectionRefused       = Catalog.GetString ( "Connection refused." );
        connectErrorNoInternetConnection    = Catalog.GetString ( "No internet connection." );
        
        configErrorHeading                  = Catalog.GetString ( "Loading configuration failed" );
        configErrorMessage                  = Catalog.GetString ( "Hamachi could not launch due to a configuration load failure." );
        
        notifyMemberOnlineHeading           = Catalog.GetString ( "Member online" );
        notifyMemberOnlineMessage           = Catalog.GetString ( "{0} came online in the network {1}" );
        notifyMemberOfflineHeading          = Catalog.GetString ( "Member offline" );
        notifyMemberOfflineMessage          = Catalog.GetString ( "{0} went offline in the network {1}" );
        notifyMemberJoinedHeading           = Catalog.GetString ( "Member joined" );
        notifyMemberJoinedMessage           = Catalog.GetString ( "{0} joined the network {1}" );
        notifyMemberLeftHeading             = Catalog.GetString ( "Member left" );
        notifyMemberLeftMessage             = Catalog.GetString ( "{0} left the network {1}" );
        
        customizeCommands                   = Catalog.GetString ( "A_vailable commands:" );
        command                             = Catalog.GetString ( "Command" );
        available                           = Catalog.GetString ( "Available" );
        
        addCommandTitle                     = Catalog.GetString ( "Add Command" );
        editCommandTitle                    = Catalog.GetString ( "Edit Command" );
        commandLabel                        = Catalog.GetString ( "_Command:" );
        labelLabel                          = Catalog.GetString ( "_Label:" );
        commandInfo                         = Catalog.GetString ( "Use %A for address and %N for nickname" );
        
        chooseIconTip                       = Catalog.GetString ( "Click to choose an icon" );
        chooseIconTitle                     = Catalog.GetString ( "Choose Icon" );
        noIconLabel                         = Catalog.GetString ( "_No Icon" );
        
        isDefault                           = Catalog.GetString ( "Default" );
        defaultLabel                        = Catalog.GetString ( "_Default" );
        
        revertTip                           = Catalog.GetString ( "Restore the default commands" );
        revertHeading                       = Catalog.GetString ( "Revert changes?" );
        revertMessage                       = Catalog.GetString ( "Are you sure you wish to revert all changes\nand thereby restore the default commands?" );
        
        moveUpTip                           = Catalog.GetString ( "Move up");
        moveDownTip                         = Catalog.GetString ( "Move down" );
        
    }

}
