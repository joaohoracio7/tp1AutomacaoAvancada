using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1
{
    class Controller
    {
        private View view;
        private LinhaDeProducao l1;
        private LinhaDeProducao l2;
        public Controller(View V)
        {
            view = V;
            l1 = new LinhaDeProducao(1);
            l2 = new LinhaDeProducao(2);
        }
    }
}
