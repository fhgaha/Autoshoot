using HarmonyLib;
using UnityEngine;
using Atomicrops.Game.GameState;

namespace AutoShoot
{
    [HarmonyPatch(typeof(GameStateManager), "AwakeSub")]
    class GameStateManager_AwakeSub_MyConcole_Patch
    {
        static void Postfix(GameStateManager __instance)
        {
            CreateMyConsole();
        }

        private static void CreateMyConsole()
        {
            var obj = new GameObject("MyConsoleObj");
            obj.AddComponent<MyConsole>();
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }
    }
}





