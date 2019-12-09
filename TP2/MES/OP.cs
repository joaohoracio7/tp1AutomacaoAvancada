using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES
{
    public class OP
    {
        private String nomeCliente;
        private int nOp;
        private int qtdProdutos;
        private String tipoGola;
        private String tipoEstampa;
        private DateTime dataPedido;

        public OP(int nOp, String nomeCliente, int qtdProdutos, String tipoGola, String tipoEstampa, String data)
        {
            this.nomeCliente = nomeCliente;
            this.nOp = nOp;
            this.qtdProdutos = qtdProdutos;
            this.tipoGola = tipoGola;
            this.tipoEstampa = tipoEstampa;
            String[] d = data.Split(' ');
            this.dataPedido = new DateTime(int.Parse(d[0].Split('/')[2]), int.Parse(d[0].Split('/')[1]), int.Parse(d[0].Split('/')[0]), int.Parse(d[1].Split(':')[0]), int.Parse(d[1].Split(':')[1]), int.Parse(d[1].Split(':')[2]));
        }

        public int getQtdProdutos()
        {
            return qtdProdutos;
        }

        public void setQtdProdutos(int qtd)
        {
            qtdProdutos = qtd;
        }
    
        public String getTipoGola()
        {
            return tipoGola.ToLower();
        }
        

        public override String ToString()
        {
            return nOp.ToString() + "\t" + nomeCliente + "\t" + qtdProdutos.ToString() + "\t" + tipoGola + "\t" + tipoEstampa + "\t" + dataPedido.ToString();
        }
    }
}
