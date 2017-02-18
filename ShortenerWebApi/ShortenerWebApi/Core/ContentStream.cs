using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShortenerWebApi.Core
{
    /// <summary>
    /// A class to wrap a stream for interception purposes
    /// and recording the number of bytes written to or read from
    /// the wrapped stream.
    /// </summary>
    public class ContentStream : Stream
    {
        protected readonly Stream Buffer;
        protected readonly Stream Stream;

        private long _contentLength;

        /// <summary>
        /// Initialize a new instance of the <see cref="ContentStream"/> class.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="stream"></param>
        public ContentStream(Stream buffer, Stream stream)
        {
            Buffer = buffer;
            Stream = stream;
        }

        /// <summary>
        /// Returns the recorded length of the underlying stream.
        /// </summary>
        public virtual long ContentLength
        {
            get { return _contentLength; }
        }

        /// <summary>
        /// Reads the content of the stream as a string.
        /// 
        /// If the contentType is not specified (null) or does not
        /// refer to a string, this function returns the content type
        /// followed by the number of bytes in the response.
        /// 
        /// If the contentType is one of the following values, the
        /// resulting content is decoded as a string.
        /// </summary>
        /// <param name="contentType">HTTP header content type.</param>
        /// <returns></returns>
        public async Task<string> ReadContentAsync(string contentType)
        {
            if (!IsTextContentType(contentType))
            {
                contentType = string.IsNullOrEmpty(contentType) ? "N/A" : contentType;
                return string.Format("{0} [{1} bytes]", contentType, ContentLength);
            }

            Buffer.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[ContentLength];
            var count = await Buffer.ReadAsync(buffer, 0, buffer.Length);

            return GetEncoding(contentType).GetString(buffer, 0, count);
        }

        protected void WriteContent(byte[] buffer, int offset, int count)
        {
            Buffer.Write(buffer, offset, count);
        }

        #region --- Implementation ---

        private static bool IsTextContentType(string contentType)
        {
            if (contentType == null)
                return false;

            var isTextContentType =
                contentType.StartsWith("application/json") ||
                contentType.StartsWith("application/xml") ||
                contentType.StartsWith("text/");

            return isTextContentType;
        }

        private static Encoding GetEncoding(string contentType)
        {
            var charset = "utf-8";
            var regex = new Regex(@";\s*charset=(?<charset>[^\s;]+)");
            var match = regex.Match(contentType);
            if (match.Success)
                charset = match.Groups["charset"].Value;

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch (ArgumentException)
            {
                return Encoding.UTF8;
            }
        }

        #endregion

        #region --- System.IO.Stream Overrides ---

        public override bool CanRead
        {
            get { return Stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return Stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return Stream.CanWrite; }
        }

        public override void Flush()
        {
            Stream.Flush();
        }

        public override long Length
        {
            get { return Stream.Length; }
        }

        public override long Position
        {
            get { return Stream.Position; }
            set { Stream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // read content from the request stream
            count = Stream.Read(buffer, offset, count);
            _contentLength += count;

            // record the read content into our temporary buffer
            if (count != 0)
                WriteContent(buffer, offset, count);

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            Stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // store the bytes into our local stream
            WriteContent(buffer, 0, count);

            // write the bytes to the response stream
            // and record the actual number of bytes written
            Stream.Write(buffer, offset, count);
            _contentLength += count;
        }

        #endregion

        #region --- IDisposable Implementation ---

        protected override void Dispose(bool disposing)
        {
            Buffer.Dispose();
        }

        #endregion
    }
}
