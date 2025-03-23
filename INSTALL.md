
  Building & Installing Haguichi
  ==============================


  Dependencies
  ------------

  The following development packages are required to build Haguichi:

   * gettext
   * meson (>= 1.3.0)
   * valac (>= 0.56)
   * gee-0.8 (>= 0.20.6)
   * glib-2.0 (>= 2.80)
   * gtk4 (>= 4.14)
   * libadwaita-1 (>= 1.5)
   * libportal (>= 0.7.1)
   * libportal-gtk4 (>= 0.7.1)

  On **Debian based distributions** you can install these packages by running the following command:

    $ sudo apt install build-essential gettext meson valac libgee-0.8-dev libglib2.0-dev libgtk-4-dev libadwaita-1-dev libportal-dev libportal-gtk4-dev


  On **Fedora** you can install these packages by running the following command:

    $ sudo dnf install gettext meson vala libgee-devel gtk4-devel libadwaita-devel libportal-devel libportal-gtk4-devel


  Build options
  -------------

  **for-elementary**

  This option enables elementary OS integration and additionally requires the *granite-7* development package.

    $ meson configure -Dfor-elementary=true


  **for-ubuntu**

  This option enables Ubuntu integration by installing Yaru style application icons.

    $ meson configure -Dfor-ubuntu=true


  Building
  --------

  To build Haguichi, run the following commands:

    $ mkdir build && cd build
    $ meson setup ..
    $ ninja


  Installing
  ----------

  After Haguichi has been built, run the following command to install it:

    $ sudo ninja install

