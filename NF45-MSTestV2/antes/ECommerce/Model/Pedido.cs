using System.Collections.Generic;

namespace ECommerce.Model
{
    public class Pedido : BaseModel
    {
        public string Cliente { get; set; }
        public List<ItemPedido> Itens { get; set; }
        public decimal Total { get; set; }
        public PedidoStatus Status { get; set; }
    }

    public enum PedidoStatus
    {
        Nenhum = 0,
        Aberto = 1,
        Fechado = 2
    }
}
