using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace N_Queens
{
    class Program
    {
        private static Random rnd = new Random();

        static void Main()
        {
            Console.WriteLine("Enter the number of queens: ");
            int queensCount = int.Parse(Console.ReadLine());
            SolveQueens(queensCount);
        }

        private static void SolveQueens(int queensCount)
        {
            var queens = GetQueens(queensCount);
            var maxLoops = queensCount * 10;
            var allQueensConflicts = new Dictionary<Queen, KeyValuePair<int, List<Queen>>>();
            var calculateAllConflicts = true;

            while (true)
            {
                if (calculateAllConflicts)
                {
                    allQueensConflicts.Clear();
                    CalculateConflictingQueens(queens, queens, allQueensConflicts);
                    calculateAllConflicts = false;
                }

                var maxConflicts = allQueensConflicts.OrderByDescending(x => x.Value.Key).First().Value.Key;
                var allQueensWithMaxConflicts = allQueensConflicts.Where(x => x.Value.Key == maxConflicts).ToList();
                var currentQueenData = allQueensWithMaxConflicts[rnd.Next(allQueensWithMaxConflicts.Count)];
                var currentQueen = currentQueenData.Key;
                var currentQueenConflicts = currentQueenData.Value.Key;
                if (currentQueenConflicts == 0)
                {
                    PrintQueens(queens);
                    return;
                }

                var minNumberOfConflicts = currentQueenConflicts;
                var minNumberOfConflictsData = new List<KeyValuePair<int, List<Queen>>>();
                var initialY = currentQueen.Y;

                for (int j = 0; j < queensCount; j++)
                {
                    if (j != initialY)
                    {
                        currentQueen.Y = j;
                        var conflictingQueens = new List<Queen>();
                        var conflicts = CalcConflicts(currentQueen, queens, out conflictingQueens);
                        if (conflicts < minNumberOfConflicts)
                        {
                            minNumberOfConflicts = conflicts;
                            minNumberOfConflictsData.Clear();
                            minNumberOfConflictsData.Add(new KeyValuePair<int, List<Queen>>(j, conflictingQueens));
                        }
                        else if (conflicts == minNumberOfConflicts)
                        {
                            minNumberOfConflictsData.Add(new KeyValuePair<int, List<Queen>>(j, conflictingQueens));
                        }
                    }
                }

                // if we cannot move the queen so that there are fewer or the same amount of conflicts
                if (minNumberOfConflictsData.Count == 0)
                {
                    continue;
                }
                var currentQueenChangeData = minNumberOfConflictsData[rnd.Next(minNumberOfConflictsData.Count)];
                currentQueen.Y = currentQueenChangeData.Key;

                // update allQueensConflicts
                allQueensConflicts[currentQueen] = new KeyValuePair<int, List<Queen>>(minNumberOfConflicts, currentQueenChangeData.Value);
                CalculateConflictingQueens(currentQueenChangeData.Value, queens, allQueensConflicts);
                CalculateConflictingQueens(currentQueenData.Value.Value, queens, allQueensConflicts);

                // if there is a deadlock - reset the board and try again
                maxLoops--;
                if (maxLoops < 0)
                {
                    maxLoops = queensCount * 10;
                    queens = GetQueens(queensCount);
                    calculateAllConflicts = true;
                }
            }
        }

        private static void CalculateConflictingQueens(List<Queen> queensToCalc, List<Queen> allQueens, Dictionary<Queen, KeyValuePair<int, List<Queen>>> queensConflicts)
        {
            foreach (var queen in queensToCalc)
            {
                var conflictingQueens = new List<Queen>();
                var queenConflicts = CalcConflicts(queen, allQueens, out conflictingQueens);
                queensConflicts[queen] = new KeyValuePair<int, List<Queen>>(queenConflicts, conflictingQueens);
            }
        }

        private static List<Queen> GetQueens(int queensCount)
        {
            var queens = new List<Queen>();
            for (int i = 0; i < queensCount; i++)
            {
                var queen = new Queen
                {
                    X = i,
                    Y = rnd.Next(queensCount),
                };

                queens.Add(queen);
            }

            return queens;
        }

        private static void PrintQueens(List<Queen> queens)
        {
            for (int i = 0; i < queens.Count; i++)
            {
                var row = "".PadRight(queens.Count, '_').ToCharArray();
                row[queens[i].Y] = 'Q';
                Console.WriteLine(new string(row));
            }
        }

        private static int CalcConflicts(Queen queen, List<Queen> queens, out List<Queen> conflictingQueens)
        {
            var conflicts = new HashSet<int>();
            conflictingQueens = new List<Queen>();
            for (int i = 0; i < queens.Count; i++)
            {
                var other = queens[i];
                if (queen.Equals(other))
                    continue;

                if (queen.X < other.X && queen.Y == other.Y)
                {
                    conflictingQueens.Add(other);
                    conflicts.Add(1);
                    continue;
                }

                if (queen.X > other.X && queen.Y == other.Y)
                {
                    conflictingQueens.Add(other);
                    conflicts.Add(2);
                    continue;
                }

                var hasDiagonalConflict = Math.Abs(queen.X - other.X) == Math.Abs(queen.Y - other.Y);

                if (hasDiagonalConflict && queen.Y > other.Y && queen.X > other.X)
                {
                    conflictingQueens.Add(other);
                    conflicts.Add(3);
                    continue;
                }

                if (hasDiagonalConflict && queen.Y < other.Y && queen.X > other.X)
                {
                    conflictingQueens.Add(other);
                    conflicts.Add(4);
                    continue;
                }

                if (hasDiagonalConflict && queen.Y > other.Y && queen.X < other.X)
                {
                    conflictingQueens.Add(other);
                    conflicts.Add(5);
                    continue;
                }

                if (hasDiagonalConflict && queen.Y < other.Y && queen.X < other.X)
                {
                    conflictingQueens.Add(other);
                    conflicts.Add(6);
                    continue;
                }
            }

            return conflicts.Count();
        }

        private class Queen
        {
            public int X { get; set; }

            public int Y { get; set; }
        }
    }
}
