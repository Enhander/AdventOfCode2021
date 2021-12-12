using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day5 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 5\Part1Input.txt";
            List<string> ventLines = LoadInput(filePath);

            int[,] ventMap = PlotVents(ventLines);

            int numberOverlapVents = CountOverlapVents(ventMap);
            Console.WriteLine("Number of overlapping vents: {0}", numberOverlapVents);
        }

        public static List<string> LoadInput(string filePath) {
            StreamReader streamReader = new StreamReader(filePath);
            List<string> input = new List<string>();

            string nextLine;
            while((nextLine = streamReader.ReadLine()) != null) {
                input.Add(nextLine);
            }

            return input;
        }

        public static int[,] PlotVents(List<string> ventLines) {
            int[,] ventMap = new int[1000, 1000];

            foreach (string ventLine in ventLines) {
                List<int[]> startEndCoordinates = ProcessLine(ventLine);
                ventMap = MarkVentLocations(startEndCoordinates[0], startEndCoordinates[1], ventMap);
            }

            return ventMap;
        }

        public static List<int[]> ProcessLine(string ventLine) {
            int[] startCoordinate, endCoordinate;

            string[] parts = ventLine.Split(' ');
            string[] startCoordinateString = parts[0].Split(',');
            string[] endCoordinateString = parts[2].Split(',');

            startCoordinate = new int[] { Int32.Parse(startCoordinateString[0]), Int32.Parse(startCoordinateString[1]) };
            endCoordinate = new int[] { Int32.Parse(endCoordinateString[0]), Int32.Parse(endCoordinateString[1]) };

            return new List<int[]> { startCoordinate, endCoordinate };
        }

        public static int[,] MarkVentLocations(int[] startCoordinates, int[] endCoordinates, int[,] ventCoordinates) {
            int[] currentCoordinates = startCoordinates;
            int xDirection = FindDirection(startCoordinates[0], endCoordinates[0]);
            int yDirection = FindDirection(startCoordinates[1], endCoordinates[1]);

            while (!CoordinatesMatch(currentCoordinates, endCoordinates)) {
                ventCoordinates[currentCoordinates[0], currentCoordinates[1]]++;

                currentCoordinates[0] += xDirection;
                currentCoordinates[1] += yDirection;
            }

            ventCoordinates[currentCoordinates[0], currentCoordinates[1]]++;

            return ventCoordinates;
        }

        public static int FindDirection(int start, int end) {
            int difference = end - start;

            if (difference > 0) {
                return 1;
            }
            else if (difference < 0) {
                return -1;
            }
            else {
                return 0;
            }
        }

        public static bool CoordinatesMatch(int[] firstCoordinate, int[] secondCoordinate) {
            return firstCoordinate[0] == secondCoordinate[0] && firstCoordinate[1] == secondCoordinate[1];
        }

        public static int CountOverlapVents(int[,] ventMap) {
            int numberOverlapVents = 0;

            for (int x = 0; x < ventMap.GetLength(0); x++) {
                for (int y = 0; y < ventMap.GetLength(1); y++) {
                    numberOverlapVents = ventMap[x, y] > 1 ? numberOverlapVents + 1 : numberOverlapVents;
                }
            }

            return numberOverlapVents;
        }
    }
}
