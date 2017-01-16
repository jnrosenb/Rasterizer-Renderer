using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ejemplo2;

namespace Ejemplo2
{
    public class Camera
    {
        public float FOV { get; set; }
        public float near { get; set; }
        public float far { get; set; }
        public tuple_3 position { get; set; }
        public tuple_3 up { get; set; }
        public tuple_3 target { get; set; }

        public Camera(float fov, tuple_3 position, tuple_3 up, tuple_3 target, float near, float far) 
        {
            this.FOV = fov;
            this.position = position;
            this.up = up;
            this.target = target;

            this.near = near;
            this.far = far;
        }

    }
}
