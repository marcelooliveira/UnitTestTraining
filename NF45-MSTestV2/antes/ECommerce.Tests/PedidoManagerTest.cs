using FluentAssertions;
using System;
using ECommerce.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECommerce.DAL;
using Moq;
using ECommerce.Model;

namespace ECommerce.Tests
{
    /// fusca
    /// <image url="https://cdn.pixabay.com/photo/2013/07/13/10/21/volkswagen-157030__340.png" scale="0.5"/>
    
    /// motor
    /// <image url = "https://cdn.pixabay.com/photo/2013/07/13/12/44/engine-160230__340.png" scale="0.5" />
    
    /// caixa de câmbio    
    /// <image url="http://www.ajautomatics.co.uk/images/content-images/aj-automatics2.jpg" scale="0.2"/>
    [TestClass]
    public class PedidoManagerTest
    {
        private Mock<IPedidoDAL> mockPedidoDAL;
        private Mock<IProdutoDAL> mockProdutoDAL;
        private PedidoManager pedidoManager;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockPedidoDAL = new Mock<IPedidoDAL>();
            this.mockProdutoDAL = new Mock<IProdutoDAL>();
            this.pedidoManager = new PedidoManager(mockPedidoDAL.Object, mockProdutoDAL.Object);
        }

        [TestMethod]
        public void Ctor_When_PedidoDAL_Is_Null_Should_Throw_Exception()
        {
            //arrange

            //act
            Action action = () => pedidoManager = new PedidoManager(null, mockProdutoDAL.Object);

            //assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("pedidoDAL");
        }

        [TestMethod]
        public void Ctor_When_ProdutoDAL_Is_Null_Should_Throw_Exception()
        {
            //arrange

            //act
            Action action = () => pedidoManager = new PedidoManager(mockPedidoDAL.Object, null);

            //assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("produtoDAL");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("         ")]
        public void CriarPedido_When_Cliente_Null_Should_Throw_Exception(string cliente)
        {
            //arrange

            //act
            Action action = () => pedidoManager.CriarPedido(cliente);

            //assert
            //Assert.ThrowsException<ArgumentNullException>(action);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("cliente");
        }

        [TestMethod]
        public void CriarPedido_When_Everything_Is_Ok_Should_Succeed()
        {
            //arrange
            mockPedidoDAL.Setup(x => x.Create(It.IsAny<string>()))
                .Returns(new Model.Pedido() { Cliente = "Zé da Silva" })
                .Verifiable();

            //act
            const string NomeCliente = "Zé da Silva";
            var pedido = pedidoManager.CriarPedido(NomeCliente);

            //assert
            pedido.Should().NotBeNull();
            pedido.Cliente.Should().Be(NomeCliente);
            pedido.Itens.Should().NotBeNull();

            mockPedidoDAL.Verify();
        }

        [TestMethod]
        public void AdicionarItem_When_Pedido_Is_Null_Should_Throw_Exception()
        {
            //arrange

            //act
            Action action = () => pedidoManager.AdicionarItem(null, "123", 7);

            //assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("pedido");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("       ")]
        public void AdicionarItem_When_Codigo_Is_Empty_Should_Throw_Exception(string codigo)
        {
            //arrange

            //act
            var pedido = new Pedido();
            Action action = () => pedidoManager.AdicionarItem(pedido, codigo, 7);

            //assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("codigo");
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(-100)]
        [DataRow(0)]
        public void AdicionarItem_When_Quantidade_Is_Invalid_Should_Throw_Exception(int quantidade)
        {
            //arrange

            //act
            var pedido = new Pedido();
            Action action = () => pedidoManager.AdicionarItem(pedido, "123", quantidade);

            //assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("quantidade");
        }

        [TestMethod]
        [DataRow(PedidoStatus.Fechado)]
        [DataRow(PedidoStatus.Nenhum)]
        public void AdicionarItem_When_Pedido_Status_Invalid_Should_Throw_Exception(PedidoStatus pedidoStatus)
        {
            //arrange

            //act
            var pedido = new Pedido();
            pedido.Status = pedidoStatus;
            Action action = () => pedidoManager.AdicionarItem(pedido, "123", 3);

            //assert
            action.Should().Throw<StatusInvalidoException>();
        }

        [TestMethod]
        public void AdicionarItem_When_Produto_NotFound_Should_Throw_Exception()
        {
            //arrange
            mockProdutoDAL.Setup(x => x.Get(It.IsAny<string>()))
                .Returns((Model.Produto)null)
                .Verifiable();

            //act
            var pedido = new Pedido();
            pedido.Status = PedidoStatus.Aberto;
            Action action = () => pedidoManager.AdicionarItem(pedido, "123", 3);

            //assert
            action.Should().Throw<ProdutoNaoEncontradoException>();

            mockProdutoDAL.Verify();
        }

        [TestMethod]
        public void AdicionarItem_When_Success()
        {
            //arrange
            mockProdutoDAL.Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new Produto("123", "Produto 123", 1.23m) { Id = 123 })
                .Verifiable();

            //act
            var pedido = new Pedido();
            pedido.Status = PedidoStatus.Aberto;
            var item = pedidoManager.AdicionarItem(pedido, "123", 3);

            //assert
            item.ProdutoId.Should().Be(123);
            item.PrecoUnitario.Should().Be(1.23m);
            item.Quantidade.Should().Be(3);
            item.Subtotal.Should().Be(3.69m);

            pedido.Itens.Should().HaveCount(1);
            pedido.Itens.Should().Contain(item);

            mockProdutoDAL.Verify();
        }
    }
}







//CriarPedido_Quando_Sucesso
//act/arrange/assert
//Assert.AreEqual
//FluentAssertions
//Should
//DataRow
//mocks
//verify
//CriarPedido_Quando_Erro_BancoDeDados
//ExpectedException
//CriarPedido_Quando_Cliente_Nao_Informado
//action.Should().Throw
//TestInitialize
//AdicionarItem_Quando_Pedido_Nulo
//AdicionarItem_Quando_Pedido_Status_Invalido
//AdicionarItem_Quando_Codigo_Invalido
//AdicionarItem_Quando_Quantidade_Invalida
//AdicionarItem_Quando_Produto_Nao_Encontrado
//AdicionarItem_Quando_Sucesso


