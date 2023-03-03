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

        public Vector3 modelColor = new Vector3(0.8f, 0, 1f);
        

        public MainWindow()
        {
            InitializeComponent();
            Grid = MainGrid;
            RenderBitmap = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgra32, null);
            _backBuffer = new byte[RenderBitmap.PixelWidth * RenderBitmap.PixelHeight * 4];
            RenderImage.Source = RenderBitmap;

            Render.Init(@"..\..\..\..\cube\source\cube.obj");
            //Render.Init(@"..\..\..\..\cube\source\head.obj");
            //Render.Init(@"..\..\..\..\shrek\source\shrek_shrinked.obj");
            //Render.Init(@"..\..\..\..\dinosaur\source\deino.obj");
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

            for (int i = 0; i < model.Poligons.Count; i++)
            {
                Vector3 point0 = new Vector3(verticies[model.Poligons[i].GeometricVerticesNumber[0] - 1].X,
                                            verticies[model.Poligons[i].GeometricVerticesNumber[0] - 1].Y,
                                            verticies[model.Poligons[i].GeometricVerticesNumber[0] - 1].Z);

                Vector3 point1 = new Vector3(verticies[model.Poligons[i].GeometricVerticesNumber[1] - 1].X,
                                             verticies[model.Poligons[i].GeometricVerticesNumber[1] - 1].Y,
                                             verticies[model.Poligons[i].GeometricVerticesNumber[1] - 1].Z);

                Vector3 point2 = new Vector3(verticies[model.Poligons[i].GeometricVerticesNumber[2] - 1].X,
                                             verticies[model.Poligons[i].GeometricVerticesNumber[2] - 1].Y,
                                             verticies[model.Poligons[i].GeometricVerticesNumber[2] - 1].Z);

                Vector3 point0N = vertexNormals[model.Poligons[i].NormalVerticesNumber[0] - 1];
                Vector3 point1N = vertexNormals[model.Poligons[i].NormalVerticesNumber[1] - 1];
                Vector3 point2N = vertexNormals[model.Poligons[i].NormalVerticesNumber[2] - 1];

                Vector3 point0M = vertexWorld[model.Poligons[i].GeometricVerticesNumber[0] - 1];
                Vector3 point1M = vertexWorld[model.Poligons[i].GeometricVerticesNumber[1] - 1];
                Vector3 point2M = vertexWorld[model.Poligons[i].GeometricVerticesNumber[2] - 1];

                Vector3 e1 = point1 - point0;
                Vector3 e2 = point2 - point0;
                float s = e2.X * e1.Y - e2.Y * e1.X;

                if (point0.X != float.PositiveInfinity && point0.Y != float.PositiveInfinity &&
                    point1.X != float.PositiveInfinity && point1.Y != float.PositiveInfinity &&
                    point2.X != float.PositiveInfinity && point2.Y != float.PositiveInfinity &&
                    s > 0)
                {
                    DrawFilledTriangle(point0, point1, point2, point0N, point1N, point2N, point0M, point1M, point2M);
                }
            }
        }

        void DrawFilledTriangle(Vector3 point0, Vector3 point1, Vector3 point2, 
            Vector3 point0N, Vector3 point1N, Vector3 point2N,
            Vector3 point0M, Vector3 point1M, Vector3 point2M)
        {
            Vector3[] light = { Vector3.Normalize(new Vector3(-1f, -1f, -1f)), 
                Vector3.Normalize(new Vector3(1f, -1f, -1f)),
                Vector3.Normalize(new Vector3(-1f, -1f, 1f)),
                Vector3.Normalize(new Vector3(1f, -1f, 1f))};
            Vector3 lightColor = Vector3.One;

            if (point0.Y > point1.Y)
            {
                (point0, point1) = (point1, point0);
                (point0N, point1N) = (point1N, point0N);
                (point0M, point1M) = (point1M, point0M);
            }
            
            if (point0.Y > point2.Y)
            {
                (point0, point2) = (point2, point0);
                (point0N, point2N) = (point2N, point0N);
                (point0M, point2M) = (point2M, point0M);
            }

            if (point1.Y > point2.Y)
            {
                (point1, point2) = (point2, point1);
                (point1N, point2N) = (point2N, point1N);
                (point1M, point2M) = (point2M, point1M);
            }

            int yMin = (int)Math.Ceiling(point0.Y);
            int yMax = (int)Math.Ceiling(point2.Y);

            for (int y = yMin; y < yMax; y++)
            {
                Vector3 kLeftLine = (point2 - point0) / (point2.Y - point0.Y);
                Vector3 kRightLine;
                Vector3 kLeftModel = (point2M - point0M) / (point2.Y - point0.Y);
                Vector3 kLeftNormal = (point2N - point0N) / (point2.Y - point0.Y);
                Vector3 kRightNormal;
                Vector3 rightNormal;
                Vector3 rightLine;
                Vector3 kRightModel;
                Vector3 rightModel;

                if (y < point1.Y)
                {
                    kRightLine = (point1 - point0) / (point1.Y - point0.Y);
                    rightLine = point0 + kRightLine * (y - point0.Y);

                    kRightNormal = (point1N - point0N) / (point1.Y - point0.Y);
                    rightNormal = point0N + kRightNormal * (y - point0.Y);

                    kRightModel = (point1M - point0M) / (point1.Y - point0.Y);
                    rightModel = point0M + kRightModel * (y - point0.Y);
                }
                else {
                    kRightLine = (point2 - point1) / (point2.Y - point1.Y);
                    rightLine = point1 + kRightLine * (y - point1.Y);

                    kRightNormal = (point2N - point1N) / (point2.Y - point1.Y);
                    rightNormal = point1N + kRightNormal * (y - point1.Y);

                    kRightModel = (point2M - point1M) / (point2.Y - point1.Y);
                    rightModel = point1M + kRightModel * (y - point1.Y);
                }

                Vector3 leftLine;
                leftLine = point0 + kLeftLine * (y - point0.Y);

                Vector3 leftNormal;
                leftNormal = point0N + kLeftNormal * (y - point0.Y);

                Vector3 leftModel;
                leftModel = point0M + kLeftModel * (y - point0.Y);

                if (leftLine.X > rightLine.X)
                {
                    (leftLine, rightLine) = (rightLine, leftLine);
                    (leftNormal, rightNormal) = (rightNormal, leftNormal);
                    (leftModel, rightModel) = (rightModel, leftModel);
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

                    Vector3 kReadyPointNormal = (rightNormal - leftNormal) / (rightLine.X - leftLine.X);
                    Vector3 readyPointNormal;
                    readyPointNormal = leftNormal + kReadyPointNormal * (x - leftLine.X);

                    Vector3 kReadyPointModel = (rightModel - leftModel) / (rightLine.X - leftLine.X);
                    Vector3 readyPointModel;
                    readyPointModel = leftModel + kReadyPointModel * (x - leftLine.X);

                    Vector3 ambient = lightColor * 0.1f * modelColor;
                    Vector3 sumLight = ambient;

                    for (int j = 0; j < light.Length; j ++)
                    {
                        float intense = Math.Max(0, (Vector3.Dot(light[j], -readyPointNormal)));
                        Vector3 diffuse = lightColor * intense * modelColor;

                        Vector3 reflectionReadyPointNormal = Vector3.Normalize(Vector3.Reflect(light[j], readyPointNormal));
                        Vector3 viewDirection = Vector3.Normalize(readyPointModel - Render.GetCameraPositionDekartWorld());

                        float specularIntense = Math.Max(0, Vector3.Dot(reflectionReadyPointNormal, -viewDirection));
                        specularIntense = (float)Math.Pow(specularIntense, 30f);
                        Vector3 specular = lightColor * specularIntense;

                        sumLight += diffuse + specular;
                    }
                    

                    DrawPoint(readyPoint, sumLight);
                }
            }
        }

        void CalcPolygonColor(Vector3 point0, Vector3 point1, Vector3 point3, Vector3 light)
        {

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
            Vector3 camera_pos_r = new Vector3(0.5f, 0, 0);
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