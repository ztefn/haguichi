### CMakeLists automatically created with AutoVala
### Do not edit

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connected-symbolic.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita/scalable/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-1-symbolic.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita/scalable/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-2-symbolic.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita/scalable/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-connecting-3-symbolic.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita/scalable/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-disconnected-symbolic.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita/scalable/status/)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/haguichi-symbolic.svg DESTINATION ${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita/scalable/apps/)
if( NOT (${ICON_UPDATE} STREQUAL "OFF" ))
	find_program ( GTK_UPDATE_ICON_CACHE NAMES gtk-update-icon-cache.3.0 gtk-update-icon-cache )
	install (CODE "execute_process ( COMMAND ${GTK_UPDATE_ICON_CACHE} -f -t ${CMAKE_INSTALL_PREFIX}/${CMAKE_INSTALL_DATAROOTDIR}/icons/Adwaita )" )
ENDIF()
