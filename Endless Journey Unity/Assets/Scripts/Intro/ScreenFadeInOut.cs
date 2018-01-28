using Assets.Scripts.CFGParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Intro
{
    [RequireComponent(typeof(IntroText))]
    public class ScreenFadeInOut : MonoBehaviour
    {

        public float fadeSpeed = 1.5f;
        private bool sceneStarting = true;
        private float lowAlpha = 0.1f;
        private float highAlpha = 0.9f;
        private IntroText intro;
        private float screenSizeVal;

        void Awake()
        {
            screenSizeVal = Screen.width + Screen.height;
            GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        }

        void Start()
        {
            intro = GetComponent<IntroText>();
            StartCoroutine(intro.TextCutscene(Globals.INTRO_TEXTS));
        }

        // Use this for initialization
        void Update()
        {
            // If screen size changed
            if (screenSizeVal != Screen.width + Screen.height)
            {
                // Update screen size value and re-create the texture rect
                screenSizeVal = Screen.width + Screen.height;
                GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            }

            // If the scene is starting...
            if (sceneStarting)
            {
                // ... call the StartScene function.
                StartScene();
            }
        }

        void FadeToClear()
        {
            GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, fadeSpeed * Time.deltaTime);
        }

        void FadeToBlack()
        {
            GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime);
        }

        void StartScene()
        {
            // Fade in and stop once intro is done
            if (intro.TextCutsceneComplete)
            {
                FadeToClear();

                if (GetComponent<GUITexture>().color.a <= lowAlpha)
                {
                    GetComponent<GUITexture>().color = Color.clear;
                    GetComponent<GUITexture>().enabled = false;
                    sceneStarting = false;
                }
            }
        }
    }
}
