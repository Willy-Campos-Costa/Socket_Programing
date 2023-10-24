using System.Net;
using System.Net.Sockets;

namespace TServidor
{
    public class Operations
    {
        //Método inicial para fazer a conexão do cliente com o servidor
        public async Task Start(){
            try{
            string ip = "25.56.249.202" ; int port = 8000;
            TcpListener server = new TcpListener(IPAddress.Parse(ip),port);
            server.Start(); Console.Clear();
            System.Console.WriteLine("Carregando filmes do servidor...");
            List<Movies> movie = new List<Movies>(); MovieList(movie); //Criando a lista e preenchendo com os filmes
            System.Console.WriteLine("Filmes carregados");
            System.Console.WriteLine("Aguardando conexões de clientes...");
            while(true){
                TcpClient client = await server.AcceptTcpClientAsync();
                Task.Run(() => ClientConnections(client, movie)); //O método Task.Run inicia o método ClientConnection em uma thread unica
            }
            }catch(Exception ex){
                System.Console.WriteLine("Problema na comunicação: "+ ex.Message);
            }
        }

        public async Task ClientConnections(TcpClient client, List<Movies> movie){
            using(var stream = client.GetStream())
            using(var write = new StreamWriter(stream))
            using(var read = new StreamReader(stream)){
                //Pegando o IP de quem se conectou
                IPEndPoint Ep = (IPEndPoint)client.Client.RemoteEndPoint;
                string clientIp = $"{Ep.Address}:{Ep.Port}";
                System.Console.WriteLine($"O client ip {clientIp} se conectou!");

                //Enviando o menu para o cliente
                await write.WriteLineAsync(FirstMessage());
                await write.FlushAsync();

                //Recebendo o valor do menu selecionado e trata cada um
                Console.WriteLine($"{clientIp}: Aguardando opção do cliente...");
                string? value = read.ReadLine();
                //Se o valor for 1, o cliente acessa o menu e escolhe um ticket
                if(value == "1"){
                    await write.WriteLineAsync($"{clientIp}: Selecione um dos filmes para assistir:");
                    int movieLenght = movie.Count();
                    await write.WriteLineAsync(movieLenght.ToString()); //Enviando o tamanho da lista
                    movieShowClient(write,movieLenght,movie);
                    await write.FlushAsync();
                    Console.WriteLine($"{clientIp}: Aguardando cliente selecionar o filme...");
                    string? MovieId = read.ReadLine(); //Recebendo o valor do filme selecionado
                    ValidateTicket(movie,clientIp,MovieId); //Valida para o servidor se existe ticket disponivel
                    string Sucess = movie[int.Parse(MovieId)-1].removeTicket(1);
                    await write.WriteLineAsync(Sucess);
                    movieShowClient(write,movieLenght,movie);
                    await write.FlushAsync();
                    System.Console.WriteLine($"Desconectado o cliente {clientIp}");
                    client.Close();
                }
                //Se o valor for 2, desconecta o cliente.
                else if(value == "2"){
                    System.Console.WriteLine($"Desconectado o cliente {clientIp}");
                    client.Close();
                }
            }

        }

        //Logica para preencher a lista de filmes 
        public IEnumerable<Movies> MovieList(List<Movies> listMovie){
            string[] movies = new string[7];
                movies[0] = "O Segredo dos Seus Olhos"; movies[1] = "Pulp Fiction"; movies[2] = "O Senhor dos Anéis: A Sociedade do Anel";movies[3] = "Matrix";movies[4] = "Cidade de Deus";movies[5] = "A Origem";movies[6] = "A Lista de Schindler";
                HashSet<string> selectedMovies = new HashSet<string>();
            for(int i=0; i<5; i++){
                    string name;
                    do{
                        name = movies[new Random().Next(movies.Length)];
                    }while(selectedMovies.Contains(name));

                    int tickets = new Random().Next(1,10);
                    double Price = new Random().Next(10,25);
                    
                    selectedMovies.Add(name);

                listMovie.Add(new Movies(name,tickets,Price));
            }
            return listMovie;
        }

        
        //Primeira mensagem enviada para cliente
        public string FirstMessage(){
            return "Bem vindos ao sistema de compra de tickets do cinema Germany!\nSelecione uma das opções: [1] Ver filmes   [2] Sair";
        }

        //Mostrar a lista de filmes para o cliente com valor atualizado dos tickets
        public async void movieShowClient(StreamWriter write, int movieLenght, List<Movies> movie){
            for(int i=0; i<movieLenght; i++){
            await write.WriteLineAsync($"Id: [{i+1}] --- Tickets Disponiveis: {movie[i].AvaliableTickets} --- Preço: ${movie[i].Price},00 --- Filme: {movie[i].Name}");
            }
        }

        public void ValidateTicket(List<Movies> movie, string clientIp, string MovieId){
            if(movie[int.Parse(MovieId)-1].AvaliableTickets <= 0 ){
            Console.WriteLine($"Error: Cliente {clientIp} tentou comprar um ingresso esgotado do filme {movie[int.Parse(MovieId)-1].Name}");
            }else{
            Console.WriteLine($"Sucesso: Cliente {clientIp} comprou um ingresso do filme {movie[int.Parse(MovieId)-1].Name}");
            }
        }
    }
}