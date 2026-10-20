using System.Collections.Generic;
using System.Linq;

namespace Domain.Enums
{
    public static class CategoriaProducto
    {
        public const string Electronica = "Electrónica";
        public const string Ropa = "Ropa";
        public const string Hogar = "Hogar";
        public const string Deportes = "Deportes";
        public const string Oficina = "Oficina";
        public const string Otro = "Otro";

        public static readonly string[] Todas = new[]
        {
            Electronica, Ropa, Hogar, Deportes, Oficina, Otro
        };

        public static bool EsValida(string categoria)
        {
            return !string.IsNullOrWhiteSpace(categoria) && Todas.Contains(categoria);
        }
    }
}
