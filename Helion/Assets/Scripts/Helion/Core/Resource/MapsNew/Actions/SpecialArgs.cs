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

        public int Arg0;
        public int Arg1;
        public int Arg2;
        public int Arg3;
        public int Arg4;

        public SpecialArgs(int arg0 = 0, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        public SpecialArgs(IEnumerable<int> args) : this()
        {
            int index = 0;
            foreach (int arg in args)
            {
                switch (index)
                {
                case 0:
                    Arg0 = arg;
                    break;
                case 1:
                    Arg1 = arg;
                    break;
                case 2:
                    Arg2 = arg;
                    break;
                case 3:
                    Arg3 = arg;
                    break;
                case 4:
                    Arg4 = arg;
                    break;
                }

                if (index == NumArgs)
                    break;
            }
        }

        public SpecialArgs(SpecialArgs other)
        {
            Arg0 = other.Arg0;
            Arg1 = other.Arg1;
            Arg2 = other.Arg2;
            Arg3 = other.Arg3;
            Arg4 = other.Arg4;
        }
    }
}
