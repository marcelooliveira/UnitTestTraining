﻿using System.Collections.Generic;

namespace ECommerce.Model
{
    public class ItemPedido : BaseModel
    {
        public ItemPedido(int produtoId, int quantidade, decimal precoUnitario)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }

        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ProdutoId;
            yield return Quantidade;
            yield return PrecoUnitario;
        }
    }
}
