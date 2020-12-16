using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using ECommerce.BLL;
using ECommerce.DAL;
using ECommerce.Model;
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
        private Mock<IProdutoDAL> produtoDALMock;
        private Mock<ILog> loggerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            this.produtoDALMock = new Mock<IProdutoDAL>();
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
            PedidoManager pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);

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
            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);

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
            IPedidoManager pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);
            
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
            pedidoDALMock
            .Setup(x => x.Create("José da Silva"))
            .Returns(new Model.Pedido()
            {
                Id = 1000,
                Cliente = "José da Silva",
                Itens = new List<Model.ItemPedido>(),
                Status = Model.PedidoStatus.Aberto,
                Total = 0
            });

            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);

            //act
            var pedido = pedidoManager.CriarPedido("José da Silva");
            Action action = () => pedidoManager.AdicionarItem(pedido, codigo, 1);

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
            pedidoDALMock
                .Setup(x => x.Create("José da Silva"))
                .Returns(new Model.Pedido()
                {
                    Id = 1000,
                    Cliente = "José da Silva",
                    Itens = new List<Model.ItemPedido>(),
                    Status = Model.PedidoStatus.Aberto,
                    Total = 0
                });
            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);

            //act
            var pedido = pedidoManager.CriarPedido("José da Silva");
            Action action = () => pedidoManager.AdicionarItem(pedido, "abc", quantidade);

            //assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                .And
                .ParamName.Should().Be("quantidade");
        }

        [TestMethod]
        public void AdicionarItem_Produto_Nao_Encontrado()
        {
            //arrange
            pedidoDALMock
                .Setup(x => x.Create("José da Silva"))
                .Returns(new Model.Pedido()
                {
                    Id = 1000,
                    Cliente = "José da Silva",
                    Itens = new List<Model.ItemPedido>(),
                    Status = Model.PedidoStatus.Aberto,
                    Total = 0
                });

            KeyNotFoundException exception = new KeyNotFoundException();
            produtoDALMock
                .Setup(x => x.Get("abc"))
                .Throws(exception)
                .Verifiable();

            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);

            //act
            var pedido = pedidoManager.CriarPedido("José da Silva");
            Action action = () => pedidoManager.AdicionarItem(pedido, "abc", 1);

            //assert
            action.Should().Throw<ProdutoNaoEncontradoException>();
            produtoDALMock.Verify();
            loggerMock.Verify(x => x.Error(exception.Message, exception));
        }

        [TestMethod]
        [DataRow(1, "abc", 1, 10)]
        [DataRow(2, "def", 1, 20)]
        [DataRow(3, "ghi", 1, 30)]
        public void AdicionarItem_Success(int produtoId, string codigo, int quantidade, double precoUnitario)
        {
            //arrange
            var p = new Model.Pedido()
              {
                  Id = 1000,
                  Cliente = "José da Silva",
                  Itens = new List<Model.ItemPedido>(),
                  Status = Model.PedidoStatus.Aberto,
                  Total = 0
              };
            var i = new Model.ItemPedido(produtoId, quantidade, (decimal)precoUnitario);

            pedidoDALMock
                .Setup(x => x.Create("José da Silva"))
                .Returns(p)
                .Verifiable();

            pedidoDALMock
                .Setup(x => x.AddItem(p, i))
                .Callback(() =>
                {
                    p.Itens.Add(i);
                })
                .Verifiable();

            KeyNotFoundException exception = new KeyNotFoundException();
            produtoDALMock
                .Setup(x => x.Get("abc"))
                .Returns(new Produto("abc", "produto abc", 10.0m) { Id = 1 });

            produtoDALMock
                .Setup(x => x.Get("def"))
                .Returns(new Produto("def", "produto def", 20.0m) { Id = 2 });

            produtoDALMock
                .Setup(x => x.Get("ghi"))
                .Returns(new Produto("ghi", "produto ghi", 30.0m) { Id = 3 });

            var pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALMock.Object, produtoDALMock.Object);

            //act
            var pedido = pedidoManager.CriarPedido("José da Silva");
            var item = pedidoManager.AdicionarItem(pedido, codigo, quantidade);

            //assert
            produtoDALMock.Verify(x => x.Get(codigo));
            pedidoDALMock.Verify();
            
            pedido.Itens
                .Should().NotBeEmpty()
                .And
                .HaveCount(1)
                .And
                .Contain(new ItemPedido(produtoId, quantidade, (decimal)precoUnitario));
        }
    }
}
