using System;
using System.Buffers;

namespace LightJson.Generator;

internal ref struct ValueStringBuilder 
{
    private int bufferPosition = 0;
    private Span<char> buffer;
    private char[] arrayFromPool;

    public readonly int Length => bufferPosition;
    public ref char this[int index] => ref buffer[index];

    public override string ToString()
    {
        return new string(buffer[..bufferPosition]);
    }

    public void Append(char c) 
    {
        if (bufferPosition == buffer.Length - 1)    
            Grow(0);

        buffer[bufferPosition++] = c;
    }

    public void Append(ReadOnlySpan<char> str) 
    {
        var newSize = str.Length + bufferPosition;
        if (newSize > buffer.Length)
            Grow(newSize * 2);
        str.CopyTo(buffer[(bufferPosition)..]);
        bufferPosition += str.Length;
    }

    public void AppendLine(ReadOnlySpan<char> str) 
    {
        Append(str);
        Append("\n");
    }

    private void Grow(int capacity) 
    {
        var currentSize = buffer.Length;
        var newSize = capacity > 0 ? capacity : currentSize * 2;
        var rented = ArrayPool<char>.Shared.Rent(newSize);

        buffer.CopyTo(rented);
        var oldBufferFromPool = arrayFromPool;
        buffer = arrayFromPool = rented;

        if (oldBufferFromPool != null)
            ArrayPool<char>.Shared.Return(oldBufferFromPool);
    }

    public readonly void Dispose() 
    {
        if (arrayFromPool != null) 
            ArrayPool<char>.Shared.Return(arrayFromPool);
    }

    public ValueStringBuilder() 
    {
        buffer = new char[32];
    }

    public ValueStringBuilder(int buffer) 
    {
        this.buffer = new char[buffer];
    }
}