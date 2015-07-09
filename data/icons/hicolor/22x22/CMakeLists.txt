### CMakeLists automatically created with AutoVala
### Do not edit

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connected.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/22x22/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-1.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/22x22/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-2.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/22x22/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-3.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/22x22/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-disconnected.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/22x22/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi.png DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor/22x22/apps/)
if( NOT (${ICON_UPDATE} STREQUAL "OFF" ))
	find_program ( GTK_UPDATE_ICON_CACHE NAMES gtk-update-icon-cache.3.0 gtk-update-icon-cache )
	install (CODE "execute_process ( COMMAND ${GTK_UPDATE_ICON_CACHE} -f -t ${CMAKE_INSTALL_PREFIX}/${CMAKE_INSTALL_DATAROOTDIR}/icons/hicolor )" )
ENDIF()
