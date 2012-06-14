scionjava = lib/SCION-Java
scionjar = $(scionjava)/build/jar/scion.jar

all : run-test

$(scionjar) :
	cd $(scionjava); make jar

scion.dll : $(scionjar)
	ikvmc -out:scion.dll $(scionjar) $(scionjava)/lib/js.jar

clean : 
	cd $(scionjava); make clean
	rm scion.dll

test/Test.exe :
	gmcs -lib:/usr/lib/cli/ikvm-0.40/,. -r:scion.dll,IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll test/Test.cs

run-test : test/Test.exe
	MONO_PATH=. test/Test.exe

test/TestServer.exe :
	gmcs -lib:/usr/lib/cli/ikvm-0.40/,.,test/lib/Json.NET/Net35/  -r:scion.dll,IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll,Newtonsoft.Json.dll test/TestServer.cs

run-test-server : test/TestServer.exe
	MONO_PATH=.:test/lib/Json.NET/Net35/ ./test/TestServer.exe

.PHONY : run-test-server run-test all clean
