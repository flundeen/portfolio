using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Group_6_Project
{
    class Tower : GameObjectClickable
    {
        //Fields
        private int damage;
        private int range;

        private Texture2D texture;
        private List<Projectile> proj;

        private int cost;

        private Texture2D bulletTexture;

        private bool attackDelay;

        //tracks the camera offset on tower
        private int xOffset;
        private int yOffset;

        bool targeting = false;
        private int count;
        private int minRange;

        private bool tesla; 

        //determines pause between shots, the higher the number the more pauses
        private int attackSpeed;
        private bool pause;

        // How many chunks does a tower take up in each axis 
        private Point size; 

        //Properties  
        public int Range
        {
            get { return range; }
            set { range = value; }
        }
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public int Cost
        {
            get { return cost; }
        }
        public bool AttackDelay
        {
            get { return attackDelay; }
            set { attackDelay = value; }
        }
        public bool Pause
        {
            get { return pause; }
            set { pause = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public int AttackSpeed
        {
            get { return attackSpeed; }
        }

        public Texture2D ProjTexture
        {
            get { return bulletTexture; }
            set { bulletTexture = value; }
        }

        //determines whether to tower is a tesla or not so that it uses the correct shooting method
        public bool Tesla
        {
            get { return tesla; }
            set { tesla = value; }
        }

        public Point Size { get { return size; } }


       

        //Constructor
        public Tower(int damage,int range,Texture2D texture,Rectangle rect, Texture2D bTexture, int attackSpeed, Point size, int cost)
        {
            this.damage = damage;
            this.range = range;
            this.texture = texture;
            this.rect = rect;
            attackDelay = false;
            bulletTexture = bTexture;
            proj = new List<Projectile>();
            this.cost = cost;
            count = 0;
            this.attackSpeed = attackSpeed;

            this.size = size;
            tesla = false;
            pause = false;
            minRange = 0;
        }
        //Constructor with min range - implemented to track for mortar
        public Tower(int damage, int range, Texture2D texture, Rectangle rect, Texture2D bTexture, int attackSpeed, Point size, int cost, int minRange)
        {
            this.damage = damage;
            this.range = range;
            this.texture = texture;
            this.rect = rect;
            attackDelay = false;
            bulletTexture = bTexture;
            proj = new List<Projectile>();
            this.cost = cost;
            count = 0;
            this.attackSpeed = attackSpeed;

            this.size = size;
            tesla = false;
            pause = false;
            this.minRange = minRange;

        }
        //constructor with tess bool
        public Tower(int damage, int range, Texture2D texture, Rectangle rect, Texture2D bTexture, int attackSpeed, Point size, int cost , bool tess)
        {
            this.damage = damage;
            this.range = range;
            this.texture = texture;
            this.rect = rect;
            attackDelay = false;
            bulletTexture = bTexture;
            proj = new List<Projectile>();
            this.cost = cost;
            count = 0;
            this.attackSpeed = attackSpeed;

            this.size = size;
            tesla = false;
            pause = false;
            minRange = 0;
            tesla = tess;
        }
        /// <summary>
        /// draws tower and projectiles
        /// </summary>
        /// <param name="sb"></param>
        public void draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle(rect.X + xOffset, rect.Y + yOffset, rect.Width, rect.Height), Color.White);
            foreach (Projectile item in proj)
            {
                item.Draw(sb, xOffset, yOffset);
            }
        }
        /// <summary>
        /// checks if an enemy is in range of the tower 
        /// </summary>
        /// <param name="item">Enemy being checked</param>
        /// <returns>if in range</returns>
        public bool inRange(Enemy item)
        {
            //if there is an enemy within the range
            if((item.Center-this.Center).Length()<=range && (item.Center - this.Center).Length()>=minRange)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// deals damage to an enemy
        /// </summary>
        /// <param name="item"></param>
        public void Attack(Enemy item)
        {
            //attacks the closest to the village
            if(inRange(item))
            {
                if(!attackDelay)
                {
                    Projectile p = new Projectile(this.Center.ToPoint(), bulletTexture, 6,item);
                    proj.Add(p);
                    
                    attackDelay = true;
                }
                else
                {
                    if(count>attackSpeed)
                    {
                        attackDelay = false;
                        count = 0;
                    }
                    count++;
                }
              
            }
            


        }
        /// <summary>
        /// returns center of tower
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }

        }
        /// <summary>
        /// offsets tower
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public void OffsetTower(int xOffset, int yOffset)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }
        /// <summary>
        /// picks an enemy to shoot
        /// </summary>
        /// <param name="list"></param>
        public void Target(List<Enemy> list)
        {
            if (pause)
            {
                return;
            }
            if (!targeting)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (inRange(list[i]))
                    {
                        Attack(list[i]);

                        break;
                    }
                }
            }
            foreach (Projectile item in proj)
            {
                if (item.IsVisible && item.Move())
                {
                    item.Target.Hp -= damage;
                }
                if(item.Target.IsDead)
                {
                    item.IsVisible = false;
                }
            }


        }

        //---------------------------------TESLA METHODS--------------------------------------

        /// <summary>
        /// shoots all enemies in range
        /// </summary>
        /// <param name="list"></param>
        public void TargetAll(List<Enemy> list)
        {
            if (pause)
            {
                return;
            }
            
            for (int i = 0; i < list.Count; i++)
            {
                if (inRange(list[i]))
                {
                     if(!AttackDelay)
                     {
                         AttackAll(list[i]);
                     }

                }
                
            }
            if (!attackDelay)
                attackDelay = true;
            if(attackDelay)
            {
                if (count > attackSpeed)
                {
                    attackDelay = false;
                    count = 0;
                }
                count++;
            }


            foreach (Projectile item in proj)
            {
                if (item.IsVisible && item.Move())
                {
                    item.Target.Hp -= damage;
                }
                if (item.Target.IsDead)
                {
                    item.IsVisible = false;
                }
            }


        }
        /// <summary>
        /// attaks all enemies without delay
        /// </summary>
        /// <param name="item"></param>
        public void AttackAll(Enemy item)
        {

            if (inRange(item))
            {
                
                    Projectile p = new Projectile(this.Center.ToPoint(), bulletTexture, 6, item);
                    proj.Add(p);

            }



        }

    }
}
