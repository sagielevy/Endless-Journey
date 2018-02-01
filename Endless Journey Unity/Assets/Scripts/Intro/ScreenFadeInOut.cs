using Assets.Scripts.CFGParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Intro
{
    public class ScreenFadeInOut : MonoBehaviour
    {
        public Text text;
        public Image FadeImg;
        public float fadeSpeed = 1.5f;
        private bool sceneStarting = true;
        private float lowAlpha = 0.05f;
        private float highAlpha = 0.95f;
        private bool TextCutsceneComplete;

        void Start()
        {
            StartCoroutine(TextCutscene(Globals.INTRO_TEXTS));
        }

        void FixedUpdate()
        {
            // If the scene is starting...
            if (sceneStarting)
            {
                // ... call the StartScene function.
                StartScene();
            }
        }

        void FadeTextIn()
        {
            text.color = Color.Lerp(text.color, Color.white, fadeSpeed * Time.deltaTime);
        }

        void FadeTextOut()
        {
            text.color = Color.Lerp(text.color, Color.clear, fadeSpeed * Time.deltaTime);
        }

        public IEnumerator TextCutscene(string[] texts)
        {
            // Init variables
            TextCutsceneComplete = false;
            text.color = Color.clear;
            text.text = texts[0];

            // Loop through the texts
            for (int i = 0; i < texts.Length; i++)
            {
                // Fade in
                while (text.color.a < highAlpha)
                {
                    FadeTextIn();
                    yield return new WaitForEndOfFrame();
                }

                // Finish white
                text.color = Color.white;

                yield return new WaitForSeconds(3f);

                // Fade out
                while (text.color.a > lowAlpha)
                {
                    FadeTextOut();
                    yield return new WaitForEndOfFrame();
                }

                // Finish clear.
                text.color = Color.clear;

                // Change text and wait a bit (if there's text to change to)
                if (i + 1 < texts.Length)
                {
                    text.text = texts[i + 1];
                }
            }

            TextCutsceneComplete = true;
        }

        void FadeToClear()
        {
            // Lerp the colour of the image between itself and transparent.
            FadeImg.color = Color.Lerp(FadeImg.color, Color.clear, fadeSpeed * Time.deltaTime);
        }

        void FadeToBlack()
        {
            // Lerp the colour of the image between itself and black.
            FadeImg.color = Color.Lerp(FadeImg.color, Color.black, fadeSpeed * Time.deltaTime);
        }

        void StartScene()
        {
            // Fade in and stop once intro is done
            if (TextCutsceneComplete)
            {
                FadeToClear();

                if (FadeImg.color.a <= lowAlpha)
                {

                    FadeImg.color = Color.clear;
                    FadeImg.enabled = false;
                    sceneStarting = false;
                }
            }
        }
    }
}
