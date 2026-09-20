
  Flatpak
  =======


  Initialize or update shared modules
  -----------------------------------

    $ git submodule update --init


  Install runtime and SDK
  -----------------------

    $ flatpak install org.freedesktop.Platform org.freedesktop.Sdk org.freedesktop.Sdk.Extension.mono6

  > [!IMPORTANT]
  > Choose the version specified in the manifest.


  Build and install locally
  -------------------------

    $ flatpak-builder build com.github.ztefn.haguichi.json --user --install --force-clean --repo=repo

  > [!WARNING]
  > It might take a while to compile all modules.


  Create a bundle
  ---------------

    $ flatpak build-bundle repo haguichi.flatpak com.github.ztefn.haguichi --runtime-repo=https://flathub.org/repo/flathub.flatpakrepo

