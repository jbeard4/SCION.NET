using System;
using System.Reflection;

using com.inficon.scion;
using java.util;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            SCXML scxml = new SCXML("/home/jbeard/workspace/scion/scxml-test-framework/test/basic/basic1.scxml");
			System.Console.WriteLine(scxml);
        	Object initialConfiguration = scxml.start();
			System.Console.WriteLine(initialConfiguration);

			Set nextConfiguration = scxml.gen("t",null);
        	System.Console.WriteLine(nextConfiguration);
			
			Type t = initialConfiguration.GetType();
			System.Console.WriteLine(t);
			
		}
    }
}
