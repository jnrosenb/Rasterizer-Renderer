using System;
using System.Collections.Generic;
using System.Drawing;
using Rasterizer;
using System.Linq;
using System.Text;

namespace Ejemplo2
{
    public static class Rasterizer
    {

        //Rasterizing Points:
        public static void RasterizePoint(List<float>[,] imageData, tuple_3 v1, tuple_3 color)
        {
            int x = (int)(v1.x + 0.5f);
            int y = (int)(v1.y + 0.5f);

            shadePixel(imageData, x, y, color);
        }


        //Rasterizing Lines:
        public static void RasterizeLine(List<float>[,] imageData, tuple_3 v1, tuple_3 v2, tuple_3 c1, tuple_3 c2)
        {
            //Saco la pendiente:
            float m = (v2.y - v1.y) / (v2.x - v1.x);
            if (v2.x == v1.x) m = float.PositiveInfinity;

            int x1 = (int)(v1.x + 0.5f); int x2 = (int)(v2.x + 0.5f);
            int y1 = (int)(v1.y + 0.5f); int y2 = (int)(v2.y + 0.5f);

            int yi = y1; int yf = y2;
            int xi = x1; int xf = x2;
            if (Math.Abs(m) >= 1 && y2 <= y1)
            {
                yi = y2; yf = y1;
            }
            else if (Math.Abs(m) < 1 && x2 <= x1)
            {
                xi = x2; xf = x1;
            }

            //Dependiendo de si es mayor o menor a uno, elijo como seguir:
            if (m == float.PositiveInfinity)
            {
                int x = x1;
                while (yi <= yf)
                {
                    tuple_3 c = Vectores.Normalize(dist(x, yi, xi, yi) * c2 + dist(x, yi, xf, yf) * c1);
                    shadePixel(imageData, x, yi, c);   //Color debe ser ponderado:
                    yi++;
                }
            }
            else if (m == 0.0f)
            {
                int y = y1;
                while (xi <= xf)
                {
                    tuple_3 c = Vectores.Normalize(dist(xi, y, xi, yi) * c2 + dist(xi, y, xf, yf) * c1);
                    shadePixel(imageData, xi, y, c);   //Color debe ser ponderado:
                    xi++;
                }
            }
            else if (Math.Abs(m) >= 1)
            {
                while (xi <= xf)
                {
                    int y = (int)((m * (xi - x1) + y1) + 0.5f);
                    tuple_3 c = Vectores.Normalize(dist(xi, y, xi, yi) * c2 + dist(xi, y, xf, yf) * c1);
                    shadePixel(imageData, xi, y, c);   //Color debe ser ponderado:
                    xi++;
                }
            }
            else
            {
                while (yi <= yf)
                {
                    int x = (int)(((yi - y1) / m + x1) + 0.5f);
                    tuple_3 c = Vectores.Normalize(dist(x, yi, xi, yi) * c2 + dist(x, yi, xf, yf) * c1);
                    shadePixel(imageData, x, yi, c);   //Color debe ser ponderado:
                    yi++;
                }
            }
        }


        //Rasterizing Triangle:
        public static void RasterizeTriangle(List<float>[,] imageData, ref float[,] z_buffer, float near, float far, tuple_3 v1, tuple_3 v2, tuple_3 v3, tuple_3 c1, tuple_3 c2, tuple_3 c3)
        {
            int x1 = (int)(v1.x + 0.5f); int x2 = (int)(v2.x + 0.5f); int x3 = (int)(v3.x + 0.5f);
            int y1 = (int)(v1.y + 0.5f); int y2 = (int)(v2.y + 0.5f); int y3 = (int)(v3.y + 0.5f);

            float xMin = Math.Min(v1.x, v2.x);  xMin = Math.Min(v3.x, xMin);
            float yMin = Math.Min(v1.y, v2.y);  yMin = Math.Min(v3.y, yMin);
            float xMax = Math.Max(v1.x, v2.x);  xMax = Math.Max(v3.x, xMax);
            float yMax = Math.Max(v1.y, v2.y);  yMax = Math.Max(v3.y, yMax);

            for (int x = (int)(xMin + 0.5f); x <= (int)(xMax + 0.5f); x++)
            {
                for (int y = (int)(yMin + 0.5f); y <= (int)(yMax + 0.5f); y++)
                {
                    tuple_3 bar = baricentric(x, y, v1, v2, v3);
                    if (bar.x <= 1.0f && bar.y <= 1.0f && bar.z <= 1.0f && bar.x >= 0.0f && bar.y >= 0.0f && bar.z >= 0.0f)
                    {
                        float z_pix = (bar.x * v1.z + bar.y * v2.z + bar.z * v3.z);
                        if (z_pix > z_buffer[x,y])
                        {
                            z_buffer[x,y] = z_pix;
                            tuple_3 c = bar.x * c1 + bar.y * c2 + bar.z * c3;
                            shadePixel(imageData, x, y, c);
                        }
                    }
                }
            }
        }


