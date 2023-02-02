using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;
namespace Group_6_Project
{
    public class Game1 : Game
    {
        //main items
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Waves waves;

        StateManager sm;

        //map textures
        private Texture2D pathTexture;
        private Texture2D seaTexture;
        private Texture2D grassTexture;
        private Texture2D basicVillage;

        //Enemy Textures
        private Texture2D basicEnemyTex;
        private Texture2D tankTexture;
        private Texture2D fastTexture;
        private Texture2D bossTexture;

        //Tower Textures
        private Texture2D basicTowerTex;
        private Texture2D bowTowerProjectile;

        private Texture2D basicProjectileTex;
        private Texture2D sniperTowerTex;
        private Texture2D mineTowerTex;
        private Texture2D mortarTowerTex;
        private Texture2D beamTowerTex;
        private Texture2D teslaTowerTex;

        //UI Background and Fonts
        private Texture2D buttonBackground;
        private SpriteFont titleScreenFont;
        private SpriteFont papyBig; 
        private Texture2D towerUIBackground;
        private Texture2D textUIBackground;
        private Texture2D titleScreen;
        private List<Texture2D> fullTutorial;

        //map width and full view box width
        private int mapWidth;
        private int mapHeight;
        private int widthFullMap;
        private int heightFullMap;

        //buttons and menus
        private Button buttonStart;
        private Button buttonPauseLeave;
        private Button buttonPauseEnter;
        private Button buttonDead;
        private Button buttonTutorial;
        private Button buttonNext;
        private Button buttonReturnToMenu;
        private Menu menu;
        private Menu menuPause;
        private Menu menuDead;
        private Menu menuWin;
        private Menu menuTutorial;


        private Map map;

        private GameState currentState;
        private List<Point> path;

        private GameplayUI gameplayUI;

        //mouse state and main objects
        private MouseState ms;
        private MouseState previousMS;
        private Village powderedEvents;

        //SFX
        private List<SoundEffect> soundEffects;
        private List<Song> songs;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //audio
            soundEffects = new List<SoundEffect>();
            songs = new List<Song>();
            fullTutorial = new List<Texture2D>();
            
        }

