using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day1 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 1\Part1Input.txt";
            List<int> input = LoadInput(filePath);

            List<int> windowMeasurements = CalculateMeasurementWindows(input);
            int numberIncreasedMeasurements = CountIncreasedMeasurements(windowMeasurements);
            Console.Write("Number of increased depth measurements: " + numberIncreasedMeasurements);
        }

        public static List<int> LoadInput(string filePath) {
            StreamReader streamReader = new StreamReader(filePath);

            List<int> input = new List<int>();
            string nextLine;
            while((nextLine = streamReader.ReadLine()) != null) {
                input.Add(Int32.Parse(nextLine));
            }

            return input;
        }

        public static int CountIncreasedMeasurements(List<int> windowMeasurements) {
            int numberIncreasedMeasurements = 0;

            for (int i = 1; i < windowMeasurements.Count; i++) {
                if (windowMeasurements[i] > windowMeasurements[i - 1]) {
                    numberIncreasedMeasurements++;
                }
            }

            return numberIncreasedMeasurements;
        }

        public static List<int> CalculateMeasurementWindows(List<int> depthMeasurements) {
            List<int> windowMeasurements = new List<int>();

            for (int i = 0; i < depthMeasurements.Count - 2; i++) {
                windowMeasurements.Add(depthMeasurements[i] + depthMeasurements[i + 1] + depthMeasurements[i + 2]);
            }

            return windowMeasurements;
        }
    }
}
