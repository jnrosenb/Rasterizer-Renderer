using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ejemplo2
{

    public class Objects
    {
        public List<Material> materials;
    }

    public class Mesh : Objects 
    {
        public string path;
        public bool cvn;
        public bool tex;

        public Dictionary<int, tuple_3> vertex = new Dictionary<int, tuple_3>();
        public Dictionary<int, tuple_3> vertex_normals = new Dictionary<int, tuple_3>();
        public Dictionary<int, tuple_3> vt_dic = new Dictionary<int, tuple_3>();
        public Dictionary<int, tuple_3> vn_dic = new Dictionary<int, tuple_3>();
        public List<tuple_3[]> faces = new List<tuple_3[]>();

        public Mesh(string path, bool cvn, List<Material> materials) 
        {
            this.path = path;
            this.cvn = cvn;
            this.materials = materials;

            load_mesh();

            if (cvn)
                compute_vertex_normals();
        }

        private void load_mesh()
        {
            //Line guarda cada linea del texto
            string line;

            //Index es para saber a que vertice se refieren las caras.
            int index = 1;
            int vt_index = 1;
            int vn_index = 1;

            System.IO.StreamReader file = new System.IO.StreamReader("..\\..\\meshes\\" + path);
            while ((line = file.ReadLine()) != null) 
            {
                string[] frag = line.Split(' ');

                if (frag[0] == "v") 
                {
                    save_vertex(frag[1], frag[2], frag[3], index);
                    index++;
                }
                else if (frag[0] == "#") { }
                else if (frag[0] == "f")
                {
                    save_face(frag[1], frag[2], frag[3], cvn);
                }
                else if (frag[0] == "vt")
                {
                    save_vt(frag[1], frag[2], "0.0", vt_index);
                    vt_index++;
                }
                else if (frag[0] == "vn")
                {
                    save_vn(frag[1], frag[2], frag[3], vn_index);
                    vn_index++;
                }
                else { }
            }
            file.Close();    
        }
        
        //Guarda vertices en diccionario que los relaciona  aun indice.
        private void save_vertex(string a, string b, string c, int index)
        {
            float x = float.Parse(a);
            float y = float.Parse(b);
            float z = float.Parse(c);
            vertex[index] = new tuple_3 { x = x, y = y, z = z };
        }

        //Guarda caras en listas. Mira primero si debe o no guardar vn y vt.
        private void save_face(string a, string b, string c, bool cvn)
        {
            tuple_3[] tuple_array = new tuple_3[3];
            if (a.Contains("/"))
            {
                string[] f1 = a.Split('/');
                string[] f2 = b.Split('/');
                string[] f3 = c.Split('/');

                for (int i = 0; i < 3; i++)
                {
                    if (f1[i] == "") f1[i] = "0.0";
                    if (f2[i] == "") f2[i] = "0.0";
                    if (f3[i] == "") f3[i] = "0.0";
                }

                tuple_array[0] = new tuple_3 { x = float.Parse(f1[0]), y = float.Parse(f2[0]), z = float.Parse(f3[0]) };
                tuple_array[1] = new tuple_3 { x = float.Parse(f1[1]), y = float.Parse(f2[1]), z = float.Parse(f3[1]) };
                tuple_array[2] = new tuple_3 { x = float.Parse(f1[2]), y = float.Parse(f2[2]), z = float.Parse(f3[2]) };
                faces.Add(tuple_array);
            }
            else
            {
                float x = float.Parse(a);
                float y = float.Parse(b);
                float z = float.Parse(c);
                tuple_array[0] = new tuple_3 { x = x, y = y, z = z };
                faces.Add(tuple_array);
            }
        }

        //Guarda los valores de las texturas.
        private void save_vt(string a, string b, string c, int index)
        {
            float x = float.Parse(a);
            float y = float.Parse(b);
            float z = float.Parse(c);
            vt_dic[index] = new tuple_3 { x = x, y = y, z = z };
        }

        //Guarda los valores de las normales.
        private void save_vn(string a, string b, string c, int index)
        {
            float x = float.Parse(a);
            float y = float.Parse(b);
            float z = float.Parse(c);
            vn_dic[index] = new tuple_3 { x = x, y = y, z = z };
        }
        
        //Para cada triangulo, computa su normal y luego guarda la info de la normal de los vertices.
        private void compute_vertex_normals()
        {
            foreach (tuple_3[] face in faces) 
            {
                tuple_3 v1 = vertex[(int)face[0].x];
                tuple_3 v2 = vertex[(int)face[0].y];
                tuple_3 v3 = vertex[(int)face[0].z];

                //Saco la normal de la cara, y la sumo a la normal asignada a cada vector en su diccionario.
                tuple_3 normal = Vectores.Normalize(Vectores.cross((v2 - v1), (v3 - v1)));

                if (!vertex_normals.ContainsKey((int)face[0].x))
                    vertex_normals.Add((int)face[0].x, normal);
                else if (vertex_normals.ContainsKey((int)face[0].x))
                    vertex_normals[(int)face[0].x] = vertex_normals[(int)face[0].x] + normal;
                    
                if (!vertex_normals.ContainsKey((int)face[0].y))
                    vertex_normals.Add((int)face[0].y, normal);
                else if (vertex_normals.ContainsKey((int)face[0].y))
                    vertex_normals[(int)face[0].y] = vertex_normals[(int)face[0].y] + normal;
                
                if (!vertex_normals.ContainsKey((int)face[0].z))
                    vertex_normals.Add((int)face[0].z, normal);
                else if (vertex_normals.ContainsKey((int)face[0].z))
                    vertex_normals[(int)face[0].z] = vertex_normals[(int)face[0].z] + normal;
            }   
        }
    }
}
