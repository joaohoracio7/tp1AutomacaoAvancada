using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1
{
    abstract class Produto
    {
        static int totalProdutos = 0;
        private readonly int codigoBarras;
        private Work[] w = new Work[4];

        public Produto()
        {
            codigoBarras = totalProdutos++;
        }

        public String GetCodigoBarras()
        {
            return codigoBarras.ToString();
        }
    }
}
