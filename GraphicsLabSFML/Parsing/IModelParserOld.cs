using GraphicsLabSFML.Models;

namespace GraphicsLabSFML.Parsing
{
    public interface IModelParserOld
    {
        ModelOld Parse(IEnumerable<string> source);
    }
}
