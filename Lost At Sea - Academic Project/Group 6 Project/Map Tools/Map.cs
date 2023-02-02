// ++++++
// Project: Lost at Sea
// Author: Narai Risser
// Description: Used to generate the map and pathfind over it. Holds info
// about placement about the map and node related information
// ++++++

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group_6_Project
{
    class Map
    {
        // The size of the map in tiles 
        private int width;
        private int height;

        // How much is the map offseted in each axis 
        private int xOffset;
        private int yOffset;

        public int XOffset { get { return xOffset; } set { xOffset = value; } }
        public int YOffset { get { return yOffset; } set { yOffset = value; } }
        public int Width { get { return width; }  }
        public int Height { get { return height; }  }

        private Node[,] nodeGrid;

        public Node[,] NodeGrid { get { return nodeGrid; } }
        public Node CenterNode { get { return nodeGrid[nodeGrid.GetLength(0) / 2, nodeGrid.GetLength(1) / 2]; } }

        // Eachtexture is the same size in pixels to keep map consistency 
        private Texture2D seaTexture, grassTexture, pathTexture;
        private Texture2D[,] defaultTile;

        // Used for making sure tower is not placed on it 
        public Texture2D PathTexture { get { return pathTexture; } }

        Random rng;

        // Size of a single side
        private int chunkSize;
        private int tileSize;

        public int ChunkSize { get { return chunkSize; } }

        public int TileSize { get { return tileSize; } }

        private NodePresetsLoader mapLoader;

        public Map(int width, int height, Texture2D seaTexture, Texture2D landTexture, Texture2D pathTexture)
        {
            rng = new Random();

            nodeGrid = new Node[width, height];

            // Does not matter which texture to choose size from 
            chunkSize = 15; // Pixels 
            // 7 chunks in a tile 
            tileSize = chunkSize * 7; 

            this.width = width;
            this.height = height;

            this.seaTexture = seaTexture;
            this.grassTexture = landTexture;
            this.pathTexture = pathTexture;

            // Sets the default tile that is not occupied to purely sea
            defaultTile = new Texture2D[7, 7];
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    defaultTile[x, y] = seaTexture;
                }
            }

            mapLoader = new NodePresetsLoader(7, seaTexture, grassTexture, pathTexture);

            // Populate nodeGrid with default nodes with position 
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodeGrid[x, y] = new Node(x, y);
                }
            }

            GenerateMaze();
            SetNodeDirections();
            SetNodeChunks();

            // Sets only center to be active 
            nodeGrid[nodeGrid.GetLength(0) / 2, nodeGrid.GetLength(1) / 2].InPlay = true;
        }



        // MAP GENERATION

        private void SetNodeChunks()
        {
            // Goes through each node
            foreach(Node node in nodeGrid)
            {
                // Get directions 
                bool[] directions = node.Directions;

                // Set the chunks to a random preset of the same directions 
                List<Texture2D[,]> presets = mapLoader.GetPresets(node.Directions);
                node.ChunkTextures = presets[rng.Next(0, presets.Count)];
            }
        }

        /// <summary>
        /// Creates a 7x7 tile node that shows both the position of 
        /// the node but also what directions they can go.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Texture2D[,] GetNodeChunks(Node node, int side)
        {
            // Each tile is split up into different chunks that could represent 
            // land, path, water, or tower 

            // Checks if the tile has already been explored by player 
            if(nodeGrid[node.X, node.Y].InPlay)
            {
                // Get the chunks from the node 
                return nodeGrid[node.X, node.Y].ChunkTextures;
            }
            else
            {
                // Display sea/default texture 
                return defaultTile;
            }
        }

        /// <summary>
        /// Creates a node tree using Prim's algorithm 
        /// </summary>
        private void GenerateMaze()
        {
            Stack<Node>[] stacks = new Stack<Node>[4];

            for (int i = 0; i < stacks.Length; i++)
            {
                stacks[i] = new Stack<Node>();
            }

            int counter = 0;


            // Setup center
            CenterNode.Directions = new bool[]
            {
                true,
                true,
                true,
                true,
            };
            CenterNode.Occupied = true;

            // Add 4 nodes surronding center
            stacks[0].Push(nodeGrid[CenterNode.X, CenterNode.Y + 1]);
            stacks[0].Peek().Directions = new bool[]
            {
                false,
                true,
                false,
                false
            };
            stacks[0].Peek().Occupied = true;


            stacks[1].Push(nodeGrid[CenterNode.X, CenterNode.Y - 1]);
            stacks[1].Peek().Directions = new bool[]
            {
                true,
                false,
                false,
                false
            };
            stacks[1].Peek().Occupied = true;

            stacks[2].Push(nodeGrid[CenterNode.X + 1, CenterNode.Y]);
            stacks[2].Peek().Directions = new bool[]
            {
                false,
                false,
                true,
                false
            };
            stacks[2].Peek().Occupied = true;

            stacks[3].Push(nodeGrid[CenterNode.X - 1, CenterNode.Y]);
            stacks[3].Peek().Directions = new bool[]
            {
                false,
                false,
                false,
                true
            };
            stacks[3].Peek().Occupied = true;

            int currentStack = 0;
            // Continue until tree is fully connected 
            do
            {
                if (stacks[currentStack].Count == 0)
                {
                    counter++;
                    continue;
                }

                // Get current
                Node current = stacks[currentStack].Peek();

                // Get neighbors
                List<Node> neighbors = GetNeighborsInGeneration(current.X, current.Y);

                // Check if valid 
                if (neighbors.Count > 0)
                {
                    // Random child 
                    Node target = neighbors[rng.Next(0, neighbors.Count)];

                    // Add child to current 
                    current.Children.Add(target);

                    // Set target to occupied 
                    target.Occupied = true;

                    // Add target to stack
                    stacks[currentStack].Push(target);

                    counter++;
                }
                else
                {
                    // No longer can go to this node 
                    stacks[currentStack].Pop();
                }

                currentStack = (currentStack + 1) % 4;

            } while (counter < (width * height));

            
        }

        /// <summary>
        /// Gets each neighbor in one axis directions but only 
        /// if they are within the bounds of the map and are 
        /// not occupied 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private List<Node> GetNeighborsInGeneration(int x, int y, bool isGeneration = true)
        {
            List<Node> neighbors = new List<Node>();

            if (x + 1 < width) // Right
            {
                if (nodeGrid[x + 1, y].Occupied == false || !isGeneration)
                    neighbors.Add(nodeGrid[x + 1, y]);
            }
            if (x - 1 >= 0) // Left 
            {
                if (nodeGrid[x - 1, y].Occupied == false || !isGeneration)
                    neighbors.Add(nodeGrid[x - 1, y]);
            }
            if (y + 1 < height) // Down
            {
                if (nodeGrid[x, y + 1].Occupied == false || !isGeneration)
                    neighbors.Add(nodeGrid[x, y + 1]);
            }
            if (y - 1 >= 0) // Up 
            {
                if (nodeGrid[x, y - 1].Occupied == false || !isGeneration)
                    neighbors.Add(nodeGrid[x, y - 1]);
            }

            return neighbors;
        }

        private List<PathNode> GetNeighbors(int x, int y, PathNode[,] pathGrid)
        {
            List<PathNode> neighbors = new List<PathNode>();

            if (x + 1 < width) // Right
            {
                if (nodeGrid[x + 1, y].InPlay == true && nodeGrid[x,y] & nodeGrid[x + 1, y])
                    neighbors.Add(pathGrid[x + 1, y]);
            }
            if (x - 1 >= 0) // Left 
            {
                if (nodeGrid[x - 1, y].InPlay == true && nodeGrid[x, y] & nodeGrid[x - 1, y])
                    neighbors.Add(pathGrid[x - 1, y]);
            }
            if (y + 1 < height) // Down
            {
                if (nodeGrid[x, y + 1].InPlay == true && nodeGrid[x, y] & nodeGrid[x, y + 1])
                    neighbors.Add(pathGrid[x, y + 1]);
            }
            if (y - 1 >= 0) // Up 
            {
                if (nodeGrid[x, y - 1].InPlay == true && nodeGrid[x, y] & nodeGrid[x, y - 1])
                    neighbors.Add(pathGrid[x, y - 1]);
            }

            return neighbors;
        }

        /// <summary>
        /// Gets the neighbors of a node and whether you want
        /// the neighbors that are in play or not in play 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inPlay"></param>
        /// <returns></returns>
        public List<Node> GetNeighbors(Node node, bool inPlay = true)
        {
            return GetNeighbors(node.X, node.Y, nodeGrid, inPlay);
        }

        /// <summary>
        /// Gets the neighbors of a node at passed in indexes. Can decide whterh
        /// user wants neighbors that are in play or not 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pathGrid"></param>
        /// <param name="inPlay"></param>
        /// <returns></returns>
        public List<Node> GetNeighbors(int x, int y, Node[,] pathGrid, bool inPlay = true)
        {
            List<Node> neighbors = new List<Node>();

            if (x + 1 < width) // Right
            {
                if (nodeGrid[x + 1, y].InPlay == inPlay)
                    neighbors.Add(pathGrid[x + 1, y]);
            }
            if (x - 1 >= 0) // Left 
            {
                if (nodeGrid[x - 1, y].InPlay == inPlay)
                    neighbors.Add(pathGrid[x - 1, y]);
            }
            if (y + 1 < height) // Down
            {
                if (nodeGrid[x, y + 1].InPlay == inPlay)
                    neighbors.Add(pathGrid[x, y + 1]);
            }
            if (y - 1 >= 0) // Up 
            {
                if (nodeGrid[x, y - 1].InPlay == inPlay)
                    neighbors.Add(pathGrid[x, y - 1]);
            }

            return neighbors;
        }


        /// <summary>
        /// Directions are found by going through each child of the 
        /// node and gets the direction to the child
        /// </summary>
        private void SetNodeDirections()
        {
            foreach (Node node in nodeGrid)
            {
                List<Node> children = node.Children;

                // For every child set proper direciton indicator 
                foreach (Node child in children)
                {
                    // Get direction 
                    int xDiff = child.X - node.X;
                    int yDiff = child.Y - node.Y;

                    bool[] parentDirections = node.Directions;
                    bool[] childDirecitons = child.Directions;

                    bool[] directions = new bool[4];

                    if (yDiff == 0) // Difference is left or right 
                    {
                        if (xDiff == 1) // right
                        {
                            directions[3] = true;
                        }
                        else // left 
                        {
                            directions[2] = true;
                        }
                    }
                    else // Difference is up or down 
                    {
                        if (yDiff == 1) // up
                        {
                            directions[0] = true;
                        }
                        else // down 
                        {
                            directions[1] = true;
                        }
                    }

                    // Setting the parents directions
                    bool[] newDirectionsParent = new bool[]
                    {
                        // Either the current direction is true
                        // or the direction one is true to make 
                        // the new set true in that direction 
                        parentDirections[0] || directions[0],
                        parentDirections[1] || directions[1],
                        parentDirections[2] || directions[2],
                        parentDirections[3] || directions[3],
                    };

                    // Setting the childs directions that correlates
                    // in the opposite diriction of its parent 
                    bool[] newDirectionsChild = new bool[]
                    {
                        childDirecitons[0] || directions[1],
                        childDirecitons[1] || directions[0],
                        childDirecitons[2] || directions[3],
                        childDirecitons[3] || directions[2],
                    };

                    node.Directions = newDirectionsParent;
                    child.Directions = newDirectionsChild;
                }
            }
        }

        /// <summary>
        /// Goes through each node and visualizes it in its correct position
        /// </summary>
        public void VisualizeMaze(SpriteBatch sb)
        {
            foreach (Node node in nodeGrid)
            {
                DrawNode(sb, node, Color.White);
            }
        }

        /// <summary>
        /// Draws a node to the screen based on its info and the offset
        /// of the map 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="node"></param>
        /// <param name="tint"></param>
        public void DrawNode(SpriteBatch sb, Node node, Color tint)
        {
            // Gets the chunks that make up a single node 
            int sideCount = (int)Math.Sqrt(node.Chunks);
            Texture2D[,] chunks = GetNodeChunks(node, sideCount);

            // Use the spritebatch to draw the node at the correct position 
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    // Draws individual chunk of a node 
                    sb.Draw(chunks[x, y],
                    new Rectangle(
                        xOffset + (x * chunkSize + (node.X * chunkSize * sideCount)),
                        yOffset + (y * chunkSize + (node.Y * chunkSize * sideCount)),
                        chunkSize,
                        chunkSize),
                    tint);
                }
            }
        }

        /// <summary>
        /// Offset the map by as many pixels desired 
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public void OffsetMap(int xOffset, int yOffset)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        // PATHFINDING 

        /// <summary>
        /// Get a list of points which represent the indexes of nodes that
        /// need to be travered to get from start to target on the map 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<Point> GetPath(Point start, Point target)
        {
            List<Point> path = new List<Point>();
            PathNode[,] pathGrid = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pathGrid[x, y] = new PathNode(new Point(x, y));
                }
            }

            Queue<PathNode> pq = new Queue<PathNode>();
            pq.Enqueue(new PathNode(start));

            while(pq.Count > 0)
            {
                PathNode current = pq.Dequeue();

                List<PathNode> neighbors = GetNeighbors(current.Pos.X, current.Pos.Y, pathGrid);

                // Sort neighbors based on distance to target 
                SortPathNodes(neighbors, target);

                // Getting neighbors only returns ones that are in play 
                foreach (PathNode neighbor in neighbors)
                {
                    if(!neighbor.Visited)
                    {
                        neighbor.Parent = current;

                        // Check to see if target has been reached 
                        if (neighbor.Equals(pathGrid[target.X, target.Y]))
                        {
                            BackTrack(path, pathGrid[target.X, target.Y], pathGrid[start.X, start.Y]);

                            return path;
                        }
                        else
                        {
                            neighbor.Visited = true;

                            pq.Enqueue(neighbor);
                        }
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Used to get the final path after necessary nodes are updated for pathfinding 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="target"></param>
        /// <param name="start"></param>
        private void BackTrack(List<Point> path, PathNode target, PathNode start)
        {

            // Todo -> Get individual path in each Tile 

            PathNode current = target;

            while(!start.Equals(current) && current != null)
            {
                path.Add(current.Pos);
                current = current.Parent;
            }
            path.Add(start.Pos);
        }

        /// <summary>
        /// Sorts a list nodes by how distance from target 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="target"></param>
        private void SortPathNodes(List<PathNode> nodes, Point target)
        {
            // Selections sort
            // Max length of nodes should be 4 so no need
            // to maximize efficiency 

            for (int i = 0; i < nodes.Count; i++)
            {
                int min = i;
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    // Checks if new value is less than current minimum 
                    if (GetDistance(nodes[j].Pos, target) < GetDistance(nodes[min].Pos, target))
                    {
                        // Sets new min 
                        min = j;
                    }
                }

                // Swap if needed
                PathNode temp = nodes[min];
                nodes[min] = nodes[i];
                nodes[i] = temp;
            }
        }

        /// <summary>
        /// Gets the nodes position in screen space 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Point GetNodePos(Node node)
        {

            return new Point(
                (int)(node.X * TileSize + XOffset),
                (int)(node.Y * TileSize + yOffset)
                );
        }

        /// <summary>
        /// Gets the distance between two points 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float GetDistance(Point a, Point b)
        {
            return MathF.Sqrt(MathF.Pow(b.X - a.X, 2) + MathF.Pow(b.Y - a.Y, 2));
        }


        /// <summary>
        /// Get node of current click 
        /// </summary>
        public Node MouseToNode(Point mousePos)
        {
            int xPos = (int)((mousePos.X - xOffset) / tileSize);
            int yPos = (int)((mousePos.Y - yOffset) / tileSize);

            if ((xPos >= 0) && (xPos < width) && (yPos >= 0) && (yPos < height))
            {
                return nodeGrid[xPos, yPos];
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Gets the position of a specific chunk within a tile 
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        public Point GetChunkPos(Point mousePos)
        {
            int xPos = (int)((mousePos.X - xOffset) / chunkSize);
            int yPos = (int)((mousePos.Y - yOffset) / chunkSize);

            return new Point(xPos, yPos);
        }
    }
}
