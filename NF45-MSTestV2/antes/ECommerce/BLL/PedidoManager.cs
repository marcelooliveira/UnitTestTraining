using ECommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BLL
{
    public interface IPedidoManager
    {
        Pedido CriarPedido(string cliente);
        ItemPedido AdicionarItem(Pedido pedido, string codigo, int quantidade);
        void RemoverItem(Pedido pedido, string codigo);
        void AtualizarItem(Pedido pedido, string codigo, int quantidade);
        void FecharPedido(Pedido pedido);
    }

    public class PedidoManager : IPedidoManager
    {
        public ItemPedido AdicionarItem(Pedido pedido, string codigo, int quantidade)
        {
            throw new NotImplementedException();
        }

        public void AtualizarItem(Pedido pedido, string codigo, int quantidade)
        {
            throw new NotImplementedException();
        }

        public Pedido CriarPedido(string cliente)
        {
            throw new NotImplementedException();
        }

        public void FecharPedido(Pedido pedido)
        {
            throw new NotImplementedException();
        }

        public void RemoverItem(Pedido pedido, string codigo)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ProdutoNaoEncontradoException : Exception
    {
        public override string Message => "Produto não encontrado.";
        public ProdutoNaoEncontradoException() { }
        public ProdutoNaoEncontradoException(string message) : base(message) { }
        public ProdutoNaoEncontradoException(string message, Exception inner) : base(message, inner) { }
        protected ProdutoNaoEncontradoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class QuantidadeInvalidaException : Exception
    {
        public QuantidadeInvalidaException() { }
        public QuantidadeInvalidaException(string message) : base(message) { }
        public QuantidadeInvalidaException(string message, Exception inner) : base(message, inner) { }
        protected QuantidadeInvalidaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class StatusInvalidoException : Exception
    {
        public StatusInvalidoException() { }
        public StatusInvalidoException(string message) : base(message) { }
        public StatusInvalidoException(string message, Exception inner) : base(message, inner) { }
        protected StatusInvalidoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
