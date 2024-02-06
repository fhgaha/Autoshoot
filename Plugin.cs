using BepInEx;
using HarmonyLib;
using Atomicrops.Core.SoDb2;
using Atomicrops.Core.Upgrades;
using Atomicrops.Core.Loot;
using Atomicrops.Crops;
using Atomicrops.Game.Loot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.ImageConversion;

//my things
using Atomicrops.Game.Player;
using Atomicrops.Game.Player.PlayerGun;
using System.Text;
using SharedLib;

namespace AutoShoot
{
    public static class MyPluginInfo
    {
        public const string PLUGIN_GUID = "fhghaha.plugin.AutoShoot";
        public const string PLUGIN_NAME = "AutoShoot";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource Log;  // Changed to public

        private void Awake()
        {
            Log = Logger;
            Logger.LogMessage($"---------------Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!---------------");

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }


    [HarmonyPatch(typeof(PlayerAutoFireWhileFarming), "Update")]
    class PlayerGunInputs_Patch
    {
        //_isOn
        //static AccessTools.FieldRef<PlayerAutoFireWhileFarming, bool> isOn
        //    = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, bool>("_isOn");

        ////!GunInputs.IsPaused()
        //static AccessTools.FieldRef<PlayerAutoFireWhileFarming, PlayerGunInputs> gunInputs
        //    = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, PlayerGunInputs>("GunInputs");

        ////GunController.GetCurrentGun() != null
        //static AccessTools.FieldRef<PlayerAutoFireWhileFarming, PlayerGunController> gunContr
        //    = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, PlayerGunController>("GunController");

        ////GridWork.GetSmartFarmButton()
        ////GridWork.IsDoingWork()
        //static AccessTools.FieldRef<PlayerAutoFireWhileFarming, PlayerGridWorkController> gridWork
        //    = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, PlayerGridWorkController>("GridWork");

        static StringBuilder sb = new();


        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, bool> hasEnemyInRange
            = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, bool>("_hasEnemyInRange");

        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, float> isFiring
            = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, float>("_isFiring");

        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, bool> isWaitingForReload
            = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, bool>("_isWaitingForReload");

        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, float> triggerPressTimer
            = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, float>("_triggerPressTimer");

        //static void Prefix(PlayerAutoFireWhileFarming __instance)
        //{
        //    sb.AppendLine("-----------------------------------------------------");
        //    sb.AppendLine($"isOn(inst)                          {isOn(__instance)}");                             //true
        //    sb.AppendLine($"gunInputs(inst).IsPaused()          {gunInputs(__instance).IsPaused()}");             //false
        //    sb.AppendLine($"gunContr(inst).GetCurrentGun()      {gunContr(__instance).GetCurrentGun()}");         //!= null
        //    sb.AppendLine($"gridWork(inst).GetSmartFarmButton() {gridWork(__instance).GetSmartFarmButton()}");    //true
        //    sb.AppendLine($"gridWork(inst).IsDoingWork()        {gridWork(__instance).IsDoingWork()}");           //true
        //    Debug.Log(sb.ToString());



        //    //imitate invocation of GridWork.GetSmartFarmButton()
        //}

        private static void Postfix(PlayerAutoFireWhileFarming __instance)
        {
            Debug.LogWarning("Posfix 22:01");

            if (hasEnemyInRange(__instance))
            {
                isFiring(__instance) = 1f;
            }
            else
            {
                isWaitingForReload(__instance) = false;
                isFiring(__instance) = -1f;
                triggerPressTimer(__instance) = -1f;
            }

            Debug.Log("--------------------------------------");
            Debug.Log($"hasEnemyInRange(__instance)     {hasEnemyInRange(__instance)}");
            Debug.Log($"isFiring(__instance)            {isFiring(__instance)}");
            Debug.Log($"isWaitingForReload(__instance)  {isWaitingForReload(__instance)}");
            Debug.Log($"triggerPressTimer(__instance)   {triggerPressTimer(__instance)}");

        }

    }




    //[HarmonyPatch(typeof(PlayerGunInputs), "Update")]
    //class PlayerGunInputs_Patch
    //{
    //    static AccessTools.FieldRef<PlayerGunInputs, bool> isTriggerDownRef
    //        = AccessTools.FieldRefAccess<PlayerGunInputs, bool>("_isTriggerDown");
    //    static AccessTools.FieldRef<PlayerGunInputs, Action<object, EventArgs>> OnWhileTriggerDownRef
    //        = AccessTools.FieldRefAccess<PlayerGunInputs, Action<object, EventArgs>>("OnWhileTriggerDown");
    //    static AccessTools.FieldRef<PlayerGunInputs, bool> blockTriggerInputsRef
    //        = AccessTools.FieldRefAccess<PlayerGunInputs, bool>("_blockTriggerInputsForFarmAutoShoot");


    //    static bool shoot = false;

    //    static void Prefix(PlayerGunInputs __instance)
    //    {
    //        Debug.Log("-------------------------------");
    //        Debug.Log($"{isTriggerDownRef(__instance)}, shoot {shoot}");

    //        if (Input.GetKey(KeyCode.H))
    //        {
    //            shoot = !shoot;
    //        }

    //        isTriggerDownRef(__instance) = shoot;

    //        Debug.Log($"{isTriggerDownRef(__instance)}, shoot {shoot}");


    //    }
    //}

}





