using System;
using System.Collections.Generic;
using System.Text;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Parser.Tokens;

namespace Helion.Core.Util.Parser
{
    /// <summary>
    /// An exception for when parsing fails.
    /// </summary>
    public class ParserException : Exception
    {
        /// <summary>
        /// The token that the parser errored out at, if applicable. Can be
        /// null if it happened out of range of any tokens.
        /// </summary>
        public Token? Token;

        /// <summary>
        /// The line that triggered the problem.
        /// </summary>
        public string OffendingLine { get; private set; }

        /// <summary>
        /// A caret offset that can be printed under the offending line to help
        /// visually see what the offending token was.
        /// </summary>
        public string CaretVisualizer { get; private set; }

        public ParserException(int tokenIndex, List<Token> tokens, string message) :
            base(message)
        {
            if (tokenIndex < 0 && tokenIndex >= tokens.Count)
                return;

            Token = tokens[tokenIndex];
            GenerateHelperText(tokenIndex, tokens);
        }

        public ParserException(int lineNumber, int lineCharOffset, string reason) :
            base($"Error at line {lineNumber} offset {lineCharOffset}: {reason}")
        {
            OffendingLine = "";
            CaretVisualizer = "";
        }

        private void GenerateHelperText(int tokenIndex, List<Token> tokens)
        {
            Token token = tokens[tokenIndex];
            int lineNumber = token.LineNumber;

            int startIndexInclusive = tokenIndex;
            int endIndexInclusive = tokenIndex;

            for (int i = tokenIndex - 1; i >= 0; i--)
            {
                if (tokens[i].LineNumber != lineNumber)
                    break;

                startIndexInclusive = i;
            }

            for (int i = tokenIndex + 1; i < tokens.Count; i++)
            {
                if (tokens[i].LineNumber != lineNumber)
                    break;

                endIndexInclusive = i;
            }

            OffendingLine = ConstructLine(tokens, startIndexInclusive, endIndexInclusive);
            CaretVisualizer = new string(' ', token.LineCharOffset) + "^";
        }

        private static string ConstructLine(List<Token> tokens, int startInclusive, int endInclusive)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = startInclusive; i <= endInclusive; i++)
            {
                Token token = tokens[i];

                while (builder.Length < token.LineCharOffset)
                    builder.Append(' ');

                if (token.Type == TokenType.QuotedString)
                    builder.Append($"\"{token.Text}\"");
                else
                    builder.Append(token.Text);
            }

            return builder.ToString();
        }
    }
}
