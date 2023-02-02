using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Group_6_Project
{
    // Tells the game what to being rendering overal 
    public enum GameState
    {
        Menu,
        Game,
        End,
        PauseMenu,
        Win,
        Tutorial
    }

    // How the player interacts with the game while it is playing 
    public enum PlayState
    {
        Expand,
        Build,
        Fight,
        Boss
    }


    class StateManager
    {
        private Map map;
        private List<Point> mainPath;

        private int screenMoveSpeed;
        private int xOffset;
        private int yOffset;

        private Point centerOffset;

        private ButtonState pastState;
        

        private PlayState state;
        private GameState mainGame;

        // Each chunk either refers to an index or null
        // Used to check if places are occupied or to get tower at location 
        private TowerNode[,] mapToTower;

        private List<Tower> towers; // Where to hold?
        private List<Tower>[] towersByRow;
        //waves
        private Waves waves;
        

        //tower textures
        private Texture2D tempBulletTex;
        private Texture2D bowTowerProjectile;
        private Texture2D tempTowerTex;
        private Texture2D sniperTowerTex;
        private Texture2D mineTowerTex;
        private Texture2D mortarTowerTex;
        private Texture2D beamTowerTex;
        private Texture2D teslaTowerTex;

        //Enemy textures
        private Texture2D enemyTex;
        private Texture2D tankTex;
        private Texture2D fastTex;
        private Texture2D bossTex;

        //Menu code here
        private Menu menu;
        private Menu pauseMenu;
        private Menu menuDead;
        private Menu menuWin;
        private Menu menuTutorial;
        private Button pauseEnter;
        private Village powderedEvents;
        private GameplayUI gameplayUI;
        
        private List<Texture2D> fullTutorial;

        private Node farthestNode;
        private List<Node> minorWaveOpenings;

        private List<Enemy> enemyList;

        private int subEnemiesAdded;

        private SpriteFont timerFont;

        //timer fields
        private GameTime time;
        private double endBuild;

        //bank tracking
        private int totalMines;
        
        //waves and balance
        private int waveCount;
        private double spawnDelay;
        private double delayTime;
        private int difMultiplier;
        private int enemiesAdded;
        private int enemiesTankAdded;
        private int enemiesFAdded;

        private int bossMaxHealth;

        private List<Node> activeNodes;
        private Dictionary<Node,List<Point>> subpaths;
        private Game1 game;

        private Tower basic;
        private Tower sniper;
        private Tower mine;
        private Tower mortar;
        private Tower beam;
        private Tower tesla;

        private int prevWave;

        Random rng;
        private const int MINWAVESBEFOREBOSS = 15;

        public StateManager(Game1 game, Map map, Menu menu, Menu menuWin, Menu menuTutorial, Menu pauseMenu, Menu menuDead, Texture2D projectileTex, Texture2D bowTowerProjectile, Texture2D towerTex, Texture2D sniperTowerTex, Texture2D mineTowerTex, Texture2D mortarTowerTex, Texture2D beamTowerTex, Texture2D teslaTowerTex, Button pauseEnter, Texture2D enemyTex, Village powderedEvents, GameplayUI gameplayUI, Texture2D tankTex, Texture2D fastTex, Texture2D bossTex, SpriteFont timerFont, List<Texture2D> fullTurotial)
        {
            this.map = map;
            this.game = game;
            mapToTower = new TowerNode[map.Width * 7, map.Height * 7];
            towers = new List<Tower>();
            towersByRow = new List<Tower>[map.Height];
            for (int i = 0; i < map.Height; i++)
            {
                towersByRow[i] = new List<Tower>();
            }

            mainPath = new List<Point>();
            //needed for reset
            farthestNode = map.CenterNode;
            minorWaveOpenings = new List<Node>();

            screenMoveSpeed = 5;

            xOffset = -map.GetNodePos(map.CenterNode).X + (int)(map.TileSize * 3.5);
            yOffset = -map.GetNodePos(map.CenterNode).Y + (int)(map.TileSize * 2);
            centerOffset = new Point(xOffset, yOffset);
            //timer
            time = new GameTime();
            endBuild = 0.0;

            //mine ttracking
            totalMines = 0;

            //menu code here
            this.menu = menu;
            this.pauseMenu = pauseMenu;
            this.pauseEnter = pauseEnter;
            this.menuDead = menuDead;
            this.menuWin = menuWin;
            this.menuTutorial = menuTutorial;
            this.powderedEvents = powderedEvents;
            this.timerFont = timerFont;

            this.fullTutorial = fullTurotial;

            this.gameplayUI = gameplayUI;

            //starting state
            this.mainGame = GameState.Menu;

            //Tower Textures
            this.tempBulletTex = projectileTex;
            this.bowTowerProjectile = bowTowerProjectile;
            this.tempTowerTex = towerTex;
            this.sniperTowerTex = sniperTowerTex;
            this.mineTowerTex = mineTowerTex;
            this.mortarTowerTex = mortarTowerTex;
            this.beamTowerTex = beamTowerTex;
            this.teslaTowerTex = teslaTowerTex;

            //Enemy Textures
            this.enemyTex = enemyTex;
            this.tankTex = tankTex;
            this.fastTex = fastTex;
            this.bossTex = bossTex;
            

            Rectangle rect1 = new Rectangle(200, 200, 100, 100);
            Rectangle rect2 = new Rectangle(50, 200, 100, 100);
            Rectangle rect3 = new Rectangle(-50, 200, 100, 100);
            Rectangle rect4 = new Rectangle(500, 200, 100, 100);
            Rectangle rect5 = new Rectangle(200, 200, 100, 100);

            enemyList = new List<Enemy>();
            
            //waves and balance
            waveCount = 0;
            enemiesAdded = 0;
            subEnemiesAdded = 0;
            enemiesTankAdded = 0;
            enemiesFAdded = 0;
            spawnDelay = 0;
            delayTime = .5;
            difMultiplier = 1;
            waves = new Waves();
            waves.LoadWaves();
            activeNodes = new List<Node>();
            subpaths = new Dictionary<Node, List<Point>>();

            rng = new Random();

            bossMaxHealth = 2500;
            prevWave = 0;

            ///Tower STATS
            ///Name: Basic
            ///Damage: 2
            ///Range: 100
            ///Attack Speed: 15
            ///Cost: 50
            basic = new Tower(2, 100, tempTowerTex, new Rectangle(), this.bowTowerProjectile, 15, new Point(2, 2), 50);
            ///Tower STATS
            ///Name: Sniper
            ///Damage: 10
            ///Range: 200
            ///Attack Speed: 50
            ///Cost: 100
            sniper = new Tower(10, 200, sniperTowerTex, new Rectangle(), tempBulletTex, 50, new Point(2, 2), 100);
            ///Tower STATS
            ///Name: Mine
            ///Damage: 0
            ///Range: 0
            ///Attack Speed: 0
            ///Cost: 75
            mine = new Tower(0, 0, mineTowerTex, new Rectangle(), tempBulletTex, 0, new Point(3, 3), 75);
            ///Tower STATS
            ///Name: Mortar
            ///Damage: 15
            ///Range: 200-400
            ///Attack Speed: 150
            ///Cost: 350
            mortar = new Tower(14, 400, mortarTowerTex, new Rectangle(), tempBulletTex, 150, new Point(2, 2), 350, 200);
            ///Tower STATS
            ///Name: Beam
            ///Damage: 1
            ///Range: 50
            ///Attack Speed: 1
            ///Cost: 175
            beam = new Tower(1, 50, beamTowerTex, new Rectangle(), tempBulletTex, 1, new Point(2, 2), 175);
            ///Tower STATS
            ///Name: Tesla
            ///Damage: 6
            ///Range: 125
            ///Attack Speed: 25
            ///Cost: 225
            tesla = new Tower(6, 125, teslaTowerTex, new Rectangle(), tempBulletTex, 50, new Point(2, 2), 250, true);
            tesla.Tesla = true;

            //needed for reset
            // Adds center node 
            AddTile(map.NodeGrid[map.NodeGrid.GetLength(0) / 2, map.NodeGrid.GetLength(1) / 2]);

            // For some reason just adding the node through the addtile function does not activate the chunks can someone else take a look please (I am tired) - Narai 
            ActivateChunks(map.CenterNode);
        }

        /// <summary>
        /// Renders the seperate parts of the game. If in renderstate Game different menu may
        /// be rendered over the necessary world objects 
        /// </summary>
        /// <param name="render"></param>
        /// <param name="sb"></param>
        /// <param name="state"></param>
        public void RenderGame(GameState render, SpriteBatch sb, MouseState ms)
        {

            switch (mainGame)
            {
                case GameState.Menu:
                    //drawn menu
                    menu.MenuDrawForTwo(sb);
                    break;
                case GameState.Tutorial:
                    //drawn tutorial menu
                    menuTutorial.MenuDrawForTwo(sb);
                    break;
                case GameState.Game:

                    // Necessary game rendering
                    map.VisualizeMaze(sb);

                    powderedEvents.Draw(sb, xOffset + (map.TileSize/4), yOffset + (map.TileSize/4));
                    //draws towers
                    for (int i = towers.Count - 1; i >= 0; i--)
                    {
                        towers[i].draw(sb);

                    }
                    //draws enemies
                    foreach (Enemy item in enemyList)
                    {
                        if(item.Speed<3)
                        {
                            item.Draw(sb, xOffset, yOffset);
                        }
                        else
                        {
                            item.Draw(sb, xOffset, yOffset, Color.Red);
                        }
                    }
                   
                    foreach (Point point in mainPath)
                    {
                        //sb.Draw(tempTowerTex, new Rectangle(point.X * map.TileSize + xOffset, point.Y * map.TileSize + yOffset, 50, 50), Color.Red);
                    }
                    foreach (Node minorOpening in minorWaveOpenings)
                    {
                        map.DrawNode(sb, minorOpening, Color.Red);
                    }

                    gameplayUI.DrawComponents(sb,waveCount+1);
                    pauseEnter.ButtonDraw(sb);

                    


                    // The states of the actual played game
                    // Is there anything different you would want to render 
                    // between each of these states? 
                    Node node = null;
                    switch (state) 
                    {
                        case PlayState.Expand:
                            //Get mouse node hover 
                            node = map.MouseToNode(ms.Position);

                            if (node != null)
                            {

                                if (node.InPlay)
                                {
                                    break;
                                }

                                foreach (Node neighbor in map.GetNeighbors(node))
                                {
                                    // If connected and in play
                                    if ((node & neighbor) && neighbor.InPlay)
                                    {
                                        ShapeBatch.BoxOutline(new Rectangle(xOffset + node.point.X * map.TileSize, yOffset + node.point.Y * map.TileSize, map.TileSize, map.TileSize), Color.Red);
                                    }
                                }
                            }
                            break;
                        case PlayState.Build:

                            sb.DrawString(timerFont, ((int)(11 - endBuild)).ToString(), ms.Position.ToVector2() + new Vector2(0, 10), Color.Black, 0, Vector2.One, 1, SpriteEffects.None, 0);

                            Point placeIndex = map.GetChunkPos(ms.Position);
                            switch (gameplayUI.SelectedTower)
                            {
                                case TowerSelect.basic:
                                    if((gameplayUI.Money - basic.Cost) >= 0)
                                    {
                                        ShapeBatch.Circle(ms.Position.ToVector2(), basic.Range, new Color(Color.Black, 0.3f));
                                        sb.Draw(tempTowerTex, new Rectangle(ms.Position - new Point(map.ChunkSize / 2, map.ChunkSize / 2), (basic.Size.ToVector2() * map.ChunkSize).ToPoint()), Color.White);
                                    }
                                        
                                    break;
                                case TowerSelect.sniper:

                                    if ((gameplayUI.Money - sniper.Cost) >= 0)
                                    {
                                        ShapeBatch.Circle(ms.Position.ToVector2(), sniper.Range, new Color(Color.Black, 0.3f));
                                        sb.Draw(sniperTowerTex, new Rectangle(ms.Position - new Point(map.ChunkSize / 2, map.ChunkSize / 2), (/* Change here! -> */sniper.Size.ToVector2() * map.ChunkSize).ToPoint()), Color.White);
                                    }
                                    break;

                                case TowerSelect.mine:

                                    if ((gameplayUI.Money - mine.Cost) >= 0)
                                    {
                                        ShapeBatch.Circle(ms.Position.ToVector2(), mine.Range, new Color(Color.Black, 0.3f));
                                        sb.Draw(mineTowerTex, new Rectangle(ms.Position - new Point(map.ChunkSize / 2, map.ChunkSize / 2), (/* Change here! -> */mine.Size.ToVector2() * map.ChunkSize).ToPoint()), Color.White);
                                    }



                                    break;
                                case TowerSelect.mortar:

                                    if ((gameplayUI.Money - mortar.Cost) >= 0)
                                    {
                                        ShapeBatch.Circle(ms.Position.ToVector2(), mortar.Range, new Color(Color.Black, 0.3f));
                                        sb.Draw(mortarTowerTex, new Rectangle(ms.Position - new Point(map.ChunkSize / 2, map.ChunkSize / 2), (/* Change here! -> */mortar.Size.ToVector2() * map.ChunkSize).ToPoint()), Color.White);
                                    }



                                    break;

                                case TowerSelect.beam:

                                    if ((gameplayUI.Money - beam.Cost) >= 0)
                                    {
                                        ShapeBatch.Circle(ms.Position.ToVector2(), beam.Range, new Color(Color.Black, 0.3f));
                                        sb.Draw(beamTowerTex, new Rectangle(ms.Position - new Point(map.ChunkSize / 2, map.ChunkSize / 2), (/* Change here! -> */beam.Size.ToVector2() * map.ChunkSize).ToPoint()), Color.White);
                                    }



                                    break;

                                case TowerSelect.tesla:

                                    if ((gameplayUI.Money - tesla.Cost) >= 0)
                                    {
                                        ShapeBatch.Circle(ms.Position.ToVector2(), tesla.Range, new Color(Color.Black, 0.3f));
                                        sb.Draw(teslaTowerTex, new Rectangle(ms.Position - new Point(map.ChunkSize / 2, map.ChunkSize / 2), (/* Change here! -> */tesla.Size.ToVector2() * map.ChunkSize).ToPoint()), Color.White);
                                    }



                                    break;
                            }



                            // Tower placement
                            Point index = map.GetChunkPos(ms.Position);
                            node = map.MouseToNode(ms.Position);
                            
                            // Makes sure node actually exists in the game 
                            if (node != null)
                            {
                                if (node.InPlay && TowerCanBePlaced(index, basic))
                                {
                                    Point point = new Point((ms.Position.X), (ms.Position.Y));
                                    Rectangle rect;

                                    switch (gameplayUI.SelectedTower)
                                    {
                                        case TowerSelect.basic:

                                            rect = new Rectangle(
                                                new Point(
                                                    point.X - (point.X % map.ChunkSize),
                                                    point.Y - (point.Y % map.ChunkSize)
                                                    ),
                                                new Point(map.ChunkSize * basic.Size.X, map.ChunkSize * basic.Size.Y)// chunk size needs to be taken into account for rect 
                                                );

                                            if (TowerCanBePlaced(index, basic) && gameplayUI.Money - basic.Cost >= 0)
                                            {
                                                ShapeBatch.BoxOutline(rect, Color.Red);
                                            }
                                            break;
                                        case TowerSelect.sniper:

                                            rect = new Rectangle(
                                                new Point(
                                                    point.X - (point.X % map.ChunkSize),
                                                    point.Y - (point.Y % map.ChunkSize)
                                                    ),
                                                new Point(map.ChunkSize * sniper.Size.X, map.ChunkSize * sniper.Size.Y)// chunk size needs to be taken into account for rect 
                                                );

                                            if (TowerCanBePlaced(index, sniper) && gameplayUI.Money - sniper.Cost >= 0)
                                            {
                                                ShapeBatch.BoxOutline(rect, Color.Red);
                                            }
                                            break;
                                        case TowerSelect.mine:

                                            rect = new Rectangle(
                                                new Point(
                                                    point.X - (point.X % map.ChunkSize),
                                                    point.Y - (point.Y % map.ChunkSize)
                                                    ),
                                                new Point(map.ChunkSize * mine.Size.X, map.ChunkSize * mine.Size.Y)// chunk size needs to be taken into account for rect 
                                                );

                                            if (TowerCanBePlaced(index, mine) && gameplayUI.Money - mine.Cost >= 0)
                                            {
                                                ShapeBatch.BoxOutline(rect, Color.Red);
                                            }
                                            break;
                                        case TowerSelect.mortar:

                                            rect = new Rectangle(
                                                new Point(
                                                    point.X - (point.X % map.ChunkSize),
                                                    point.Y - (point.Y % map.ChunkSize)
                                                    ),
                                                new Point(map.ChunkSize * mortar.Size.X, map.ChunkSize * mortar.Size.Y)// chunk size needs to be taken into account for rect 
                                                );

                                            if (TowerCanBePlaced(index, mortar) && gameplayUI.Money - mortar.Cost >= 0)
                                            {
                                                ShapeBatch.BoxOutline(rect, Color.Red);
                                            }
                                            break;

                                        case TowerSelect.beam:

                                            rect = new Rectangle(
                                                new Point(
                                                    point.X - (point.X % map.ChunkSize),
                                                    point.Y - (point.Y % map.ChunkSize)
                                                    ),
                                                new Point(map.ChunkSize * beam.Size.X, map.ChunkSize * beam.Size.Y)// chunk size needs to be taken into account for rect 
                                                );

                                            if (TowerCanBePlaced(index, beam) && gameplayUI.Money - beam.Cost >= 0)
                                            {
                                                ShapeBatch.BoxOutline(rect, Color.Red);
                                            }
                                            break;

                                        case TowerSelect.tesla:

                                            rect = new Rectangle(
                                                new Point(
                                                    point.X - (point.X % map.ChunkSize),
                                                    point.Y - (point.Y % map.ChunkSize)
                                                    ),
                                                new Point(map.ChunkSize * tesla.Size.X, map.ChunkSize * tesla.Size.Y)// chunk size needs to be taken into account for rect 
                                                );

                                            if (TowerCanBePlaced(index, tesla) && gameplayUI.Money - tesla.Cost >= 0)
                                            {
                                                ShapeBatch.BoxOutline(rect, Color.Red);
                                            }
                                            break;
                                    }

                                }
                            }


                            break;
                        case PlayState.Fight:
                            break;
                        case PlayState.Boss:
                            // If the boss is in game 
                            if(enemyList.Count >0)
                            {
                                ShapeBatch.Box(new Rectangle(200, 400, ((int)((float)enemyList[0].Hp / (float)bossMaxHealth * 500f)), 20), Color.Red);
                            }
                            break;
                    }
                    break;
                case GameState.PauseMenu:
                    pauseMenu.MenuDraw(sb);
                    break;

                case GameState.End:
                    menuDead.MenuDraw(sb);
                    break;

                case GameState.Win:
                    menuWin.MenuDraw(sb);
                    break;
            }
        }

        public void PlayGame(MouseState ms, KeyboardState kb, GameTime t, MouseState previousMS)
        {
            
            // Able to offset game in any mode 
            OffsetWindow(kb);
            time = t;

            //Updates GameplayUI
            gameplayUI.UpdateUI();

            

            switch (mainGame)
            { 
                //main menu
                case GameState.Menu:
                    if (menu.ButtonUsed.isClicked(ms, previousMS) == true)
                    {
                        mainGame = GameState.Game;
                    }
                    if (menu.ButtonUsed2.isClicked(ms, previousMS) == true)
                    {
                        mainGame = GameState.Tutorial;
                    }
                    break;
                //tutorial
                case GameState.Tutorial:
                    if (menuTutorial.ButtonUsed.isClicked(ms, previousMS) == true)
                    {
                        mainGame = GameState.Menu;
                    }
                    if (menuTutorial.ButtonUsed2.isClicked(ms, previousMS) == true)
                    {
                        for (int i = 0; i < fullTutorial.Count; i++)
                        {
                            
                            if(i == fullTutorial.Count - 1)
                            {
                                menuTutorial.Screen = fullTutorial[0];
                                break;
                            }
                            if (menuTutorial.Screen == fullTutorial[i])
                            {
                                menuTutorial.Screen = fullTutorial[i + 1];
                                break;
                            }
                        }
                    }
                    break;
                //pause menu
                case GameState.PauseMenu:
                    if (pauseMenu.ButtonUsed.isClicked(ms, previousMS) == true)
                    {
                        mainGame = GameState.Game;
                        Pause(false);
                    }
                    break;
                //the game
                case GameState.Game:
                    if (pauseEnter.isClicked(ms, previousMS) == true)
                        {
                        mainGame = GameState.PauseMenu;
                        Pause(true);
                        }
                    switch (state)
                    {

                        case PlayState.Expand:
                            

                            // Turns node on or off
                            if (ms.LeftButton == ButtonState.Pressed && pastState == ButtonState.Released)
                            {
                                Node node = map.MouseToNode(ms.Position);

                                // Makes sure node actually exists in the game 
                                if (node != null)
                                {
                                    if (node.InPlay)
                                    {
                                        break;
                                    }


                                    // Brings a new tile into the game 
                                    if (AddTile(node))
                                    {
                                        // If adding the tile was successful

                                        // Check if new node is new farthest from center 
                                        if (node.Equals(map.CenterNode))
                                        {
                                            farthestNode = node;
                                        }
                                        else
                                        {
                                            List<Point> possiblePath = map.GetPath(node.point, map.CenterNode.point);

                                            // Repetive code but will crash if fining path from center to center 
                                            bool lengthCompareGreater = map.GetPath(farthestNode.point, map.CenterNode.point).Count <= possiblePath.Count;
                                            if (lengthCompareGreater)
                                            {
                                                farthestNode = node;
                                                mainPath = possiblePath;
                                            }
                                           
                                        }
                                        Sound.SoundEffects[1].Play();
                                        state = PlayState.Build;
                                    }
                                }
                            }

                            

                            break;
                        case PlayState.Build:

                            //you win!
                            /*if (activeNodes.Count == 24)
                            {
                                mainGame = GameState.Win;
                            }*/

                            


                            // Turns node on or off
                            if (ms.LeftButton == ButtonState.Pressed && pastState == ButtonState.Released)
                            {
                                Node node = map.MouseToNode(ms.Position);

                                // Makes sure node actually exists in the game 
                                if (node != null)
                                {
                                    if (node.InPlay)
                                    {
                                        //Checks if selected tower is basic tower and the player has enough money to place it
                                        if (gameplayUI.SelectedTower == TowerSelect.basic && gameplayUI.Money >= basic.Cost)
                                        {
                                            int towerCount = towers.Count;
                                           
                                            AddTower(ms, basic);

                                            //checks if the tower was successfully added
                                            if (towerCount < towers.Count)
                                            {
                                                //if it was subtract the tower cost from money
                                                gameplayUI.Money -= basic.Cost;
                                            }
                                        }
                                        //Checks if selected tower is unique tower and the player has enough money to place it
                                        if (gameplayUI.SelectedTower == TowerSelect.sniper && gameplayUI.Money >= sniper.Cost)
                                        {
                                            int towerCount = towers.Count;
                                           
                                            AddTower(ms, sniper);
                                            //checks if the tower was successfully added
                                            if(towerCount < towers.Count)
                                            {
                                                //if it was, subtract the tower cost from money
                                                gameplayUI.Money -= sniper.Cost;
                                            }
                                            
                                        }
                                        if (gameplayUI.SelectedTower == TowerSelect.mine && gameplayUI.Money >= mine.Cost)
                                        {
                                            int towerCount = towers.Count;

                                            AddTower(ms, mine);
                                            totalMines++;
                                            //checks if the tower was successfully added
                                            if (towerCount < towers.Count)
                                            {
                                                //if it was subtract the tower cost from money
                                                gameplayUI.Money -= mine.Cost;
                                            }
                                        }
                                        if (gameplayUI.SelectedTower == TowerSelect.mortar && gameplayUI.Money >= mortar.Cost)
                                        {
                                            int towerCount = towers.Count;
                                            ///Tower STATS
                                            ///Name: Mortar
                                            ///Damage: 8
                                            ///Range: 300
                                            ///Attack Speed: 40
                                            ///Cost: 250
                                            AddTower(ms, mortar);
                                            //checks if the tower was successfully added
                                            if (towerCount < towers.Count)
                                            {
                                                //if it was subtract the tower cost from money
                                                gameplayUI.Money -= mortar.Cost;
                                            }
                                        }
                                        if (gameplayUI.SelectedTower == TowerSelect.beam && gameplayUI.Money >= beam.Cost)
                                        {
                                            int towerCount = towers.Count;
                                            ///Tower STATS
                                            ///Name: Beam
                                            ///Damage: 1
                                            ///Range: 50
                                            ///Attack Speed: 1
                                            ///Cost: 175
                                            AddTower(ms, beam);
                                            //checks if the tower was successfully added
                                            if (towerCount < towers.Count)
                                            {
                                                //if it was subtract the tower cost from money
                                                gameplayUI.Money -= beam.Cost;
                                            }
                                        }
                                        if (gameplayUI.SelectedTower == TowerSelect.tesla && gameplayUI.Money >= tesla.Cost)
                                        {
                                            int towerCount = towers.Count;

                                            AddTower(ms, tesla);
                                            //checks if the tower was successfully added
                                            if (towerCount < towers.Count)
                                            {
                                                //if it was subtract the tower cost from money
                                                gameplayUI.Money -= tesla.Cost;
                                            }
                                        }
                                    }
                                }
                            }

                            endBuild += t.ElapsedGameTime.TotalSeconds;


                            if (endBuild >= 10)
                            {
                                endBuild = 0;
                                subpaths.Clear();

                                // As aproaching 
                                double max = map.Width * map.Height;
                                double current = (waveCount) - MINWAVESBEFOREBOSS; // y = mx + b

                                if (current > 0)
                                {
                                    // If the value is less than the current chance of winning then start boss 
                                    if(current >= 0)
                                    {
                                        double rand = rng.NextDouble();
                                        
                                        // Checks with percent of map completed 
                                        if (rand <= (current / (max - MINWAVESBEFOREBOSS)))
                                        {
                                            state = PlayState.Boss;
                                        }
                                        else
                                        {
                                            state = PlayState.Fight;
                                        }
                                    }
                                    else
                                    {
                                        state = PlayState.Fight;
                                    }
                                }
                                else
                                {
                                    state = PlayState.Fight;
                                }

                            }
                            break;

                        case PlayState.Fight:

                            //subwave work
                            foreach (Node item in activeNodes)
                            {
                                List<Node> neighbors = map.GetNeighbors(item,false);
                                foreach (Node neighbor in neighbors)
                                {
                                    if (neighbor & item && neighbor.InPlay != true)
                                    {
                                        if(!subpaths.ContainsKey(item))
                                        {
                                            subpaths.Add(item, map.GetPath(item.point, map.CenterNode.point));
                                        }
                                        
                                    }
                                }
                            }
                            
                            // new Rectangle(point.X * map.TileSize + xOffset, point.Y * map.TileSize + yOffset, 50, 50)w

                            // Pathfind on space press
                            // path = map.GetPath(map.NodeGrid[0, 0].point, map.NodeGrid[2, 2].point);
                            if (spawnDelay > delayTime)
                            {
                                if (enemiesAdded < waves[waveCount][0])
                                {
                                    enemyList.Add(new Enemy(10*difMultiplier, 2, 1, 20, new Rectangle((map.GetNodePos(farthestNode).X + map.TileSize / 4) - xOffset, (map.GetNodePos(farthestNode).Y + map.TileSize / 4) - yOffset, 50, 50), enemyTex,1, 0, mainPath));

                                    // Vector2 rectOffset = new Vector2(enemyList[enemyList.Count - 1].Bounds.Width / 2, enemyList[enemyList.Count - 1].Bounds.Height / 2);
                                    //enemyList[enemyList.Count - 1].Center -= rectOffset;
                                    
                                    enemiesAdded++;
                                    
                                }
                                if (enemiesTankAdded < waves[waveCount][1])
                                {
                                    enemyList.Add(new Enemy(30 * difMultiplier, 1, 2, 30, new Rectangle((map.GetNodePos(farthestNode).X + map.TileSize / 4) - xOffset, (map.GetNodePos(farthestNode).Y + map.TileSize / 4) - yOffset, 50, 50), tankTex, 1, 0,  mainPath));

                                    // Vector2 rectOffset = new Vector2(enemyList[enemyList.Count - 1].Bounds.Width / 2, enemyList[enemyList.Count - 1].Bounds.Height / 2);
                                    //enemyList[enemyList.Count - 1].Center -= rectOffset;

                                    enemiesTankAdded++;

                                }
                                if (enemiesFAdded < waves[waveCount][2])
                                {
                                    enemyList.Add(new Enemy(15 * difMultiplier, 3, 2, 25, new Rectangle((map.GetNodePos(farthestNode).X + map.TileSize / 4) - xOffset, (map.GetNodePos(farthestNode).Y + map.TileSize / 4) - yOffset, 50, 50), fastTex, 4, 50, mainPath));

                                    // Vector2 rectOffset = new Vector2(enemyList[enemyList.Count - 1].Bounds.Width / 2, enemyList[enemyList.Count - 1].Bounds.Height / 2);
                                    //enemyList[enemyList.Count - 1].Center -= rectOffset;

                                    enemiesFAdded++;

                                }
                                //makes subwaves
                                foreach (Node item in subpaths.Keys)
                                {
                                    if(waveCount<15)
                                    {
                                        if (subEnemiesAdded < waveCount * 2*subpaths.Keys.Count)
                                        {
                                            enemyList.Add(new Enemy(10 * difMultiplier, 2, 1, 20, new Rectangle((map.GetNodePos(item).X + map.TileSize / 4) - xOffset, (map.GetNodePos(item).Y + map.TileSize / 4) - yOffset, 50, 50), enemyTex, 1, 0, subpaths[item]));
                                            subEnemiesAdded++;
                                        }
                                    }
                                    else if(waveCount<20)
                                    {
                                        if (subEnemiesAdded < waveCount * 2 * subpaths.Keys.Count)
                                        {
                                            enemyList.Add(new Enemy(30 * difMultiplier, 1, 2, 30, new Rectangle((map.GetNodePos(item).X + map.TileSize / 4) - xOffset, (map.GetNodePos(item).Y + map.TileSize / 4) - yOffset, 50, 50), tankTex, 1, 0, subpaths[item]));
                                            subEnemiesAdded++;
                                        }
                                    }
                                    else
                                    {
                                        if (subEnemiesAdded < waveCount * 2 * subpaths.Keys.Count)
                                        {
                                            enemyList.Add(new Enemy(10 , 3, 2, 25, new Rectangle((map.GetNodePos(item).X + map.TileSize / 4) - xOffset, (map.GetNodePos(item).Y + map.TileSize / 4) - yOffset, 50, 50), fastTex, 1, 0, subpaths[item]));
                                            subEnemiesAdded++;
                                        }
                                    }
                                    

                                }
                                spawnDelay = 0;
                            }
                            spawnDelay += t.ElapsedGameTime.TotalSeconds;


                            if(mainPath != null)
                            {
                                
                                foreach (Enemy item in enemyList)
                                {
                                    
                                    FollowPath(item, item.Path);

                                }


                                
                                for (int i = 0; i < enemyList.Count; i++)
                                {
                                    if (enemyList[i].IsDead)
                                    {
                                        enemyList.RemoveAt(i);
                                        Sound.SoundEffects[0].Play();
                                        i--;
                                    }
                                }
                                
                                foreach (Tower tower in towers)
                                {
                                    if (tower.Tesla)
                                    {
                                        tower.TargetAll(enemyList);
                                    }
                                    else
                                    {
                                        tower.Target(enemyList);
                                    }
                                }

                                if (enemyList.Count == 0 && enemiesAdded == waves[waveCount][0]&& enemiesFAdded==waves[waveCount][2])
                                {
                                    if(subpaths.Count==0|| subEnemiesAdded == waveCount * 2 * subpaths.Keys.Count)
                                    {
                                        enemiesAdded = 0;
                                        subEnemiesAdded = 0;
                                        enemiesTankAdded = 0;
                                        enemiesFAdded = 0;
                                        waveCount++;
                                        if(waveCount==10)
                                        {
                                            delayTime = .3;
                                        }
                                        if(waveCount==15)
                                        {
                                            delayTime = .2;
                                            difMultiplier = 2;
                                        }
                                        if(waveCount==20)
                                        {
                                            delayTime = .1;
                                            difMultiplier = 3;
                                        }
                                        
                                        state = PlayState.Expand;
                                        
                                    }
                                    
                                }
                                if (powderedEvents.Health <= 0)
                                {
                                    mainGame = GameState.End;
                                }
                                
                            }
                            
                            break;
                        ///boss state
                        case PlayState.Boss:
                            
                            if (enemiesAdded<1)
                            {
                                Sound.SoundEffects[3].Play();
                                enemyList.Add(new Enemy(bossMaxHealth, 1, 100000, 200000000, new Rectangle((map.GetNodePos(farthestNode).X + map.TileSize / 4) - xOffset, (map.GetNodePos(farthestNode).Y + map.TileSize / 4) - yOffset, 75, 75), bossTex, 1, 0, mainPath));
                                enemiesAdded++;
                            }
                           
                            

                            if (mainPath != null)
                            {

                                foreach (Enemy item in enemyList)
                                {

                                    FollowPath(item, item.Path);

                                }
                                for (int i = 0; i < enemyList.Count; i++)
                                {
                                    if (enemyList[i].IsDead)
                                    {
                                        enemyList.RemoveAt(i);
                                        i--;
                                    }
                                }

                                foreach (Tower tower in towers)
                                {
                                    tower.Target(enemyList);
                                }

                                
                                if (powderedEvents.Health <= 0)
                                { 
                                    mainGame = GameState.End;
                                }

                            }

                            if(enemyList.Count == 0 && powderedEvents.Health>0)
                            {
                                mainGame = GameState.Win;
                            }

                            break;
                    }
                    
                    break;
                case GameState.End:

                    if (menuDead.ButtonUsed.isClicked(ms, previousMS) == true && menuDead.ButtonUsed.IsVisible == true)
                    {
                        mainGame = GameState.Menu;
                        //add everything for resetting game
                        game.Quit();
                    }

                    break;
                case GameState.Win:

                    if (menuWin.ButtonUsed.isClicked(ms, previousMS) == true && menuWin.ButtonUsed.IsVisible == true)
                    {
                        game.Quit();
                    }

                    break;
            }
            pastState = Mouse.GetState().LeftButton;

            if(prevWave != waveCount)
            {
                gameplayUI.village.waveGold(totalMines);
            }

            prevWave = waveCount; 
        }
        /// <summary>
        /// makes enemy move on its path
        /// </summary>
        /// <param name="item">enemy being moved</param>
        /// <param name="path">path the enemy follows</param>
        private void FollowPath(Enemy item, List<Point> path)
        {
            //item.Target = path[item.PathIndex].X * map. + xOffset, point.Y * map.TileSize + yOffset;
            item.Target = new Point(path[path.Count - 1 - item.PathIndex].X * map.TileSize + map.TileSize / 2, path[path.Count - 1 - item.PathIndex].Y * map.TileSize + map.TileSize / 2);

            // Get distance between enemy and its target 
            // move to next target along path once reached 
            if (item.MoveToPoint(item.Target.ToVector2()))
            {
                if (item.PathIndex < path.Count - 1)
                {
                    item.PathIndex++;
                    item.PrevTarget = item.Target;
                }
                else
                {
                    powderedEvents.takeDamage(item);
                    Sound.SoundEffects[2].Play();
                }
            }
        }




        /// <summary>
        /// adds tile if it has active neighbor
        /// </summary>
        /// <param name="node">node being added</param>
        /// <returns></returns>
        private bool AddTile(Node node)
        {
            List<Node> neighbors = map.GetNeighbors(node);
            foreach (Node neighbor in neighbors)
            {
                // Check if they point at one another 
                if (node & neighbor)
                {
                    node.InPlay = true;
                    ActivateChunks(node);
                    activeNodes.Add(node);
                    

                    return true;
                }
            }

            

            return false;
        }
        /// <summary>
        /// sets array for where chunks are for towers
        /// </summary>
        /// <param name="node"></param>
        private void ActivateChunks(Node node)
        {
            // Iterate through chunks in tile and set them to play 
            Point root = new Point(node.X * 7, node.Y * 7);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    if(node.ChunkTextures[x, y] == map.PathTexture)
                    {
                        mapToTower[root.X + x, root.Y + y] = new TowerNode(null, false);
                    }
                    else
                    {
                        mapToTower[root.X + x, root.Y + y] = new TowerNode(null, true);
                    }
                }
            }
        }
        /// <summary>
        /// adds tower to map at mouse place
        /// </summary>
        /// <param name="ms">mouse state</param>
        /// <param name="tower">tower being added</param>
        private void AddTower(MouseState ms, Tower tower) //int damage, int range, int attackSpeed, Texture2D texture, Point size)
        {
            Point point = new Point((ms.Position.X)- xOffset, (ms.Position.Y ) - yOffset);
            Point index = map.GetChunkPos(ms.Position);

            
                // Add currently selected tower 

                // Todo -> Change following tower to what the user has selected 
                
                Tower towerToPlace =
                    new Tower(
                        tower.Damage,
                        tower.Range,
                        tower.Texture,
                        new Rectangle(
                            new Point(
                                point.X - (point.X % map.ChunkSize),
                                point.Y - (point.Y % map.ChunkSize)
                                ),
                            new Point(map.ChunkSize * tower.Size.X, map.ChunkSize * tower.Size.Y)// chunk size needs to be taken into account for rect 
                            ),
                        tempBulletTex,
                        tower.AttackSpeed,
                        tower.Size, //2x2 to see if bigger sizes work  
                        tower.Cost
                        );
                towerToPlace.Tesla = tower.Tesla;

                towerToPlace.OffsetTower(xOffset, yOffset); // Gets rid of "jumping" glitch 

                bool fits = true;
                fits = TowerCanBePlaced(index, towerToPlace);

                if (fits)
                {
                    for (int x = 0; x < tower.Size.X; x++)
                    {
                        for (int y = 0; y < tower.Size.Y; y++)
                        {

                            mapToTower[index.X + x, index.Y + y] = new TowerNode(towerToPlace, true);
                        }
                    }

                    towers.Add(towerToPlace);
                    if (towersByRow[index.Y / map.ChunkSize] != null)
                    {
                        towersByRow[index.Y / map.ChunkSize] = new List<Tower>();
                    }
                    towersByRow[index.Y / map.ChunkSize].Add(tower);
                    
                }

            


        }
        /// <summary>
        /// checks if tower can be placed
        /// </summary>
        /// <param name="index">index being checked</param>
        /// <param name="tower">tower being placed</param>
        /// <returns></returns>
        private bool TowerCanBePlaced(Point index, Tower tower)
        {
            bool fits = true;

            // Adds reference to placed tower to each chunk it covers 
            for (int x = 0; x < tower.Size.X; x++)
            {
                for (int y = 0; y < tower.Size.Y; y++)
                {
                    // Checks if fits in the bounds of the 2d array 
                    if (index.X + x < mapToTower.GetLength(0) && index.X + x >= 0 && index.Y + y < mapToTower.GetLength(1) && index.Y + y >= 0)
                    {
                        // Checks if it actually is in the game 
                        if (mapToTower[index.X + x, index.Y + y].InPlay == false || mapToTower[index.X + x, index.Y + y].Tower != null)
                        {
                            fits = false;
                            break;
                        }
                    }
                    else
                    {
                        fits = false;
                        break;
                    }
                }
            }

            return fits;
        }



        /// <summary>
        /// Changes the offset of any object rendered in game that is not UI  
        /// </summary>
        /// <param name="kb"></param>
        public void OffsetWindow(KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.W))
            {
                yOffset += screenMoveSpeed;
            }
            if (kb.IsKeyDown(Keys.S))
            {
                yOffset -= screenMoveSpeed;
            }
            if (kb.IsKeyDown(Keys.A))
            {
                xOffset += screenMoveSpeed;
            }
            if (kb.IsKeyDown(Keys.D))
            {
                xOffset -= screenMoveSpeed;
            }

            if (kb.IsKeyDown(Keys.Space))
            {
                xOffset = centerOffset.X;
                yOffset = centerOffset.Y;
            }

            // Offset the map
            map.OffsetMap(xOffset, yOffset);

            // Offset the towers 
            foreach (Tower tower in towers)
            {
                tower.OffsetTower(xOffset, yOffset);
            }
            // Offset the projectiles 

            // Offset the enemies 

            // Offset anything else? 
        }
    

        /// <summary>
        /// places tower at point
        /// </summary>
        /// <param name="tower"></param>
        /// <param name="pos"></param>
        public void PlaceTower(Tower tower, Point pos)
        {
            towers.Add(tower);
        }

        /// <summary>
        /// pauses all towers and enemies
        /// </summary>
        /// <param name="pause"></param>
        public void Pause(bool pause)
        {
            foreach (Tower tower in towers)
            {
                tower.Pause = pause;
            }

            foreach (Enemy item in enemyList)
            {
                item.Pause = pause;
            }
        }

        /// <summary>
        /// Used to simplify the process of tower management 
        /// </summary>
        private struct TowerNode
        {
            private Tower tower;
            private bool inPlay;

            public Tower Tower { get { return tower; } }
            public bool InPlay { get { return inPlay; } }

            public TowerNode(Tower tower, bool inPlay)
            {
                this.tower = tower;
                this.inPlay = inPlay;
            }
        }
    }
}



