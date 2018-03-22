
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

