﻿using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
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
        GroundColor1,
        GroundColor2,
        GroundColor3
        // Clouds are always white - affected by sky & horizon colors
    }

    // Implement each section component here
    public class SentenceDataHolder : ISkyData, IAnimalsData, ICloudsData, IGroundData, IMusicData, IPathData,
                                      IPlantsData, IRocksData, ISectionData
    {
        const char colorDelimiter = ' ';
        const string colon = ";;;";
        const string arrayBracketOpen = "@!!";
        const string arrayBracketClose = "!!@";
        const string musicToken = "music";
        const string plantsToken = "plants";
        const string animalsToken = "animals";
        const string rocksToken = "rocks";
        const string cloudsToken = "clouds";
        const string metallicToken = "metallic";
        const string smoothnessToken = "smoothness";
        const string pathGlowToken = "path_glow";
        const string pathThresholdToken = "path_threshold";
        const string colorSchemeToken = "color_scheme";
        const string skyGradientToken = "sky_gradient";
        const string skyGradientTrue = "gradient";
        const string anglesToken = "angles";
        const string angleAngleToken = "angle";
        const string anglePosXToken = "pos_x";
        const string anglePosZToken = "pos_z";
        const string itemXPercentToken = "item_x";
        const string itemZPercentToken = "item_z";
        const string itemScaleToken = "scale";
        const string plantsSubtypeToken = "plant_subtype";
        const string rocksSubtypeToken = "rock_subtype";
        const string cloudSubtypeToken = "cloud_subtype";
        const string animalSubtypeToken = "animal_subtype";
        const string animalAngleToken = "animal_angle";
        const string sectionLengthToken = "section_length";

        private string orgSentence;
        string[] colorPalettes;
        JSONNode root;
        
        public SentenceDataHolder(string cfgSentence, string[] colorPalettes)
        {
            this.colorPalettes = colorPalettes;

            // Manually remove , before }
            orgSentence = cfgSentence.Replace(",}", "}").Replace(colon, ":").
                                      Replace(arrayBracketOpen, "[").Replace(arrayBracketClose, "]").Replace(",]", "]");
            //Debug.Log(orgSentence);
            root = JSON.Parse(orgSentence);
        }

        public Item[] Animals()
        {
            return Items(animalsToken, animalSubtypeToken, animalAngleToken);
        }

        public Item[] Clouds()
        {
            return Items(cloudsToken, cloudSubtypeToken);
        }

        public Item[] Rocks()
        {
            return Items(rocksToken, rocksSubtypeToken);
        }

        public Item[] Plants()
        {
            return Items(plantsToken, plantsSubtypeToken);
        }

        public string ColorIndex()
        {
            return root[colorSchemeToken].Value;
        }

        public string ColorScheme()
        {
            return  colorPalettes[int.Parse(root[colorSchemeToken].Value)];
        }

        public string ColorHorizon()
        {
            return colorPalettes[int.Parse(root[colorSchemeToken].Value)].Split(colorDelimiter)[(int)ColorIndicies.HorizonColor];
        }

        public string ColorSky()
        {
            return colorPalettes[int.Parse(root[colorSchemeToken].Value)].Split(colorDelimiter)[(int)ColorIndicies.SkyColor];
        }
        public string GroundColor1()
        {
            return colorPalettes[int.Parse(root[colorSchemeToken].Value)].Split(colorDelimiter)[(int)ColorIndicies.GroundColor1];
        }

        public string GroundColor2()
        {
            return colorPalettes[int.Parse(root[colorSchemeToken].Value)].Split(colorDelimiter)[(int)ColorIndicies.GroundColor2];
        }

        public string GroundColor3()
        {
            return colorPalettes[int.Parse(root[colorSchemeToken].Value)].Split(colorDelimiter)[(int)ColorIndicies.GroundColor3];
        }

        public string[] GroundColors()
        {
            // MAGIC NUMBERS YAY YAY YAY
            string[] result = new string[3];
            result[0] = GroundColor1();
            result[1] = GroundColor2();
            result[2] = GroundColor3();
            return result;
        }

        public bool IsSkyGradient()
        {
            return root[skyGradientToken].Value == skyGradientTrue;
        }

        public float Metallic()
        {
            return float.Parse(root[metallicToken].Value);
        }

        public MusicTrack[] MusicTracks()
        {
            // Magic numbers make me orgasm oh oh oh yeah
            MusicTrack[] result = new MusicTrack[3];
            for (int i = 0; i < 3; i++)
            {
                MusicTrack track = new MusicTrack();
                var token = root[musicToken][i];
                track.track = int.Parse(token[0]);
                track.vol = float.Parse(token[1]);
                result[i] = track;
            }

            return result;
        }

        public SectionAngle[] SectionAngles()
        {
            int length = root[anglesToken].Children.Count();
            var result = new SectionAngle[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = new SectionAngle(float.Parse(root[anglesToken][i][anglePosXToken].Value),
                                        float.Parse(root[anglesToken][i][anglePosZToken].Value),
                                        float.Parse(root[anglesToken][i][angleAngleToken].Value));
            }

            return result;
        }

        public int SectionLength()
        {
            return int.Parse(root[sectionLengthToken].Value);
        }

        public float Smoothness()
        {
            return float.Parse(root[smoothnessToken].Value);
        }

        public override string ToString()
        {
            return orgSentence;
        }

        private Item[] Items(string itemTypeToken, string itemSubtypeToken, string angleToken = null)
        {
            int length = root[itemTypeToken].Children.Count();
            Item[] result = new Item[length];

            for (int i = 0; i < length; i++)
            {
                if (angleToken == null)
                {
                    result[i] = new Item(float.Parse(root[itemTypeToken][i][itemXPercentToken].Value),
                                         float.Parse(root[itemTypeToken][i][itemZPercentToken].Value),
                                         float.Parse(root[itemTypeToken][i][itemScaleToken].Value),
                                         int.Parse(root[itemTypeToken][i][itemSubtypeToken].Value));
                }
                else
                {
                    result[i] = new Item(float.Parse(root[itemTypeToken][i][itemXPercentToken].Value),
                                         float.Parse(root[itemTypeToken][i][itemZPercentToken].Value),
                                         float.Parse(root[itemTypeToken][i][itemScaleToken].Value),
                                         float.Parse(root[itemTypeToken][i][angleToken].Value),
                                         int.Parse(root[itemTypeToken][i][itemSubtypeToken].Value));
                }
            }

            return result;
        }

        public float BloomIntensity()
        {
            return float.Parse(root[pathGlowToken].Value);
        }

        public float BloomThreshold()
        {
            return float.Parse(root[pathThresholdToken].Value);
        }
    }

    public struct SectionAngle
    {
        public float pos_x, pos_z, angle;

        public SectionAngle(float pos_x, float pos_z, float angle)
        {
            this.pos_x = pos_x;
            this.pos_z = pos_z;
            this.angle = angle;
        }
    }
}
