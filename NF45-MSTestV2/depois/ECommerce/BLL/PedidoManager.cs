using ECommerce.DAL;
using ECommerce.Model;
using log4net;
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
        private readonly ILog log;
        private readonly IPedidoDAL pedidoDAL;
        private readonly IProdutoDAL produtoDAL;

        public PedidoManager(ILog log, IPedidoDAL pedidoDAL, IProdutoDAL produtoDAL)
        {
            this.log = log;
            this.pedidoDAL = pedidoDAL;
            this.produtoDAL = produtoDAL;
        }

        public ItemPedido AdicionarItem(Pedido pedido, string codigo, int quantidade)
        {
            if (pedido == null)
            {
                throw new ArgumentNullException(nameof(pedido));
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("Erro no código", nameof(codigo));
            }

            if (quantidade < 1 )
            {
                throw new ArgumentOutOfRangeException(nameof(quantidade));
            }

            if (pedido.Status != PedidoStatus.Aberto)
            {
                throw new StatusInvalidoException();
            }

            Produto produto;
            try
            {
                produto = produtoDAL.Get(codigo);
            }
            catch (KeyNotFoundException ex)
            {
                throw new ProdutoNaoEncontradoException("Produto não encontrado", ex);
            }

            if (pedido.Itens == null)
            {
                pedido.Itens = new List<ItemPedido>();
            }

            ItemPedido item = new ItemPedido(produto.Id, quantidade, produto.PrecoUnitario);
            pedido.Itens.Add(item);

            return item;
        }

        private Exception NotImplementedException()
        {
            throw new NotImplementedException();
        }

        public void AtualizarItem(Pedido pedido, string codigo, int quantidade)
        {
            throw new NotImplementedException();
        }

        public Pedido CriarPedido(string cliente)
        {
            //var pedido = new Pedido
            //{
            //    Cliente = cliente,
            //    Status = PedidoStatus.Aberto,
            //    Itens = new List<ItemPedido>()
            //};
            //return pedido;

            if (string.IsNullOrWhiteSpace(cliente))
            {
                throw new ArgumentNullException(nameof(cliente));
            }

            Pedido pedido = null;
            try
            {
                pedido = pedidoDAL.Create(cliente);
            }
            catch (Exception ex)
            {
                log.Error("Erro ao criar pedido no banco de dados.");
                throw;
            }

            log.Info($"Pedido {pedido.Id} gravado com sucesso.");
            return pedido;
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
