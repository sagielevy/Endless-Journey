﻿using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public enum ColorIndicies
    {
        SkyColor = 0,
        HorizonColor,
        // PathColor,
        GroundColor,
        ItemColor1,
        ItemColor2
        // Clouds are always white - affected by sky & horizon colors
    }

    // Implement each section component here
    public class SentenceDataHolder : ISkyData, IAnimalsData, ICloudsData, IGroundData, IMusicData, IPathData,
                                      IPlantsData, ISectionData
    {
        const char colorDelimiter = ' ';
        const string colon = ";;;";
        const string arrayBracketOpen = "@!!";
        const string arrayBracketClose = "!!@";
        const string musicToken = "music";
        const string plantsToken = "plants";
        const string animalsToken = "animals";
        const string cloudsToken = "clouds";
        const string metallicToken = "metallic";
        const string smoothnessToken = "smoothness";
        const string pathGlowToken = "path_glow";
        const string colorSchemeToken = "color_scheme";
        const string skyGradientToken = "sky_gradient";
        const string skyGradientTrue = "sky_gradient";
        const string anglesToken = "angles";
        const string angleXToken = "angle_x";
        const string angleZToken = "angle_z";
        const string itemXPercentToken = "item_x";
        const string itemZPercentToken = "item_z";
        const string plantsSubtypeToken = "plant_subtype";
        const string cloudSubtypeToken = "cloud_subtype";
        const string animalSubtypeToken = "animal_subtype";
        const string sectionLengthToken = "section_length";

        private string orgSentence;
        JSONNode root;
       
        public SentenceDataHolder(string cfgSentence)
        {
            // Manually remove , before }
            orgSentence = cfgSentence.Replace(",}", "}").Replace(colon, ":").
                                      Replace(arrayBracketOpen, "[").Replace(arrayBracketClose, "]").Replace(",]", "]");
            //Debug.Log(orgSentence);
            root = JSON.Parse(orgSentence);

            Debug.Log("Color: " + ColorScheme());
        }

        public Item[] Animals()
        {
            return Items(animalsToken, animalSubtypeToken);
        }

        public Item[] Clouds()
        {
            return Items(cloudsToken, cloudSubtypeToken);
        }

        public Item[] Plants()
        {
            return Items(plantsToken, plantsSubtypeToken);
        }

        public string ColorScheme()
        {
            return root[colorSchemeToken].ToString();
        }

        public string ColorHorizon()
        {
            return root[colorSchemeToken].ToString().Split(colorDelimiter)[(int)ColorIndicies.HorizonColor];
        }

        public string ColorSky()
        {
            return root[colorSchemeToken].ToString().Split(colorDelimiter)[(int)ColorIndicies.SkyColor];
        }

        public bool IsSkyGradient()
        {
            return root[skyGradientToken].ToString() == skyGradientTrue;
        }

        public float Metallic()
        {
            return float.Parse(root[metallicToken].ToString());
        }

        public int MusicIndex()
        {
            return int.Parse(root[musicToken].ToString());
        }

        public float PathGlow()
        {
            return float.Parse(root[pathGlowToken].ToString());
        }

        public float[,] SectionAngles()
        {
            int length = root[anglesToken].Children.Count();
            float[,] result = new float[2, length];

            for (int i = 0; i < length; i++)
            {
                result[0,i] = float.Parse(root[anglesToken][i][angleXToken].ToString());
                result[1, i] = float.Parse(root[anglesToken][i][angleZToken].ToString());
            }

            return result;
        }

        public int SectionLength()
        {
            return int.Parse(root[sectionLengthToken].ToString());
        }

        public float Smoothness()
        {
            return float.Parse(root[smoothnessToken].ToString());
        }

        public override string ToString()
        {
            return orgSentence;
        }

        private Item[] Items(string itemTypeToken, string itemSubtypeToken)
        {
            int length = root[itemTypeToken].Children.Count();
            Item[] result = new Item[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = new Item(float.Parse(root[itemTypeToken][i][itemXPercentToken].ToString()),
                                     float.Parse(root[itemTypeToken][i][itemZPercentToken].ToString()),
                                     int.Parse(root[itemTypeToken][i][itemSubtypeToken].ToString()));
            }

            return result;
        }
    }
}
