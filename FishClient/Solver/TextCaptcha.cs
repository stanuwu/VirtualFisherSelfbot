using System.Diagnostics;
using System.Drawing;
using System.Text;
using FishClient.Commands;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace FishClient.Solver;

public class TextCaptcha
{
    private TesseractEngine engine;

    public TextCaptcha()
    {
        engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    }
    
    //https://github.com/charlesw/tesseract-samples/blob/master/src/Tesseract.ConsoleDemo/Program.cs
    public String Solve(Bitmap bitmap)
    {
        for (int x = 0; x < bitmap.Width; x++)
        for (int y = 0; y < bitmap.Height; y++)
        {
            Color px = bitmap.GetPixel(x, y);
            if (px.R < 100 && px.G < 100 && px.B < 100) bitmap.SetPixel(x, y, Color.White);
            else bitmap.SetPixel(x, y, Color.Black);
        }
        byte[] data;
        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Jpeg);
            data = ms.ToArray();
        }
        StringBuilder str = new StringBuilder();
        using (var img = Pix.LoadFromMemory(data))
        {
            using (var page = engine.Process(img))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        str.Append(iter.GetText(PageIteratorLevel.Word));
                    } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                }
            }
        }

        return str.ToString().Replace(" ", "");
    }
}