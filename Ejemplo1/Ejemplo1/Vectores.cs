using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ejemplo2
{

    public struct tuple_3
    {
        public float x;
        public float y;
        public float z;

        //Override simbolo suma (+).
        public static tuple_3 operator +(tuple_3 A, tuple_3 B)
        {
            return new tuple_3 { x = A.x + B.x, y = A.y + B.y, z = A.z + B.z };
        }

        //Override simbolo resta (-).
        public static tuple_3 operator -(tuple_3 A, tuple_3 B)
        {
            return new tuple_3 { x = A.x - B.x, y = A.y - B.y, z = A.z - B.z };
        }

        //Override simbolo resta (-).
        public static tuple_3 operator -(tuple_3 A)
        {
            return new tuple_3 { x = -A.x, y = -A.y, z = -A.z };
        }

        //Override simbolo mult (*).
        public static tuple_3 operator *(float a, tuple_3 B)
        {
            return new tuple_3 { x = a * B.x, y = a * B.y, z = a * B.z };
        }

        //Override simbolo mult (*).
        public static tuple_3 operator *(tuple_3 A, tuple_3 B)
        {
            return new tuple_3 { x = A.x * B.x, y = A.y * B.y, z = A.z * B.z };
        }

        //Override simbolo div (/).
        public static tuple_3 operator /(tuple_3 A, float b)
        {
            if (b != 0.0f)
                return new tuple_3 { x = A.x / b, y = A.y / b, z = A.z / b };
            else
                return new tuple_3();
        }
        
        //Override simbolo eq (==).
        public static bool operator ==(tuple_3 A, tuple_3 B)
        {
            return (A.x == B.x && A.y == B.y && A.z == B.z );
        }
        
        //Override simbolo not eq (!=).
        public static bool operator !=(tuple_3 A, tuple_3 B)
        {
            return (A.x != B.x || A.y != B.y || A.z != B.z);
        }
    }

    public struct tuple_4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        //Override simbolo suma (+).
        public static tuple_4 operator +(tuple_4 A, tuple_4 B)
        {
            return new tuple_4 { x = A.x + B.x, y = A.y + B.y, z = A.z + B.z, w = A.w + B.w };
        }

        //Override simbolo resta (-).
        public static tuple_4 operator -(tuple_4 A, tuple_4 B)
        {
            return new tuple_4 { x = A.x - B.x, y = A.y - B.y, z = A.z - B.z, w = A.w - B.w };
        }

        //Override simbolo mult (*).
        public static tuple_4 operator *(float a, tuple_4 B)
        {
            return new tuple_4 { x = a * B.x, y = a * B.y, z = a * B.z, w = a * B.w };
        }
        
        //Override simbolo mult (*).
        public static tuple_4 operator *(tuple_4 B, float a)
        {
            return new tuple_4 { x = a * B.x, y = a * B.y, z = a * B.z, w = a * B.w };
        }

        //Override simbolo mult (*).
        public static tuple_4 operator *(tuple_4 A, tuple_4 B)
        {
            return new tuple_4 { x = A.x * B.x, y = A.y * B.y, z = A.z * B.z, w = A.w * B.w };
        }

        //Override simbolo div (/).
        public static tuple_4 operator /(tuple_4 A, float b)
        {
            if (b != 0.0f)
                return new tuple_4 { x = A.x / b, y = A.y / b, z = A.z / b, w = A.w / b };
            else
                Console.WriteLine("Error en division de vector 4 por escalar");
            return new tuple_4();
        }
    }

    public struct color_3
    {
        public float R;
        public float G;
        public float B;
    }
     

    public static class Vectores
    {
        //Suma de vectores  (x1, y1, z1) + (x2, y2, z2) = (x1 + x2, y1 + y2, z1 + z2)
        public static tuple_3 Sum(tuple_3 A, tuple_3 B) 
        {
            return new tuple_3{x = A.x + B.x, y = A.y + B.y, z = A.z + B.z};
        }

        //Resta de vectores (x1, y1, z1) - (x2, y2, z2) = (x1 - x2, y1 - y2, z1 - z2) 
        public static tuple_3 Sub(tuple_3 A, tuple_3 B)
        {
            return new tuple_3 { x = A.x - B.x, y = A.y - B.y, z = A.z - B.z };
        }

        //Ponderacion escalar por vector s * (x1, y1, z1) = (s*x1, s*y1, s*z1)
        public static tuple_3 ScalarMult(float s, tuple_3 A)
        {
            return new tuple_3 { x = s * A.x, y = s * A.y, z = s * A.z };
        }

        //Division vector por escalar (x1, y1, z1)/s = (x1/s, y1/s, z1/s)
        public static tuple_3 ScalarDiv(float s, tuple_3 A)
        {
            if (s != 0.0f)
                return new tuple_3 { x = A.x / s, y = A.y / s, z = A.z / s };
            return new tuple_3();
        }


        //Multiplicacion de vectores  (x1, y1, z1) * (x2, y2, z2) = (x1 * x2, y1 * y2, z1 * z1) 
        public static tuple_3 Mult(tuple_3 A, tuple_3 B)
        {
            return new tuple_3 { x = A.x * B.x, y = A.y * B.y, z = A.z * B.z };
        }

        //Magnitud de un vector mag((x1, y1, z1)) = sqrt(x1 * x1 + y1 * y1 + z1 * z1)
        public static float Module(tuple_3 A)
        {
            return (float)Math.Sqrt((float)Math.Pow(A.x, 2) + (float)Math.Pow(A.y, 2) + (float)Math.Pow(A.z, 2));
        }

        //Normalizar un vector norm((x1, y1, z1)) = (x1, y1, z1) / mag((x1, y1, z1))
        public static tuple_3 Normalize(tuple_3 A)
        {
            float magnitude = Module(A);
            if (magnitude == 0)
                return new tuple_3 { x = 0.0f, y = 0.0f, z = 0.0f };
            return new tuple_3 { x = A.x / magnitude, y = A.y / magnitude, z = A.z / magnitude };
        }

        //Producto cruz (x1, y1, z1) x (x2, y2, z2) = (y1*z2 - y2*z1, x2*z1 - x1*z2, x1*y2 - x2*y1)
        public static tuple_3 cross(tuple_3 A, tuple_3 B)
        { 
            return new tuple_3 { x = A.y * B.z - B.y * A.z, 
                                 y = B.x * A.z - A.x * B.z, 
                                 z = A.x * B.y - B.x * A.y };
        }

        //Producto punto  (x1, y1, z1) . (x2, y2, z2) = x1 * x2 + y1 * y2 + z1 * z2)
        public static float dot(tuple_3 A, tuple_3 B)
        {
            return  (A.x * B.x) + (A.y * B.y) + (A.z * B.z);
        }
        
        //Imprime el vector.
        public static void Print(tuple_3 A) 
        {
            Console.WriteLine("({0}, {1}, {2})", A.x, A.y, A.z);
        }
    }

    public static class Vectores2
    {
        //Suma de vectores  (x1, y1, z1) + (x2, y2, z2) = (x1 + x2, y1 + y2, z1 + z2)
        public static List<float> vectorSum(List<float> A, List<float> B)
        {
            return new List<float> { A[0] + B[0], A[1] + B[1], A[2] + B[2] };
        }

        //Resta de vectores (x1, y1, z1) - (x2, y2, z2) = (x1 - x2, y1 - y2, z1 - z2) 
        public static List<float> vectorSub(List<float> A, List<float> B)
        {
            return new List<float> { A[0] - B[0], A[1] - B[1], A[2] - B[2] };
        }

        //Ponderacion escalar por vector s * (x1, y1, z1) = (s*x1, s*y1, s*z1)
        public static List<float> vectorEscMult(float s, List<float> A)
        {
            return new List<float> { s * A[0], s * A[1], s * A[2] };
        }

        //Division vector por escalar (x1, y1, z1)/s = (x1/s, y1/s, z1/s)
        public static List<float> vectorEscDiv(float s, List<float> A)
        {
            if (s != 0.0f)
                return new List<float> { A[0] / s, A[1] / s, A[2] / s };
            return new List<float>();
        }

        //Multiplicacion de vectores  (x1, y1, z1) * (x2, y2, z2) = (x1 * x2, y1 * y2, z1 * z1) 
        public static List<float> vectorMult(List<float> A, List<float> B)
        {
            return new List<float> { A[0] * B[0], A[1] * B[1], A[2] * B[2] };
        }

        //Magnitud de un vector mag((x1, y1, z1)) = sqrt(x1 * x1 + y1 * y1 + z1 * z1)
        public static float vectorMagnitude(List<float> A)
        {
            return (float)Math.Sqrt(Math.Pow(A[0], 2) + Math.Pow(A[1], 2) + Math.Pow(A[2], 2));
        }

        //Normalizar un vector norm((x1, y1, z1)) = (x1, y1, z1) / mag((x1, y1, z1))
        public static List<float> vectorNormalize(List<float> A)
        {
            float magnitude = vectorMagnitude(A);
            return new List<float> { A[0] / magnitude, A[1] / magnitude, A[2] / magnitude };
        }

        //Producto punto  (x1, y1, z1) . (x2, y2, z2) = x1 * x2 + y1 * y2 + z1 * z2) 
        public static float vectorPointProduct(List<float> A, List<float> B)
        {
            return (A[0] * B[0]) + (A[1] * B[1]) + (A[2] * B[2]);
        }

        //Producto cruz (x1, y1, z1) x (x2, y2, z2) = (y1*z2 - y2*z1, x2*z1 - x1*z2, x1*y2 - x2*y1)
        public static List<float>  vectorCrossProduct(List<float> A, List<float> B)
        {
            return new List<float> { A[1] * B[2] - B[1] * A[2], B[0] * A[2] - A[0] * B[2], A[0] * B[1] - B[0] * A[1] };
        }

        //Imprime el vector.
        public static void vectorPrint(List<float> A)
        {
            Console.WriteLine("({0}, {1}, {2})", A[0], A[1], A[2]);
        }
    }

    //Struct que va a descibir las matrice de 4x4:
    public struct matrix_4
    {
        public float a00; public float a01; public float a02; public float a03;
        public float a10; public float a11; public float a12; public float a13;
        public float a20; public float a21; public float a22; public float a23;
        public float a30; public float a31; public float a32; public float a33; 

        //Override simbolo suma (+).
        public static matrix_4 operator +(matrix_4 A, matrix_4 B)
        {
            return new matrix_4 { a00 = A.a00 + B.a00, a01 = A.a01 + B.a01, a02 = A.a02 + B.a02, a03 = A.a03 + B.a03,
                                  a10 = A.a10 + B.a10, a11 = A.a11 + B.a11, a12 = A.a12 + B.a12, a13 = A.a13 + B.a13,
                                  a20 = A.a20 + B.a20, a21 = A.a21 + B.a21, a22 = A.a22 + B.a22, a23 = A.a23 + B.a23,
                                  a30 = A.a30 + B.a30, a31 = A.a31 + B.a31, a32 = A.a32 + B.a32, a33 = A.a33 + B.a33
            };
        }

        //Override simbolo resta (-).
        public static matrix_4 operator -(matrix_4 A, matrix_4 B)
        {
            return new matrix_4 { a00 = A.a00 - B.a00, a01 = A.a01 - B.a01, a02 = A.a02 - B.a02, a03 = A.a03 - B.a03,
                                  a10 = A.a10 - B.a10, a11 = A.a11 - B.a11, a12 = A.a12 - B.a12, a13 = A.a13 - B.a13,
                                  a20 = A.a20 - B.a20, a21 = A.a21 - B.a21, a22 = A.a22 - B.a22, a23 = A.a23 - B.a23,
                                  a30 = A.a30 - B.a30, a31 = A.a31 - B.a31, a32 = A.a32 - B.a32, a33 = A.a33 - B.a33
            };
        }

        //Override simbolo mult (*).
        public static matrix_4 operator *(float a, matrix_4 A)
        {
            return new matrix_4 { a00 = A.a00 * a, a01 = A.a01 * a, a02 = A.a02 * a, a03 = A.a03 * a,
                                  a10 = A.a10 * a, a11 = A.a11 * a, a12 = A.a12 * a, a13 = A.a13 * a,
                                  a20 = A.a20 * a, a21 = A.a21 * a, a22 = A.a22 * a, a23 = A.a23 * a,
                                  a30 = A.a30 * a, a31 = A.a31 * a, a32 = A.a32 * a, a33 = A.a33 * a
            };
        }

        //Override simbolo mult (*).
        public static matrix_4 operator *(matrix_4 A, float a)
        {
            return new matrix_4 { a00 = A.a00 * a, a01 = A.a01 * a, a02 = A.a02 * a, a03 = A.a03 * a,
                                  a10 = A.a10 * a, a11 = A.a11 * a, a12 = A.a12 * a, a13 = A.a13 * a,
                                  a20 = A.a20 * a, a21 = A.a21 * a, a22 = A.a22 * a, a23 = A.a23 * a,
                                  a30 = A.a30 * a, a31 = A.a31 * a, a32 = A.a32 * a, a33 = A.a33 * a
            };
        }

        //Override simbolo mult (*).
        public static tuple_4 operator *(matrix_4 A, tuple_4 B)
        {
            return new tuple_4 {    x = A.a00 * B.x + A.a01 * B.y + A.a02 * B.z + A.a03 * B.w,
                                    y = A.a10 * B.x + A.a11 * B.y + A.a12 * B.z + A.a13 * B.w,
                                    z = A.a20 * B.x + A.a21 * B.y + A.a22 * B.z + A.a23 * B.w,
                                    w = A.a30 * B.x + A.a31 * B.y + A.a32 * B.z + A.a33 * B.w
            };
        }

        //Override simbolo mult (*).
        public static matrix_4 operator *(matrix_4 A, matrix_4 B)
        {   
            //  a00,  a01,  a02,  a03           //  a00,  a01,  a02,  a03 
            //  a10,  a11,  a12,  a13           //  a10,  a11,  a12,  a13 
            //  a20,  a21,  a22,  a23           //  a20,  a21,  a22,  a23 
            //  a30,  a31,  a32,  a33           //  a30,  a31,  a32,  a33 

            return new matrix_4
            {
                a00 = A.a00 * B.a00 + A.a01 * B.a10 + A.a02 * B.a20 + A.a03 * B.a30,
                a01 = A.a00 * B.a01 + A.a01 * B.a11 + A.a02 * B.a21 + A.a03 * B.a31,
                a02 = A.a00 * B.a02 + A.a01 * B.a12 + A.a02 * B.a22 + A.a03 * B.a32,
                a03 = A.a00 * B.a03 + A.a01 * B.a13 + A.a02 * B.a23 + A.a03 * B.a33,

                a10 = A.a10 * B.a00 + A.a11 * B.a10 + A.a12 * B.a20 + A.a13 * B.a30,
                a11 = A.a10 * B.a01 + A.a11 * B.a11 + A.a12 * B.a21 + A.a13 * B.a31,
                a12 = A.a10 * B.a02 + A.a11 * B.a12 + A.a12 * B.a22 + A.a13 * B.a32,
                a13 = A.a10 * B.a03 + A.a11 * B.a13 + A.a12 * B.a23 + A.a13 * B.a33,
                
                a20 = A.a20 * B.a00 + A.a21 * B.a10 + A.a22 * B.a20 + A.a23 * B.a30,
                a21 = A.a20 * B.a01 + A.a21 * B.a11 + A.a22 * B.a21 + A.a23 * B.a31,
                a22 = A.a20 * B.a02 + A.a21 * B.a12 + A.a22 * B.a22 + A.a23 * B.a32,
                a23 = A.a20 * B.a03 + A.a21 * B.a13 + A.a22 * B.a23 + A.a23 * B.a33,
                
                a30 = A.a30 * B.a00 + A.a31 * B.a10 + A.a32 * B.a20 + A.a33 * B.a30,
                a31 = A.a30 * B.a01 + A.a31 * B.a11 + A.a32 * B.a21 + A.a33 * B.a31,
                a32 = A.a30 * B.a02 + A.a31 * B.a12 + A.a32 * B.a22 + A.a33 * B.a32,
                a33 = A.a30 * B.a03 + A.a31 * B.a13 + A.a32 * B.a23 + A.a33 * B.a33,
            };
        }

        //Override simbolo div (/).
        public static matrix_4 operator /(matrix_4 A, float a)
        {
            return new matrix_4 { a00 = A.a00 / a, a01 = A.a01 / a, a02 = A.a02 / a, a03 = A.a03 / a,
                                  a10 = A.a10 / a, a11 = A.a11 / a, a12 = A.a12 / a, a13 = A.a13 / a,
                                  a20 = A.a20 / a, a21 = A.a21 / a, a22 = A.a22 / a, a23 = A.a23 / a,
                                  a30 = A.a30 / a, a31 = A.a31 / a, a32 = A.a32 / a, a33 = A.a33 / a
            };
        }

        //Override simbolo div (/).
        public static matrix_4 operator /(float a, matrix_4 A)
        {
            return new matrix_4 { a00 = A.a00 / a, a01 = A.a01 / a, a02 = A.a02 / a, a03 = A.a03 / a,
                                  a10 = A.a10 / a, a11 = A.a11 / a, a12 = A.a12 / a, a13 = A.a13 / a,
                                  a20 = A.a20 / a, a21 = A.a21 / a, a22 = A.a22 / a, a23 = A.a23 / a,
                                  a30 = A.a30 / a, a31 = A.a31 / a, a32 = A.a32 / a, a33 = A.a33 / a
            };
        }

        //Override simbolo eq (==).
        public static bool operator ==(matrix_4 A, matrix_4 B)
        {
            return 
            (
                A.a00 == B.a00 &&
                A.a01 == B.a01 &&
                A.a02 == B.a02 &&
                A.a03 == B.a03 &&
                A.a10 == B.a10 &&
                A.a11 == B.a11 &&
                A.a12 == B.a12 &&
                A.a13 == B.a13 &&
                A.a20 == B.a20 &&
                A.a21 == B.a21 &&
                A.a22 == B.a22 &&
                A.a23 == B.a23 &&
                A.a30 == B.a30 &&
                A.a31 == B.a31 &&
                A.a32 == B.a32 &&
                A.a33 == B.a33
            );
        }

        //Override simbolo not eq (!=).
        public static bool operator !=(matrix_4 A, matrix_4 B)
        {
            return
            (
                A.a00 != B.a00 ||
                A.a01 != B.a01 ||
                A.a02 != B.a02 ||
                A.a03 != B.a03 ||
                A.a10 != B.a10 ||
                A.a11 != B.a11 ||
                A.a12 != B.a12 ||
                A.a13 != B.a13 ||
                A.a20 != B.a20 ||
                A.a21 != B.a21 ||
                A.a22 != B.a22 ||
                A.a23 != B.a23 ||
                A.a30 != B.a30 ||
                A.a31 != B.a31 ||
                A.a32 != B.a32 ||
                A.a33 != B.a33
            );
        }
    }

    public static class Matrix 
    {
        //Retorna la matriz inversa
        public static matrix_4 inverse(matrix_4 A)
        {
            matrix_4 cofact = cofactor(A);
            matrix_4 adj = traspond(cofact);
            float dt = det(A);
            
            return (1.0f / dt) * adj;
        }
        
        
        //Retorna la matriz traspuesta
        public static matrix_4 traspond(matrix_4 A)
        {
            return new matrix_4 { a00 = A.a00, a01 = A.a10, a02 = A.a20, a03 = A.a30,
                                  a10 = A.a01, a11 = A.a11, a12 = A.a21, a13 = A.a31,
                                  a20 = A.a02, a21 = A.a12, a22 = A.a22, a23 = A.a32,
                                  a30 = A.a03, a31 = A.a13, a32 = A.a23, a33 = A.a33
            };
        }

        //Retorna el determinante de la matriz de 4x4:
        public static float det(matrix_4 A)
        {
            //Solo para ayuda visual:
            //  a00,  a01,  a02,  a03 
            //  a10,  a11,  a12,  a13 
            //  a20,  a21,  a22,  a23 
            //  a30,  a31,  a32,  a33 

            float f0 = + (A.a00) * det3(A.a11, A.a12, A.a13, A.a21, A.a22, A.a23, A.a31, A.a32, A.a33);
            float f1 = - (A.a10) * det3(A.a01, A.a02, A.a03, A.a21, A.a22, A.a23, A.a31, A.a32, A.a33);
            float f2 = + (A.a20) * det3(A.a01, A.a02, A.a03, A.a11, A.a12, A.a13, A.a31, A.a32, A.a33);
            float f3 = - (A.a30) * det3(A.a01, A.a02, A.a03, A.a11, A.a12, A.a13, A.a21, A.a22, A.a23); 

            return (f0 + f1 + f2 + f3);
        }

        //Retorna la matriz de cofactores:
        public static matrix_4 cofactor(matrix_4 A)
        {    
            //Solo para ayuda visual:
            //  a00,  a01,  a02,  a03 
            //  a10,  a11,  a12,  a13 
            //  a20,  a21,  a22,  a23 
            //  a30,  a31,  a32,  a33 

            return new matrix_4
            {
                a00 = + det3(A.a11, A.a12, A.a13, A.a21, A.a22, A.a23, A.a31, A.a32, A.a33),
                a01 = - det3(A.a10, A.a12, A.a13, A.a20, A.a22, A.a23, A.a30, A.a32, A.a33),
                a02 = + det3(A.a10, A.a11, A.a13, A.a20, A.a21, A.a23, A.a30, A.a31, A.a33),
                a03 = - det3(A.a10, A.a11, A.a12, A.a20, A.a21, A.a22, A.a30, A.a31, A.a32),

                a10 = - det3(A.a01, A.a02, A.a03, A.a21, A.a22, A.a23, A.a31, A.a32, A.a33),
                a11 = + det3(A.a00, A.a02, A.a03, A.a20, A.a22, A.a23, A.a30, A.a32, A.a33),
                a12 = - det3(A.a00, A.a01, A.a03, A.a20, A.a21, A.a23, A.a30, A.a31, A.a33),
                a13 = + det3(A.a00, A.a01, A.a02, A.a20, A.a21, A.a22, A.a30, A.a31, A.a32),

                a20 = + det3(A.a01, A.a02, A.a03, A.a11, A.a12, A.a13, A.a31, A.a32, A.a33),
                a21 = - det3(A.a00, A.a02, A.a03, A.a10, A.a12, A.a13, A.a30, A.a32, A.a33),
                a22 = + det3(A.a00, A.a01, A.a03, A.a10, A.a11, A.a13, A.a30, A.a31, A.a33),
                a23 = - det3(A.a00, A.a01, A.a02, A.a10, A.a11, A.a12, A.a30, A.a31, A.a32),

                a30 = - det3(A.a01, A.a02, A.a03, A.a11, A.a12, A.a13, A.a21, A.a22, A.a23),
                a31 = + det3(A.a00, A.a02, A.a03, A.a10, A.a12, A.a13, A.a20, A.a22, A.a23),
                a32 = - det3(A.a00, A.a01, A.a03, A.a10, A.a11, A.a13, A.a20, A.a21, A.a23),
                a33 = + det3(A.a00, A.a01, A.a02, A.a10, A.a11, A.a12, A.a20, A.a21, A.a22)
            };
        }

        public static float det3(float a11, float a12, float a13, float a21, float a22, float a23, float a31, float a32, float a33) 
        {
            //   a11,  a12,  a13  
            //   a21,  a22,  a23  
            //   a31,  a32,  a33  

            float f1 = + (a11) * dt(a22, a23, a32, a33);
            float f2 = - (a21) * dt(a12, a13, a32, a33);
            float f3 = + (a31) * dt(a12, a13, a22, a23);
            return (f1 + f2 + f3);
        }

        //No obtiene determinante, Obtiene un ponderador por el determinante de una matriz 2x2.
        public static float dt(float a11, float a12, float a21, float a22)
        {
            return  (a11 * a22 - a12 * a21);
        }
    }
}
