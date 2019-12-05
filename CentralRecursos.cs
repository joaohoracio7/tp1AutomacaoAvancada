using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TP1
{
    // Classe que tratará exclusão mútua
    public class CentralRecursos
    {
        // Cria uma instância de semaforo binário para cada recurso
        // Que são estáticos para que tenha apenas um semaforo para todas as instâncias

        public static Semaphore tesoura1 = new Semaphore(1, 1);
        public static Semaphore tesoura2 = new Semaphore(1, 1);
        public static Semaphore maquinaCostura1 = new Semaphore(1, 1);
        public static Semaphore maquinaCostura2 = new Semaphore(1, 1);
        public static Semaphore estampadora = new Semaphore(1, 1);
        public static Semaphore empacotadora = new Semaphore(1, 1);

    }
}
