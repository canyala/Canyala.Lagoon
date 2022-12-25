//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//------------------------------------------------------------------------------- 

namespace Canyala.Lagoon.IO;

/// <summary>
/// A SubStream restricts access to a specific area of a stream and
/// provides relative positioning in that area.
/// </summary>
public class SubStream : Stream
{
    private readonly Stream _stream;
    private readonly long _position;
    private readonly int _length;

    public SubStream(Stream stream, long position, int length)
    {
        _stream = stream;
        _stream.Position = _position = position;
        _length = length;
    }

    public override bool CanRead
    { get { return _stream.CanRead; } }

    public override bool CanSeek
    { get { return _stream.CanSeek; } }

    public override bool CanWrite
    { get { return _stream.CanWrite; } }

    public override void Flush()
    { _stream.Flush(); }

    public override long Length
    { get { return _length; } }

    public override long Position
    {
        get { return _stream.Position - _position; }

        set
        {
            if (value < 0 || value > _length)
                throw new IndexOutOfRangeException();

            _stream.Position = _position + value;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (Position + count > _length)
            throw new IndexOutOfRangeException();

        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
            offset = _position + offset;
        else if (origin == SeekOrigin.End)
            offset = _position + _length - offset;

        return _position - _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        throw new InvalidOperationException("SubStreams have a fixed length.");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (Position + count > _length)
            throw new IndexOutOfRangeException();

        _stream.Write(buffer, offset, count);
    }
}
