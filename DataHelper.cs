using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace LogiWiz
{
    public class DataHelper
    {
        public static int Port = 38899;
        private static string ConfigFile = @"\Config.txt";

        // Function to handle user input and call other functions that test connection to the target bulb and fetch it's current state.
        // This function will deterime what state needs to be changed based on the user input and mode, pass it off to "PrepBasicParams" to prepare
        // the parameters, call "SendData" to send those parameters to the bulb to change the state, and the return a message depending on the result
        // to display to the user.

        public static string ResolveInput(int input, int mode, string IP)
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
            string currState = SendData(getCurrState, IP);
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
            // Taking returned string response containing the bulb's current and parsing it into
            // the dictonary that was just created.
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
                    lightResponse = SendData(newParams, IP);
                    Console.WriteLine(lightResponse);
                    actionStatus = $"Setting Brightness To {newDimState}";
                    break;
                // Case for warming or cooling lights
                case 1:
                    upper = 6000;
                    lower = 2200;
                    modifier = 500;
                    string newWarmState = PrepBasicParams(input, stateMap["temp"], upper, lower, modifier);
                    string dimState = stateMap["dimming"];
                    newParams = $"{{\"method\": \"setPilot\",\"params\": {{\"temp\": {newWarmState}, \"dimming\": {dimState}}}}}";
                    lightResponse = SendData(newParams, IP);
                    Console.WriteLine(lightResponse);
                    actionStatus = $"Setting Temperature To {newWarmState}";
                    break;
                // Case for turning lights on or off
                case 2:
                    // Upper, lower, and modifier need to be passed in here, but will not be used.
                    string newPowerState = PrepBasicParams(input, stateMap["power"], upper, lower, modifier);
                    newParams = $"{{\"method\": \"setPilot\",\"params\": {{\"state\": {newPowerState}}}}}";
                    lightResponse = SendData(newParams, IP);
                    // Reading previous power state and users new desired power state to determine and inform the user
                    // if the bulb is already on, already off, going on, or turing off.  
                    if (stateMap["power"] == "true" && newPowerState == "true")
                    {
                        actionStatus = "Bulb is already on";
                    }
                    else if(stateMap["power"] == "true" && newPowerState == "false")
                    {
                        actionStatus = "Turning bulb off";
                    }
                    else if (stateMap["power"] == "false" && newPowerState == "false")
                    {
                        actionStatus = "Bulb is already off";
                    }
                    else
                    {
                        actionStatus = "Turning bulb on";
                    }
                    break;
                // Case for setting scene
                case 3:

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
            try
            {
                Ping netTest = new Ping();
                PingReply pingRes = netTest.Send(endpoint, 1000);
                if (pingRes.Status.ToString() == "TimedOut")
                {
                    ConnectionGood = false;
                }
                return ConnectionGood;
            }
            catch
            {
                ConnectionGood = false;
                return ConnectionGood;
            }
        }

        private static string SendData(string reqParams, string CurrIP)
        {
            UdpClient udpClient = new UdpClient();
            IPAddress BulbIP = IPAddress.Parse(CurrIP);
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

        private static List<string> ReadBulbsFile(string FileName)
        {
            string ExeDir = Directory.GetCurrentDirectory();
            string fullPath = $"{ExeDir}{FileName}";
            List<string> BulbData = new List<string>();
            try
            {
                string[] RawBulbData = File.ReadAllLines(fullPath);
                BulbData = new List<string>(RawBulbData);
                return BulbData;
            }
            catch (FileNotFoundException ex)
            {
                // Will raise alert if file is not found and try to create an
                // empty file and run the function again. Even if the file is empty,
                // this will be handled later.
                Console.WriteLine("File not found: " + ex.Message);
                try
                {
                    File.Create(fullPath);
                    return ReadBulbsFile(fullPath);
                }
                catch(Exception exception)
                {
                    Console.WriteLine("Unable to create file: " + exception.Message);
                    return BulbData;
                }
                
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
                return BulbData;
            }
        }

        public static string ChangeBulb(int BulbNum, out int CurrBulb)
        {
            List<string> BulbsList = ReadBulbsFile(ConfigFile);
            string newBulbIP = "";
            if (BulbsList.Count <= 0)
            {
                CurrBulb = 0;
                return "172.16.254.1";
            }
            if(BulbNum < (BulbsList.Count - 1))
            {
                CurrBulb = BulbNum + 1;
                newBulbIP = BulbsList[CurrBulb];
            }
            else
            {
                CurrBulb = 0;
                newBulbIP = BulbsList[CurrBulb];
            }
            return newBulbIP;
        }

        public static string ChangeScene(string SceneName)
        {
            return "";
        }
    }
}