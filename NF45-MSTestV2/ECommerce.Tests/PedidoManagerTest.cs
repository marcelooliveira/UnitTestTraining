using ECommerce.BLL;
using ECommerce.DAL;
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


            Mock<ILog> loggerMock = new Mock<ILog>();
            loggerMock
                .Setup(x => x.Info($"Pedido {pedidoId} gravado com sucesso."))
                //.Setup(x => x.Info(It.IsAny<string>()))
                .Verifiable();

            Mock<IPedidoDAL> pedidoDALmock = new Mock<IPedidoDAL>();
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

            IPedidoManager pedidoManager = new PedidoManager(loggerMock.Object, pedidoDALmock.Object);

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
            Mock<IPedidoDAL> pedidoDALMock = new Mock<IPedidoDAL>();
            pedidoDALMock
                .Setup(x => x.Create(It.IsAny<string>()))
                .Throws(new ApplicationException("Erro ao criar pedido no banco de dados."))
                .Verifiable();

            Mock<ILog> loggerMock = new Mock<ILog>();
            loggerMock
                .Setup(x => x.Error("Erro ao criar pedido no banco de dados."))
                .Verifiable();

            //Act
            PedidoManager pedidoManager
                = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);
            
            Action action = ()
                => pedidoManager.CriarPedido("Fulano de Tal");

            //Assert
            action.Should().Throw<ApplicationException>()
                .And
                .Message.Should().Be("Erro ao criar pedido no banco de dados.");

            Mock.VerifyAll();
        }


        [TestMethod]
        public void CriarPedido_Cliente_Nulo()
        {
            //Arrange
            Mock<IPedidoDAL> pedidoDALMock = new Mock<IPedidoDAL>();

            Mock<ILog> loggerMock = new Mock<ILog>();

            //Act
            PedidoManager pedidoManager
                = new PedidoManager(loggerMock.Object, pedidoDALMock.Object);

            Action action = ()
                => pedidoManager.CriarPedido(null);

            //Assert
            action.Should().Throw<ArgumentNullException>()
                .And
                .ParamName.Should().Be("cliente");
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
//AdicionarItem_Success()
//AdicionarItem_Pedido_Status_Invalido()


