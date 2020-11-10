using System.Windows.Forms;
using System;

namespace Тренировка_на_уроках
{
    public delegate void RadiusChageEventHandler(object sender, radiusEventArgs e);

    public partial class Form3 : Form
    {
        public bool IsOpen = false;
        public event RadiusChageEventHandler OnRadius;

        public Form3()
        {
            InitializeComponent();
            
        }

        private void trackBar1_Scroll(object sender, System.EventArgs e)
        {
            if (OnRadius != null)
            {
                OnRadius(this.trackBar1, new radiusEventArgs(trackBar1.Value));
            }
            
        }

        private void trackBar1_Move(object sender, EventArgs e)
        {

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            
        }
    }
}
