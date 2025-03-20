# StreamingVideoIndexer

Microsserviço em dotnet que monitora entradas em um diretorio especificado na config e as indexa, adicionando as propriedades do arquivo no banco de dados.
O controle de indexação é feito através de links simbólicos para evitar redundância
A monitoração do diretório é feita usando a classe FileSystemWatcher

Suporte a subtitulos para os vídeos da plataforma de streaming serão adicionados no futuro

Também serão adicionados testes unitários e de integração, juntamente com a implementação de uma pipeline para build com docker, github actions e jenkins
