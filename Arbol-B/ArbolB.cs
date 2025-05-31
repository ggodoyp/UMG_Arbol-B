using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbol_B
{
    internal class ArbolB
    {
        private ArbolNodo NodoB;
        private int T;

        public ArbolB(int t)
        {
            NodoB = null;
            T = t;
        }

        public void Atravesar()
        {
            if (NodoB != null) NodoB.Atravesar();
        }

        public ArbolNodo Buscar(int key)
        {
            return (NodoB == null) ? null : NodoB.Buscar(key);
        }

        public void ImprimirEstructura()
        {
            if (NodoB != null)
                NodoB.ImprimirEstructura();
        }

        public void Insert(int key)
        {
            if (NodoB == null)
            {
                NodoB = new ArbolNodo(T, true);
                NodoB.Llaves[0] = key;
                NodoB.LlaveCount = 1;
            }
            else
            {
                if (NodoB.LlaveCount == 2 * T - 1)
                {
                    ArbolNodo s = new ArbolNodo(T, false);
                    s.Hijo[0] = NodoB;
                    s.HijoDividido(0, NodoB);

                    int i = 0;
                    if (s.Llaves[0] < key)
                        i++;

                    s.Hijo[i].InsertarNoLleno(key);
                    NodoB = s;
                }
                else
                {
                    NodoB.InsertarNoLleno(key);
                }
            }
        }

        public void Delete(int key)
        {
            if (NodoB == null)
            {
                Console.WriteLine("El árbol está vacío.");
                return;
            }

            NodoB.Remove(key);

            if (NodoB.LlaveCount == 0)
            {
                if (NodoB.EsHoja)
                    NodoB = null;
                else
                    NodoB = NodoB.Hijo[0];
            }
        }
    }
}
