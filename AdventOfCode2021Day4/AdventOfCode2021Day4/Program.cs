using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode2021Day4 {
    class Program {
        public class Board {
            public int[,] boardNumbers;
            public bool[,] calledNumbers;

            public Board(int[,] thisBoardNumbers, bool[,] thisCalledNumbers) {
                boardNumbers = thisBoardNumbers;
                calledNumbers = thisCalledNumbers;
            }

            public bool MarkNumber(int calledNumber) {
                for (int row = 0; row < boardNumbers.GetLength(0); row++) {
                    for (int col = 0; col < boardNumbers.GetLength(1); col++) {
                        if (boardNumbers[row, col] == calledNumber) {
                            calledNumbers[row, col] = true;
                            return CheckBoardWon(row, col);
                        }
                    }
                }

                return false;
            }

            private bool CheckBoardWon(int calledRow, int calledCol) {
                bool rowWon = true;
                for (int col = 0; col < calledNumbers.GetLength(1); col++) {
                    if (!calledNumbers[calledRow, col]) {
                        rowWon = false;
                        break;
                    }
                }

                bool colWon = true;
                for (int row = 0; row < calledNumbers.GetLength(0); row++) {
                    if (!calledNumbers[row, calledCol]) {
                        colWon = false;
                        break;
                    }
                }

                return rowWon || colWon;
            }

            public int BoardScore(int lastCalledNumber) {
                int sum = 0;

                for (int row = 0; row < boardNumbers.GetLength(0); row++) {
                    for (int col = 0; col < boardNumbers.GetLength(1); col++) {
                        sum = !calledNumbers[row, col] ? sum + boardNumbers[row, col] : sum;
                    }
                }

                return sum * lastCalledNumber;
            }
        }

        static void Main(string[] args) {
            string filePath = @"C:\Users\Enhander\Documents\Programming\Advent of Code\2021\Day 4\Part1Input.txt";
            List<int> drawnNumbers;
            List<Board> boards = LoadInput(filePath, out drawnNumbers);

            bool lastBoardWon = false;
            for (int i = 0; i < drawnNumbers.Count; i++) {
                if (lastBoardWon) {
                    break;
                }

                List<Board> winningBoards = DrawNumber(drawnNumbers[i], boards);

                if (winningBoards.Count > 0) {
                    foreach (Board winningBoard in winningBoards) {
                        if (boards.Count > 1) {
                            boards.Remove(winningBoard);
                        }
                        else {
                            Console.WriteLine("Last winning board's score: {0}", winningBoard.BoardScore(drawnNumbers[i]));
                            lastBoardWon = true;
                        }
                    }
                }
            }
        }

        public static List<Board> LoadInput(string filePath, out List<int> drawnNumbers) {
            StreamReader streamReader = new StreamReader(filePath);
            List<Board> boards = new List<Board>();
            drawnNumbers = new List<int>();

            string nextLine;

            nextLine = streamReader.ReadLine();
            string[] drawnNumberString = nextLine.Split(',');

            for (int i = 0; i < drawnNumberString.Length; i++) {
                drawnNumbers.Add(Int32.Parse(drawnNumberString[i]));
            }

            int boardLine = 0;
            Board tempBoard = new Board(new int[5, 5], new bool[5, 5]);
            nextLine = streamReader.ReadLine();

            while ((nextLine = streamReader.ReadLine()) != null) {
                if (nextLine == "") {
                    boards.Add(tempBoard);
                    boardLine = 0;
                    tempBoard = new Board(new int[5, 5], new bool[5, 5]);
                }
                else {
                    string[] boardRowNumbers = nextLine.Split(' ');
                    List<string> boardRowNumbersList = new List<string>(boardRowNumbers);

                    for (int i = boardRowNumbersList.Count - 1; i >= 0; i--) {
                        if (boardRowNumbersList[i] == " " || boardRowNumbersList[i] == "") {
                            boardRowNumbersList.Remove(boardRowNumbersList[i]);
                        }
                    }

                    for (int i = 0; i < boardRowNumbersList.Count; i++) {
                        tempBoard.boardNumbers[boardLine, i] = Int32.Parse(boardRowNumbersList[i]);
                    }

                    boardLine++;
                }
            }

            boards.Add(tempBoard);

            return boards;
        }

        public static List<Board> DrawNumber(int drawnNumber, List<Board> boards) {
            List<Board> winningBoards = new List<Board>();
            foreach (Board board in boards) {
                if (board.MarkNumber(drawnNumber)) {
                    winningBoards.Add(board);
                }
            }

            return winningBoards;
        }
    }
}
