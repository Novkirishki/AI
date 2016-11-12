using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = new char[,]
            {
                { ' ', ' ', ' ' },
                { ' ', ' ', ' ' },
                { ' ', ' ', ' ' },
            };
            Console.WriteLine(Play(board, 'x', 'o', false));
        }

        private static char Play(char[,] board, char currentPlayerSymbol, char otherPlayerSymbol, bool isSimulation)
        {
            var winnerSymbol = IsGameOver(board);
            if (winnerSymbol == 'n' && !IsBoardFull(board))
            {
                var possibleMoves = new List<Move>();
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] == ' ')
                        {
                            var newBoard = new char[board.GetLength(0), board.GetLength(1)];
                            Array.Copy(board, newBoard, board.Length);
                            newBoard[i, j] = currentPlayerSymbol;
                            var winner = Play(newBoard, otherPlayerSymbol, currentPlayerSymbol, true);

                            int outcome = 0;
                            if (winner == currentPlayerSymbol)
                                return Play(newBoard, otherPlayerSymbol, currentPlayerSymbol, isSimulation);
                            else if (winner == otherPlayerSymbol)
                                outcome = -1;

                            possibleMoves.Add(new Move
                            {
                                Outcome = outcome,
                                X = i,
                                Y = j,
                                Symbol = currentPlayerSymbol
                            });
                        }
                    }
                }

                possibleMoves.Sort();
                var move = possibleMoves.First();
                var boardCopy = new char[board.GetLength(0), board.GetLength(1)];
                Array.Copy(board, boardCopy, board.Length);
                boardCopy[move.X, move.Y] = move.Symbol;
                return Play(boardCopy, otherPlayerSymbol, currentPlayerSymbol, isSimulation);
            }

            if (!isSimulation)
            {
                PrintBoard(board);
            }
            return winnerSymbol;
        }

        private static char IsGameOver(char[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                var isMainDiagonal = true;
                var isSubDiagonal = true;
                var isRow = true;
                var isCol = true;

                for (int j = 0; j < board.GetLength(1) - 1; j++)
                {
                    if (isRow && board[i, j] != board[i, j + 1])
                        isRow = false;

                    if (isCol && board[j, i] != board[j + 1, i])
                        isCol = false;

                    if (isMainDiagonal && board[j, j] != board[j + 1, j + 1])
                        isMainDiagonal = false;

                    if (isSubDiagonal && board[j, board.GetLength(1) - j - 1] != board[j + 1, board.GetLength(1) - j - 2])
                        isSubDiagonal = false;

                    if (!isRow && !isCol && !isMainDiagonal && !isSubDiagonal)
                        break;
                }

                if ((isRow || isCol || isMainDiagonal) && board[i, i] != ' ')
                    return board[i, i];

                if (isSubDiagonal && board[i, board.GetLength(1) - i - 1] != ' ')
                    return board[i, board.GetLength(1) - i - 1];
            }

            return 'n';
        }

        private static void PrintBoard(char[,] board)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    sb.Append(string.Format(" {0} |", board[i, j]));
                }

                sb.Remove(sb.Length - 1, 1);
                sb.AppendLine();
            }

            Console.WriteLine(sb);
        }

        private static bool IsBoardFull(char[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == ' ')
                        return false;
                }
            }

            return true;
        }

        private class Move : IComparable<Move>
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Outcome { get; set; }

            public char Symbol { get; set; }

            public int CompareTo(Move other)
            {
                return other.Outcome.CompareTo(this.Outcome);
            }
        }
    }
}
