using System.Collections.Generic;
using Helion.Core.Util.Parser.Tokens;

namespace Helion.Core.Util.Parser.Preprocessor
{
    /// <summary>
    /// A preprocessor for tokens.
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Takes a list of unprocessed tokens and handles any include
        /// directives.
        /// </summary>
        /// <param name="unprocessedTokens">The tokens to handle.</param>
        /// <returns>A list of tokens, or an empty value if they could not be
        /// handled or there was infinite recursion.</returns>
        Optional<List<Token>> Process(List<Token> unprocessedTokens);
    }
}
