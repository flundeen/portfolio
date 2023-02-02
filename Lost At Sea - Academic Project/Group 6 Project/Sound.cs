using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Group_6_Project
{
    public static class Sound
    {
        /// <summary>
        /// The sounds are universal
        /// </summary>
        private static List<SoundEffect> soundEffects;
        private static List<Song> songs;

        /// <summary>
        /// The list of sound effects
        /// </summary>
        public static List<SoundEffect> SoundEffects
        {
            get { return soundEffects; }
            set { soundEffects = value; }
        }
        /// <summary>
        /// list of songs
        /// </summary>
        public static List<Song> Songs
        {
            get { return songs; }
            set { songs = value; }
        }
    }
}
