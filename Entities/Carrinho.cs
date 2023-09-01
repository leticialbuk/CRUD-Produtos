using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CRUD_Produtos.Entities
{
    public class Carrinho
    {
        public Carrinho(Produto produto)
        {
            Produtos = new List<Produto> { produto };
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
