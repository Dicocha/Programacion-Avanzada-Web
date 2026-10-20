using System.Collections.Generic;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IProductoRepository
    {
        void Add(Producto producto);
        IList<Producto> GetAll();
    }
}
