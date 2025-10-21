import React, { useState } from 'react'

export default function CsvUploadPage(){
  const [log, setLog] = useState(null)
    const fileToBase64 = (f) => new Promise((resolve, reject) => {
      const reader = new FileReader()
      reader.onload = () => resolve(reader.result.split(',')[1]) // remove "data:*/*;base64," prefix
      reader.onerror = () => reject(new Error('Falha ao ler arquivo'))
      reader.readAsDataURL(f)
    })


  async function handleUpload(e){
    e.preventDefault()
    const file = e.target.file.files[0]
    if (!file) return

    const base64 = {base64 : await fileToBase64(file)}

    const r = await fetch((import.meta.env.VITE_API_URL || 'https://localhost:57009') + '/api/import/csv', {
      method: 'POST',
      body: JSON.stringify(base64),
      headers: { 'Content-Type': 'application/json' }
    })
    const j = await r.json()
    setLog(j)
  }

  return (
    <div>
      <h2>Importar CSV</h2>
      <div className="section">
        <form onSubmit={handleUpload} style={{display:'flex', gap:10, alignItems:'center'}}>
          <input type="file" name="file" accept=".csv" />
          <button type="submit">Enviar</button>
        </form>
      </div>

      <h3 style={{marginTop:16}}>Relatório</h3>
      <div className="section">
        {log ? (
          <div>
            <div style={{display:'flex', gap:12, marginBottom:8}}>
              <strong>Processados:</strong> <span>{log.qtProcessados}</span>
              <strong>Inseridos:</strong> <span>{log.qtInseridos}</span>
              <strong>Erros:</strong> <span>{log.qtErros}</span>
            </div>

            {Array.isArray(log.erros) && log.erros.length > 0 ? (
              <div style={{display:'flex', flexDirection:'column', gap:8}}>
                {log.erros.map((err, i) => {
                  return (
                    <div key={i} style={{padding:12, background:'#0b0c0e', color:'#c7d2fe', borderRadius:8}}>
                      <div style={{display:'flex', gap:8, alignItems:'baseline'}}>
                        <strong style={{minWidth:70}}>Linha {err.linha}</strong>
                        <span>{err.erro}</span>
                      </div>
                    </div>
                  )
                })}
              </div>
            ) : (
              <pre style={{background:'#0b0c0e', color:'#c7d2fe', padding:12, margin:0, borderRadius:10}}>
                {JSON.stringify(log, null, 2)}
              </pre>
            )}
          </div>
        ) : (
          <pre style={{background:'#0b0c0e', color:'#c7d2fe', padding:12, margin:0, borderRadius:10}}>
            Aguardando upload...
          </pre>
        )}
        <p className="note">Tarefa: melhorar o relatório de erros (linhas e motivos mais claros; opcional transação por lote).</p>
      </div>
    </div>
  )
}
