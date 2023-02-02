using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Group_6_Project
{
    class Projectile
    {
        private Point start;
        
        private Texture2D texture;
        private Rectangle rect;
        private bool isVisible;
        private int speed;
        private Enemy target;
        //properties
        public Point Start
        {
            get { return start; }
            set { start = value; }
        }
       
        public Vector2 Center
        {
            get { return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }

        }
        public Enemy Target
        {
            get { return target; }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
        //constructor
        public Projectile(Point start,  Texture2D texture, int speed, Enemy target)
        {
            this.start = start;
            
            this.texture = texture;
            //adjust to change size
            rect = new Rectangle(start.X - 5, start.Y - 5, 10, 10);
            this.speed = speed;
            isVisible = true;
            this.target = target;
        }
        /// <summary>
        /// moves object from one point to another
        /// </summary>
        /// <param name="targLoc"></param>
        /// <returns></returns>
        public bool MoveToPoint(Vector2 targLoc)
        {
            float y = targLoc.Y - Center.Y;
            float x = targLoc.X - Center.X;
            Vector2 vec = new Vector2(x, y);
            vec.Normalize();

            rect.X += (int)(vec.X * speed);
            rect.Y += ((int)(vec.Y * speed));
            if (targLoc == Center)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// moves bullet to one enemy
        /// </summary>
        public bool Move()
        {
            if(!MoveToPoint(target.Center))
            {
                MoveToPoint(target.Center);
                return false;
            }
            else
            {
                isVisible = false;
                return true;
            }
            
        }
        /// <summary>
        /// draws enemy
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb, int xoffset, int yOffset)
        {
            if(isVisible)
            {
                Rectangle temp = rect;
                temp.X += xoffset;
                temp.Y += yOffset;
                sb.Draw(texture, temp, Color.White);
            }
            
        }
    }
}
