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

namespace AutoShoot
{
    public static class Hold
    {
        public static Material mat;
        public static Texture2D dithTex;
        public static Texture2D rampTex;
    }

    [HarmonyPatch(typeof(GameStateManager), "AwakeSub")]
    class GameStateManager_AwakeSub_Patch
    {
        public static string ModDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Postfix(GameStateManager __instance)
        {
            CreateMyConsole();
            ApplyDither(__instance);
        }

        private static void CreateMyConsole()
        {
            var obj = new GameObject("MyConsoleObj");
            obj.AddComponent<MyConsole>();
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }

        private static void ApplyDither(GameStateManager inst)
        {
            SetUpHold();

            Camera cam = Camera.current != null ? Camera.current : Camera.main;
            AwesomeScreenShader holder = cam.gameObject.AddComponent<AwesomeScreenShader>();
            holder.SetMat(Hold.mat);
        }

        private static void SetUpHold()
        {
            var dith = AssetBundle.LoadFromFile(Path.Combine(ModDirectory, "AssetBundles", "dithab"));
            var shader = (Shader)dith.LoadAsset("Dither");

            Hold.mat = new Material(shader);
            Hold.dithTex = (Texture2D)dith.LoadAsset("bayer1920x1080");
            Hold.rampTex = (Texture2D)dith.LoadAsset("ramp4x1");
            Hold.mat.SetTexture("_Dither", Hold.dithTex);
            Hold.mat.SetTexture("_ColorRamp", Hold.rampTex);
            Debug.Log("_MainTex: " + Hold.mat.GetTexture("_MainTex"));
            Debug.Log("_Dither: " + Hold.mat.GetTexture("_Dither"));
            Debug.Log("_ColorRamp: " + Hold.mat.GetTexture("_ColorRamp"));
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
                    holder.SetMat(Hold.mat);
                }
        }
    }

    [HarmonyPatch(typeof(GameStateManager), "OnSceneStart")]
    class GameStateManager_OnSceneStart_Patch
    {
        class MyTempComp : MonoBehaviour
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

        class CamCatcher : MonoBehaviour
        {
            public static string Name = "CamCatcherObj";
            public GameObject cam;

            void Start()
            {
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
                        Debug.Log($"CamCatcher did not found UiCam yet");
                    }
                }


            }
        }

        static void Postfix(GameStateManager __instance)
        {
            Debug.Log($"Camera.main == null: {Camera.main == null}");
            Debug.Log($"Camera.main.GetComponent<PixelPerfectCamera>(): {Camera.main.GetComponent<PixelPerfectCamera>()}");
            DisableAndEnablePixelPerfectCamera();

            new GameObject(CamCatcher.Name, typeof(CamCatcher));



            Canvas[] canvasses = GameObject.FindObjectsOfType<Canvas>();
            foreach (var canv in canvasses)
            {
                var canvOldScale = canv.transform.localScale;
                canv.renderMode = RenderMode.ScreenSpaceCamera;
                canv.worldCamera = Camera.main;

                foreach (Transform chld in canv.transform)
                {
                    var newScale = new Vector3
                    (
                        chld.localScale.x / canvOldScale.x,
                        chld.localScale.y / canvOldScale.y,
                        1
                    );
                    chld.localScale = newScale;

                    //pos?
                }
            }
        }

        private static void DisableAndEnablePixelPerfectCamera()
        {
            PixelPerfectCamera ppc = Camera.main.GetComponent<PixelPerfectCamera>();

            GameObject go = new GameObject("temp", typeof(MyTempComp));
            MyTempComp cmp = go.GetComponent<MyTempComp>();

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
}





