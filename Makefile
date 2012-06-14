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

.PHONY : run-test all
