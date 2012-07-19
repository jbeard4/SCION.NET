using System;
using System.Net;
using  System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using SCION;
//using com.inficon.scion;
//using java.util;

class TestServer{
    public static string getResponseJson(int session,IList<string> configuration){
        string result = "{\"sessionToken\" : " + session + ", \"nextConfiguration\" : [";

        for(int i=0; i < configuration.Count; i++){
            string stateId = configuration[i];
            result += "\"" + stateId + "\"";
            if(i != configuration.Count - 1){
                result += ",";
            }
        }
        
        result += "]}";

        return result;
    }

    public static void Main(string[] args)
    {
        if (!HttpListener.IsSupported)
        {
            Console.WriteLine ("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            return;
        }

        // Create a listener.
        HttpListener listener = new HttpListener();
        // Add the prefixes.
        listener.Prefixes.Add("http://+:42000/");

        int sessionCounter = 0;
        Dictionary<int,SCXML> sessions = new Dictionary<int,SCXML>();

        listener.Start();

        while(true){
            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            System.IO.Stream output = response.OutputStream;

            String jsonBody  = new StreamReader(request.InputStream).ReadToEnd();

            //Dictionary<string, Dictionary<string,string>> values = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,string>>>(jsonBody);
            JObject reqJson = JObject.Parse(jsonBody);

            JToken scxmlToken,eventToken,sessionJsonToken;
            int sessionToken;

            try{
                if(reqJson.TryGetValue("load",out scxmlToken)){
                    Console.WriteLine("Loading new statechart");

                    string scxmlStr = scxmlToken.ToString();
                    SCXML scxml = new SCXML(new System.Uri(scxmlStr));
                    IList<string> initialConfiguration = scxml.Start();

                    sessionToken = sessionCounter++;
                    sessions[sessionToken] = scxml;

                    // Construct a response.
                    string responseString = TestServer.getResponseJson(sessionToken,initialConfiguration);
                    System.Console.WriteLine(responseString);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    output.Write(buffer,0,buffer.Length);
                }else if(reqJson.TryGetValue("event",out eventToken) && reqJson.TryGetValue("sessionToken",out sessionJsonToken)){

                    string eventName = (string) reqJson["event"]["name"];
                    sessionToken = (int) reqJson["sessionToken"];
                    
                    IList<string> nextConfiguration = sessions[sessionToken].Gen(eventName,null);

                    string responseString = TestServer.getResponseJson(sessionToken,nextConfiguration);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    output.Write(buffer,0,buffer.Length);
                }else{
                    string responseString = "Unrecognized request.";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    output.Write(buffer,0,buffer.Length);
                }
            }catch(Exception e){
                string responseString = "An error occured: " + e.ToString();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                output.Write(buffer,0,buffer.Length);
            }

            output.Close();
        }
        
    }
}
