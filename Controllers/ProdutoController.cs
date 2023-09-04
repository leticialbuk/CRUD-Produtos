using CRUD_Produtos.Data;
using CRUD_Produtos.Entities;
using CRUD_Produtos.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Globalization;

namespace CRUD_Produtos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdutoController() => _context = new AppDbContext();

        [HttpPost]
        public IActionResult AdicionarProduto([FromBody] ProdutoModel model)
        {
            var produto = new Produto(model.NomeProduto, model.Descricao, model.Preco);
            _context.Produtos.InsertOne(produto);

            return Ok("Produto registrado!");
        }

        [HttpGet]
        public IActionResult ProdutosDisponiveis(string? nomeProduto, int? precoMenorQue, int? precoMaiorQue, bool ordernarPrecoMenorQue, bool ordernarPrecoMaiorQue, int? skip, int? take)
        {
            var builder = Builders<Produto>.Filter;
            var filter = builder.Eq(x => x.Vendido, false);

            if (nomeProduto != null)
                filter &= builder.Eq(x => x.NomeProduto, nomeProduto);

            if (precoMenorQue != null)
                filter &= builder.Where(x => x.Preco < precoMenorQue);

            if (precoMaiorQue != null)
                filter &= builder.Where(x => x.Preco > precoMaiorQue);

            skip = skip == null ? 0 : skip;
            take = take == null ? 10 : take;

            var query = _context.Produtos.Find(filter);
            if (ordernarPrecoMenorQue == true)
                query = query.SortBy(x => x.Preco);

            if (ordernarPrecoMaiorQue == true)
                query = query.SortByDescending(x => x.Preco);

            var listaProdutos = query.Skip(skip).Limit(take).ToList();
            return Ok(listaProdutos);
        }

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
                return BadRequest();

            var precoTotal = 0;
            foreach(var produto in carrinho.Produtos)
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

        [HttpGet("vendas")]
        public IActionResult TotalVendas(DateTime dataInicio, DateTime dataFim) 
        {
            var builer = Builders<Produto>.Filter;
            var filter = builer.Eq(x => x.Vendido, true);
            filter &= builer.Where(x => x.DataVenda >= dataInicio && x.DataVenda <= dataFim);

            var produtos = _context.Produtos.Find(filter).ToList();
           
            return Ok(produtos);
        }

        [HttpDelete]
        public IActionResult ExcluirProduto(string id) 
        {
            var produto = _context.Produtos.Find(x => x.Id == id).FirstOrDefault();
            _context.Produtos.DeleteOne(x => x.Id == id);

            return Ok("Produto removido com sucesso!");
        }
    }
}
