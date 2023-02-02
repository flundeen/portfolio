using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    abstract class GameObjectClickable
    {
        public Rectangle rect;
        public Rectangle Rect
        {
            get { return rect; }
        }

        /// <summary>
        /// can check if the mouse is over a game object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Intersects(MouseState ms)
        {
            if (ms.X >= Rect.X && ms.X <= Rect.X + Rect.Width)
            {
                if (ms.Y >= Rect.Y && ms.Y <= Rect.Y + Rect.Height)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// checks if an object is clicked this update
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="prev">previous mousestate</param>
        /// <returns></returns>
        public bool isClicked(MouseState ms, MouseState prev)
        {
            if (prev.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed)
            {

                if(Intersects(ms))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
