using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbol_B
{
    public class ArbolNodo
    {
        public int[] Llaves { get; set; }
        public int T { get; set;}
        public ArbolNodo[] Hijo { get; set;}
        public int LlaveCount { get; set; }
        public bool EsHoja { get; set; }

        public ArbolNodo(int t, bool esHoja)
        {
            T = t;
            EsHoja = esHoja;
            Llaves = new int[2 * t - 1];
            Hijo = new ArbolNodo[2 * t];
            LlaveCount = 0;
        }

        public void Atravesar()
        {
            for (int i = 0; i < LlaveCount; i++)
            {
                if (!EsHoja)
                    Hijo[i].Atravesar();

                Console.Write(Llaves[i] + " ");
            }

            if (!EsHoja)
                Hijo[LlaveCount].Atravesar();
        }

        public ArbolNodo Buscar(int key)
        {
            int i = 0;
            while (i < LlaveCount && key > Llaves[i])
                i++;

            if (i < LlaveCount && Llaves[i] == key)
                return this;

            return EsHoja ? null : Hijo[i].Buscar(key);
        }

        public void InsertarNoLleno(int key)
        {
            int i = LlaveCount - 1;

            if (EsHoja)
            {
                while (i >= 0 && Llaves[i] > key)
                {
                    Llaves[i + 1] = Llaves[i];
                    i--;
                }

                Llaves[i + 1] = key;
                LlaveCount++;
            }
            else
            {
                while (i >= 0 && Llaves[i] > key)
                    i--;

                if (Hijo[i + 1].LlaveCount == 2 * T - 1)
                {
                    HijoDividido(i + 1, Hijo[i + 1]);

                    if (Llaves[i + 1] < key)
                        i++;
                }

                Hijo[i + 1].InsertarNoLleno(key);
            }
        }

        public void HijoDividido(int i, ArbolNodo y)
        {
            ArbolNodo z = new ArbolNodo(y.T, y.EsHoja);
            z.LlaveCount = T - 1;

            for (int j = 0; j < T - 1; j++)
                z.Llaves[j] = y.Llaves[j + T];

            if (!y.EsHoja)
            {
                for (int j = 0; j < T; j++)
                    z.Hijo[j] = y.Hijo[j + T];
            }

            y.LlaveCount = T - 1;

            for (int j = LlaveCount; j >= i + 1; j--)
                Hijo[j + 1] = Hijo[j];

            Hijo[i + 1] = z;

            for (int j = LlaveCount - 1; j >= i; j--)
                Llaves[j + 1] = Llaves[j];

            Llaves[i] = y.Llaves[T - 1];
            LlaveCount++;
        }

        public void ImprimirEstructura(string indent = "", bool esUltimo = true)
        {
            Console.Write(indent);
            Console.Write(esUltimo ? "└── " : "├── ");
            Console.WriteLine("[" + string.Join(", ", Llaves.Take(LlaveCount)) + "]");

            for (int i = 0; i <= LlaveCount; i++)
            {
                if (Hijo[i] != null)
                {
                    Hijo[i].ImprimirEstructura(indent + (esUltimo ? "    " : "│   "), i == LlaveCount);
                }
            }
        }

        public void Remove(int key)
        {
            int idx = FindKey(key);

            if (idx < LlaveCount && Llaves[idx] == key)
            {
                if (EsHoja)
                    RemoveFromLeaf(idx);
                else
                    RemoveFromNonLeaf(idx);
            }
            else
            {
                if (EsHoja)
                    return;

                bool flag = (idx == LlaveCount);

                if (Hijo[idx].LlaveCount < T)
                    Fill(idx);

                if (flag && idx > LlaveCount)
                    Hijo[idx - 1].Remove(key);
                else
                    Hijo[idx].Remove(key);
            }
        }

        private void RemoveFromLeaf(int idx)
        {
            for (int i = idx + 1; i < LlaveCount; ++i)
                Llaves[i - 1] = Llaves[i];
            LlaveCount--;
        }

        private void RemoveFromNonLeaf(int idx)
        {
            int key = Llaves[idx];

            if (Hijo[idx].LlaveCount >= T)
            {
                int pred = GetPredecessor(idx);
                Llaves[idx] = pred;
                Hijo[idx].Remove(pred);
            }
            else if (Hijo[idx + 1].LlaveCount >= T)
            {
                int succ = GetSuccessor(idx);
                Llaves[idx] = succ;
                Hijo[idx + 1].Remove(succ);
            }
            else
            {
                Merge(idx);
                Hijo[idx].Remove(key);
            }
        }

        private int GetPredecessor(int idx)
        {
            ArbolNodo cur = Hijo[idx];
            while (!cur.EsHoja)
                cur = cur.Hijo[cur.LlaveCount];
            return cur.Llaves[cur.LlaveCount - 1];
        }

        private int GetSuccessor(int idx)
        {
            ArbolNodo cur = Hijo[idx + 1];
            while (!cur.EsHoja)
                cur = cur.Hijo[0];
            return cur.Llaves[0];
        }

        private void Fill(int idx)
        {
            if (idx != 0 && Hijo[idx - 1].LlaveCount >= T)
                BorrowFromPrev(idx);
            else if (idx != LlaveCount && Hijo[idx + 1].LlaveCount >= T)
                BorrowFromNext(idx);
            else
            {
                if (idx != LlaveCount)
                    Merge(idx);
                else
                    Merge(idx - 1);
            }
        }

        private void BorrowFromPrev(int idx)
        {
            ArbolNodo child = Hijo[idx];
            ArbolNodo sibling = Hijo[idx - 1];

            for (int i = child.LlaveCount - 1; i >= 0; --i)
                child.Llaves[i + 1] = child.Llaves[i];

            if (!child.EsHoja)
            {
                for (int i = child.LlaveCount; i >= 0; --i)
                    child.Hijo[i + 1] = child.Hijo[i];
            }

            child.Llaves[0] = Llaves[idx - 1];

            if (!child.EsHoja)
                child.Hijo[0] = sibling.Hijo[sibling.LlaveCount];

            Llaves[idx - 1] = sibling.Llaves[sibling.LlaveCount - 1];

            child.LlaveCount += 1;
            sibling.LlaveCount -= 1;
        }

        private void BorrowFromNext(int idx)
        {
            ArbolNodo child = Hijo[idx];
            ArbolNodo sibling = Hijo[idx + 1];

            child.Llaves[child.LlaveCount] = Llaves[idx];

            if (!child.EsHoja)
                child.Hijo[child.LlaveCount + 1] = sibling.Hijo[0];

            Llaves[idx] = sibling.Llaves[0];

            for (int i = 1; i < sibling.LlaveCount; ++i)
                sibling.Llaves[i - 1] = sibling.Llaves[i];

            if (!sibling.EsHoja)
            {
                for (int i = 1; i <= sibling.LlaveCount; ++i)
                    sibling.Hijo[i - 1] = sibling.Hijo[i];
            }

            child.LlaveCount += 1;
            sibling.LlaveCount -= 1;
        }

        private void Merge(int idx)
        {
            ArbolNodo child = Hijo[idx];
            ArbolNodo sibling = Hijo[idx + 1];

            child.Llaves[T - 1] = Llaves[idx];

            for (int i = 0; i < sibling.LlaveCount; ++i)
                child.Llaves[i + T] = sibling.Llaves[i];

            if (!child.EsHoja)
            {
                for (int i = 0; i <= sibling.LlaveCount; ++i)
                    child.Hijo[i + T] = sibling.Hijo[i];
            }

            for (int i = idx + 1; i < LlaveCount; ++i)
                Llaves[i - 1] = Llaves[i];

            for (int i = idx + 2; i <= LlaveCount; ++i)
                Hijo[i - 1] = Hijo[i];

            child.LlaveCount += sibling.LlaveCount + 1;
            LlaveCount--;
        }

        private int FindKey(int key)
        {
            int idx = 0;
            while (idx < LlaveCount && Llaves[idx] < key)
                ++idx;
            return idx;
        }

    }

}
