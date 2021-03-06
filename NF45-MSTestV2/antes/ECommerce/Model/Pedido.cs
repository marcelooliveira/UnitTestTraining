﻿using System.Collections.Generic;

namespace ECommerce.Model
{
    public class Pedido : BaseModel
    {
        public string Cliente { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public decimal Total { get; set; }
        public PedidoStatus Status { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Cliente;
            yield return Itens;
            yield return Total;
            yield return Status;
        }
    }

    public enum PedidoStatus
    {
        Nenhum = 0,
        Aberto = 1,
        Fechado = 2
    }
}
