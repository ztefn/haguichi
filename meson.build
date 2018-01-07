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
gettext_pkg = app_name

add_project_arguments(
    '-DGETTEXT_PACKAGE="' + gettext_pkg + '"',
    language: 'c'
)

conf = configuration_data()
conf.set('PACKAGE_NAME', app_name)
conf.set('VERSION', app_version)
conf.set('BINARY_NAME', binary_name)
conf.set('ICON_NAME', icon_name)
conf.set('GETTEXT_PACKAGE', gettext_pkg)
conf.set('DATADIR', join_paths(get_option('prefix'), get_option('datadir')))
conf.set('LOCALEDIR', join_paths(get_option('prefix'), get_option('localedir')))

subdir('data')
subdir('po')
subdir('src')

meson.add_install_script('meson_post_install.py')
