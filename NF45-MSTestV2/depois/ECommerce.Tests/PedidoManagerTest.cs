using System;
using System.Collections.Generic;
using ECommerce.BLL;
using ECommerce.DAL;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECommerce.Tests
{
    [TestClass]
    public class PedidoManagerTest
    {
        [TestMethod]
        public void CriarPedido_Success()
        {
            Mock<IPedidoDAL> pedidoDALMock = new Mock<IPedidoDAL>();
            pedidoDALMock
                .Setup(x => x.Create("Fulano de Tal"))
                .Returns(new Model.Pedido()
                {
                    Id = 1000,
                    Cliente = "Fulano de Tal",
                    Itens = new List<Model.ItemPedido>(),
                    Status = Model.PedidoStatus.Aberto,
                    Total = 0
                });

            //arrange
            IPedidoManager pedidoManager = new PedidoManager(pedidoDALMock.Object);
            
            //act
            var pedido = pedidoManager.CriarPedido("Fulano de Tal");

            //assert
            //Assert.AreEqual(Model.PedidoStatus.Aberto, pedido.Status);
            //Assert.AreEqual("Fulano de Tal", pedido.Cliente);
            //Assert.AreEqual(1000, pedido.Id);
            //Assert.AreEqual(0, pedido.Total);
            //Assert.AreEqual(0, pedido.Itens.Count);

            // Efetua todas as validações, mesmo que uma falhe
            using (new AssertionScope())
            {
                pedido.Id.Should().Be(1000);
                pedido.Total.Should().Be(0);
                pedido.Status.Should().Be(Model.PedidoStatus.Aberto);
                pedido.Cliente.Should().Be("Fulano de Tal");
                pedido.Itens.Should().BeEmpty();
            }
        }
    }
}
