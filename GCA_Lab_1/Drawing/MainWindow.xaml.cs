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
        public MainWindow()
        {
            InitializeComponent();
            Grid = MainGrid;
            RenderBitmap = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgra32, null);
            _backBuffer = new byte[RenderBitmap.PixelWidth * RenderBitmap.PixelHeight * 4];
            RenderImage.Source = RenderBitmap;

            Render.Init(@"..\..\..\..\models\head\source\head.obj");            
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

        public void PutPixel(int x, int y, Color color)
        {
            var index = (x + y * RenderBitmap.PixelWidth) * 4;

            _backBuffer[index] = color.B;
            _backBuffer[index + 1] = color.G;
            _backBuffer[index + 2] = color.R;
            _backBuffer[index + 3] = color.A;
        }

        public void DrawPoint(Vector2 point)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < RenderBitmap.PixelWidth && point.Y < RenderBitmap.PixelHeight)
                PutPixel((int)point.X, (int)point.Y, Color.FromRgb(255, 255, 255));
        }

        public void DrawModel(ObjModel model)
        {
            int X1Index, X2Index, Y1Index, Y2Index;
            float X1, X2, Y1, Y2;

            foreach (var item in model.Poligons)
            {
                for (int i = 1; i < item.GeometricVerticesNumber.Count; i++)
                {
                    var a = Render._projectedVertices;
                    X1Index = item.GeometricVerticesNumber[i - 1];
                    X2Index = item.GeometricVerticesNumber[i];
                    Y1Index = item.GeometricVerticesNumber[i - 1];
                    Y2Index = item.GeometricVerticesNumber[i];

                    X1 = Render._projectedVertices[X1Index - 1].X;
                    X2 = Render._projectedVertices[X2Index - 1].X;
                    Y1 = Render._projectedVertices[Y1Index - 1].Y;
                    Y2 = Render._projectedVertices[Y2Index - 1].Y;

                    if (X1 != float.PositiveInfinity && X2 != float.PositiveInfinity && Y1 != float.PositiveInfinity && Y2 != float.PositiveInfinity)
                    {
                        DrawBresenhemLine((int)X1, (int)Y1, (int)X2, (int)Y2);
                    }
                }

                X1Index = item.GeometricVerticesNumber[item.GeometricVerticesNumber.Count - 1];
                X2Index = item.GeometricVerticesNumber[0];
                Y1Index = item.GeometricVerticesNumber[item.GeometricVerticesNumber.Count - 1];
                Y2Index = item.GeometricVerticesNumber[0];

                X1 = Render._projectedVertices[X1Index - 1].X;
                X2 = Render._projectedVertices[X2Index - 1].X;
                Y1 = Render._projectedVertices[Y1Index - 1].Y;
                Y2 = Render._projectedVertices[Y2Index - 1].Y;

                if (X1 != float.PositiveInfinity && X2 != float.PositiveInfinity && Y1 != float.PositiveInfinity && Y2 != float.PositiveInfinity)
                {
                    DrawBresenhemLine((int)X1, (int)Y1, (int)X2, (int)Y2);
                }
            }
        }

        void DrawBresenhemLine(int x1, int y1, int x2, int y2)
        {
            int deltaX = Math.Abs(x2 - x1);
            int deltaY = Math.Abs(y2 - y1);
            int signX = x1 < x2 ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;
            int error = deltaX - deltaY;
            DrawPoint(new Vector2(x2, y2));
            // DrawPoint(new Vector2(x1, y1));

            while (true)
            {
                DrawPoint(new Vector2(x1, y1));
                

                if ((x1 == x2) && (y1 == y2)) break;
                int error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }
            /*while (x1 != x2 || y1 != y2)
            {
                Line point_new = new Line();
                point_new.Stroke = Brushes.White;
                point_new.X1 = x1;
                point_new.Y1 = y1;
                point_new.X2 = x1 + 1;
                point_new.Y2 = y1 + 1;
                Grid.Children.Add(point_new);

                int error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }*/
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {           
            Line line = new Line();
            line.Stroke = Brushes.White;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            Grid.Children.Add(line);
        }
        public float x = 0f;

        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            Vector3 camera_pos_r = new Vector3(0.1f, 0, 0);
            Vector3 camera_pos_teta = new Vector3(0, 1, 0);
            Vector3 camera_pos_fi = new Vector3(0, 0, 1);
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
