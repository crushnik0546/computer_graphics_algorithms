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
        public static List<Vector3> normalVectorsPolygons;
        public static List<List<float>> zbuffer = new List<List<float>>();

        private static Vector3 cameraPosition = new Vector3(5f, 90f, 0f);
        private static Matrix4x4 worldMatrix;
        private static Matrix4x4 projectionMatrix;
        private static Matrix4x4 viewportMatrix;


        public static void Init(string model)
        {
            var objFileReader = new ObjReader(model);
            _model = objFileReader.ReadObjFile();
            _projectedVertices = new List<Vector4>();
            worldMatrix = Logic.CreateScaleMatrix(1f, 1f, 1f) * Logic.CreateRotationMatrix(0f, 0f, 0f) * Logic.CreateTranslationMatrix(0f, 0f, 0f);
            projectionMatrix = Logic.CreateProjectionMatrix();
            viewportMatrix = Logic.CreateViewportMatrix();
            CalcNormalVectors();

            for (int y = 0; y <= 1080; y++)
            {
                List<float> zbufferRow = new List<float>();
                for (int x = 0; x <= 1920; x++)
                {
                    zbufferRow.Add(float.PositiveInfinity);
                }
                zbuffer.Add(zbufferRow);
            }
        }

        private static void CalcNormalVectors()
        {
            normalVectorsPolygons = new List<Vector3>();
            foreach (var polygon in _model.Poligons)
            {
                List<Vector3> vertices = new List<Vector3>();
                for (int i = 0; i < 3; i++)
                {
                    Vector4 modelVertex = new Vector4(_model.GeometricVertices[polygon.GeometricVerticesNumber[i] - 1].X,
                                                      _model.GeometricVertices[polygon.GeometricVerticesNumber[i] - 1].Y,
                                                      _model.GeometricVertices[polygon.GeometricVerticesNumber[i] - 1].Z,
                                                      _model.GeometricVertices[polygon.GeometricVerticesNumber[i] - 1].W);

                    Vector4 tmp = Vector4.Transform(modelVertex, worldMatrix);
                    vertices.Add(new Vector3(tmp.X, tmp.Y, tmp.Z));
                }

                Vector3 normalVector = Vector3.Normalize(Vector3.Cross(vertices[2] - vertices[0], vertices[1] - vertices[0]));
                normalVectorsPolygons.Add(normalVector);
            }
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
            for (int y = 0; y <= 1080; y++)
            {
                for (int x = 0; x <= 1920; x++)
                {
                    zbuffer[y][x] = float.PositiveInfinity;
                }
            }

            _projectedVertices.Clear();
            Matrix4x4 observerMatrix = Logic.CreateObserverMatrix();

            Matrix4x4 tmp = worldMatrix * observerMatrix * projectionMatrix;
            foreach (var vertex in _model.GeometricVertices)
            {
                Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                var point = Vector4.Transform(vector, tmp);

                var point1 = new Vector4(point.X / point.W, point.Y / point.W, point.Z / point.W, point.W / point.W);

                if (point1.X >= -1 && point1.X <= 1 && point1.Y >= -1 && point1.Y <= 1 && point1.Z >= -1 && point1.Z <= 1)
                {
                    point1 = Vector4.Transform(point1, viewportMatrix);
                    _projectedVertices.Add(point1);
                }
                else
                {
                    _projectedVertices.Add(new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity));
                }
            }
        }
    }
}