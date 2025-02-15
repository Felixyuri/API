# 🛠️ Pré-requisito
    .NET 8 SDK
    Visual Studio ou Visual Studio Code
    dotnet-ef (pode instalar usando este comando no terminal de preferência: dotnet tool install --global dotnet-ef)

# 🛠️ Como Executar o Projeto (Insira no terminal):
    git clone https://github.com/Felixyuri/API.git
    cd API
    dotnet restore
    cd API.Api
    dotnet ef database update
    dotnet run

# 🛠️ A API estará disponível em:
    http://localhost:5172
    ou http://localhost:5000

# 🛠️ Acesse o swagger pelo link:
    http://localhost:5172/swagger
    ou http://localhost:5172/

# 🛠️ Executar testes unitários (Insira no terminal):
    cd API.Tests
    dotnet test
