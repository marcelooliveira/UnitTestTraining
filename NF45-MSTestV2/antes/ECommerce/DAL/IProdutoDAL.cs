using ECommerce.Model;

namespace ECommerce.DAL
{
    public interface IProdutoDAL
    {
        Produto Get(string codigo);
    }
}
