using System;

namespace SetOperations
{
    public class InputExceptions : Exception
    {
        public InputExceptions()
        {
        }

        public InputExceptions(string message, string messagePrefix = "Malformed input")
            : base($"{messagePrefix}: {message}")
        {
        }

        public InputExceptions(string message, Exception inner)
            : base(message, inner)
        {
        }

        public InputExceptions(string message, int atLine, int atCol, string messagePrefix = "Malformed input - Unexpected character")
            : base($"{messagePrefix}\nat line {atLine}, col {atCol}:\n{message}")
        {
        }

        public InputExceptions(int atLine, int atCol, string messagePrefix = "Malformed input - Unexpected character")
            : base($"{messagePrefix}\nat line {atLine}, col {atCol}")
        {
        }

        protected InputExceptions(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
