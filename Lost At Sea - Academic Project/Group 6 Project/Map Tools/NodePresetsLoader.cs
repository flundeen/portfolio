// ++++++
// Project: Lost at Sea
// Author: Narai Risser
// Description: Used to load information regarding nodes and their layouts
// ++++++

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Group_6_Project
{
    class NodePresetsLoader
    {
        // Each index is a list of presets of that direction openings
        private List<List<Texture2D[,]>> presets;

        private int presetSide;

        private Texture2D sea, grass, path;

        public NodePresetsLoader(int presetSideLength, Texture2D sea, Texture2D grass, Texture2D path)
        {
            int presetsCount = DebugFileWriter();


            presetSide = presetSideLength;
            presets = new List<List<Texture2D[,]>>(); // Todo -> change to be exact and not 2^5

            for (int i = 0; i < Math.Pow(2,5); i++)
            {
                presets.Add(new List<Texture2D[,]>());
            }

            this.sea = sea;
            this.grass = grass;
            this.path = path;

            // Go through each item on the file and read
            // the presets 


            StreamReader reader = new StreamReader("presets.txt");

            int count = Convert.ToInt32(reader.ReadLine());

            for (int i = 0; i < count; i++)
            {
                // Get directions which are stored with bool values 
                string[] directionsString = reader.ReadLine().Split();
                bool[] directions = new bool[]
                {
                    Convert.ToBoolean(directionsString[0]), // down 
                    Convert.ToBoolean(directionsString[1]), // up
                    Convert.ToBoolean(directionsString[2]), // left
                    Convert.ToBoolean(directionsString[3])  // right
                };

                // Gets the index based on the directions 
                int index = GetIndex(directions);

                // Get preset 
                // Adds to the list that holds all presets with 
                // this type of directional openings 
                presets[index].Add(ReadPreset(reader));
            }

            reader.Close();

        }

        /// <summary>
        /// Reads the necessary data for a single chunk and
        /// returns it back as a 2d array of textures 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Texture2D[,] ReadPreset(StreamReader reader)
        {
            Texture2D[,] chunks = new Texture2D[presetSide, presetSide];
            // Read the individual chunk values 

            // Get chunk data
            for (int y = 0; y < presetSide; y++)
            {
                string presetLine = reader.ReadLine();
                for (int x = 0; x < presetSide; x++)
                {
                    // Single chunk to add which is
                    // represented by 
                    char chunk = presetLine[x];
                    switch(chunk)
                    {
                        case 'G':
                        case 'g':
                            chunks[x, y] = grass;
                            break;
                        case 'P':
                        case 'p':
                            chunks[x, y] = path;
                            break;
                        case 'S':
                        case 's':
                            chunks[x, y] = sea;
                            break;
                        default:
                            //chunks[x, y] = sea;
                            break;
                    }
                }
            }

            return chunks;
        }

        /// <summary>
        /// Gets a array of possible texture sets for a node with different opening types 
        /// </summary>
        /// <param name="directions"></param>
        /// <returns></returns>
        public List<Texture2D[,]> GetPresets(bool[] directions)
        {
            int index = GetIndex(directions);

            return presets[index];  
        }

        /// <summary>
        /// Gets the index of the node based on its directions 
        /// </summary>
        /// <param name="directions"></param>
        /// <returns></returns>
        private int GetIndex(bool[] directions)
        {
            // Gets the index based on the directions 
            int index = 0;
            for (int j = 0; j < directions.Length; j++)
            {
                if (directions[j])
                {
                    index += (int)Math.Pow(2, j);
                }
            }

            return index;
        }

        /// <summary>
        /// Sets up the default tiles that can be used for debugging purposes
        /// before the external tool is set up 
        /// </summary>
        private int DebugFileWriter() 
        {
            // Though not as efficient, allows for easier debugging when looking
            // at the file with a streamwriter rather than binary 
            StreamWriter writer = new StreamWriter("presets.txt");

            List<string[]> defaultPresets = new List<string[]>();
            List<bool[]> presetDirections = new List<bool[]>();
            // [0] down
            // [1] up 
            // [2] left
            // [3] right 

            presetDirections.Add(new bool[] { false, false, false, false });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { false, false, false, true });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGPPPP",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { false, false, true, false });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "PPPPGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { false, false, true, true });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "PPPPPPP",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { false, true, false, false });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });
            
            presetDirections.Add(new bool[] { false, true, false, true });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPPPP",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { false, true, true, false });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "PPPPGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { false, true, true, true });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "PPPPPPP",
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG"
            });

            presetDirections.Add(new bool[] { true, false, false, false });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, false, false, true });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "GGGPPPP",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, false, true, false });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "PPPPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, false, true, true });
            defaultPresets.Add(new string[]
            {
                "GGGGGGG",
                "GGGGGGG",
                "GGGGGGG",
                "PPPPPPP",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, true, false, false });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, true, false, true });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPPPP",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, true, true, false });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "PPPPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });

            presetDirections.Add(new bool[] { true, true, true, true });
            defaultPresets.Add(new string[]
            {
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG",
                "PPPPPPP",
                "GGGPGGG",
                "GGGPGGG",
                "GGGPGGG"
            });
           
            // Writes the info to the map 
            writer.WriteLine(defaultPresets.Count);
            for (int i = 0; i < defaultPresets.Count; i++)
            {
                // Writes the directional openings for current preset 
                for (int j = 0; j < presetDirections[i].Length; j++)
                {
                    writer.Write(presetDirections[i][j].ToString() + " ");
                }
                writer.WriteLine();
                // Writes out the entire preset line by line 
                for (int k = 0; k < defaultPresets[i].Length; k++)
                {
                    writer.WriteLine(defaultPresets[i][k]);
                }
            }

            writer.Close();
            return defaultPresets.Count;
        }

    }
}