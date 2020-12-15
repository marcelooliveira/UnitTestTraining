using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using ECommerce.BLL;
using ECommerce.DAL;
using FluentAssertions;
using FluentAssertions.Execution;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECommerce.Tests
{
    [TestClass]
    public class PedidoManagerTest
    {
        [TestMethod]
        [DataRow(1000, "Fulano de Tal")]
        [DataRow(1001, "Maria Bonita")]
        [DataRow(1002, "Zé Pequeno")]
        public void CriarPedido_Success(int pedidoId, string cliente)
        {
            Mock<IPedidoDAL> pedidoDALMock = new Mock<IPedidoDAL>();
            pedidoDALMock
                .Setup(x => x.Create(cliente))
                .Returns(new Model.Pedido()
                {
                    Id = pedidoId,
                    Cliente = cliente,
                    Itens = new List<Model.ItemPedido>(),
                    Status = Model.PedidoStatus.Aberto,
                    Total = 0
                });

            Mock<ILog> loggerMock = new Mock<ILog>();
            loggerMock
                .Setup(x => x.Info($"Pedido {pedidoId} gravado com sucesso."))
                .Verifiable();

            //arrange
            IPedidoManager pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);
            
            //act
            var pedido = pedidoManager.CriarPedido(cliente);

            //assert
            //Assert.AreEqual(Model.PedidoStatus.Aberto, pedido.Status);
            //Assert.AreEqual("Fulano de Tal", pedido.Cliente);
            //Assert.AreEqual(1000, pedido.Id);
            //Assert.AreEqual(0, pedido.Total);
            //Assert.AreEqual(0, pedido.Itens.Count);

            // Efetua todas as validações, mesmo que uma falhe
            using (new AssertionScope())
            {
                pedido.Id.Should().Be(pedidoId);
                pedido.Total.Should().Be(0);
                pedido.Status.Should().Be(Model.PedidoStatus.Aberto);
                pedido.Cliente.Should().Be(cliente);
                pedido.Itens.Should().BeEmpty();
            }

            Mock.Verify(pedidoDALMock);
            Mock.Verify(loggerMock);
        }
    }
}
