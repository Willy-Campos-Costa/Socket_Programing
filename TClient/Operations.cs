using System;
using System.Net;
using System.Net.Sockets;

namespace TClient
{
    public class Operations
    {
        //Método inicial para fazer a conexão do cliente com o servidor
        public async Task Start(){
            try{
            string ip = "25.56.249.202" ; int port = 8000;
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(ip),port);
            await Communication(client);
            }catch(Exception ex){
                System.Console.WriteLine("Problema ao comunicar com o servidor: "+ex.Message);
            };
        }

        //Método principal de comunicação com o servidor (Troca de mensagem)
        public async Task Communication(TcpClient client){
            Console.Clear();
            using (var stream = client.GetStream())
            using (var write = new StreamWriter(stream))
            using (var read = new StreamReader(stream))
            {
                //Aguardando a resposta inicial do servidor - MENU
                Console.WriteLine(await read.ReadLineAsync());                
                Console.WriteLine(await read.ReadLineAsync());
                
                //Entra no método de digitar opção e envia p/ servidor
                string value = SelectOption(2);                 
                System.Console.WriteLine("\nvalor: " + value);
                await write.WriteLineAsync(value);
                await write.FlushAsync();

                //Recebe a lista de filmes p/ selecionar
                Console.Clear();
                System.Console.WriteLine(await read.ReadLineAsync());
                int movieLenght = int.Parse(await read.ReadLineAsync());
                movieList(read,movieLenght);
                string MovieId = SelectOption(movieLenght);
                System.Console.WriteLine();

                //Enviando a opção do filme selecionado
                await write.WriteLineAsync(MovieId); 
                await write.FlushAsync();
                Console.WriteLine(await read.ReadLineAsync());                
            }
        }

        //Método para seleção de opção
        public string SelectOption(int x){
            while(true){
                var op = Console.ReadKey().KeyChar.ToString();
                var val = int.TryParse(op, out int valor);
                if(valor <= x && valor > 0){
                    return op.ToString();
                }else{
                    System.Console.WriteLine(" <-- Valor invalido");
                }
            }
        }

        //Método para listar os filmes disponiveis
        public async void movieList(StreamReader read, int movieLenght){
            for(int i=0; i<movieLenght; i++){
            System.Console.WriteLine(await read.ReadLineAsync());
            }
        }
    }
}