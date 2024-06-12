
  Building & Installing Haguichi
  ==============================


  Dependencies
  ------------

  The following development packages are required to build Haguichi:

   * gettext
   * meson (>= 0.62)
   * valac (>= 0.56)
   * gee-0.8 (>= 0.20.6)
   * glib-2.0 (>= 2.78)
   * gtk4 (>= 4.12)
   * libadwaita-1 (>= 1.4)
   * libportal (>= 0.7.1)
   * libportal-gtk4 (>= 0.7.1)

  On **Debian based distributions** you can install these packages by running the following command:

    $ sudo apt install build-essential gettext meson valac libgee-0.8-dev libglib2.0-dev libgtk-4-dev libadwaita-1-dev libportal-dev libportal-gtk4-dev


  Build options
  -------------

  **for-elementary**

  This option enables elementary OS integration by installing elementary style application icons.

    $ meson configure -Dfor-elementary=true


  **for-ubuntu**

  This option enables Ubuntu integration by installing Yaru style application icons.

    $ meson configure -Dfor-ubuntu=true


  Building
  --------

  To build Haguichi, run the following commands:

    $ mkdir build && cd build
    $ meson ..
    $ ninja


  Installing
  ----------

  After Haguichi has been built, run the following command to install it:

    $ sudo ninja install

