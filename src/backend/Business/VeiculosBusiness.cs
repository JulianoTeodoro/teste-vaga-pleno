using Parking.Api.Data.Repositories;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;
using System.Linq.Expressions;
using System.Numerics;

namespace Parking.Api.Business
{
    public class VeiculosBusiness : IVeiculosBusiness
    {
        private readonly IVeiculosEFRepository _veiculosEFRepository;
        private readonly IClienteVeiculoVigenciaEFRepository _clienteVeiculoVigenciaEF;

        public VeiculosBusiness(IVeiculosEFRepository veiculosEFRepository, IClienteVeiculoVigenciaEFRepository clienteVeiculoVigenciaEF)
        {
            _veiculosEFRepository = veiculosEFRepository;
            _clienteVeiculoVigenciaEF = clienteVeiculoVigenciaEF;
        }

        public async Task<bool> AnyAsync(Expression<Func<Veiculo, bool>>? predicate, CancellationToken ct = default)
             => await _veiculosEFRepository.AnyAsync(predicate);

        public async Task<List<Veiculo>> ListAsync(Guid? clienteId)
            => await _veiculosEFRepository.ListAsync(clienteId);

        public async Task<Veiculo?> GetById(Guid? id)
            => await _veiculosEFRepository.GetFirstOrDefaultAsync(x => x.Id == id);

        public async Task<Veiculo> Create(VeiculoCreateDto dto)
        {
            var v = new Veiculo { Placa = dto.Placa, Modelo = dto.Modelo, Ano = dto.Ano, ClienteId = dto.ClienteId };

            var vigencia = new ClienteVeiculoVigencia
            {
                ClienteId = dto.ClienteId,
                VeiculoId = v.Id,
                DtInicio = DateTime.Now,
            };

            await _veiculosEFRepository.CreateAsync(v);
            await _clienteVeiculoVigenciaEF.CreateAsync(vigencia);
            await _veiculosEFRepository.SaveChangesAsync();

            return v;
        }

        public async Task<Veiculo> Update(Guid id, VeiculoUpdateDto dto)
        {
            var v = await GetById(id);
            if (!v.ClienteId.Equals(dto.ClienteId))
            {
                v.ClienteId = dto.ClienteId;
                var vigenciaAtual = await _clienteVeiculoVigenciaEF.GetFirstOrDefaultAsync(p => p.ClienteId == v.ClienteId && p.VeiculoId == id && p.DtFim == null);

                if (vigenciaAtual != null)
                {
                    vigenciaAtual.DtFim = DateTime.Now;
                }

                await _clienteVeiculoVigenciaEF.CreateAsync(new ClienteVeiculoVigencia
                {
                    ClienteId = dto.ClienteId,
                    VeiculoId = id,
                    DtInicio = DateTime.Now,
                });
            }

            v.Placa = dto.Placa;
            v.Modelo = dto.Modelo;
            v.Ano = dto.Ano;
            await _veiculosEFRepository.Update(v);
            await _veiculosEFRepository.SaveChangesAsync();
            return v;
        }

        public async Task Delete(Guid id)
        {
            var v = await GetById(id);
            if (v != null)
            {
                await _veiculosEFRepository.Delete(v);
                await _veiculosEFRepository.SaveChangesAsync();
            }
        }
    }
}
