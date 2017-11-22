using System.Drawing;

namespace PrintCertificateBarcode.Core.Draw
{
    public interface IDrawLabel
    {
        int Width { get; }

        int Height { get; }

        float DPI { get; }

        Image DrawToImage(DrawGoodsInfo drawGoodsInfo);
    }
}
