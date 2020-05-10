using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    public class FileInfo : IInfo
    {
        public string Text { get; }
        public int Line { get; }
        public int Column { get; }

        public FileInfo(string text, int line, int column)
        {
            Text = text;
            Line = line;
            Column = column;
        }
    }
}
