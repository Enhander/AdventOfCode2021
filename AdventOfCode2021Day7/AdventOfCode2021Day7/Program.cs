using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day7 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 7\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            List<int> crabPositions = ParseCrabPositions(input);

            int currentBestFuelUsage;
            int alignmentPoint = FindCentralPosition(crabPositions, out currentBestFuelUsage);

            Console.WriteLine("Current best fuel usage is {0} at point {1}.", currentBestFuelUsage, alignmentPoint);
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

        public static List<int> ParseCrabPositions(List<string> input) {
            string[] crabPositionStrings = input[0].Split(',');
            List<int> crabPositions = new List<int>();
            
            foreach (string crabPosition in crabPositionStrings) {
                crabPositions.Add(Int32.Parse(crabPosition));
            }

            return crabPositions;
        }

        public static int FindCentralPosition(List<int> crabPositions, out int currentBestFuelUsage) {
            crabPositions.Sort();

            int midPoint = crabPositions[crabPositions.Count / 2];
            currentBestFuelUsage = CalculateFuelUsage(crabPositions, midPoint);
            int fuelUsage = Int32.MinValue;

            while (true) {
                fuelUsage = CalculateFuelUsage(crabPositions, midPoint + 1);
                if (fuelUsage < currentBestFuelUsage) {
                    midPoint++;
                    currentBestFuelUsage = fuelUsage;
                }
                else {
                    break;
                }
            }

            while (true) {
                fuelUsage = CalculateFuelUsage(crabPositions, midPoint - 1);
                if (fuelUsage < currentBestFuelUsage) {
                    midPoint--;
                    currentBestFuelUsage = fuelUsage;
                }
                else {
                    break;
                }
            }

            return midPoint;
        }

        public static int CalculateFuelUsage(List<int> crabPositions, int targetPosition) {
            int totalFuelUsed = 0;

            foreach (int crabPosition in crabPositions) {
                int distance = Math.Abs(crabPosition - targetPosition);
                int fuelUsage = (distance * (distance + 1)) / 2;
                totalFuelUsed += fuelUsage;
            }

            return totalFuelUsed;
        }
    }
}
