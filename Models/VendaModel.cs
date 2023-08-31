namespace CRUD_Produtos.Models
{
    public class VendaModel
    {
        public VendaModel(int totalPreco) 
        {
            TotalPreco = totalPreco;
        }

        public int TotalPreco { get; set; }
    }
}
