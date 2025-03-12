config = configure_file(
          input: 'config.vala.in',
         output: 'config.vala',
  configuration: conf
)

sources = [
  'application.vala',
  'bubble.vala',
  'command.vala',
  'connection.vala',
  'controller.vala',
  'custom-command.vala',
  'dialogs/add-edit-command.vala',
  'dialogs/attach.vala',
  'dialogs/change-access.vala',
  'dialogs/change-password.vala',
  'dialogs/confirm.vala',
  'dialogs/join-create-network.vala',
  'hamachi.vala',
  'indicator.vala',
  'inhibitor.vala',
  'member.vala',
  'member-event.vala',
  'network.vala',
  'network-list.vala',
  'preferences.vala',
  'session.vala',
  'sidebar.vala',
  'status.vala',
  'status-notifier-item.vala',
  'utils.vala',
  'widgets/command-entry-row.vala',
  'widgets/command-variable-button.vala',
  'widgets/commands-editor.vala',
  'widgets/commands-editor-row.vala',
  'widgets/sidebar-row.vala',
  'widgets/sidebar-page.vala',
  'widgets/throbber.vala',
  'window.vala',
]

deps = [
  dependency('gee-0.8',        version: '>= 0.20.6'),
  dependency('gio-unix-2.0',   version: '>= 2.78'),
  dependency('gtk4',           version: '>= 4.12'),
  dependency('libadwaita-1',   version: '>= 1.4'),
  dependency('libportal',      version: '>= 0.7.1'),
  dependency('libportal-gtk4', version: '>= 0.7.1'),
]

if dependency('libadwaita-1').version().version_compare('>= 1.6')
  add_project_arguments(
    ['-D', 'ADW_1_6'],
    language: 'vala'
  )
endif

if get_option('for-elementary')
  deps += dependency('granite-7')
  add_project_arguments(
    ['-D', 'FOR_ELEMENTARY'],
    language: 'vala'
  )
endif

executable(binary_name, config, sources, resources,
  dependencies: deps,
       install: true,
)
