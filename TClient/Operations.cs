using System;
using System.Net;
using System.Net.Sockets;

namespace TClient
{
    public class Operations
    {
        public async Task Start(){
            try{
            string ip = "192.168.100.18" ; int port = 8000;
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(ip),port);
            await Communication(client);
            }catch(Exception ex){
                System.Console.WriteLine("Problema ao comunicar com o servidor: "+ex.Message);
            };
        }
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
                for(int i=0; i<movieLenght; i++){
                    System.Console.WriteLine(await read.ReadLineAsync());
                }
                string MovieId = SelectOption(movieLenght);
                System.Console.WriteLine($"\nValor: {MovieId}");
                await write.WriteLineAsync(MovieId); //Enviando a opção do filme selecionado
                await write.FlushAsync();
                Console.WriteLine(await read.ReadLineAsync());                

                for(int i=0; i<movieLenght; i++){
                    System.Console.WriteLine(await read.ReadLineAsync());
                }
            }
        }
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
    }
}