# 🤖 Agentic Context Engine (Prova - Laboratório de Programação)

Este projeto é uma aplicação WEB desenvolvida em **C# (ASP.NET Core)** com banco de dados **PostgreSQL**, atuando como o "cérebro estrutural" para a memória de agentes digitais (Chatbots). O sistema possui um front-end nativo em HTML/CSS/JS e implementa forte uso de Orientação a Objetos no backend.

---

## 🚀 Como Rodar o Projeto Passo a Passo

Para que o projeto funcione 100% na sua máquina (ou na apresentação), siga exatamente a ordem abaixo:

### Passo 1: Preparar o Banco de Dados (PostgreSQL)
1. Certifique-se de que o **PostgreSQL** está instalado e rodando no seu computador.
2. Crie um banco de dados vazio no pgAdmin ou via terminal para o projeto.
3. Abra o arquivo `appsettings.json` (dentro da pasta `ME_Laboratorio_Programacao`) e configure a **Connection String** com o seu usuário, senha e nome do banco criado.
4. Caso tenha o script `.sql` de criação das tabelas, rode-o no banco. Caso contrário, o Entity Framework pode criar as tabelas se as Migrations estiverem configuradas.

### Passo 2: Iniciar a API (Backend)
O Front-end não funciona sozinho! Ele precisa do servidor C# rodando para processar os logins e mensagens.
1. Abra um terminal (`cmd` ou aba "Terminal" do VS Code).
2. Navegue até a pasta do projeto (onde está o arquivo `.csproj`):
   ```bash
   cd ME_Laboratorio_Programacao
   ```
3. Execute o servidor do .NET:
   ```bash
   dotnet run
   ```
4. Aguarde aparecer a mensagem verde no terminal dizendo que o servidor está rodando (ex: `Now listening on: http://localhost:5268`). **Não feche esse terminal!**

### Passo 3: Abrir o Front-end (Site)
Você tem duas opções para abrir o painel:
- **Opção A (Acesso Direto via .NET):** Com o comando `dotnet run` ativo, basta abrir o navegador e acessar o endereço informado no terminal (ex: `http://localhost:5268`). O C# já está configurado para servir a página de login automaticamente.
- **Opção B (Via Live Server):** Se preferir rodar no VS Code, abra a pasta `wwwroot`, clique com o botão direito no arquivo `index.html` e escolha **"Open with Live Server"**. O código JavaScript está preparado para identificar automaticamente a porta da API.

---

## 🛠️ Resolução de Problemas (Troubleshooting)

### ❌ Erro: "Erro de conexão de servidor" ao tentar logar
Se você tentou entrar no sistema e o balão vermelho de erro apareceu, siga essa checklist de verificação:

1. **O Backend está rodando?**
   O erro mais comum é tentar abrir o `index.html` sem ter rodado o comando `dotnet run`. A tela precisa da API para validar o usuário. Volte no Passo 2 e verifique se o servidor não está parado ou se o terminal fechou.
2. **O Banco de Dados ligou?**
   Verifique se o serviço do PostgreSQL (pgAdmin) está ativo. Se o backend tentou ligar, mas o banco estava desligado ou a senha no `appsettings.json` estava errada, a API quebra silenciosamente e recusa as conexões.
3. **Página abrindo como Arquivo Local (`file:///...`)**
   Você não pode abrir o `index.html` apenas dando um "duplo clique" no arquivo do seu computador. Os navegadores bloqueiam chamadas de API (CORS/Segurança) quando você está no protocolo `file://`. **Use sempre o Live Server ou acesse via `http://localhost:...`**.
4. **Verifique o Console do Navegador**
   Na página de Login, aperte `F12` e vá na aba **Console** ou **Network**. Lá mostrará o motivo exato da falha de conexão com a API (ex: `ERR_CONNECTION_REFUSED` indica que o servidor .NET está desligado).

---

