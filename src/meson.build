sources = [
  'Command.cs',
  'CommandsEditor.cs',
  'Connection.cs',
  'Controller.cs',
  'Debug.cs',
  'GlobalEvents.cs',
  'Hamachi.cs',
  'Main.cs',
  'MainWindow.cs',
  'Member.cs',
  'Network.cs',
  'NetworkView.cs',
  'Notify.cs',
  'PanelIcon.cs',
  'Platform.cs',
  'Session.cs',
  'Status.cs',
  'TextStrings.cs',
  'Utilities.cs',
  'Config/Key.cs',
  'Config/Settings.cs',
  'Dialogs/About.cs',
  'Dialogs/AddEditCommand.cs',
  'Dialogs/Attach.cs',
  'Dialogs/Base.cs',
  'Dialogs/ChangeNick.cs',
  'Dialogs/ChangePassword.cs',
  'Dialogs/Confirm.cs',
  'Dialogs/Information.cs',
  'Dialogs/JoinCreate.cs',
  'Dialogs/Message.cs',
  'Dialogs/Preferences.cs',
  'Menus/CommandMenuItem.cs',
  'Menus/MemberMenu.cs',
  'Menus/Menubar.cs',
  'Menus/NetworkMenu.cs',
  'Menus/QuickMenu.cs',
  'Widgets/GroupBox.cs',
  'Widgets/IndentHBox.cs',
  'Widgets/MessageBar.cs',
  'Widgets/SpaceBox.cs',
  'Widgets/WrapLabel.cs',
]

deps = [
  dependency('glib-sharp-2.0',      version: '>= 2.12.9'),
  dependency('gtk-sharp-2.0',       version: '>= 2.12.9'),
  dependency('gconf-sharp-2.0',     version: '>= 2.24.0'),
  dependency('dbus-sharp-2.0',      version: '>= 0.8.0'),
  dependency('dbus-sharp-glib-2.0', version: '>= 0.6.0'),
  dependency('notify-sharp',        version: '>= 0.4.0'),
]

args = ['-reference:Mono.Posix']
icon_path = '../data/icons/hicolor/'

foreach size : ['16', '22', '24', '32', '48', '64']
  args += '-resource:' + icon_path + size + 'x' + size + '/' + icon_name + '.png,' + size + 'x' + size + '.' + icon_name
endforeach

foreach size : ['16', '22', '24', '48']
  foreach state : ['connected', 'connecting-1', 'connecting-2', 'connecting-3', 'disconnected']
    args += '-resource:' + icon_path + size + 'x' + size + '/' + icon_name + '-' + state + '.png,' + size + 'x' + size + '.' + state
  endforeach
endforeach

foreach size : ['12', '18']
  foreach node : ['error', 'offline', 'online', 'online-relayed', 'unapproved', 'unreachable']
    args += '-resource:' + icon_path + size + 'x' + size + '/' + icon_name + '-node-' + node + '.png,' + size + 'x' + size + '.node-' + node
  endforeach
endforeach

executable(app_name, sources,
  dependencies: deps,
       cs_args: args,
       install: true,
)
