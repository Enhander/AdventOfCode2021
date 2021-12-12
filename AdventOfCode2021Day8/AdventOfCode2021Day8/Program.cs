using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021Day8 {
    public class NoteEntry {
        public string[] uniqueSignalPatterns;
        public string[] fourDigitOutputValue;

        public NoteEntry(string noteEntry) {
            string[] twoParts = noteEntry.Split('|');

            uniqueSignalPatterns = twoParts[0].Split(' ');
            fourDigitOutputValue = twoParts[1].Split(' ');
        }

        public int DecodeFourDigitOutputValue(Dictionary<char, char> signalMap, string[] fourDigitOutputValue) {
            string fourDigitOutputValueString = "";

            foreach (string digitSignal in fourDigitOutputValue) {
                string digitSignalTransformed = TransformSignal(digitSignal, signalMap);
                char[] digitSignals = digitSignalTransformed.ToCharArray();
                Array.Sort(digitSignals);
                string digitSignalTransformedSorted = new string(digitSignals);

                switch (digitSignalTransformedSorted) {
                    case "abcefg":
                        fourDigitOutputValueString += "0";
                        break;
                    case "cf":
                        fourDigitOutputValueString += "1";
                        break;
                    case "acdeg":
                        fourDigitOutputValueString += "2";
                        break;
                    case "acdfg":
                        fourDigitOutputValueString += "3";
                        break;
                    case "bcdf":
                        fourDigitOutputValueString += "4";
                        break;
                    case "abdfg":
                        fourDigitOutputValueString += "5";
                        break;
                    case "abdefg":
                        fourDigitOutputValueString += "6";
                        break;
                    case "acf":
                        fourDigitOutputValueString += "7";
                        break;
                    case "abcdefg":
                        fourDigitOutputValueString += "8";
                        break;
                    case "abcdfg":
                        fourDigitOutputValueString += "9";
                        break;
                    default:
                        break;
                }
            }

            return Int32.Parse(fourDigitOutputValueString);
        }

        public string TransformSignal(string digitSignal, Dictionary<char, char> signalMap) {
            Dictionary<char, char> decodeMap = signalMap.ToDictionary(x => x.Value, x => x.Key);
            string transformedSignal = "";

            foreach (char signal in digitSignal) {
                transformedSignal += decodeMap[signal];
            }

            return transformedSignal;
        }

        public Dictionary<char, char> FindSignalMap() {
            Dictionary<char, char> signalMap = new Dictionary<char, char>();

            List<string> fiveSegmentSignals, sixSegmentSignals;
            Dictionary<int, string> uniqueSegmentSignals = SortByNumSegments(uniqueSignalPatterns, out fiveSegmentSignals, out sixSegmentSignals);

            char aSignalPair = FindASignalMapping(uniqueSegmentSignals[1], uniqueSegmentSignals[7]);
            signalMap.Add('a', aSignalPair);

            char[] cfSignalPairIdentified = FindCFSignalMapping(new List<char>(uniqueSegmentSignals[1].ToCharArray()), sixSegmentSignals);
            signalMap.Add('c', cfSignalPairIdentified[0]);
            signalMap.Add('f', cfSignalPairIdentified[1]);

            List<char> bdSignalPairs = FindBDSignalPairs(uniqueSegmentSignals[1], uniqueSegmentSignals[4]);
            char[] bdSignalPairIdentified = FindBDSignalMapping(bdSignalPairs, sixSegmentSignals);
            signalMap.Add('b', bdSignalPairIdentified[0]);
            signalMap.Add('d', bdSignalPairIdentified[1]);

            char gSignalPair = FindGSignalMapping(sixSegmentSignals, signalMap);
            signalMap.Add('g', gSignalPair);

            char eSignalPair = FindESignalMapping(uniqueSegmentSignals[8], signalMap);
            signalMap.Add('e', eSignalPair);

            return signalMap;
        }

        public Dictionary<int, string> SortByNumSegments(string[] uniqueSignalPatterns, out List<string> fiveSegmentSignals, out List<string> sixSegmentSignals) {
            Dictionary<int, string> uniqueSegmentSignals = new Dictionary<int, string>();
            fiveSegmentSignals = new List<string>();
            sixSegmentSignals = new List<string>();

            foreach (string signalPattern in uniqueSignalPatterns) {
                switch (signalPattern.Length) {
                    case 2:
                        uniqueSegmentSignals.Add(1, signalPattern);
                        break;
                    case 3:
                        uniqueSegmentSignals.Add(7, signalPattern);
                        break;
                    case 4:
                        uniqueSegmentSignals.Add(4, signalPattern);
                        break;
                    case 5:
                        fiveSegmentSignals.Add(signalPattern);
                        break;
                    case 6:
                        sixSegmentSignals.Add(signalPattern);
                        break;
                    case 7:
                        uniqueSegmentSignals.Add(8, signalPattern);
                        break;
                    default:
                        break;
                }
            }

            return uniqueSegmentSignals;
        }

        public char FindASignalMapping(string oneSignal, string sevenSignal) {
            foreach (char signal in sevenSignal) {
                if (!oneSignal.Contains(signal)) {
                    return signal;
                }
            }

            return 'n';
        }

        public char[] FindCFSignalMapping(List<char> cfSignalPairs, List<string> sixSegmentSignals) {
            char[] cfSignalPairIdentified = new char[2];

            foreach (string sixSegmentSignal in sixSegmentSignals) {
                if (!sixSegmentSignal.Contains(cfSignalPairs[0])) {
                    cfSignalPairIdentified[0] = cfSignalPairs[0];
                    cfSignalPairIdentified[1] = cfSignalPairs[1];
                    break;
                }
                else if (!sixSegmentSignal.Contains(cfSignalPairs[1])) {
                    cfSignalPairIdentified[0] = cfSignalPairs[1];
                    cfSignalPairIdentified[1] = cfSignalPairs[0];
                    break;
                }
            }

            return cfSignalPairIdentified;
        }

        public List<char> FindBDSignalPairs(string oneSignal, string fourSignal) {
            List<char> bdSignalPairs = new List<char>();

            foreach (char signal in fourSignal) {
                if (!oneSignal.Contains(signal)) {
                    bdSignalPairs.Add(signal);
                }
            }

            return bdSignalPairs;
        }

        public char[] FindBDSignalMapping(List<char> bdSignalPairs, List<string> sixSegmentSignals) {
            char[] bdSignalPairIdentified = new char[2];

            foreach (string sixSegmentSignal in sixSegmentSignals) {
                if (!sixSegmentSignal.Contains(bdSignalPairs[0])) {
                    bdSignalPairIdentified[0] = bdSignalPairs[1];
                    bdSignalPairIdentified[1] = bdSignalPairs[0];
                    break;
                }
                else if (!sixSegmentSignal.Contains(bdSignalPairs[1])) {
                    bdSignalPairIdentified[0] = bdSignalPairs[0];
                    bdSignalPairIdentified[1] = bdSignalPairs[1];
                    break;
                }
            }

            return bdSignalPairIdentified;
        }

        public char FindGSignalMapping(List<string> sixSegmentSignals, Dictionary<char, char> signalMap) {
            foreach (string sixSegmentSignal in sixSegmentSignals) {
                if (sixSegmentSignal.Contains(signalMap['c']) && sixSegmentSignal.Contains(signalMap['d'])) {
                    foreach (char signal in sixSegmentSignal) {
                        if (!signalMap.ContainsValue(signal)) {
                            return signal;
                        }
                    }
                }
            }

            return 'n';
        }

        public char FindESignalMapping(string eightSignal, Dictionary<char, char> signalMap) {
            foreach (char signal in eightSignal) {
                if (!signalMap.ContainsValue(signal)) {
                    return signal;
                }
            }

            return 'n';
        }
    }

    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 8\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            List<NoteEntry> notes = GenerateNotes(input);
            List<int> decodedNotes = DecodeNotes(notes);

            int sum = 0;
            foreach (int decodedNote in decodedNotes) {
                sum += decodedNote;
            }

            Console.WriteLine("Sum of four digit displays: {0}", sum);
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

        public static List<NoteEntry> GenerateNotes(List<string> input) {
            List<NoteEntry> notes = new List<NoteEntry>();

            foreach (string signalEntry in input) {
                NoteEntry noteEntry = new NoteEntry(signalEntry);
                notes.Add(noteEntry);
            }

            return notes;
        }

        public static List<int> DecodeNotes(List<NoteEntry> notes) {
            List<int> decodedNotes = new List<int>();

            foreach (NoteEntry note in notes) {
                Dictionary<char, char> signalMap = note.FindSignalMap();
                decodedNotes.Add(note.DecodeFourDigitOutputValue(signalMap, note.fourDigitOutputValue));
            }

            return decodedNotes;
        }
    }
}
