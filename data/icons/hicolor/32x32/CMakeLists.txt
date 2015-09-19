### CMakeLists automatically created with AutoVala
### Do not edit

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-node-error.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-node-offline.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-node-online-relayed.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-node-online.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-node-unapproved.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-node-unreachable.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/32x32/apps/)
if( NOT (${ICON_UPDATE} STREQUAL "OFF" ))
	find_program ( GTK_UPDATE_ICON_CACHE NAMES gtk-update-icon-cache.3.0 gtk-update-icon-cache )
	install (CODE "execute_process ( COMMAND ${GTK_UPDATE_ICON_CACHE} -f -t ${CMAKE_INSTALL_PREFIX}/${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor )" )
ENDIF()
