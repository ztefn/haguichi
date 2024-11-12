
  Building & Installing Haguichi
  ==============================


  Dependencies
  ------------

  The following development packages are required to build Haguichi:

   * gettext
   * meson (>= 0.59.0)
   * glib-sharp-2.0 (>= 2.12.9)
   * gtk-sharp-2.0 (>= 2.12.9)
   * gconf-sharp-2.0 (>= 2.24.0)
   * dbus-sharp-2.0 (>= 0.8.0)
   * dbus-sharp-glib-2.0 (>= 0.6.0)
   * notify-sharp (>= 0.4.0)

  On **Fedora** you can install these packages by running the following command:

    $ sudo dnf install gettext meson mono-core gtk-sharp2-devel gnome-sharp-devel dbus-sharp-devel dbus-sharp-glib-devel notify-sharp-devel


  Build options
  -------------

  **enable-appindicator**

  This option enables appindicator integration and additionally requires the *appindicator-sharp-0.1* development package.

    $ meson configure -Denable-appindicator=true


  **for-ubuntu**

  This option enables Ubuntu integration by installing ubuntu-mono status icons.

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

