using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    /// <summary>
    /// Defines a MemoryStream that does not sit on the Large Object Heap, thus avoiding memory fragmentation.
    /// </summary>
    public class ChunkedMemoryStream : Stream
    {
        /// <summary>
        /// Defines the default chunk size. Currently defined as 0x10000.
        /// </summary>
        public const int DefaultChunkSize = 0x10000; // needs to be < 85000

        private List<byte[]> _chunks = new List<byte[]>();
        private long _position;
        private int _chunkSize;
        private int _lastChunkPos;
        private int _lastChunkPosIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class.
        /// </summary>
        public ChunkedMemoryStream()
            : this(DefaultChunkSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class.
        /// </summary>
        /// <param name="chunkSize">Size of the underlying chunks.</param>
        public ChunkedMemoryStream(int chunkSize)
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class based on the specified byte array.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create the current stream.</param>
        public ChunkedMemoryStream(byte[] buffer)
            : this(DefaultChunkSize, buffer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class based on the specified byte array.
        /// </summary>
        /// <param name="chunkSize">Size of the underlying chunks.</param>
        /// <param name="buffer">The array of unsigned bytes from which to create the current stream.</param>
        public ChunkedMemoryStream(int chunkSize, byte[] buffer)
        {
            FreeOnDispose = true;
            ChunkSize = chunkSize;
            _chunks.Add(new byte[chunkSize]);
            if (buffer != null)
            {
                Write(buffer, 0, buffer.Length);
                Position = 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to free the underlying chunks on dispose.
        /// </summary>
        /// <value><c>true</c> if [free on dispose]; otherwise, <c>false</c>.</value>
        public bool FreeOnDispose { get; set; }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (FreeOnDispose)
            {
                if (_chunks != null)
                {
                    _chunks = null;
                    _chunkSize = 0;
                    _position = 0;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// This implementation does nothing.
        /// </summary>
        public override void Flush()
        {
            // do nothing
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if ((buffer.Length - offset) < count)
                throw new ArgumentException(null, "count");

            CheckDisposed();

            int chunkIndex = (int)(_position / ChunkSize);
            if (chunkIndex == _chunks.Count)
                return 0;

            int chunkPos = (int)(_position % ChunkSize);
            count = (int)Math.Min(count, Length - _position);
            if (count == 0)
                return 0;

            int left = count;
            int inOffset = offset;
            int total = 0;
            do
            {
                int toCopy = Math.Min(left, ChunkSize - chunkPos);
                Buffer.BlockCopy(_chunks[chunkIndex], chunkPos, buffer, inOffset, toCopy);
                inOffset += toCopy;
                left -= toCopy;
                total += toCopy;
                if ((chunkPos + toCopy) == ChunkSize)
                {
                    if (chunkIndex == (_chunks.Count - 1))
                    {
                        // last chunk
                        break;
                    }
                    chunkPos = 0;
                    chunkIndex++;
                }
                else
                {
                    chunkPos += toCopy;
                }
            }
            while (left > 0);
            _position += total;
            return total;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override int ReadByte()
        {
            CheckDisposed();
            if (_position >= Length)
                return -1;

            byte b = _chunks[(int)(_position / ChunkSize)][_position % ChunkSize];
            _position++;
            return b;
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;

                case SeekOrigin.Current:
                    Position += offset;
                    break;

                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }
            return Position;
        }

        private void CheckDisposed()
        {
            if (_chunks == null)
                throw new ObjectDisposedException(null, "Cannot access a disposed stream");
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            CheckDisposed();
            if (value < 0)
                throw new ArgumentOutOfRangeException("value");

            if (value > Length)
                throw new ArgumentOutOfRangeException("value");

            long needed = value / ChunkSize;
            if ((value % ChunkSize) != 0)
            {
                needed++;
            }

            if (needed > int.MaxValue)
                throw new ArgumentOutOfRangeException("value");

            if (needed < _chunks.Count)
            {
                int remove = (int)(_chunks.Count - needed);
                for (int i = 0; i < remove; i++)
                {
                    _chunks.RemoveAt(_chunks.Count - 1);
                }
            }
            _lastChunkPos = (int)(value % ChunkSize);
        }

        /// <summary>
        /// Converts the current stream to a byte array.
        /// </summary>
        /// <returns>An array of bytes</returns>
        public byte[] ToArray()
        {
            CheckDisposed();
            byte[] bytes = new byte[Length];
            int offset = 0;
            for (int i = 0; i < _chunks.Count; i++)
            {
                int count = (i == (_chunks.Count - 1)) ? _lastChunkPos : _chunks[i].Length;
                if (count > 0)
                {
                    Buffer.BlockCopy(_chunks[i], 0, bytes, offset, count);
                    offset += count;
                }
            }
            return bytes;
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if ((buffer.Length - offset) < count)
                throw new ArgumentException(null, "count");

            CheckDisposed();

            int chunkPos = (int)(_position % ChunkSize);
            int chunkIndex = (int)(_position / ChunkSize);
            if (chunkIndex == _chunks.Count)
            {
                _chunks.Add(new byte[ChunkSize]);
            }

            int left = count;
            int inOffset = offset;
            do
            {
                int copied = Math.Min(left, ChunkSize - chunkPos);
                Buffer.BlockCopy(buffer, inOffset, _chunks[chunkIndex], chunkPos, copied);
                inOffset += copied;
                left -= copied;
                if ((chunkPos + copied) == ChunkSize)
                {
                    chunkIndex++;
                    chunkPos = 0;
                    if (chunkIndex == _chunks.Count)
                    {
                        _chunks.Add(new byte[ChunkSize]);
                    }
                }
                else
                {
                    chunkPos += copied;
                }
            }
            while (left > 0);
            _position += count;
            if (chunkIndex == (_chunks.Count - 1))
            {
                if ((chunkIndex > _lastChunkPosIndex) ||
                    ((chunkIndex == _lastChunkPosIndex) && (chunkPos > _lastChunkPos)))
                {
                    _lastChunkPos = chunkPos;
                    _lastChunkPosIndex = chunkIndex;
                }
            }
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void WriteByte(byte value)
        {
            CheckDisposed();
            int chunkIndex = (int)(_position / ChunkSize);
            int chunkPos = (int)(_position % ChunkSize);

            if (chunkPos > (ChunkSize - 1)) //changed from (chunkPos >= (ChunkSize - 1))
            {
                chunkIndex++;
                chunkPos = 0;
                if (chunkIndex == _chunks.Count)
                {
                    _chunks.Add(new byte[ChunkSize]);
                }
            }
            _chunks[chunkIndex][chunkPos++] = value;
            _position++;
            if (chunkIndex == (_chunks.Count - 1))
            {
                if ((chunkIndex > _lastChunkPosIndex) ||
                    ((chunkIndex == _lastChunkPosIndex) && (chunkPos > _lastChunkPos)))
                {
                    _lastChunkPos = chunkPos;
                    _lastChunkPosIndex = chunkIndex;
                }
            }
        }

        /// <summary>
        /// Writes to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void WriteTo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            CheckDisposed();
            for (int i = 0; i < _chunks.Count; i++)
            {
                int count = (i == (_chunks.Count - 1)) ? _lastChunkPos : _chunks[i].Length;
                stream.Write(_chunks[i], 0, count);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Length
        {
            get
            {
                CheckDisposed();
                if (_chunks.Count == 0)
                    return 0;

                return (_chunks.Count - 1) * ChunkSize + _lastChunkPos;
            }
        }

        /// <summary>
        /// Gets or sets the size of the underlying chunks. Cannot be greater than or equal to 85000.
        /// </summary>
        /// <value>The chunks size.</value>
        public int ChunkSize
        {
            get
            {
                return _chunkSize;
            }
            set
            {
                if ((value <= 0) || (value >= 85000))
                    throw new ArgumentOutOfRangeException("value");

                _chunkSize = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get
            {
                CheckDisposed();
                return _position;
            }
            set
            {
                CheckDisposed();
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value > Length)
                    throw new ArgumentOutOfRangeException("value");

                _position = value;
            }
        }
    }
}