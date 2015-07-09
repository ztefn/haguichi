### CMakeLists automatically created with AutoVala
### Do not edit

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connected.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/ubuntu-mono-dark/status/24/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-1.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/ubuntu-mono-dark/status/24/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-2.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/ubuntu-mono-dark/status/24/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-3.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/ubuntu-mono-dark/status/24/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-disconnected.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/ubuntu-mono-dark/status/24/)
if( NOT (${ICON_UPDATE} STREQUAL "OFF" ))
	find_program ( GTK_UPDATE_ICON_CACHE NAMES gtk-update-icon-cache.3.0 gtk-update-icon-cache )
	install (CODE "execute_process ( COMMAND ${GTK_UPDATE_ICON_CACHE} -f -t ${CMAKE_INSTALL_PREFIX}/${CMAKE_INSTALL_DATAROOTDIR}/icons/ubuntu-mono-dark )" )
ENDIF()
