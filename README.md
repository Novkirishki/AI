# AI
Homeworks repo for the university AI course

## Task 1 - Frogs
Solution to the Frogs game using a DFS algorith. The rules of the game are that we have an 2n+1 sized board. On the board are place 2n frogs, with the middle space empty.N frogs can jump only to the right and are placed
on spaces 0 to n-1, and n frogs can jump only to the left and are placed on spaces n+1 to 2n. A frog can jump only on an empty space and can jump 1 or 2 spaces. The task is to swap the two types of frogs. Example:
>>_<< becomes <<_>>.

## Task 2 - Sliding Blocks

## Task 3 - N Queens

## Task 4 - Knapsack problem
A program that solves the knapsack problem using a genetic algorithm. The initial population is created randomly. Each next population is created from 25% of the "best" individuals from the previous population. 25% of the individuals are created using selection, 25% using crossover and 50% using mutation. The algorithm prints the current best solution on every 50 iterations.

## Task 5 - Tic Tac Toe
A game of Tic Tac Toe realized using the MinMax algorithm with alpha-beta pruning. The game can be played AI vs AI or AI vs Player.
It can also be played on a larger than 3x3 board. However on an empty larger board the AI takes a lot of time to make a move.
A game on a larger board can be tested by adding some random initial values to the board.
