namespace GraphicsLabSFML.Models
{
    public struct FaceInfo
    {
        public int[] VerticesIndices { get; }

        public FaceInfo(IEnumerable<int> verticesIndices)
        {
            VerticesIndices = verticesIndices.ToArray();
        }
    }
}
