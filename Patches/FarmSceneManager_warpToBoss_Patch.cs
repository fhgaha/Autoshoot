using HarmonyLib;
using System.Collections.Generic;
using SharedLib;
using Atomicrops.Global;
using UnityEngine;
using Atomicrops.Game.Scenes.Farm;
using Atomicrops.Game.GameState;
using UnityEngine.Assertions;
using Atomicrops.Game.Data;

namespace AutoShoot
{
    [HarmonyPatch(typeof(FarmSceneManager), "_warpToBoss")]
    class FarmSceneManager_warpToBoss_Patch
    {
        static bool Prefix() => false;

        static void Postfix(FarmSceneManager __instance)
        {
            Debug.Log($"FarmSceneManager_warpToBoss_Patch.Postfix");

            if (SingletonSceneScopeAutoLoad<DayNightSystem>.I.GetState() == DayNightSystem.States.Dusk)
                SingletonSceneScopeAutoLoad<DayNightSystem>.I.GoToPhase(DayNightSystem.States.Night);

            var season = GameDate.GetSeason();
            List<BossDef> bosses = season switch
            {
                Season.Spring => SingletonScriptableObject<ConfigGame>.I.Bosses.Spring,
                Season.Summer => SingletonScriptableObject<ConfigGame>.I.Bosses.Summer,
                Season.Fall => SingletonScriptableObject<ConfigGame>.I.Bosses.Fall,
                Season.Winter => SingletonScriptableObject<ConfigGame>.I.Bosses.Winter,
                _ => null
            };

            Assert.IsTrue(bosses != null, $"no season bosses found");
            Assert.IsTrue(bosses.Count == 2, $"booses count is not 2: {bosses.Count}");

            bosses.ForEach(b => Debug.Log($"boss for season {season}: {b}"));

            SingletonSceneScope<BossesManager>.I.StartBoss(bosses[0], season);
            SingletonSceneScope<BossesManager>.I.StartBoss(bosses[1], season);


            int curHealth = SingletonScriptableObject<GameData>.I.PlayerHealth.Get();
            SingletonScriptableObject<GameData>.I.PlayerHealth.Add(Mathf.CeilToInt(curHealth * 0.5f)); 
        }
    }

}





