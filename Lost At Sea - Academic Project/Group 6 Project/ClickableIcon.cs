using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    class ClickableIcon : GameObjectClickable
    {
        //Fields
        Texture2D image;

        //Constructor
        public ClickableIcon(Rectangle rect, Texture2D img)
        {
            this.rect = rect;
            this.image = img;
        }

        /// <summary>
        /// Draws the icon's image at it's specific bounds 
        /// </summary>
        /// <param name="sb"></param>
        public void DrawIcon(SpriteBatch sb)
        {
            sb.Draw(image, rect, Color.White);
        }
    }
}
