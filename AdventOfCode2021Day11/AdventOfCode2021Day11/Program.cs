using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day11 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 11\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            int[,] octopusGrid = ConstructOctopusGrid(input);

            int[,] endOctopusGrid;
            int numStepsToSimultaneousFlash;
            int totalOctopusFlashes = SimulateOctopusFlashes(octopusGrid, out numStepsToSimultaneousFlash, out endOctopusGrid);

            Console.WriteLine("Final octopus grid: ");

            for (int row = 0; row < endOctopusGrid.GetLength(1); row++) {
                for (int col = 0; col < endOctopusGrid.GetLength(0); col++) {
                    Console.Write(endOctopusGrid[col, row]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("Total octopus flashes: {0}", totalOctopusFlashes);

            Console.WriteLine("Steps to simultaneous flash: {0}", numStepsToSimultaneousFlash);
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

        public static int[,] ConstructOctopusGrid(List<string> input) {
            int[,] octopusGrid = new int[input[0].Length, input.Count];

            for (int row = 0; row < octopusGrid.GetLength(1); row++) {
                for(int col = 0; col < octopusGrid.GetLength(0); col++) {
                    octopusGrid[col, row] = Int32.Parse(input[row][col].ToString());
                }
            }

            return octopusGrid;
        }

        public static int SimulateOctopusFlashes(int[,] initialOctopusGrid, out int numSteps, out int[,] endOctopusGrid) {
            int[,] currentOctopusGrid = initialOctopusGrid;
            int totalOctopusFlashes = 0;
            numSteps = 0;
            bool simultaneousFlash = false;

            while (!simultaneousFlash) {
                currentOctopusGrid = IncrementOctopusEnergies(currentOctopusGrid);

                int stepOctopusFlashes;
                currentOctopusGrid = ResolveOctopusFlashes(currentOctopusGrid, out stepOctopusFlashes, out simultaneousFlash);

                totalOctopusFlashes += stepOctopusFlashes;
                numSteps++;
            }

            endOctopusGrid = currentOctopusGrid;
            return totalOctopusFlashes;
        }

        public static int[,] IncrementOctopusEnergies(int[,] currentOctopusGrid) {
            for (int row = 0; row < currentOctopusGrid.GetLength(1); row++) {
                for (int col = 0; col < currentOctopusGrid.GetLength(0); col++) {
                    currentOctopusGrid[col, row]++;
                }
            }

            return currentOctopusGrid;
        }

        public static int[,] ResolveOctopusFlashes(int[,] currentOctopusGrid, out int numFlashes, out bool allSpent) {
            List<int[]> octopusesToFlash = new List<int[]>();
            List<int[]> spentOctopuses = new List<int[]>();
            numFlashes = 0;
            allSpent = false;

            for (int row = 0; row < currentOctopusGrid.GetLength(1); row++) {
                for (int col = 0; col < currentOctopusGrid.GetLength(0); col++) {
                    if (currentOctopusGrid[col, row] > 9) {
                        int[] octopusToFlash = new int[] { col, row };
                        octopusesToFlash.Add(octopusToFlash);
                        spentOctopuses.Add(octopusToFlash);
                    }
                }
            }

            while (octopusesToFlash.Count > 0) {
                List<int[]> newOctopusesToFlash = new List<int[]>();
                
                for (int i = octopusesToFlash.Count - 1; i >= 0; i--) {
                    numFlashes++;

                    List<int[]> newlyPrimedOctopuses;
                    currentOctopusGrid = FlashOctopus(currentOctopusGrid, octopusesToFlash[i][1], octopusesToFlash[i][0], spentOctopuses, out newlyPrimedOctopuses);

                    octopusesToFlash.Remove(octopusesToFlash[i]);

                    spentOctopuses.AddRange(newlyPrimedOctopuses);
                    newOctopusesToFlash.AddRange(newlyPrimedOctopuses);
                }

                octopusesToFlash.AddRange(newOctopusesToFlash);
            }

            allSpent = spentOctopuses.Count == currentOctopusGrid.GetLength(0) * currentOctopusGrid.GetLength(1);

            foreach (int[] spentOctopus in spentOctopuses) {
                currentOctopusGrid[spentOctopus[0], spentOctopus[1]] = 0;
            }

            return currentOctopusGrid;
        }

        public static int[,] FlashOctopus(int[,] octopusGrid, int row, int col, List<int[]> spentOctopuses, out List<int[]> newlyPrimedOctopuses) {
            newlyPrimedOctopuses = new List<int[]>();
            for (int rowMod = -1; rowMod <= 1; rowMod++) {
                for (int colMod = -1; colMod <= 1; colMod++) {
                    int adjacentRow = row + rowMod;
                    int adjacentCol = col + colMod;
                    if (adjacentRow >= 0 && adjacentRow < octopusGrid.GetLength(1) && adjacentCol >= 0 && adjacentCol < octopusGrid.GetLength(0)) {
                        if (!(rowMod == 0 && colMod == 0)) {
                            octopusGrid[adjacentCol, adjacentRow]++;
                            int[] adjacentOctopus = new int[] { adjacentCol, adjacentRow };
                            if (octopusGrid[adjacentCol, adjacentRow] > 9 && !OctopusIsSpent(spentOctopuses, adjacentOctopus)) {
                                newlyPrimedOctopuses.Add(adjacentOctopus);
                            }
                        }
                    }
                }
            }

            return octopusGrid;
        }

        public static bool OctopusIsSpent(List<int[]> spentOctopuses, int[] thisOctopus) {
            foreach (int[] spentOctopus in spentOctopuses) {
                if (spentOctopus[0] == thisOctopus[0] && spentOctopus[1] == thisOctopus[1]) {
                    return true;
                }
            }

            return false;
        }
    }
}
