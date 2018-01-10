using System;
using System.Collections.Generic;
using System.Text;

namespace CFGGeneratorCoreDll.Generator
{
    /// <summary>
    /// Generate sentences and enqueue them in an async queue by request
    /// </summary>
    public class AsyncCFGGenerator
    {
        private const int SentencesCapacity = 100; // Make this configurable?

        // How many sentences to generate to randomly select from for each round
        private const int NumOfSentencesPerRun = 20;
        
        // TODO Make singleton?
        // TODO Use some async queue data structure and generate sentences into it.
        // TODO THIS IS VERY IMPORTANT! Think of how to implement some callback that will generate a new sentence once 
        //      a sentence has been removed from the queue by the user of this generator. Also, make sure the inital generation
        //      fills up the queue to the limit of the capacity constant.

        public void EnqueueNewSentence()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate a new sentence based on given CFG
        /// </summary>
        /// <returns></returns>
        private string GenerateNewSentence()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluate weight based on how similar both sentences are.
        /// The closer they are, the larger the weight of the current sentence
        /// </summary>
        /// <param name="original"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private Sentence EvaluateWeightForSentence(string original, string current)
        {
            throw new NotImplementedException();
        }

        private string RandomlySelectNextSentence(Sentence[] sentences)
        {
            throw new NotImplementedException();
        }
    }

    class Sentence
    {
        string SentenceData;
        float Weight;
    }
}
