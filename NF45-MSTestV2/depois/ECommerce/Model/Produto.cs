using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Model
{
    public class Produto : BaseModel
    {
        public Produto(string codigo, string nome, decimal precoUnitario)
        {
            Codigo = codigo;
            Nome = nome;
            PrecoUnitario = precoUnitario;
        }

        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
