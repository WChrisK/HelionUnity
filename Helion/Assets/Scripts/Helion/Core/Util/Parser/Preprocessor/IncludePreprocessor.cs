using System;
using System.Collections.Generic;
using Helion.Core.Util.Parser.Tokens;

namespace Helion.Core.Util.Parser.Preprocessor
{
    /// <summary>
    /// A helper class that converts #include directives into tokens.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class IncludePreprocessor : IPreprocessor
    {
        private const int MaxRecursion = 32;

        private int recursionCount;
        private readonly Func<string, Optional<string>> includeLocatorFunc;

        /// <summary>
        /// Creates a new include locator that resolves include locations.
        /// </summary>
        /// <param name="includeLocatorFunction">The function that takes an
        /// include path and reads the text from it or returns an empty value
        /// if the resource cannot be read.</param>
        public IncludePreprocessor(Func<string, Optional<string>> includeLocatorFunction)
        {
            includeLocatorFunc = includeLocatorFunction;
        }

        public Optional<List<Token>> Process(List<Token> unprocessedTokens)
        {
            recursionCount = 0;
            return InternalProcess(unprocessedTokens);
        }

        private Optional<List<Token>> InternalProcess(List<Token> unprocessedTokens)
        {
            List<Token> tokens = new List<Token>(unprocessedTokens.Count);

            bool foundHash = false;
            bool foundInclude = false;

            // We need to track tokens that we may need to add but cannot. For
            // example if we come across `#pragma test`, we want to hold on
            // adding tokens in case we run into an #include... but if we don't
            // then we want to return the tokens to the list.
            List<Token> buffer = new List<Token>();

            foreach (Token token in unprocessedTokens)
            {
                if (token.Type == TokenType.Hash)
                {
                    foundHash = true;
                    buffer.Add(token);
                    continue;
                }

                if (foundHash)
                {
                    if (IsIncludeToken(token))
                    {
                        foundInclude = true;
                        buffer.Add(token);
                        continue;
                    }

                    if (foundInclude)
                    {
                        // We're going to force the preprocessor to treat the
                        // final token as something that is required.
                        if (token.Type != TokenType.QuotedString)
                            return Optional<List<Token>>.Empty();

                        Optional<List<Token>> newTokens = HandleIncludeDirective(token.Text);
                        if (!newTokens)
                            return Optional<List<Token>>.Empty();

                        tokens.AddRange(newTokens.Value);
                        foundHash = false;
                        foundInclude = false;
                        buffer.Clear();
                        continue;
                    }

                    foundHash = false;
                    tokens.AddRange(buffer);
                    buffer.Clear();
                    continue;
                }

                tokens.Add(token);
            }

            // If we run out of tokens while doing inclusions, we want to make
            // sure we don't lose any tokens at the end.
            tokens.AddRange(buffer);

            return tokens;
        }

        private Optional<List<Token>> HandleIncludeDirective(string includeText)
        {
            recursionCount++;
            if (recursionCount > MaxRecursion)
                return Optional<List<Token>>.Empty();

            Optional<string> text = includeLocatorFunc(includeText);
            if (!text)
                return Optional<List<Token>>.Empty();

            List<Token> tokens = Tokenizer.Read(text.Value);
            return Process(tokens);
        }

        private static bool IsIncludeToken(Token token)
        {
            return token.Type == TokenType.String &&
                   token.Text.Equals("include", StringComparison.OrdinalIgnoreCase);
        }
    }
}
