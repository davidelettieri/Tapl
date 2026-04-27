using System.Collections.Generic;
using System.Text;

public sealed class PrettyPrinter
{
    private readonly int _lineWidth;
    private readonly StringBuilder _buffer = new();
    private readonly Stack<Box> _boxes = new();

    private int _column;
    private bool _pendingBreak;
    private int _pendingBreakSpaces;
    private int _pendingBreakOffset;

    private readonly struct Box
    {
        public Box(int startColumn, int indent)
        {
            StartColumn = startColumn;
            Indent = indent;
        }

        public int StartColumn { get; }
        public int Indent { get; }
    }

    public PrettyPrinter(int lineWidth = 80)
    {
        _lineWidth = lineWidth;
    }

    public void Write(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        FlushBreakIfNeeded(text.Length);

        _buffer.Append(text);
        _column += text.Length;
    }

    public void NewLine()
{
    _pendingBreak = false;
    _buffer.AppendLine();
    _column = 0;
}

    public void PrintSpace()
    {
        PrintBreak(1, 0);
    }

    public void OpenHvBox(int indent)
    {
        _boxes.Push(new Box(_column, indent));
    }

    public void CloseBox()
    {
        if (_boxes.Count > 0)
            _boxes.Pop();
    }

    public void PrintBreak(int spaces, int offset)
    {
        _pendingBreak = true;
        _pendingBreakSpaces = spaces;
        _pendingBreakOffset = offset;
    }

    public void Obox0() => OpenHvBox(0);

    public void Obox() => OpenHvBox(2);

    public void Cbox() => CloseBox();

    public void Break() => PrintBreak(0, 0);

    public override string ToString() => _buffer.ToString();

    private void FlushBreakIfNeeded(int nextTokenLength)
    {
        if (!_pendingBreak)
            return;

        _pendingBreak = false;

        var shouldBreak = _boxes.Count > 0 && _column + _pendingBreakSpaces + nextTokenLength > _lineWidth;

        if (!shouldBreak)
        {
            if (_pendingBreakSpaces > 0)
            {
                _buffer.Append(' ', _pendingBreakSpaces);
                _column += _pendingBreakSpaces;
            }

            return;
        }

        var box = _boxes.Peek();
        var indent = box.StartColumn + box.Indent + _pendingBreakOffset;

        _buffer.AppendLine();
        _buffer.Append(' ', indent);
        _column = indent;
    }
}