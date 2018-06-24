using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace test_TZ
{
    public partial class Form1 : Form
    {
        List<Point> pointsForPaint;
        List<Polygon> polygons;
        Polygon polygon;

        public Form1()
        {
            InitializeComponent();

            pointsForPaint = new List<Point>();
            polygons = new List<Polygon>();
            polygon = new Polygon();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkClick.Checked == false)
            {
                pointsForPaint.Add(new Point(e.X, e.Y));
                polygon.points.Add(new Point(e.X, e.Y));

                pictureBox1.CreateGraphics().FillRectangle(Brushes.Black, e.X, e.Y, 2, 2);

                pictureBox1.CreateGraphics().Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pointsForPaint.Count != 0)
            {
                polygons.Add(polygon);
                paintOnPicture(pointsForPaint);

                pointsForPaint.Clear();
                polygon = new Polygon();
            }
            else
            {
                MessageBox.Show("Нет доступных точек для рисования, возможно вы находитесь в режиме проверки.", "Обратите внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Метод для отрисовывания полигонов на объекте picturebox1. 
        /// </summary>
        /// <param name="pointsForPaint">Точки для отрисовывания полигонов</param>
        public void paintOnPicture(List<Point> pointsForPaint)
        {
            Point[] pointsArray = new Point[pointsForPaint.Count];
            for (int i = 0; i < pointsForPaint.Count; i++)
            {
                pointsArray[i] = pointsForPaint[i];
            }

            pictureBox1.CreateGraphics().DrawPolygon(Pens.Black, pointsArray);
            pictureBox1.CreateGraphics().FillPolygon(Brushes.LightGray, pointsArray);
        }

        /// <summary>
        /// Метод для отрисовывания полигонов на объекте picturebox1.
        /// </summary>
        /// <param name="polygons">Полигоны для отрисосвывания</param>
        public void paintOnPicture(List<Polygon> polygons)
        {
            Point[] pointsArray;

            foreach (Polygon polygon in polygons)
            {
                pointsArray = new Point[polygon.points.Count];

                for (int i = 0; i < polygon.points.Count; i++)
                {
                    pointsArray[i] = polygon.points[i];
                }
                pictureBox1.CreateGraphics().DrawPolygon(Pens.Black, pointsArray);
                pictureBox1.CreateGraphics().FillPolygon(Brushes.LightGray, pointsArray);
            }
        }

        private void checkClick_CheckedChanged(object sender, EventArgs e)
        {
            if (checkClick.Checked == false)
            {
                label1.Text = "Не активно";
            }
            else
            {
                label1.Text = "";
            }
        }

        /// <summary>
        /// Алгоритм проверки пересечения отрезков. По правилу Крамера.
        /// </summary>
        /// <param name="p1">Первая точка первого отрезка</param>
        /// <param name="p2">Вторая точка первого отрезка</param>
        /// <param name="p3">Первая точка второго отрезка</param>
        /// <param name="p4">Вторая точка второго отрезка</param>
        /// <returns></returns>
        public bool isCrossLine(Point p1, Point p2, Point p3, Point p4)
        {
            double d;

            double dx1 = p1.X - p2.X;
            double dy1 = p1.Y - p2.Y;
            double dx2 = p4.X - p3.X;
            double dy2 = p4.Y - p3.Y;

            d = dx1 * dy2 - dx2 * dy1;

            if (d == 0)
                if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4)
                    return true;
                else
                    return false;

            double u1 = ((p4.X - p2.X) * (p4.Y - p3.Y) - (p4.X - p3.X) * (p4.Y - p2.Y)) / d;
            double u2 = ((p1.X - p2.X) * (p4.Y - p2.Y) - (p4.X - p2.X) * (p1.Y - p2.Y)) / d;

            return (u1 >= 0 && u1 <= 1 && u2 >= 0 && u2 <= 1);

        }

        /// <summary>
        /// Проверка на нахождение точки внутри полигона, либо снаружи. Метод трассирвоки луча. Учет числа пересечений.
        /// </summary>
        /// <param name="polygons">Полигоны</param>
        /// <param name="point">Точка проверки</param>
        /// <returns></returns>
        public bool checkPointInPolygon(List<Polygon> polygons, Point point)
        {
            int checkForParity = 0;

            foreach (Polygon polygon in polygons)
            {
                for (int i = 0; i < polygon.points.Count; i++)
                {
                    if (i == polygon.points.Count - 1)
                    {
                        if (isCrossLine(polygon.points[i], polygon.points[0], point, new Point(pictureBox1.Size.Width, point.Y)))
                            checkForParity++;
                    }
                    else if (isCrossLine(polygon.points[i], polygon.points[i + 1], point, new Point(pictureBox1.Size.Width, point.Y)))
                        checkForParity++;
                }
            }

            if (checkForParity % 2 == 0)
                return false;
            return true;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkClick.Checked == true)
            {
                if (checkPointInPolygon(polygons, new Point(e.X, e.Y)))
                    label1.Text = "Внутри полигона";
                else
                    label1.Text = "Снаружи полигона";
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileXML.Filter = "XML файлы (*.xml) |*.xml";

            if (saveFileXML.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Polygon>));

                Stream writer = new FileStream(saveFileXML.FileName, FileMode.Create);

                xs.Serialize(writer, polygons);

                writer.Close();
            }
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileXML.Filter = "XML файлы (*.xml) |*.xml|Все файлы |*.*";

            if (openFileXML.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Polygon>));

                Stream reader = new FileStream(openFileXML.FileName, FileMode.Open);

                polygons = (List<Polygon>)xs.Deserialize(reader);

                reader.Close();

                paintOnPicture(polygons);
            }
        }

        /// <summary>
        /// Очистка элемента picturebox1 и polygons.
        /// </summary>
        public void clearAll()
        {
            polygons.Clear();
            pictureBox1.Image = null;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            clearAll();
        }
    }
}
