using Assets.Scripts.CFGParser.DataHolder;
using Assets.Scripts.CFGParser.Modifiers;
using Scripts.Tracery.Generator;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public class Parser : MonoBehaviour
    {
        // TODO either make this class a MonoBehaviour and reference all the required components in the scene
        // or have another class do it. Either way make interface getters just like the sentence data ones.
        // Send to modifiers
        SentenceDataHolder sentenceDataHolder;
        List<IWorldModifier<ISentenceData>> modifiers;
        SectionModifier sectionModifier;
        AsyncCFGGenerator cFGGenerator;
        string[] colorPalettes;

        // Unity objects from scene
        Material SkyMat;

        public void Start()
        {
            // Init stuff
            SkyMat = RenderSettings.skybox;

            colorPalettes = Resources.Load<TextAsset>("Color Palettes" + Path.DirectorySeparatorChar + "Palettes").text.Split('\n');
            cFGGenerator = new AsyncCFGGenerator("CFG" + Path.DirectorySeparatorChar + "EndlessJourneyCFG", colorPalettes);
            modifiers = new List<IWorldModifier<ISentenceData>>();

            modifiers.Add(new SkyModifier(SkyMat))
        }

        public void Update()
        {
            
        }

        // TODO Think of a way to free items created previously
    }
}
