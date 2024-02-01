using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarWashing
{
    public partial class Start_Screen : Form
    {
        public Start_Screen()
        {
            InitializeComponent();
        }

        bool wait = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!wait)
            {
                this.Opacity += 0.01;
            }
            if (this.Opacity == 1.0)
            {
                wait = true;
            }
            if (wait)
            {
                this.Opacity -= 0.01;
                if (this.Opacity == 0)
                {
                    Interface go = new Interface();
                    go.Show();
                    this.Hide();
                    timer1.Enabled = false;
                }
            } 
        }
        private void Start_Screen_Load(object sender, EventArgs e)
        {

        }
    }
}
