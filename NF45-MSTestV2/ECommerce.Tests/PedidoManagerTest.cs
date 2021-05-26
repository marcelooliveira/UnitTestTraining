using ECommerce.BLL;
using ECommerce.DAL;
using ECommerce.Model;
using FluentAssertions;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace ECommerce.Tests
{
    /// fusca
    /// <image url="https://cdn.pixabay.com/photo/2013/07/13/10/21/volkswagen-157030__340.png" scale="0.5"/>

    /// motor
    //// <image url = "https://cdn.pixabay.com/photo/2013/07/13/12/44/engine-160230__340.png" scale="0.5" />

    /// caixa de câmbio    
    //// <image url="http://www.ajautomatics.co.uk/images/content-images/aj-automatics2.jpg" scale="0.2"/>

    [TestClass]
    public class PedidoManagerTest
    {
        Mock<ILog> loggerMock;
        Mock<IPedidoDAL> pedidoDALmock;
        Mock<IProdutoDAL> produtoDALmock;

        [TestInitialize]
        public void TestInitialize()
        {
            loggerMock = new Mock<ILog>();
            pedidoDALmock = new Mock<IPedidoDAL>();
            produtoDALmock = new Mock<IProdutoDAL>();
        }

        [TestMethod]
        [DataRow(1000, "Fulano de Tal")]
        [DataRow(1001, "Maria Bonita")]
        [DataRow(1002, "Zé Pequeno")]
        public void CriarPedido_Success(int pedidoId, string cliente)
        {
            //Red -> Green -> Refactor
            ///3 As
            //arrange - preparação

            //Mock = boneco/imitação/fantoche

            loggerMock
                .Setup(x => x.Info($"Pedido {pedidoId} gravado com sucesso."))
                //.Setup(x => x.Info(It.IsAny<string>()))
                .Verifiable();

            pedidoDALmock
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(new Model.Pedido()
                {
                    Id = pedidoId,
                    Total = 0,
                    Cliente = cliente,
                    Status = Model.PedidoStatus.Aberto,
                    Itens = new List<Model.ItemPedido>()
                })
                .Verifiable();

            IPedidoManager pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            //act
            var pedido = pedidoManager.CriarPedido("Fulano de Tal");

            //assert
            Assert.AreEqual(cliente, pedido.Cliente);
            Assert.AreEqual(Model.PedidoStatus.Aberto, pedido.Status);
            Assert.AreEqual(0, pedido.Itens.Count);

            pedido.Id.Should().Be(pedidoId);
            pedido.Total.Should().Be(0);
            pedido.Cliente.Should().Be(cliente);
            pedido.Status.Should().Be(Model.PedidoStatus.Aberto);
            pedido.Itens.Should().BeEmpty();

            Mock.Verify(loggerMock);
            Mock.Verify(pedidoDALmock);
            //Mock.VerifyAll();
        }

        [TestMethod]
        //[ExpectedException(typeof(ApplicationException))]
        public void CriarPedido_Erro_BancoDeDados()
        {
            //Arrange
            pedidoDALmock
                .Setup(x => x.Create(It.IsAny<string>()))
                .Throws(new ApplicationException("Erro ao criar pedido no banco de dados."))
                .Verifiable();

            loggerMock
                .Setup(x => x.Error("Erro ao criar pedido no banco de dados."))
                .Verifiable();

            //Act
            PedidoManager pedidoManager
                = new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);
            
            Action action = ()
                => pedidoManager.CriarPedido("Fulano de Tal");

            //Assert
            action.Should().Throw<ApplicationException>()
                .And
                .Message.Should().Be("Erro ao criar pedido no banco de dados.");

            Mock.Verify(pedidoDALmock);
            Mock.Verify(loggerMock);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("   ")]
        public void CriarPedido_Cliente_Nulo(string nomeCliente)
        {
            //Arrange
            
            //Act
            PedidoManager pedidoManager
                = new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            Action action = ()
                => pedidoManager.CriarPedido(nomeCliente);

            //Assert
            action.Should().Throw<ArgumentNullException>()
                .And
                .ParamName.Should().Be("cliente");
        }

        [TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        public void AdicionarItem_Pedido_Nulo()
        {
            //Arrange

            //Act
            PedidoManager pedidoManager =
                new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            Action action = () =>
                pedidoManager.AdicionarItem(null, "abc", 3);

            //Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .And
                .ParamName.Should().Be("pedido");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("    ")]
        public void AdicionarItem_Codigo_Invalido(string codigo)
        {
            //Arrange

            //Act
            PedidoManager pedidoManager =
                new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            var pedido = new Pedido();

            Action action = () =>
                pedidoManager.AdicionarItem(pedido, codigo, 3);

            //Assert
            action
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("codigo");
        }

        [TestMethod]
        [DataRow(-100)]
        [DataRow(-7)]
        [DataRow(-1)]
        [DataRow(0)]
        public void AdicionarItem_Quantidade_Invalida(int quantidade)
        {
            //Arrange

            //Act
            PedidoManager pedidoManager =
                new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            var pedido = new Pedido();

            Action action = () =>
                pedidoManager.AdicionarItem(pedido, "abc", quantidade);

            //Assert
            action
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .And
                .ParamName.Should().Be("quantidade");
        }

        [TestMethod]
        public void AdicionarItem_Produto_Nao_Encontrado()
        {
            //Arrange
            produtoDALmock
                .Setup(x => x.Get("abc"))
                .Throws(new KeyNotFoundException())
                .Verifiable();

            //Act
            PedidoManager pedidoManager =
                new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            var pedido = new Pedido();
            pedido.Status = PedidoStatus.Aberto;

            Action action = () =>
                pedidoManager.AdicionarItem(pedido, "abc", 123);

            //Assert
            action
                .Should()
                .Throw<ProdutoNaoEncontradoException>();

        }

        [TestMethod]
        [DataRow(PedidoStatus.Fechado)]
        [DataRow(PedidoStatus.Nenhum)]
        public void AdicionarItem_Pedido_Status_Invalido(PedidoStatus pedidoStatus)
        {
            //Arrange
            produtoDALmock
                .Setup(x => x.Get("abc"))
                .Returns(new Produto("abc", "Produto ABC", 10.59m))
                .Verifiable();

            //Act
            PedidoManager pedidoManager =
                new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            var pedido = new Pedido();
            pedido.Status = pedidoStatus;

            Action action = () =>
                pedidoManager.AdicionarItem(pedido, "abc", 123);

            //Assert
            action
                .Should()
                .Throw<StatusInvalidoException>();
        }

        [TestMethod]
        public void AdicionarItem_Success()
        {
            //Arrange
            produtoDALmock
                .Setup(x => x.Get("abc"))
                .Returns(new Produto("abc", "Produto ABC", 10.59m))
                .Verifiable();

            //Act
            PedidoManager pedidoManager =
                new PedidoManager(loggerMock.Object, pedidoDALmock.Object, produtoDALmock.Object);

            var pedido = new Pedido();
            pedido.Status = PedidoStatus.Aberto;

            pedidoManager.AdicionarItem(pedido, "abc", 123);

            //Assert
            pedido.Itens.Should().HaveCount(1);
        }
    }
}

//Verify() com loggerMock & pedidoDALMock
//try-catch em PedidoManager com throw e log.Error
//CriarPedido_Cliente_Nulo com DataRow e valores inválidos
//TestInitialize() com loggerMock & pedidoDALMock
//AdicionarItem_Pedido_Nulo()
//AdicionarItem_Codigo_Invalido()
//AdicionarItem_Quantidade_Invalida()
//ProdutoDALMock
//AdicionarItem_Produto_Nao_Encontrado()
//AdicionarItem_Pedido_Status_Invalido()
//AdicionarItem_Success()


