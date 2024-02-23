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

                        //Debug.Log($"CamCatcher did not found UiCam yet");
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
            Debug.Log($"--OnSceneStart.Postfix, cur scene: {SceneManager.GetActiveScene().name}");

            switch (SceneManager.GetActiveScene().name)
            {
                case "MainMenu":
                    HandleMainMenu();
                    break;
                case "FarmHouse":
                    HanldeFarmHouse();
                    break;
            }
        }

        static void HandleMainMenu()
        {
            var canvObj = GameObject.Find("Canvas");
            Debug.Log($"canvObj == null: { canvObj == null}");
            var canv = canvObj.GetComponent<Canvas>();
            Debug.Log($"canvCmp == null: { canv == null}");
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

        static void HanldeFarmHouse()
        {
            var copiaObj = GameObject.Find("CopiaHud");
            ChangeScaleLocPos(copiaObj, new[] { "Image", "Text" });




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

        static void ChangeScaleLocPos(GameObject copiaObj, params string[] chldNames)
        {
            var canv = copiaObj.GetComponent<Canvas>();
            var canvOldScale = canv.transform.localScale;

            List<(Transform, Vector3)> children = chldNames
                .Select(c => copiaObj.transform.Find(c))
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
}





