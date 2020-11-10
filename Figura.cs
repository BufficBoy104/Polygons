using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Тренировка_на_уроках
{
    [Serializable]
    public abstract class Figura // главный класс
    {
        protected int x_fig;
        protected int y_fig;
        [NonSerialized] public static int R;
        [NonSerialized] public static Color color;
        public int del_x;
        public int del_y;
        public int up;
        public int down;
        public bool IsDragged;
        public bool IsinShell;
        protected Point p1, p2, p3;
        public int SetX
        {
            get { return x_fig; }
            set { x_fig = value; }
        }
        public int SetY
        {
            get { return y_fig; }
            set {  y_fig = value; }
        }
        public int SetR
        {
            get { return R; }
            set {R = value; }
        }
        static Figura()
        {
            R = 25;
        }
        public Figura(int x_fig, int y_fig)
        {
            this.x_fig = x_fig;
            this.y_fig = y_fig;
        }
        public abstract void Draw(Graphics graph);
        public abstract Boolean Check(double x, double y);
    }
    [Serializable]
    public class Tr : Figura //1 класс наследник треугольник 
    {
        public Tr(int x_fig, int y_fig) : base(x_fig, y_fig)
        {

        }
        public override void Draw(Graphics graph)
        {
            Point[] point = new Point[3];
            p1.X = x_fig;
            p1.Y = (y_fig - (R*2));

            p2.X = x_fig + (int)(Math.Sqrt(3) * R);
            p2.Y = y_fig + R;

            p3.X = x_fig - (int)(Math.Sqrt(3) * R);
            p3.Y = y_fig + R ;

            point[0] = new Point(p1.X, p1.Y);
            point[1] = new Point(p2.X, p2.Y);
            point[2] = new Point(p3.X, p3.Y);

            graph.FillPolygon(new SolidBrush(color), point);
        }

        public override Boolean Check(double x, double y) // проверка на попадание мышки
        {
            if (y<=y_fig+R && y+2*x - y_fig+R*2-2*x_fig >= 0 && y-2*x-y_fig+R*2+2*x_fig >= 0) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }// 1 класс наследник треугольник
    [Serializable]
    public class Cir : Figura // 2 класс наследник круг
    {
        public Cir(int x_fig, int y_fig) : base(x_fig, y_fig)
        {
            
        }
        public override void Draw(Graphics graph)
        {
            
            graph.FillEllipse(new SolidBrush(color), x_fig-(R), y_fig-(R) , R*2, R*2);
            /*
            graph.FillEllipse(new SolidBrush(color), new Rectangle(x_fig - R, y_fig - R, 2 * R, 2 * R));
            */
        }
        public override bool Check(double x, double y) // проверка на попадание мышки
        {
            /*
            return R >= Math.Sqrt(Math.Pow(x - x_fig, 2) + Math.Pow(y - y_fig, 2));
            */
            return Math.Pow(x - x_fig, 2) + Math.Pow(y - y_fig, 2) <= Math.Pow(R, 2) ? true : false;
        }
        
    }// 2 класс наследник круг
    [Serializable]
    public class Sq : Figura // 3 класс наследник квадрат
    {
        public Sq(int x_fig, int y_fig) : base(x_fig, y_fig)
        {

        }
        public override void Draw(Graphics graph)
        {
            graph.FillRectangle(new SolidBrush(color), x_fig-(R), y_fig-(R) , R*2, R*2);
        }
        public override bool Check(double x, double y) //проверка на попадение мышки
        {
            if(x >= x_fig-R && x <=x_fig +R && y>=y_fig-R &&y <= y_fig+R)
            {
                return true;
            }
            
            else
            {
                return false;
            }
        }
    }// 3 класс наследник квадрат
}