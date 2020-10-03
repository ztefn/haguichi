project(
    'haguichi',
    ['vala', 'c'],
    version: '1.143.0',
    default_options: ['prefix=/usr'],
    meson_version: '>= 0.40'
)

app_name    = meson.project_name()
app_id      = 'com.github.ztefn.' + app_name
app_version = meson.project_version()

if get_option('use-rdnn-everywhere') or get_option('for-elementary')
    pkg_name = app_id
else
    pkg_name = app_name
endif

binary_name = pkg_name
icon_name   = app_id
gettext_pkg = pkg_name

add_project_arguments(
    '-DGETTEXT_PACKAGE="' + gettext_pkg + '"',
    language: 'c'
)

conf = configuration_data()
conf.set('PACKAGE_NAME', pkg_name)
conf.set('VERSION', app_version)
conf.set('BINARY_NAME', binary_name)
conf.set('ICON_NAME', icon_name)
conf.set('GETTEXT_PACKAGE', gettext_pkg)
conf.set('DATADIR', join_paths(get_option('prefix'), get_option('datadir')))
conf.set('LOCALEDIR', join_paths(get_option('prefix'), get_option('localedir')))

i18n = import('i18n')

po_dir = join_paths(meson.source_root(), 'po')

subdir('data')
subdir('po')
subdir('src')

meson.add_install_script('meson_post_install.py')
