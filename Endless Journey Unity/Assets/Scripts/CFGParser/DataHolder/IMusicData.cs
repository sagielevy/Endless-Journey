using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public struct MusicTrack
    {
        public int track;
        public float vol;
    }

    public interface IMusicData : ISentenceData
    {
        MusicTrack[] MusicTracks();
    }
}
