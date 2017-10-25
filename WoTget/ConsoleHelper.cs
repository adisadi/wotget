using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoTget
{
    public static class ConsoleHelper
    {
        private static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }

        public static void ExecuteLongTaskProgressbar<T>(IEnumerable<T> list, Action<T> action)
        {
            int i = 0;
            foreach (var item in list)
            {
                drawTextProgressBar(i + 1, list.Count());
                i++;

                action(item);
            }
            Console.WriteLine("");
        }

        public static void ExecuteLongTask(string description, Action action)
        {
            Console.Write(description);
            ConsoleSpiner spiner = new ConsoleSpiner();
            var task = Task.Factory.StartNew(action);
            while (!task.IsCompleted)
            {
                spiner.Turn();
            }
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.WriteLine("done");
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
        }

        public static void ColoredConsoleWriteLine(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }

        public static void WriteError(string message)
        {
            ColoredConsoleWriteLine(ConsoleColor.Red, message);
        }

 
    }

    public class ConsoleSpiner
    {
        int counter;
        public ConsoleSpiner()
        {
            counter = 0;
        }
        public void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
