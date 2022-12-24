//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.IO
{
    /// <summary>
    /// A SubStream restricts access to a specific area of a stream and
    /// provides relative positioning in that area.
    /// </summary>
    public class SubStream : Stream
    {
        private Stream _stream;
        private long _position;
        private int _length;

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
}
