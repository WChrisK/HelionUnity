using System;
using Helion.Resource.Decorate.Definitions;
using Helion.Util.Geometry;
using Helion.Util.Interpolation;
using UnityEngine;

namespace Helion.Worlds.Entities
{
    public class Entity : IDisposable
    {
        public readonly int ID;
        public readonly ActorDefinition Definition;
        public Vector3Interpolation Position;
        public BitAngle Angle;

        public Entity(int id, ActorDefinition definition, Vector3 position, BitAngle angle)
        {
            ID = id;
            Definition = definition;
            Position = new Vector3Interpolation(position);
            Angle = angle;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
