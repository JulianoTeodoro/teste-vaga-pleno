## Documentação técnica das atividades solicitadas no teste.


As funcionalidades foram em .NET 8 no back-end e React com React Query no front-end para controle de estado assíncrono, garantindo sincronização com o backend e experiência fluida ao usuário.

### Tarefa 1 : Completar a Tela de Cliente

Contexto: Anteriormente, o módulo Clientes do sistema permitia apenas o cadastro, listagem e exclusão de clientes.
Com esta entrega, foi implementada a seção de edição de clientes, permitindo ao usuário atualizar as informações de um cliente já existente, com interação direta com o endpoint PUT /api/clientes/{id} da API.

#### Back-End

🚀 Endpoint
PUT /api/clientes/{id}

Atualiza as informações de um cliente existente.

```
{
  "nome": "João da Silva",
  "telefone": "11999998888",
  "endereco": "Rua das Flores, 123",
  "mensalista": true,
  "valorMensalidade": 250.00
}
```
🔹 Respostas possíveis

| Codigo | Tipo | Descrição |
|:-----------:|:------------|------------:|
| 200    | OK    | Cliente atualizado com sucesso    |
| 404  | Not Found | Cliente não cadastrado!!  |
| 409  | Conflict  | Cliente já existente.  |
| 400  | BadRequest  | Mensagem genérica para erros de validação  |

````
{
  "id": "8df5a7c4-90f3-4d7e-b4b3-15a9a2b7990e",
  "nome": "João da Silva",
  "telefone": "11999998888",
  "endereco": "Rua das Flores, 123",
  "mensalista": true,
  "valorMensalidade": 250.0
}
````

Decisões técnicas :
````
Segregação de responsabilidades por camadas (Controller, Business, Repository): Foi adotada uma arquitetura em camadas, seguindo o padrão Clean Architecture / DDD simplificado, evitando duplicação de lógica e mantém o código mais fácil de testar e evoluir.

Utilização de DTOs: Foi criada a classe ClienteUpdateDto para receber apenas os campos editáveis (Nome, Telefone, Endereço, Mensalista, ValorMensalidade).
Essa decisão impede exposição direta da entidade Cliente e reduz riscos de atualizações indevidas em propriedades sensíveis.

Validação de existência e unicidade: A validação de unicidade (Nome + Telefone) foi movida para a camada de negócio antes da atualização, utilizando o método AnyAsync.

Padronização de respostas HTTP : Foram utilizadas respostas HTTP adequadas para cada cenário, seguindo as boas práticas REST e melhorando a comunicação entre front-end e back-end.

````

#### Front-End

O código foi implementado dentro do componente principal ClientesPage.jsx.

##### Principais responsabilidades:

Incluir nova seção de edição;
Controlar estado de filtros, formulários e ações (editar/excluir).

Decisões técnicas: 

````
Separação de estado entre criação e edição

Foram criados dois objetos de estado distintos:
form (para novo cliente) e formEditar (para edição).

Essa decisão evita conflito entre os dois formulários e garante que os dados de edição não interfiram no formulário de criação.

Controle condicional de exibição da seção de edição: A variável booleana editar foi adicionada para exibir ou ocultar o formulário de edição.
Mantém a tela limpa e evita múltiplas edições simultâneas.

Reutilização de lógica de formulário com handleChange: A função handleChange foi centralizada para lidar com diferentes tipos de input (text, checkbox, number, select).
Reduz duplicação de código e melhora manutenção futura.

Integração com o backend via React Query (useMutation)

Atualização automática da lista após edição

Após uma edição bem-sucedida, o cache da query ['clientes'] é invalidado: Essa decisão elimina a necessidade de recarregar manualmente a lista e mantém os dados sempre sincronizados com o servidor.

Mensagens de sucesso e erro são tratadas dentro da própria mutation (onSuccess / onError).

````

### Tarefa 2 : Completar a Tela de Veiculos

Contexto: Anteriormente a aplicação só atualizava o modelo do veiculo, foi solicitado que seja incluido os campos "Placa" e "Ano", e que seja possivel alterar o dono do veículo.

Com esta entrega, foi implementada a seção de edição de veiculos, permitindo ao usuário atualizar as informações de um veiculo já existente, com interação direta com o endpoint PUT /api/veiculos/{id} da API.

#### Back-End

🚀 Endpoint
PUT /api/veiculos/{id}

Atualiza as informações de um veiculo existente.

