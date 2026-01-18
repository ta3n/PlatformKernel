namespace Liberty.Media.Models;

/// <summary>
/// Represents a stream that provides read access to a specific segment of an underlying stream.
/// </summary>
/// <remarks>
/// The <see cref="SegmentedStream"/> allows reading from a specific range within the underlying stream,
/// defined by a starting position and a fixed length. Once the specified segment is exhausted, no further
/// reading is allowed. Seeking, writing, and other operations beyond reading are not supported.
/// </remarks>
/// <seealso cref="System.IO.Stream"/>
public class SegmentedStream : Stream
{
    /// <summary>
    /// Represents the underlying base <see cref="Stream"/> on which this segmented stream operates.
    /// This stream is used to read data starting from a specified position for a specified length,
    /// providing a subset view of the data in the base stream.
    /// </summary>
    private readonly Stream _baseStream;

    /// <summary>
    /// Represents the length of the segmented stream in bytes.
    /// This value specifies the maximum number of bytes that can
    /// be read from the underlying stream in this segment.
    /// </summary>
    private readonly long _length;

    /// <summary>
    /// Represents the current position within the <see cref="SegmentedStream"/>.
    /// This field tracks the number of bytes read relative to the segmented stream's start.
    /// Value is updated after each read operation to reflect the progress within the stream.
    /// </summary>
    /// <remarks>
    /// The <c>_position</c> field is used internally within the <see cref="SegmentedStream"/>
    /// to manage the position, ensuring it does not exceed the segment's defined length.
    /// </remarks>
    private long _position;

    /// <summary>
    /// Represents a stream that is a portion of another stream. This class limits read access
    /// to a specific segment of the base stream.
    /// </summary>
    public SegmentedStream(
        Stream baseStream,
        long start,
        long length
    )
    {
        _baseStream = baseStream;
        _length = length;
        _position = 0;

        _baseStream.Seek(start, SeekOrigin.Begin);
    }

    /// <summary>
    /// Gets a value indicating whether the current stream supports reading.
    /// </summary>
    /// <remarks>
    /// This property reflects the readability of the underlying base stream. If the base
    /// stream supports reading, this property returns <c>true</c>; otherwise, it returns <c>false</c>.
    /// </remarks>
    /// <value>
    /// <c>true</c> if the stream supports reading; <c>false</c> otherwise.
    /// </value>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the underlying stream has been disposed at the time of access.
    /// </exception>
    public override bool CanRead => _baseStream.CanRead;

    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking.
    /// </summary>
    /// <remarks>
    /// The <see cref="CanSeek"/> property always returns <c>false</c> for instances of <see cref="SegmentedStream"/>.
    /// Seeking is not supported in this implementation.
    /// Attempting to use seek-related functionality, such as <see cref="Seek"/>, will result in a <see cref="NotSupportedException"/>.
    /// </remarks>
    public override bool CanSeek => false;

    /// <summary>
    /// Gets a value indicating whether the current stream supports writing.
    /// </summary>
    /// <remarks>
    /// For the <see cref="SegmentedStream"/>, this property always returns <c>false</c>
    /// as writing is not supported for this stream implementation.
    /// </remarks>
    /// <value>
    /// <c>false</c>, indicating write operations are not supported.
    /// </value>
    public override bool CanWrite => false;

    /// <summary>
    /// Gets the total length of the segmented stream. This value represents the fixed size
    /// of the segment defined during the creation of the <see cref="SegmentedStream"/>.
    /// <remarks>
    /// This property returns the maximum number of bytes that can be read from the segment.
    /// It does not modify the position of the underlying base stream or the current position
    /// within the segmented stream.
    /// </remarks>
    /// </summary>
    /// <value>
    /// The length of the segmented stream, measured in bytes.
    /// </value>
    /// <exception cref="NotSupportedException">
    /// Thrown if an attempt is made to set this property, as the stream length is immutable.
    /// </exception>
    public override long Length => _length;

    /// <summary>
    /// Gets or sets the current position within the <see cref="SegmentedStream"/>.
    /// </summary>
    /// <remarks>
    /// This property reflects the current read position within the segmented portion
    /// of the base stream. The position starts at zero and increases with each read operation.
    /// Setting the position is not supported and will throw a <see cref="NotSupportedException"/>.
    /// When attempting to set the position, consider using the underlying base stream directly
    /// for custom seeking operations, if needed.
    /// The value of this property is automatically updated as data is read via the
    /// <see cref="Stream.Read(byte[], int, int)"/> or its asynchronous counterparts.
    /// </remarks>
    /// <exception cref="NotSupportedException">Thrown when setting this property.</exception>
    /// <value>
    /// The current position within the segmented portion of the stream. The value
    /// represents the number of bytes read from the segmented stream relative to its start.
    /// </value>
    public override long Position { get => _position; set => throw new NotSupportedException(); }

