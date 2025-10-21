import React, { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiGet, apiPost, apiPut, apiDelete } from '../api'

export default function VeiculosPage(){
  const qc = useQueryClient()
  const [clienteId, setClienteId] = useState('')
  const clientes = useQuery({ queryKey:['clientes-mini'], queryFn:() => apiGet('/api/clientes?pagina=1&tamanho=100') })
  const veiculos = useQuery({ queryKey:['veiculos', clienteId], queryFn:() => apiGet(`/api/veiculos${clienteId?`?clienteId=${clienteId}`:''}`) })
  const [form, setForm] = useState({ placa:'', modelo:'', ano:'', clienteId:'' })
  const [editarForm, setEditarForm] = useState({ placa:'', modelo:'', ano:'', clienteId:'' })
  const [editar, setEditar] = useState(false)

  const create = useMutation({
    mutationFn: (data) => apiPost('/api/veiculos', data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey:['veiculos'] });
      alert("Veículo criado com sucesso!");
    },
    onError: (error) => {
      alert("Erro ao criar veículo: " + error.message);
    }
  })
  const update = useMutation({
    mutationFn: ({id, data}) => apiPut(`/api/veiculos/${id}`, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey:['veiculos'] });
      alert("Veículo atualizado com sucesso!");
    },
    onError: (error) => {
      alert("Erro ao atualizar veículo: " + error.message);
    }
  })
  const remover = useMutation({
    mutationFn: (id) => apiDelete(`/api/veiculos/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey:['veiculos'] });
      alert("Veículo removido com sucesso!");
    },
    onError: (error) => {
      alert("Erro ao remover veículo: " + error.message);
    }
  })

  useEffect(()=>{
    if(clientes.data?.itens?.length && !clienteId){
      setClienteId(clientes.data.itens[0].id)
      setForm(f => ({...f, clienteId: clientes.data.itens[0].id}))
    }
  }, [clientes.data])

  return (
    <div>
      <h2>Veículos</h2>

      <div className="section">
        <div style={{display:'flex', gap:10, alignItems:'center'}}>
          <label>Cliente: </label>
          <select value={clienteId} onChange={e=>{ setClienteId(e.target.value); setForm(f=>({...f, clienteId:e.target.value}))}}>
            {clientes.data?.itens?.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
          </select>
        </div>
      </div>

      <h3>Novo veículo</h3>
      <div className="section">
        <div className="grid grid-4">
          <input placeholder="Placa" value={form.placa} onChange={e=>setForm({...form, placa:e.target.value})}/>
          <input placeholder="Modelo" value={form.modelo} onChange={e=>setForm({...form, modelo:e.target.value})}/>
          <input placeholder="Ano" value={form.ano} onChange={e=>setForm({...form, ano:e.target.value})}/>
          <button onClick={()=>create.mutate({
            placa: form.placa, modelo: form.modelo, ano: form.ano? Number(form.ano): null, clienteId: form.clienteId || clienteId
          })}>Salvar</button>
        </div>
      </div>

      <h3 style={{marginTop:16}}>Lista</h3>
      <div className="section">
        {veiculos.isLoading? <p>Carregando...</p> : (
          <table>
            <thead><tr><th>Placa</th><th>Modelo</th><th>Ano</th><th>ClienteId</th><th>Ações</th></tr></thead>
            <tbody>
              {veiculos.data?.map(v=>(
                <tr key={v.id}>
                  <td>{v.placa}</td>
                  <td>{v.modelo}</td>
                  <td>{v.ano ?? '-'}</td>
                  <td>{v.clienteId}</td>
                  <td style={{display:'flex', gap:8}}>
                    <button className="btn-ghost" onClick={()=>{ setEditarForm(v); setEditar(true);
                    }}>Editar</button>
                    <button className="btn-ghost" onClick={()=>remover.mutate(v.id)}>Excluir</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
        
        <p className="note">TODO: permitir troca de cliente na edição e garantir atualização sem recarregar a página (React Query já invalida a lista).</p>
      </div>
      {editar && (
        <div className="modal">
          <div
            className="modal-content"
            style={{
              width: 600,
              maxWidth: '95%',
              padding: 20,
              boxSizing: 'border-box',
              display: 'flex',
              flexDirection: 'column',
              gap: 12
            }}
          >
            <h3>Editar Veículo</h3>

            {/* campos empilhados um abaixo do outro */}
            <div style={{display:'flex', flexDirection:'column', gap:10}}>
              <label style={{fontSize:12}}>Placa</label>
              <input
                placeholder="Placa"
                value={editarForm.placa}
                onChange={e => setEditarForm({ ...editarForm, placa: e.target.value })}
              />

              <label style={{fontSize:12}}>Modelo</label>
              <input
                placeholder="Modelo"
                value={editarForm.modelo}
                onChange={e => setEditarForm({ ...editarForm, modelo: e.target.value })}
              />

              <label style={{fontSize:12}}>Ano</label>
              <input
                placeholder="Ano"
                value={editarForm.ano ?? ''}
                onChange={e => setEditarForm({ ...editarForm, ano: e.target.value })}
              />

              <label style={{fontSize:12}}>Cliente</label>
              <select
                value={editarForm.clienteId ?? clienteId}
                onChange={e => setEditarForm({ ...editarForm, clienteId: e.target.value })}
              >
                {clientes.data?.itens?.map(c => (
                  <option key={c.id} value={c.id}>{c.nome}</option>
                ))}
              </select>

              <div style={{display:'flex', gap:8, marginTop:6}}>
                <button
                  onClick={() => {
                    update.mutate({ id: editarForm.id, data: {
                      placa: editarForm.placa,
                      modelo: editarForm.modelo,
                      ano: editarForm.ano ? Number(editarForm.ano) : null,
                      clienteId: editarForm.clienteId ?? clienteId
                    } });
                    setEditar(false);
                  }}
                  >
                      Salvar
                      </button>
                      <button onClick={()=>setEditar(false)}>Cancelar</button>
                    </div>
                  </div>
              </div>
          </div>
          )}
      </div>
  )
}
