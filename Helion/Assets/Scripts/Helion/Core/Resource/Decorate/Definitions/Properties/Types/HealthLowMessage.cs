using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class HealthLowMessage
    {
        public int? Value;
        public Optional<string> Message = new Optional<string>();

        public HealthLowMessage()
        {
        }

        public HealthLowMessage(HealthLowMessage other)
        {
            Value = other.Value;
            Message = new Optional<string>(Message.Value);
        }

        public void Set(int value, string message)
        {
            Value = value;
            Message = message;
        }
    }
}
