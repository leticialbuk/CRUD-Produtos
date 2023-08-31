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

        [HttpGet("byid")]
        public IActionResult Carrinho(string id)
        {
            var produto = _context.Produtos.Find(x => x.Id == id).FirstOrDefault();
            if (produto == null) return BadRequest();

            produto.Vendido = true;
            produto.DataVenda = DateTime.Now.Date;
            _context.Produtos.ReplaceOne(x => x.Id == id, produto);
            
            return Ok(produto); 
        }

        [HttpGet("Vendas")]
        public IActionResult TotalVendas(DateTime dataInicio, DateTime dataFim) 
        {
            var builer = Builders<Produto>.Filter;
            var filter = builer.Eq(x => x.Vendido, true);
            filter &= builer.Where(x => x.DataVenda >= dataInicio && x.DataVenda <= dataFim);

            var produtos = _context.Produtos.Find(filter).ToList();
            var totalVendas = produtos.Count();

            var totalPreco = 0;
            foreach (var produto in produtos) 
                totalPreco += produto.Preco;

            var vendaModel = new VendaModel(totalPreco);

            return Ok(vendaModel);
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
