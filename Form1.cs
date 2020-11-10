using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Тренировка_на_уроках
{
    public partial class Form1 : Form
    {
        public Color _FigureColor;
        private bool Draw;
        public bool ShellDragged;
        private bool shake_pos = false;
        private DeleteVertex DelCmd;
        private DragAndDrop DnDCmd;
        private int shapenum;
        public static bool DelAndDnD = false;
        public string FileName_Path = @"1.bin";
        public List<Figura> vrtx_list = new List<Figura>();

        public Form1()
        {
            ShellDragged = false;
            Figura.color = Color.ForestGreen;
            DoubleBuffered = true;
            InitializeComponent();
        }

        Form3 form3 = new Form3();

        private void ChangeRadiusForAll(object sender, radiusEventArgs e)
        {
            ChangeRadius radiusCmd = new ChangeRadius(Figura.R, e.Rad);
            radiusCmd.Execute();
            UndoRedoManager.UndoCommands.Push(radiusCmd);
            Invalidate();
        }
        
        private bool IsInside(Point Mouse)
        {
            List<Figura> shOb = new List<Figura>();
            shOb.AddRange(vrtx_list);
            shOb.Add(new Cir(Mouse.X, Mouse.Y));
            for(int i=0; i< shOb.Count; i++) { shOb[i].IsinShell = false; }
            
            for(int i=0; i< shOb.Count; i++)
            {
                for(int j=i+1; j< shOb.Count; j++)
                {
                    bool UP = true;
                    bool DOWN = true;
                    for(int k=0; k<shOb.Count; k++)
                    {
                        if(k!= j && k != i && i!= j)
                        {
                            if(((shOb[j].SetY - shOb[i].SetY) * shOb[k].SetX) + ((shOb[i].SetX - shOb[j].SetX) * shOb[k].SetY) + (shOb[j].SetX * shOb[i].SetY - shOb[j].SetY * shOb[i].SetX) >= 0)
                            {
                                DOWN = false;
                            }
                            if(((shOb[j].SetY - shOb[i].SetY) * shOb[k].SetX) + ((shOb[i].SetX - shOb[j].SetX) * shOb[k].SetY) + (shOb[j].SetX * shOb[i].SetY - shOb[j].SetY * shOb[i].SetX) < 0)
                            {
                                UP = false;
                            }
                        }
                    }
                    if (DOWN || UP)
                    {
                        shOb[i].IsinShell = true;
                        shOb[j].IsinShell = true;
                    }
                }
            }

            if (shOb.Last().IsinShell) { return false; }
            else { return true; }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            DnDCmd?.DdDo(e.X, e.Y, ShellDragged);
            this.Invalidate();
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            DnDCmd?.DdEnd();
            for (int i=0; i < vrtx_list.Count; i++)
            {
                if (vrtx_list[i].IsDragged)
                {
                    vrtx_list[i].IsDragged = false;
                    Refresh();
                }
                
            }
            if (vrtx_list.Count > 2)
            {
                for(int j=0; j<vrtx_list.Count; j++)
                {
                    if (!vrtx_list[j].IsinShell)
                    {
                        vrtx_list.RemoveAt(j); //добавить удаление команду
                        //DelCmd = new DeleteVertex(vrtx_list, vrtx_list[j]);
                        //UndoRedoManager.UndoCommands.Push(DelCmd);
                        //DelCmd?.Execute();
                        //DelAndDnD = true;
                    }
                }
            }
            ShellDragged = false;
            this.Invalidate();

        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool create_new = false;
            if (vrtx_list != null) // есть ли список
            {
                
                for (int i = 0; i < vrtx_list.Count; i++)
                {
                    if (vrtx_list[i].Check(e.X, e.Y)) //попадает ли мышка в вершину. Если да то считаем дельты
                    {
                        if (e.Button == MouseButtons.Left) // ЛКМ
                        {
                            DnDCmd = new DragAndDrop(vrtx_list);
                            UndoRedoManager.UndoCommands.Push(DnDCmd);
                            DnDCmd.DdStart(e.X, e.Y, ShellDragged);
                            vrtx_list[i].IsDragged = true;
                            vrtx_list[i].del_x = e.X - vrtx_list[i].SetX;
                            vrtx_list[i].del_y = e.Y - vrtx_list[i].SetY;
                            create_new = true;
                        }
                        if (e.Button == MouseButtons.Right) //ПКМ
                        {
                            DelCmd = new DeleteVertex(vrtx_list, vrtx_list[i]);
                            UndoRedoManager.UndoCommands.Push(DelCmd);
                            DelCmd?.Execute();
                            DelCmd = null;

                        }
                        Refresh();
                    }
                }
                if (!create_new)
                {
                    if(e.Button == MouseButtons.Left)
                    {
                        if(vrtx_list.Count> 2)  // многоугольник есть
                        {
                            if(IsInside(new Point(e.X, e.Y)))
                            {
                                foreach (Figura item in vrtx_list)
                                {
                                    item.del_x = e.X - item.SetX;
                                    item.del_y = e.Y - item.SetY;
                                    item.IsDragged = true;
                                }
                                ShellDragged = true;
                                DnDCmd = new DragAndDrop(vrtx_list);
                                UndoRedoManager.UndoCommands.Push(DnDCmd);
                                DnDCmd.DdStartShell(vrtx_list);

                            }
                            else
                            {
                                Draw = true;
                                Figura temp1;
                                switch (shapenum)
                                {
                                    case 1:
                                        temp1 = new Cir(e.X, e.Y);
                                        break;
                                    case 2:
                                        temp1 = new Sq(e.X, e.Y);
                                        break;
                                    case 3:
                                        temp1 = new Tr(e.X, e.Y);
                                        break;
                                    default:
                                        temp1 = new Cir(e.X, e.Y);
                                        break;
                                }
                                AddVertex addvertsCmd = new AddVertex(vrtx_list, temp1);
                                UndoRedoManager.UndoCommands.Push(addvertsCmd);
                                addvertsCmd.Execute();
                                Invalidate();
                            }
                        }
                        else   // новую фигуру
                        {
                            Draw = true;
                            Figura temp1;
                            switch (shapenum)
                            {
                                case 1:
                                    temp1 = new Cir(e.X, e.Y);
                                    break;
                                case 2:
                                    temp1 = new Sq(e.X, e.Y);
                                    break;
                                case 3:
                                    temp1 = new Tr(e.X, e.Y);
                                    break;
                                default:
                                    temp1 = new Cir(e.X, e.Y);
                                    break;
                            }
                            AddVertex addvertsCmd = new AddVertex(vrtx_list, temp1);
                            UndoRedoManager.UndoCommands.Push(addvertsCmd);
                            addvertsCmd.Execute();

                            this.Invalidate();
                        }
                    }
                    
                }
                this.Invalidate();
            }

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Figura.color = colorDialog1.Color;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Graphics g = e.Graphics;
            if (Draw == true)
            {
                g.Clear(Color.White);
                if(vrtx_list.Count > 2)
                {
                    for(int i1=0; i1< vrtx_list.Count; i1++)
                    {
                        vrtx_list[i1].IsinShell = false;
                    }
                    for (int i = 0; i < vrtx_list.Count; i++)
                    {
                        for (int j = i+1; j < vrtx_list.Count; j++)
                        {
                            int up = 0;
                            int down = 0;
                            for (int k = 0; k < vrtx_list.Count; k++)
                            {
                                if(k != i && k != j && i != j)
                                {
                                   int temp = ((vrtx_list[j].SetY - vrtx_list[i].SetY)* vrtx_list[k].SetX)+((vrtx_list[i].SetX- vrtx_list[j].SetX)* vrtx_list[k].SetY)+(vrtx_list[j].SetX* vrtx_list[i].SetY - vrtx_list[j].SetY* vrtx_list[i].SetX);
                                   if ( temp< 0){ down++; }
                                   if (temp> 0){ up++; }
                                }
                            }
                            if (up == 0 || down == 0)
                            {
                                g.DrawLine(new Pen(Color.Black), vrtx_list[i].SetX, vrtx_list[i].SetY, vrtx_list[j].SetX, vrtx_list[j].SetY);
                                vrtx_list[i].IsinShell = true;
                                vrtx_list[j].IsinShell = true;
                            }

                        }
                    }
                }
                
                foreach (Figura item in vrtx_list)
                {
                    item.Draw(g);
                }
            }
        }
        private void кругToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            shapenum = 1;
        }

        private void квадратToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shapenum = 2;
        }

        private void ТреугольникToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shapenum = 3;
        }

        private void цветToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            //_FigureColor = colorDialog1.Color;
            ChangeColorCmd colorCmd = new ChangeColorCmd(Figura.color, colorDialog1.Color);
            UndoRedoManager.UndoCommands.Push(colorCmd);
            colorCmd.Execute();
            //UndoRedoManager.UndoCommands.Push(colorCmd);
            Invalidate();
        }
        
        private void радиусToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (form3.Created == false)
            {
                form3.Show();
                form3.OnRadius += ChangeRadiusForAll;
            }
            else
            {
                form3.Show();
                form3.Focus();
            }
        }

        private void Play_Click(object sender, EventArgs e)
        {
            timer1.Start();
            timer1.Interval = 5;
            Invalidate();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Invalidate();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Random random = new Random(); 
            int rnd = random.Next(-1, 1);
            int a = random.Next(-1, 1);
            int b = random.Next(-1, 1);
            if (shake_pos)
            {
                foreach (Figura item in vrtx_list)
                {
                    int temp = random.Next(-1, 1);
                    item.SetX += temp;
                    item.SetY += temp;
                }
                shake_pos = false;
            }
            else
            {
                foreach (Figura item in vrtx_list)
                {
                    int temp = random.Next(-1, 1);
                    item.SetX -= temp;
                    item.SetY -= temp;
                }
                shake_pos = true;
            }
            Invalidate();
        }

        private void СохранитьВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            FileName_Path = saveFileDialog1.FileName;
            try { Stream _stream = File.Create(FileName_Path);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(_stream, vrtx_list);
                serializer.Serialize(_stream, Figura.color);
                serializer.Serialize(_stream, Figura.R);
                _stream.Close();
            }
            catch
            {
                MessageBox.Show("Введите название файла");
            }
        }

        private void ВыгрузитьИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = FileName_Path;
            openFileDialog1.ShowDialog();
            if (File.Exists(FileName_Path))
            {
                Stream _stream2 = File.OpenRead(FileName_Path);
                BinaryFormatter deserializer = new BinaryFormatter();
                vrtx_list = (List<Figura>)deserializer.Deserialize(_stream2);
                Figura.color = (Color)deserializer.Deserialize(_stream2);
                Figura.R = (Int32)deserializer.Deserialize(_stream2);
                _stream2.Close();
            }
            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoRedoManager.Undo();
                Invalidate();
            }
            else if(e.Control && e.KeyCode == Keys.Y)
            {
                UndoRedoManager.Redo();
                Invalidate();
            }
            else if(e.KeyCode == Keys.Back)
            {
                MessageBox.Show("стек очищен");
                UndoRedoManager.UndoCommands.Clear();
                UndoRedoManager.RedoCommands.Clear();
            }
        }
    }
}