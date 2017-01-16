using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ejemplo2;

namespace Ejemplo2
{

    public class Light 
    {
        public string name;
    }

    public class PointLight : Light
    {
        public tuple_3 position;
        public List<float> color;

        public PointLight(tuple_3 position, List<float> color) 
        {
            this.position = position;
            this.color = color;
            this.name = "point_light";
        }
    }

    public class AmbientLight : Light 
    {
        public List<float> color;
        public AmbientLight(List<float> amb_color)
        {
            this.name = "ambient_light";
            this.color = amb_color;
        }
    }
}
