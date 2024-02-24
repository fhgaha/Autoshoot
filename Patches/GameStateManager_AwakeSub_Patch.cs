using HarmonyLib;
using UnityEngine;
using Atomicrops.Game.GameState;
using System.IO;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace AutoShoot
{
    public static class Mat
    {
        public static Material mat;
        public static Texture2D dithTex;
        public static Texture2D rampTex;

        public static void SetUp(string path)
        {
            var dith = AssetBundle.LoadFromFile(path);
            var shader = (Shader)dith.LoadAsset("Dither");

            mat = new Material(shader);
            dithTex = (Texture2D)dith.LoadAsset("bayer1920x1080");
            rampTex = (Texture2D)dith.LoadAsset("ramp4x1");
            mat.SetTexture("_Dither", dithTex);
            mat.SetTexture("_ColorRamp", rampTex);
            Debug.Log("_MainTex: " + mat.GetTexture("_MainTex"));
            Debug.Log("_Dither: " + mat.GetTexture("_Dither"));
            Debug.Log("_ColorRamp: " + mat.GetTexture("_ColorRamp"));
        }
    }

    [HarmonyPatch(typeof(GameStateManager), "AwakeSub")]
    class GameStateManager_AwakeSub_Patch
    {
        public static string ModDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Postfix(GameStateManager __instance)
        {
            CreateMyConsole();
            ApplyDitherOnCam(__instance);
        }

        private static void CreateMyConsole()
        {
            var obj = new GameObject("MyConsoleObj");
            obj.AddComponent<MyConsole>();
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }

        private static void ApplyDitherOnCam(GameStateManager inst)
        {
            Mat.SetUp(Path.Combine(ModDirectory, "AssetBundles", "dithab"));

            Camera cam = Camera.current != null ? Camera.current : Camera.main;
            AwesomeScreenShader holder = cam.gameObject.AddComponent<AwesomeScreenShader>();
            holder.SetMat(Mat.mat);
        }
    }

    [HarmonyPatch(typeof(GameStateManager), "Update")]
    class GameStateManager_Update_Patch
    {
        static void Postfix(GameStateManager __instance)
        {
            //set shader to current cam (every update)
            if (Camera.current is var cam && cam != null)
                if (cam.gameObject.GetComponent<AwesomeScreenShader>() == null)
                {
                    AwesomeScreenShader holder = cam.gameObject.AddComponent<AwesomeScreenShader>();
                    holder.SetMat(Mat.mat);
                }
        }
    }

    [HarmonyPatch(typeof(GameStateManager), "OnSceneStart")]
    class GameStateManager_OnSceneStart_Patch
    {
        class Waiter : MonoBehaviour
        {
            public void Wait(Action action)
            {
                StartCoroutine(Rout(action));
                IEnumerator Rout(Action action)
                {
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    action?.Invoke();
                }
            }

            public void Wait(float seconds, Action action)
            {
                StartCoroutine(Rout(seconds, action));
                IEnumerator Rout(float seconds, Action action)
                {
                    yield return new WaitForSeconds(seconds);
                    action?.Invoke();
                }
            }
        }

        class Cathcer : MonoBehaviour
        {
            public static Cathcer Inst;
            public static string Name = "CamCatcherObj";
            public GameObject cam;

            void Start()
            {
                if (Inst != null)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }
                Inst = this;
                GameObject.DontDestroyOnLoad(gameObject);

                if (GameObject.Find(Name) is var other && other != null && other != gameObject)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }

                StartCoroutine(Rtn());
                IEnumerator Rtn()
                {
                    while (cam == null)
                    {
                        yield return new WaitForEndOfFrame();
                        var found = GameObject.Find("UiCam");
                        if (found != null)
                        {
                            cam = found;
                            Debug.Log($"CamCatcher found camera: {found}");
                            yield break;
                        }

                        //Debug.Log($"CamCatcher did not found UiCam yet");
                    }
                }
            }

            public void DoWhenFind(string objPath, Action action)
            {
                GameObject found = null;
                StartCoroutine(R());
                IEnumerator R()
                {
                    while (found == null)
                    {
                        found = GameObject.Find(objPath);
                        yield return new WaitForSeconds(0.1f);
                    }
                    action();
                    yield break;
                }
            }
        }

        static void Postfix(GameStateManager __instance)
        {

            //deal with ui
            Debug.Log($"Camera.main == null: {Camera.main == null}");
            Debug.Log($"Camera.main.GetComponent<PixelPerfectCamera>(): {Camera.main.GetComponent<PixelPerfectCamera>()}");
            DisableAndEnablePixelPerfectCamera();

            new GameObject(Cathcer.Name, typeof(Cathcer));
            Debug.Log($"--OnSceneStart.Postfix, cur scene: {SceneManager.GetActiveScene().name}");

            switch (SceneManager.GetActiveScene().name)
            {
                case "NewGameScene":    //this is loading screen
                    break;
                case "MainMenu":
                    HandleMainMenu();
                    break;
                case "FarmHouse":       //hub area
                    HandleFarmHouse();
                    break;
                case "Farm":
                    HandleFarm();
                    break;
                case "Town":
                    HandleTown();
                    break;
            }
        }

        static void HandleMainMenu()
        {
            ChangeScaleLocPos(GameObject.Find("/Canvas"), new[] { "MainMenu" });

            //ChangeScaleLocPos(GameObject.Find("/__UiLoadingSreen"), new[] { "Spring", "Summer", "Fall", "Winter", "Atomic", "Blank" });


            ////it starts ok thank resets
            var obj = GameObject.Find("/__UiLoadingSreen");
            var canv = obj.GetComponent<Canvas>();
            canv.renderMode = RenderMode.ScreenSpaceCamera;
            canv.worldCamera = Camera.main;


        }

        static void HandleFarmHouse()
        {
            //weird
            //var obj = GameObject.Find("/__UiLoadingSreen");
            //var canv = obj.GetComponent<Canvas>();
            //canv.renderMode = RenderMode.ScreenSpaceCamera;
            //canv.worldCamera = Camera.main;

            ChangeScaleLocPos(GameObject.Find("/NoTransform/CopiaHud"), new[] { "Image", "Text" });
            ChangeScaleLocPos(GameObject.Find("/NoTransform/HudCatCount"), new[] { "Grp" });
            ChangeScaleLocPos(GameObject.Find("/UiDialogInfoRegion(Clone)"), new[] { "UiDialogInfoRegion" });
            ChangeScaleLocPos(GameObject.Find("/NoTransform/UiMetaUpgradeDisplay"), new[] { "Grp" });
            ChangeScaleLocPos(GameObject.Find("/NoTransform/UiCatMetaGame"), new[] { "Grp" });
            ChangeScaleLocPos(GameObject.Find("/NoTransform/ChangeYearUi"), new[] { "Grp" });
            ChangeScaleLocPos(GameObject.Find("/Placeables/ChangeCharacterHatch/ChangeCharacterHatchUi"), new[] { "Grp" });




            //var canv = copiaObj.GetComponent<Canvas>();
            //var canvOldScale = canv.transform.localScale;

            ////before
            ////https://i.imgur.com/4HYNspK.png
            ////https://i.imgur.com/LrExMVP.png

            ////after
            ////https://i.imgur.com/i0oFAsF.png
            ////https://i.imgur.com/UbVPHmF.png

            //var imgTrans = copiaObj.transform.Find("Image");
            //var imgOldLocPos = imgTrans.localPosition;

            //var textTrans = copiaObj.transform.Find("Text");
            //var textOldLocPos = textTrans.localPosition;

            //canv.renderMode = RenderMode.ScreenSpaceCamera;
            //canv.worldCamera = Camera.main;

            //imgTrans.localPosition = new Vector3(imgOldLocPos.x / canvOldScale.x, imgOldLocPos.y / canvOldScale.y, 1);
            //imgTrans.localScale = new Vector3(imgTrans.localScale.x / canvOldScale.x, imgTrans.localScale.y / canvOldScale.y, 1);

            //textTrans.localPosition = new Vector3(textOldLocPos.x / canvOldScale.x, textOldLocPos.y / canvOldScale.y, 1);
            //textTrans.localScale = new Vector3(textTrans.localScale.x / canvOldScale.x, textTrans.localScale.y / canvOldScale.y, 1);
        }

        static void HandleFarm()
        {
            ChangeScaleLocPos(GameObject.Find("/MainUi2(Clone)"));
            ChangeScaleLocPos(GameObject.Find("/UiHudSeedWheel"));
            ChangeScaleLocPos(GameObject.Find("/UiHudTime"));
            ChangeScaleLocPos(GameObject.Find("/Atomicrops.Game.Ui.Hud.UiHudDate"));
            ChangeScaleLocPos(GameObject.Find("/UiHudTractor"));
            ChangeScaleLocPos(GameObject.Find("/UiHudNightWaves"));
            ChangeScaleLocPos(GameObject.Find("/UiHudCropLevelingProg"));
        }

        static void HandleTown()
        {
            //same as handle farm
            ChangeScaleLocPos(GameObject.Find("/MainUi2(Clone)"));
            ChangeScaleLocPos(GameObject.Find("/UiHudSeedWheel"));
            ChangeScaleLocPos(GameObject.Find("/UiHudTime"));
            ChangeScaleLocPos(GameObject.Find("/Atomicrops.Game.Ui.Hud.UiHudDate"));
            ChangeScaleLocPos(GameObject.Find("/UiHudTractor"));
            ChangeScaleLocPos(GameObject.Find("/UiHudNightWaves"));
            ChangeScaleLocPos(GameObject.Find("/UiHudCropLevelingProg"));

            //Cathcer.Inst.DoWhenFind("/UiTallyDaily2_pool", 
            //    () => ChangeScaleLocPos(GameObject.Find("/UiTallyDaily2_pool/UiTallyDaily2(Clone)")));


            //var found = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(obj => obj.name == "UiTallyDaily2_pool");
            //Debug.Log($"found == null: {found == null}");

            //foreach (var item in SceneManager.GetActiveScene().GetRootGameObjects())
            //{
            //    Debug.Log($"***found root obj: {item.name}");
            //}

            //var UiTallyDaily2_pool = GameObject.Find("/UiTallyDaily2_pool");    //null
            //var UiTallyDaily2 = GameObject.Find("/UiTallyDaily2_pool/UiTallyDaily2(Clone)");    //null
            //Debug.Log($"UiTallyDaily2_pool: {UiTallyDaily2_pool}, UiTallyDaily2: {UiTallyDaily2}");

            //ChangeScaleLocPos(GameObject.Find("/UiTallyDaily2_pool/UiTallyDaily2(Clone)"));   //null ref
        }

        public static void ChangeScaleLocPos(GameObject parent, params string[] chldNames)
        {
            var canv = parent.GetComponent<Canvas>();
            var canvOldScale = canv.transform.localScale;

            List<(Transform, Vector3)> children = chldNames
                .Select(c => parent.transform.Find(c))
                .Select(c => (trans: c, oldLocPos: c.localPosition))
                .ToList();

            if (children.Count < chldNames.Length)
                Debug.LogError($"children.Count < chldNames.Length: {children.Count} < {chldNames.Length}");

            canv.renderMode = RenderMode.ScreenSpaceCamera;
            canv.worldCamera = Camera.main;

            foreach ((Transform trans, Vector3 oldLocPos) in children)
            {
                trans.localPosition = new Vector3(oldLocPos.x / canvOldScale.x, oldLocPos.y / canvOldScale.y, 1);
                trans.localScale = new Vector3(trans.localScale.x / canvOldScale.x, trans.localScale.y / canvOldScale.y, 1);
            }
        }

        public static void ChangeScaleLocPos(GameObject parent)
        {
            var canv = parent.GetComponent<Canvas>();
            var canvOldScale = canv.transform.localScale;

            List<(Transform, Vector3)> children = parent.transform.GetTopLevelChildren()
                .Select(c => (trans: c, oldLocPos: c.localPosition)).ToList();

            canv.renderMode = RenderMode.ScreenSpaceCamera;
            canv.worldCamera = Camera.main;

            foreach ((Transform trans, Vector3 oldLocPos) in children)
            {
                trans.localPosition = new Vector3(oldLocPos.x / canvOldScale.x, oldLocPos.y / canvOldScale.y, 1);
                trans.localScale = new Vector3(trans.localScale.x / canvOldScale.x, trans.localScale.y / canvOldScale.y, 1);
            }
        }

        private static void DisableAndEnablePixelPerfectCamera()
        {
            PixelPerfectCamera ppc = Camera.main.GetComponent<PixelPerfectCamera>();

            GameObject go = new GameObject("temp", typeof(Waiter));
            Waiter cmp = go.GetComponent<Waiter>();

            if (ppc != null)
            {
                ppc.enabled = false;
                cmp.Wait(() =>
                {
                    ppc.enabled = true;
                    GameObject.Destroy(go);
                });
            }
        }



    }

    [HarmonyPatch(typeof(UiTallyDaily2), "Awake")]
    class UiTallyDaily2_Awake_Patch
    {
        static void Postfix(UiTallyDaily2 __instance)
        {
            GameStateManager_OnSceneStart_Patch.ChangeScaleLocPos(__instance.gameObject);
        }
    }
}





