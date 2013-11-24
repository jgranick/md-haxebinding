Warning
=======

Current version is for nightly builds (3.1.0 haxe candidate)

That's for of original md_haxebinding

Introduction
============


The "Haxe Language Binding" is an initial step towards bringing the functionality we enjoy in FlashDevelop to a Mac and Linux environment.

Since both MonoDevelop and FlashDevelop share a common language (C#), extending MonoDevelop provides an opportunity to port functionality, while building on a mature, stable editor.



Installation
------------


You can install this add-in using the "Add-in Manager" from within MonoDevelop. That's preffered way to do it.

The current version is designed to support MonoDevelop 4.0.

http://www.monodevelop.com/download

If you would like to help develop the add-in, you should clone this repository then add a symlink between the "MonoDevelop.HaxeBinding.dll" file under /HaxBinding/bin/Debug and the /LocalInstall/Addins directory.

Or you can run addin directly from MonoDevelop.

To open solution Addin Maker should be installed. This addon is available from MonoDevelop addins repository, but only in windows repo. So, if you are developing in non-Windows os, you should add http://addins.monodevelop.com/Stable/Windows/4.0.12/root.mrep to your repos.

On Linux this is located at "~/.local/share/MonoDevelop-4.0/LocalInstall/Addins". Create the directory if it does not exist.

On Mac it is located at "~/Library/Application Support/MonoDevelop-4.0/LocalInstall/Addins" and on Windows it is "C:\Users\(your user name)\AppData\Local\XamarinStudio-4.0\LocalInstall\Addins" 



Supported Features
------------------


* Project creation for C++, Flash, JS, Neko, PHP and NME (3.2+) and openfl
* Haxe language highlighting
* Haxe compiler-based code completion
* Build and run support


Feedback
--------


Please feel free to contact me on Twitter (@zaynyatyi) or by e-mail (zaynyatyi@gmail.com) with any feedback. Thanks!


