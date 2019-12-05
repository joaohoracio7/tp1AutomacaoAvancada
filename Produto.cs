using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TP1
{
    // Classe abstrata responsável por representar um produto e seu processo de produção
    public class Produto
    {
        private static int totalProdutos = 0;
        private readonly int codigoBarras;
        private Boolean feito;
        private Thread t;
        private View view;

        public Produto(View View)
        {
            codigoBarras = totalProdutos++;
            feito = false;
            view = View;
            t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
        }

        public String GetCodigoBarras()
        {
            return codigoBarras.ToString();
        }

        public void Produz()
        {
            t.Start();
        }

        private void ThreadProc()
        {
            CentralRecursos.tesoura1.WaitOne();
            view.atualizaLinha(1, 1, "v", true);
            Thread.Sleep(1000);
            view.atualizaLabel("labelCorte1" , true);

            CentralRecursos.maquinaCostura1.WaitOne();
            view.atualizaLabel("labelCorte1", false);
            view.atualizaLinha(1, 1, "v", false);
            CentralRecursos.tesoura1.Release();
            view.atualizaLinha(1, 2, "v", true);
            Thread.Sleep(2000);
            view.atualizaLabel("labelCostura1", true);

            CentralRecursos.estampadora.WaitOne();
            view.atualizaLabel("labelCostura1", false);
            view.atualizaLinha(1, 2, "v", false);
            CentralRecursos.maquinaCostura1.Release();
            view.atualizaLinha(1, 3, "v", true);
            view.atualizaLabel("labelEstampadoraLinha1", true);
            Thread.Sleep(3000);
            view.atualizaLabel("labelEstampa1", true);

            CentralRecursos.empacotadora.WaitOne();
            view.atualizaLabel("labelEstampa1", false);
            view.atualizaLinha(1, 3, "v", false);
            view.atualizaLabel("labelEstampadoraLinha1", false);
            CentralRecursos.estampadora.Release();
            view.atualizaLinha(1, 4, "v", true);
            view.atualizaLabel("labelEmpacotadoraLinha1", true);
            Thread.Sleep(1000);

            view.atualizaLinha(1, 4, "v", false);
            view.atualizaLabel("labelEmpacotadoraLinha1", false);
            CentralRecursos.empacotadora.Release();

            t.Abort();
        }
    }
}
