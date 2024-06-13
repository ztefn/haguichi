project('Haguichi', ['c', 'vala'],
          version: '1.5.0',
    meson_version: '>= 1.3.0',
  default_options: ['prefix=/usr', 'warning_level=2', 'werror=false'],
)

app_name    = meson.project_name()
app_id      = 'com.github.ztefn.' + app_name.to_lower()
app_version = meson.project_version()
binary_name = app_name.to_lower()
gettext_pkg = app_name.to_lower()

add_project_arguments(
  '-DG_LOG_DOMAIN="' + app_name + '"',
  '-DGETTEXT_PACKAGE="' + gettext_pkg + '"',
  language: 'c'
)

conf = configuration_data()
conf.set('APP_ID',          app_id)
conf.set('APP_NAME',        app_name)
conf.set('VERSION',         app_version)
conf.set('BINARY_NAME',     binary_name)
conf.set('GETTEXT_PACKAGE', gettext_pkg)
conf.set('LOCALEDIR',       get_option('prefix') / get_option('localedir'))

i18n  = import('i18n')
gnome = import('gnome')

subdir('data')
subdir('po')
subdir('src')

gnome.post_install(
     glib_compile_schemas: true,
    gtk_update_icon_cache: true,
  update_desktop_database: true,
)
