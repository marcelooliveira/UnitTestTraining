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
        private Mock<IPedidoDAL> pedidoDALMock;
        private Mock<ILog> loggerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            this.pedidoDALMock = new Mock<IPedidoDAL>();
            this.loggerMock = new Mock<ILog>();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CriarPedido_Erro_BancoDeDados()
        {
            //arrange
            pedidoDALMock
                .Setup(x => x.Create(It.IsAny<string>()))
                .Throws(new ApplicationException("Erro ao criar pedido no banco de dados."))
                .Verifiable();

            loggerMock
                .Setup(x => x.Error("Erro ao criar pedido no banco de dados."))
                .Verifiable();

            //act
            PedidoManager pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);

            //assert
            var pedido = pedidoManager.CriarPedido("Fulano de Tal");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("   ")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CriarPedido_Cliente_Nao_Informado(string cliente)
        {
            //arrange
            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);

            //act
            pedidoManager.CriarPedido(cliente);
        }

        [TestMethod]
        [DataRow(1000, "Fulano de Tal")]
        [DataRow(1001, "Maria Bonita")]
        [DataRow(1002, "Zé Pequeno")]
        public void CriarPedido_Success(int pedidoId, string cliente)
        {
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
            loggerMock.Verify(x => x.Info($"Pedido {pedidoId} gravado com sucesso."), Times.Once);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")]
        public void AdicionarItem_Codigo_Invalido(string codigo)
        {
            //arrange
            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);

            //act
            Action action = () => pedidoManager.AdicionarItem(codigo, 1);

            //assert
            action.Should().Throw<ArgumentNullException>()
                .And
                .ParamName.Should().Be("codigo");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void AdicionarItem_Quantidade_Invalida(int quantidade)
        {
            //arrange
            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);

            //act
            Action action = () => pedidoManager.AdicionarItem("abc", quantidade);

            //assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                .And
                .ParamName.Should().Be("quantidade");
        }
    }
}
