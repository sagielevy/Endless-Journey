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
        
        float startTime;
        Dictionary<string, float> originalVolumes;

        public MusicModifier(GameObject musicAudioSources)
        {
            originalVolumes = new Dictionary<string, float>();
            this.musicAudioSources = musicAudioSources;
            this.startTime = Time.time;
            AudioSource[] sources = musicAudioSources.GetComponentsInChildren<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                originalVolumes[sources[i].name] = sources[i].volume;
            }
        }

        public void ModifySection(ISentenceData data)
        {
            var musicData = data as IMusicData;
            MusicTrack[] tracks = musicData.MusicTracks();
            AudioSource[] sources = musicAudioSources.GetComponentsInChildren<AudioSource>();

            for (int i = 0; i < sources.Length; i++)
            {
                // TODO: Using tracks length in such a fasion assumes that the number of 'a' tracks is the same number
                // of 'b' tracks and the same number of 'percussion' tracks etc. If we want a different size array for each
                // type of track this code needs to be changed accordingly.
                int trackIndex = i / tracks.Length;
                float vol = 0;
                int innerIndex = i % tracks.Length;

                if(innerIndex  == tracks[trackIndex].track)
                {
                    // Debug.Log("I: " + i.ToString() + " track index: " + trackIndex.ToString() + " Inner index: " + innerIndex.ToString());
                    vol = tracks[trackIndex].vol;
                }
                
                var newVolume = Mathf.Lerp(originalVolumes[sources[i].name], vol, Globals.speedChange * 2 * (Time.time - startTime));
                sources[i].volume = newVolume;
            }
        }
    }
}
