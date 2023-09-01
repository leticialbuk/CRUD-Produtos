using CRUD_Produtos.Entities;

namespace CRUD_Produtos.Models
{
    public class CarrinhoModel
    {
        public CarrinhoModel(List<Produto> produtos, int precoTotal)
        {
            Produtos = produtos;
            PrecoTotal = precoTotal;
        }

        public List<Produto> Produtos { get; set; }
        public int PrecoTotal { get; set; }
    }
}
