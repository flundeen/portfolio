//Freddie Douglas
//2/25/2022
// The player class for the main village maker


using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    class Village
    {
        //feilds

        //The varibles for the top fo screen
        private int money;
        private float health;
        private int waveNumber;

        //for drawing village
        private Vector2 startPos;

        //for hit boxes with village
        private Rectangle villageBounds;
        private Texture2D villageTex;
        private Rectangle bounds;
        //properties

        /// <summary>
        /// money of the player
        /// </summary>
        public int Money
        {
            get { return money; }
            set { money = value; }
        }
        /// <summary>
        /// Health of player
        /// <summary>
        public float Health
        {
            get { return health; }
            set { health = value; }
        }
        /// <summary>
        /// Wave number of village
        /// </summary>
        public int WaveNumber
        {
            get { return waveNumber; }
            set { waveNumber = value; }
        }
        /// <summary>
        /// The bounds of the village
        /// </summary>
        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        //constructor
        public Village(int money, float health, int waveNumber, Vector2 positon, Rectangle bounds, Texture2D villageTexture)
        {
            this.money = money;
            this.health = health;
            this.waveNumber = waveNumber;
            this.startPos = positon;
            this.villageBounds = bounds;
            this.villageTex = villageTexture;
        }
        //Methods

        /// <summary>
        /// The gold for the village generation
        /// </summary>
        /// <param name="numberOfBank">Number of bank towers</param>
        public void waveGold(int numberOfBank)
        {
            double goldPerWave = 100;
            goldPerWave += (numberOfBank * 25);
            goldPerWave += goldPerWave*(waveNumber * .75);
            Money += (int)goldPerWave;
        }

        /// <summary>
        /// The village takes damage
        /// </summary>
        /// <param name="enemyHit"> the enemy that made it to village</param>
        public void takeDamage(Enemy enemyHit)
        {
            
                health -= enemyHit.Dmg;
                enemyHit.Hp = 0;

        }

        /// <summary>
        /// Village draw method
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch,int xOffSet, int yOffSet)
        {
            if (Health > 0)
            {
                bounds = villageBounds;
                bounds.X += xOffSet;
                bounds.Y += yOffSet;
                
                spriteBatch.Draw(villageTex, bounds, Color.White);
            }
        }
    }
}