        //Rasterizing Triangle, version con shading a nivel de pixeles:
        public static void RasterizeTriangle(List<float>[,] imageData, Scene scene, Mesh mesh, ref float[,] z_buffer, vertex v1_pack, vertex v2_pack, vertex v3_pack)
        {
            tuple_3 v1 = v1_pack.screen_coord;
            tuple_3 v2 = v2_pack.screen_coord;
            tuple_3 v3 = v3_pack.screen_coord;
            int x1 = (int)(v1.x + 0.5f); int x2 = (int)(v2.x + 0.5f); int x3 = (int)(v3.x + 0.5f);
            int y1 = (int)(v1.y + 0.5f); int y2 = (int)(v2.y + 0.5f); int y3 = (int)(v3.y + 0.5f);

            float xMin = Math.Min(v1.x, v2.x); xMin = Math.Min(v3.x, xMin);
            float yMin = Math.Min(v1.y, v2.y); yMin = Math.Min(v3.y, yMin);
            float xMax = Math.Max(v1.x, v2.x); xMax = Math.Max(v3.x, xMax);
            float yMax = Math.Max(v1.y, v2.y); yMax = Math.Max(v3.y, yMax);

            for (int x = (int)(xMin + 0.5f); x <= (int)(xMax + 0.5f); x++)
            {
                for (int y = (int)(yMin + 0.5f); y <= (int)(yMax + 0.5f); y++)
                {
                    tuple_3 bar = baricentric(x, y, v1, v2, v3);
                    if (bar.x <= 1.0f && bar.y <= 1.0f && bar.z <= 1.0f && bar.x >= 0.0f && bar.y >= 0.0f && bar.z >= 0.0f)
                    {
                        float z_pix = (bar.x * v1.z + bar.y * v2.z + bar.z * v3.z);
                        if (z_pix > z_buffer[x, y])
                        {
                            z_buffer[x, y] = z_pix;
                            tuple_3 normal = bar.x * v1_pack.normal + bar.y * v2_pack.normal + bar.z * v3_pack.normal;
                            tuple_3 world_coord = bar.x * v1_pack.world_coord + bar.y * v2_pack.world_coord + bar.z * v3_pack.world_coord;
                            tuple_3 c = Scene.shade(scene, mesh, normal, world_coord);

                            if (v1_pack.uv.x != -1.0f && v1_pack.uv.y != -1.0f)
                            {
                                foreach(Material m in mesh.materials)
                                {
                                    if (!m.use_tex) continue;
                                    Material_brdf_textured mat = (Material_brdf_textured)m; 

                                    tuple_3 uv_pix = bar.x * v1_pack.uv + bar.y * v2_pack.uv + bar.z * v3_pack.uv;

                                    int ti = (int)(uv_pix.x * (mat.bitmaps[mat.color_texture].Width - 1) + 0.05f);
                                    int tj = (int)(uv_pix.y * (mat.bitmaps[mat.color_texture].Height - 1) + 0.05f);

                                    Color c_tex = m.bitmaps[mat.color_texture].GetPixel(ti, tj);

                                    c += new tuple_3{ x = c_tex.R, y = c_tex.G, z = c_tex.B};
                                    c = Vectores.Normalize(c);
                                }
                            }
                            
                            shadePixel(imageData, x, y, c);
                        }
                    }
                }
            }
        }


        //Retorna tuple_3 con alfa beta y gamma:
        public static tuple_3 baricentric(int x, int y, tuple_3 v1, tuple_3 v2, tuple_3 v3)
        {
            float b1 = x - v1.x;
            float b2 = y - v1.y;

            float a = v2.x - v1.x;
            float b = v3.x - v1.x;
            float c = v2.y - v1.y;
            float d = v3.y - v1.y;

            float detA = a * d - b * c;
            float detA1 = b1 * d - b * b2;
            float detA2 = a * b2 - b1 * c;

            float beta = detA1 / detA;
            float gamma = detA2 / detA;
            float alfa = 1.0f - beta - gamma;

            return new tuple_3 { x = alfa, y = beta, z = gamma };
        }


        //Dado un punto y un color, lo pinta de ese color:
        public static void shadePixel(List<float>[,] imageData, int x, int y, tuple_3 color)
        {
            imageData[x, y] = new List<float>() { color.x, color.y, color.z };
        }


        //Distancia euclidiana entre 2 puntos en 2d:
        public static float dist(int x1, int y1, int x2, int y2)
        {
            return (float)Math.Sqrt(Math.Pow(y2 - y1, 2.0f) + Math.Pow(x2 - x1, 2.0f));
        }
    }
}
