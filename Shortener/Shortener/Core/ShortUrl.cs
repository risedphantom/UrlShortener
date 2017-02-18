using System;
using System.Linq;

namespace Shortener.Core
{
    /// <summary>
    /// Helper for creating short url
    /// </summary>
    class ShortUrl
    {
        // Encode each rank of simple number with 
        private readonly string[] _alphabet =
        {
            "d8zmiq62c3ryfo57xw9aketpnjs4gb",
            "cg94qnzjsxiro27bpa5w3f8ety6dmk",
            "782irx6ayjet4fkbq5opgsdnc9wmz3",
            "omkrpjzt6qsgx89735eiwdacy2f4nb",
            "fg76k23nyqr4z8mjpsobc5iwaxd9et",
            "bagsc3f9ozrx2mtq8yw4pkid657jne",
            "qndi6pkabs3m284gj7tyzcf9rx5owe",
            "tz42raq3dfpiy7x8gcw695njbkmeos"
        };

        // x30 base
        private const string BASE30 = "0123456789ABCDEFGHIJKLMNOPQRST";

        private readonly string _url;
        private readonly int _int;

        /// <summary>
        /// Initialize object with numeric value
        /// </summary>
        /// <param name="index">number to convert to base30</param>
        public ShortUrl(int index)
        {
            if (index < 0)
                throw new ShortUrlException("Out of range", new ArgumentOutOfRangeException(nameof(index)));

            _int = index;
            _url = Base30Encode(DecToBase30(index));
        }

        /// <summary>
        /// Initialize object with base30 value
        /// </summary>
        /// <param name="url">base30 value to convert to int</param>
        public ShortUrl(string url)
        {
            if (url.Length > _alphabet.Length)
                throw new ShortUrlException("Out of range", new ArgumentOutOfRangeException(nameof(url)));

            if (url.Any(c => _alphabet[0].IndexOf(c) == -1))
                throw new ShortUrlException("Bad value", new ArgumentException("Incorrect value", nameof(url)));

            _url = url;
            _int = Base30ToDec(Base30Decode(url));
        }

        #region --- Main logic ---
        private string DecToBase30(int value)
        {
            var res = "";
            do
            {
                res += BASE30[value%BASE30.Length];
                value /= BASE30.Length;
            } while (value > 0);

            var chars = res.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        private int Base30ToDec(string value)
        {
            if (value.Any(c => BASE30.IndexOf(c) == -1))
                throw new ShortUrlException("Bad value", new ArgumentException("Incorrect value", nameof(value)));

            return value.Select(t => BASE30.IndexOf(t)).Select((k, i) => k*(Pow(BASE30.Length, value.Length - i - 1))).Sum();
        }

        private string Base30Encode(string value)
        {
            if (value.Length > _alphabet.Length)
                throw new ShortUrlException("Out of range", new ArgumentOutOfRangeException(nameof(value)));

            var res = "";
            for (var i = 0; i < value.Length; i++)
                res += _alphabet[i][BASE30.IndexOf(value[i])];

            return res;
        }

        private string Base30Decode(string value)
        {
            if (value.Length > _alphabet.Length)
                throw new ShortUrlException("Out of range", new ArgumentOutOfRangeException(nameof(value)));

            var res = "";
            for (var i = 0; i < value.Length; i++)
                res += BASE30[_alphabet[i].IndexOf(value[i])];

            return res;
        }

        private int Pow(int value, int degree)
        {
            var res = 1;
            for (var i = 0; i < degree; i++)
                res *= value;

            return res;
        }
        #endregion

        #region --- Casting ---
        public long ToInt()
        {
            return _int;
        }

        public override string ToString()
        {
            return _url;
        }
        #endregion
    }

    /// <summary>
    /// Exception class for short url
    /// </summary>
    public class ShortUrlException : Exception
    {
        public ShortUrlException()
        {
        }

        public ShortUrlException(string message)
            : base(message)
        {
        }

        public ShortUrlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
