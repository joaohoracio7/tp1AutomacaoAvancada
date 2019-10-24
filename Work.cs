using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TP1
{
    class Work
    {
        private Thread t;
        bool feito = false;
        private View view;

        public Work(View View)
        {
            view = View;
            t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
        }

        private void ThreadProc()
        {
            while (true)
            {
                view.WriteTextSafe("This text was set safely.");
                Thread.Sleep(10);
            }
        }
    }
}
