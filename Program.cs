using System;
using System.Threading;

namespace LogiWiz
{

    class LogiWizMain
    {
        static void Main(string[] args)
        {
            // Initialize the SDK
            if (LogitechGSDK.LogiLcdInit("LogiWiz", LogitechGSDK.LOGI_LCD_TYPE_MONO | LogitechGSDK.LOGI_LCD_TYPE_COLOR))
            {
                Console.WriteLine("Logitech SDK initialized successfully.");
                // Default mode is brightness labeled by 0
                int CurrMode = 0;
                while (true)
                {
                    bool Shutdown = false;
                    // sleep is dynamically set if a button is pressed so the display will pause and the user
                    // can see the displayed message. 
                    int sleep = 0;
                    bool btn0 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0);
                    bool btn1 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1);
                    bool btn2 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_2);
                    bool btn3 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_3);
                    if (btn0)
                    {
                        sleep = 1000;
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string dataSent = DataHelper.ResolveInput(0, CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" {dataSent}");
                        LogitechGSDK.LogiLcdMonoSetText(2, "");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Mode      Exit");
                    }
                    else if (btn1)
                    {
                        sleep = 1000;
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string dataSent = DataHelper.ResolveInput(1, CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" {dataSent}");
                        LogitechGSDK.LogiLcdMonoSetText(2, "");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Mode      Exit");
                    }
                    else if (btn2)
                    {
                        sleep = 1000;
                        CurrMode = IncrementMode(CurrMode);
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, " Changing Mode");
                        LogitechGSDK.LogiLcdMonoSetText(2, "");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Mode      Exit");
                    }
                    else if (btn3)
                    {
                        sleep = 1000;
                        Shutdown = true;
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, " LogiWiz Mode: ");
                        LogitechGSDK.LogiLcdMonoSetText(1, " Shutting Down");
                        LogitechGSDK.LogiLcdMonoSetText(2, "");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Mode      Exit");
                    }
                    // Normal display loop at rest when no button is pressed
                    else
                    {
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, "");
                        LogitechGSDK.LogiLcdMonoSetText(2, "");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Mode      Exit");
                    }

                    // Update the LCD
                    LogitechGSDK.LogiLcdUpdate();
                    Thread.Sleep(sleep);
                    if (Shutdown)
                    {
                        LogitechGSDK.LogiLcdShutdown();
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to initialize Logitech SDK.");
            }
        }

        public static int IncrementMode(int mode)
        {
            int newMode;
            if(mode < 2)
            {
                newMode = mode += 1;
            }
            else
            {
                newMode = 0;
            }
            return newMode;
        }

        public static string DisplayCurrMode(int mode)
        {
            string ModeToDisplay = "";
            switch (mode)
            {
                case 0:
                    ModeToDisplay = "Brightness";
                    break;
                case 1:
                    ModeToDisplay = "Temperature";
                    break;
                case 2:
                    ModeToDisplay = "On/Off";
                    break;
            }
            return ModeToDisplay;
        }
        // Function to change inputs depending on currrent mode. Inputs to display will adjust whitespace depending
        // on which inputs need to be displayed.
        public static string DetermineInputDisplay(int mode)
        {
            string InputToDisplay = "";
            switch (mode)
            {
                case 0:
                    InputToDisplay = "     +            -          ";
                    break;
                case 1:
                    InputToDisplay = "     +            -          ";
                    break;
                case 2:
                    InputToDisplay = "   Off         On        ";
                    break;
            }
            return InputToDisplay;
        }
    }
}