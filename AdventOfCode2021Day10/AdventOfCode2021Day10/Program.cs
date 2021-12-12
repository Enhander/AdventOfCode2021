using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day10 {
    class Program {
        public enum LineState {
            Normal = 0,
            Incomplete = 1,
            Corrupted = 2
        }

        public static Dictionary<char, int> closingCharacters = new Dictionary<char, int>() { { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 } };

        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 10\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            Dictionary<char, char> characterPairs = new Dictionary<char, char>() { { '(', ')' }, { '[', ']' }, { '{', '}' }, { '<', '>' } };
            Dictionary<char, int> closingCharacters = new Dictionary<char, int>() { { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 } };
            Dictionary<char, int> closingCompletionScores = new Dictionary<char, int>() { { ')', 1 }, { ']', 2 }, { '}', 3 }, { '>', 4 } };

            int score = CalculateSyntaxCheckScore(input, characterPairs, closingCharacters);
            Console.WriteLine("Syntax Check Score: {0}", score);

            long winningCompletionScore = FindAutocompleteWinner(input, characterPairs, closingCharacters, closingCompletionScores);
            Console.WriteLine("Autocomplete Check Winning Score: {0}", winningCompletionScore);
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

        public static LineState AnalyzeNavigationLine(string navigationLine, Dictionary<char, char> characterPairs, Dictionary<char, int> closingCharacters, out char invalidChar, out int invalidCharIndex, out Stack<char> openChars) {
            openChars = new Stack<char>();
            invalidChar = 'n';
            invalidCharIndex = -1;

            for (int i = 0; i < navigationLine.Length; i++) {
                if (characterPairs.ContainsKey(navigationLine[i])) {
                    openChars.Push(navigationLine[i]);
                }
                else if (closingCharacters.ContainsKey(navigationLine[i])) {
                    char expectedCharacter = characterPairs[openChars.Peek()];
                    if (navigationLine[i] == expectedCharacter) {
                        openChars.Pop();
                    }
                    else {
                        invalidChar = navigationLine[i];
                        invalidCharIndex = i;
                        return LineState.Corrupted;
                    }
                }
            }

            return openChars.Count == 0 ? LineState.Normal : LineState.Incomplete;
        }

        public static int CalculateSyntaxCheckScore(List<string> input, Dictionary<char, char> characterPairs, Dictionary<char, int> closingCharacters) {
            int score = 0;

            foreach (string navigationLine in input) {
                char invalidChar;
                int invalidCharIndex;
                Stack<char> openChars;

                LineState lineState = AnalyzeNavigationLine(navigationLine, characterPairs, closingCharacters, out invalidChar, out invalidCharIndex, out openChars);

                if (lineState == LineState.Corrupted) {
                    score += closingCharacters[invalidChar];
                }
            }

            return score;
        }

        public static List<string> FindCompletionStrings(List<string> navigationLines, Dictionary<char, char> characterPairs, Dictionary<char, int> closingCharacters) {
            List<string> completionStrings = new List<string>();

            foreach (string navigationline in navigationLines) {
                char invalidChar;
                int invalidCharIndex;
                Stack<char> openChars;

                LineState lineState = AnalyzeNavigationLine(navigationline, characterPairs, closingCharacters, out invalidChar, out invalidCharIndex, out openChars);

                if (lineState == LineState.Incomplete) {
                    string completionString = "";
                    string openString = "";

                    while (openChars.Count > 0) {
                        openString = openChars.Peek() + openString;
                        completionString += characterPairs[openChars.Pop()];
                    }

                    completionStrings.Add(completionString);
                }
            }

            return completionStrings;
        }

        public static List<long> CalculateCompletionScores(List<string> navigationLines, Dictionary<char, char> characterPairs, Dictionary<char, int> closingCharacters, Dictionary<char, int> closingCompletionScores) {
            List<string> completionStrings = FindCompletionStrings(navigationLines, characterPairs, closingCharacters);
            List<long> completionScores = new List<long>();

            foreach (string completionString in completionStrings) {
                long completionScore = 0;

                foreach (char completionCharacter in completionString) {
                    completionScore *= 5;
                    completionScore += closingCompletionScores[completionCharacter];
                }

                completionScores.Add(completionScore);
            }

            return completionScores;
        }

        public static long FindAutocompleteWinner(List<string> navigationLines, Dictionary<char, char> characterPairs, Dictionary<char, int> closingCharacters, Dictionary<char, int> closingCompletionScores) {
            List<long> completionScores = CalculateCompletionScores(navigationLines, characterPairs, closingCharacters, closingCompletionScores);
            completionScores.Sort();
            return completionScores[completionScores.Count / 2];
        }
    }
}
