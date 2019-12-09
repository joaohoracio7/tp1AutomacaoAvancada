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
        private View view;
        private Queue<Produto> fila;
        private Thread t;
        private int nLinha;
        private static int qtdRedonda = 0;
        private static int qtdEspecifico = 0;
        protected Dictionary<string, Semaphore> recursos;
        public LinhaDeProducao(View V, int nLinha)
        {
            view = V;
            this.nLinha = nLinha;
            // Fila polimorfica
            fila = new Queue<Produto>();
            t = new Thread(new ThreadStart(ThreadProc));
            recursos = new Dictionary<string, Semaphore>
            {
                { "tesoura1", CentralRecursos.tesoura1 },
                { "tesoura2", CentralRecursos.tesoura2 },
            };
            t.Start();
        }

        private int getCountFila()
        {
            return fila.Count;
        }
        private void InsereFila(Produto P)
        {
            fila.Enqueue(P);
        }
        private void ThreadProc()
        {
            while(true)
            {
                if(fila.Count > 0)
                {
                    // Verifica se a tesoura da linha disponivel para poder começar a produzir
                    recursos["tesoura" + nLinha].WaitOne();
                    if (fila.Peek().GetType().Name == "CamisaRedonda")
                        qtdRedonda++;
                    else
                        qtdEspecifico++;
                    fila.Dequeue().Produz();
                    recursos["tesoura" + nLinha].Release();

                }
            }
        }
    }
}
