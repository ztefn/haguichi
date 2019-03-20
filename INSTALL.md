
  Building & Installing Haguichi
  ==============================


  Dependencies
  ------------

  The following development packages are required to build Haguichi:

   * gettext
   * meson (>= 0.40)
   * valac (>= 0.30)
   * glib-2.0 (>= 2.48)
   * gtk+-3.0 (>= 3.18)
   * libnotify (>= 0.7.6)

  On Debian based distributions you can install these packages by running the following command:

    $ sudo apt install build-essential gettext meson valac libglib2.0-dev libgtk-3-dev libnotify-dev

  On Solus you can install these packages by running the following command:

    $ sudo eopkg it -c system.devel vala glib2-devel libgtk-3-devel libnotify-devel


  Build options
  -------------

  **enable-appindicator**

  This option enables appindicator integration and additionally requires the *appindicator3* development package.

    $ meson configure -Denable-appindicator=true


  **enable-wingpanel-indicator**

  This option enables wingpanel integration and additionally requires the *wingpanel-2.0* development package.

    $ meson configure -Denable-wingpanel-indicator=true


  **for-elementary**

  This option enables full integration with elementary OS and additionally requires the *granite* development package.

    $ meson configure -Dfor-elementary=true


  **for-ubuntu**

  This option enables full integration with Ubuntu by installing ubuntu-mono and Suru icons.

    $ meson configure -Dfor-ubuntu=true


  **use-rdnn-everywhere**

  This option enables usage of [RDNN](https://en.wikipedia.org/wiki/Reverse_domain_name_notation "Reverse Domain Name Notation") everywhere, specifically for binary, icon and gettext package.

    $ meson configure -Duse-rdnn-everywhere=true


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

