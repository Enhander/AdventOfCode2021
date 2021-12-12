using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day2 {
    class Program {
        public struct MovementInstruction {
            public enum Instruction {
                Forward = 0,
                Down = 1,
                Up = 2
            }

            public Instruction instruction;
            public int distance;

            public MovementInstruction(Instruction thisInstruction, int thisDistance) {
                instruction = thisInstruction;
                distance = thisDistance;
            }
        }

        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 2\Part1Input.txt";
            List<string> input = LoadInput(filePath);

            List<MovementInstruction> movementInstructions = ParseInput(input);

            int[] startingPosition = new int[] { 0, 0 };
            int startingAim = 0;
            int[] finalPosition = ProjectFinishLocation(startingPosition, startingAim, movementInstructions);
            int product = finalPosition[0] * finalPosition[1];

            Console.Write("Submarine will move to ({0}, {1}).  Coordinate Product is {2}", finalPosition[0], finalPosition[1], product);
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

        public static List<MovementInstruction> ParseInput(List<string> rawInput) {
            List<MovementInstruction> movementInstructions = new List<MovementInstruction>();

            for (int i = 0; i < rawInput.Count; i++) {
                string[] splitInstruction = rawInput[i].Split(" ");

                MovementInstruction.Instruction instruction = MovementInstruction.Instruction.Forward;
                switch (splitInstruction[0]) {
                    case "forward":
                        instruction = MovementInstruction.Instruction.Forward;
                        break;
                    case "down":
                        instruction = MovementInstruction.Instruction.Down;
                        break;
                    case "up":
                        instruction = MovementInstruction.Instruction.Up;
                        break;
                }

                int distance = Int32.Parse(splitInstruction[1]);

                movementInstructions.Add(new MovementInstruction(instruction, distance));
            }

            return movementInstructions;
        }

        public static int[] ProjectFinishLocation(int[] startingPosition, int startingAim, List<MovementInstruction> movementInstructions) {
            int[] currentPosition = startingPosition;
            int currentAim = startingAim;

            for (int i = 0; i < movementInstructions.Count; i++) {
                switch (movementInstructions[i].instruction) {
                    case MovementInstruction.Instruction.Forward:
                        currentPosition[0] += movementInstructions[i].distance;
                        currentPosition[1] += movementInstructions[i].distance * currentAim;
                        break;
                    case MovementInstruction.Instruction.Down:
                        currentAim += movementInstructions[i].distance;
                        break;
                    case MovementInstruction.Instruction.Up:
                        currentAim -= movementInstructions[i].distance;
                        break;
                }
            }

            return currentPosition;
        }
    }
}
