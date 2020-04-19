using System.Collections.Generic;
using System.Text;

namespace Helion.Core.Util.Parser.Tokens
{
    /// <summary>
    /// Supports tokenizing a stream of characters into a list of tokens.
    /// </summary>
    public class Tokenizer
    {
        private readonly List<Token> tokens = new List<Token>();
        private readonly StringBuilder identifierBuilder = new StringBuilder();
        private readonly string text;
        private int lineNumber = 1;
        private int lineCharOffset;
        private int textIndex;

        private bool BuildingIdentifier => identifierBuilder.Length > 0;

        private Tokenizer(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Reads text into a series of tokens.
        /// </summary>
        /// <param name="text">The text to read.</param>
        /// <returns>A list of the tokens.</returns>
        /// <exception cref="ParserException">If there are malformed components
        /// like a string that doesn't have an ending before a new line, or a
        /// floating point number with badly placed periods.
        /// </exception>
        public static List<Token> Read(string text)
        {
            Tokenizer tokenizer = new Tokenizer(text);
            tokenizer.Tokenize();
            return tokenizer.tokens;
        }

        private static bool IsSpace(char c) => c == ' ' || c == '\t' || c == '\r';

        private static bool IsNumber(char c) => c >= '0' && c <= '9';

        private static bool IsPrintableCharacter(char c) => c >= 32 && c <= 126;

        private static bool IsEscapableStringChar(char c) => c == '"' || c == '\\';

        private static bool IsIdentifier(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
        }

        private static bool IsSymbol(char c)
        {
            return (c >= 33 && c <= 47) || (c >= 58 && c <= 64) || (c >= 91 && c <= 96) || (c >= 123 && c <= 126);
        }

        private void ResetLineInfoTrackers()
        {
            // The next iteration will increment it to zero, which is what
            // we want the line character offset to be.
            lineCharOffset = -1;
            lineNumber++;
        }

        private void ReadEscapeCharacterOrThrow(StringBuilder innerStringBuilder)
        {
            if (textIndex + 1 >= text.Length)
                throw new ParserException(lineNumber, lineCharOffset, "Expected character after escaping in a string");

            char nextChar = text[textIndex + 1];
            if (!IsEscapableStringChar(nextChar))
                throw new ParserException(lineNumber, lineCharOffset + 1, "Expecting an escaped quote to follow a backslash in a string");

            innerStringBuilder.Append(nextChar);
            textIndex++;
            lineCharOffset++;
        }

        private void CompleteIdentifierIfAvailable()
        {
            if (!BuildingIdentifier)
                return;

            string identifierText = identifierBuilder.ToString();
            int targetLineCharOffset = this.lineCharOffset - identifierText.Length;
            int targetCharOffset = textIndex - identifierText.Length;

            Token token = new Token(lineNumber, targetLineCharOffset, targetCharOffset, identifierText, TokenType.String);
            tokens.Add(token);

            identifierBuilder.Clear();
        }

        private void AddCompletedQuotedString(StringBuilder builder, int targetLineCharOffset, int targetTextIndex)
        {
            string innerString = builder.ToString();
            Token token = new Token(lineNumber, targetLineCharOffset, targetTextIndex, innerString, TokenType.QuotedString);
            tokens.Add(token);
        }

        private void ConsumeQuotedString()
        {
            int startingLineCharOffset = lineCharOffset;
            int startingTextIndex = textIndex;

            // We're on the opening quote, so move ahead to the starting character.
            textIndex++;
            lineCharOffset++;

            StringBuilder innerBuilder = new StringBuilder();
            for (; textIndex < text.Length; textIndex++, lineCharOffset++)
            {
                char c = text[textIndex];

                if (c == '"')
                {
                    AddCompletedQuotedString(innerBuilder, startingLineCharOffset, startingTextIndex);
                    return;
                }

                if (c == '\\')
                {
                    ReadEscapeCharacterOrThrow(innerBuilder);
                    continue;
                }

                if (IsPrintableCharacter(c))
                    innerBuilder.Append(c);
                else if (c == '\n')
                {
                    const string endingErrorMessage = "Ended line before finding terminating string quotation mark";
                    throw new ParserException(lineNumber, startingLineCharOffset, endingErrorMessage);
                }
            }

            // This is for some exotic case where we run into EOF when trying
            // to complete the quoted string.
            const string errorMessage = "String missing ending quote, found end of text instead";
            throw new ParserException(lineNumber, startingLineCharOffset, errorMessage);
        }

        private void ConsumeNumber()
        {
            bool isFloat = false;
            int startCharOffset = textIndex;
            int startLineCharOffset = lineCharOffset;
            StringBuilder numberBuilder = new StringBuilder();

            while (textIndex < this.text.Length)
            {
                char c = this.text[textIndex];

                if (IsNumber(c))
                    numberBuilder.Append(c);
                else if (c == '.')
                {
                    if (isFloat)
                        throw new ParserException(lineNumber, lineCharOffset, "Decimal number cannot have two decimals");
                    isFloat = true;
                    numberBuilder.Append(c);
                }
                else
                    break;

                textIndex++;
                lineCharOffset++;
            }

            string numberText = numberBuilder.ToString();
            if (numberText.EndsWith("."))
                throw new ParserException(lineNumber, lineCharOffset - 1, "Decimal number cannot end with a period");

            if (isFloat)
            {
                Token floatToken = new Token(lineNumber, startLineCharOffset, startCharOffset, numberText, TokenType.FloatingPoint);
                tokens.Add(floatToken);
            }
            else
            {
                Token intToken = new Token(lineNumber, startLineCharOffset, startCharOffset, numberText, TokenType.Integer);
                tokens.Add(intToken);
            }

            // When we return control to the iteration loop, it'll consume a
            // character and bypass it. Decrementing here makes it so this will
            // not happen.
            textIndex--;
            lineCharOffset--;
        }

        private void ConsumeSlashTokenOrComments()
        {
            if (textIndex + 1 < text.Length)
            {
                char nextChar = text[textIndex + 1];

                if (nextChar == '/')
                {
                    textIndex += 2;
                    lineCharOffset += 2;
                    ConsumeSingleLineComment();
                    return;
                }

                if (nextChar == '*')
                {
                    textIndex += 2;
                    lineCharOffset += 2;
                    ConsumeMultiLineComment();
                    return;
                }
            }

            Token token = new Token(lineNumber, lineCharOffset, textIndex, '/');
            tokens.Add(token);
        }

        private void ConsumeSingleLineComment()
        {
            for (; textIndex < text.Length; textIndex++)
            {
                if (text[textIndex] != '\n')
                    continue;

                ResetLineInfoTrackers();
                return;
            }
        }

        private void ConsumeMultiLineComment()
        {
            // We want to start one ahead so comments like /*/ don't work.
            textIndex++;
            lineCharOffset++;

            // However we can skip \n by moving ahead, so track that data.
            int prevIndex = textIndex - 1;
            if (prevIndex < text.Length && text[prevIndex] == '\n')
                ResetLineInfoTrackers();

            // Note that the way this loop works means that if find EOF first,
            // it is considered okay. This is what zdoom appears to do, so we
            // will have to do it as well for compatibility reasons since the
            // enforcement of requiring a terminating */ will break wads.
            for (; textIndex < text.Length; textIndex++, lineCharOffset++)
            {
                char c = text[textIndex];

                if (c == '\n')
                {
                    ResetLineInfoTrackers();
                    continue;
                }

                // We don't increment here because when we return the control
                // back to the main loop, it'll do the incrementing instead.
                if (c == '/' && text[textIndex - 1] == '*')
                    return;
            }
        }

        private void Tokenize()
        {
            for (textIndex = 0; textIndex < text.Length; textIndex++, lineCharOffset++)
            {
                char c = text[textIndex];

                if (IsIdentifier(c))
                {
                    identifierBuilder.Append(c);
                }
                else if (IsSpace(c))
                {
                    if (BuildingIdentifier)
                        CompleteIdentifierIfAvailable();
                }
                else if (IsNumber(c))
                {
                    if (BuildingIdentifier)
                        identifierBuilder.Append(c);
                    else
                        ConsumeNumber();
                }
                else if (IsSymbol(c))
                {
                    CompleteIdentifierIfAvailable();

                    if (c == '"')
                        ConsumeQuotedString();
                    else if (c == '/')
                        ConsumeSlashTokenOrComments();
                    else
                    {
                        Token token = new Token(lineNumber, lineCharOffset, textIndex, c);
                        tokens.Add(token);
                    }
                }
                else if (c == '\n')
                {
                    CompleteIdentifierIfAvailable();
                    ResetLineInfoTrackers();
                }
            }

            // If there's any lingering uncompleted identifier we are building,
            // we should consume it.
            CompleteIdentifierIfAvailable();
        }
    }
}
