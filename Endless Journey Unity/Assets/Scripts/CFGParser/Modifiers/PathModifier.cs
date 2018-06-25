using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class PathModifier : IWorldModifier//<IPathData>
    {
        PostProcessingProfile myProfile;
        float orgIntensity, orgThreshold, startTime;

        public PathModifier(PostProcessingProfile myProfile)
        {
            this.myProfile = myProfile;
            orgIntensity = myProfile.bloom.settings.bloom.intensity;
            orgThreshold = myProfile.bloom.settings.bloom.threshold;
            startTime = Time.time;
        }

        public IEnumerator<WaitForEndOfFrame> ModifySection(ISentenceData data)
        {
            var pathData = data as IPathData;

            // Keep runing till replaced by new enumerator
            while (true)
            {
                BloomModel.Settings effectSettings = myProfile.bloom.settings;

                // Lerp changes
                effectSettings.bloom.intensity = Mathf.Lerp(orgIntensity, pathData.BloomIntensity(), Globals.speedChange * (Time.time - startTime));
                effectSettings.bloom.threshold = Mathf.Lerp(orgThreshold, pathData.BloomThreshold(), Globals.speedChange * (Time.time - startTime));

                myProfile.bloom.settings = effectSettings;

                yield return Globals.EndOfFrame;
            }
        }
    }
}
