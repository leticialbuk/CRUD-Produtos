using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CRUD_Produtos.Entities
{
    public class Produto
    {
        public Produto(string nomeProduto, string descricao, int preco)
        {
            NomeProduto = nomeProduto;
            Descricao = descricao;
            Preco = preco;
            Vendido = false;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string NomeProduto { get; set; }
        public string Descricao { get; set; }
        public int Preco { get; set; }
        public bool Vendido{ get; set; }
        public DateTime? DataVenda { get; set; }

    }
}
