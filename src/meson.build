config = configure_file(
    input: 'config.vala.in',
    output: 'config.vala',
    configuration: conf
)

sources = [
    'bubble.vala',
    'command.vala',
    'connection.vala',
    'controller.vala',
    'debug.vala',
    'dialogs/add-edit-command.vala',
    'dialogs/attach.vala',
    'dialogs/base.vala',
    'dialogs/change-nick.vala',
    'dialogs/change-password.vala',
    'dialogs/confirm.vala',
    'dialogs/join-create-network.vala',
    'dialogs/message.vala',
    'global-actions.vala',
    'global-events.vala',
    'haguichi.vala',
    'hamachi.vala',
    'headerbar.vala',
    'inhibitor.vala',
    'key.vala',
    'member-event.vala',
    'member.vala',
    'menus/command-menu-item.vala',
    'menus/join-create-menu.vala',
    'menus/member-menu.vala',
    'menus/network-menu.vala',
    'network-view.vala',
    'network.vala',
    'preferences.vala',
    'session.vala',
    'settings.vala',
    'sidebar.vala',
    'status.vala',
    'text.vala',
    'utils.vala',
    'widgets/command-button.vala',
    'widgets/commands-editor.vala',
    'widgets/commands-editor-row.vala',
    'widgets/message-bar.vala',
    'widgets/message-box.vala',
    'widgets/preferences-box.vala',
    'widgets/preferences-label.vala',
    'widgets/preferences-switch.vala',
    'widgets/sidebar-entry.vala',
    'widgets/sidebar-label.vala',
    'window.vala'
]

deps = [
    dependency('gio-2.0'),
    dependency('gio-unix-2.0'),
    dependency('glib-2.0', version: '>= 2.42'),
    dependency('gobject-2.0'),
    dependency('gtk+-3.0', version: '>= 3.14'),
    dependency('libnotify', version: '>= 0.7.6')
]

gtk_version = dependency('gtk+-3.0').version()

if (gtk_version.version_compare('>= 3.20'))
    add_project_arguments(
        ['-D', 'GTK_3_20'],
        language: 'vala'
    )
endif

if (gtk_version.version_compare('>= 3.22'))
    add_project_arguments(
        ['-D', 'GTK_3_22'],
        language: 'vala'
    )
endif

if get_option('enable-appindicator')
    add_project_arguments(
        ['-D', 'ENABLE_APPINDICATOR'],
        language: 'vala'
    )
    deps += dependency('appindicator3-0.1')
    sources += [
        'indicator.vala',
        'indicator-menu.vala'
    ]
endif

if get_option('for-elementary')
    add_project_arguments(
        ['-D', 'FOR_ELEMENTARY'],
        language: 'vala'
    )
    deps += dependency('granite', version: '>= 0.5')
endif

executable(
    binary_name,
    config,
    sources,
    resources,
    dependencies: deps,
    install: true
)
