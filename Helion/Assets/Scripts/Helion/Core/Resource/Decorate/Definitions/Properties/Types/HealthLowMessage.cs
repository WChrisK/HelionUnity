using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class HealthLowMessage
    {
        public int? Value;
        public Optional<UpperString> Message = new Optional<UpperString>();

        public HealthLowMessage()
        {
        }

        public HealthLowMessage(HealthLowMessage other)
        {
            Value = other.Value;
            Message = new Optional<UpperString>(Message.Value);
        }

        public void Set(int value, UpperString message)
        {
            Value = value;
            Message = message;
        }
    }
}
