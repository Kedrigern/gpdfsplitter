#!/bin/bash
# Script for create deb package


function prepareDEB {
	if msgfmt -V 1> /dev/null; then :
	else exit 1;
	fi;

	# make structure for dpkg-deb
	mkdir -p tmp
	cd tmp
	mkdir -p DEBIAN
	mkdir -p usr
	mkdir -p usr/share
	mkdir -p usr/share/applications
	mkdir -p usr/share/gpdfsplitter
	mkdir -p usr/share/pixmaps
	mkdir -p usr/share/locale/cs/LC_MESSAGES

	# copy files
	cp ../gpdfsplitter/bin/Release/ICSharpCode.SharpZipLib.dll usr/share/gpdfsplitter
	cp ../gpdfsplitter/bin/Release/PDFClown.dll                usr/share/gpdfsplitter
	cp ../gpdfsplitter/bin/Release/gpdfsplitter.exe            usr/share/gpdfsplitter
	if [ -f ../cs.po ]; then
		msgfmt ../cs.po -o gpdfsplitter.mo
		mv gpdfsplitter.mo usr/share/locale/cs/LC_MESSAGES/
	fi;
	if [ -e ../icon.png ]; then 
	    cp ../icon.png usr/share/pixmaps
    fi;

	echo "[Desktop Entry]
Name=gPDFsplitter
Comment=Small graphical (GTK) utility for splitting PDF.
Exec=mono /usr/share/gpdfsplitter/gpdfsplitter.exe
Terminal=false
Type=Application
Icon=gpdfsplitter.png
Encoding=UTF-8
Categories=Office" > usr/share/applications/gpdfsplitter.desktop 

	# create md5sum
	find * -type f ! -regex '^DEBIAN/.*' -exec md5sum {} \; > DEBIAN/md5sums

	#size of folder in Kb 
	size=`du -k --total usr | sed -n '$p' | cut -f 1`

	
	# create control file
		echo -n "Package: gpdfsplitter
Version: 0.01
Section: 
Priority: optional
Recommends: 
Depends: pdftk , gtk-sharp2
Architecture: all
Homepage: http://github.com/Kedrigern/gpdfsplitter
Installed-Size:" > DEBIAN/control

echo $size >> DEBIAN/control

echo "Maintainer: Ondřej Profant <ondrej.profant@gmail.com>
Description: Easy graphical tool for splitting pdf files.
 Easy graphical tool for splitting pdf files. Writen in C# (mono) and GTK# (gtk front-end)." >> DEBIAN/control
 
 
 	echo "This package was debianized by Ondřej Profant <ondrej.profant@gmail.com> on
Mon, 12 Nov 2007 16:31:40 +0900.

It was downloaded from <>

Upstream Author: 

    Ondřej Profant

License:
	GPL
	
And DLLs:

usr/share/gpdfsplitter/ICSharpCode.SharpZipLib.dll
usr/share/gpdfsplitter/PDFClown.dll

are under LGPL, homepage: http://sourceforge.net/projects/clown/


" > DEBIAN/copyright
 
	cd ..
}

prepareDEB 

sudo chown -hR root:root tmp/*
sudo dpkg-deb -b tmp gpdfsplitter_0.1.deb

#clean
sudo rm -r tmp
