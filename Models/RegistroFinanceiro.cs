namespace FinanceApi.Models;

public class RegistroFinanceiro
{
    public int Id { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public string? Categoria { get; set; }
    public DateTime Data { get; set; }
    public string? Tipo { get; set; } // "Receita" ou "Despesa"
}
