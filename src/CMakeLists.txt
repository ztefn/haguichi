### CMakeLists automatically created with AutoVala
### Do not edit

set (DATADIR "${CMAKE_INSTALL_PREFIX}/${CMAKE_INSTALL_DATAROOTDIR}")
set (PKGDATADIR "${DATADIR}/haguichi")
set (GETTEXT_PACKAGE "haguichi")
set (RELEASE_NAME "haguichi")
set (CMAKE_C_FLAGS "")
set (PREFIX ${CMAKE_INSTALL_PREFIX})
set (VERSION "1.3.0")
set (TESTSRCDIR "${CMAKE_SOURCE_DIR}")
set (DOLLAR "$")

configure_file (${CMAKE_SOURCE_DIR}/src/Config.vala.cmake ${CMAKE_BINARY_DIR}/src/Config.vala)
add_definitions(-DGETTEXT_PACKAGE=\"${GETTEXT_PACKAGE}\")
set (VERSION "1.3.0")
add_definitions (${DEPS_CFLAGS})
link_libraries ( ${DEPS_LIBRARIES} )
link_directories ( ${DEPS_LIBRARY_DIRS} )
find_package (Vala REQUIRED)
include (ValaVersion)
ensure_vala_version ("0.26" MINIMUM)
include (ValaPrecompile)

set (VALA_PACKAGES ${VALA_PACKAGES} atk)
set (VALA_PACKAGES ${VALA_PACKAGES} cairo)
set (VALA_PACKAGES ${VALA_PACKAGES} gdk-3.0)
set (VALA_PACKAGES ${VALA_PACKAGES} gdk-pixbuf-2.0)
set (VALA_PACKAGES ${VALA_PACKAGES} gio-2.0)
set (VALA_PACKAGES ${VALA_PACKAGES} glib-2.0)
set (VALA_PACKAGES ${VALA_PACKAGES} gobject-2.0)
set (VALA_PACKAGES ${VALA_PACKAGES} gtk+-3.0)
set (VALA_PACKAGES ${VALA_PACKAGES} libnotify)
set (VALA_PACKAGES ${VALA_PACKAGES} pango)
set (VALA_PACKAGES ${VALA_PACKAGES} x11)

set (APP_SOURCES ${APP_SOURCES} ${CMAKE_CURRENT_BINARY_DIR}/Config.vala)
set (APP_SOURCES ${APP_SOURCES} bubble.vala)
set (APP_SOURCES ${APP_SOURCES} command.vala)
set (APP_SOURCES ${APP_SOURCES} connection.vala)
set (APP_SOURCES ${APP_SOURCES} controller.vala)
set (APP_SOURCES ${APP_SOURCES} debug.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/add-edit-command.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/attach.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/base.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/change-nick.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/change-password.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/confirm.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/join-create-network.vala)
set (APP_SOURCES ${APP_SOURCES} dialogs/message.vala)
set (APP_SOURCES ${APP_SOURCES} global-actions.vala)
set (APP_SOURCES ${APP_SOURCES} global-events.vala)
set (APP_SOURCES ${APP_SOURCES} haguichi.vala)
set (APP_SOURCES ${APP_SOURCES} hamachi.vala)
set (APP_SOURCES ${APP_SOURCES} headerbar.vala)
set (APP_SOURCES ${APP_SOURCES} key.vala)
set (APP_SOURCES ${APP_SOURCES} member-event.vala)
set (APP_SOURCES ${APP_SOURCES} member.vala)
set (APP_SOURCES ${APP_SOURCES} menus/command-menu-item.vala)
set (APP_SOURCES ${APP_SOURCES} menus/member-menu.vala)
set (APP_SOURCES ${APP_SOURCES} menus/network-menu.vala)
set (APP_SOURCES ${APP_SOURCES} network-view.vala)
set (APP_SOURCES ${APP_SOURCES} network.vala)
set (APP_SOURCES ${APP_SOURCES} preferences.vala)
set (APP_SOURCES ${APP_SOURCES} session.vala)
set (APP_SOURCES ${APP_SOURCES} settings.vala)
set (APP_SOURCES ${APP_SOURCES} sidebar.vala)
set (APP_SOURCES ${APP_SOURCES} status.vala)
set (APP_SOURCES ${APP_SOURCES} text.vala)
set (APP_SOURCES ${APP_SOURCES} utils.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/command-button.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/commands-editor.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/group-box.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/message-bar.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/message-box.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/sidebar-entry.vala)
set (APP_SOURCES ${APP_SOURCES} widgets/sidebar-label.vala)
set (APP_SOURCES ${APP_SOURCES} window.vala)

set (COMPILE_OPTIONS ${COMPILE_OPTIONS} --target-glib 2.32 )
set (COMPILE_OPTIONS ${COMPILE_OPTIONS} --thread )

vala_precompile(VALA_C haguichi
	${APP_SOURCES}
PACKAGES
	${VALA_PACKAGES}
OPTIONS
	${COMPILE_OPTIONS}
)

add_executable(haguichi ${VALA_C})
target_link_libraries( haguichi notify )

install(TARGETS
	haguichi
RUNTIME DESTINATION
	${CMAKE_INSTALL_BINDIR}/
)

if(HAVE_VALADOC)
	valadoc(haguichi
		${CMAKE_BINARY_DIR}/valadoc/haguichi
		${APP_SOURCES}
	PACKAGES
		${VALA_PACKAGES}
	CUSTOM_VAPIS
		${CUSTOM_VAPIS_LIST}
	)
	install(DIRECTORY
		${CMAKE_BINARY_DIR}/valadoc
	DESTINATION
		${CMAKE_INSTALL_DATAROOTDIR}/doc/haguichi
	)
endif()
