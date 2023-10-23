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
            string ip = "172.18.52.12" ; int port = 8000;
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
                IPEndPoint Ep = (IPEndPoint)client.Client.RemoteEndPoint;
                System.Console.WriteLine($"O client ip {Ep.Address} se conectou!");

                await write.WriteLineAsync(FirstMessage());
                await write.FlushAsync();

                Console.WriteLine("Aguardando valor do cliente...");
                string value = await read.ReadLineAsync();
                Console.WriteLine("Valor do cliente: " + value);

                // string value = await read.ReadLineAsync();
                // System.Console.WriteLine("Valor do cliente: " + value);
                // if(value == "1"){
                //     System.Console.WriteLine("valor 1");
                // }else if(value == "2"){
                //     System.Console.WriteLine("valor 2");
                // }


                // string request = await read.ReadLineAsync();
                // Console.WriteLine($"Cliente diz: {request}");

                // await write.WriteLineAsync("Olá, cliente!");
            }

        }
        public string FirstMessage(){
            return "Bem vindos ao sistema de compra de tickets do cinema Germany!\n\nSelecione uma das opções: [1] Ver filmes   [2] Sair";
        }

        public IEnumerable<Movies> MovieList(List<Movies> listMovie){
            string[] filmes = new string[7];
                filmes[0] = "O Segredo dos Seus Olhos"; filmes[1] = "Pulp Fiction"; filmes[2] = "O Senhor dos Anéis: A Sociedade do Anel";filmes[3] = "Matrix";filmes[4] = "Cidade de Deus";filmes[5] = "A Origem";filmes[6] = "A Lista de Schindler";

            listMovie = new List<Movies>();
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