using HarmonyLib;
using UnityEngine;

//my things

namespace AutoShoot
{
    //this thing allows to autoshoot, like when you farming, on holding rmb
    [HarmonyPatch(typeof(PlayerAutoFireWhileFarming), "Update")]
    class PlayerAutoFireWhileFarming_Patch
    {
        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, bool> hasEnemyInRange = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, bool>("_hasEnemyInRange");
        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, float> isFiring = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, float>("_isFiring");
        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, bool> isWaitingForReload = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, bool>("_isWaitingForReload");
        static AccessTools.FieldRef<PlayerAutoFireWhileFarming, float> triggerPressTimer = AccessTools.FieldRefAccess<PlayerAutoFireWhileFarming, float>("_triggerPressTimer");

        static void Postfix(PlayerAutoFireWhileFarming __instance)
        {
            WorkingMethod_Traverse(__instance);
        }

        static void WorkingMethod1(PlayerAutoFireWhileFarming __instance)
        {
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
        }

        //this method has larger range, but player sprite is a little broken
        static void WorkingMethod_Traverse(PlayerAutoFireWhileFarming __instance)
        {
            var trav = Traverse.Create(__instance);
            trav.Field<float>("_isFiring").Value = 1f;
        }
    }


}





