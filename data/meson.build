if get_option('for-elementary')
    icon_themes = ['elementary/hicolor']
else
    icon_themes = ['hicolor']
endif

if get_option('for-ubuntu')
    icon_themes += ['Suru', 'ubuntu-mono-dark', 'ubuntu-mono-light']
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

desktop = app_id + '.desktop'

desktop_in = configure_file(
    input: desktop + '.in.in',
    output: desktop + '.in',
    configuration: conf
)

i18n.merge_file(
    type: 'desktop',
    input: desktop_in,
    output: desktop,
    po_dir: po_dir,
    install: true,
    install_dir: join_paths(get_option('datadir'), 'applications')
)

autostart = app_id + '.autostart.desktop'

autostart_in = configure_file(
    input: autostart + '.in.in',
    output: autostart + '.in',
    configuration: conf
)

i18n.merge_file(
    type: 'desktop',
    input: autostart_in,
    output: autostart,
    po_dir: po_dir,
    install: true,
    install_dir: join_paths(get_option('sysconfdir'), 'xdg', 'autostart'),
)

gnome = import('gnome')

resources = gnome.compile_resources(
    'haguichi-resources',
    app_id + '.gresource.xml',
    source_dir: ['icons/resources', 'stylesheets', 'ui']
)

install_data(
    app_id + '.gschema.xml',
    install_dir: join_paths(get_option('datadir'), 'glib-2.0', 'schemas')
)
