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
            list.Enqueue(sentences[0].SentenceData);
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
            if(orig == null)
            {
                return 0;
            }
            int[] origColors = GetColors(orig);
            int[] compareColors = GetColors(toCompare);

            // Calc the differences
            float distance = 0;

            for (int i = 0; i < origColors.Length; i++)
            {
                distance += Math.Abs(origColors[i] - compareColors[i]);
            }

            return distance;
        }

        private int[] GetColors(SentenceDataHolder dataHolder)
        {
            try
            {
                return dataHolder.ColorScheme().Split(' ').Select(color => int.Parse(color, NumberStyles.HexNumber)).ToArray();
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

    public struct Sentence : IComparable
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
}
