using System;
using System.Collections.Generic;

using SCION;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Here 1");
            SCXML scxml = new SCXML("/home/jbeard/workspace/scion/scxml-test-framework/test/basic/basic1.scxml");
            System.Console.WriteLine(scxml);
            List<string> initialConfiguration = scxml.Start();
            System.Console.WriteLine(initialConfiguration);

            List<string> nextConfiguration = scxml.Gen("t",null);
            System.Console.WriteLine(nextConfiguration);
        }
    }
}
