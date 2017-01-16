using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ejemplo2;

namespace Rasterizer
{ 

    //Clase que carga toda la escena y maneja el proceso de rendering.
    public class Scene
    {
        public Dictionary<string, object> Parameters { get; set; }
        public Camera Camera { get; set; }
        public List<Objects> Objects { get; set; }
        public List<Light> Lights { get; set; }
        public AmbientLight ambient_light { get; set; }
        public static Random rand = new Random(DateTime.Now.Millisecond);
        public static int shading_mode;


        //Constructor de Scene.
        Scene(Dictionary<string, object> parameters, Camera camera, List<Objects> objects, List<Light> lights, AmbientLight ambient)//Dictionary<string, Light> lights)
        {
            Parameters = parameters;
            Camera = camera;
            Objects = objects;
            Lights = lights;
            ambient_light = ambient;
        }


        //Metodo recursivo que saca todos los datos de la escena y los guarda en scene.
        private static object ObjectHook(JToken token)
        {
          switch (token.Type)
          {
            case JTokenType.Object:

              var children = token.Children<JProperty>();
              var dic = children.ToDictionary(prop => prop.Name, prop => ObjectHook(prop.Value));
          
              if (dic.ContainsKey("__type__"))
              {
                if (dic["__type__"].ToString() == "scene")
                {
                    AmbientLight ambient = null;
                    var camera = (Camera)dic["camera"];
                    var Lights = ((List<Object>)dic["lights"]).ConvertAll(x => (Light)x);
                    foreach (object l in (List<Object>)dic["lights"])
                    {
                        Light light = (Light)l;
                        if (light.name == "ambient_light")
                        {
                            ambient = (AmbientLight)light;
                            break;
                        }
                    }
                    var Objects = ((List<Object>)dic["objects"]).ConvertAll(x => (Objects)x);
                    return new Scene((Dictionary<string, object>)dic["params"], camera, Objects, Lights, ambient);
                }
                else if (dic["__type__"].ToString() == "camera")
                {
                  var fov = Convert.ToSingle(dic["fov"]);
                  var position = ((List<object>)dic["position"]).Select(Convert.ToSingle).ToList();
                  var up = ((List<object>)dic["up"]).Select(Convert.ToSingle).ToList();
                  var target = ((List<object>)dic["target"]).Select(Convert.ToSingle).ToList();

                  var near = -1.0f;
                  if (dic.ContainsKey("near"))
                      near = Convert.ToSingle(dic["near"]);
                  var far = -1.0f;
                  if (dic.ContainsKey("far"))
                      far = Convert.ToSingle(dic["far"]);

                  tuple_3 pos = new tuple_3 { x = position[0], y = position[1], z = position[2]};
                  tuple_3 cup = new tuple_3 { x = up[0], y = up[1], z = up[2] };
                  tuple_3 tgt = new tuple_3 { x = target[0], y = target[1], z = target[2] };

                  return new Camera(fov, pos, cup, tgt, near, far);
                }
                else if (dic["__type__"].ToString() == "mesh")
                {
                    var path = (string)dic["file_path"];
                
                    var cvn = false;
                    if (dic.ContainsKey("compute_vertex_normals"))
                        cvn = (bool)dic["compute_vertex_normals"];

                    var names = ((List<Object>)dic["materials"]).ConvertAll(x => (string)x);
                    List<Material> materials = new List<Material>();
                    foreach (string name in names)
                    {
                        materials.Add(Resources.materials[name]);
                    }

                    return new Mesh(path, cvn, materials);
                }
                else if (dic["__type__"].ToString() == "point_light")
                {
                    var position = ((List<object>)dic["position"]).Select(Convert.ToSingle).ToList();
                    var color = ((List<object>)dic["color"]).Select(Convert.ToSingle).ToList();
                    tuple_3 pos = new tuple_3 { x = position[0], y = position[1], z = position[2] };

                    return new PointLight(pos, color);
                }
                else if (dic["__type__"].ToString() == "ambient_light")
                {
                    var color = ((List<object>)dic["color"]).Select(Convert.ToSingle).ToList();
                    return new AmbientLight(color);
                }
              }
              return dic;

            case JTokenType.Array:
              return token.Select(ObjectHook).ToList();

            default:
              return ((JValue)token).Value;
          }
        }


