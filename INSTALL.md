
  Building & Installing Haguichi
  ==============================


  Dependencies
  ------------

  The following development packages are required to build Haguichi:

   * gettext
   * cmake (>= 2.6)
   * valac (>= 0.26)
   * glib-2.0 (>= 2.42)
   * gtk+-3.0 (>= 3.14)
   * libnotify (>= 0.7.6)

  On Debian based distributions you can install these packages by running the following command:

    $ sudo apt install build-essential gettext cmake valac libglib2.0-dev libgtk-3-dev libnotify-dev


  Building
  --------

  To build Haguichi, run the following commands:

    $ mkdir build
    $ cd build
    $ cmake .. -DCMAKE_INSTALL_PREFIX=/usr
    $ make


  Installing
  ----------

  After Haguichi has been built, run the following command to install it:

    $ sudo make install

