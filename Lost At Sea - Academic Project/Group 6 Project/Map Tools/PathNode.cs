// ++++++
// Project: Lost at Sea
// Author: Narai Risser
// Description: Used to keep track of tiles regarding pathfinding 
// +++++

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    class PathNode
    {
        private bool visited;
        private PathNode parent;
        private Point pos;

        public bool Visited { get { return visited; } set { visited = value; } }
        public PathNode Parent { get { return parent; } set { parent = value; } }
        public Point Pos { get { return pos; } }

        public PathNode(Point pos)
        {
            this.pos = pos;
        }

        /// <summary>
        /// Checks if pathfinding nodes are equal 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Equals(PathNode node)
        {
            if(node == null)
            {
                return false;
            }

            return pos == node.pos;
        }
    }
}
