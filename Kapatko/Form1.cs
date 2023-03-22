using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kapatko
{
    public partial class Form1 : Form
    {
        private Image _image;
        private Bitmap _bitmap;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_lastColor))
            {
                Clipboard.SetText(_lastColor);
                ReplaceColor();
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                _image = Image.FromFile(dialog.FileName);
            }

            SetImage();
        }

        private void ReplaceColor()
        {
            var dialog = new ColorDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            for (var i = 0; i < _bitmap.Height; i++)
            {
                for (var j = 0; j < _bitmap.Width; j++)
                {
                    if (_bitmap.GetPixel(j, i) == _actualColor)
                    {
                        _bitmap.SetPixel(j, i, dialog.Color);
                    }
                }
            }
        }

        private void SetImage()
        {
            var g = Graphics.FromImage(_bitmap);
            g.DrawImage(_image, 0, 0);
            BackgroundImage = _bitmap;
            Cursor = Cursors.Cross;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _bitmap = new Bitmap(Width, Height);
        }

        private Point _lastLocation = new Point(0, 0);

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            _lastLocation = e.Location;
            Invalidate();
        }

        private const int ELLIPSE_SIZE = 50;
        private const int COLOR_MAX_VALUE = 100;

        private string _lastColor = string.Empty;
        private Color _actualColor;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_image == null)
                return;

            var pixel = _bitmap.GetPixel(_lastLocation.X, _lastLocation.Y);
            _actualColor = pixel;
            var g = e.Graphics;
            var brush = new SolidBrush(pixel);
            g.FillEllipse(brush, _lastLocation.X, _lastLocation.Y, ELLIPSE_SIZE, ELLIPSE_SIZE);
            var color = Color.White;
            if (pixel.R > COLOR_MAX_VALUE && pixel.B > COLOR_MAX_VALUE && pixel.G > COLOR_MAX_VALUE)
            {
                color = Color.Black;
            }

            var textColor = Color.Black;
            if (color == Color.Black)
            {
                textColor = Color.White;
            }

            g.DrawEllipse(new Pen(color, 2), _lastLocation.X, _lastLocation.Y, ELLIPSE_SIZE, ELLIPSE_SIZE);
            g.FillRectangle(new SolidBrush(color), _lastLocation.X, _lastLocation.Y + ELLIPSE_SIZE + 5, 80, 20);
            _lastColor = $"#{pixel.R:x}{pixel.G:x}{pixel.B:x}".ToUpper();

            g.DrawString(_lastColor, new Font(FontFamily.GenericSansSerif, 13), new SolidBrush(textColor),
                _lastLocation.X, _lastLocation.Y + ELLIPSE_SIZE + 5);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    _lastColor = string.Empty;
                    _image = null;
                    BackgroundImage = null;
                    Invalidate();
                    break;
            }
        }
    }
}