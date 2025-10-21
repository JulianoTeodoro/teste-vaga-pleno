using Parking.Api.Data.Repositories;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Business
{
    public class ClienteBusiness : IClienteBusiness
    {
        private readonly IClienteEFRepository _clienteEFRepository;
        public ClienteBusiness(IClienteEFRepository clienteEFRepository)
        {
            _clienteEFRepository = clienteEFRepository;
        }
        public async Task<PagedResults<Cliente>> ListAsync(int pagina, int tamanho, string? filtro, string mensalista, CancellationToken ct = default)
            => await _clienteEFRepository.ListAsync(pagina, tamanho, filtro, mensalista, ct);

        public async Task<bool> AnyAsync(Expression<Func<Cliente, bool>>? predicate, CancellationToken ct = default)
             => await _clienteEFRepository.AnyAsync(predicate);

        public async Task<Cliente> CriarCliente(ClienteCreateDto dto)
        {
            var c = new Cliente
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco,
                Mensalista = dto.Mensalista,
                ValorMensalidade = dto.ValorMensalidade,
            };

            await _clienteEFRepository.CreateAsync(c);
            await _clienteEFRepository.SaveChangesAsync();

            return c;
        }

        public async Task<Cliente?> GetById(Guid id)
            => await _clienteEFRepository.GetById(id);

        public async Task<Cliente> Update(Guid id, ClienteUpdateDto dto)
        {
            var c = await _clienteEFRepository.GetById(id);

            c.Nome = dto.Nome;
            c.Telefone = dto.Telefone;
            c.Endereco = dto.Endereco;
            c.Mensalista = dto.Mensalista;
            c.ValorMensalidade = dto.ValorMensalidade;

            await _clienteEFRepository.SaveChangesAsync();

            return c;
        }

        public async Task Delete(Guid id)
        {
            var c = await _clienteEFRepository.GetById(id);
            if (c != null)
            {
               await _clienteEFRepository.Delete(c);
               await _clienteEFRepository.SaveChangesAsync();
            }
        }


    }
}
