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
        public static readonly float speedChange = 0.01f;
        public static readonly float maxHeight = 100f; // TODO this is a guess!
        public static readonly float cloudHeight = 30f; // TODO this is a bad solution
        public static readonly float birdHeight = 17f; // TODO this is a bad solution
        public static readonly float groundSeperateMul = 1.5f;
        public static readonly float cloudSeperateMul = 2.1f;
        public static readonly float animalSeperateMul = 1.1f;
    }
}
