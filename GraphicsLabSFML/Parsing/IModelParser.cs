using GraphicsLabSFML.Models;

namespace GraphicsLabSFML.Parsing
{
    public interface IModelParser
    {
        Model Parse(IEnumerable<string> source);

        TriangulatedModel ParseTriangulated(IEnumerable<string> source);
    }
}
