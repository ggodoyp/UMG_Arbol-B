namespace Arbol_B
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ArbolB nodoB = new ArbolB(3);

            int[] datos = { 10, 20, 5, 6, 12, 30, 7, 17, 2, 4, 18, 19, 31 };

            foreach (int dato in datos)
                nodoB.Insert(dato);

            Console.WriteLine("Recorrido del árbol B:");
            nodoB.Atravesar();
            Console.WriteLine();

            int buscar = 30;
            Console.WriteLine($"¿Existe el elemento {buscar}? " +
                (nodoB.Buscar(buscar) != null ? "Sí" : "No"));


            Console.WriteLine("\nEstructura del árbol B:");
            nodoB.ImprimirEstructura();

            Console.WriteLine("\n Eliminar del árbol B: 12");
            nodoB.Delete(12);

            Console.WriteLine("\nEstructura del árbol B:");
            nodoB.ImprimirEstructura();
                       
            Console.ReadKey();
        }
    }
}
