using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day6 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 6\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            List<int> lanternfishSchool = ConstructLanternfishSchool(input);

            int initialRespawnTime = 8;
            int respawnTime = 6;
            int days = 256;

            long totalLanternfishPopulation = ProjectLanternfishSpawn(lanternfishSchool, initialRespawnTime, respawnTime, days);

            Console.WriteLine("Number of lanternfish: {0}", totalLanternfishPopulation);
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

        public static List<int> ConstructLanternfishSchool(List<string> input) {
            List<int> lanternfishSchool = new List<int>();

            string[] fishTimers = input[0].Split(',');
            
            foreach (string fishTimer in fishTimers) {
                lanternfishSchool.Add(Int32.Parse(fishTimer));
            }

            return lanternfishSchool;
        }

        public static List<int> LanternfishDay(List<int> lanternfishSchool) {
            int lanternfishSpawn = 0;
            for (int i = 0; i < lanternfishSchool.Count; i++) {
                lanternfishSchool[i]--;

                if (lanternfishSchool[i] < 0) {
                    lanternfishSchool[i] = 6;
                    lanternfishSpawn++;
                }
            }

            for (int i = 0; i < lanternfishSpawn; i++) {
                lanternfishSchool.Add(8);
            }

            return lanternfishSchool;
        }

        public static long ProjectLanternfishSpawn(List<int> initialLanternfishTimers, int initialRespawnTime, int respawnTime, int daysToProject) {
            long[] fishAtTimer = new long[respawnTime + 1];
            long[] babyFishAtTimer = new long[initialRespawnTime + 1];

            foreach (int initialLanternfishTimer in initialLanternfishTimers) {
                fishAtTimer[initialLanternfishTimer]++;
            }

            for (int i = 0; i < daysToProject; i++) {
                long adultFishReadyToSpawn = fishAtTimer[0];
                long babyFishReadyToSpawn = babyFishAtTimer[0];

                for (int j = 0; j < babyFishAtTimer.Length; j++) {
                    if (j < fishAtTimer.Length - 1) {
                        fishAtTimer[j] = fishAtTimer[j + 1];
                        babyFishAtTimer[j] = babyFishAtTimer[j + 1];
                    }
                    else if (j < fishAtTimer.Length) {
                        fishAtTimer[j] = adultFishReadyToSpawn + babyFishReadyToSpawn;
                        babyFishAtTimer[j] = babyFishAtTimer[j + 1];
                    }
                    else if (j < babyFishAtTimer.Length - 1) {
                        babyFishAtTimer[j] = babyFishAtTimer[j + 1];
                    }
                    else {
                        babyFishAtTimer[j] = adultFishReadyToSpawn + babyFishReadyToSpawn;
                    }
                }
            }

            long totalFishPopulation = 0;
            foreach (long fish in fishAtTimer) {
                totalFishPopulation += fish;
            }
            foreach (long fish in babyFishAtTimer) {
                totalFishPopulation += fish;
            }

            return totalFishPopulation;
        }
    }
}