    /// <summary>
    /// Reads a block of bytes from the current segmented stream and writes the data to a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to which the data will be written.</param>
    /// <param name="offset">The zero-based byte offset in the buffer at which to begin storing the data read from the stream.</param>
    /// <param name="count">The maximum number of bytes to read from the stream.</param>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number of bytes requested
    /// if that many bytes are not currently available, or zero if the end of the segment has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
    /// <exception cref="ArgumentException">Thrown if the sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the underlying stream has been disposed.</exception>
    public override int Read(
        byte[] buffer,
        int offset,
        int count
    )
    {
        var remaining = _length - _position;
        if (remaining <= 0)
        {
            return 0;
        }

        var readCount = (int)Math.Min(count, remaining);
        var read = _baseStream.Read(buffer, offset, readCount);
        _position += read;
        return read;
    }

    /// <summary>
    /// Asynchronously reads a sequence of bytes from the segmented stream and advances the position
    /// within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">The region of memory to write the data read from the stream into.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The total number of bytes read into the buffer. This might be less than the number of bytes requested
    /// if the end of the segmented stream is reached or if fewer bytes are available.
    /// </returns>
    /// <remarks>
    /// This method reads only within the boundaries of the segment defined for the stream, ensuring data beyond
    /// the segment is not accessed.
    /// </remarks>
    public override async ValueTask<int> ReadAsync(
        Memory<byte> buffer,
        CancellationToken cancellationToken = default
    )
    {
        var remaining = _length - _position;
        if (remaining <= 0)
        {
            return 0;
        }

        var readCount = (int)Math.Min(buffer.Length, remaining);
        var read = await _baseStream.ReadAsync(
            buffer[..readCount],
            cancellationToken
        );
        _position += read;
        return read;
    }

    /// <summary>
    /// Asynchronously reads a sequence of bytes from the current segmented stream and advances the position within the stream
    /// by the number of bytes read.
    /// </summary>
    /// <param name="buffer">
    /// The buffer to write the data into. It must have enough size to hold the number of bytes specified by <paramref name="count"/>.
    /// </param>
    /// <param name="offset">
    /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.
    /// </param>
    /// <param name="count">
    /// The maximum number of bytes to read from the current stream.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous read operation. The value of the task's result is the total number of bytes read into the buffer.
    /// The result can be less than the number of bytes requested if the end of the segmented stream is reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="buffer"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> or <paramref name="count"/> is invalid for the given <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when the base stream associated with the segmented stream has been disposed.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the stream is not readable.
    /// </exception>
    public override async Task<int> ReadAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken
    )
    {
        var remaining = _length - _position;
        if (remaining <= 0)
        {
            return 0;
        }

        var readCount = (int)Math.Min(count, remaining);
        var read = await _baseStream.ReadAsync(
            buffer.AsMemory(offset, readCount),
            cancellationToken
        );
        _position += read;
        return read;
    }

    /// <summary>
    /// Flushes the underlying stream. This operation is not supported for <see cref="SegmentedStream"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown in all cases because the <see cref="SegmentedStream"/> does not support flushing.
    /// </exception>
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Seeks to the specified position in the segmented stream. This operation is not supported
    /// by the <see cref="SegmentedStream"/> class and always throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="offset">The offset to seek to relative to the position indicated by <paramref name="origin"/>.</param>
    /// <param name="origin">Specifies the reference point for seeking within the stream.</param>
    /// <returns>This method does not return a value because it always throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Always thrown because seeking is not supported.</exception>
    public override long Seek(
        long offset,
        SeekOrigin origin
    )
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Sets the length of the current stream. This method is not supported
    /// for <see cref="SegmentedStream"/> and will always throw a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="value">The desired length of the stream in bytes. This parameter is ignored as the operation is not supported.</param>
    /// <exception cref="NotSupportedException">Always thrown to indicate that the operation is not supported.</exception>
    public override void SetLength(
        long value
    )
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Represents a stream that segments a given base stream by specifying a start position and length.
    /// Allows reading from a specific segment of the base stream while preventing actions like writing or seeking.
    /// </summary>
    public override void Write(
        byte[] buffer,
        int offset,
        int count
    )
    {
        throw new NotSupportedException();
    }
}
