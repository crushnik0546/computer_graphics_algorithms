using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainLogic.Components;

namespace MainLogic.Graphics
{
    public static class Logic
    {
        public static Matrix4x4 CreateTranslationMatrix(float translX, float translY, float translZ)
        {
            return Matrix4x4.Transpose(new Matrix4x4(
                1f, 0f, 0f, translX,
                0f, 1f, 0f, translY,
                0f, 0f, 1f, translZ,
                0f, 0f, 0f, 1f));
        }

        public static Matrix4x4 CreateScaleMatrix(float scaleX, float scaleY, float scaleZ)
        {
            return Matrix4x4.Transpose(new Matrix4x4(
                scaleX, 0f, 0f, 0f,
                0f, scaleY, 0f, 0f,
                0f, 0f, scaleZ, 0f,
                0f, 0f, 0f, 1.0f));
        }

        public static Matrix4x4 CreateRotationMatrix(float x, float y, float z)
        {
            float cosX = (float)Math.Cos(x);
            float sinX = (float)Math.Sin(x);
            float cosY = (float)Math.Cos(y);
            float sinY = (float)Math.Sin(y);
            float cosZ = (float)Math.Cos(z);
            float sinZ = (float)Math.Sin(z);

            Matrix4x4 rotateX = Matrix4x4.Transpose(new Matrix4x4(
                1.0f, 0f, 0f, 0f,
                0f, cosX, -sinX, 0f,
                0f, sinX, cosX, 0f,
                0f, 0f, 0f, 1f));

            Matrix4x4 rotateY = Matrix4x4.Transpose(new Matrix4x4(
                cosY, 0f, sinY, 0f,
                0f, 1f, 0f, 0f,
                -sinY, 0f, cosY, 0f,
                0f, 0f, 0f, 1f));

            Matrix4x4 rotateZ = Matrix4x4.Transpose(new Matrix4x4(
                cosZ, -sinZ, 0f, 0f,
                sinZ, cosZ, 0f, 0f,
                0f, 0f, 1f, 0f,
                0f, 0f, 0f, 1f));

            return rotateX * rotateY * rotateZ;
        }

        public static Matrix4x4 CreateObserverMatrix()
        {
            // (r, teta, fi)
            // (x,    y,  z)
            Vector3 eye_sphere = Render.GetCameraPosition();
            Vector3 eye = new Vector3(0, 0, 0);
            // Convert sphere coordinates to dekart
            eye.X = eye_sphere.X * (float)Math.Sin(eye_sphere.Y * Math.PI / 180) * (float)Math.Sin(eye_sphere.Z * Math.PI / 180);
            eye.Y = eye_sphere.X * (float)Math.Cos(eye_sphere.Y * Math.PI / 180);
            eye.Z = eye_sphere.X * (float)Math.Sin(eye_sphere.Y * Math.PI / 180) * (float)Math.Cos(eye_sphere.Z * Math.PI / 180);

            Vector3 target = new Vector3(0f, 0f, 0f);
            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 zAxis = Vector3.Normalize(eye - target);
            Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
            Vector3 yAxis = Vector3.Normalize(Vector3.Cross(xAxis, -zAxis));

            return Matrix4x4.Transpose(new Matrix4x4(
                xAxis.X, xAxis.Y, xAxis.Z, -Vector3.Dot(xAxis, eye),
                yAxis.X, yAxis.Y, yAxis.Z, -Vector3.Dot(yAxis, eye),
                zAxis.X, zAxis.Y, zAxis.Z, -Vector3.Dot(zAxis, eye),
                0f, 0f, 0f, 1f));
        }
        public static Matrix4x4 CreateProjectionMatrix()
        {
            float tan = (float)Math.Tan(1.5f / 2);
            float zNear = 0.001f;
            float zFar = 100000f;
            float aspect = 1920f / 1080;

            return Matrix4x4.Transpose(new Matrix4x4(
                1.0f / (aspect * tan), 0f, 0f, 0f,
                0f, 1.0f / tan, 0f, 0f,
                0f, 0f, zFar / (zNear - zFar), zFar * zNear / (zNear - zFar),
                0f, 0f, -1.0f, 0f));            
        }

        public static Matrix4x4 CreateViewportMatrix()
        {
            float xMin = 0f;
            float yMin = 0f;

            return new Matrix4x4(
                1920.0f / 2, 0f, 0f, 0f,
                0f, -1080.0f / 2, 0f, 0,
                0f, 0f, 1.0f, 0f,
                0f + 1920.0f / 2, 0f + 1080.0f / 2, 0f, 1.0f);
        }

        public static Vector4 MulAndGetVector(Vector4 vector, Matrix4x4 matrix)
        {
            return new Vector4(
                vector.X * matrix.M11 + vector.Y * matrix.M12 + vector.Z * matrix.M13 + vector.W * matrix.M14,
                vector.X * matrix.M21 + vector.Y * matrix.M22 + vector.Z * matrix.M23 + vector.W * matrix.M24,
                vector.X * matrix.M31 + vector.Y * matrix.M32 + vector.Z * matrix.M33 + vector.W * matrix.M34,
                vector.X * matrix.M41 + vector.Y * matrix.M42 + vector.Z * matrix.M43 + vector.W * matrix.M44);
        }
    }
}
