using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    class Enemy
    {
        //------------Fields------------
        //Stats
        private int hitpoints; //Amount of health enemy has
        private int speed; //How fast enemy is
        private int damage; //How much life is lost when enemy gets through
        private int reward; //How much money is gained
        private bool pause;
        private Vector2 move;
        private Point target;
        private int pathIndex;
        private Point prevTarget;
        //Location & Hitbox
        private Rectangle bounds;

        //texture & animation
        private Texture2D texture;
        private int currentFrame;
        private double fps;
        private double secondsPerFrame;
        private double timeCounter;
        private int frameCount;
        private GameTime gameTime;
        private int widthOfSingleSprite;
        private int heightOffset;
        

        private List<Point> path;

        //------------Properties----------

        //Stats
        public int Hp
        {
            get { return hitpoints; }
            set
            {
                if( hitpoints < 0)
                {
                    hitpoints = 0;
                }
                else
                {
                    hitpoints =  value;
                }
            }
        }

        public int Sp
        {
            get { return speed; }
        }

        public int Dmg
        {
            get { return damage; }
        }

        public int Gp
        {
            get { return reward; }
        }
        public bool Pause
        {
            get { return pause; }
            set { pause = value; }
        }


        //Location and HitBox
        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Vector2 Center
        {
            get { return new Vector2(bounds.X + bounds.Width/2, bounds.Y + bounds.Height/2); }
            set { bounds.X = (int)value.X + bounds.Width / 2; bounds.Y = (int)value.Y + bounds.Height / 2; }

        }
        /// <summary>
        /// returns true if enemy is dead
        /// </summary>
        public bool IsDead
        {
            get 
            { 
                if (Hp<=0)
                {
                    return true;
                }
                return false;
            }
            
        }

        public List<Point> Path
        {
            get { return path; }
        }
        public Point Target { get { return target; } set { target = value; } }
        public Point PrevTarget { get { return prevTarget; } set { prevTarget = value; } }
        public int PathIndex { get { return pathIndex;  } set { pathIndex = value;} }
        public int Speed { get { return speed; } }

        //--------------Constructor----------------------------
        public Enemy(int hitpoints, int speed, int damage, int reward, Rectangle bounds, Texture2D texture, int frameCount, int heightOffset, List<Point> path)
        {
            this.hitpoints = hitpoints;
            this.speed = speed;
            this.damage = damage;
            this.reward = reward;
            this.bounds = bounds;
            this.texture = texture;
            pause = false;
            this.path = path;
            pathIndex = 0;
            move = new Vector2(bounds.X, bounds.Y);
            prevTarget = default;

            currentFrame = 1;
            fps = 10.0;
            secondsPerFrame = 1.0f / fps;
            timeCounter = 0;
            this.frameCount = frameCount;
            gameTime = new GameTime();

            this.heightOffset = heightOffset;

            widthOfSingleSprite = texture.Width / frameCount;
        }
      
        //---------------Methods-------------------------------

        /// <summary>
        /// Moves the enemy towards to Target Location at a specific speed. If the speed goes past 
        /// the target point. The enemy's location will instead be set to that point. The method will
        /// then return true if the enemy reaches the point, else it will return false
        /// </summary>
        /// <param name="targLoc">desired result location of enemy</param>
        /// <returns>whether enemy reached location or not</returns>
        public bool MoveToPoint(Vector2 targLoc)
        {
            if(prevTarget==default)
            {
                prevTarget = target;
            }
            if(pause)
            {
                return false;
            }


            Vector2 difference = this.bounds.Center.ToVector2() - targLoc;
            difference.Normalize();
            //for direction
            if(prevTarget.X>target.X)
            {
                if ((difference * speed).X < difference.X)
                {
                    move.X -= difference.X;
                }
                else
                {
                    move.X -= (difference * speed).X;
                }
            }
            else
            {
                if ((difference * speed).X > difference.X)
                {
                    move.X -= difference.X;
                }
                else
                {
                    move.X -= (difference * speed).X;
                }
            }

            if (prevTarget.Y > target.Y)
            {
                if ((difference * speed).Y < difference.Y)
                {
                    move.Y -= difference.Y;
                }
                else
                {
                    move.Y -= (difference * speed).Y;
                }
            }
            else
            {
                if ((difference * speed).Y > difference.Y)
                {
                    move.Y -= difference.Y;
                }
                else
                {
                    move.Y -= (difference * speed).Y;
                }
            }



            //  move.Y -= (difference * speed).Y;

            bounds.X = (int)move.X;
            bounds.Y = (int)move.Y;
            

            if (targLoc == Center)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// draws enemy
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public void Draw(SpriteBatch spriteBatch, int xOffset, int yOffset)
        {
            if(Hp>0)
            {
                Rectangle temp = bounds;
                temp.X += xOffset;
                temp.Y += yOffset;

                int xDifference = target.X - bounds.Center.X;
                if (target.X - bounds.Center.X < 0)
                {
                    spriteBatch.Draw(texture, temp, Color.White);

                }
                else
                {
                    spriteBatch.Draw(texture, temp, null, Color.White, 0, Vector2.One, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }
        /// <summary>
        /// draws enemy with a tint
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="tint"></param>
        public void Draw(SpriteBatch spriteBatch, int xOffset, int yOffset,Color tint)
        {
            if (Hp > 0)
            {
                Rectangle temp = bounds;
                temp.X += xOffset;
                temp.Y += yOffset;

                int xDifference = target.X - bounds.Center.X;
                if (target.X - bounds.Center.X < 0)
                {
                    spriteBatch.Draw(texture, temp, tint);

                }
                else
                {
                    spriteBatch.Draw(texture, temp, null, tint, 0, Vector2.One, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }

        

        /// <summary>
		/// Updates the animation time
		/// </summary>
		/// <param name="gameTime">Game time information</param>
		private void UpdateAnimation()
        {
            
            // Add to the time counter (need TOTALSECONDS here)
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // Has enough time gone by to actually flip frames?
            if (timeCounter >= secondsPerFrame)
            {
                // Update the frame and wrap
                currentFrame++;
                if (currentFrame >= frameCount) currentFrame = 1;

                // Remove one "frame" worth of time
                timeCounter -= secondsPerFrame;
            }

        }

    }
}
