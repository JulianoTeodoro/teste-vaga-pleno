import React, { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiGet, apiPost, apiDelete, apiPut } from '../api'

export default function ClientesPage(){
  const qc = useQueryClient()
  const [filtro, setFiltro] = useState('')
  const [editar, setEditar] = useState(false)
  const [mensalista, setMensalista] = useState('all')
  const [form, setForm] = useState({ nome:'', telefone:'', endereco:'', mensalista:false, valorMensalidade:'' })
  const [formEditar, setFormEditar] = useState({ id:'', nome:'', telefone:'', endereco:'', mensalista:false, valorMensalidade:0 })

  function handleChange(e) {
    const { name, type, value, checked } = e.target;
    let newValue = value;

    if (type === 'checkbox') {
      newValue = checked;
    } else if (type === 'number') {
      newValue = value === '' ? '' : Number(value);
    } else if (e.target.tagName === 'SELECT' || type === 'select-one') {
      if (value === 'true') newValue = true;
      else if (value === 'false') newValue = false;
      else newValue = value;
    }

    setFormEditar(prev => ({ ...prev, [name]: newValue }));
  }

  const editarItem = (c) => {
    setEditar(true)
    setFormEditar({
      id: c.id,
      nome: c.nome ?? '',
      telefone: c.telefone ?? '',
      endereco: c.endereco ?? '',
      mensalista: c.mensalista,
      valorMensalidade: c.valorMensalidade ?? ''
    })

  }

  function handleSubmit(e) {
    e.preventDefault();
    edit.mutate({
      ...formEditar
    });
  }

  const q = useQuery({
    queryKey:['clientes', filtro, mensalista],
    queryFn:() => apiGet(`/api/clientes?pagina=1&tamanho=20&filtro=${encodeURIComponent(filtro)}&mensalista=${mensalista}`)
  })

  const create = useMutation({
    mutationFn: (data) => apiPost('/api/clientes', data),
    onSuccess: () => qc.invalidateQueries({ queryKey:['clientes'] }),
  })

  const edit = useMutation({
    mutationFn: (data) => apiPut(`/api/clientes/${formEditar.id}`, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey:['clientes'] });
      alert("Cliente atualizado com sucesso!");
      setEditar(false);
    },
    onError: (error) => {
      alert("Erro ao atualizar o cliente: " + error.message);
      setEditar(false);
    }
  })

  const remover = useMutation({
    mutationFn: (id) => apiDelete(`/api/clientes/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey:['clientes'] })
  })

  const edicaoComponent = (
    <div className="max-w-md mx-auto p-6 bg-white rounded-2xl shadow">
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div>
                    <input
                      type="text"
                      placeholder="Nome"
                      name="nome"
                      value={formEditar.nome}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div>
                    <input
                      type="text"
                      name="endereco"
                      placeholder="Endereco"
                      value={formEditar.endereco}
                      onChange={handleChange}
                    />
                  </div>

                  <div>
                    <input
                      type="tel"
                      name="telefone"
                      placeholder="Telefone"
                      value={formEditar.telefone}
                      onChange={handleChange}
                      required
                    />
                  </div>

                  <div>
                    <select name="mensalista" value={formEditar.mensalista} onChange={handleChange} placeholder="Mensalista">
                      <option value="true">Sim</option>
                      <option value="false">Não</option>
                    </select>
                  </div>

                  <div>
                    <input
                    name="valorMensalidade"
                      type="number"
                      placeholder="Valor da Mensalidade"
                      value={Number(formEditar.valorMensalidade) ?? 0}
                      min="0"
                      step="0.01"
                      onChange={handleChange}
                    />
                  </div>

                  <button
                    type="submit"
                    className="botao-editar"
                  >
                    Enviar
                  </button>
                </form>
              </div>
  )

  return (
    <div>
      <h2>Clientes</h2>

      <div className="section">
        <div className="grid grid-3">
          <input placeholder="Buscar por nome" value={filtro} onChange={e=>setFiltro(e.target.value)} />
          <select value={mensalista} onChange={e=>setMensalista(e.target.value)}>
            <option value="all">Todos</option>
            <option value="true">Mensalistas</option>
            <option value="false">Não mensalistas</option>
          </select>
          <div/>
        </div>
      </div>

      <h3>Novo cliente</h3>
      <div className="section">
        <div className="grid grid-4">
          <input placeholder="Nome" value={form.nome} onChange={e=>setForm({...form, nome:e.target.value})}/>
          <input placeholder="Telefone" value={form.telefone} onChange={e=>setForm({...form, telefone:e.target.value})}/>
          <input placeholder="Endereço" value={form.endereco} onChange={e=>setForm({...form, endereco:e.target.value})}/>
          <label style={{display:'flex', alignItems:'center', gap:8}}>
            <input type="checkbox" checked={form.mensalista} onChange={e=>setForm({...form, mensalista:e.target.checked})}/> Mensalista
          </label>
          <input placeholder="Valor mensalidade" value={form.valorMensalidade} onChange={e=>setForm({...form, valorMensalidade:e.target.value})}/>
          <div/>
          <div/>
          <button onClick={()=>create.mutate({
            nome:form.nome, telefone:form.telefone, endereco:form.endereco,
            mensalista:form.mensalista, valorMensalidade:form.valorMensalidade? Number(form.valorMensalidade): null
          })}>Salvar</button>
        </div>
      </div>

      <h3 style={{marginTop:16}}>Lista</h3>
      <div className="section">
        {q.isLoading? <p>Carregando...</p> : (
          <table>
            <thead><tr><th>Nome</th><th>Telefone</th><th>Mensalista</th><th></th></tr></thead>
            <tbody>
              {q.data.itens.map(c=>(
                <tr key={c.id}>
                  <td>{c.nome}</td>
                  <td>{c.telefone}</td>
                  <td>{c.mensalista? 'Sim':'Não'}</td>
                  <td>
                    <button className="btn-ghost" onClick={()=>editarItem(c)}>Editar</button>
                  </td>
                  <td>
                    <button className="btn-ghost" onClick={()=>remover.mutate(c.id)}>Excluir</button>
                  </td>

                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
      {editar && <h3>Editar Cliente</h3>}
      <div className="section">
        {editar && 
              edicaoComponent

        }
      </div>
    </div>
  )
}
