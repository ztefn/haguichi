if get_option('for-elementary')
  icon_themes = ['elementary/hicolor']
else
  icon_themes = ['hicolor']
endif

if get_option('for-ubuntu')
  icon_themes += ['Yaru']
endif

foreach theme : icon_themes
  install_subdir(theme,
    follow_symlinks: true,
        install_dir: get_option('datadir') / 'icons'
  )
endforeach
