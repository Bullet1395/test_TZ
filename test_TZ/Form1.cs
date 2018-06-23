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
        List<Point> pointsAll;

        public Form1()
        {
            InitializeComponent();
            pointsAll = new List<Point>();

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkClick.Checked == false)
            {
                pointsAll.Add(new Point(e.X, e.Y));

                pictureBox1.CreateGraphics().FillRectangle(Brushes.Black, e.X, e.Y, 2, 2);

                pictureBox1.CreateGraphics().Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pointsAll.Count != 0)
            {
                paintsOnPicture(pointsAll);
            }
            else
            {
                MessageBox.Show("Нет доступных точек для рисования, возможно вы находитесь в режиме проверки.", "Обратите внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void paintsOnPicture(List<Point> pointsForPaint)
        {
            Point[] pointsArray = new Point[pointsForPaint.Count];
            for (int i = 0; i < pointsForPaint.Count; i++)
            {
                pointsArray[i] = pointsForPaint[i];
            }

            pictureBox1.CreateGraphics().DrawPolygon(Pens.Black, pointsArray);
            pictureBox1.CreateGraphics().FillPolygon(Brushes.LightGray, pointsArray);
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

        public bool isCrossLine(Point p1, Point p2, Point p3, Point p4)
        {
            long x;
            long y;

            long dx1 = p2.X - p1.X;
            long dy1 = p2.Y - p1.Y;
            long dx2 = p4.X - p3.X;
            long dy2 = p4.Y - p3.Y;

            x = dy1 * dx2 - dy2 * dx1;

            if (x == 0 || dx2 == 0)
                return
                    false;

            y = p3.X * p4.Y - p3.Y * p4.X;
            x = ((p1.X * p2.Y - p1.Y * p2.X) * dx2 - y * dx1) / x;
            y = (dy2 * x - y) / dx2;

            return
                ((p1.X <= x && p2.X >= x) || 
                (p2.X <= x && p1.X >= x)) && 
                ((p3.X <= x && p4.X >= x) || 
                (p4.X <= x && p3.X >= x));

        }

        public bool checkPointInPolygon(List<Point> points, Point point)
        {
            int checkForParity = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                {
                    if (isCrossLine(points[i], points[0], point, new Point(pictureBox1.Size.Width, point.Y)))
                        checkForParity++;
                }
                else if (isCrossLine(points[i], points[i + 1], point, new Point(pictureBox1.Size.Width, point.Y)))
                    checkForParity++;
            }

            if (checkForParity % 2 == 0)
                return false;
            return true;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkClick.Checked == true)
            {
                if (checkPointInPolygon(pointsAll, new Point(e.X, e.Y)))
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
                XmlSerializer xs = new XmlSerializer(typeof(List<Point>));

                Stream writer = new FileStream(saveFileXML.FileName, FileMode.Create);
                xs.Serialize(writer, pointsAll);
                writer.Close();
            }
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileXML.Filter = "XML файлы (*.xml) |*.xml|Все файлы |*.*";

            if (openFileXML.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Point>));

                Stream reader = new FileStream(openFileXML.FileName, FileMode.Open);
                pointsAll = (List<Point>)xs.Deserialize(reader);
                reader.Close();

                paintsOnPicture(pointsAll);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            pointsAll.Clear();
            pictureBox1.Image = null;
        }
    }
}
