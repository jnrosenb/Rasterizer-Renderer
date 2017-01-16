using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;

namespace Ejemplo2
{
    public class Material
    {
        public string name { get; set; }
        public List<float> color { get; set; }
        public string material_type { get; set; }
        public List<Texture> Textures { get; set; }

        public bool use_tex = false;
        public Dictionary<string, Bitmap> bitmaps;
    }

    public class Material_brdf: Material
    {
        public bool use_for_ambient{ get; set; }
        public Dictionary<string, object> brdfParams{ get; set; }

        public Material_brdf(string name, List<float> color, string type, Dictionary<string, object> param, bool ambient = false) 
        {
            this.name = name;
            this.color = color;
            this.material_type = type;
            this.use_for_ambient = ambient;
            this.brdfParams = param;
        }
    }

    public class Material_brdf_textured : Material
    {
        public bool use_for_ambient { get; set; }
        public Dictionary<string, object> brdfParams { get; set; }
        public string color_texture { get; set; }
        public string texture_filtering { get; set; }

        public Material_brdf_textured(string name, string type, Dictionary<string, object> param, string color_tex, string tex_filt, bool ambient = false)
        {
            this.name = name;
            this.material_type = type;
            this.use_for_ambient = ambient;
            this.brdfParams = param;
            this.bitmaps = new Dictionary<string, Bitmap>();

            this.color_texture = color_tex;
            this.texture_filtering = tex_filt;
            use_tex = true;
        }

        public void Load_textures()
        {
            foreach (Texture t in Textures)
            {
                string[] split = t.path.Split('/');
                Bitmap bitmap = (Bitmap)Image.FromFile(@"..\\..\\resources\\textures\\" + split[1]);
                this.bitmaps.Add(t.name, bitmap);
            }
        }
    }

    public class Texture
    {
        public string name { get; set; }
        public string path { get; set; }

        public Texture(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}
