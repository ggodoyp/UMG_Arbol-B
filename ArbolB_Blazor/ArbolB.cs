namespace ArbolB_Blazor
{

    using System;

    public class ArbolB
    {
        public NodoB Raiz;
        private int orden;

        public ArbolB(int orden)
        {
            this.orden = orden;
            Raiz = new NodoB(orden);
        }

        ///////////////////////////////////////////////////////////////////////  INSERTAR ///////////////////////////////////////////////////////////////////////
        public void Insertar(int clave)
        {
            if (Raiz.Cuenta == 2 * orden)
            {
                var nuevaRaiz = new NodoB(orden);
                nuevaRaiz.Hijos[0] = Raiz;
                DividirHijo(nuevaRaiz, 0, Raiz);
                Raiz = nuevaRaiz;
            }

            InsertarNoLleno(Raiz, clave);
        }

        private void InsertarNoLleno(NodoB nodo, int clave)
        {
            int i = nodo.Cuenta - 1;

            if (nodo.EsHoja)
            {
                while (i >= 0 && clave < nodo.Claves[i])
                {
                    nodo.Claves[i + 1] = nodo.Claves[i];
                    i--;
                }
                nodo.Claves[i + 1] = clave;
                nodo.Cuenta++;
            }
            else
            {
                while (i >= 0 && clave < nodo.Claves[i])
                {
                    i--;
                }
                i++;

                if (nodo.Hijos[i].Cuenta == 2 * orden)
                {
                    DividirHijo(nodo, i, nodo.Hijos[i]);

                    if (clave > nodo.Claves[i])
                        i++;
                }
                InsertarNoLleno(nodo.Hijos[i], clave);
            }
        }

        private void DividirHijo(NodoB padre, int indice, NodoB hijo)
        {
            NodoB nuevo = new NodoB(orden);

            // Copiar las claves derechas (después de la mediana)
            for (int j = 0; j < orden - 1; j++)
            {
                nuevo.Claves[j] = hijo.Claves[j + orden + 1];
            }
            nuevo.Cuenta = orden - 1;

            // Si no es hoja, copiar los hijos derechos
            if (!hijo.EsHoja)
            {
                for (int j = 0; j < orden; j++)
                {
                    nuevo.Hijos[j] = hijo.Hijos[j + orden + 1];
                }
            }

            // Mover hijos y claves del padre para hacer espacio
            for (int j = padre.Cuenta; j > indice; j--)
            {
                padre.Hijos[j + 1] = padre.Hijos[j];
                padre.Claves[j] = padre.Claves[j - 1];
            }

            // Insertar nuevo hijo
            padre.Hijos[indice + 1] = nuevo;

            // Subir clave mediana al padre
            padre.Claves[indice] = hijo.Claves[orden];
            padre.Cuenta++;

            // Ajustar el nodo hijo (izquierdo) para que solo tenga las claves de la izquierda
            hijo.Cuenta = orden;
        }

        ///////////////////////////////////////////////////////////////////////  BUSCAR ///////////////////////////////////////////////////////////////////////


        public bool Buscar(int clave)
        {
            return BuscarEnNodo(Raiz, clave);
        }

        private bool BuscarEnNodo(NodoB nodo, int clave)
        {
            int i = 0;

            while (i < nodo.Cuenta && clave > nodo.Claves[i])
            {
                i++;
            }

            if (i < nodo.Cuenta && clave == nodo.Claves[i])
            {
                return true;
            }

            if (nodo.EsHoja)
            {
                return false;
            }

            return BuscarEnNodo(nodo.Hijos[i], clave);
        }

        ///////////////////////////////////////////////////////////////////////  ELIMINAR ///////////////////////////////////////////////////////////////////////

        public void Eliminar(int clave)
        {
            Eliminar(Raiz, clave);

            // Paso final: si la raíz quedó sin claves y tiene un solo hijo, reducir altura
            if (Raiz.Cuenta == 0 && !Raiz.EsHoja)
            {
                Raiz = Raiz.Hijos[0];
            }
        }

        private void Eliminar(NodoB nodo, int clave)
        {
            int idx = 0;
            while (idx < nodo.Cuenta && clave > nodo.Claves[idx])
                idx++;

            // Paso 1: La clave está en este nodo
            if (idx < nodo.Cuenta && nodo.Claves[idx] == clave)
            {
                if (nodo.EsHoja)
                {
                    // 1.1 Eliminar directamente de una hoja
                    for (int i = idx; i < nodo.Cuenta - 1; i++)
                        nodo.Claves[i] = nodo.Claves[i + 1];
                    nodo.Cuenta--;
                }
                else
                {
                    // 3. Clave está en un nodo interno
                    NodoB hijoIzq = nodo.Hijos[idx];
                    NodoB hijoDer = nodo.Hijos[idx + 1];

                    if (hijoIzq.Cuenta >= orden)
                    {
                        // 3.1 Reemplazar por predecesor
                        int predecesor = ObtenerPredecesor(hijoIzq);
                        nodo.Claves[idx] = predecesor;
                        Eliminar(hijoIzq, predecesor);
                    }
                    else if (hijoDer.Cuenta >= orden)
                    {
                        // 3.1 Reemplazar por sucesor
                        int sucesor = ObtenerSucesor(hijoDer);
                        nodo.Claves[idx] = sucesor;
                        Eliminar(hijoDer, sucesor);
                    }
                    else
                    {
                        // 3.2 Fusionar ambos hijos y bajar la clave
                        Fusionar(nodo, idx);
                        Eliminar(hijoIzq, clave); // hijoIzq ahora tiene todo fusionado
                    }
                }
            }
            else
            {
                // Paso 2: Clave no está en este nodo
                if (nodo.EsHoja)
                    return; // clave no existe

                bool irUltimo = (idx == nodo.Cuenta);
                NodoB hijo = nodo.Hijos[idx];

                // 2.1 Si el hijo tiene menos de orden claves, corregirlo
                if (hijo.Cuenta < orden)
                {
                    CorregirHijo(nodo, idx);
                }

                if (irUltimo && idx > nodo.Cuenta)
                    Eliminar(nodo.Hijos[idx - 1], clave);
                else
                    Eliminar(nodo.Hijos[idx], clave);
            }
        }

        private int ObtenerPredecesor(NodoB nodo)
        {
            while (!nodo.EsHoja)
                nodo = nodo.Hijos[nodo.Cuenta];
            return nodo.Claves[nodo.Cuenta - 1];
        }

        private int ObtenerSucesor(NodoB nodo)
        {
            while (!nodo.EsHoja)
                nodo = nodo.Hijos[0];
            return nodo.Claves[0];
        }

        // Paso 2.1: Redistribuir si se puede, si no fusionar
        private void CorregirHijo(NodoB padre, int idx)
        {
            if (idx > 0 && padre.Hijos[idx - 1].Cuenta >= orden)
            {
                TomarPrestadoIzquierda(padre, idx);
            }
            else if (idx < padre.Cuenta && padre.Hijos[idx + 1].Cuenta >= orden)
            {
                TomarPrestadoDerecha(padre, idx);
            }
            else
            {
                if (idx < padre.Cuenta)
                    Fusionar(padre, idx);
                else
                    Fusionar(padre, idx - 1);
            }
        }

        // Redistribución desde la izquierda
        private void TomarPrestadoIzquierda(NodoB padre, int idx)
        {
            NodoB hijo = padre.Hijos[idx];
            NodoB hermano = padre.Hijos[idx - 1];

            for (int i = hijo.Cuenta; i > 0; i--)
                hijo.Claves[i] = hijo.Claves[i - 1];

            if (!hijo.EsHoja)
            {
                for (int i = hijo.Cuenta + 1; i > 0; i--)
                    hijo.Hijos[i] = hijo.Hijos[i - 1];
            }

            hijo.Claves[0] = padre.Claves[idx - 1];
            if (!hijo.EsHoja)
                hijo.Hijos[0] = hermano.Hijos[hermano.Cuenta];

            padre.Claves[idx - 1] = hermano.Claves[hermano.Cuenta - 1];
            hijo.Cuenta++;
            hermano.Cuenta--;
        }

        // Redistribución desde la derecha
        private void TomarPrestadoDerecha(NodoB padre, int idx)
        {
            NodoB hijo = padre.Hijos[idx];
            NodoB hermano = padre.Hijos[idx + 1];

            hijo.Claves[hijo.Cuenta] = padre.Claves[idx];
            if (!hijo.EsHoja)
                hijo.Hijos[hijo.Cuenta + 1] = hermano.Hijos[0];

            padre.Claves[idx] = hermano.Claves[0];

            for (int i = 0; i < hermano.Cuenta - 1; i++)
                hermano.Claves[i] = hermano.Claves[i + 1];

            if (!hermano.EsHoja)
            {
                for (int i = 0; i < hermano.Cuenta; i++)
                    hermano.Hijos[i] = hermano.Hijos[i + 1];
            }

            hijo.Cuenta++;
            hermano.Cuenta--;
        }

        // Fusión de hijo con su hermano derecho, bajando la clave del padre
        private void Fusionar(NodoB padre, int idx)
        {
            NodoB hijoIzquierdo = padre.Hijos[idx];
            NodoB hijoDerecho = padre.Hijos[idx + 1];

            // Mover clave del padre hacia el hijo izquierdo
            hijoIzquierdo.Claves[orden - 1] = padre.Claves[idx];

            // Copiar claves del hijo derecho al hijo izquierdo
            for (int i = 0; i < hijoDerecho.Cuenta; i++)
            {
                hijoIzquierdo.Claves[i + orden] = hijoDerecho.Claves[i];
            }

            // Si no es hoja, mover también los hijos del hijo derecho
            if (!hijoDerecho.EsHoja)
            {
                for (int i = 0; i <= hijoDerecho.Cuenta; i++)
                {
                    hijoIzquierdo.Hijos[i + orden] = hijoDerecho.Hijos[i];
                }
            }

            // Actualizar cantidad de claves en el hijo fusionado
            hijoIzquierdo.Cuenta += hijoDerecho.Cuenta + 1;

            // Mover claves del padre una posición a la izquierda
            for (int i = idx + 1; i < padre.Cuenta; i++)
            {
                padre.Claves[i - 1] = padre.Claves[i];
                padre.Hijos[i] = padre.Hijos[i + 1];
            }

            // Limpiar el último hijo y clave del padre
            padre.Claves[padre.Cuenta - 1] = 0;
            padre.Hijos[padre.Cuenta] = null;

            // Decrementar la cantidad de claves del padre
            padre.Cuenta--;
        }




    }
}

