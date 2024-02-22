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



            ////set all image texture
            //UnityEngine.Object[] imgObjs = GameObject.FindObjectsOfTypeAll(typeof(Image));
            //var imgs = imgObjs?.Select(i => (Image)i).ToList();

            //foreach (var img in imgs)
            //{
            //    img.material = Hold.mat;
            //}


            var canv = GameObject.Find("Canvas");
            Debug.Log($"canv == null: {canv == null}");   //false on mainmenu scene, true on farm scene
            var cnvCmp = canv.GetComponent<Canvas>();
            Debug.Log($"cnvCmp == null: {cnvCmp == null}");
            cnvCmp.renderMode = RenderMode.ScreenSpaceCamera;
            cnvCmp.worldCamera = Camera.main;
            canv = GameObject.Find("Canvas");
            var imgChld = canv.GetComponentInChildren<Image>();     //just get child

            Debug.Log($"imgChld.transform.localScale: {imgChld.transform.localScale}");     //(1,0, 1,0, 1,0)
            Debug.Log($"canv.transform.localScale: {canv.transform.localScale}");           //(0,1, 0,1, 0,1)

            var newScale = new Vector3
            (
                //imgChld.transform.localScale.x / canv.transform.localScale.x,
                //imgChld.transform.localScale.y / canv.transform.localScale.y,
                //imgChld.transform.localScale.z / canv.transform.localScale.z
                0.25f, 0.25f, 1
            );
            imgChld.transform.localScale = newScale;

            Debug.Log($"imgChld.transform.localScale: {imgChld.transform.localScale}");     //(8,0, 8,0, 8,0)
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





