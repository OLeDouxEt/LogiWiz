﻿using System;
using System.Threading;

namespace LogiWiz
{

    class LogiWizMain
    {
        static void Main(string[] args)
        {
            HideWindow.Hide();
            // Initialize the SDK
            if (LogitechGSDK.LogiLcdInit("LogiWiz", LogitechGSDK.LOGI_LCD_TYPE_MONO))
            {
                Console.WriteLine("Logitech SDK initialized successfully.");
                // Default mode is brightness labeled by 0
                int CurrMode = 0;
                // Dynamically set by change bulb to determine which bulb 
                string BulbIP = "";
                int CurrBulb = 0;
                while (true)
                {
                    // sleep is dynamically set if a button is pressed so the display will pause and the user
                    // can see the displayed message. 
                    int sleep = 100;
                    bool btn0 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0);
                    bool btn1 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1);
                    bool btn2 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_2);
                    bool btn3 = LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_3);
                    // 
                    if (btn0)
                    {
                        sleep = 1000;
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string dataSent = DataHelper.ResolveInput(0, CurrMode, BulbIP);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" Controlling Bulb At: {BulbIP}");
                        LogitechGSDK.LogiLcdMonoSetText(2, $" {dataSent}");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Bulb      Mode");
                    }
                    // 
                    else if (btn1)
                    {
                        sleep = 1000;
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string dataSent = DataHelper.ResolveInput(1, CurrMode, BulbIP);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" Controlling Bulb At: {BulbIP}");
                        LogitechGSDK.LogiLcdMonoSetText(2, $" {dataSent}");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Bulb      Mode");
                    }
                    // Condition for changing blulb to control
                    else if (btn2)
                    {
                        BulbIP = DataHelper.ChangeBulb(CurrBulb,out CurrBulb);
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        sleep = 1000;
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" Controlling Bulb At: {BulbIP}");
                        LogitechGSDK.LogiLcdMonoSetText(2, " ");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Bulb      Mode");
                    }
                    // Condition for changing mode
                    else if (btn3)
                    {
                        sleep = 1000;
                        CurrMode = IncrementMode(CurrMode);
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" Controlling Bulb At: {BulbIP}");
                        LogitechGSDK.LogiLcdMonoSetText(2, " Changing Mode");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Bulb      Mode");
                    }
                    // Normal display loop at rest when no button is pressed
                    else
                    {
                        string modeDisplay = DisplayCurrMode(CurrMode);
                        string inputDisplay = DetermineInputDisplay(CurrMode);
                        LogitechGSDK.LogiLcdMonoSetText(0, $" LogiWiz Mode: {modeDisplay}");
                        LogitechGSDK.LogiLcdMonoSetText(1, $" Controlling Bulb At: {BulbIP}");
                        LogitechGSDK.LogiLcdMonoSetText(2, " ");
                        LogitechGSDK.LogiLcdMonoSetText(3, $"{inputDisplay}Bulb      Mode");
                    }

                    // Update the LCD
                    LogitechGSDK.LogiLcdUpdate();
                    Thread.Sleep(sleep);
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
            if(mode < 3)
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
                case 3:
                    ModeToDisplay = "Set Scene";
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
                case 3:
                    InputToDisplay = " Previous  Next      ";
                    break;
            }
            return InputToDisplay;
        }
    }
}