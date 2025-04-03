using FinanceApi.Data;
using FinanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FinanceApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FinancasController : ControllerBase
{
    private readonly FinanceContext _context;

    public FinancasController(FinanceContext context)
    {
        _context = context;
    }


    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistroFinanceiro>>> GetRegistros()
    {
        return await _context.Registros.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<RegistroFinanceiro>> PostRegistro(RegistroFinanceiro registro)
    {
        _context.Registros.Add(registro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRegistros), new { id = registro.Id }, registro);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegistro(int id)
    {
        var registro = await _context.Registros.FindAsync(id);
        if (registro == null) return NotFound();

        _context.Registros.Remove(registro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