        protected override void Initialize()
        {
            mapWidth =7;
            mapHeight = 7;
            currentState = GameState.Menu;

            _graphics.IsFullScreen = true;
            //_graphics.PreferredBackBufferWidth = 1000;
            //_graphics.PreferredBackBufferHeight = 500;
            _graphics.ApplyChanges();
         
            base.Initialize();
            widthFullMap = _graphics.PreferredBackBufferWidth;
            heightFullMap = _graphics.PreferredBackBufferHeight;

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //textures
            pathTexture = Content.Load<Texture2D>("Path_Tile");
            seaTexture = Content.Load<Texture2D>("Sea_Tile");
            grassTexture = Content.Load<Texture2D>("Grass_Tile");
            titleScreen = Content.Load<Texture2D>("Menu_Background");
            titleScreenFont = Content.Load<SpriteFont>("Oswald");
            fullTutorial.Add(Content.Load<Texture2D>("Tutorial page 0"));
            fullTutorial.Add(Content.Load<Texture2D>("Tutorial page 1"));
            fullTutorial.Add(Content.Load<Texture2D>("tutorial page 2"));
            fullTutorial.Add(Content.Load<Texture2D>("Turtorial Page 3 "));
            papyBig = Content.Load<SpriteFont>("PapyBig");
            buttonBackground = Content.Load<Texture2D>("Button_Background");

            //Enemy Textures
            basicEnemyTex = Content.Load<Texture2D>("Basic_Fish_Enemy");
            tankTexture = Content.Load<Texture2D>("Tank image");
            fastTexture = Content.Load<Texture2D>("crabTexture");
            bossTexture = Content.Load<Texture2D>("Boss_Enemy");

            //Tower Textures
            basicTowerTex = Content.Load<Texture2D>("Basic_Bow_Tower");
            bowTowerProjectile = Content.Load<Texture2D>("Basic_Tower_Arrow");
            sniperTowerTex = Content.Load<Texture2D>("TowerUnique");
            basicProjectileTex = Content.Load<Texture2D>("BasicProjectile");
            mineTowerTex = Content.Load<Texture2D>("Mine_tower");
            mortarTowerTex = Content.Load<Texture2D>("Mortar_tower");
            beamTowerTex = Content.Load<Texture2D>("Eye_Beam");
            teslaTowerTex = Content.Load<Texture2D>("Tesla_Tower_Tex");


            basicVillage = Content.Load<Texture2D>("Village_v2");
            towerUIBackground = Content.Load<Texture2D>("GameplayUI background");
            textUIBackground = Content.Load<Texture2D>("GameplayUI text");


            //sounds
            soundEffects.Add(Content.Load<SoundEffect>("Enemy death1"));
            soundEffects.Add(Content.Load<SoundEffect>("LandRise1"));
            soundEffects.Add(Content.Load<SoundEffect>("enemyHit"));
            soundEffects.Add(Content.Load<SoundEffect>("BossMusic"));
            songs.Add(Content.Load<Song>("MainMusic"));

            Sound.SoundEffects = soundEffects;
            Sound.Songs = songs;


            //this makes a new map
            map = new Map(mapWidth, mapHeight, seaTexture, grassTexture, pathTexture);

            //buttons and menus
            buttonStart = new Button(buttonBackground, titleScreenFont, "Press to Start", new Vector2(350, 250), new Rectangle(150, 210, 500, 100));
            buttonPauseLeave = new Button(buttonBackground, titleScreenFont, "Press to Go back", new Vector2(350, 250), new Rectangle(150, 210, 500, 100));
            buttonDead = new Button(buttonBackground, titleScreenFont, "Quit Game", new Vector2(350, 250), new Rectangle(150, 210, 500, 100));
            buttonPauseEnter = new Button(buttonBackground, titleScreenFont, "Pause",new Vector2(700,50), new Rectangle(675, 50, 100, 25));
            buttonTutorial = new Button(buttonBackground, titleScreenFont, "How to play?", new Vector2(350, 380), new Rectangle(300, 370, 200, 50));
            buttonNext = new Button(buttonBackground, titleScreenFont, "Next page", new Vector2(710, 455), new Rectangle(700,450, 200, 50));
            buttonReturnToMenu = new Button(buttonBackground, titleScreenFont, "Return to Title Screen", new Vector2(600, 0), new Rectangle(575, 0, 200, 25));
            menu = new Menu(titleScreen, titleScreenFont, "", 800, 600, buttonStart, buttonTutorial);
            menuPause = new Menu(pathTexture, titleScreenFont, "Pause Menu\nLost At Sea", 850, 750, buttonPauseLeave);
            menuDead = new Menu(grassTexture, papyBig, "Welp you died", 800, 750, buttonDead);
            menuTutorial = new Menu(fullTutorial[0], titleScreenFont, "", 800, 480, buttonReturnToMenu, buttonNext);
            menuWin = new Menu(seaTexture, titleScreenFont, "You built your empire and WON!", 800, 750, buttonDead);


            //village
            powderedEvents = new Village(500, 20, 4, new Vector2(0, 0), new Rectangle(map.GetNodePos(map.CenterNode).X, map.GetNodePos(map.CenterNode).Y, 52, 52), basicVillage);

            //Gameplay UI - must be after Village instantiation 
            gameplayUI = new GameplayUI(titleScreenFont, powderedEvents, towerUIBackground, textUIBackground, basicTowerTex, sniperTowerTex, mineTowerTex, mortarTowerTex, beamTowerTex, teslaTowerTex);


            sm = new StateManager(this,map, menu, menuWin, menuTutorial, menuPause, menuDead, basicProjectileTex, bowTowerProjectile, basicTowerTex, sniperTowerTex, mineTowerTex, mortarTowerTex, beamTowerTex, teslaTowerTex, buttonPauseEnter, basicEnemyTex, powderedEvents, gameplayUI, tankTexture,fastTexture,bossTexture, papyBig, fullTutorial);

            //wave enemy and tower loader
            waves = new Waves();
            waves.LoadWaves();
            
            
            path = new List<Point>();
            //media player
            MediaPlayer.Volume -= 0.5f;
            MediaPlayer.Play(Sound.Songs[0]);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //mousestate previous and current
            if(ms == null)
            {
                ms = Mouse.GetState();
            }
            else
            {
                previousMS = ms;
                ms = Mouse.GetState();
            }

            KeyboardState kb = Keyboard.GetState();
           
            // Offsets the window based on key input 
            sm.PlayGame(ms, kb, gameTime, previousMS);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);

            sm.RenderGame(currentState, _spriteBatch, ms);

            
            
            _spriteBatch.End();

            base.Draw(gameTime);
            ShapeBatch.End();
        }

        /// <summary>
        /// quits game
        /// </summary>
        public void Quit()
        {
            this.Exit();
        }
        
        
    }
}
