using cAPParel.ConsoleClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Helpers
{
    public static class MenuBuilder
    {
        public async static Task CreateSingularMenu(List<Option> options)
        {
            int index = 0;
            ConsoleKeyInfo keyinfo;
            do
            {
                WriteMenu(options, options[index]);
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }

                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    if (index == options.Count - 1)
                    {
                        await options[index].Selected.Invoke();
                        break;
                    }
                    else
                    {
                        await options[index].Selected.Invoke();
                        break;
                    }

                }
            }
            while (true);
        }

        public async static Task CreateMenu(List<Option> options)
        {
            int index = 0;
            ConsoleKeyInfo keyinfo;
            do
            {
                WriteMenu(options, options[index]);
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }

                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    if (index == options.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        await options[index].Selected.Invoke();
                        index = 0;
                    }

                }
            }
            while (true);
        }


        static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(option.Name);
            }
            Console.ResetColor();
        }

        public async static Task CreateSingularColorMenu(List<Option> options)
        {
            int index = 0;
            ConsoleKeyInfo keyinfo;
            do
            {
                WriteColorMenu(options, options[index]);
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteColorMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteColorMenu(options, options[index]);
                    }
                }

                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    if (index == options.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        await options[index].Selected.Invoke();
                        break;
                    }

                }
            }
            while (true);
        }

        public static void WriteColorMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();
            string prefix;
            foreach (Option option in options)
            {
                switch (option.Name)
                {
                    case "Red":
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    case "Blue":
                        Console.BackgroundColor = ConsoleColor.Blue;
                        break;
                    case "Green":
                        Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    case "Yellow":
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        break;
                    case "Black":
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case "White":
                        Console.BackgroundColor = ConsoleColor.White;
                        break;
                    case "Gray":
                        Console.BackgroundColor = ConsoleColor.Gray;
                        break;
                    case "Brown":
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "Pink":
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        break;
                    case "Purple":
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case "Orange":
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;
                    default:
                        break;
                }
                if (option == selectedOption)
                {
                    prefix= ">";
                }
                else
                {
                    prefix = " ";
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(prefix+option.Name);
            }
            Console.ResetColor();
        }

        public static string ConvertImageToAscii(byte[] imageBytes, int width)
        {
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes))
            {

                int height = (int)Math.Ceiling((double)width * image.Height / image.Width);

                image.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Max
                }));

                string asciiChars = "@%#*+=-:. ";
                int totalChars = asciiChars.Length;

                var result = new char[width * height + height];
                int index = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < image.Width && y < image.Height)
                        {
                            var pixel = image[x, y];
                            int brightness = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

                            int charIndex = brightness * (totalChars - 1) / 255;
                            result[index++] = asciiChars[charIndex];
                        }
                    }

                    if (y < height - 1)
                        result[index++] = '\n';
                }

                return new string(result);
            }
        }

    }
}
