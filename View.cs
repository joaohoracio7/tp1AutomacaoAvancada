using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP1
{
    public partial class View : Form
    {
        Controller Controller;
        private delegate void DelegateOut(string text);

        public View()
        {
            InitializeComponent();
            Controller = new Controller(this);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //Produto p = new ProdutoA();
            //Controller.PrintaNaTela(p.GetCodigoBarras());
            //new Work(this);

            richTextBox1.Text += "\n" + "é nois mano";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            new Work(this);
        }

        public void WriteTextSafe(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                var d = new DelegateOut(WriteTextSafe);
                richTextBox1.Invoke(d, text);
            }
            else
            {
                richTextBox1.AppendText(text + "else");
            }
        }
    }
}
