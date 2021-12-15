using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day14 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 14\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            string polymerTemplate;
            Dictionary<string, char> pairInsertionRules = ParseInput(input, out polymerTemplate);

            Dictionary<string, long> finalDimerCounts = Polymerize(polymerTemplate, pairInsertionRules, 40);

            char[] endMonomers = new char[] { polymerTemplate[0], polymerTemplate[polymerTemplate.Length - 1] };
            long result = CalculateResult(finalDimerCounts, endMonomers);
            Console.WriteLine("Result: {0}", result);
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

        public static Dictionary<string, char> ParseInput(List<string> inputs, out string polymerTemplate) {
            Dictionary<string, char> pairInsertionRules = new Dictionary<string, char>();
            polymerTemplate = "";

            foreach (string input in inputs) {
                if (!input.Equals("")) {
                    if (input.Contains('>')) {
                        string[] parts = input.Split(" -> ");
                        pairInsertionRules.Add(parts[0], char.Parse(parts[1]));
                    }
                    else {
                        polymerTemplate = input;
                    }
                }
            }

            return pairInsertionRules;
        }

        public static Dictionary<string, long> Polymerize(string polymerTemplate, Dictionary<string, char> pairInsertionRules, int numPolymerizationSteps) {
            Dictionary<string, long> dimerCounts = SeedDimers(polymerTemplate, pairInsertionRules);

            for (int i = 0; i < numPolymerizationSteps; i++) {
                dimerCounts = PolymerizationStep(dimerCounts, pairInsertionRules);
            }

            return dimerCounts;
        }

        public static Dictionary<string, long> PolymerizationStep(Dictionary<string, long> dimerCounts, Dictionary<string, char> pairInsertionRules) {
            Dictionary<string, long> newDimers = new Dictionary<string, long>();

            foreach (KeyValuePair<string, char> pairInsertionRule in pairInsertionRules) {
                string firstNewDimer = pairInsertionRule.Key[0] + pairInsertionRule.Value.ToString();
                string secondNewDimer = pairInsertionRule.Value.ToString() + pairInsertionRule.Key[1];

                if (!newDimers.ContainsKey(firstNewDimer)) {
                    newDimers.Add(firstNewDimer, dimerCounts[pairInsertionRule.Key]);
                }
                else {
                    newDimers[firstNewDimer] += dimerCounts[pairInsertionRule.Key];
                }

                if (!newDimers.ContainsKey(secondNewDimer)) {
                    newDimers.Add(secondNewDimer, dimerCounts[pairInsertionRule.Key]);
                }
                else {
                    newDimers[secondNewDimer] += dimerCounts[pairInsertionRule.Key];
                }

                if (!newDimers.ContainsKey(pairInsertionRule.Key)) {
                    newDimers.Add(pairInsertionRule.Key, -dimerCounts[pairInsertionRule.Key]);
                }
                else {
                    newDimers[pairInsertionRule.Key] -= dimerCounts[pairInsertionRule.Key];
                }
            }

            foreach (KeyValuePair<string, long> newDimer in newDimers) {
                dimerCounts[newDimer.Key] += newDimer.Value;
            }

            return dimerCounts;
        }

        public static Dictionary<string, long> SeedDimers(string polymerTemplate, Dictionary<string, char> pairInsertionRules) {
            Dictionary<string, long> dimerCounts = FindAllDimers(pairInsertionRules);

            foreach (KeyValuePair<string, char> pairInsertionRule in pairInsertionRules) {
                for (int i = 0; i < polymerTemplate.Length - (pairInsertionRule.Key.Length - 1); i++) {
                    if (polymerTemplate[i] == pairInsertionRule.Key[0]) {
                        bool matchingSegment = true;

                        for (int j = 1; j < pairInsertionRule.Key.Length; j++) {
                            if (polymerTemplate[i + j] != pairInsertionRule.Key[j]) {
                                matchingSegment = false;
                            }
                        }

                        if (matchingSegment) {
                            dimerCounts[pairInsertionRule.Key]++;
                        }
                    }
                }
            }

            return dimerCounts;
        }

        public static Dictionary<string, long> FindAllDimers(Dictionary<string, char> pairInsertionRules) {
            Dictionary<string, long> dimers = new Dictionary<string, long>();

            foreach (KeyValuePair<string, char> pairInsertionRule in pairInsertionRules) {
                string firstProducedDimer = pairInsertionRule.Key[0] + pairInsertionRule.Value.ToString();
                string secondProducedDimer = pairInsertionRule.Value.ToString() + pairInsertionRule.Key[1];

                if (!dimers.ContainsKey(pairInsertionRule.Key)) {
                    dimers.Add(pairInsertionRule.Key, 0);
                }

                if (!dimers.ContainsKey(firstProducedDimer)) {
                    dimers.Add(firstProducedDimer, 0);
                }

                if (!dimers.ContainsKey(secondProducedDimer)) {
                    dimers.Add(secondProducedDimer, 0);
                }
            }

            return dimers;
        }

        public static long CalculateResult(Dictionary<string, long> dimerCounts, char[] endMonomers) {
            Dictionary<char, long> elementFrequencies = new Dictionary<char, long>();

            foreach (KeyValuePair<string, long> dimerCount in dimerCounts) {
                if (!elementFrequencies.ContainsKey(dimerCount.Key[0])) {
                    elementFrequencies.Add(dimerCount.Key[0], dimerCount.Value);
                }
                else {
                    elementFrequencies[dimerCount.Key[0]] += dimerCount.Value;
                }

                if (!elementFrequencies.ContainsKey(dimerCount.Key[1])) {
                    elementFrequencies.Add(dimerCount.Key[1], dimerCount.Value);
                }
                else {
                    elementFrequencies[dimerCount.Key[1]] += dimerCount.Value;
                }
            }

            Dictionary<char, long> correctedElementFrequencies = new Dictionary<char, long>();

            foreach (KeyValuePair<char, long> elementFrequency in elementFrequencies) {
                correctedElementFrequencies.Add(elementFrequency.Key, elementFrequency.Value / 2);
            }

            foreach (char endMonomer in endMonomers) {
                correctedElementFrequencies[endMonomer]++;
            }

            if (endMonomers[0] == endMonomers[1]) {
                correctedElementFrequencies[endMonomers[0]]--;
            }

            long[] highestAndLowestElementFrequencies = HighestAndLowestElementFrequencies(correctedElementFrequencies);

            return highestAndLowestElementFrequencies[0] - highestAndLowestElementFrequencies[1];
        }

        public static long[] HighestAndLowestElementFrequencies(Dictionary<char, long> elementFrequencies) {
            long highestElementFrequency = long.MinValue, lowestElementFrequency = long.MaxValue;

            foreach (KeyValuePair<char, long> elementFrequency in elementFrequencies) {
                if (elementFrequency.Value > highestElementFrequency) {
                    highestElementFrequency = elementFrequency.Value;
                }
                if (elementFrequency.Value < lowestElementFrequency) {
                    lowestElementFrequency = elementFrequency.Value;
                }
            }

            return new long[] { highestElementFrequency, lowestElementFrequency };
        }
    }
}
