# Introduction

This is the first step in efforts to bring the functionality we've enjoyed from FlashDevelop to a Mac/Linux environment.

For more details, please feel free to read this post:

http://www.joshuagranick.com/blog/2012/04/06/flashdevelop-for-maclinux-part-1/


# Installation

You can install this add-in using the "Add-in Manager" from within MonoDevelop. If you are using Ubuntu 12.04, you can install MonoDevelop 2.8 using "sudo apt-get install monodevelop".

You can find instructions for other Linux distributions or download MonoDevelop 2.8 for Mac or Windows, here:

http://www.monodevelop.com/download

If you would like to help develop the add-in, you should clone this repository then add a symlink between the "MonoDevelop.HaxeBinding.dll" file under /HaxBinding/bin/Debug and the /LocalInstall/Addins directory.

On Linux this is located at ~/.local/share/MonoDevelop-2.8/LocalInstall/Addins. Create the directory if it does not exist. On Mac it is located at ~/Library/Application Support/MonoDevelop-2.8/LocalInstall/Addins and on Windows it is C:\Users\(your user name)\AppData\Local\MonoDevelop-2.8\LocalInstall\Addins 


# Supported Features

* Project creation for C++, Flash, JS, Neko, PHP and NME (3.2+)
* Haxe language highlighting
* Haxe compiler-based code completion
* Build and run support
* Initial support for C++ debugging


# Other Notes

The add-in does support the "compilation server" feature in Haxe 2.09, which caches completion to improve response times. This is performed automatically.

A known limitation of Haxe compiler-based code completion is that it will occur only on a period or a parenthesis. This covers the majority of cases but not all. This may be improved in the future.

If you're interested in bringing features that exist in FlashDevelop but not this language binding, much of FlashDevelop's code is transferrable since both IDEs are written using C#, but since we are using a different text editor and GUI framework, adjustments will have to be made.


# Feedback

Please feel free to contact me on Twitter (@singmajesty) with any feedback.

