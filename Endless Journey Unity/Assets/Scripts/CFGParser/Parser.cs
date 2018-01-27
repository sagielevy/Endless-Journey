using Assets.Scripts.CFGParser.DataHolder;
using Assets.Scripts.CFGParser.Modifiers;
using Scripts.Tracery.Generator;
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

        SentenceDataHolder sentenceDataHolder;
        List<IWorldModifier> modifiers;
        SectionModifier sectionModifier;
        AsyncCFGGenerator cFGGenerator;
        string[] colorPalettes;
        GameObject originalModels;
        GameObject musicAudioSources;
        
        // Unity objects from scene
        Material SkyMat;

        public void Start()
        {
            // Init stuff
            SkyMat = RenderSettings.skybox;

            colorPalettes = Resources.Load<TextAsset>("Color Palettes" + Path.DirectorySeparatorChar + "Palettes").text.Split('\n');
            cFGGenerator = new AsyncCFGGenerator("CFG" + Path.DirectorySeparatorChar + "EndlessJourneyCFG", colorPalettes);
            modifiers = new List<IWorldModifier>();
            originalModels = GameObject.Find("OriginalModels");
            musicAudioSources = GameObject.Find("Tracks");

            sentenceDataHolder = cFGGenerator.GetSentence(); // Load first sentence

            CreateNewModifiers();
        }

        public void Update()
        {
            if (!sectionModifier.IsSectionComplete())
            {
                // Run each modifier once per frame
                foreach (var modifier in modifiers)
                {
                    modifier.ModifySection(sentenceDataHolder);
                }
            }
            else
            {
                // New section!
                Debug.Log("New Section!");
                sentenceDataHolder = cFGGenerator.GetSentence();
                CreateNewModifiers();
            }
        }

        private void CreateNewModifiers()
        {
            sectionModifier = new SectionModifier(Camera.main.transform.position, Camera.main.transform, sentenceDataHolder);

            // Clear Previous modifiers
            modifiers.Clear();

            modifiers.Add(new SkyModifier(SkyMat));
            modifiers.Add(new GroundModifier(GetComponent<TerrainGenerator>().textureSettings, 
                            GetComponent<TerrainGenerator>().mapMaterial));
            modifiers.Add(new PlantsModifier(sentenceDataHolder, originalModels, 
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new RocksModifier(sentenceDataHolder, originalModels,
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new CloudModifier(sentenceDataHolder, originalModels, 
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new AnimalModifier(sentenceDataHolder, originalModels, 
                            GetComponent<TerrainGenerator>(), GetComponent<TerrainGenerator>().viewer.position));
            modifiers.Add(new MusicModifier(musicAudioSources));
            modifiers.Add(new PathModifier(myProfile));
        }
    }
}
