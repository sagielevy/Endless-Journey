using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public readonly static string[] INTRO_TEXTS = new string[] { "Endless Journey",
                                                                     "Use WASD and the Cursor to move around",
                                                                     "Press Space to jump and hold Left Shift to run",
                                                                     "Press F to change focus and E to take a photo" };
    }
}
