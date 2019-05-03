# API - "IPTGram"

Este projeto serve como um "starter" para o projeto "IPTGram" para a Unidade Curricular de Tecnologias de Internet 2, ano letivo 2018/2019.

**Recomendo a leitura deste ficheiro na íntegra antes de fazer o setup do projeto!**

## Estrutura do projeto

- `Controllers`: Controllers para as várias operações do projeto.
- `Data`: Classes da base de dados, e modelo de dados.
- `images`: Contém as imagens necessárias para o seed de dados.
- `Migrations`: Migrações de dados necessárias para atualizar a base de dados.
- `ModelBinders/JsonFormFileModelBinder.cs`: Uma classe útil para quando se trabalha com JSON e ficheiros em simultâneo (ex: upload de fotografias)
- `Models`: Modelos para input e output de dados.
- `SeedData`: Informação necessária para o seed da base de dados, tanto users como dados.
- `Views/Home/Index.cshtml`: Uma página de arranque.
- `AppOptions.cs`: Classe usada para guardar configurações da aplicação.
- `appsettings.*.json`: Usado para guardar connection strings e as configurações da aplicação.
- `Program.cs`: Main.
- `README.md`: Ver `README.md`.
- `Startup.cs`: Ficheiro que configura a aplicação toda.

## Preparação e configuração

É necessário o [.net Core SDK](https://dotnet.microsoft.com/download) versão 2.2. Está disponível para macOS, Linux, e Windows.

### Visual Studio

1. Ler este ficheiro na íntegra.
2. Transferir os ficheiros necessários do GitHub, Moodle, e afins.
3. Abrir o ficheiro `IPTGram.csproj` através da opção `File > Open > Project/Solution...`.
4. Abrir o `Package Manager Console`.
5. Restaurar os NuGet packages, se necessário.
6. Executar o comando `Update-Database -Verbose` para criar a base de dados.

### Visual Studio Code

1. Ler este ficheiro na íntegra.
2. Transferir os ficheiros necessários do GitHub, Moodle, e afins.
3. Abrir a pasta do projecto no Visual Studio Code.
4. Certificar que a extensão [c#](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) está instalada.
5. Abrir um terminal na pasta do projecto.
6. No terminal, executar `dotnet restore` para restaurar os NuGet packages.
7. No terminal, executar `dotnet ef database update` para criar a base de dados.

## Base de dados

O projecto foi configurado para usar SQL Server LocalDB. Em caso do Windows, o ficheiro fica em `c:\users\<user>\IPTGram.mdf`. **macOS e Linux não suportam SQL Server LocalDB, logo necessitam de outro SGBD (ex: MySQL)!**

### Usar outras bases de dados (ex: MySQL)

É possível usar outras bases de dados, desde que sejam instalados os NuGet packages necessários. Para MySQL, é necessário adicionar os seguintes NuGet packages:

- `MySql.Data.EntityFrameworkCore`

Para instalar no Visual Studio Code, executa-se o seguinte comando no terminal: `dotnet add package <nome do package>`. No Visual Studio, pode-se usar o `Manage NuGet Packages for Solution...`.

Depois de instalado e configurado, deve-se alterar o ficheiro `Startup.cs` para usar MySQL, no método `ConfigureServices`. Notar que a opção de `UseSqlServer` deve ser comentada se se pretende usar MySQL.

Mais informação e tutoriais:

- https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core.html
- https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-example.html

## Seed

O seed da base de dados e sua autenticação é feito no seguinte ficheiro: `Data/DbInitializer.cs`, e é executado no arranque do servidor (ver ficheiro `Startup.cs`, método `Configure`).

Todos os users têm a mesma palavra passe. A informação usada para inicializar a base de dados estão nos ficheiros `SeedData/users.json` e `SeedData/data.json`.

Por questões de segurança, os dados dos utilizadores estão no [Moodle da Unidade Curricular](https://doctrino.ipt.pt/mod/resource/view.php?id=23987). Devem substituir o ficheiro `SeedData/users.json` pelo ficheiro que está no Moodle.

**Nota: A API disponibilizada pelo professor tem palavras-passe diferentes para todos os users! É favor contactar o professor caso queiram o vosso username e password.**