        //Carga la escena completa y deja los pixeles listos para pintar.
        public static void LoadScene(string fileName)
        {
            try
            {
                //Recupero el json y guardo todos los valores en el objeto scene.
                var jsonString1 = File.ReadAllText(fileName);
                Resources.load();
                var scene1 = (Scene)ObjectHook(JToken.Parse(jsonString1));

                //Defino el width, height y near de la escena. Representa ancho y alto en espacio imagen.
                int mode = 2;
                Console.Write("Elija entre rendering por puntos (1), lineas (2), o triangulos (3): ");
                mode = int.Parse(Console.ReadLine());
                Console.Write("Ingrese width imagen: ");
                int width = int.Parse(Console.ReadLine());
                Console.Write("Ingrese height imagen: ");
                int height = int.Parse(Console.ReadLine());
                Console.Write("Ingrese 1 si desea vertex shading o 2 si desea pixel shading: ");
                shading_mode = int.Parse(Console.ReadLine());

                //Dejo seteado altiro el fondo con su color de fondo:
                List<float>[,] imageData = new List<float>[width, height];
                for (int i = 0; i < width; i++)
                {
                    List<float> bgc = ((List<object>)scene1.Parameters["background_color"]).Select(Convert.ToSingle).ToList();
                    for (int j = 0; j < height; j++)
                        imageData[i, j] = bgc;
                }

                //Aqui se llama al metodo que manejara los meshes:
                paint_mesh(imageData, scene1, width, height, mode);

                Display.GenerateImage(imageData, width, height);
            }
            catch (IOException)
            {
                Console.WriteLine("Error, archivo no existe!");
                Console.Read();
            }
        }


