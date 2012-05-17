Introduction
============


The "Haxe Language Binding" is an initial step towards bringing the functionality we enjoy in FlashDevelop to a Mac and Linux environment.

Since both MonoDevelop and FlashDevelop share a common language (C#), extending MonoDevelop provides an opportunity to port functionality, while building on a mature, stable editor.

For more background on the project, please feel free to read:
	
http://www.joshuagranick.com/blog/2012/04/06/flashdevelop-for-maclinux-part-1/



Installation
------------


You can install this add-in using the "Add-in Manager" from within MonoDevelop.

Previous versions are compatible with MonoDevelop 2.8, which is the latest version in the Ubuntu repositories.

The current version is designed to support MonoDevelop 3.0.

You can install MonoDevelop 3.0 in Ubuntu using the "keks9n" PPA:
	

	sudo add-apt-repository ppa:keks9n/monodevelop-latest
	sudo apt-get install monodevelop
	

You can find instructions for other Linux distributions or download MonoDevelop 2.8 for Mac or Windows, here:

	
http://www.monodevelop.com/download
	

If you would like to help develop the add-in, you should clone this repository then add a symlink between the "MonoDevelop.HaxeBinding.dll" file under /HaxBinding/bin/Debug and the /LocalInstall/Addins directory.

On Linux this is located at "~/.local/share/MonoDevelop-3.0/LocalInstall/Addins". Create the directory if it does not exist.

On Mac it is located at "~/Library/Application Support/MonoDevelop-3.0/LocalInstall/Addins" and on Windows it is "C:\Users\(your user name)\AppData\Local\MonoDevelop-3.0\LocalInstall\Addins" 



Supported Features
------------------


* Project creation for C++, Flash, JS, Neko, PHP and NME (3.2+)
* Haxe language highlighting
* Haxe compiler-based code completion
* Build and run support
* Initial support for C++ debugging


Other Notes
-----------


Code completion for method parameters is not available in MonoDevelop 3.0, as the API was changed. I will try to have this fixed as soon as I am able.

The add-in does support the "compilation server" feature in Haxe 2.09, which caches completion to improve response times. This is performed automatically.

A known limitation of Haxe compiler-based code completion is that it will occur only on a period or a parenthesis. This covers the majority of cases but not all. This may be improved in the future, either with workarounds or with improvements in the Haxe compiler.


Feedback
--------


Please feel free to contact me on Twitter (@singmajesty) with any feedback. Thanks!

