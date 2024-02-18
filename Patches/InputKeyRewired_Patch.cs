using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Patches
{
    //[HarmonyPatch(typeof(InputKeyRewired), "GetHold", new Type[] { typeof(object) })]
    //class InputKeyRewired_Patch
    //{
    //    static AccessTools.FieldRef<InputKeyRewired, bool> fromCode = AccessTools.FieldRefAccess<InputKeyRewired, bool>("_fromCode");
    //    static AccessTools.FieldRef<InputKeyRewired, bool> fromCodeIsHolding = AccessTools.FieldRefAccess<InputKeyRewired, bool>("_fromCodeIsHolding");
    //    static AccessTools.FieldRef<InputKeyRewired, object> reserver = AccessTools.FieldRefAccess<InputKeyRewired, object>("_reserver");

    //    static bool hold = false;

    //    static bool Prefix() => false;

    //    static void Postfix(InputKeyRewired __instance, object requester)
    //    {
    //        bool fromCode = (bool)Traverse.Create(__instance).Field("_fromCode").GetValue();
    //        bool fromCodeIsHolding = (bool)Traverse.Create(__instance).Field("_fromCodeIsHolding").GetValue();
    //        object reserver = Traverse.Create(__instance).Field("_reserver").GetValue();
    //        Rewired.Player player = null;

    //        //Debug.Log($"fromCode {fromCode}, fromCodeIsHolding {fromCodeIsHolding}, reserver {reserver }");


    //        if (fromCode)
    //        {
    //            return fromCodeIsHolding;
    //        }
    //        if (reserver == null)
    //        {
    //            return _player.GetButton((int)this._action);
    //        }
    //        return this._reserver == requester && this._player.GetButton((int)this._action);


    //    }
    //}
}
