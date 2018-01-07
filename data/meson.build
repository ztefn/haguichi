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

configure_file(
    input: app_id + '.appdata.xml.in',
    output: app_id + '.appdata.xml',
    install_dir: join_paths(get_option('datadir'), 'metainfo'),
    configuration: conf
)

configure_file(
    input: app_id + '.desktop.in',
    output: app_id + '.desktop',
    install_dir: join_paths(get_option('datadir'), 'applications'),
    configuration: conf
)

configure_file(
    input: app_id + '-autostart.desktop.in',
    output: app_id + '-autostart.desktop',
    install_dir: join_paths(get_option('sysconfdir'), 'xdg', 'autostart'),
    configuration: conf
)

install_data(
    app_id + '.gschema.xml',
    install_dir: join_paths(get_option('datadir'), 'glib-2.0', 'schemas')
)
