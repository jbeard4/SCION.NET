scionjava = lib/SCION-Java
scionjar = $(scionjava)/build/jar/scion.jar
sciondll = build/scion-java/scion.dll
scionbinding = build/scion-binding/SCXML.dll
sciondotnet = build/scion.net/SCION.dll
testscript = build/test/Test.exe
testserver = build/test/TestServer.exe

#environment can override with paths to compilers and other tools
ILREPACK = ~/Downloads/ILRepack.exe	
MCS = gmcs
IKVMC = ikvmc

all : run-test

$(scionjar) :
	mkdir -p $(dir $(scionjar))
	cd $(scionjava); make jar

$(sciondll)  : $(scionjar)
	mkdir -p $(dir $(sciondll))
	$(IKVMC) -out:$(sciondll) $(scionjar) $(scionjava)/lib/js.jar

$(scionbinding) : SCION/SCXML.cs
	mkdir -p $(dir $(scionbinding))
	$(MCS) -out:$(scionbinding) -t:library -lib:/usr/lib/cli/ikvm-0.40/,$(dir $(sciondll)) -r:$(sciondll),IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll SCION/SCXML.cs

$(sciondotnet) : $(sciondll) $(scionbinding)
	mkdir -p $(dir $(sciondotnet))
	$(ILREPACK) -out:$(sciondotnet) $(sciondll) $(scionbinding)

$(testscript) : $(sciondotnet) test/Test.cs
	mkdir -p $(dir $(testscript))
	$(MCS) -out:$(testscript) -lib:/usr/lib/cli/ikvm-0.40/,$(dir $(sciondotnet)) -r:$(sciondotnet),IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll test/Test.cs

run-test : $(testscript)
	MONO_PATH=$(dir $(sciondotnet)) $(testscript)

$(testserver) :
	$(MCS) -out:$(testserver) -lib:/usr/lib/cli/ikvm-0.40/,test/lib/Json.NET/Net35/,$(dir $(sciondotnet))  -r:$(sciondotnet),IKVM.Runtime.dll,IKVM.OpenJDK.Core.dll,Newtonsoft.Json.dll test/TestServer.cs

run-test-server : $(testserver)
	MONO_PATH=test/lib/Json.NET/Net35/:$(dir $(sciondotnet)) $(testserver)

clean : 
	cd $(scionjava); make clean
	rm -rf build

.PHONY : run-test-server run-test all clean
