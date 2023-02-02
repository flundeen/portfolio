using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Group_6_Project
{
    class Button : GameObjectClickable
    {
        //feilds
        private Texture2D screen;
        private SpriteFont font;
        private string buttonText;
        private Vector2 positionOfText;

        /// <summary>
        /// is button visible
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// The constructor of the button
        /// </summary>
        /// <param name="screen">Texture of button</param>
        /// <param name="font">Font of button</param>
        /// <param name="buttonText">The text of button</param>
        /// <param name="positionOfText"></param>
        /// <param name="rect">The bounds of button</param>
        public Button(Texture2D screen,SpriteFont font, string buttonText,Vector2 positionOfText, Rectangle rect)
        {
            this.screen = screen;
            this.font = font;
            this.buttonText = buttonText;
            this.rect = rect;
            this.positionOfText = positionOfText;
            IsVisible = false;
        }

        /// <summary>
        /// Draws the buttons
        /// </summary>
        /// <param name="sb"></param>
        public void ButtonDraw(SpriteBatch sb)
        {
            //temp
            IsVisible = true;
            sb.Draw(screen, rect, Color.DarkCyan); //all buttons are blue for now
            sb.DrawString(font, buttonText, positionOfText, Color.White);
            
        }
    }
}
