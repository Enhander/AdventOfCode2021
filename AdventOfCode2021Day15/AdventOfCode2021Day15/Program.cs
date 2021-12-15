using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode2021Day15 {
    public class GridNode {
        public Point coordinates;
        private GridNode fromNode;
        public int risk;
        public int heuristicCost;
        public int pathCost;

        public GridNode FromNode {
            get {
                return fromNode;
            }
            set {
                if (fromNode == null) {
                    fromNode = value;
                    pathCost = risk + fromNode.pathCost;
                }
                else {
                    if (pathCost > risk + value.pathCost) {
                        fromNode = value;
                        pathCost = risk + fromNode.pathCost;
                    }
                }
            }
        }

        public GridNode(Point thisCoordinates, int thisRisk, int thisHeuristicCost) {
            coordinates = thisCoordinates;
            risk = thisRisk;
            heuristicCost = thisHeuristicCost;
            pathCost = 0;
        }

        public bool Equals(GridNode otherNode) {
            return coordinates.Equals(otherNode.coordinates);
        }
    }

    public class Grid {
        public GridNode[,] grid;

        public Grid(GridNode[,] thisGrid) {
            grid = thisGrid;
        }

        public static int ManhattanDistance(GridNode pointA, GridNode pointB) {
            return Math.Abs(pointB.coordinates.X - pointA.coordinates.X) + Math.Abs(pointB.coordinates.Y - pointA.coordinates.Y);
        }

        public void AStar(GridNode startPoint, GridNode endPoint) {
            List<GridNode> frontierNodes = new List<GridNode>() { startPoint };
            GridNode currentNode = startPoint;

            do {
                frontierNodes.RemoveAt(0);

                List<GridNode> neighbors = FindNeighbors(currentNode);

                foreach (GridNode neighbor in neighbors) {
                    if (neighbor != null && neighbor != currentNode.FromNode) {
                        neighbor.FromNode = currentNode;
                        
                        if (neighbor.FromNode == currentNode) {
                            neighbor.heuristicCost = CalculateHeuristicCost(neighbor, endPoint);

                            if (!frontierNodes.Contains(neighbor)) {
                                frontierNodes.Add(neighbor);
                            }
                        }
                    }
                }

                frontierNodes.Sort((x, y) => x.heuristicCost.CompareTo(y.heuristicCost));
                currentNode = frontierNodes[0];
            }
            while (!currentNode.Equals(endPoint));
        }

        private List<GridNode> FindNeighbors(GridNode node) {
            List<GridNode> neighbors = new List<GridNode>();

            if (node.coordinates.Y > 0) {
                GridNode topNeighbor = grid[node.coordinates.X, node.coordinates.Y - 1];
                neighbors.Add(topNeighbor);
            }

            if (node.coordinates.X < grid.GetLength(0) - 1) {
                GridNode rightNeighbor = grid[node.coordinates.X + 1, node.coordinates.Y];
                neighbors.Add(rightNeighbor);
            }

            if (node.coordinates.Y < grid.GetLength(1) - 1) {
                GridNode bottomNeighbor = grid[node.coordinates.X, node.coordinates.Y + 1];
                neighbors.Add(bottomNeighbor);
            }
            
            if (node.coordinates.X > 0) {
                GridNode leftNeighbor = grid[node.coordinates.X - 1, node.coordinates.Y];
                neighbors.Add(leftNeighbor);
            }
            

            return neighbors;
        }

        private int CalculateHeuristicCost(GridNode node, GridNode endPoint) {
            return node.pathCost + ManhattanDistance(node, endPoint);
        }
    }

    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 15\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            GridNode startNode, endNode;
            Grid grid = ConstructGrid(input, 5, out startNode, out endNode);

            grid.AStar(startNode, endNode);
            Console.WriteLine("Lowest path cost: {0}", endNode.pathCost);
        }

        public static List<string> LoadInput(string filePath) {
            StreamReader streamReader = new StreamReader(filePath);
            List<string> input = new List<string>();

            string nextLine;
            while ((nextLine = streamReader.ReadLine()) != null) {
                input.Add(nextLine);
            }

            return input;
        }

        public static Grid ConstructGrid(List<string> input, int duplicationFactor, out GridNode startNode, out GridNode endNode) {
            GridNode[,] gridNodes = new GridNode[input.Count * duplicationFactor, input[0].Length * duplicationFactor];

            for (int row = 0; row < input.Count * duplicationFactor; row++) {
                int inputYIndex = row % input.Count;
                int yRiskIncrease = row / input.Count;

                for (int col = 0; col < input[inputYIndex].Length * duplicationFactor; col++) {
                    int inputXIndex = col % input[inputYIndex].Length;
                    int xRiskIncrease = col / input[inputYIndex].Length;

                    int risk = (Int32.Parse(input[inputYIndex][inputXIndex].ToString()) + xRiskIncrease + yRiskIncrease) % 9;
                    risk = risk == 0 ? 9 : risk;

                    GridNode newNode = new GridNode(new Point(col, row), risk, Int32.MaxValue);
                    gridNodes[col, row] = newNode;
                }
            }

            startNode = gridNodes[0, 0];
            endNode = gridNodes[gridNodes.GetLength(0) - 1, gridNodes.GetLength(1) - 1];
            return new Grid(gridNodes);
        }
    }
}
