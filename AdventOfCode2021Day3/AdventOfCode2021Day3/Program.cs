using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day3 {
    class Program {
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 3\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            int gammaRate = FindGammaRate(input);
            int epsilonRate = FindEpsilonRate(input);
            int powerConsumption = gammaRate * epsilonRate;

            int oxygenGeneratorRating = FindOxygenGeneratorRating(input);
            int co2ScrubberRating = FindCO2ScrubberRating(input);
            int lifeSupportRating = oxygenGeneratorRating * co2ScrubberRating;

            Console.WriteLine("Gamma Rate: {0}, Epsilon Rate: {1}, Power Consumption: {2}", gammaRate, epsilonRate, powerConsumption);
            Console.WriteLine("Oxygen Generator Rating: {0}, CO2 Scrubber Rating: {1}, Life Support Rating: {2}", oxygenGeneratorRating, co2ScrubberRating, lifeSupportRating);
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

        public static int FindGammaRate(List<string> diagnosticReport) {
            string gammaRateBinary = "";

            for (int i = 0; i < diagnosticReport[0].Length; i++) {
                int[] bitCounts = CountBitsInPosition(i, diagnosticReport);

                gammaRateBinary = bitCounts[0] > bitCounts[1] ? gammaRateBinary + "0" : gammaRateBinary + "1";
            }

            return BinaryToDecimal(gammaRateBinary);
        }

        public static int FindEpsilonRate(List<string> diagnosticReport) {
            string epsilonRateBinary = "";

            for (int i = 0; i < diagnosticReport[0].Length; i++) {
                int[] bitCounts = CountBitsInPosition(i, diagnosticReport);

                epsilonRateBinary = bitCounts[0] < bitCounts[1] ? epsilonRateBinary + "0" : epsilonRateBinary + "1";
            }

            return BinaryToDecimal(epsilonRateBinary);
        }

        public static int FindOxygenGeneratorRating(List<string> diagnosticReport) {
            List<string> remainingBinaryNumbers = new List<string>(diagnosticReport);

            int bitPosition = 0;
            while (remainingBinaryNumbers.Count > 1) {
                int[] bitCounts = CountBitsInPosition(bitPosition, remainingBinaryNumbers);

                if (bitCounts[0] > bitCounts[1]) {
                    remainingBinaryNumbers = PruneBinaryNumbers(0, bitPosition, remainingBinaryNumbers);
                }
                else if (bitCounts[0] <= bitCounts[1]) {
                    remainingBinaryNumbers = PruneBinaryNumbers(1, bitPosition, remainingBinaryNumbers);
                }

                bitPosition++;
            }

            return BinaryToDecimal(remainingBinaryNumbers[0]);
        }

        public static int FindCO2ScrubberRating(List<string> diagnosticReport) {
            List<string> remainingBinaryNumbers = new List<string>(diagnosticReport);

            int bitPosition = 0;
            while (remainingBinaryNumbers.Count > 1) {
                int[] bitCounts = CountBitsInPosition(bitPosition, remainingBinaryNumbers);

                if (bitCounts[0] <= bitCounts[1]) {
                    remainingBinaryNumbers = PruneBinaryNumbers(0, bitPosition, remainingBinaryNumbers);
                }
                else if (bitCounts[0] > bitCounts[1]) {
                    remainingBinaryNumbers = PruneBinaryNumbers(1, bitPosition, remainingBinaryNumbers);
                }

                bitPosition++;
            }

            return BinaryToDecimal(remainingBinaryNumbers[0]);
        }

        public static int[] CountBitsInPosition(int positionIndex, List<string> binaryNumbers) {
            int numZeroes = 0;
            int numOnes = 0;

            foreach (string binaryNumber in binaryNumbers) {
                string bit = binaryNumber.Substring(positionIndex, 1);
                if (Int32.Parse(bit) == 0) {
                    numZeroes++;
                }
                else if (Int32.Parse(bit) == 1) {
                    numOnes++;
                }
            }

            return new int[] { numZeroes, numOnes };
        }

        public static List<string> PruneBinaryNumbers(int keepBit, int keepBitPosition, List<string> binaryNumbers) {
            List<string> prunedBinaryNumbers = binaryNumbers;

            for (int i = prunedBinaryNumbers.Count - 1; i >= 0; i--) {
                if (prunedBinaryNumbers[i].Substring(keepBitPosition, 1) != keepBit.ToString()) {
                    prunedBinaryNumbers.Remove(prunedBinaryNumbers[i]);
                }
            }

            return prunedBinaryNumbers;
        }

        public static int BinaryToDecimal(string binaryNumber) {
            int exponent = 0;
            int decimalNumber = 0;
            for (int i = binaryNumber.Length - 1; i >= 0; i--) {
                decimalNumber = Int32.Parse(binaryNumber.Substring(i, 1)) == 1 ? decimalNumber + (int)Math.Pow(2, exponent) : decimalNumber;
                exponent++;
            }

            return decimalNumber;
        }
    }
}
