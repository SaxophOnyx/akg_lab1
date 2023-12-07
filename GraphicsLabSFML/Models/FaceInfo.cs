namespace GraphicsLabSFML.Models
{
    public struct FaceInfo
    {
        public int[] VerticesIndices { get; }

        public int[] NormalsIndices { get; }


        public FaceInfo(IEnumerable<int> verticesIndices, IEnumerable<int> normalsIndices)
        {
            VerticesIndices = verticesIndices.ToArray();
            NormalsIndices = normalsIndices.ToArray();
        }
    }
}
