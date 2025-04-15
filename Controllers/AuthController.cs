using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceApi.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

    [Authorize(Policy = "UsuarioAutenticado")]
    [HttpGet("perfil")]
    public async Task<IActionResult> Perfil()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("ID do usuário não encontrado.");

        var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));

        if (usuario == null)
            return NotFound("Usuário não encontrado.");

        return Ok(new { usuario.Nome, usuario.Email });
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return BadRequest("Email e senha são obrigatórios.");

        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return Conflict("Email já está em uso.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        usuario.SenhaHash = null; // Segurança!

        return Created("", usuario);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return BadRequest("Email e senha são obrigatórios.");

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            return Unauthorized("Email ou senha inválidos!");

        var token = _authService.GerarToken(usuario);

        return Ok(new { token });
    }

}

