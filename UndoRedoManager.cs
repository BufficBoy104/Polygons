using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Тренировка_на_уроках
{
    static class UndoRedoManager
    {
        public static void Undo()
        {
            if (UndoCommands.Count > 0)
            {
                ICommand temp = UndoCommands.Peek();
                temp.Undo();
                RedoCommands.Push(temp);
                UndoCommands.Pop();
            }
        }

        public static void Redo()
        {
            if(RedoCommands.Count > 0)
            {
                ICommand temp = RedoCommands.Peek();
                temp.Execute();
                UndoCommands.Push(temp);
                RedoCommands.Pop();
            }
        }

        public static Stack<ICommand> UndoCommands = new Stack<ICommand>();
        public static Stack<ICommand> RedoCommands = new Stack<ICommand>();
    }

    public interface ICommand
    {
        void Execute(); // выполнение
        void Undo(); //откат

    }
    public sealed class ChangeColorCmd : ICommand
    {
        private Color prevColor;
        private Color newColor;
        public ChangeColorCmd(Color _prevColor, Color _newColor)
        {
            prevColor = _prevColor;
            newColor = _newColor;
        }
        Form1 f = new Form1();
        public void Execute()
        {
            f._FigureColor = newColor;
            //Figura.color = newColor;
        }
        public void Undo()
        {
            f._FigureColor = prevColor;
            //Figura.color = prevColor;
        }
    }
    public sealed class ChangeRadius : ICommand
    {
        private int prevRad;
        private int newRad;
        public ChangeRadius(int OldRad, int Newrad)
        {
            prevRad = OldRad;
            newRad = Newrad;
        }
        public void Execute()
        {
            Figura.R = newRad;
        }
        public void Undo()
        {
            Figura.R = prevRad;
        }
    }
    public sealed class AddVertex : ICommand
    {
        private List<Figura> _List;
        private Figura vertex;
        public AddVertex(List<Figura> _prev, Figura vrtx)
        {
            _List = _prev;
            vertex = vrtx;
        }
        Form1 f = new Form1();
        public void Execute()
        {
            _List.Add(vertex);
        }

        public void Undo()
        {
            _List.Remove(vertex);
        }
    }
    public sealed class DeleteVertex : ICommand
    {
        private List<Figura> _List;
        private Figura vertex;
        public DeleteVertex(List<Figura>_prev, Figura vrtx)
        {
            _List = _prev;
            vertex = vrtx;
        }
        Form1 f = new Form1();
        public void Execute()
        {
            _List.Remove(vertex);
        }
        public void Undo()
        {
            _List.Add(vertex);
        }
    }
    public sealed class DragAndDrop : ICommand
    {
        private List<Figura> polygon;
        private List<Figura> _dragged = new List<Figura>();
        private List<Point> startingPoints = new List<Point>();
        private List<Point> endPoints = new List<Point>();

        public DragAndDrop(List<Figura> vrtxList)
        {
            polygon = vrtxList;
        }

        Form1 f = new Form1();

        public void Execute()
        {
            foreach(Figura item in _dragged)
            {
                item.SetX = endPoints[endPoints.Count-1].X;
                item.SetY = endPoints[endPoints.Count - 1].Y;
            }
        }
        public void Undo()
        {
            for(int i=_dragged.Count-1; i>=0; i--)
            {
                _dragged[i].SetX = startingPoints[i].X;  
                _dragged[i].SetY = startingPoints[i].Y; 
            }
        }
        public void DdStart(int x, int y, bool ShallDrag)
        {
            #region waste
            //if (ShallDrag)
            //{
            //    foreach (Figura item in polygon)
            //    {
            //      _dragged.Add(item);
            //      startingPoints.Add(new Point(item.SetX, item.SetY));
            //    }
            //}
            #endregion
            foreach (Figura item in polygon)
            {
                if (item.Check(x, y))
                {
                    _dragged.Add(item);
                    startingPoints.Add(new Point(item.SetX, item.SetY));
                }
            }
        }
        public void DdDo(int MouseX, int MouseY, bool ShDr)
        {
            foreach (Figura s in _dragged)
            {
                if (s.IsDragged)
                {
                    s.SetX = MouseX - s.del_x;
                    s.SetY = MouseY - s.del_y;
                }
            }

            #region waste_deleteIt
            /*
            if (ShDr)
            {
                MessageBox.Show(" кря");
                foreach (Figura s in polygon)
                {
                    s.SetX = MouseX - s.del_x;
                    s.SetY = MouseY - s.del_y;
                }
            }
            else
            {
                foreach (Figura s in _dragged)
                {
                    if (s.IsDragged)
                    {
                        s.SetX = MouseX - s.del_x;
                        s.SetY = MouseY - s.del_y;
                    }
                }
            }
            */
            #endregion
        }
        public void DdEnd()
        {
            foreach(Figura item in _dragged)
            {
                endPoints.Add(new Point(item.SetX, item.SetY));
                item.IsDragged = false;
            }
        }
        public bool CanDragged(List<Figura> vrtx, int x, int y)
        {
            return true;
        }
        public void DdStartShell(List<Figura> pol)
        {
            foreach (Figura item in pol)
            {
                _dragged.Add(item);
                startingPoints.Add(new Point(item.SetX, item.SetY));

            }
        }
    }
}
