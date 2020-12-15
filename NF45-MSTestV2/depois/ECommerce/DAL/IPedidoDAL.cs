using ECommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DAL
{
    public interface IPedidoDAL
    {
        Pedido Create(string Cliente);
        void AddItem(Pedido pedido, ItemPedido itemPedido);
        void UpdateItem(Pedido pedido, ItemPedido itemPedido);
        void RemoveItem(Pedido pedido, ItemPedido itemPedido);
        void UpdateStatus(Pedido pedido, int status);
    }
}
