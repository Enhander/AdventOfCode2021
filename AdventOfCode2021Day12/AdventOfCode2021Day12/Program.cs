using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day12 {
    public class Cave {
        public List<Cave> linkedCaves;
        public bool isBigCave;
        public string caveName;

        public Cave (string thisCaveName, bool thisIsBigCave) {
            caveName = thisCaveName;
            isBigCave = thisIsBigCave;

            linkedCaves = new List<Cave>();
        }

        public void AddLinkedCave(Cave linkedCave) {
            if (!linkedCaves.Contains(linkedCave)) {
                linkedCaves.Add(linkedCave);
            }
        }
    }

    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 12\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            Dictionary<string, Cave> caveSystem = ConstructCaveSystem(input);

            List<List<Cave>> allPaths = FindAllPaths(caveSystem);

            Console.WriteLine("Total number of valid paths through cave system: {0}", allPaths.Count);
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

        public static Dictionary<string, Cave> ConstructCaveSystem(List<string> caveLinks) {
            Dictionary<string, Cave> caveSystem = new Dictionary<string, Cave>();

            foreach (string caveLink in caveLinks) {
                string[] cavesLinked = caveLink.Split('-');

                foreach (string cave in cavesLinked) {
                    if (!caveSystem.ContainsKey(cave)) {
                        Cave newCave = new Cave(cave, Char.IsUpper(cave[0]));
                        caveSystem.Add(cave, newCave);
                    }
                }

                caveSystem[cavesLinked[0]].AddLinkedCave(caveSystem[cavesLinked[1]]);
                caveSystem[cavesLinked[1]].AddLinkedCave(caveSystem[cavesLinked[0]]);
            }

            return caveSystem;
        }

        public static List<List<Cave>> FindAllPaths(Dictionary<string, Cave> caveSystem) {
            List<List<Cave>> allPaths = new List<List<Cave>>();
            List<Cave> initialPath = new List<Cave>();

            initialPath.Add(caveSystem["start"]);

            foreach (Cave linkedCave in caveSystem["start"].linkedCaves) {
                List<List<Cave>> pathBranches = ExplorePath(initialPath, linkedCave);
                allPaths.AddRange(pathBranches);
            }

            return allPaths;
        }

        public static List<List<Cave>> ExplorePath(List<Cave> pathSoFar, Cave caveToExplore) {
            List<List<Cave>> paths = new List<List<Cave>>();
            List<Cave> fullPath = new List<Cave>(pathSoFar);
            if (caveToExplore.caveName == "end") {
                fullPath.Add(caveToExplore);
                paths.Add(fullPath);
            }
            else if (IsValidPath(pathSoFar, caveToExplore)) {
                fullPath.Add(caveToExplore);

                foreach (Cave linkedCave in caveToExplore.linkedCaves) {
                    List<List<Cave>> pathBranches = ExplorePath(fullPath, linkedCave);
                    paths.AddRange(pathBranches);
                }
            }

            return paths;
        }

        public static bool IsValidPath(List<Cave> pathSoFar, Cave caveToValidate) {
            if (caveToValidate.caveName == "start") {
                return false;
            }
            else if (caveToValidate.isBigCave) {
                return true;
            }
            else if (SmallCaveVisitedTwice(pathSoFar)) {
                return !pathSoFar.Contains(caveToValidate);
            }
            else {
                return true;
            }
        }

        public static bool SmallCaveVisitedTwice(List<Cave> pathSoFar) {
            for (int i = 0; i < pathSoFar.Count; i++) {
                if (!pathSoFar[i].isBigCave) {
                    List<Cave> tempPath = new List<Cave>(pathSoFar);
                    tempPath.RemoveAt(i);

                    if (tempPath.Contains(pathSoFar[i])) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
