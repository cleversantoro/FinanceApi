using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly FinanceContext _context;
    private readonly AuthService _authService;

    public AuthController(FinanceContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(Usuario usuario)
    {
        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        usuario.SenhaHash = null; // Segurança!

        return Created("", usuario);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Usuario credenciais)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == credenciais.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(credenciais.SenhaHash, usuario.SenhaHash))
            return Unauthorized("Email ou senha inválidos!");

        var token = _authService.GerarToken(usuario);

        return Ok(new { token });
    }
}
