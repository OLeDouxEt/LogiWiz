using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace LogiWiz
{
    public class DataHelper
    {
        public static string IP = "";
        public static int Port = 38899;

        public static string ResolveInput(int input, int mode)
        {
            // Variable will be set depending on users input, if the input action was successfully, and will
            // be returned to be displayed to inform the user.
            string actionStatus = "";

            // THIS WILL IMMEDIATELY return an error to display if a connection to the bulb cannot be
            // established to avoid any further errors.
            bool connectionStatus = TestConnection(IP);
            if(connectionStatus == false)
            {
                actionStatus = "Cannot Communicate With Bulb";
                return actionStatus;
            }

            // Need to check the bulbs current state before sending a new setting.
            string getCurrState = "{\"method\":\"getPilot\",\"params\":{}}";
            string currState = SendData(getCurrState);
            if (currState == "null") {
                Console.WriteLine("Unable to communicate with Bulb");
                actionStatus = "Cannot Communicate With Bulb";
                return actionStatus;
            }
            // Need to save the bulb's current setting as only one will change. Others will be passed to the new params sent.
            // Important parameters are Power State, Temperature settings, and Brightness settings.
            Dictionary<string, string> stateMap = new Dictionary<string, string>()
            {
                {"temp", "0"},
                {"dimming", "0"},
                {"power", "true"}
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
                }else if (s.Contains("state"))
                {
                    string[] powerArr = s.Split(":");
                    stateMap["power"] = powerArr[1];
                }
            }
            
            // Need to set dynamic limits and modifiers for temp settings and brightness settings as the range
            // for brightness is 10-100 while temp is 1000-6200.
            int upper = 100;
            int lower = 10;
            int modifier = 10;
            string newParams;
            string lightResponse;
            switch (mode)
            {
                // Case for dimming or brightening lights
                case 0:
                    string newDimState = PrepBasicParams(input, stateMap["dimming"], upper, lower, modifier);
                    // Passing in the current state of temp setting fetched from
                    string warmthState = stateMap["temp"];
                    newParams = $"{{\"method\": \"setPilot\",\"params\": {{\"temp\": {warmthState}, \"dimming\": {newDimState}}}}}";
                    lightResponse = SendData(newParams);
                    Console.WriteLine(lightResponse);
                    actionStatus = $"Setting Brightness To {newDimState}";
                    break;
                // Case for warming or cooling lights
                case 1:
                    upper = 6200;
                    lower = 1000;
                    modifier = 1000;
                    string newWarmState = PrepBasicParams(input, stateMap["temp"], upper, lower, modifier);
                    string dimState = stateMap["dimming"];
                    newParams = $"{{\"method\": \"setPilot\",\"params\": {{\"temp\": {newWarmState}, \"dimming\": {dimState}}}}}";
                    lightResponse = SendData(newParams);
                    Console.WriteLine(lightResponse);
                    actionStatus = $"Setting Temperature To {newWarmState}";
                    break;
                // Case for turning lights on or off
                case 2:
                    // Upper, lower, and modifier need to be passed in here, but will not be used.
                    string newPowerState = PrepBasicParams(input, stateMap["power"], upper, lower, modifier);
                    newParams = $"{{\"method\": \"setPilot\",\"params\": {{\"state\": {newPowerState}}}}}";
                    lightResponse = SendData(newParams);
                    Console.WriteLine(lightResponse);
                    if (newParams == "true")
                    {
                        actionStatus = "Turning bulb on";
                    }
                    actionStatus = "Turning bulb off";
                    break;
            }
            return actionStatus;
        }

        // Function to handle changing temperature, brightness, and power state. For brightness and temperature, upper and lower limits
        // are passed in to set the ranges.
        private static string PrepBasicParams(int input, string currState, int up, int low, int mod)
        {
            if (currState == "true" || currState == "false")
            {
                string newPowState = "true";
                if (input == 0)
                {
                    newPowState = "false";
                }
                return newPowState;
            }
            else
            {
                int newState = 0;
                int intState = int.Parse(currState);
                switch (input)
                {
                    case 0:
                        if (intState <= (up - mod))
                        {
                            newState = intState + mod;
                        }
                        else
                        {
                            newState = up;
                        }
                        break;
                    case 1:
                        if (intState >= (low + mod))
                        {
                            newState = intState - mod;
                        }
                        else
                        {
                            newState = low;
                        }
                        break;
                }
                string newStringState = $"{newState}";
                return newStringState;
            }
        }
        // Function to check and see if bulb is online and will accept commands. Helps
        // prevent unnecessary connection attempts from the "SendData" function to a bulb offline.
        public static bool TestConnection(string endpoint)
        {
            bool ConnectionGood = true;
            Ping netTest = new Ping();
            PingReply pingRes = netTest.Send(endpoint, 1000);
            if (pingRes.Status.ToString() == "TimedOut")
            {
                ConnectionGood = false;
            }
            return ConnectionGood;
        }

        private static string SendData(string reqParams)
        {
            UdpClient udpClient = new UdpClient();
            IPAddress BulbIP = IPAddress.Parse(IP);
            IPEndPoint Endpoint = new IPEndPoint(BulbIP, Port);
            udpClient.Connect(BulbIP, Port);
            udpClient.Client.ReceiveTimeout = 2000;
            udpClient.Client.SendTimeout = 2000;

            byte[] data = Encoding.ASCII.GetBytes(reqParams);
            try
            {
                int sent = udpClient.Send(data, data.Length);
                byte[] rawRes = udpClient.Receive(ref Endpoint);
                string response = System.Text.Encoding.ASCII.GetString(rawRes, 0, rawRes.Length);
                Console.WriteLine(response);
                return response;
            }
            catch (Exception e)
            {
                string response = e.ToString();
                Console.WriteLine(response);
                return "null";
            }
        }
        /*
        private static string SendAndReceive()
        {

        }*/
    }
}