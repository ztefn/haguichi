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

desktop_utils = find_program('desktop-file-validate', required: false)
if desktop_utils.found()
  test('Validate desktop file', desktop_utils, args: [desktop_file])
endif

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

appstream = app_name.to_lower() + '.metainfo.xml'

appstream_in = configure_file(
          input: appstream + '.in.in',
         output: appstream + '.in',
  configuration: conf
)

appstream_file = i18n.merge_file(
        input: appstream_in,
       output: appstream,
       po_dir: '../po',
      install: true,
  install_dir: get_option('datadir') / 'metainfo'
)

appstreamcli = find_program('appstreamcli', required: false, disabler: true)
test('Validate appstream file', appstreamcli,
  args: ['validate', '--no-net', '--explain', appstream_file]
)

subdir('icons')
