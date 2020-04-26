namespace Helion.Resource.Decorate.Definitions.Properties.Types
{
    public struct DecorateRange<T> where T : struct
    {
        public readonly T Low;
        public readonly T High;

        public DecorateRange(T low, T high)
        {
            Low = low;
            High = high;
        }
    }
}