        //Metodo que pinta los meshes.
        private static void paint_mesh(List<float>[,] imageData, Scene scene, int width, int height, int mode)
        {
            //Near y far estan en su valor absoluto, como distancias:
            float N = -scene.Camera.near;
            float F = -scene.Camera.far;
            tuple_3 e = scene.Camera.position;
            tuple_3 t = scene.Camera.target;

            //Se crea el z-buffer y se inicializa todo a 1:
            float[,] z_buffer = new float[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    z_buffer[i, j] = 0.0f;
            }

            //Se obtienen valores top, bottom right y left a partir de los otros.
            float top = scene.Camera.near * (float)Math.Tan(DegreeToRadian(scene.Camera.FOV / 2.0f));
            float bottom = -top;
            float right = ((float)width / height) * (top);
            float left = -right;

            //Ahora se obtienen los vectores unitarios u,v,w.
            tuple_3 w = Vectores.Normalize(e - t);
            tuple_3 u = Vectores.Normalize(Vectores.cross(scene.Camera.up, w));
            tuple_3 v = Vectores.Normalize(Vectores.cross(w, u));

            foreach (Objects obj in scene.Objects)
            {
                Mesh mesh = (Mesh)obj;
                foreach (tuple_3[] face in mesh.faces)
                {
                    //Elementos de la ecuacion de interseccion con plano:
                    tuple_3 v1 = mesh.vertex[(int)face[0].x];
                    tuple_3 v2 = mesh.vertex[(int)face[0].y];
                    tuple_3 v3 = mesh.vertex[(int)face[0].z];
                    tuple_4 v_1 = to_v4(v1);
                    tuple_4 v_2 = to_v4(v2);
                    tuple_4 v_3 = to_v4(v3);

                    //Aca obtiene las normales en espacio mundo para cada vertice:
                    tuple_3 n1 = new tuple_3();
                    tuple_3 n2 = new tuple_3();
                    tuple_3 n3 = new tuple_3();
                    if (mesh.cvn)
                    {
                        n1 = Vectores.Normalize(mesh.vertex_normals[(int)face[0].x]);
                        n2 = Vectores.Normalize(mesh.vertex_normals[(int)face[0].y]);
                        n3 = Vectores.Normalize(mesh.vertex_normals[(int)face[0].z]);
                    }
                    else if (mesh.vn_dic.Count == 0)
                    {
                        n1 = Vectores.Normalize(Vectores.cross((v2 - v1), (v3 - v1)));
                        n2 = Vectores.Normalize(Vectores.cross((v2 - v1), (v3 - v1)));
                        n3 = Vectores.Normalize(Vectores.cross((v2 - v1), (v3 - v1)));
                    }
                    else
                    {
                        n1 = mesh.vn_dic[(int)face[2].x];
                        n2 = mesh.vn_dic[(int)face[2].y];
                        n3 = mesh.vn_dic[(int)face[2].z];
                    }

                    //Se encarga de coordenadas U, V por vertex:
                    tuple_3 uv1 = new tuple_3() { x = -1.0f, y = -1.0f, z = 0.0f };
                    tuple_3 uv2 = new tuple_3() { x = -1.0f, y = -1.0f, z = 0.0f };
                    tuple_3 uv3 = new tuple_3() { x = -1.0f, y = -1.0f, z = 0.0f };
                    if (mesh.vt_dic.Count > 0)
                    {
                        uv1 = mesh.vt_dic[(int)face[1].x];
                        uv2 = mesh.vt_dic[(int)face[1].y];
                        uv3 = mesh.vt_dic[(int)face[1].z];
                    }

                    //Obtengo la matriz de transformacion desde espacio mundo a espacio camara:
                    matrix_4 C = new matrix_4 { a00 =  u.x, a01 =  v.x, a02 =  w.x, a03 =  e.x,
                                                a10 =  u.y, a11 =  v.y, a12 =  w.y, a13 =  e.y,
                                                a20 =  u.z, a21 =  v.z, a22 =  w.z, a23 =  e.z,
                                                a30 = 0.0f, a31 = 0.0f, a32 = 0.0f, a33 =  1.0f };
                    C = Matrix.inverse(C);

                    //Ahora obtengo matriz para perspectiva y z-buffer:
                    float a = (N + F) / N;
                    float b = -F;
                    float c = 1.0f / N;
                    matrix_4 S = new matrix_4 {  a00 = 1, a01 = 0, a02 = 0, a03 = 0 ,
                                                 a10 = 0, a11 = 1, a12 = 0, a13 = 0 ,
                                                 a20 = 0, a21 = 0, a22 = a, a23 = b ,
                                                 a30 = 0, a31 = 0, a32 = c, a33 = 0 };

                    //Ahora hago la transformacion desde espacio camara a espacio proyeccion:
                    matrix_4 P1 = new matrix_4 { a00 = 1.0f, a01 = 0.0f, a02 = 0.0f, a03 = -left,
                                                 a10 = 0.0f, a11 = 1.0f, a12 = 0.0f, a13 = -bottom,
                                                 a20 = 0.0f, a21 = 0.0f, a22 = 1.0f, a23 = -F,
                                                 a30 = 0.0f, a31 = 0.0f, a32 = 0.0f, a33 = 1.0f };
                    matrix_4 P2 = new matrix_4 { a00 = (2/(right-left)), a01 = 0.0f,             a02 = 0.0f,           a03 = 0.0f,
                                                 a10 = 0.0f,             a11 = (2/(top-bottom)), a12 = 0.0f,           a13 = 0.0f,
                                                 a20 = 0.0f,             a21 = 0.0f,             a22 = (2/(N-F)), a23 = 0.0f,
                                                 a30 = 0.0f,             a31 = 0.0f,             a32 = 0.0f,           a33 = 1.0f };
                    matrix_4 P3 = new matrix_4 { a00 = 1.0f, a01 = 0.0f, a02 = 0.0f, a03 = -1.0f,
                                                 a10 = 0.0f, a11 = 1.0f, a12 = 0.0f, a13 = -1.0f,
                                                 a20 = 0.0f, a21 = 0.0f, a22 = 1.0f, a23 = -1.0f,
                                                 a30 = 0.0f, a31 = 0.0f, a32 = 0.0f, a33 =  1.0f };
                    matrix_4 P = P3 * P2 * P1;

                    //Ahora hago la transformacion desde espacio camara a espacio proyeccion:
                    float hdi2 = (height / 2.0f);
                    float wdi2 = (width / 2.0f);
                    matrix_4 I1 = new matrix_4 { a00 = 1.0f, a01 = 0.0f, a02 = 0.0f, a03 = 1.0f,
                                                 a10 = 0.0f, a11 = 1.0f, a12 = 0.0f, a13 = 1.0f,
                                                 a20 = 0.0f, a21 = 0.0f, a22 = 1.0f, a23 = 1.0f,
                                                 a30 = 0.0f, a31 = 0.0f, a32 = 0.0f, a33 = 1.0f };
                    matrix_4 I2 = new matrix_4 { a00 = wdi2, a01 = 0.0f, a02 = 0.0f, a03 = 0.0f,
                                                 a10 = 0.0f, a11 = hdi2, a12 = 0.0f, a13 = 0.0f,
                                                 a20 = 0.0f, a21 = 0.0f, a22 = 0.5f, a23 = 0.0f,
                                                 a30 = 0.0f, a31 = 0.0f, a32 = 0.0f, a33 = 1.0f };
                    matrix_4 I3 = new matrix_4 { a00 = 1.0f, a01 = 0.0f, a02 = 0.0f, a03 = -0.5f,
                                                 a10 = 0.0f, a11 = 1.0f, a12 = 0.0f, a13 = -0.5f,
                                                 a20 = 0.0f, a21 = 0.0f, a22 = 1.0f, a23 = 0.0f,
                                                 a30 = 0.0f, a31 = 0.0f, a32 = 0.0f, a33 = 1.0f };
                    matrix_4 I = I3 * I2 * I1;

                    //De espacio mundo a espacio imagen:
                    matrix_4 post_persp = I * P;
                    tuple_4 v1_img = C * v_1;
                    tuple_4 v2_img = C * v_2;
                    tuple_4 v3_img = C * v_3;
                    
                    v1_img = S * v1_img;
                    v2_img = S * v2_img;
                    v3_img = S * v3_img;
                    v1_img = v1_img / (v1_img.w);
                    v2_img = v2_img / (v2_img.w);
                    v3_img = v3_img / (v3_img.w);
                    v1_img = post_persp * v1_img;
                    v2_img = post_persp * v2_img;
                    v3_img = post_persp * v3_img;

                    if (mode == 1)
                    {
                        tuple_3 c1;
                        if (obj.materials.Count > 0)
                            c1 = new tuple_3 { x = obj.materials[0].color[0], y = obj.materials[0].color[1], z = obj.materials[0].color[2] };
                        else
                            c1 = new tuple_3 { x = 0.0f, y = 0.0f, z = 0.0f };
                            
                        Ejemplo2.Rasterizer.RasterizePoint(imageData, to_v3(v1_img), c1);
                        Ejemplo2.Rasterizer.RasterizePoint(imageData, to_v3(v2_img), c1);
                        Ejemplo2.Rasterizer.RasterizePoint(imageData, to_v3(v3_img), c1);                        
                    }
                    else if (mode == 2)
                    {
                        tuple_3 c1, c2;
                        if (obj.materials.Count > 0)
                            c1 = new tuple_3 { x = obj.materials[0].color[0], y = obj.materials[0].color[1], z = obj.materials[0].color[2] };
                        else
                        {
                            c1 = new tuple_3 { x = 0.0f, y = 0.0f, z = 0.0f };
                            c2 = new tuple_3 { x = 0.0f, y = 0.0f, z = 0.0f };
                        }
                        
                        Ejemplo2.Rasterizer.RasterizeLine(imageData, to_v3(v1_img), to_v3(v2_img), c1, c1); //, c2);
                        Ejemplo2.Rasterizer.RasterizeLine(imageData, to_v3(v2_img), to_v3(v3_img), c1, c1); //, c2);
                        Ejemplo2.Rasterizer.RasterizeLine(imageData, to_v3(v3_img), to_v3(v1_img), c1, c1); //, c2);
                    }
                    else if (mode == 3)
                    {
                        tuple_3 c1 = new tuple_3(); tuple_3 c2 = new tuple_3(); tuple_3 c3 = new tuple_3();

                        if (shading_mode == 1)
                        {
                            c1 = shade(scene, mesh, n1, v1);
                            c2 = shade(scene, mesh, n2, v2);
                            c3 = shade(scene, mesh, n3, v3);
                            
                            Ejemplo2.Rasterizer.RasterizeTriangle(imageData, ref z_buffer, N, F, to_v3(v1_img), to_v3(v2_img), to_v3(v3_img), c1, c2, c3);
                        }
                        else
                        {
                            vertex v1_pack = new vertex(v1, to_v3(v1_img), n1, c1, uv1);
                            vertex v2_pack = new vertex(v2, to_v3(v2_img), n2, c2, uv2);
                            vertex v3_pack = new vertex(v3, to_v3(v3_img), n3, c3, uv3);
                            
                            Ejemplo2.Rasterizer.RasterizeTriangle(imageData, scene, mesh, ref z_buffer, v1_pack, v2_pack, v3_pack);
                        }
                    }
                }
            }
        }


