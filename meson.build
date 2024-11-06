project('Haguichi', 'cs',
    meson_version: '>= 0.59.0',
  default_options: ['prefix=/usr', 'sysconfdir=/etc'],
)

app_name    = meson.project_name()
binary_name = app_name.to_lower()
icon_name   = app_name.to_lower()
gettext_pkg = app_name.to_lower()

conf = configuration_data()
conf.set('APP_NAME',        app_name)
conf.set('BINARY_NAME',     binary_name)
conf.set('ICON_NAME',       icon_name)
conf.set('GETTEXT_PACKAGE', gettext_pkg)
conf.set('BINDIR',          get_option('prefix') / get_option('bindir'))
conf.set('LOCALEDIR',       get_option('prefix') / get_option('localedir'))

configure_file(
          input: binary_name + '.in',
         output: binary_name,
  configuration: conf,
    install_dir: get_option('bindir'),
)

i18n  = import('i18n')
gnome = import('gnome')

subdir('data')
subdir('po')
subdir('src')

gnome.post_install(
    gtk_update_icon_cache: true,
  update_desktop_database: true,
)
