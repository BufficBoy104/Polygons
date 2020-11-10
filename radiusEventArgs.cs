using System;

namespace Тренировка_на_уроках
{
    public class radiusEventArgs : EventArgs
    {
        public int Rad { get; set; }
        public radiusEventArgs(int Rad)
        {
            this.Rad = Rad;
        }
    }
}