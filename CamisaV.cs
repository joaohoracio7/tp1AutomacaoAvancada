using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TP1
{
    public class CamisaV : Produto
    {
        public CamisaV(View view, int linha) : base(view, linha) { }

        protected override void ThreadProc()
        {
            // Verifica se a tesoura da linha esta realmente disponivel
            recursos["tesoura"+linha.ToString()].WaitOne();
            // Quando estiver, representa a etapa do processo e simula o tempo de execução
            view.atualizaLinha(linha, 1, "v", true);
            Thread.Sleep(1000);
            // Indica que vai tentar pegar o proximo recurso
            view.atualizaLabel("labelCorte"+linha.ToString(), true);
            // Tenta pegar o proximo recurso
            recursos["maquinaCostura" + linha.ToString()].WaitOne();
            // Quando conseguir remove a indicação da espera
            view.atualizaLabel("labelCorte" + linha.ToString(), false);
            // Retira a representação do processo anterior
            view.atualizaLinha(linha, 1, "v", false);
            // Libera o recurso anterior
            recursos["tesoura" + linha.ToString()].Release();
            // Representa a etapa do processo e simula o tempo de execução
            view.atualizaLinha(linha, 2, "v", true);
            Thread.Sleep(2000);
            // Indica que vai tentar pegar o proximo recurso
            view.atualizaLabel("labelCostura" + linha.ToString(), true);
            // Tenta pegar o proximo recurso
            recursos["estampadora"].WaitOne();
            // Quando conseguir remove a indicação da espera
            view.atualizaLabel("labelCostura" + linha.ToString(), false);
            // Retira a representação do processo anterior
            view.atualizaLinha(linha, 2, "v", false);
            // Libera o recurso anterior
            recursos["maquinaCostura" + linha.ToString()].Release();
            // Representa a etapa do processo e simula o tempo de execução
            view.atualizaLinha(linha, 3, "v", true);
            view.atualizaLabel("labelEstampadoraLinha" + linha.ToString(), true);
            Thread.Sleep(3000);
            // Indica que vai tentar pegar o proximo recurso
            view.atualizaLabel("labelEstampa" + linha.ToString(), true);
            // Tenta pegar o proximo recurso
            recursos["empacotadora"].WaitOne();
            // Quando conseguir remove a indicação da espera
            view.atualizaLabel("labelEstampa" + linha.ToString(), false);
            // Retira a representação do processo anterior
            view.atualizaLinha(linha, 3, "v", false);
            view.atualizaLabel("labelEstampadoraLinha" + linha.ToString(), false);
            // Libera o recurso anterior
            recursos["estampadora"].Release();
            // Representa a etapa do processo e simula o tempo de execução
            view.atualizaLinha(linha, 4, "v", true);
            view.atualizaLabel("labelEmpacotadoraLinha" + linha.ToString(), true);
            Thread.Sleep(1000);
            // Remove a indicação da espera
            view.atualizaLinha(linha, 4, "v", false);
            view.atualizaLabel("labelEmpacotadoraLinha" + linha.ToString(), false);
            // Libera o recurso anterior
            recursos["empacotadora"].Release();
            // Indica que o produto está feito e termina a thread
            feito = true;
            t.Abort();
        }
    }
}
