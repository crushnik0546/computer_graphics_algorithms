using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;

using System.Numerics;
using MainLogic;
using MainLogic.Graphics;
using System.Drawing;

namespace Drawing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Grid Grid;
        private byte[] _backBuffer;
        public WriteableBitmap RenderBitmap;

        public Vector3 modelColor = new Vector3(0.05f, 0.05f, 0.05f);
        
        public static string PathToFile = @"..\..\..\..\models\ship\source\Model.obj";
        public MainWindow()
        {
            InitializeComponent();
            Grid = MainGrid;
            RenderBitmap = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgra32, null);
            _backBuffer = new byte[RenderBitmap.PixelWidth * RenderBitmap.PixelHeight * 4];
            RenderImage.Source = RenderBitmap;

            //Render.Init(@"..\..\..\..\models\cube\source\cube.obj");
            Render.Init(PathToFile);
            //Render.Init(@"..\..\..\..\models\shrek\source\shrek_shrinked.obj");
            //Render.Init(@"..\..\..\..\models\dinosaur\source\deino.obj");
            //Render.Init(@"..\..\..\..\models\monster\source\Model.obj");
        }

        public void Clear(byte r, byte g, byte b, byte a)
        {
            for (int i = 0; i < _backBuffer.Length; i += 4)
            {
                _backBuffer[i] = b;
                _backBuffer[i + 1] = g;
                _backBuffer[i + 2] = r;
                _backBuffer[i + 3] = a;
            }
        }

        public void DrawFromBackbuffer()
        {
            RenderBitmap.Lock();
            RenderBitmap.WritePixels(new Int32Rect(0, 0, RenderBitmap.PixelWidth, RenderBitmap.PixelHeight),
                _backBuffer, RenderBitmap.PixelWidth * 4, 0);
            RenderBitmap.Unlock();
        }

        public void PutPixel(int x, int y, Vector3 color)
        {
            var index = (x + y * RenderBitmap.PixelWidth) * 4;

            _backBuffer[index] = (byte)Math.Min(255, color.Z * 255);
            _backBuffer[index + 1] = (byte)Math.Min(255, color.Y * 255);
            _backBuffer[index + 2] = (byte)Math.Min(255, color.X * 255);
            _backBuffer[index + 3] = 255;
        }

        public void DrawPoint(Vector3 point, Vector3 col)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < RenderBitmap.PixelWidth && point.Y < RenderBitmap.PixelHeight)
            {
                if (Render.zbuffer[(int)point.Y][(int)point.X] > point.Z)
                {
                    Render.zbuffer[(int)point.Y][(int)point.X] = point.Z;
                    PutPixel((int)point.X, (int)point.Y, col);
                }
            }
        }

        public void DrawModel(ObjModel model)
        {
            var verticies = Render._projectedVertices;
            var vertexNormals = Render.verticesNormals;            
            var vertexWorld = Render.verticesWorld;
            var texturalWorld = Render.texturalWorld;
            var oneDivZ = Render.oneDivZ;

            for (int i = 0; i < model.Poligons.Count; i++)
            {
                for (int j = 1; j < model.Poligons[i].GeometricVerticesNumber.Count - 1; j++)
                {
                    Vector3 point0 = new Vector3(verticies[model.Poligons[i].GeometricVerticesNumber[0] - 1].X,
                                            verticies[model.Poligons[i].GeometricVerticesNumber[0] - 1].Y,
                                            verticies[model.Poligons[i].GeometricVerticesNumber[0] - 1].Z);

                    Vector3 point1 = new Vector3(verticies[model.Poligons[i].GeometricVerticesNumber[j] - 1].X,
                                                 verticies[model.Poligons[i].GeometricVerticesNumber[j] - 1].Y,
                                                 verticies[model.Poligons[i].GeometricVerticesNumber[j] - 1].Z);

                    Vector3 point2 = new Vector3(verticies[model.Poligons[i].GeometricVerticesNumber[j + 1] - 1].X,
                                                 verticies[model.Poligons[i].GeometricVerticesNumber[j + 1] - 1].Y,
                                                 verticies[model.Poligons[i].GeometricVerticesNumber[j + 1] - 1].Z);

                    Vector3 point0T = texturalWorld[model.Poligons[i].TextureVerticesNumber[0] - 1];
                    Vector3 point1T = texturalWorld[model.Poligons[i].TextureVerticesNumber[j] - 1];
                    Vector3 point2T = texturalWorld[model.Poligons[i].TextureVerticesNumber[j + 1] - 1];

                    Vector3 point0N = vertexNormals[model.Poligons[i].NormalVerticesNumber[0] - 1];
                    Vector3 point1N = vertexNormals[model.Poligons[i].NormalVerticesNumber[j] - 1];
                    Vector3 point2N = vertexNormals[model.Poligons[i].NormalVerticesNumber[j + 1] - 1];

                    Vector3 point0M = vertexWorld[model.Poligons[i].GeometricVerticesNumber[0] - 1];
                    Vector3 point1M = vertexWorld[model.Poligons[i].GeometricVerticesNumber[j] - 1];
                    Vector3 point2M = vertexWorld[model.Poligons[i].GeometricVerticesNumber[j + 1] - 1];

                    float point0Z = oneDivZ[model.Poligons[i].GeometricVerticesNumber[0] - 1];
                    float point1Z = oneDivZ[model.Poligons[i].GeometricVerticesNumber[j] - 1];
                    float point2Z = oneDivZ[model.Poligons[i].GeometricVerticesNumber[j + 1] - 1];

                    Vector3 e1 = point1 - point0;
                    Vector3 e2 = point2 - point0;
                    float s = e2.X * e1.Y - e2.Y * e1.X;

                    if (point0.X != float.PositiveInfinity && point0.Y != float.PositiveInfinity &&
                        point1.X != float.PositiveInfinity && point1.Y != float.PositiveInfinity &&
                        point2.X != float.PositiveInfinity && point2.Y != float.PositiveInfinity &&
                        s > 0)
                    {
                        Bitmap diffuseMap = model.diffuseTexture;
                        Bitmap normalMap = model.normalTexture;
                        Bitmap specularMap = model.specularTexture;
                        Bitmap mraoMap = model.mraoTexture;

                        DrawFilledTriangle(point0, point1, point2,
                            point0N, point1N, point2N,
                            point0T, point1T, point2T,
                            point0M, point1M, point2M,
                            point0Z, point1Z, point2Z,
                            diffuseMap, normalMap, specularMap, mraoMap,
                            model.cubeMap);
                    }
                }
                
            }
        }

        void DrawFilledTriangle(Vector3 point0, Vector3 point1, Vector3 point2, 
            Vector3 point0N, Vector3 point1N, Vector3 point2N,
            Vector3 point0T, Vector3 point1T, Vector3 point2T,
            Vector3 point0M, Vector3 point1M, Vector3 point2M,
            float point0Z, float point1Z, float point2Z,
            Bitmap diffuseMap, Bitmap normalMap, Bitmap specularMap, Bitmap mraoMap,
            List<Bitmap> cubeMap)
        {
            Vector3 light = Vector3.Normalize(new Vector3(0f, 0f, -1f));
            //Vector3[] light = { Vector3.Normalize(new Vector3(-1f, -1f, -1f)), 
            //    Vector3.Normalize(new Vector3(1f, -1f, -1f)),
            //    Vector3.Normalize(new Vector3(-1f, -1f, 1f)),
            //    Vector3.Normalize(new Vector3(1f, -1f, 1f))};
            Vector3 lightColor = Vector3.One;

            if (point0.Y > point1.Y)
            {
                (point0, point1) = (point1, point0);
                (point0T, point1T) = (point1T, point0T);
                (point0M, point1M) = (point1M, point0M);
                (point0N, point1N) = (point1N, point0N);
                (point0Z, point1Z) = (point1Z, point0Z);
            }
            
            if (point0.Y > point2.Y)
            {
                (point0, point2) = (point2, point0);
                (point0T, point2T) = (point2T, point0T);
                (point0M, point2M) = (point2M, point0M);
                (point0N, point2N) = (point2N, point0N);
                (point0Z, point2Z) = (point2Z, point0Z);
            }

            if (point1.Y > point2.Y)
            {
                (point1, point2) = (point2, point1);
                (point1T, point2T) = (point2T, point1T);
                (point1M, point2M) = (point2M, point1M);
                (point1N, point2N) = (point2N, point1N);
                (point1Z, point2Z) = (point2Z, point1Z);
            }

            point0T = point0T * point0Z;
            point1T = point1T * point1Z;
            point2T = point2T * point2Z;

            point0M = point0M * point0Z;
            point1M = point1M * point1Z;
            point2M = point2M * point2Z;

            point0N = point0N * point0Z;
            point1N = point1N * point1Z;
            point2N = point2N * point2Z;

            int yMin = (int)Math.Ceiling(point0.Y);
            int yMax = (int)Math.Ceiling(point2.Y);

            for (int y = yMin; y < yMax; y++)
            {
                Vector3 kLeftLine = (point2 - point0) / (point2.Y - point0.Y);
                Vector3 kRightLine;
                Vector3 kLeftModel = (point2M - point0M) / (point2.Y - point0.Y);
                Vector3 kLeftTextural = (point2T - point0T) / (point2.Y - point0.Y);
                Vector3 kLeftNormal = (point2N - point0N) / (point2.Y - point0.Y);
                Vector3 kRightTextural;
                Vector3 kRightNormal;
                Vector3 rightTextural;
                Vector3 rightNormal;
                Vector3 rightLine;
                Vector3 kRightModel;
                Vector3 rightModel;
                float rightOneDivZ;
                float kLeftOneDivZ = (point2Z - point0Z) / (point2.Y - point0.Y);
                float kRightOneDivZ;

                if (y < point1.Y)
                {
                    kRightLine = (point1 - point0) / (point1.Y - point0.Y);
                    rightLine = point0 + kRightLine * (y - point0.Y);

                    kRightTextural = (point1T - point0T) / (point1.Y - point0.Y);
                    rightTextural = point0T + kRightTextural * (y - point0.Y);

                    kRightModel = (point1M - point0M) / (point1.Y - point0.Y);
                    rightModel = point0M + kRightModel * (y - point0.Y);

                    kRightNormal = (point1N - point0N) / (point1.Y - point0.Y);
                    rightNormal = point0N + kRightNormal * (y - point0.Y);

                    kRightOneDivZ = (point1Z - point0Z) / (point1.Y - point0.Y);
                    rightOneDivZ = point0Z + kRightOneDivZ * (y - point0.Y);
                }
                else {
                    kRightLine = (point2 - point1) / (point2.Y - point1.Y);
                    rightLine = point1 + kRightLine * (y - point1.Y);

                    kRightTextural = (point2T - point1T) / (point2.Y - point1.Y);
                    rightTextural = point1T + kRightTextural * (y - point1.Y);

                    kRightModel = (point2M - point1M) / (point2.Y - point1.Y);
                    rightModel = point1M + kRightModel * (y - point1.Y);

                    kRightNormal = (point2N - point1N) / (point2.Y - point1.Y);
                    rightNormal = point1N + kRightNormal * (y - point1.Y);

                    kRightOneDivZ = (point2Z - point1Z) / (point2.Y - point1.Y);
                    rightOneDivZ = point1Z + kRightOneDivZ * (y - point1.Y);
                }

                Vector3 leftLine;
                leftLine = point0 + kLeftLine * (y - point0.Y);

                Vector3 leftTextural;
                leftTextural = point0T + kLeftTextural * (y - point0.Y);

                Vector3 leftModel;
                leftModel = point0M + kLeftModel * (y - point0.Y);

                Vector3 leftNormal;
                leftNormal = point0N + kLeftNormal * (y - point0.Y);

                float leftOneDivZ = point0Z + kLeftOneDivZ * (y - point0.Y);

                if (leftLine.X > rightLine.X)
                {
                    (leftLine, rightLine) = (rightLine, leftLine);
                    (leftTextural, rightTextural) = (rightTextural, leftTextural);
                    (leftModel, rightModel) = (rightModel, leftModel);
                    (leftNormal, rightNormal) = (rightNormal, leftNormal);
                    (leftOneDivZ, rightOneDivZ) = (rightOneDivZ, leftOneDivZ);
                }

                int xMin = (int)Math.Ceiling(leftLine.X);
                int xMax = (int)Math.Ceiling(rightLine.X);

                for (int x = xMin; x < xMax; x++)
                {
                    float kZ = (rightLine.Z - leftLine.Z) / (rightLine.X - leftLine.X);
                    Vector3 readyPoint = new Vector3();
                    readyPoint.X = x;
                    readyPoint.Y = y;
                    readyPoint.Z = leftLine.Z + (x - leftLine.X) * kZ;

                    Vector3 kReadyPointTextural = (rightTextural - leftTextural) / (rightLine.X - leftLine.X);
                    Vector3 readyPointTextural;
                    readyPointTextural = leftTextural + kReadyPointTextural * (x - leftLine.X);

                    Vector3 kReadyPointModel = (rightModel - leftModel) / (rightLine.X - leftLine.X);
                    Vector3 readyPointModel;
                    readyPointModel = leftModel + kReadyPointModel * (x - leftLine.X);
                    
                    Vector3 kReadyPointNormal = (rightNormal - leftNormal) / (rightLine.X - leftLine.X);
                    Vector3 readyPointNormal;
                    readyPointNormal = leftNormal + kReadyPointNormal * (x - leftLine.X);

                    float kReadyPointOneDivZ = (rightOneDivZ - leftOneDivZ) / (rightLine.X - leftLine.X);
                    float readyPointOneDivZ = leftOneDivZ + kReadyPointOneDivZ * (x - leftLine.X);

                    readyPointTextural = readyPointTextural / readyPointOneDivZ;
                    readyPointModel = readyPointModel / readyPointOneDivZ;
                    readyPointNormal = readyPointNormal / readyPointOneDivZ;

                    Vector3 normalValue;
                    Vector3 normal;

                    if (normalMap != null)
                    {
                        normalValue = GetTextureColor(normalMap, readyPointTextural);
                        normal = new Vector3(normalValue.X / 255f * 2f - 1, normalValue.Y / 255f * 2f - 1, normalValue.Z / 255f * 2f - 1);
                    }

                    Vector3 reflectionReadyPointNormal = Vector3.Normalize(Vector3.Reflect(light, readyPointNormal));
                    Vector3 viewDirection = Vector3.Normalize(readyPointModel - Render.GetCameraPositionDekartWorld());

                    Vector3 reflectionViewDirection = Vector3.Reflect(viewDirection, readyPointNormal);
                    Vector3 reflectionColor = GetCubeMapColor(cubeMap, reflectionViewDirection);
                    Vector3 normalReflectionColor = new Vector3(reflectionColor.X / 255f, reflectionColor.Y / 255f, reflectionColor.Z / 255f);
                    
                    Vector3 ambient = (lightColor + normalReflectionColor) * 0.1f * modelColor;

                    float intense = Math.Max(0, Vector3.Dot(light, -readyPointNormal));
                    Vector3 diffuseModelColor = GetTextureColor(diffuseMap, readyPointTextural);
                    Vector3 diffuseNormalModelColor = new Vector3(diffuseModelColor.X / 255f, diffuseModelColor.Y / 255f, diffuseModelColor.Z / 255f);
                    Vector3 diffuse = diffuseNormalModelColor * intense * (lightColor + normalReflectionColor);

                    float specularIntense = Math.Max(0, Vector3.Dot(reflectionReadyPointNormal, -viewDirection));
                    specularIntense = (float)Math.Pow(specularIntense, 30f);
                    Vector3 specular = Vector3.Zero;

                    if (specularMap != null)
                    {
                        Vector3 roughness = GetTextureColor(specularMap, readyPointTextural);
                        Vector3 normalRoughness = new Vector3(roughness.X / 255f, roughness.Y / 255f, roughness.Z / 255f);
                        specular = (lightColor + normalReflectionColor) * specularIntense * normalRoughness;
                    }

                    if (mraoMap != null)
                    {
                        // R G B
                        // M R AO
                        Vector3 mrao = GetTextureColor(mraoMap, readyPointTextural);
                        Vector3 normalMrao = new Vector3(mrao.X / 255f, mrao.Y / 255f, mrao.Z / 255f);
                        diffuse *= normalMrao.Z;
                        specular = (lightColor + normalReflectionColor) * specularIntense * (1 - normalMrao.Y);
                    }

                    DrawPoint(readyPoint, ambient + diffuse + specular);
                }
            }
        }

        private static Vector3 GetTextureColor(in Bitmap bitmap, in Vector3 uv)
        {
            if (uv.X < 0f || uv.X > 1f || uv.Y < 0f || uv.Y > 1f)
            {
                return default;
            }

            int x = (int)((bitmap.Width - 1) * uv.X);
            int y = (int)((bitmap.Height - 1) * (1f - uv.Y));

            System.Drawing.Color c = bitmap.GetPixel(x, y);

            return new Vector3(c.R, c.G, c.B);
        }

        private static Vector3 GetCubeMapColor(List<Bitmap> cubeMap, in Vector3 v)
        {
            Vector3 vAbs = Vector3.Abs(v);
            double ma;
            Vector2 uv;
            int faceIndex;
            Bitmap texture;
            if (vAbs.Z >= vAbs.X && vAbs.Z >= vAbs.Y)
            {
                texture = v.Z < 0.0 ? cubeMap[5] : cubeMap[4];
                ma = 0.5 / vAbs.Z;
                uv = new Vector2(v.Z < 0.0 ? -v.X : v.X, v.Y);
            }
            else if (vAbs.Y >= vAbs.X)
            {
                texture = v.Y < 0.0 ? cubeMap[3] : cubeMap[2];
                ma = 0.5 / vAbs.Y;
                uv = new Vector2(v.X, v.Y < 0.0 ? v.Z : -v.Z);
            }
            else
            {
                texture = v.X < 0.0 ? cubeMap[1] : cubeMap[0];
                ma = 0.5 / vAbs.X;
                uv = new Vector2(v.X < 0.0 ? v.Z : -v.Z, v.Y);
            }
            Vector2 uvTexture = uv;
            uvTexture *= (float)ma;
            uvTexture.X += 0.5f;
            uvTexture.Y += 0.5f;

            int x = (int)((texture.Width - 1) * uvTexture.X);
            int y = (int)((texture.Height - 1) * (1f - uvTexture.Y));

            System.Drawing.Color c = texture.GetPixel(x, y);
            return new Vector3(c.R, c.G, c.B);
        }

        //void DrawBresenhemLine(Vector3 point1, Vector3 point2, Color col)
        //{
        //    int x1 = (int)point1.X;
        //    int y1 = (int)point1.Y;
        //    int x2 = (int)point2.X;
        //    int y2 = (int)point2.Y;
        //    int deltaX = Math.Abs(x2 - x1);
        //    int deltaY = Math.Abs(y2 - y1);
        //    int signX = x1 < x2 ? 1 : -1;
        //    int signY = y1 < y2 ? 1 : -1;
        //    int error = deltaX - deltaY;

        //    if (Render.zbuffer[y2][x2] > point2.Z)
        //    {
        //       Render.zbuffer[y2][x2] = point2.Z;
        //       // DrawPoint(new Vector2(x2, y2), col);
        //    }

        //    while (true)
        //    {
        //        //float currentZ = ((y1 - point1.Y) * (point2.Z - point1.Z)) / (point2.Y - point1.Y) + point1.Z;
        //        if (Render.zbuffer[y1][x1] > point1.Z)
        //        {
        //            Render.zbuffer[y1][x1] = point1.Z;
        //           // DrawPoint(new Vector2(x1, y1), col);
        //        }

        //        if ((x1 == x2) && (y1 == y2)) break;
        //        int error2 = error * 2;
        //        if (error2 > -deltaY)
        //        {
        //            error -= deltaY;
        //            x1 += signX;
        //        }
        //        if (error2 < deltaX)
        //        {
        //            error += deltaX;
        //            y1 += signY;
        //        }
        //    }
        //}

        //public void DrawLine(float x1, float y1, float x2, float y2)
        //{
        //    Line line = new Line();
        //    line.Stroke = Brushes.White;
        //    line.X1 = x1;
        //    line.Y1 = y1;
        //    line.X2 = x2;
        //    line.Y2 = y2;
        //    Grid.Children.Add(line);
        //}
        //public float x = 0f;

        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            Vector3 camera_pos_r = new Vector3(1f, 0, 0);
            Vector3 camera_pos_teta = new Vector3(0, 5, 0);
            Vector3 camera_pos_fi = new Vector3(0, 0, 5);
            Clear(0, 0, 0, 0);

            if (e.Key == Key.Up)
            {
                // (r, teta - delta, fi)
                Render.SetCameraPosition(Render.GetCameraPosition() - camera_pos_teta);
            }
            if (e.Key == Key.Down)
            {
                // (r, teta + delta, fi)
                Render.SetCameraPosition(Render.GetCameraPosition() + camera_pos_teta);
            }
            if (e.Key == Key.Left)
            {
                // (r, teta, fi - delta)
                Render.SetCameraPosition(Render.GetCameraPosition() - camera_pos_fi);
            }
            if (e.Key == Key.Right)
            {
                // (r, teta, fi + delta)
                Render.SetCameraPosition(Render.GetCameraPosition() + camera_pos_fi);
            }
            if (e.Key == Key.Enter)
            {
                // (r - delta, teta, fi)
                Render.SetCameraPosition(Render.GetCameraPosition() - camera_pos_r);
            }
            if (e.Key == Key.Space)
            {
                // (r + delta, teta, fi)
                Render.SetCameraPosition(Render.GetCameraPosition() + camera_pos_r);
            }

            Render.RenderModel();
            DrawModel(Render._model);
            DrawFromBackbuffer();
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid.Focus();
        }
    }
}