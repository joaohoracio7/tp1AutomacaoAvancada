using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TP1
{
    // Classe abstrata responsável por representar um produto e seu processo de produção
    public abstract class Produto
    {
        protected static int totalProdutos = 0;
        private readonly int codigoBarras;
        protected readonly int linha;
        protected bool feito;
        protected Thread t;
        protected View view;
        protected Dictionary<string, Semaphore> recursos;

        public Produto(View View, int l)
        {
            codigoBarras = totalProdutos++;
            linha = l;
            feito = false;
            view = View;
            recursos = new Dictionary<string, Semaphore>
            {
                { "tesoura1", CentralRecursos.tesoura1 },
                { "tesoura2", CentralRecursos.tesoura2 },
                { "maquinaCostura1", CentralRecursos.maquinaCostura1 },
                { "maquinaCostura2", CentralRecursos.maquinaCostura2 },
                { "estampadora", CentralRecursos.estampadora },
                { "empacotadora", CentralRecursos.empacotadora }
            };
            t = new Thread(new ThreadStart(ThreadProc));
            Produz();
        }

        public String GetCodigoBarras()
        {
            return codigoBarras.ToString();
        }

        public void Produz()
        {
            t.Start();
        }

        protected abstract void ThreadProc();
    }
}
