using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using System.IO;
using Atomicrops.Game.Player.PlayerGun;
using UnityEngine;

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
            Logger.LogMessage($"---------------{GetBuildDateTime()}---------------");

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }

        private static DateTime? GetBuildDateTime()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attr = Attribute.GetCustomAttribute(assembly, typeof(BuildDateTimeAttribute)) as BuildDateTimeAttribute;
            return attr?.Built;
        }
    }

    //    [HarmonyPatch(typeof(InputKeyRewired), "GetHold", new Type[] { typeof(object) })]
    //    class InputKeyRewired_Patch
    //    {
    //        static AccessTools.FieldRef<InputKeyRewired, bool> fromCode = AccessTools.FieldRefAccess<InputKeyRewired, bool>("_fromCode");
    //        static AccessTools.FieldRef<InputKeyRewired, bool> fromCodeIsHolding = AccessTools.FieldRefAccess<InputKeyRewired, bool>("_fromCodeIsHolding");
    //        static AccessTools.FieldRef<InputKeyRewired, object> reserver = AccessTools.FieldRefAccess<InputKeyRewired, object>("_reserver");

    //        static bool hold = false;

    //        static bool Prefix() => false;

    //        static void Postfix(InputKeyRewired __instance, object requester)
    //        {
    //            bool fromCode = (bool)Traverse.Create(__instance).Field("_fromCode").GetValue();
    //            bool fromCodeIsHolding = (bool)Traverse.Create(__instance).Field("_fromCodeIsHolding").GetValue();
    //            object reserver = Traverse.Create(__instance).Field("_reserver").GetValue();
    //            Rewired.Player player = null;

    //            //Debug.Log($"fromCode {fromCode}, fromCodeIsHolding {fromCodeIsHolding}, reserver {reserver }");


    //            if (fromCode)
    //            {
    //                return fromCodeIsHolding;
    //            }
    //            if (reserver == null)
    //            {
    //                return _player.GetButton((int)this._action);
    //            }
    //            return this._reserver == requester && this._player.GetButton((int)this._action);


    //        }
    //    }
}





