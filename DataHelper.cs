using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LogiWiz
{
    public class DataHelper
    {
        public static string IP = "172.20.1.42";
        public static int Port = 38899;

        public static string ResolveInput(int input, int mode)
        {
            // Need to check the bulbs current state before sending a new setting.
            string getCurrState = "{\"method\":\"getPilot\",\"params\":{}}";
            string currState = SendData(getCurrState);
            if (currState == null) {
                Console.WriteLine("Unable to communicate with Bulb");
                return "1";
            }
            //
            Dictionary<string, string> stateMap = new Dictionary<string, string>()
            {
                {"temp", "0"},
                {"dimming", "0"}
            };
            string[] resArray = currState.Split(',');
            foreach (string s in resArray)
            {
                if (s.Contains("temp"))
                {
                    string[] tempArr = s.Split(":");
                    stateMap["temp"] = tempArr[1];
                }else if (s.Contains("dimming"))
                {
                    string[] dimArr = s.Split(":");
                    stateMap["dimming"] = dimArr[1].Trim('}');
                }
            }
            
            switch (mode)
            {
                // Case for dimming or brightening lights
                case 0:
                    string newDimState = PrepParams(input, stateMap["dimming"]);
                    string warmthState = stateMap["temp"];
                    string newParams = $"{{\"method\": \"setPilot\",\"params\": {{\"temp\": {warmthState}, \"dimming\": {newDimState}}}}}";
                    Console.WriteLine(newParams);
                    string lightResponse = SendData(newParams);
                    Console.WriteLine(lightResponse);
                    break;
                // Case for warming or cooling lights
                case 1:
                    PrepParams(1, stateMap["temp"]);
                    break;
                // Case for turning lights on or off
                case 2:
                    break;
            }
            return currState;
        }

        private static string PrepParams(int input, string currState)
        {
            int newState = 0;
            int intState = int.Parse(currState);
            switch (input)
            {
                case 0:
                    if (intState <= 90)
                    {
                        newState = intState + 10;
                    }
                    else
                    {
                        newState = 100;
                    }
                    break;
                case 1:
                    if (intState >= 20)
                    {
                        newState = intState - 10;
                    }
                    else
                    {
                        newState = 10;
                    }
                    break;
            }
            string newStringState = $"{newState}";
            return newStringState;
        }

        private static string SendData(string reqParams)
        {
            UdpClient udpClient = new UdpClient();
            IPAddress BulbIP = IPAddress.Parse(IP);
            IPEndPoint Endpoint = new IPEndPoint(BulbIP, Port);
            udpClient.Connect(BulbIP, Port);
            udpClient.Client.ReceiveTimeout = 3000;
            udpClient.Client.SendTimeout = 3000;

            byte[] data = Encoding.ASCII.GetBytes(reqParams);
            try
            {
                int sent = udpClient.Send(data, data.Length);
                Console.WriteLine(sent);
                byte[] rawRes = udpClient.Receive(ref Endpoint);
                string response = System.Text.Encoding.ASCII.GetString(rawRes, 0, rawRes.Length);
                return response;
            }
            catch (Exception e)
            {
                string response = e.ToString();
                return response;
            }
        }
    }
}
