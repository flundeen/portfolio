using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Group_6_Project
{
    class Menu
    {
        //feilds
        private Texture2D screen;
        private SpriteFont font;
        private string title;
        private int width;
        private int height;
        private Button button;
        private Button button2;

        /// <summary>
        /// Button being used
        /// </summary>
        public Button ButtonUsed
        {
            get { return button; }
            set { button = value; }
        }
        /// <summary>
        /// texture of screen((used in tutorial
        /// </summary>
        public Texture2D Screen
        {
            get { return screen; }
            set { screen = value; }
        }
        /// <summary>
        /// Secound button used?
        /// </summary>
        public Button ButtonUsed2
        {
            get { return button2; }
            set { button2 = value; }
        }
        /// <summary>
        /// single button draw, in a menu
        /// </summary>
        public Menu(Texture2D screen, SpriteFont font, string title, int width, int height, Button button)
        {
            this.screen = screen;
            this.font = font;
            this.title = title;
            this.width = width;
            this.height = height;
            this.button = button;
        }
        /// <summary>
        /// Double button draw, in a menu
        /// </summary>
        public Menu(Texture2D screen, SpriteFont font, string title, int width, int height, Button button, Button button2)
        {
            this.screen = screen;
            this.font = font;
            this.title = title;
            this.width = width;
            this.height = height;
            this.button = button;
            this.button2 = button2;
        }


        /// <summary>
        /// Draws the menu's for the main and the pause menues
        /// </summary>
        /// <param name="sb"></param>
        public void MenuDraw(SpriteBatch sb)
        {
            sb.Draw(screen,new Rectangle(0,0, width, height), Color.White);
            sb.DrawString(font, title, new Vector2(350, 100), Color.Black);
            button.ButtonDraw(sb);
        }
        /// <summary>
        /// Draws menu with two buttons
        /// </summary>
        /// <param name="sb"></param>
        public void MenuDrawForTwo(SpriteBatch sb)
        {
            sb.Draw(screen, new Rectangle(0, 0, width, height), Color.White);
            sb.DrawString(font, title, new Vector2(350, 100), Color.White);
            button.ButtonDraw(sb);
            button2.ButtonDraw(sb);
        }
    }
}
