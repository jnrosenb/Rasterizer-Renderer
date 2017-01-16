using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ejemplo2;
using Rasterizer;

namespace Ejemplo2
{
    class Program
    {
        //ImageData sera el canvas que se pasara para pintar:
        public static List<float>[,] imageData { get; set; }

        static void Main(string[] args)
        {
            //Codigo para hacer rasterizing:
            Console.Write("RASTERIZER \n\n"); 
            Console.Write("Ingresar path de archivo con extension .json (debe estar en la carpeta json): ");
            string path = Console.ReadLine();
            Scene.LoadScene("..\\..\\Json\\" + path);
        }
    }
}
