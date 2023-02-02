// ++++++
// Project: Lost at Sea
// Author: Narai Risser
// Description: used to represent a single node in the map.
// Holds info about the node and how to draw it.
// ++++++

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    class Node
    {



        // RENDERING INFORMATION 
        // The amount chunks that make up a single tile length and width are 7
        private const int CHUNKS = 7 * 7;

        private Texture2D[,] chunkTextures;

        // Whether or not the player has added this node to the grid 
        private bool inPlay;



        // MAZE GENERATION INFORMATION 
        private bool occupied;
        private bool d, u, l, r; // Stores which directions are open 
        private int x, y;
        private List<Node> children;
        private Node parent;



        public bool InPlay { get { return inPlay; } set { inPlay = value; } }
        public Texture2D[,] ChunkTextures { get { return chunkTextures; } set { chunkTextures = value; } }
        public int Chunks { get { return CHUNKS; } }
        public bool Occupied { get { return occupied; } set { occupied = value; } }
        public bool[] Directions
        {
            get
            {
                return new bool[] { d, u, l, r };
            }
            set
            {
                d = value[0];
                u = value[1];
                l = value[2];
                r = value[3];
            }
        }
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public List<Node> Children { get { return children; } }
        public Node Parent { get { return parent; } set { parent = value; } }

        public Point point { get { return new Point(x, y); } }

        /// <summary>
        /// Constructs a node with a given index 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Node(int x, int y)
        {
            occupied = false;

            d = false;
            u = false;
            l = false;
            r = false;

            this.x = x;
            this.y = y;

            children = new List<Node>();
            parent = null;

            ChunkTextures = new Texture2D[7, 7];
            inPlay = false;

        }

        /// <summary>
        /// Checks whether two nodes are equal based on their index 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Equals(Node node)
        {
            return x == node.X && y == node.Y;
        }

        /// <summary>
        /// Gets a string that shows info about its index and whether it is inplay or not 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "This Node has an index of [" + x + ", " + y + " ] " + " and inPlay is " + InPlay;
        }

        

        /// <summary>
        /// Finds out if the two nodes have an opening between one another 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool operator &(Node A, Node B)
        {

            Point dir = B.point - A.point;

            if (dir.X != 0) // Left or right 
            {
                if (dir.X < 0) // Left
                {
                    return A.Directions[2] && B.Directions[3];
                }
                else // Right 
                {
                    return A.Directions[3] && B.Directions[2];
                }
            }
            else // Up or down 
            {
                if (dir.Y < 0) // Up
                {
                    return A.Directions[1] && B.Directions[0];
                }
                else // Down 
                {
                    return A.Directions[0] && B.Directions[1];
                }
            }
        }
    }
}
