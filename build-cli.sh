xbuild SimpleBench.sln
mkbundle -c -o SimpleBenchRunner/bin/Debug/temp.c -oo SimpleBenchRunner/bin/Debug/temp.o --deps SimpleBenchRunner/bin/Debug/SimpleBenchRunner.exe SimpleBenchRunner/bin/Debug/Mono.Options.dll SimpleBenchRunner/bin/Debug/SimpleBench.dll
cc -g -o SimpleBenchRunner/bin/Debug/simplebench-runner -Wall SimpleBenchRunner/bin/Debug/temp.c -framework CoreFoundation -lobjc -liconv `pkg-config --cflags --libs mono-2` SimpleBenchRunner/bin/Debug/temp.o