        //Retorna color de shading del vertice:
        public static tuple_3 shade(Scene scene, Mesh mesh, tuple_3 normal, tuple_3 pos)
        {
            //Vector de direccion desde punto (obj_point) hasta la camara (e).
            tuple_3 vision_dir = Vectores.Normalize(scene.Camera.position - pos); //OJO, PUSE VERTEXPOS COMO OBJ_POINT****
            Material_brdf lambert = null;
            Material_brdf blinnPhong = null;

            //Se asignan los distintos materiales a sus variables (Por ahora, solo 1 de cada tipo):
            foreach (Material m in mesh.materials)
            {
                if (m.GetType() == typeof(Material_brdf))
                {
                    Material_brdf material = (Material_brdf)m;
                    if (m.material_type == "lambert")
                        lambert = material;
                    if (m.material_type == "blinnPhong")
                        blinnPhong = material;
                }
            }
            //Se declara el color difuso y especular del objeto.
            List<float> obj_difuse_color = new List<float> { 0.0f, 0.0f, 0.0f };
            List<float> obj_specular_color = new List<float> { 0.0f, 0.0f, 0.0f };

            //Asigna los colores especulares y difusos si existen los materiales que correspondan:
            if (lambert != null) obj_difuse_color = lambert.color;
            if (blinnPhong != null) obj_specular_color = blinnPhong.color;
            tuple_3 difuse_color_sum = new tuple_3 { x = 0.0f, y = 0.0f, z = 0.0f };
            tuple_3 specular_color_sum = new tuple_3 { x = 0.0f, y = 0.0f, z = 0.0f };

            //Aqui define lo referente a la luz ambiente de la escena:
            List<float> obj_ambient_color = new List<float> { 0.0f, 0.0f, 0.0f };
            if (scene.ambient_light != null)
            {
                AmbientLight ambient = scene.ambient_light;
                if (lambert != null && lambert.use_for_ambient)
                    obj_ambient_color = new List<float> { ambient.color[0] * obj_difuse_color[0], ambient.color[1] * obj_difuse_color[1], ambient.color[2] * obj_difuse_color[2] };
            }

            //Aqui se encarga de las luces y los brillos difusos y especular:
            foreach (Light l in scene.Lights)
            {
                if (l.name == "point_light")
                {
                    //Declaro todos los vectores unitarios que me interesan:
                    PointLight light = (PointLight)l;
                    tuple_3 light_dir = new tuple_3();
                    tuple_3 light_pos = new tuple_3();
                    light_pos = light.position;
                    light_dir = Vectores.Normalize(light.position - pos);
                    
                    //Brillo difuso con metodo lambert:
                    if (lambert != null)
                    {
                        //Ahora calculo el coseno entre la normal y el vector de luz:
                        float cos_theta = Vectores.dot(normal, light_dir);
                        float f_difuse = Math.Max(0.0f, cos_theta);
                        tuple_3 difuse_color = new tuple_3 { x = f_difuse * light.color[0], y = f_difuse * light.color[1], z = f_difuse * light.color[2] };
                        difuse_color_sum += difuse_color;
                    }

                    //Brillo especular con metodo blinnphon:
                    if (blinnPhong != null)
                    {
                        tuple_3 h = Vectores.Normalize((light_dir + vision_dir) / 2);
                        float cos_theta2 = Vectores.dot(normal, h);
                        float f_specular = (float)Math.Pow(Math.Max(0.0f, cos_theta2), Convert.ToInt32(blinnPhong.brdfParams["shininess"]));
                        tuple_3 specular_color = new tuple_3 { x = f_specular * light.color[0], y = f_specular * light.color[1], z = f_specular * light.color[2] };
                        specular_color_sum += specular_color;
                    }
                }
            }
            if (lambert != null)
                obj_difuse_color = new List<float> { obj_difuse_color[0] * difuse_color_sum.x, obj_difuse_color[1] * difuse_color_sum.y, obj_difuse_color[2] * difuse_color_sum.z };
            if (blinnPhong != null)
                obj_specular_color = new List<float> { obj_specular_color[0] * specular_color_sum.x, obj_specular_color[1] * specular_color_sum.y, obj_specular_color[2] * specular_color_sum.z };
            
            //Color final que tendra el punto en cuestion:
            List<float> obj_color = new List<float> { 0.0f, 0.0f, 0.0f };

            if (lambert != null)
                obj_color = Vectores2.vectorSum(obj_color, obj_difuse_color);
            if (blinnPhong != null)
                obj_color = Vectores2.vectorSum(obj_color, obj_specular_color);
            if (scene.ambient_light != null)
                obj_color = Vectores2.vectorSum(obj_color, obj_ambient_color);
            
            return new tuple_3 { x = obj_color[0], y = obj_color[1], z = obj_color[2] };
        }


