using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Ejemplo2;

namespace Rasterizer
{
  class Display
  {
    public static void GenerateImage(List<float>[,] imageData, int width, int height)
    {
      var image = new Bitmap(width, height);
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
            var color = imageData[i, height - j - 1];
            color = color.Select(c => Math.Min(c, 1.0f)).ToList();
            image.SetPixel(i, j, Color.FromArgb(((int)(color[0] * 255)),
            ((int)(color[1] * 255)), ((int)(color[2] * 255))));
        }
      }

      Console.Write("Ingrese nombre del archivo de imagen: ");
      string output_name = Console.ReadLine();
      image.Save(output_name + ".png", ImageFormat.Png);

    }
  }
}
