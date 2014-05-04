
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = ../build/HaxeBinding.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../build

HAXEBINDING_DLL_MDB_SOURCE=bin/Debug/HaxeBinding.dll.mdb
HAXEBINDING_DLL_MDB=$(BUILD_DIR)/HaxeBinding.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = ../build/HaxeBinding.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../build

HAXEBINDING_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(HAXEBINDING_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(HAXEBINDING_PC)  


RESGEN=resgen2

INSTALL_DIR = $(DESTDIR)$(prefix)/lib/monodevelop/AddIns/HaxeBinding

HAXEBINDING_PC = $(BUILD_DIR)/monodevelop-haxebinding.pc

FILES =  \
	AssemblyInfo.cs \
	gtk-gui/generated.cs \
	gtk-gui/MonoDevelop.HaxeBinding.Languages.Gui.HaxeOptionsWidget.cs \
	gtk-gui/MonoDevelop.HaxeBinding.Projects.Gui.HaxeOutputOptionsWidget.cs \
	gtk-gui/MonoDevelop.HaxeBinding.Projects.Gui.HaxeProjectOptionsWidget.cs \
	gtk-gui/MonoDevelop.HaxeBinding.Projects.Gui.NMEOutputOptionsWidget.cs \
	gtk-gui/MonoDevelop.HaxeBinding.Projects.Gui.NMEProjectOptionsWidget.cs \
	Helpers/PathHelper.cs \
	Languages/HaxeLanguageBinding.cs \
	Languages/HXMLLanguageBinding.cs \
	Languages/NMMLLanguageBinding.cs \
	Languages/Gui/HaxeOptionsPanel.cs \
	Languages/Gui/HaxeParameterDataProvider.cs \
	Languages/Gui/HaxeTextEditorCompletion.cs \
	Projects/HaxeProject.cs \
	Projects/HaxeProjectBinding.cs \
	Projects/HaxeProjectConfiguration.cs \
	Projects/NMEProject.cs \
	Projects/NMEProjectBinding.cs \
	Projects/NMEProjectConfiguration.cs \
	Projects/OpenFLProject.cs \
	Projects/OpenFLProjectBinding.cs \
	Projects/OpenFLProjectConfiguration.cs \
	Projects/Gui/HaxeOutputOptionsPanel.cs \
	Projects/Gui/HaxeProjectOptionsPanel.cs \
	Projects/Gui/NMEOutputOptionsPanel.cs \
	Projects/Gui/NMEProjectOptionsPanel.cs \
	Projects/Gui/HaxeOutputOptionsPanel.cs \
	Tools/HaxeCompilerManager.cs \
	Tools/NMECommandLineToolsManager.cs \
	Tools/OpenFLCommandLineToolsManager.cs

DATA_FILES = 

RESOURCES =  \
	gtk-gui/gui.stetic \
	HaxeBinding.addin.xml \
	Resources/Icons/flashdevelop-icon-16.png \
	Resources/Icons/flashdevelop-icon-22.png \
	Resources/Icons/flashdevelop-icon-24.png \
	Resources/Icons/flashdevelop-icon-32.png \
	Resources/Icons/flashdevelop-overlay-16.png \
	Resources/Icons/flashdevelop-overlay-32.png \
	Resources/SyntaxModes/FlashDevelopStyle.xml \
	Resources/SyntaxModes/HaxeSyntaxMode.xml \
	Resources/SyntaxModes/NMMLSyntaxMode.xml \
	Resources/Templates/EmptyHaxeCPPProject.xpt.xml \
	Resources/Templates/EmptyHaxeFlashProject.xpt.xml \
	Resources/Templates/EmptyHaxeJSProject.xpt.xml \
	Resources/Templates/EmptyHaxeNekoProject.xpt.xml \
	Resources/Templates/EmptyHaxePHPProject.xpt.xml \
	Resources/Templates/EmptyHaxeSourceFile.xft.xml \
	Resources/Templates/EmptyNMEProject.xpt.xml \
	Resources/Templates/EmptyNMMLFile.xft.xml \
	Resources/Templates/EmptyOpenFLProject.xpt.xml \
	Resources/Templates/EmptyOpenFLProjectXMLFile.xft.xml

EXTRAS = \
	monodevelop-haxebinding.pc.in 

REFERENCES =  \
	Mono.Posix \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	-pkg:mono-addins \
	-pkg:monodevelop \
	-pkg:monodevelop-core-addins \
	System \
	System.Core \
	System.Data \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-wrapper,HAXEBINDING_PC,monodevelop-haxebinding.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


LOCAL_PKGCONFIG=PKG_CONFIG_PATH=../../../local-config:$$PKG_CONFIG_PATH


$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(LOCAL_PKGCONFIG) $(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	mkdir -p $(INSTALL_DIR)
	$(call cp,$(ASSEMBLY),$(INSTALL_DIR))
	$(call cp,$(ASSEMBLY_MDB),$(INSTALL_DIR))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(HAXEBINDING_PC),$(DESTDIR)$(libdir)/pkgconfig)
	$(call cp,$(HAXEBINDING_DLL_MDB),$(INSTALL_DIR))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(INSTALL_DIR))
	$(call rm,$(ASSEMBLY_MDB),$(INSTALL_DIR))
	$(call rm,$(HAXEBINDING_PC),$(DESTDIR)$(libdir)/pkgconfig)
	$(call rm,$(HAXEBINDING_DLL_MDB),$(INSTALL_DIR))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
