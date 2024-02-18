using Atomicrops.Game.DebugTools;
using Atomicrops.Game.GameState;
using HarmonyLib;
using System;
using UnityEngine;

namespace AutoShoot
{
    public static class DayNightSystem_Holder
    {
        public static bool dayPaused = false;
    }

    //this thing allows to autoshoot, like when you farming, on holding rmb
    [HarmonyPatch(typeof(DayNightSystem), "AwakeSub")]
    class DayNightSystem_AwakeSub_Patch
    {
        static void Postfix(DayNightSystem __instance)
        {
            __instance.onDayBegin += () =>
            {
                DayNightSystem_Holder.dayPaused = true;
                //disaple time counter
                DebugCommands.NoDayProgressOn();
            };
        }
    }

    [HarmonyPatch(typeof(DayNightSystem), "Update")]
    class DayNightSystem_Update_Patch
    {
        static void Postfix(DayNightSystem __instance)
        {
            if (Input.GetKeyUp(KeyCode.N) && DayNightSystem_Holder.dayPaused)
            {
                DebugCommands.NoDayProgressOff();
                DebugCommands.GoToDuskPhase();
                //enable time counter
                DayNightSystem_Holder.dayPaused = false;
            }
        }

    }
}





