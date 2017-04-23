﻿using GTA; // This is a reference that is needed! do not edit this
using GTA.Native; // This is a reference that is needed! do not edit this
using GTA.Math;
using System;
using System.Windows.Forms; // This is a reference that is needed! do not edit this
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace CommunicatorScriptExample
{
    public class UsageExample : Script // declare Modname as a script
    {
        ScriptCommunicator ExampleCommunicator = new ScriptCommunicator("CommunicatorScriptExample");
        //the EventName MUST be the same as the filename (without the extension) in order to communicate with my menu. 
        //So since this script's filename is CommunicatorScriptExample.cs, the event name should be CommunicatorScriptExample.

        public UsageExample() // main function
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            Interval = 0;
        }

        void OnTick(object sender, EventArgs e) // This is where most of your script goes
        {
            if (ExampleCommunicator.IsEventTriggered()) //if the communicator menu triggered the event for this script
            {
                Game.Player.Character.Task.Skydive();
                UI.ShowSubtitle("Looks like it works :)");

                ExampleCommunicator.ResetEvent(); //reset the event so that the menu is allowed to trigger this event again.

                ExampleCommunicator.BlockScriptCommunicatorModMenu();
                //This is optional. This will block the communicator menu from being able to open until you use UnblockScriptCommunicatorModMenu();. 
                //This is useful if you don't want to accidentally bring up the menu when your own mod's menu is open.
            }

            if (Game.Player.Character.IsWalking)
            {
                ExampleCommunicator.UnblockScriptCommunicatorModMenu(); 
                /*Allow the communicator menu to be allowed when you walk. 
                 * Be sure this is called if you ever use BlockScriptCommunicatorModMenu();, 
                 * or else the communicator menu will never be able to be opened! 
                 * Typically you would run this when your own menu is closed.
                 */
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
        }
    }

    class ScriptCommunicator
    {
        EventWaitHandle MainHandle { get; set; }
        EventWaitHandle ScriptCommunicatorModMenuHandle;
        bool ScriptCommunicatorModMenuIsBlocked;

        public ScriptCommunicator(string EventName)
        {
            MainHandle = new EventWaitHandle(false, EventResetMode.ManualReset, EventName);
            ScriptCommunicatorModMenuHandle = new EventWaitHandle(true, EventResetMode.ManualReset, "ScriptCommunicator");
        }

        public bool IsEventTriggered()
        {
            return MainHandle.WaitOne(0);
        }

        public void TriggerEvent()
        {
            MainHandle.Set();
        }

        public void ResetEvent()
        {
            MainHandle.Reset();
        }

        public void BlockScriptCommunicatorModMenu()
        {
            if (!ScriptCommunicatorModMenuIsBlocked)
            {
                bool waitHandleExists = EventWaitHandle.TryOpenExisting("ScriptCommunicator", out ScriptCommunicatorModMenuHandle);
                if (waitHandleExists) { ScriptCommunicatorModMenuHandle.Reset(); ScriptCommunicatorModMenuIsBlocked = true; }
            }
            else
            {
                ScriptCommunicatorModMenuHandle.Reset();
            }
        }

        public void UnblockScriptCommunicatorModMenu()
        {
            if (IsScriptCommunicatorMenuBlocked() && ScriptCommunicatorModMenuIsBlocked)
            {
                ScriptCommunicatorModMenuHandle.Set();
                ScriptCommunicatorModMenuIsBlocked = false;
            }
        }

        public bool IsScriptCommunicatorMenuBlocked()
        {
            return !ScriptCommunicatorModMenuHandle.WaitOne(0);
        }

        public bool DoesScriptCommunicatorMenuExist()
        {
            return File.Exists(@"scripts\ScriptCommunicator.dll");
        }
    }
}