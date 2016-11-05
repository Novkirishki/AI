using System;
using System.Collections.Generic;
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
            var maxLoops = queensCount * queensCount;

            while (true)
            {
                for (int i = 0; i < queensCount; i++)
                {
                    var currentQueen = queens[i];
                    var currentQueenConflicts = CalcConflicts(currentQueen, queens);
                    if (currentQueenConflicts == 0)
                        continue;

                    var minNumberOfConflicts = currentQueenConflicts;
                    var minNumberOfConflictsY = new List<int>();
                    minNumberOfConflictsY.Add(currentQueen.Y);
                    var initialY = currentQueen.Y;

                    for (int j = 0; j < queensCount; j++)
                    {
                        if (j != initialY)
                        {
                            currentQueen.Y = j;
                            var conflicts = CalcConflicts(currentQueen, queens);
                            if (conflicts < minNumberOfConflicts)
                            {
                                minNumberOfConflicts = conflicts;
                                minNumberOfConflictsY.Clear();
                                minNumberOfConflictsY.Add(j);
                            }
                            else if (conflicts == minNumberOfConflicts)
                            {
                                minNumberOfConflictsY.Add(j);
                            }
                        }
                    }

                    currentQueen.Y = minNumberOfConflictsY[rnd.Next(minNumberOfConflictsY.Count)];

                    if (queens.Where(x => CalcConflicts(x, queens) != 0).Count() == 0)
                    {
                        PrintQueens(queens);
                        return;
                    }
                }

                // if there is a deadlock - reset the board and try again
                maxLoops--;
                if (maxLoops < 0)
                {
                    maxLoops = queensCount * queensCount;
                    queens = GetQueens(queensCount);
                }
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
                    Y = 0,
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

        private static int CalcConflicts(Queen queen, List<Queen> queens)
        {
            var conflicts = new HashSet<int>();
            for (int i = 0; i < queens.Count; i++)
            {
                var other = queens[i];
                if (queen.Equals(other))
                    continue;

                if (queen.X == other.X && queen.Y > other.Y)
                    conflicts.Add(1);

                if (queen.X == other.X && queen.Y > other.Y)
                    conflicts.Add(2);

                if (queen.X < other.X && queen.Y == other.Y)
                    conflicts.Add(3);

                if (queen.X > other.X && queen.Y == other.Y)
                    conflicts.Add(4);

                var hasDiagonalConflict = Math.Abs(queen.X - other.X) == Math.Abs(queen.Y - other.Y);

                if (hasDiagonalConflict && queen.Y > other.Y && queen.X > other.X)
                    conflicts.Add(5);

                if (hasDiagonalConflict && queen.Y < other.Y && queen.X > other.X)
                    conflicts.Add(6);

                if (hasDiagonalConflict && queen.Y > other.Y && queen.X < other.X)
                    conflicts.Add(7);

                if (hasDiagonalConflict && queen.Y < other.Y && queen.X < other.X)
                    conflicts.Add(8);
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
