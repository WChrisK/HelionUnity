using System.Collections.Generic;
using System.Linq;

namespace Helion.Core.Resource.MapsNew.Actions
{
    /// <summary>
    /// A collection of the standard five numbers associated with specials.
    /// This can also be used on other things beyond just specials.
    /// </summary>
    public class SpecialArgs
    {
        public const int NumArgs = 5;

        public readonly int[] Args;

        public int Arg0 => Args[0];
        public int Arg1 => Args[1];
        public int Arg2 => Args[2];
        public int Arg3 => Args[3];
        public int Arg4 => Args[4];

        public SpecialArgs(int arg0 = 0, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
        {
            Args = new[] { arg0, arg1, arg2, arg3, arg4 };
        }

        public SpecialArgs(IEnumerable<int> args)
        {
            Args = new[] { 0, 0, 0, 0, 0 };

            int count = 0;
            foreach (int arg in args)
            {
                Args[count++] = arg;

                if (count >= NumArgs)
                    break;
            }
        }

        public SpecialArgs(SpecialArgs other)
        {
            Args = other.Args.ToArray();
        }
    }
}
