project(
    'haguichi',
    ['vala', 'c'],
    version: '1.3.9',
    default_options: ['prefix=/usr', 'sysconfdir=/etc'],
    meson_version: '>= 0.40'
)

app_name    = meson.project_name()
app_id      = 'com.github.ztefn.' + app_name
app_version = meson.project_version()
binary_name = app_name
icon_name   = app_name

add_project_arguments(
    '-DGETTEXT_PACKAGE="' + app_name + '"',
    language: 'c'
)

conf = configuration_data()
conf.set('PACKAGE_NAME', app_name)
conf.set('VERSION', app_version)
conf.set('BINARY_NAME', binary_name)
conf.set('ICON_NAME', icon_name)
conf.set('GETTEXT_PACKAGE', app_name)
conf.set('DATADIR', join_paths(get_option('prefix'), get_option('datadir')))
conf.set('LOCALEDIR', join_paths(get_option('prefix'), get_option('localedir')))

config = configure_file(
    input: 'src/config.vala.in',
    output: 'config.vala',
    configuration: conf
)

subdir('data')
subdir('po')

sources = [
    config,
    'src/bubble.vala',
    'src/command.vala',
    'src/connection.vala',
    'src/controller.vala',
    'src/debug.vala',
    'src/dialogs/add-edit-command.vala',
    'src/dialogs/attach.vala',
    'src/dialogs/base.vala',
    'src/dialogs/change-nick.vala',
    'src/dialogs/change-password.vala',
    'src/dialogs/confirm.vala',
    'src/dialogs/join-create-network.vala',
    'src/dialogs/message.vala',
    'src/global-actions.vala',
    'src/global-events.vala',
    'src/haguichi.vala',
    'src/hamachi.vala',
    'src/headerbar.vala',
    'src/inhibitor.vala',
    'src/key.vala',
    'src/member-event.vala',
    'src/member.vala',
    'src/menus/command-menu-item.vala',
    'src/menus/join-create-menu.vala',
    'src/menus/member-menu.vala',
    'src/menus/network-menu.vala',
    'src/network-view.vala',
    'src/network.vala',
    'src/preferences.vala',
    'src/session.vala',
    'src/settings.vala',
    'src/sidebar.vala',
    'src/status.vala',
    'src/text.vala',
    'src/utils.vala',
    'src/widgets/command-button.vala',
    'src/widgets/commands-editor.vala',
    'src/widgets/group-box.vala',
    'src/widgets/message-bar.vala',
    'src/widgets/message-box.vala',
    'src/widgets/sidebar-entry.vala',
    'src/widgets/sidebar-label.vala',
    'src/window.vala'
]

deps = [
    dependency('gio-2.0'),
    dependency('gio-unix-2.0'),
    dependency('glib-2.0', version: '>= 2.42'),
    dependency('gobject-2.0'),
    dependency('gtk+-3.0', version: '>= 3.14'),
    dependency('libnotify', version: '>= 0.7.6')
]

if get_option('enable-appindicator')
    add_project_arguments(
        ['-D', 'ENABLE_APPINDICATOR'],
        language: 'vala'
    )
    deps += dependency('appindicator3-0.1')
    sources += [
        'src/indicator.vala',
        'src/indicator-menu.vala'
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
    sources,
    dependencies: deps,
    install: true
)

meson.add_install_script('meson_post_install.py')
