using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day13 {
    class Program {
        public class Instruction {
            public enum Axis {
                X = 0,
                Y = 1
            }
            public Axis foldingAxis;
            public int foldingLineOffset;

            public Instruction(string instructionLine) {
                string[] instructionParts = instructionLine.Split('=');

                foldingAxis = instructionParts[0].Contains('y') ? Axis.X : Axis.Y;
                foldingLineOffset = Int32.Parse(instructionParts[1]);
            }
        }

        public class Dot {
            public int xPosition, yPosition;

            public Dot(int thisXPosition, int thisYPosition) {
                xPosition = thisXPosition;
                yPosition = thisYPosition;
            }

            public bool Equals(Dot otherDot) {
                return xPosition == otherDot.xPosition && yPosition == otherDot.yPosition;
            }

            public static Dot Translate(Dot oldDot, int xTranslation, int yTranslation) {
                return new Dot(oldDot.xPosition + xTranslation, oldDot.yPosition + yTranslation);
            }

            public static Dot FoldAlongXAxis(Dot oldDot, int yAxisOffset) {
                Dot newDot = new Dot(oldDot.xPosition, oldDot.yPosition);
                newDot = Translate(newDot, 0, -yAxisOffset);

                if (newDot.yPosition > 0) {
                    newDot.yPosition = -newDot.yPosition;
                }

                newDot = Translate(newDot, 0, yAxisOffset);

                return newDot;
            }

            public static Dot FoldAlongYAxis(Dot oldDot, int xAxisOffset) {
                Dot newDot = new Dot(oldDot.xPosition, oldDot.yPosition);
                newDot = Translate(newDot, -xAxisOffset, 0);

                if (newDot.xPosition > 0) {
                    newDot.xPosition = -newDot.xPosition;
                }

                newDot = Translate(newDot, xAxisOffset, 0);

                return newDot;
            }
        }
        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 13\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            List<Instruction> instructions;
            List<Dot> dots = GetDotsAndInstructions(input, out instructions);

            dots = ExecuteInstructions(dots, instructions);
            PrintFoldedDots(dots);
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

        public static List<Dot> GetDotsAndInstructions(List<string> input, out List<Instruction> instructions) {
            List<Dot> dots = new List<Dot>();
            instructions = new List<Instruction>();

            foreach (string inputLine in input) {
                if (inputLine.Contains(',')) {
                    dots.Add(ParseDot(inputLine));
                }
                else if (inputLine.Contains('=')) {
                    instructions.Add(new Instruction(inputLine));
                }
            }

            return dots;
        }

        public static Dot ParseDot(string inputLine) {
            string[] coordinates = inputLine.Split(',');
            return new Dot(Int32.Parse(coordinates[0]), Int32.Parse(coordinates[1]));
        }

        public static List<Dot> ExecuteInstructions(List<Dot> dots, List<Instruction> instructions) {
            foreach (Instruction instruction in instructions) {
                for (int i = dots.Count - 1; i >= 0; i--) {
                    Dot dotAfterReflection = null;
                    switch (instruction.foldingAxis) {
                        case Instruction.Axis.X:
                            dotAfterReflection = Dot.FoldAlongXAxis(dots[i], instruction.foldingLineOffset);
                            break;
                        case Instruction.Axis.Y:
                            dotAfterReflection = Dot.FoldAlongYAxis(dots[i], instruction.foldingLineOffset);
                            break;
                    }

                    if (!dotAfterReflection.Equals(dots[i]) && DotsContain(dots, dotAfterReflection)) {
                        dots.RemoveAt(i);
                    }
                    else {
                        dots[i] = dotAfterReflection;
                    }
                }
            }

            return dots;
        }

        public static bool DotsContain(List<Dot> dots, Dot dotToSearch) {
            foreach (Dot dot in dots) {
                if (dot.Equals(dotToSearch)) {
                    return true;
                }
            }

            return false;
        }

        public static void PrintFoldedDots(List<Dot> dots) {
            int[] paperSize = FindPaperSize(dots);

            for (int row = 0; row < paperSize[1] + 1; row++) {
                for (int col = 0; col < paperSize[0] + 1; col++) {
                    Dot thisPoint = new Dot(col, row);

                    if (DotsContain(dots, thisPoint)) {
                        Console.Write('#');
                    }
                    else {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }

        public static int[] FindPaperSize(List<Dot> dots) {
            int largestXPosition = 0, largestYPosition = 0;

            foreach (Dot dot in dots) {
                if (dot.xPosition > largestXPosition) {
                    largestXPosition = dot.xPosition;
                }
                if (dot.yPosition > largestYPosition) {
                    largestYPosition = dot.yPosition;
                }
            }

            return new int[] { largestXPosition, largestYPosition };
        }
    }
}
