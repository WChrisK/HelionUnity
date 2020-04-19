namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public struct RunSpeed
    {
        public readonly float Run;
        public readonly float Walk;

        public RunSpeed(float run, float walk)
        {
            Run = run;
            Walk = walk;
        }
    }
}
