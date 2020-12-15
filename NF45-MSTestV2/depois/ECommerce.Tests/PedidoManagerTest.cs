using System;
using ECommerce.BLL;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECommerce.Tests
{
    [TestClass]
    public class PedidoManagerTest
    {
        [TestMethod]
        public void CriarPedido_Success()
        {
            //arrange
            IPedidoManager pedidoManager = new PedidoManager();
            
            //act
            var pedido = pedidoManager.CriarPedido("Fulano de Tal");

            //assert
            Assert.AreEqual(Model.PedidoStatus.Aberto, pedido.Status);
            Assert.AreEqual("Fulano de Tal", pedido.Cliente);
            Assert.AreEqual(0, pedido.Itens.Count);

            pedido.Status.Should().Be(Model.PedidoStatus.Aberto);
            pedido.Cliente.Should().Be("Fulano de Tal");
            pedido.Itens.Should().BeEmpty();
        }
    }
}
