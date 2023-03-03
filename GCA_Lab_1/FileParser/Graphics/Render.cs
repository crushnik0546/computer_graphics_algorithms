using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MainLogic.Graphics
{
    public static class Render
    {
        public static ObjModel _model;
        public static List<Vector4> _projectedVertices;
        private static Vector3 cameraPosition = new Vector3(5f, 90f, 0f);
 
        public static void Init(string model)
        {            
            var objFileReader = new ObjReader(model);
            _model = objFileReader.ReadObjFile();
            _projectedVertices = new List<Vector4>();
        }

        public static Vector3 GetCameraPosition()
        {
            return cameraPosition;
        }

        public static void SetCameraPosition(Vector3 new_position)
        {
            if (Math.Abs(new_position.Y) < 180 && Math.Abs(new_position.Y) > 0 && new_position.X >= 0.05 && new_position.X <= 10000)
            {
                cameraPosition = new_position;
            }
        }

        public static void RenderModel()
        {
            _projectedVertices.Clear();
            Matrix4x4 worldMatrix = Logic.CreateScaleMatrix(1f, 1f, 1f) * Logic.CreateRotationMatrix(0f, 0f, 0f) * Logic.CreateTranslationMatrix(0f, 0f, 0f);
            Matrix4x4 observerMatrix = Logic.CreateObserverMatrix();
            Matrix4x4 projectionMatrix = Logic.CreateProjectionMatrix();
            Matrix4x4 viewportMatrix = Logic.CreateViewportMatrix();
            //Matrix4x4 atrix = worldMatrix * observerMatrix * projectionMatrix * viewportMatrix;

            Matrix4x4 tmp = worldMatrix * observerMatrix * projectionMatrix;
            foreach (var vertex in _model.GeometricVertices)
            {
                Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                var point = Vector4.Transform(vector, tmp);
                
                // var point = Vector4.Transform(vector, atrix);
                var point1 = new Vector4(point.X / point.W, point.Y / point.W, point.Z / point.W, point.W / point.W);

                if (point1.X >= -1 && point1.X <= 1 && point1.Y >= -1 && point1.Y <= 1 && point1.Z >= -1 && point1.Z <= 1)
                {
                    point1 = Vector4.Transform(point1, viewportMatrix);
                    _projectedVertices.Add(point1);
                } else
                {
                    _projectedVertices.Add(new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity));
                }
            }
        }
    }
}
