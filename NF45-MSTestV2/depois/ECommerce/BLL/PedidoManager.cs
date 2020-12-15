﻿using ECommerce.DAL;
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
        void AdicionarItem(string codigo, int quantidade);
        void RemoverItem(string codigo);
        void AtualizarItem(string codigo, int quantidade);
        void FecharPedido(Pedido pedido);
    }

    public class PedidoManager : IPedidoManager
    {
        private readonly ILog log;
        private readonly IPedidoDAL pedidoDAL;

        public PedidoManager(ILog log, IPedidoDAL pedidoDAL)
        {
            this.log = log;
            this.pedidoDAL = pedidoDAL;
        }

        public void AdicionarItem(string codigo, int quantidade)
        {
            throw new NotImplementedException();
        }

        public void AtualizarItem(string codigo, int quantidade)
        {
            throw new NotImplementedException();
        }

        public Pedido CriarPedido(string cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente))
            {
                throw new ArgumentNullException(cliente);
            }

            Pedido pedido = null;
            try
            {
                pedido = pedidoDAL.Create(cliente);
                log.Info($"Pedido {pedido.Id} gravado com sucesso.");
            }
            catch (Exception exc)
            {
                log.Error("Erro ao criar pedido no banco de dados.");
                throw;
            }

            return pedido;
        }

        public void FecharPedido(Pedido pedido)
        {
            throw new NotImplementedException();
        }

        public void RemoverItem(string codigo)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ProdutoNaoEncontradoException : Exception
    {
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
