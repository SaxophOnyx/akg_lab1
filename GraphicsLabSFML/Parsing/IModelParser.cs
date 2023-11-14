using GraphicsLabSFML.Models;

namespace GraphicsLabSFML.Parsing
{
    public interface IModelParser
    {
        TriangulatedModel ParseTriangulated(IEnumerable<string> source);
    }
}
