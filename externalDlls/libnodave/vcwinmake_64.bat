set PATH_TO_MAKE="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\nmake"
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" amd64
cd externalDlls\libnodave\
del *.obj
%PATH_TO_MAKE% /f MAKEFILE.64 %1
cd ..
cd ..