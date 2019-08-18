using System;
using System.Diagnostics;
using System.Reflection;
using Sync.Tools;

namespace OsuLivePlayer.WPF
{
    internal class Logger
    {
        public static void LogInfo(string text) => InternalWriteColor(text, ConsoleColor.Blue, 2);

        public static void LogSuccess(string text) => InternalWriteColor(text, ConsoleColor.Green, 2);

        public static void LogError(string text) => InternalWriteColor(text, ConsoleColor.Red, 2);

        public static void WriteColor(string text, ConsoleColor color, bool useNewLine = true, bool useTime = true) =>
            InternalWriteColor(text, color, 2, useNewLine, useTime);

        public static string GetSource() => InternalGetSource(1);

        private static void InternalWriteColor(string text, ConsoleColor color, int offset, bool useNewLine = true,
            bool useTime = true) =>
            IO.CurrentIO.WriteColor(InternalGetSource(offset) + text, color, useNewLine, useTime);

        private static string InternalGetSource(int offset)
        {
            StackTrace st = new StackTrace(true);
            MethodBase mb = st.GetFrame(offset).GetMethod();
            return $"[OsuLivePlayer.{mb.DeclaringType?.Name}] ";
        }

        public static void LogDebug(string text)
        {
#if DEBUG
            InternalWriteColor(text, ConsoleColor.Cyan, 2);
#endif
        }
    }
}
