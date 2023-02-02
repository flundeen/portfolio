using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    //Tower selection enums
    public enum TowerSelect
    {
        basic,
        sniper,
        mine,
        mortar,
        beam, 
        tesla
    }
    class GameplayUI
    {
        SpriteFont font;
        public Village village;

        public int waveCount;

        //ui background
        Texture2D towerBackground;
        Texture2D textBackground;

        //basic tower stuff
        Texture2D tower1Texture;
        ClickableIcon tower1Icon;

        //sniper tower stuff
        Texture2D sniperTexture;
        ClickableIcon sniperIcon;

        //mine tower stuff
        Texture2D mineTexture;
        ClickableIcon mineIcon;

        //mortar tower stuff
        Texture2D mortarTexture;
        ClickableIcon mortarIcon;

        //beam tower stuff
        Texture2D beamTexture;
        ClickableIcon beamIcon;

        //tesla tower stuff
        Texture2D teslaTexture;
        ClickableIcon teslaIcon;

        MouseState ms;
        MouseState msPrev;

        private TowerSelect selectedTower;

        //tracks which tower the player has selected to be built
        public TowerSelect SelectedTower
        {
            get { return selectedTower; }
        }

        //tracks how much money the 
        public int Money
        {
            get { return village.Money; }
            set { village.Money = value; }
        }

        //constructor
        public GameplayUI(SpriteFont font, Village village, Texture2D TBack, Texture2D TextBack, Texture2D T1img, Texture2D T2img, Texture2D mineImg, Texture2D mortarImg, Texture2D beamImg, Texture2D teslaImg)
        {
            this.font = font;
            this.village = village;

            towerBackground = TBack;
            textBackground = TextBack;

            tower1Texture = T1img;
            tower1Icon = new ClickableIcon(new Rectangle(0, 65, 50, 60), tower1Texture);

            sniperTexture = T2img;
            sniperIcon = new ClickableIcon(new Rectangle(50, 65, 50, 60), sniperTexture);

            mineTexture = mineImg;
            mineIcon = new ClickableIcon(new Rectangle(0, 135, 50, 60), mineTexture);

            mortarTexture = mortarImg;
            mortarIcon = new ClickableIcon(new Rectangle(50, 135, 50, 55), mortarTexture);

            beamTexture = beamImg;
            beamIcon = new ClickableIcon(new Rectangle(0, 205, 50, 55), beamTexture);

            teslaTexture = teslaImg;
            teslaIcon = new ClickableIcon(new Rectangle(50, 205, 50, 60), teslaTexture);

            selectedTower = TowerSelect.basic;

            ms = Mouse.GetState();
            msPrev = ms;
        }

        /// <summary>
        /// draws all of the icons and info for the player to have in their "hud" while playing the game
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="wave"></param>
        public void DrawComponents(SpriteBatch sb, int wave)
        {
            //draws ui background
            sb.Draw(towerBackground, new Rectangle(0, 75, 100, 200), Color.White);
            sb.Draw(textBackground, new Rectangle(0, 0, 150, 75), Color.White);

            //draw stats and other info
            sb.DrawString(font, "Money: " + village.Money + "$", new Vector2(5, 0), Color.White);
            sb.DrawString(font, "Health: " + village.Health, new Vector2(5, 17), Color.White);
            sb.DrawString(font, "Selected: " + selectedTower, new Vector2(5, 52), Color.White);
            sb.DrawString(font, "Wave: " + wave,new Vector2(5,35),Color.White);

            //draws tower buttons
            tower1Icon.DrawIcon(sb);
            sniperIcon.DrawIcon(sb);
            mineIcon.DrawIcon(sb);
            mortarIcon.DrawIcon(sb);
            beamIcon.DrawIcon(sb);
            teslaIcon.DrawIcon(sb);

            //draws tower costs
            sb.DrawString(font, "50$", new Vector2(5, 115), Color.White);
            sb.DrawString(font, "100$", new Vector2(55, 115), Color.White);
            sb.DrawString(font, "75$", new Vector2(5, 185), Color.White);
            sb.DrawString(font, "200$", new Vector2(55, 185), Color.White);
            sb.DrawString(font, "175$", new Vector2(5, 255), Color.White);
            sb.DrawString(font, "250$", new Vector2(55, 255), Color.White);

        }

        /// <summary>
        /// Tracks if the player has changed their selected tower to build through either mouseclick or  
        /// </summary>
        public void UpdateUI()
        {
            //mouse interaction
            ms = Mouse.GetState();
            if (tower1Icon.Intersects(ms))
            {
                if (tower1Icon.isClicked(ms,msPrev))
                {
                    selectedTower = TowerSelect.basic;
                }
            }
            if (sniperIcon.Intersects(ms))
            {
                if (sniperIcon.isClicked(ms, msPrev))
                {
                    selectedTower = TowerSelect.sniper;
                }
            }
            if (mineIcon.Intersects(ms))
            {
                if(mineIcon.isClicked(ms, msPrev))
                {
                    selectedTower = TowerSelect.mine;
                }
            }
            if (mortarIcon.Intersects(ms))
            {
                if(mortarIcon.isClicked(ms, msPrev))
                {
                    selectedTower = TowerSelect.mortar;
                }
            }
            if (beamIcon.Intersects(ms))
            {
                if (beamIcon.isClicked(ms, msPrev))
                {
                    selectedTower = TowerSelect.beam;
                }
            }
            if (teslaIcon.Intersects(ms))
            {
                if (teslaIcon.isClicked(ms, msPrev))
                {
                    selectedTower = TowerSelect.tesla;
                }
            }

            //keybinds for selecting towers

            KeyboardState key = Keyboard.GetState();

            if (key.IsKeyDown(Keys.Z))
            {
                selectedTower = TowerSelect.basic;
            }
            else if (key.IsKeyDown(Keys.X))
            {
                selectedTower = TowerSelect.sniper;
            }
            else if (key.IsKeyDown(Keys.C))
            {
                selectedTower = TowerSelect.mine;
            }
            else if (key.IsKeyDown(Keys.V))
            {
                selectedTower = TowerSelect.mortar;
            }
            else if (key.IsKeyDown(Keys.B))
            {
                selectedTower = TowerSelect.beam;

            }
            else if (key.IsKeyDown(Keys.N))
            {
                selectedTower = TowerSelect.tesla;
            }
            //key used for god mode
            else if (key.IsKeyDown(Keys.G))
            {
                village.Health = 5000;
                village.Money = 5000;

            }



            msPrev = ms;
        }
    }
}
