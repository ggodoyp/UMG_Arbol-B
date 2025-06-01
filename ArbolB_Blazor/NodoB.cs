namespace ArbolB_Blazor
{

    public class NodoB
    {
        public int[] Claves;
        public NodoB[] Hijos;
        public int Cuenta;
        public bool EsHoja => Hijos[0] == null; // Si no tiene hijo en 0, es hoja

        public NodoB(int orden)
        {
            Claves = new int[2 * orden ];    // máximo 2d - 1 claves
            Hijos = new NodoB[2 * orden];       // máximo 2d hijos
            Cuenta = 0;
        }

        public void InsertarClave(int clave)
        {
            int i = Cuenta ;
            while (i >= 0 && Claves[i] > clave)
            {
                Claves[i + 1] = Claves[i];
                i--;
            }
            Claves[i + 1] = clave;
            Cuenta++;
        }
    }


}
