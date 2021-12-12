using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day9 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 9\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            int[,] map = ConstructMap(input);
            List<int[]> lowPoints = FindLowPoints(map);
            List<int> basinSizes = FindBasinSizes(lowPoints, map);

            basinSizes.Sort((a, b) => b.CompareTo(a));
            int product = basinSizes[0] * basinSizes[1] * basinSizes[2];

            Console.WriteLine("Product of three largest basin sizes: {0}", product);
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

        public static int[,] ConstructMap(List<string> input) {
            int[,] map = new int[input[0].Length, input.Count];

            for (int row = 0; row < input.Count; row++) {
                for (int col = 0; col < input[0].Length; col++) {
                    map[col, row] = Int32.Parse(input[row].Substring(col, 1));
                }
            }

            return map;
        }

        public static List<int[]> FindLowPoints(int[,] map) {
            List<int[]> lowPoints = new List<int[]>();

            for (int row = 0; row < map.GetLength(1); row++) {
                for (int col = 0; col < map.GetLength(0); col++) {
                    if (NorthHigher(row, col, map) && EastHigher(row, col, map) && SouthHigher(row, col, map) && WestHigher(row, col, map)) {
                        lowPoints.Add(new int[] { col, row });
                    }
                }
            }

            return lowPoints;
        }

        public static bool NorthHigher(int row, int col, int[,] map) {
            return row == 0 || map[col, row - 1] > map[col, row];
        }

        public static bool EastHigher(int row, int col, int[,] map) {
            return col == map.GetLength(0) - 1 || map[col + 1, row] > map[col, row];
        }

        public static bool SouthHigher(int row, int col, int[,] map) {
            return row == map.GetLength(1) - 1 || map[col, row + 1] > map[col, row];
        }

        public static bool WestHigher(int row, int col, int[,] map) {
            return col == 0 || map[col - 1, row] > map[col, row];
        }

        public static List<int> FindBasinSizes(List<int[]> lowPoints, int[,] map) {
            List<int> basinSizes = new List<int>();

            foreach (int[] lowPoint in lowPoints) {
                basinSizes.Add(FindBasinSize(lowPoint, map));
            }

            return basinSizes;
        }

        public static int FindBasinSize(int[] lowPoint, int[,] map) {
            List<int[]> frontierPoints = new List<int[]> { lowPoint };
            List<int[]> exploredPoints = new List<int[]>();

            while (frontierPoints.Count > 0) {
                for (int i = frontierPoints.Count - 1; i >= 0; i--) {
                    int[] frontierPoint = frontierPoints[i];

                    int[] northernPoint = new int[] { frontierPoint[0], frontierPoint[1] - 1 };
                    if (northernPoint[1] >= 0 && map[northernPoint[0], northernPoint[1]] != 9) {
                        if (!ContainsPoint(frontierPoints, northernPoint) && !ContainsPoint(exploredPoints, northernPoint)) {
                            frontierPoints.Add(new int[] { northernPoint[0], northernPoint[1] });
                        }
                    }

                    int[] easternPoint = new int[] { frontierPoint[0] + 1, frontierPoint[1] };
                    if (easternPoint[0] < map.GetLength(0) && map[easternPoint[0], easternPoint[1]] != 9) {
                        if (!ContainsPoint(frontierPoints, easternPoint) && !ContainsPoint(exploredPoints, easternPoint)) {
                            frontierPoints.Add(new int[] { easternPoint[0], easternPoint[1] });
                        }
                    }

                    int[] southernPoint = new int[] { frontierPoint[0], frontierPoint[1] + 1 };
                    if (southernPoint[1] < map.GetLength(1) && map[southernPoint[0], southernPoint[1]] != 9) {
                        if (!ContainsPoint(frontierPoints, southernPoint) && !ContainsPoint(exploredPoints, southernPoint)) {
                            frontierPoints.Add(new int[] { southernPoint[0], southernPoint[1] });
                        }
                    }

                    int[] westernPoint = new int[] { frontierPoint[0] - 1, frontierPoint[1] };
                    if (westernPoint[0] >= 0 && map[westernPoint[0], westernPoint[1]] != 9) {
                        if (!ContainsPoint(frontierPoints, westernPoint) && !ContainsPoint(exploredPoints, westernPoint)) {
                            frontierPoints.Add(new int[] { westernPoint[0], westernPoint[1] });
                        }
                    }

                    exploredPoints.Add(frontierPoint);
                    frontierPoints.Remove(frontierPoint);
                }
            }

            return exploredPoints.Count;
        }

        public static bool ContainsPoint(List<int[]> points, int[] pointToCheck) {
            foreach (int[] point in points) {
                if (point[0] == pointToCheck[0] && point[1] == pointToCheck[1]) {
                    return true;
                }
            }

            return false;
        }

        public static int CalculateLowPointRiskLevel(List<int[]> lowPoints, int[,] map) {
            int sum = 0;

            foreach (int[] lowPoint in lowPoints) {
                sum += map[lowPoint[0], lowPoint[1]] + 1;
            }

            return sum;
        }
    }
}