```
{
  "placa": "HEX-0313",
  "modelo": "Uno",
  "ano":2024,
  "clienteid":"3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
🔹 Respostas possíveis

| Codigo | Tipo | Descrição |
|:-----------:|:------------|------------:|
| 200    | OK    | Veiculo atualizado com sucesso.    |
| 404  | Not Found | Veiculo não existe!!  |
| 409  | Conflict  | Placa já existe.  |
| 400  | BadRequest  | Placa inválida.  |

````
{
  "id": "8df5a7c4-90f3-4d7e-b4b3-15a9a2b7990e",
  "placa": "HEX-0313",
  "modelo": "Uno",
  "ano":2024,
  "clienteid":"3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
````

Decisões técnicas: 

````
Segregação de responsabilidades por camadas (Controller, Business, Repository): Foi adotada uma arquitetura em camadas, seguindo o padrão Clean Architecture / DDD simplificado, evitando duplicação de lógica e mantém o código mais fácil de testar e evoluir.

Validação e normalização de placa: Foi utilizado o serviço _placa para sanitizar (remover espaços e formatações) e validar o padrão da placa antes de qualquer operação de banco. Essa decisão previne inconsistências de formato e duplicidades lógicas no cadastro de veículos.

Regra de unicidade da Placa: Antes de salvar, a aplicação garante que nenhuma outra entidade com Id diferente possua a mesma placa.

-- Controle de vigência Cliente–Veículo

   Quando ocorre troca de cliente:
      A vigência atual é encerrada (definindo DtFim).
      Uma nova relação é criada com data de início atual.
      Essa abordagem preserva o histórico de associações, permitindo rastrear qual cliente possuía o veículo em cada período.

Padronização das respostas HTTP: Foram mantidos os retornos RESTful conforme boas práticas.
````

#### Front-End

##### Principais responsabilidades:

Incluir novo modal de Edição;
Incluir os campos Placa, Ano e alteração de usuários.

Decisões técnicas: 

````
Edição via Modal: A antiga abordagem com prompts foi substituída por um componente modal que exibe um formulário completo de edição.
O modal contém campos para: Placa, Modelo, Ano, Cliente associado (select dinâmico)

Esse modal é exibido quando o usuário clica em “Editar” e fechado após salvar ou cancelar.

Ampliação dos Campos Editáveis: Anteriormente, apenas o Modelo podia ser alterado.
Agora, os campos Ano e ClienteId também podem ser modificados.
Isso permitiu alinhar o front com a lógica de negócio implementada no back-end (que atualiza vigências e valida placas).

Integração com React Query: A tela faz uso intensivo de React Query, que garante controle reativo dos dados:

useQuery → Busca clientes e veículos.
useMutation → Cria, atualiza e remove veículos.
invalidateQueries → Atualiza automaticamente a listagem após operações de CRUD.

Isso elimina a necessidade de recarregar a página manualmente após qualquer alteração.

Estado Local Estruturado: 
Foram adicionados dois estados distintos para formular os dados:
form → Para criação de veículos.
editarForm → Para edição (utilizado dentro do modal).
Além disso, o estado editar controla a exibição do modal.

Feedback Visual e UX: Mensagens de sucesso e erro são exibidas com alertas descritivos, mas o código está preparado para futura integração com toasts/snackbars. Os botões “Salvar” e “Cancelar” dentro do modal trazem controle direto sobre o fluxo de edição, evitando erros de navegação.

```` 

### Tarefa 3 : Melhorar Upload CSV

### Tarefa 4 : Faturamento Parcial

----------------------------------------------------------
### Stack de Referência
- **Backend**: .NET 8 Web API + EF Core + PostgreSQL  
- **Frontend**: React (Vite) + React Router + React Query  
- **Sem containers**: a conexão é configurada diretamente em `appsettings.json`.  

> É permitido substituir React por Angular/Vue e/ou trocar o ORM, desde que o escopo seja mantido e as decisões sejam explicadas no README. O boilerplate fornecido está em React com JavaScript.  

### Execução Local

#### Banco PostgreSQL
1. Crie um banco local (ex.: `parking_test`) e ajuste a `ConnectionString` em `appsettings.json`, se necessário.  
2. Rode o seed pelo terminal (bash/WSL):  
   ```bash
   psql -h localhost -U postgres -d parking_test -f scripts/seed.sql
   ```  
   Caso utilize Windows sem WSL, execute o script pelo gerenciador de banco de dados de sua preferência (ex.: DBeaver).  

#### Backend
```bash
cd src/backend
dotnet restore
dotnet run
```
A API será iniciada (por padrão) em `http://localhost:5000`. Swagger ativado em `/swagger`.  

#### Frontend
```bash
cd src/frontend
npm install
npm run dev
```
A aplicação ficará disponível em `http://localhost:5173`.  
Configure `VITE_API_URL` caso seja necessário apontar para outra porta.  

### 4.3 Estrutura de Pastas
```
/src/backend        -> API .NET 8
/src/frontend       -> React (Vite)
/scripts/seed.sql   -> Criação e seed do banco
/scripts/exemplo.csv-> CSV de exemplo
```

