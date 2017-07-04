project('haguichi', ['vala', 'c'],
    version: '1.3.8',
    default_options: ['prefix=/usr', 'sysconfdir=/etc'],
    meson_version: '>= 0.40'
)

add_project_arguments(
    '-DPACKAGE_NAME="' + meson.project_name() + '"',
    '-DVERSION="' + meson.project_version() + '"',
    '-DGETTEXT_PACKAGE="' + meson.project_name() + '"',
    '-DDATADIR="' + join_paths(get_option('prefix'), get_option('datadir')) + '"',
    '-DLOCALEDIR="' + join_paths(get_option('prefix'), get_option('localedir')) + '"',
    language: 'c'
)

subdir('data')
subdir('po')

sources = [
    'vapi/config.vapi',
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
    dependency('glib-2.0', version: '>=2.42'),
    dependency('gobject-2.0'),
    dependency('gtk+-3.0', version: '>=3.14'),
    dependency('libnotify', version: '>=0.7.6')
]

executable(
    meson.project_name(),
    sources,
    dependencies: deps,
    install: true
)

meson.add_install_script('meson_post_install.py')
