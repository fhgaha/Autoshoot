using HarmonyLib;
using SharedLib;
using System.Collections.Generic;
using UnityEngine;
using Atomicrops.Global;
using System.Linq;

namespace AutoShoot
{
    ////this is called right before boss apearance
    //[HarmonyPatch(typeof(BossesManager), "GetBossDefForSeason", new[] { typeof(Season) })]
    //class BossesManager_GetBossDefForSeason_Patch
    //{
    //    static List<BossDef> pool;

    //    static bool Prefix() => false;

    //    static void Postfix(ref BossDef __result, BossesManager __instance, Season season)
    //    {
    //        if (pool == null)
    //        {
    //            List<BossDef> spring = SingletonScriptableObject<ConfigGame>.I.Bosses.Spring;
    //            List<BossDef> summer = SingletonScriptableObject<ConfigGame>.I.Bosses.Summer;
    //            List<BossDef> fall = SingletonScriptableObject<ConfigGame>.I.Bosses.Fall;
    //            List<BossDef> winter = SingletonScriptableObject<ConfigGame>.I.Bosses.Winter;
    //            pool = spring.Union(summer).Union(fall).Union(winter).ToList();
    //        }

    //        if (pool.Count == 0)
    //        {
    //            Debug.LogError("No bosses found");
    //            __result = null;
    //            return;
    //        }

    //        Debug.Log($"@@@ BossesManager_GetBossDefForSeason_Patch.Postfix");
    //        foreach (var def in pool)
    //        {
    //            Debug.Log($"{def}");
    //        }

    //        var idx = new System.Random().Next(pool.Count);
    //        Debug.Log($"Random boss picked: {pool[idx]}");
    //        __result = pool[idx];
    //        pool.RemoveAt(idx);
    //    }
    //}



}