        //Clipping de 1 o dos vertices:
        public static tuple_4[] clipping(Scene scene, tuple_4 v_1, tuple_4 v_2, tuple_4 v_3, float near, int num)
        {
            //Los paso a v3 para mas comodidad al manipularlos:
            tuple_3 v1 = to_v3(v_1);
            tuple_3 v2 = to_v3(v_2);
            tuple_3 v3 = to_v3(v_3);
            tuple_3 v1_new, v2_new;

            //No se si funcione, pero usare esto como la normal del plano near:
            tuple_3 n = Vectores.Normalize(-scene.Camera.target);

            if (num == 1)
            {
                float ta = (0.0f - Vectores.dot(v1, n)) / (Vectores.dot((v3 - v1), n));
                float tb = (0.0f - Vectores.dot(v1, n)) / (Vectores.dot((v2 - v1), n));
                v1_new = v1 + ta * (v3 - v1);
                v2_new = v1 + tb * (v2 - v1); 
            }
            else
            {
                float ta = (0.0f - Vectores.dot(v1, n)) / (Vectores.dot((v3 - v1), n));
                float tb = (0.0f - Vectores.dot(v2, n)) / (Vectores.dot((v3 - v2), n));
                v1_new = v1 + ta * (v3 - v1);
                v2_new = v2 + tb * (v3 - v2);
            }

            //Retorna los nuevos vertices que reemplazaran a los viejos:
            return new tuple_4[2]{ to_v4(v1_new), to_v4(v2_new) };
        }


