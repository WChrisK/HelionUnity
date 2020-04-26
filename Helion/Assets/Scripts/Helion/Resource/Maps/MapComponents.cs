using Helion.Archives;
using Helion.Resource.Maps;
using Helion.Util;
using static Helion.Util.OptionalHelper;

namespace Helion.Resource.Maps
{
    /// <summary>
    /// A lightweight collection of map components when reading through maps.
    /// </summary>
    public class MapComponents
    {
        public UpperString Name;
        public Optional<IEntry> Marker { get; private set; } = Empty;
        public Optional<IEntry> Vertices { get; private set; } = Empty;
        public Optional<IEntry> Sectors { get; private set; } = Empty;
        public Optional<IEntry> Sidedefs { get; private set; } = Empty;
        public Optional<IEntry> Linedefs { get; private set; } = Empty;
        public Optional<IEntry> Things { get; private set; } = Empty;
        public Optional<IEntry> Segments { get; private set; } = Empty;
        public Optional<IEntry> Subsectors { get; private set; } = Empty;
        public Optional<IEntry> Nodes { get; private set; } = Empty;
        public Optional<IEntry> Blockmap { get; private set; } = Empty;
        public Optional<IEntry> Reject { get; private set; } = Empty;
        public Optional<IEntry> TextMap { get; private set; } = Empty;
        public Optional<IEntry> ZNodes { get; private set; } = Empty;
        public Optional<IEntry> Dialogue { get; private set; } = Empty;
        public Optional<IEntry> Behavior { get; private set; } = Empty;
        public Optional<IEntry> Scripts { get; private set; } = Empty;
        public Optional<IEntry> EndMap { get; private set; } = Empty;
        public Optional<IEntry> GLMarker { get; private set; } = Empty;
        public Optional<IEntry> GLVertices { get; private set; } = Empty;
        public Optional<IEntry> GLSegments { get; private set; } = Empty;
        public Optional<IEntry> GLSubsectors { get; private set; } = Empty;
        public Optional<IEntry> GLNodes { get; private set; } = Empty;

        /// <summary>
        /// Gets the map type based on what entries are present.
        /// </summary>
        public MapType MapType => TextMap ? MapType.UDMF : (Behavior ? MapType.Hexen : MapType.Doom);

        internal MapComponents(UpperString name)
        {
            Name = name;
        }

        internal void TrackMapMarker(IEntry marker)
        {
            if (marker != null)
                Marker = new Optional<IEntry>(marker);
        }

        internal void Track(IEntry entryToTrack)
        {
            Optional<IEntry> entry = new Optional<IEntry>(entryToTrack);

            switch (entryToTrack.Path.Name)
            {
            case "THINGS":
                Things = entry;
                break;
            case "LINEDEFS":
                Linedefs = entry;
                break;
            case "SIDEDEFS":
                Sidedefs = entry;
                break;
            case "VERTEXES":
                Vertices = entry;
                break;
            case "SEGS":
                Segments = entry;
                break;
            case "SSECTORS":
                Subsectors = entry;
                break;
            case "NODES":
                Nodes = entry;
                break;
            case "SECTORS":
                Sectors = entry;
                break;
            case "REJECT":
                Reject = entry;
                break;
            case "BLOCKMAP":
                Blockmap = entry;
                break;
            case "BEHAVIOR":
                Behavior = entry;
                break;
            case "SCRIPTS":
                Scripts = entry;
                break;
            case "TEXTMAP":
                TextMap = entry;
                break;
            case "ZNODES":
                ZNodes = entry;
                break;
            case "DIALOGUE":
                Dialogue = entry;
                break;
            case "ENDMAP":
                EndMap = entry;
                break;
            case "GL_VERT":
                GLVertices = entry;
                break;
            case "GL_SEGS":
                GLSegments = entry;
                break;
            case "GL_SSECT":
                GLSubsectors = entry;
                break;
            case "GL_NODES":
                GLNodes = entry;
                break;
            default:
                // We assume this is the GL_<mapname> marker.
                GLMarker = entry;
                break;
            }
        }

        internal bool IsValid()
        {
            if (!Marker)
                return false;

            switch (MapType)
            {
            case MapType.Doom:
                return Vertices && Sectors && Sidedefs && Linedefs && Things;
            case MapType.Hexen:
                return Vertices && Sectors && Sidedefs && Linedefs && Things && Behavior;
            case MapType.UDMF:
                return TextMap;
            default:
                return false;
            }
        }
    }
}
