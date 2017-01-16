using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.IO;

namespace Ejemplo2
{
    public static class Resources
    {

        public static Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public static void load()
        {
            var jsonString = File.ReadAllText("..\\..\\Resources\\Resources.json");
            var resources = (List<Material>)loadResources(JToken.Parse(jsonString));
            
            foreach (var res in resources)
            {
                if (!materials.Keys.Contains(res.name))
                    materials.Add((string)res.name, (Material)res);
            }
        }

        //Metodo que va a cargar los materiales y sus caracteristicas desde json.
        public static Object loadResources(JToken token)
        {
            switch (token.Type)
            {
                //Si es otro objeto json, entonces entra a trabajar dentro de el.
                case JTokenType.Object:

                    var children = token.Children<JProperty>();
                    var dic = children.ToDictionary(prop => prop.Name, prop => loadResources(prop.Value));

                    if (dic.ContainsKey("__type__"))
                    {
                        if (dic["__type__"].ToString() == "resources")
                        {
                            //Esto sera una lista de objetos tipo Material. 
                            var Materials = ((List<Object>)dic["materials"]).ConvertAll(x => (Material)x);
                            var Textures = ((List<Object>)dic["textures"]).ConvertAll(x => (Texture)x);
                            
                            foreach (Material m in Materials)
                            {
                                if (m.use_tex)
                                {
                                    Material_brdf_textured material = (Material_brdf_textured)m;
                                    material.Textures = Textures;
                                    material.Load_textures();
                                }
                            }
                            
                            return Materials;
                        }
                        else if (dic["__type__"].ToString() == "brdf_material")
                        {
                            var ambient = false;
                            var name = (string)(dic["name"]);
                            var color = ((List<object>)dic["color"]).Select(Convert.ToSingle).ToList();
                            var type = (string)(dic["brdf"]);
                            if (type == "lambert")
                                ambient = (bool)(dic["use_for_ambient"]);
                            var brdfParams = (Dictionary<string, object>)dic["brdfParams"];

                            return new Material_brdf(name, color, type, brdfParams, ambient);
                        }
                        else if (dic["__type__"].ToString() == "brdf_color_texture_material")
                        {
                            var ambient = false;
                            var name = (string)(dic["name"]);
                            var color_tex = (string)(dic["color_texture"]);
                            var tex_filter = (string)(dic["texture_filtering"]);
                            var type = (string)(dic["brdf"]);
                            if (type == "lambert")
                                ambient = (bool)(dic["use_for_ambient"]);
                            var brdfParams = (Dictionary<string, object>)dic["brdfParams"];

                            return new Material_brdf_textured(name, type, brdfParams, color_tex, tex_filter, ambient);
                        }
                        else if (dic["__type__"].ToString() == "color_texture")
                        {
                            var name = (string)(dic["name"]);
                            var path = (string)(dic["file_path"]);

                            return new Texture(name, path);
                        }
                    }
                    return dic;

                //En caso de encontrarse con que el objeto es un array, lo transforma a lista de objects.
                case JTokenType.Array:
                    return token.Select(loadResources).ToList();

                //Si es un valor, lo retorna como tal (int, char, etc).
                default:
                    return ((JValue)token).Value;
            }
        }
    }
}
