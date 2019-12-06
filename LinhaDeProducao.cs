using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TP1
{
    // Classe responsável por abstrair a linha de produção
    public class LinhaDeProducao
    {
        private Queue<Produto> fila;
        private Thread t;

        public LinhaDeProducao()
        {
            // Fila polimorfica
            fila = new Queue<Produto>(3);
            t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
        }

        private void ThreadProc()
        {
            while(true)
            {

            }
        }
    }
}
