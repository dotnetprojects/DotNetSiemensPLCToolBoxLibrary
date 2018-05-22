set PATH_TO_MAKE="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\nmake"
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat"
cd externalDlls\libnodave\
del *.obj
%PATH_TO_MAKE% /f MAKEFILE.VC %1
cd ..
cd ..
