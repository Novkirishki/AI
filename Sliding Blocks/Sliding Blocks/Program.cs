using System;
using System.Collections.Generic;
using System.Linq;

namespace Sliding_Blocks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a number:");
            var numersCount = int.Parse(Console.ReadLine()) + 1;
            var board = EnterBoard((int)Math.Sqrt(numersCount));
            var mahattanDistance = CalculateManhattanDistance(board);
            Console.WriteLine(mahattanDistance);
            var toBeCheckedStates = new List<BoardState>();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var number = board[i, j];
                    if (number == 0)
                    {
                        var initialBoardCondition = new BoardState
                        {
                            ManhattanDistance = mahattanDistance,
                            Board = board,
                            EmptyX = i,
                            EmptyY = j,
                            PreviousSteps = new string[0]
                        };

                        var stepsToSolution = GetStepsToSolution(initialBoardCondition, toBeCheckedStates);
                        PrintSteps(stepsToSolution);
                        return;
                    }
                }
            }
        }

        private static void PrintSteps(string[] stepsToSolution)
        {
            foreach (var step in stepsToSolution)
            {
                Console.WriteLine(step);
            }
        }

        private static void HandleChildState(BoardState childState, List<BoardState> toBeCheckedStates, List<BoardState> checkedStates)
        {
            if (childState != null && !checkedStates.Contains(childState))
            {
                var sameState = toBeCheckedStates.Where(x => x.Equals(childState)).FirstOrDefault();
                if (sameState == null)
                {
                    toBeCheckedStates.Add(childState);
                }
                else if (childState.PreviousSteps.Length < sameState.PreviousSteps.Length)
                {
                    toBeCheckedStates.Remove(sameState);
                    toBeCheckedStates.Add(childState);
                }
            }
        }

        private static string[] GetStepsToSolution(BoardState state, List<BoardState> toBeCheckedStates)
        {
            var checkedStates = new List<BoardState>();
            checkedStates.Add(state);

            while (state.ManhattanDistance != 0)
            {
                // left
                var childState = GetBoardStateAfterMove(state, state.EmptyX, state.EmptyY, state.EmptyX, state.EmptyY - 1);
                HandleChildState(childState, toBeCheckedStates, checkedStates);

                // right
                childState = GetBoardStateAfterMove(state, state.EmptyX, state.EmptyY, state.EmptyX, state.EmptyY + 1);
                HandleChildState(childState, toBeCheckedStates, checkedStates);

                // up
                childState = GetBoardStateAfterMove(state, state.EmptyX, state.EmptyY, state.EmptyX - 1, state.EmptyY);
                HandleChildState(childState, toBeCheckedStates, checkedStates);

                // down
                childState = GetBoardStateAfterMove(state, state.EmptyX, state.EmptyY, state.EmptyX + 1, state.EmptyY);
                HandleChildState(childState, toBeCheckedStates, checkedStates);

                toBeCheckedStates.Sort();
                state = toBeCheckedStates[0];
                toBeCheckedStates.RemoveAt(0);
            }

            return state.PreviousSteps;
        }

        private static BoardState GetBoardStateAfterMove(BoardState boardState, int emptyX, int emptyY, int newX, int newY)
        {
            if (0 <= newX && newX < boardState.Board.GetLength(0) && 0 <= newY && newY < boardState.Board.GetLength(1))
            {
                var initialManhattanDistance = CalculateManhattanDistance(boardState.Board, newX, newY, boardState.Board[newX, newY]);
                var manhattanDistanceAfterMove = CalculateManhattanDistance(boardState.Board, emptyX, emptyY, boardState.Board[newX, newY]);
                var boardAfterMove = (int[,])boardState.Board.Clone();
                var numberToMove = boardAfterMove[newX, newY];
                boardAfterMove[emptyX, emptyY] = numberToMove;
                boardAfterMove[newX, newY] = 0;
                var stepsAfterMove = new string[boardState.PreviousSteps.Length + 1];
                boardState.PreviousSteps.CopyTo(stepsAfterMove, 0);
                stepsAfterMove[stepsAfterMove.Length - 1] = GetDirection(emptyX, emptyY, newX, newY);

                return new BoardState
                {
                    ManhattanDistance = boardState.ManhattanDistance - initialManhattanDistance + manhattanDistanceAfterMove,
                    Board = boardAfterMove,
                    EmptyX = newX,
                    EmptyY = newY,
                    PreviousSteps = stepsAfterMove
                };
            }

            return null;
        }

        private static string GetDirection(int oldX, int oldY, int newX, int newY)
        {
            if (oldX < newX)
                return "up";

            if (oldX > newX)
                return "down";

            if (oldY < newY)
                return "left";

            if (oldY > newY)
                return "right";

            return "invalid move";
        }

        private static int CalculateManhattanDistance(int[,] board)
        {
            var boardRowSize = board.GetLength(0);
            var boardColumnSize = board.GetLength(1);
            var manhattanDistance = 0;
            for (int i = 0; i < boardRowSize; i++)
            {
                for (int j = 0; j < boardColumnSize; j++)
                {
                    manhattanDistance += CalculateManhattanDistance(board, i, j, board[i, j]);
                }
            }

            return manhattanDistance;
        }

        private static int CalculateManhattanDistance(int[,] board, int row, int col, int number)
        {
            if (number == 0)
                return 0;

            var correctRowIndex = (number - 1) / board.GetLength(0);
            var correctColIndex = (number - 1) % board.GetLength(1);

            return (Math.Abs(row - correctRowIndex) + Math.Abs(col - correctColIndex));
        }

        private static int[,] EnterBoard(int boardSize)
        {
            var board = new int[boardSize, boardSize];

            for (int i = 0; i < boardSize; i++)
            {
                var line = Console.ReadLine();
                var numbersInLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (numbersInLine.Count() != boardSize)
                {
                    throw new ArgumentException("Board is size is not correct.");
                }

                for (int j = 0; j < numbersInLine.Count(); j++)
                {
                    board[i, j] = int.Parse(numbersInLine[j]);
                }
            }

            return board;
        }

        private class BoardState : IComparable<BoardState>
        {
            public int ManhattanDistance { get; set; }

            public string[] PreviousSteps { get; set; }

            public int[,] Board { get; set; }

            public int EmptyX { get; set; }

            public int EmptyY { get; set; }

            public int CompareTo(BoardState other)
            {
                return (this.ManhattanDistance + this.PreviousSteps.Length).CompareTo(other.ManhattanDistance + other.PreviousSteps.Length);
            }

            public override bool Equals(object obj)
            {
                var otherState = (BoardState)obj;
                if (otherState == null)
                    return false;

                var rowsCount = this.Board.GetLength(0);
                var columnsCount = this.Board.GetLength(1);

                if (rowsCount != otherState.Board.GetLength(0) || columnsCount != otherState.Board.GetLength(1))
                    return false;

                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < columnsCount; j++)
                    {
                        if (this.Board[i, j] != otherState.Board[i, j])
                            return false;
                    }
                }

                return true;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
