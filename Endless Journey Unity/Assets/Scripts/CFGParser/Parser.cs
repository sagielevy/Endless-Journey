using Assets.Scripts.CFGParser.DataHolder;
using Assets.Scripts.CFGParser.Modifiers;
using EZObjectPools;
using Scripts.Tracery.Generator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.Scripts.CFGParser
{
    [RequireComponent(typeof(TerrainGenerator))]
    public class Parser : MonoBehaviour
    {
        public PostProcessingProfile myProfile;
        public WindZone wind;
        public ParticleSystem hail;

        SentenceDataHolder sentenceDataHolder;
        List<IWorldModifier> modifiers;
        List<IEnumerator<WaitForEndOfFrame>> modifierEnumerators;
        SectionModifier sectionModifier;
        AsyncCFGGenerator cFGGenerator;
        string[] colorPalettes;
        EZObjectPool[] pools;
        GameObject musicAudioSources;
        GameObject lights;
        bool afterInit;
        
        // Unity objects from scene
        Material SkyMat;

        public void Awake()
        {
            // Change seed!
            GetComponent<TerrainGenerator>().heightMapSettings.noiseSettings.seed = new System.Random().Next();

            // Init some stuff
            colorPalettes = Resources.Load<TextAsset>("Color Palettes" + Path.DirectorySeparatorChar + "Palettes").text.Split('\n');
            cFGGenerator = new AsyncCFGGenerator("CFG" + Path.DirectorySeparatorChar + "EndlessJourneyCFG", colorPalettes);
            modifiers = new List<IWorldModifier>();
            modifierEnumerators = new List<IEnumerator<WaitForEndOfFrame>>();
            afterInit = false;
        }

        public void Start()
        {
            // Init stuff
            SkyMat = RenderSettings.skybox;
            
            pools = GameObject.Find("OriginalModels").GetComponentsInChildren<EZObjectPool>();
            musicAudioSources = GameObject.Find("Tracks");
            lights = GameObject.Find("Lights");
        }

        public void FixedUpdate()
        {
            // If section modifier exists and is not completed, run modifiers
            if (sectionModifier == null || sectionModifier.IsSectionComplete())
            {
                // New section!
                sentenceDataHolder = cFGGenerator.GetSentence();

                // If null try getting sentence next call
                if (sentenceDataHolder != null)
                {
                    CreateNewModifiers();
                    afterInit = true;
                }
            }
            else if (afterInit)
            {
                // Call once after initializing new modifiers
                afterInit = false;

                // First off stop any coroutines running at this moment
                StopAllCoroutines();

                // Start coroutines once to run each modifier once per frame
                foreach (var modifierEnumerator in modifierEnumerators)
                {
                    StartCoroutine(modifierEnumerator);
                }
            }
        }

        private void CreateNewModifiers()
        {
            sectionModifier = new SectionModifier(GetComponent<TerrainGenerator>().viewer, sentenceDataHolder);

            // Clear Previous modifiers
            modifiers.Clear();

            //modifiers.Add(sectionModifier);
            modifiers.Add(new SkyModifier(SkyMat, lights));
            modifiers.Add(new GroundModifier(GetComponent<TerrainGenerator>().textureSettings, 
                            GetComponent<TerrainGenerator>().mapMaterial));
            modifiers.Add(new MusicModifier(musicAudioSources));
            modifiers.Add(new PathModifier(myProfile));
            modifiers.Add(new WeatherModifier(wind, hail));

            // Item modifiers
            modifiers.Add(new PlantsModifier(sentenceDataHolder, pools,
                GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new RocksModifier(sentenceDataHolder, pools,
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new CloudModifier(sentenceDataHolder, pools,
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new AnimalModifier(sentenceDataHolder, pools,
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));

            // Get all new enumerators
            modifierEnumerators.Clear();

            foreach (var modifier in modifiers)
            {
                modifierEnumerators.Add(modifier.ModifySection(sentenceDataHolder));
            }
        }
    }
}
