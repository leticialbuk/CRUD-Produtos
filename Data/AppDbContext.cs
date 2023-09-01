using CRUD_Produtos.Entities;
using MongoDB.Driver;

namespace CRUD_Produtos.Data
{
    public class AppDbContext
    {
        public const string STRING_DE_CONEXAO = "mongodb://localhost:27017";
        public const string NOME_DA_BASE = "Loja";
        public const string NOME_DA_COLECAO_PRODUTOS = "Produtos";
        public const string NOME_DA_COLECAO_CARRINHOS = "Carrinhos";

        private static readonly IMongoClient _client;
        public static readonly IMongoDatabase _database;

        static AppDbContext()
        {
            _client = new MongoClient(STRING_DE_CONEXAO);
            _database = _client.GetDatabase(NOME_DA_BASE);
        }

        public IMongoClient Client { get { return _client; } }  
        public IMongoCollection<Produto> Produtos { get { return _database.GetCollection<Produto>(NOME_DA_COLECAO_PRODUTOS); } }
        public IMongoCollection<Carrinho> Carrinhos { get { return _database.GetCollection<Carrinho>(NOME_DA_COLECAO_CARRINHOS); } }
    }
}
