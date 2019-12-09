using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCP
{
    public class OP
    {
        private static int nOps = 0;
        private String nomeCliente;
        private int nOp;
        private int qtdProdutos;
        private String tipoGola;
        private String tipoEstampa;
        private DateTime dataPedido;

        public OP(String nomeCliente, decimal qtdProdutos, String tipoGola, String tipoEstampa)
        {
            this.nomeCliente = nomeCliente;
            this.nOp = ++nOps;
            this.qtdProdutos = int.Parse(qtdProdutos.ToString());
            this.tipoGola = tipoGola;
            this.tipoEstampa = tipoEstampa;
            dataPedido = DateTime.Now;
        }

        public override string ToString()
        {
            return nOp.ToString() + "\t" + nomeCliente + "\t" + qtdProdutos.ToString() + "\t" + tipoGola + "\t" + tipoEstampa + "\t" + dataPedido.ToString();
        }
    }
}
