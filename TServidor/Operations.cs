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
            string ip = "172.18.52.11" ; int port = 8000;
            TcpListener server = new TcpListener(IPAddress.Parse(ip),port);
            server.Start();
            System.Console.WriteLine("Aguardando conexões de clientes...");
            while(true){
                TcpClient client = await server.AcceptTcpClientAsync();
                ClientConnections(client);
            }
            }catch(Exception ex){
                System.Console.WriteLine("Problema na comunicação: "+ ex.Message);
            }
        }

        public async Task ClientConnections(TcpClient client){
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
                System.Console.WriteLine("Valor do cliente: " + value);
                if(value == "1"){
                    await write.WriteLineAsync("Selecione um dos filmes para assistir.");
                    List<Movies> movie = new List<Movies>(); MovieList(movie); //Criando a lista e preenchendo
                    int movieLenght = movie.Count();
                    
                    await write.WriteLineAsync(movieLenght.ToString());
                    await write.FlushAsync();
                }else if(value == "2"){
                    System.Console.WriteLine("valor 2 - FORA");
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
        public void ShowMovies(BinaryWriter write){
            List<Movies> movies = new List<Movies>();
            MovieList(movies);

            write.Write(movies.Count());
            foreach (var movie in movies)
            {
                write.Write($"Filme: {movie.Name} --- Preço: {movie.Price} --- Qtd Disponivel: {movie.AvaliableTickets}");
            }
        }
    }
}