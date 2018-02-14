using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public static class Globals
    {
        // Can turn 60 degrees to each side
        public static readonly float theta = 50;
        public static readonly float speedChange = 0.08f;
        public static readonly float maxHeight = 35f; // TODO this is a guess!
        public static readonly float cloudHeight = 30f; // TODO this is a bad solution. Change Y dynamically
        public static readonly float birdHeight = 17f; // TODO this is a bad solution. Change Y dynamically
        public static readonly float groundSeperateMul = 1.0f;
        public static readonly float cloudSeperateMul = 1.0f;
        public static readonly float animalSeperateMul = 1.0f;
        public static readonly Quaternion defaultRotation = Quaternion.Euler(-90, 0, 0);
        public readonly static string[] INTRO_TEXTS = new string[] { "Endless Journey\n\nAll content in this world is procedurally genrated.\nPress F1 for control info.\n\n\nPress G to skip",
                                                                      "Credits\n\nDevelopers: Sagie Levy & Tom Feigin\nMusic: Ori Angel\nModels: Oded Kofian\n\nPress G to skip"};
    }
}
