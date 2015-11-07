### CMakeLists automatically created with AutoVala
### Do not edit

IF(IS_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/haguichi.appdata.xml)
	install(DIRECTORY
		${CMAKE_CURRENT_SOURCE_DIR}/haguichi.appdata.xml
	DESTINATION
		share/appdata/
	)
ELSE()
	install(FILES
		${CMAKE_CURRENT_SOURCE_DIR}/haguichi.appdata.xml
	DESTINATION
		share/appdata/
	)
ENDIF()

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi.desktop DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/applications/ )
if( NOT ( ${CMAKE_INSTALL_PREFIX} MATCHES "^/home/" ) )
	install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-autostart.desktop DESTINATION /etc/xdg/autostart/ )
else()
	set ( CONFIG_HOME $ENV{XDG_CONFIG_HOME})
	if ( NOT (CONFIG_HOME))
		set ( CONFIG_HOME $ENV{HOME}/.config )
	endif()
	install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-autostart.desktop DESTINATION ${CONFIG_HOME}/autostart )
endif()
include(GSettings)
add_schema(org.haguichi.gschema.xml)
