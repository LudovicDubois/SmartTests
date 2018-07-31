using System;
using System.Text;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class LongType : NumericType<long, LongType>
    {
        /// <inheritdoc />
        protected override long MinValue => long.MinValue;
        /// <inheritdoc />
        protected override long MaxValue => long.MaxValue;

        /// <inheritdoc />
        protected override long GetPrevious(long n) => n - 1;
        /// <inheritdoc />
        protected override long GetNext(long n) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue(out long value)
        {
            // Ensure values are well distributed
            var max = long.MinValue;
            foreach (var chunk in Chunks)
                max += chunk.Max - chunk.Min;
            var random = new Random();

            value = random.NextLong(long.MinValue, max);
            max = long.MinValue;
            foreach (var chunk in Chunks)
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if (value > max)
                    continue;
                value = value - min + chunk.Min;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        private static string ToString(long n)
        {
            if (n == long.MinValue)
                return "long.MinValue";
            if (n == long.MaxValue)
                return "long.MaxValue";
            return n.ToString();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder("Long");
            foreach (var chunk in Chunks)
                result.Append($".Range({ToString(chunk.Min)}, {ToString(chunk.Max)})");
            return result.ToString();
        }
    }
}