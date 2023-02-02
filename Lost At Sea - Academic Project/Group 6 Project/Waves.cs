using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
namespace Group_6_Project
    //Author Michael Steffler
    //Date 3/20/22
{
    class Waves
    {
        private List<List<int>> waves;

        public Waves()
        {
            waves = new List<List<int>>();
            for (int i = 0; i < 200; i++)
            {
                List<int> n = new List<int>();
                n.Add(0);
                n.Add(0);
                n.Add(0);
                waves.Add(n);
            }
        }
        /// <summary>
        /// loads waves from file
        /// </summary>
        public void LoadWaves()
        {
            StreamReader input;
            
                input= new StreamReader("Content/wave.txt");
                for (int i = 0; i < 200; i++)
                {
                    string holder = input.ReadLine();
                    String[] data = holder.Split(",");
                    waves[i][0] = int.Parse(data[0]);
                    waves[i][1] = int.Parse(data[1]);
                    waves[i][2] = int.Parse(data[2]);
                }
                input.Close();
           

        }
        //returns wave at index
        public List<int> this[int index]
        {
            get { return waves[index]; }
        }
    }
}
