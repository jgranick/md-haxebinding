# Introduction

FlashDevelop is an excellent code editor for Actionscript 3 and Haxe projects, but (unfortunately) it is only available for Windows.

There are technical reasons why the code editor and windowing for FlashDevelop would need to be replaced in order to run on Mac or Linux. That is how the process of looking for replacements, considering a new GTK# front-end, and eventually moving to create "add-ins" for MonoDevelop began.

The "Haxe Language Binding" is a step on the road to bringing the functionality we love from FlashDevelop and making it available on Unix platforms. MonoDevelop is very much like FlashDevelop in a lot of ways, especially when it is successfully extended for languages and features we love from FlashDevelop.


# Installation

First you will need MonoDevelop:

http://www.monodevelop.com/Download

The add-in has been tested for MonoDevelop 2.8, but may work in newer versions. If you would like to use an older version, let me know and we can see if the add-in can be made compatible.

The add-in has been submitted to the MonoDevelop add-in repository. When it has been approved, you will be able to find "Haxe Language Binding" in the add-in manager.

Until then, you can copy the "MonoDevelop.HaxeBinding.dll" file from this repository (under /HaxeBinding/bin/Debug) to the local install directory on your machine. In Linux, this should be "~/.local/share/MonoDevelop-2.8/LocalInstall/Addins" (create the directory) and on Mac it should be "~/Library/Application Support/MonoDevelop-2.8/LocalInstall/Addins"

If you would like to help improve the addin, you may want to use a symlink to connect the DLL under /bin/Debug with the local install directory, so that the latest version is used whenever MonoDevelop is started. 


# Supported Features

* Project creation for C++, Flash, JS, Neko, PHP and NME
* Haxe language highlighting
* Code completion
* Build and run support for most projects


# Feedback

Please feel free to contact me on Twitter (@singmajesty) with any feedback.

