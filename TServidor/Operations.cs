using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TServidor
{
    public class Operations
    {
        public async Task Start(){
            try{
            string ip = "192.168.100.18" ; int port = 8000;
            TcpListener server = new TcpListener(IPAddress.Parse(ip),port);
            server.Start(); Console.Clear();
            System.Console.WriteLine("Carregando filmes do servidor...");
            List<Movies> movie = new List<Movies>(); MovieList(movie); //Criando a lista e preenchendo com os filmes
            System.Console.WriteLine("Filmes carregados");
            System.Console.WriteLine("Aguardando conexões de clientes...");
            while(true){
                TcpClient client = await server.AcceptTcpClientAsync();
                ClientConnections(client, movie);
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
                System.Console.WriteLine($"O client ip {Ep.Address} se conectou!");

                //Enviando o menu para o cliente
                await write.WriteLineAsync(FirstMessage());
                await write.FlushAsync();

                //Recebendo o valor do menu selecionado e trata cada um
                Console.WriteLine("Aguardando valor do cliente...");
                string? value = read.ReadLine();
                if(value == "1"){
                    await write.WriteLineAsync("Selecione um dos filmes para assistir:");
                    int movieLenght = movie.Count();
                    await write.WriteLineAsync(movieLenght.ToString()); //Enviando o tamanho da lista
                    for(int i=0; i<movieLenght; i++){
                        await write.WriteLineAsync($"Id: [{i+1}] --- Tickets Disponiveis: {movie[i].AvaliableTickets} --- Preço: ${movie[i].Price},00 --- Filme: {movie[i].Name}");
                    }
                    await write.FlushAsync();
                    Console.WriteLine("Aguardando cliente selecionar o filme...");
                    string? MovieId = read.ReadLine(); //Recebendo o valor do filme selecionado
                    string Sucess = movie[int.Parse(MovieId)-1].removeTicket(1);
                    System.Console.WriteLine($"Sucesso: Cliente {Ep.Address} comprou um ingresso do filme {movie[int.Parse(MovieId)-1].Name}");
                    await write.WriteLineAsync(Sucess);

                    for(int i=0; i<movieLenght; i++){
                        await write.WriteLineAsync($"Id: [{i+1}] --- Tickets Disponiveis: {movie[i].AvaliableTickets} --- Preço: ${movie[i].Price},00 --- Filme: {movie[i].Name}");
                    }
                    
                    await write.FlushAsync();
                }else if(value == "2"){
                    System.Console.WriteLine($"Desconectado o cliente {Ep.Address}");
                    client.Close();
                }
            }

        }
        public string FirstMessage(){
            return "Bem vindos ao sistema de compra de tickets do cinema Germany!\nSelecione uma das opções: [1] Ver filmes   [2] Sair";
        }

        public IEnumerable<Movies> MovieList(List<Movies> listMovie){
            string[] filmes = new string[7];
                filmes[0] = "O Segredo dos Seus Olhos"; filmes[1] = "Pulp Fiction"; filmes[2] = "O Senhor dos Anéis: A Sociedade do Anel";filmes[3] = "Matrix";filmes[4] = "Cidade de Deus";filmes[5] = "A Origem";filmes[6] = "A Lista de Schindler";

            for(int i=0; i<5; i++){
                    string name = filmes[new Random().Next(filmes.Length)];
                    int tickets = new Random().Next(1,10);
                    double Price = new Random().Next(10,25);
                    
                listMovie.Add(new Movies(name,tickets,Price));
            }
            return listMovie;
        }
    }
}