scionjava = lib/SCION-Java
scionjar = $(scionjava)/build/jar/scion.jar
sciondll = build/scion-java/scion.dll
scionbinding = build/scion-binding/SCXML.dll
sciondotnet = build/scion.net/SCION.dll
testscript = build/test/Test.exe
testserver = build/test/TestServer.exe

#environment can override with paths to compilers and other tools
ILREPACK = ~/Downloads/ILRepack.exe	
MCS = dmcs 
IKVMC = ikvmc

all : run-test $(testserver)

deps/ILRepack.exe:
	mkdir -p deps/
	wget https://github.com/downloads/gluck/il-repack/ILRepack_1.15.zip 
	mv ILRepack_1.15.zip deps/
	cd deps && unzip ILRepack_1.15.zip
	chmod +x deps/ILRepack.exe

get-deps : deps/ILRepack.exe

$(scionjar) :
	mkdir -p $(dir $(scionjar))
	cd $(scionjava); make jar

$(sciondll)  : $(scionjar)
	mkdir -p $(dir $(sciondll))
	$(IKVMC) -out:$(sciondll) $(scionjar) $(scionjava)/lib/js.jar

$(scionbinding) : SCION/SCXML.cs
	mkdir -p $(dir $(scionbinding))
	$(MCS) -out:$(scionbinding) -t:library -lib:/usr/lib/ikvm/,/usr/lib/cli/ikvm-0.40/,$(dir $(sciondll)) -r:$(sciondll),IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll SCION/SCXML.cs

$(sciondotnet) : $(sciondll) $(scionbinding) get-deps
	mkdir -p $(dir $(sciondotnet))
	./deps/ILRepack.exe -out:$(sciondotnet) $(sciondll) $(scionbinding)

$(testscript) : $(sciondotnet) test/Test.cs
	mkdir -p $(dir $(testscript))
	$(MCS) -out:$(testscript) -lib:/usr/lib/ikvm/,/usr/lib/cli/ikvm-0.40/,$(dir $(sciondotnet)) -r:$(sciondotnet),IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll test/Test.cs

run-test : $(testscript)
	MONO_PATH=$(dir $(sciondotnet)) $(testscript)

$(testserver) : test/TestServer.cs $(sciondotnet)
	$(MCS) -out:$(testserver) -lib:/usr/lib/ikvm/,/usr/lib/cli/ikvm-0.40/,test/lib/Json.NET/Net35/,$(dir $(sciondotnet))  -r:$(sciondotnet),IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll,Newtonsoft.Json.dll test/TestServer.cs

run-test-server : $(testserver)
	MONO_PATH=test/lib/Json.NET/Net35/:$(dir $(sciondotnet)) $(testserver)

clean : 
	cd $(scionjava); make clean
	rm -rf build

.PHONY : get-deps run-test-server run-test all clean
