using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Intro
{
    public class IntroText : MonoBehaviour
    {
        public bool TextCutsceneComplete;
        public float fadeSpeed = 1.5f;
        private float lowAlpha = 0.1f;
        private float highAlpha = 0.9f;

        void FadeTextIn()
        {
            GetComponent<GUIText>().color = Color.Lerp(GetComponent<GUIText>().color, Color.white, fadeSpeed * Time.deltaTime);
        }

        void FadeTextOut()
        {
            GetComponent<GUIText>().color = Color.Lerp(GetComponent<GUIText>().color, Color.clear, fadeSpeed * Time.deltaTime);
        }

        public IEnumerator TextCutscene(string[] texts)
        {
            // Init variables
            TextCutsceneComplete = false;
            GetComponent<GUIText>().color = Color.clear;
            GetComponent<GUIText>().text = texts[0];

            // Loop through the texts
            for (int i = 0; i < texts.Length; i++)
            {
                // Fade in
                while (GetComponent<GUIText>().color.a < highAlpha)
                {
                    FadeTextIn();
                    yield return new WaitForEndOfFrame();
                }

                // Finish white
                GetComponent<GUIText>().color = Color.white;

                yield return new WaitForSeconds(3f);

                // Fade out
                while (GetComponent<GUIText>().color.a > lowAlpha)
                {
                    FadeTextOut();
                    yield return new WaitForEndOfFrame();
                }

                // Finish clear.
                GetComponent<GUIText>().color = Color.clear;

                // Change text and wait a bit (if there's text to change to)
                if (i + 1 < texts.Length)
                {
                    GetComponent<GUIText>().text = texts[i + 1];
                }
            }

            TextCutsceneComplete = true;
        }
    }
}