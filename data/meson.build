icon_themes = ['hicolor']

if get_option('for-ubuntu')
    icon_themes += ['ubuntu-mono-dark', 'ubuntu-mono-light']
endif

foreach theme : icon_themes
    install_subdir(
        'icons/' + theme,
        install_dir: join_paths(get_option('datadir'), 'icons')
    )
endforeach

install_data(
    meson.project_name() + '.appdata.xml',
    install_dir: join_paths(get_option('datadir'), 'metainfo')
)

install_data(
    meson.project_name() + '.desktop',
    install_dir: join_paths(get_option('datadir'), 'applications')
)

install_data(
    meson.project_name() + '-autostart.desktop',
    install_dir: join_paths(get_option('sysconfdir'), 'xdg', 'autostart')
)

install_data(
    'org.' + meson.project_name() + '.gschema.xml',
    install_dir: join_paths(get_option('datadir'), 'glib-2.0', 'schemas')
)
