using CRUD_Produtos.Data;
using CRUD_Produtos.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CRUD_Produtos.Controllers
{
    [ApiController]
    [Route("[vendaController]")]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VendasController() => _context = new AppDbContext();

        [HttpGet("vendas")]
        public IActionResult TotalVendas(DateTime dataInicio, DateTime dataFim)
        {
            var builer = Builders<Produto>.Filter;
            var filter = builer.Eq(x => x.Vendido, true);
            filter &= builer.Where(x => x.DataVenda >= dataInicio && x.DataVenda <= dataFim);

            var produtos = _context.Produtos.Find(filter).ToList();

            return Ok(produtos);
        }
    }
}
