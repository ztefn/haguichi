### CMakeLists automatically created with AutoVala
### Do not edit

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi.appdata.xml DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/appdata/ )
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi.desktop DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/applications/ )
if( NOT ( ${CMAKE_INSTALL_PREFIX} MATCHES "^/home/" ) )
	install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-autostart.desktop DESTINATION /etc/xdg/autostart/ )
else()
	MESSAGE(STATUS "[33mAutostart file data/haguichi-autostart.desktop will not be installed. You must create your own .desktop file and put it at ~/.config/autostart[39m")
endif()
include(GSettings)
add_schema(org.haguichi.gschema.xml)
