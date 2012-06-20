using System;
using System.Collections.Generic;

using SCION;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            SCXML scxml = new SCXML("/home/jbeard/workspace/scion/scxml-test-framework/test/basic/basic1.scxml");
            System.Console.WriteLine(scxml);
            IList<string> initialConfiguration = scxml.Start();
            System.Console.WriteLine(initialConfiguration);
            foreach (string stateId in initialConfiguration){
                System.Console.WriteLine(stateId);
            } 

            IList<string> nextConfiguration = scxml.Gen("t",null);
            System.Console.WriteLine(nextConfiguration);
            foreach (string stateId in nextConfiguration){
                System.Console.WriteLine(stateId);
            } 
        }
    }
}
