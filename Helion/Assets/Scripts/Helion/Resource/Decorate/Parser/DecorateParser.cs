using System.Collections.Generic;
using System.Text;
using Helion.Archives;
using Helion.Resource.Decorate.Definitions;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Logging;
using Helion.Util.Parser;
using Helion.Util.Parser.Preprocessor;
using static Helion.Util.OptionalHelper;

namespace Helion.Resource.Decorate.Parser
{
    /// <summary>
    /// Parses decorate text into definitions.
    /// </summary>
    public partial class DecorateParser : ParserBase
    {
        private static readonly Log Log = LogManager.Instance();

        public readonly List<ActorDefinition> Definitions = new List<ActorDefinition>();
        private readonly Dictionary<UpperString, ActorDefinition> nameToDefinition = new Dictionary<UpperString, ActorDefinition>();
        private ActorDefinition currentDefinition = new ActorDefinition("", Empty);

        public DecorateParser(IArchive archive) : base(CreateIncludePreprocessorWith(archive))
        {
        }

        protected override void PerformParsing()
        {
            while (!Done)
            {
                if (Peek("const"))
                    ConsumeVariable();
                else if (Peek("enum"))
                    ConsumeEnum();
                else
                    ConsumeActorDefinition();
            }
        }

        private static IncludePreprocessor CreateIncludePreprocessorWith(IArchive archive)
        {
            return new IncludePreprocessor(path =>
            {
                return archive.FindPath(path).Map(entry => Encoding.UTF8.GetString(entry.Data));
            });
        }

        private void ConsumeVariable()
        {
            throw MakeException("Variables not supported in decorate currently");
        }

        private void ConsumeEnum()
        {
            throw MakeException("Enums not supported in decorate currently");
        }

        private Optional<ActorDefinition> LookupActor(UpperString name)
        {
            Optional<ActorDefinition> parsedActor = nameToDefinition.Find(name);
            return parsedActor ? parsedActor : DecorateManager.Find(name);
        }

        private void ConsumeActorDefinition()
        {
            Consume("actor");
            currentDefinition = ConsumeActorHeader();
            Consume('{');
            InvokeUntilAndConsume('}', ConsumeActorBodyComponent);

            Definitions.Add(currentDefinition);
            nameToDefinition[currentDefinition.Name] = currentDefinition;
        }

        private ActorDefinition ConsumeActorHeader()
        {
            UpperString name = ConsumeString();

            ActorDefinition parent = DecorateManager.BaseActorDefinition;
            if (ConsumeIf(':'))
            {
                UpperString parentName = ConsumeString();
                Optional<ActorDefinition> parentOpt = LookupActor(parentName);
                if (parentOpt)
                    parent = parentOpt.Value;
                else
                    throw MakeException($"Unable to find parent {parentName} for actor {name}");
            }

            if (ConsumeIf("replaces"))
            {
                ConsumeString();
                throw MakeException("Unsupported 'replaces' keyword temporarily!");
            }

            int? editorId = ConsumeIfInt();

            return new ActorDefinition(name, parent, editorId);
        }

        private void ConsumeActorBodyComponent()
        {
            if (Peek('+') || Peek('-'))
                ConsumeActorFlag();
            else if (ConsumeIf("states"))
                ConsumeActorStates();
            else
                ConsumeActorPropertyOrCombo();
        }
    }
}
