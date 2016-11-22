using System;
using System.IO;
using System.Text;

namespace less
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);

            Console.CursorVisible = true;

            ConsoleKeyInfo keyInfo;

            Status status = new Status();

            do
            {
                PrintPage(lines, status);

                status.SavePrevLine();

                keyInfo = Console.ReadKey(true);

                if (!status.InGoToMode)
                {
                    if (keyInfo.KeyChar == ':')
                    {
                        status.InGoToMode = true;
                    }
                }

                switch (keyInfo.Key)
                {
                    case ConsoleKey.K:
                    case ConsoleKey.UpArrow:
                        --status.CurrentLine; break;
                    case ConsoleKey.J:
                    case ConsoleKey.DownArrow:
                        ++status.CurrentLine; break;
                    case ConsoleKey.PageUp:
                        status.CurrentLine -= Console.WindowHeight -1 ; break;
                    case ConsoleKey.PageDown:
                        status.CurrentLine += Console.WindowHeight -1; break;
                    case ConsoleKey.Enter:
                        status.InGoToMode = false;
                        int line;
                        if (int.TryParse(status.GoToText, out line))
                        {
                            status.CurrentLine = line;
                        }
                        status.GoToText = string.Empty;
                        break;
                    default:
                        if (status.InGoToMode)
                        {
                            int boo;
                            if (int.TryParse(keyInfo.KeyChar.ToString(), out boo))
                            {
                                status.GoToText += keyInfo.KeyChar;
                            }

                            if (keyInfo.Key == ConsoleKey.Backspace)
                            {
                                status.GoToText = status.GoToText.Remove(status.GoToText.Length - 1);
                            }
                        }
                        break;
                }

                if (status.CurrentLine < 0) status.CurrentLine = 0;
                if (status.CurrentLine == lines.Length) status.CurrentLine = lines.Length - 1;
                if ((status.CurrentLine + Console.WindowHeight) > lines.Length)
                    status.CurrentLine = lines.Length - Console.WindowHeight +1;

            } while (keyInfo.Key != ConsoleKey.Escape && keyInfo.Key != ConsoleKey.Q);
        }

        static void PrintPage(string[] lines, Status status)
        {
            PrintLines(lines, status);
            PrintCommandBar(status);
        }

        static void PrintLines(string[] lines, Status status)
        {
            if (!status.LineChanged())
                return;

            StringBuilder builder = new StringBuilder();

            for (int i = status.CurrentLine; i < status.CurrentLine + Console.WindowHeight - 1; ++i)
            {
                string lineInfo = string.Format("{0,4}", i);

                builder.AppendLine(
                    lineInfo +
                    lines[i] + " ".PadLeft(Console.WindowWidth -
                        lines[i].Length - 1 - lineInfo.Length));
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(builder.ToString());
        }

        static void PrintCommandBar(Status status)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);

            StringBuilder builder = new StringBuilder();
            builder.Append("$$");

            if (status.InGoToMode)
            {
                builder.Append(" : " + status.GoToText);
            }

            builder.Append(" ".PadLeft(Console.WindowWidth - builder.Length -1));

            Console.Write(builder.ToString());
        }

        class Status
        {
            internal bool InGoToMode = false;
            internal string GoToText = string.Empty;

            internal int CurrentLine = 0;
            internal int PrevLine = -1;

            internal bool LineChanged()
            {
                return PrevLine != CurrentLine;
            }

            internal void SavePrevLine()
            {
                PrevLine = CurrentLine;
            }
        }
    }
}
