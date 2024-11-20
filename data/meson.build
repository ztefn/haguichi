desktop = app_name.to_lower() + '-menu.desktop'

desktop_in = configure_file(
          input: desktop + '.in.in',
         output: desktop + '.in',
  configuration: conf
)

desktop_file = i18n.merge_file(
        input: desktop_in,
       output: app_name.to_lower() + '.desktop',
         type: 'desktop',
       po_dir: '../po',
      install: true,
  install_dir: get_option('datadir') / 'applications'
)

if not get_option('flatpak')
  autostart = app_name.to_lower() + '-autostart.desktop'

  autostart_in = configure_file(
            input: autostart + '.in.in',
           output: autostart + '.in',
    configuration: conf
  )

  i18n.merge_file(
          input: autostart_in,
         output: autostart,
           type: 'desktop',
         po_dir: '../po',
        install: true,
    install_dir: get_option('sysconfdir') / 'xdg' / 'autostart'
  )
endif

subdir('icons')
