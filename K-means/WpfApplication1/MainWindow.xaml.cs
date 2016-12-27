using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            const string FileName = "../../../unbalance.txt";
            const int NumberOfClusters = 8;

            InitializeComponent();

            var points = GetPoints(FileName);
            var centroids = GetClusterCentroids(NumberOfClusters, points);
            var clusters = new List<List<Point>>();
            for (int i = 0; i < NumberOfClusters; i++)
            {
                clusters.Add(new List<Point> { centroids[i] });
            }

            clusters.First().AddRange(points);
            Clusturize(clusters);
            DrawPoints(clusters);
        }

        private void DrawPoints(List<List<Point>> clusters)
        {
            Random rand = new Random();
            const int Width = 700;
            const int Height = 400;

            // calculate scaling factor
            var minX = clusters.SelectMany(x => x).OrderBy(x => x.X).Select(x => x.X).First();
            var maxX = clusters.SelectMany(x => x).OrderByDescending(x => x.X).Select(x => x.X).First();
            var minY = clusters.SelectMany(x => x).OrderBy(x => x.Y).Select(x => x.Y).First();
            var maxY = clusters.SelectMany(x => x).OrderByDescending(x => x.Y).Select(x => x.Y).First();
            var diffX = maxX - minX;
            var diffY = maxY - minY;
            var scaleX = Width / diffX;
            var scaleY = Height / diffY;
            var scale = Math.Min(scaleX, scaleY);

            foreach (var cluster in clusters)
            {
                Byte[] b = new Byte[3];
                rand.NextBytes(b);
                Color color = Color.FromRgb(b[0], b[1], b[2]);
                var brush = new SolidColorBrush();
                brush.Color = color;

                foreach (var point in cluster.Skip(1))
                {
                    var line = new Line();
                    line.Stroke = brush;
                    line.StrokeThickness = 3;
                    line.X1 = (point.X - minX) * scale;
                    line.X2 = (point.X - minX) * scale + 3;
                    line.Y1 = (point.Y - minY) * scale;
                    line.Y2 = (point.Y - minY) * scale;
                    mainGrid.Children.Add(line);
                }
            }
        }

        private static void Clusturize(List<List<Point>> clusters)
        {
            var areClusterized = true;
            var centroids = clusters.Select(x => x.FirstOrDefault()).ToList();
            foreach (var cluster in clusters)
            {
                for (int i = 1; i < cluster.Count(); i++)
                {
                    var point = cluster[i];
                    var nearestCentroid = centroids.OrderBy(x => x.GetDistanceTo(point)).First();
                    if (nearestCentroid != cluster.First())
                    {
                        areClusterized = false;
                        cluster.Remove(point);
                        var clusterToMoveInto = clusters.Where(x => x.First() == nearestCentroid).First();
                        clusterToMoveInto.Add(point);
                    }
                }
            }

            if (areClusterized)
                return;

            // recalculate centroids
            foreach (var cluster in clusters)
            {
                var newCentroid = GetCenterOfGravity(cluster.Skip(1).ToList());
                cluster[0].X = newCentroid.X;
                cluster[0].Y = newCentroid.Y;
            }

            Clusturize(clusters);
        }

        private static List<Point> GetPoints(string fileName)
        {
            var points = new List<Point>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    break;

                var pointData = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                points.Add(new Point
                {
                    X = double.Parse(pointData[0]),
                    Y = double.Parse(pointData[1])
                });
            }

            return points;
        }

        private static Point GetCenterOfGravity(List<Point> points)
        {
            var x = points.Sum(p => p.X) / points.Count;
            var y = points.Sum(p => p.Y) / points.Count;
            return new Point { X = x, Y = y };
        }

        private static List<Point> GetClusterCentroids(int numberOfClusters, List<Point> points)
        {
            var centroids = new List<Point>();
            var minX = points.Min(x => x.X);
            var minY = points.Min(x => x.Y);
            var maxX = points.Max(x => x.X);
            var maxY = points.Max(x => x.Y);
            var rand = new Random();

            for (int i = 0; i < numberOfClusters; i++)
            {
                centroids.Add(new Point
                {
                    X = rand.NextDouble() * (maxX - minX) + minX,
                    Y = rand.NextDouble() * (maxY - minY) + minY
                });
            }

            return centroids;
        }

        private class Point
        {
            public double X { get; set; }

            public double Y { get; set; }

            public double GetDistanceTo(Point other)
            {
                return Math.Sqrt(Math.Pow(Math.Abs(this.X - other.X), 2) + Math.Pow(Math.Abs(this.Y - other.Y), 2));
            }
        }
    }
}
