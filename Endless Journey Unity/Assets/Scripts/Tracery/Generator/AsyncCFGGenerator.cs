using Assets.Scripts.CFGParser;
using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Scripts.Tracery.Generator
{
    /// <summary>
    /// Generate sentences and enqueue them in an async queue by request
    /// </summary>
    public class AsyncCFGGenerator
    {
        private const int LIST_CAP = 30;

        // How many sentences to generate to randomly select from for each round
        private const int NUM_OF_SENTENCES_PER_RUN = 20;

        private TraceryHandler handler;
        private MyList<SentenceDataHolder> list;
        string[] colorPalettes;

        public AsyncCFGGenerator(string cfgPath, string[] colorPalettes)
        {
            this.colorPalettes = colorPalettes;
            handler = new TraceryHandler();
            handler.LoadCFG(cfgPath);
            list = new MyList<SentenceDataHolder>();

            // Generate the initial sentences.
            for (int i = 0; i < LIST_CAP; i++)
            {
                EnqueueNewSentence();
            }
        }

        public SentenceDataHolder GetSentenceDebug()
        {
            return new SentenceDataHolder(handler.GenerateSentence(), colorPalettes);
        }

        public SentenceDataHolder GetSentence()
        {
            Thread t = new Thread(EnqueueNewSentence);
            t.Start();
            return list.Dequeue();
        }

        private void EnqueueNewSentence()
        {
            SentenceDataHolder lastSentence = list.Last;
            // Debug.Log("Last sentence colors: " + ((lastSentence != null) ? (lastSentence.ColorScheme()) : ("NULL")));
            Sentence[] sentences = new Sentence[NUM_OF_SENTENCES_PER_RUN];
            for (int i = 0; i < sentences.Length; i++)
            {
                SentenceDataHolder sentenceData = GenerateNewSentence();
                float weight = CalcDistance(lastSentence, sentenceData);
                Sentence s = new Sentence();
                s.Weight = weight;
                s.SentenceData = sentenceData;
                // Debug.Log("Color: " + sentenceData.ColorScheme() + " Weight: " + weight.ToString());
                sentences[i] = s;
            }

            // Sort the sentences by weight
            Array.Sort(sentences);

            // The first element should have the minimal distance
            //list.Enqueue(sentences[0].SentenceData);
            list.Enqueue(WeightedRandomization.Choose(sentences).SentenceData);
        }

        /// <summary>
        /// Generate a new sentence based on given CFG
        /// </summary>
        /// <returns></returns>
        private SentenceDataHolder GenerateNewSentence()
        {
            string strSentence = handler.GenerateSentence();
            return new SentenceDataHolder(strSentence, colorPalettes);
        }

        private float CalcDistance(SentenceDataHolder orig, SentenceDataHolder toCompare)
        {
            //return 5;

            if (orig == null)
            {
                // Default
                return 1;
            }

            var origColors = GetColors(orig);
            var compareColors = GetColors(toCompare);

            // Calc the differences
            float distance = 0;

            for (int i = 0; i < origColors.Length; i++)
            {
                // Add distance between each color in the palette, but modify distance by color weight
                distance += Mathf.Abs(origColors[i] - compareColors[i]);

                // Adds square so that the distance will not linearly affect weight
                //distance += Mathf.Sqrt(Mathf.Abs(origColors[i] - compareColors[i]));
            }

            return distance;
        }

        private float[] GetColors(SentenceDataHolder dataHolder)
        {
            try
            {
                return dataHolder.ColorScheme().Split(' ').Select(color => Helpers.FromText(color).Sum()).ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError("Colors: " + dataHolder.ColorScheme());
                Debug.LogError("Color Index: " + dataHolder.ColorIndex());
                return null;
            }
        }
    }

    public class Sentence : IComparable
    {
        public SentenceDataHolder SentenceData;
        public float Weight;

        public int CompareTo(object obj)
        {
            Sentence s = (Sentence)obj;
            float distance = Weight - s.Weight;

            if(distance > 0)
            {
                return 1;
            }
            else if(distance < 0)
            {
                return -1;
            }
            return 0;
        }
    }

    static class WeightedRandomization
    {
        //public static T Choose<T>(List<T> list) where T : Sentence
        public static T Choose<T>(T[] list) where T : Sentence
        {
            if (list.Length == 0)
            {
                return default(T);
            }

            float totalweight = list.Sum(c => c.Weight);
            float choice = (float)new System.Random().NextDouble() * totalweight;
            float sum = 0;

            foreach (var obj in list)
            {
                var range = choice - sum;

                // 0 <= choice - sum < obj.Weight
                if (0 <= range && range < obj.Weight)
                {
                    return obj;
                }

                sum += obj.Weight;
            }

            return list.First();
        }
    }
}
