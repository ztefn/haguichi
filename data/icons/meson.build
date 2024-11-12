themes_dir = get_option('prefix') / 'share' / 'icons'

foreach size : ['16', '22', '24', '32', '48', '64', '256']
  icon_dir = size + 'x' + size

  install_data(
        sources: 'hicolor' / icon_dir / icon_name + '.png',
    install_dir: themes_dir / 'hicolor' / icon_dir / 'apps',
  )
endforeach

foreach size : ['16', '22', '24', '48']
  foreach state : ['connected', 'connecting-1', 'connecting-2', 'connecting-3', 'disconnected']
    icon_dir = size + 'x' + size

    install_data(
          sources: 'hicolor' / size + 'x' + size / icon_name + '-' + state + '.png',
      install_dir: themes_dir / 'hicolor' / icon_dir / 'status',
    )
  endforeach
endforeach

if get_option('for-ubuntu')
  foreach theme : ['ubuntu-mono-dark', 'ubuntu-mono-light']
    foreach size : ['16', '22', '24']
      foreach state : ['connected', 'connecting-1', 'connecting-2', 'connecting-3', 'disconnected']
        install_data(
              sources: theme / size + 'x' + size / icon_name + '-' + state + '.svg',
          install_dir: themes_dir / theme / 'status' / size,
        )
      endforeach
    endforeach
  endforeach
endif
