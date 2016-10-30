using System;
using System.Collections.Generic;
using System.Linq;

namespace Frogs
{
    class Program
    {
        private static Stack<string> stepsToSolution;

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter a number:");
            var number = int.Parse(Console.ReadLine());

            var beginningStep = new char[number * 2 + 1];
            for (int i = 0; i < number; i++)
            {
                beginningStep[i] = '>';
                beginningStep[beginningStep.Length - i - 1] = '<'; 
            }
            
            beginningStep[(beginningStep.Length - 1) / 2] = '_';
            stepsToSolution = new Stack<string>();
            stepsToSolution.Push(new string(beginningStep));
            FindSolution(stepsToSolution);
            PrintSteps(stepsToSolution);
        }

        private static void PrintSteps(Stack<string> stepsToSolution)
        {
            foreach (var step in stepsToSolution.Reverse())
            {
                Console.WriteLine(step);
            }
        }

        private static bool FindSolution(Stack<string> stepsToSolution)
        {
            var currentStep = stepsToSolution.Peek();

            if (IsFinalStep(currentStep))
                return true;

            var result = false;

            for (int i = 0; i < currentStep.Length; i++)
            {
                if (currentStep[i] == '>')
                {
                    if (i + 1 < currentStep.Length && currentStep[i + 1] == '_')
                    {
                        var tempStep = SwapFrogs(currentStep, i, i + 1);
                        stepsToSolution.Push(tempStep);
                        if (FindSolution(stepsToSolution))
                            return true;
                        
                        stepsToSolution.Pop();
                    }


                    if (i + 2 < currentStep.Length && currentStep[i + 2] == '_')
                    {
                        var tempStep = SwapFrogs(currentStep, i, i + 2);
                        stepsToSolution.Push(tempStep);
                        if (FindSolution(stepsToSolution))
                            return true;

                        stepsToSolution.Pop();
                    }
                }

                if (currentStep[i] == '<')
                {
                    if (i - 1 >= 0 && currentStep[i - 1] == '_')
                    {
                        var tempStep = SwapFrogs(currentStep, i, i - 1);
                        stepsToSolution.Push(tempStep);
                        if (FindSolution(stepsToSolution))
                            return true;

                        stepsToSolution.Pop();
                    }

                    if (i - 2 >= 0 && currentStep[i - 2] == '_')
                    {
                        var tempStep = SwapFrogs(currentStep, i, i - 2);
                        stepsToSolution.Push(tempStep);
                        if (FindSolution(stepsToSolution))
                            return true;

                        stepsToSolution.Pop();
                    }
                }
            }

            return false;
        }

        private static bool IsFinalStep(string currentStep)
        {
            for (int i = 0; i < (currentStep.Length - 1) / 2; i++)
            {
                if (currentStep[i] != '<' || currentStep[currentStep.Length - 1 - i] != '>')
                    return false;
            }

            return true;
        }

        private static string SwapFrogs(string currentStep, int frogPosition, int emptySpacePosition)
        {
            char[] array = currentStep.ToCharArray();
            char temp = array[frogPosition];
            array[frogPosition] = array[emptySpacePosition]; 
            array[emptySpacePosition] = temp;
            return new string(array);
        }
    }
}