        //Convierte de Angulo a Radianes.
        public static float DegreeToRadian(float angle)
        {
            return (float)Math.PI * angle / 180.0f;
        }


        //No obtiene determinante, sino uno de los elementos para sacarlo.
        public static float dt(float pond, float a, float b, float c, float d) 
        {
            float det_A = pond * (a * d - b * c);
            return det_A;
        }


        //Retorna float aleatorio entre ambos valores.
        public static float next_float(float min, float max)
        {
            float r = (float)rand.NextDouble();
            r *= (max - min);
            r += min;
            return r;
        }


        //Retorna vector 4 para transformaciones.
        public static tuple_4 to_v4(tuple_3 v)
        {
            return new tuple_4 { x = v.x, y = v.y, z = v.z, w = 1.0f};
        }
     
 
        //Retorna vector 3 para transformaciones.
        public static tuple_3 to_v3(tuple_4 v)
        {
            return new tuple_3 { x = v.x, y = v.y, z = v.z};
        }
    }   

    //Empaquetando para no pasar tanto argumento:
    public struct vertex 
    {
        public tuple_3 world_coord;
        public tuple_3 screen_coord;
        public tuple_3 normal;
        public tuple_3 color;
        public tuple_3 uv;

        public vertex(tuple_3 world_coord, tuple_3 screen_coord, tuple_3 normal, tuple_3 color, tuple_3 uv)
        {
            this.world_coord = world_coord;
            this.screen_coord = screen_coord;
            this.normal = normal;
            this.color = color;
            this.uv = uv;
        }
    }
}
