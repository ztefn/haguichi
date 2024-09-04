desktop = app_id + '.desktop'

desktop_in = configure_file(
          input: desktop + '.in.in',
         output: desktop + '.in',
  configuration: conf
)

desktop_file = i18n.merge_file(
        input: desktop_in,
       output: desktop,
         type: 'desktop',
       po_dir: '../po',
      install: true,
  install_dir: get_option('datadir') / 'applications'
)

desktop_utils = find_program('desktop-file-validate', required: false)
if desktop_utils.found()
  test('Validate desktop file', desktop_utils, args: [desktop_file])
endif

appstream = app_id + '.metainfo.xml'

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

install_data(app_id + '.gschema.xml',
  install_dir: get_option('datadir') / 'glib-2.0' / 'schemas'
)

compile_schemas = find_program('glib-compile-schemas', required: false, disabler: true)
test('Validate schema file', compile_schemas,
  args: ['--strict', '--dry-run', meson.current_source_dir()]
)

resources = gnome.compile_resources(app_name.to_lower() + '-resources', app_id + '.gresource.xml',
      c_name: app_name.to_lower(),
  source_dir: ['stylesheets']
)

subdir('icons')
