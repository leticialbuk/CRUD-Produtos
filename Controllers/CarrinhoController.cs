using CRUD_Produtos.Data;
using CRUD_Produtos.Entities;
using CRUD_Produtos.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CRUD_Produtos.Controllers
{
    [ApiController]
    [Route("[carrinhoController]")]
    public class CarrinhoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CarrinhoController() => _context = new AppDbContext();

        [HttpPost("carrinho")]
        public IActionResult AdicionarCarrinho(string? idCarrinho, string idProduto)
        {
            var produto = _context.Produtos.Find(x => x.Id == idProduto && x.Vendido == false).FirstOrDefault();
            if (produto == null)
                return BadRequest();

            var carrinho = _context.Carrinhos.Find(x => x.Id == idCarrinho).FirstOrDefault();
            if (carrinho == null)
            {
                carrinho = new Carrinho(produto);
                _context.Carrinhos.InsertOne(carrinho);
            }
            else
            {
                carrinho.Produtos.Add(produto);
                _context.Carrinhos.ReplaceOne(x => x.Id == idCarrinho, carrinho);
            }

            return Ok(carrinho);
        }

        [HttpGet("carrinho")]
        public IActionResult ObterCarrinho(string idCarrinho)
        {
            var carrinho = _context.Carrinhos.Find(x => x.Id == idCarrinho).FirstOrDefault();
            if (carrinho == null)
                return BadRequest("Ops! Parece que seu carrinho está vazio");

            var precoTotal = 0;
            foreach (var produto in carrinho.Produtos)
                precoTotal += produto.Preco;

            var carrinhoModel = new CarrinhoModel(carrinho.Produtos, precoTotal);

            return Ok(carrinhoModel);
        }

        [HttpPost("compra")]
        public IActionResult ConcluirCompra(string idCarrinho)
        {
            var carrinho = _context.Carrinhos.Find(x => x.Id == idCarrinho).FirstOrDefault();
            if (carrinho == null)
                return BadRequest();

            foreach (var produto in carrinho.Produtos)
            {
                produto.Vendido = true;
                produto.DataVenda = DateTime.Now.Date;
                _context.Produtos.ReplaceOne(x => x.Id == produto.Id, produto);
            }

            return Ok("Compra realizada com sucesso");
        }
    }
}
