using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    class MusicModifier : IWorldModifier//<IMusicData>
    {
        GameObject musicAudioSources;

        public MusicModifier(GameObject musicAudioSources)
        {
            this.musicAudioSources = musicAudioSources;
        }

        public void ModifySection(ISentenceData data)
        {
            var musicData = data as IMusicData;
            throw new NotImplementedException();
        }
    }
}
