namespace SvgDrawing
{
    using Svg;

    public class SvgDocumentWrapper
    {
        private readonly SvgDocument _document = new SvgDocument();

        public SvgDocumentWrapper(int width, int height)
        {
            _document.Width = width;
            _document.Height = height;
        }

        public void Add(SvgElement element)
        {
            _document.Children.Add(element);
        }

        public void Save(string path)
        {
            _document.Write(path);
        }
    }
}
