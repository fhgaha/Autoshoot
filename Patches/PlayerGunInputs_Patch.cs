//using HarmonyLib;
//using System;
//using System.Reflection;
//using UnityEngine;

////my things
//using Atomicrops.Game.Player;
//using Atomicrops.Game.Player.PlayerGun;
//using SharedLib;
//using Atomicrops.Core.CameraSystem;
//using UnityEngine.EventSystems;
//using Atomicrops.Core.GunSystem;

//namespace AutoShoot
//{
//    [HarmonyPatch(typeof(PlayerGunInputs), "Update")]
//    class PlayerGunInputs_Patch
//    {
//        static AccessTools.FieldRef<PlayerGunInputs, Action<object, EventArgs>> OnWhileTriggerDown = AccessTools.FieldRefAccess<PlayerGunInputs, Action<object, EventArgs>>("OnWhileTriggerDown");
//        static AccessTools.FieldRef<PlayerGunInputs, Camera> mainCam = AccessTools.FieldRefAccess<PlayerGunInputs, Camera>("_mainCam");
//        static AccessTools.FieldRef<PlayerGunInputs, PlayerComp> playerComp = AccessTools.FieldRefAccess<PlayerGunInputs, PlayerComp>("_playerComp");
//        static MethodInfo TriggerUp = AccessTools.Method(typeof(PlayerGunInputs), "TriggerUp");
//        static AccessTools.FieldRef<PlayerGunInputs, bool> isPaused = AccessTools.FieldRefAccess<PlayerGunInputs, bool>("_isPaused");
//        static AccessTools.FieldRef<PlayerGunInputs, bool> isTriggerDown = AccessTools.FieldRefAccess<PlayerGunInputs, bool>("_isTriggerDown");
//        static AccessTools.FieldRef<PlayerGunInputs, bool> forceInputFarmAutoShoot = AccessTools.FieldRefAccess<PlayerGunInputs, bool>("_forceInputFarmAutoShoot");
//        static AccessTools.FieldRef<PlayerGunInputs, Vector2> forceMousePos = AccessTools.FieldRefAccess<PlayerGunInputs, Vector2>("_forceMousePos");
//        static AccessTools.FieldRef<PlayerGunInputs, Vector2> forceAimDir = AccessTools.FieldRefAccess<PlayerGunInputs, Vector2>("_forceAimDir");
//        static AccessTools.FieldRef<PlayerGunInputs, EventSystem> eventSystem = AccessTools.FieldRefAccess<PlayerGunInputs, EventSystem>("_eventSystem");
//        static AccessTools.FieldRef<PlayerGunInputs, PlayerGunController> gunController = AccessTools.FieldRefAccess<PlayerGunInputs, PlayerGunController>("_gunController");
//        static AccessTools.FieldRef<PlayerGunInputs, Vector2> mousePos = AccessTools.FieldRefAccess<PlayerGunInputs, Vector2>("_mousePos");
//        static AccessTools.FieldRef<PlayerGunInputs, Vector2> _aimDir = AccessTools.FieldRefAccess<PlayerGunInputs, Vector2>("_aimDir");
//        AccessTools.FieldRef<PlayerGunController, IPlayerGun> gun = AccessTools.FieldRefAccess<PlayerGunController, IPlayerGun>("_currentGun");

//        static bool shoot;

//        //break original method
//        static bool Prefix() => false;

//        static void Postfix(PlayerGunInputs __instance)
//        {
//            if (mainCam(__instance) == null)
//            {
//                mainCam(__instance) = Camera.main;
//            }
//            Vector2 vector = Vector2.zero;
//            if (SingletonSceneScopeAutoLoad<InputHelper>.I != null)
//            {
//                vector = mainCam(__instance).ScreenToWorldPoint(SingletonSceneScopeAutoLoad<InputHelperRewired>.I.CursorPosition());
//            }
//            else
//            {
//                vector = mainCam(__instance).ScreenToWorldPoint(Input.mousePosition);
//            }
//            vector = CameraHelper.PixelPointToWorldPoint(vector);
//            Vector2 aimDir = SingletonSceneScope<PlayerInfo>.I.GetAimDirPotentiallyAutoAimed();
//            if (playerComp(__instance).IsDead())
//            {
//                if (isTriggerDown(__instance))
//                {
//                    __instance.TriggerUp();
//                }
//                return;
//            }
//            if (isPaused(__instance))
//            {
//                if (isTriggerDown(__instance))
//                {
//                    __instance.TriggerUp();
//                }
//                return;
//            }
//            if (forceInputFarmAutoShoot(__instance))
//            {
//                vector = forceMousePos(__instance);
//                aimDir = forceAimDir(__instance);
//            }

//            if (!eventSystem(__instance).IsPointerOverGameObject() && gunController(__instance).GetCurrentGun() != null)
//            {
//                if (SingletonSceneScopeAutoLoad<InputHelperRewired>.I.GetFire().GetHold(__instance))
//                {
//                    shoot = !shoot;
//                }

//                if (shoot)
//                {
//                    __instance.SwitchTo0(aimDir, vector);
//                    __instance.TriggerDown(aimDir, vector);
//                }
//                else
//                {
//                    __instance.SwitchTo0(aimDir, vector);
//                    __instance.TriggerUp();
//                }
//            }

//            __instance.ManualUpdate(aimDir, vector);
//            mousePos(__instance) = vector;
//            _aimDir(__instance) = aimDir;

//            if (isTriggerDown(__instance) && OnWhileTriggerDown(__instance) != null)
//            {
//                OnWhileTriggerDown(__instance)(__instance, EventArgs.Empty);
//            }
//        }
//    }





//}





