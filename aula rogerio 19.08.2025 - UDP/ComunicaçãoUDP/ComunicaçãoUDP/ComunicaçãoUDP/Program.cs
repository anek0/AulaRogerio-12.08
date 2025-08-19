using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        // Pergunta ao usuário em qual porta ele quer ESCUTAR mensagens recebidas
        Console.Write("Digite a porta para receber mensagens: ");
        int porta = int.Parse(Console.ReadLine());

        // Pergunta o IP do computador para o qual as mensagens serão enviadas
        Console.Write("Digite o IP do computador destino: ");
        string ipDestino = Console.ReadLine();

        // Pergunta a porta do computador de destino
        Console.Write("Digite a porta do computador destino: ");
        int portaDestino = int.Parse(Console.ReadLine());

        // Criamos um UdpClient que ficará escutando na porta informada
        UdpClient udpReceiver = new UdpClient(porta);

        // Criamos outro UdpClient para enviar mensagens
        UdpClient udpSender = new UdpClient();

        // Criamos uma thread separada para ESCUTAR mensagens recebidas
        Thread receiveThread = new Thread(() =>
        {
            // Endereço remoto (quem enviou a mensagem)
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    // Recebe dados UDP de qualquer remetente
                    byte[] data = udpReceiver.Receive(ref remoteEP);

                    // Converte de bytes para string (UTF8)
                    string message = Encoding.UTF8.GetString(data);

                    // Mostra a mensagem na tela com o IP de quem enviou
                    Console.WriteLine($"\n[{remoteEP.Address}]: {message}");

                    // Reposiciona o cursor para que o usuário continue digitando
                    Console.Write("> ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao receber: " + ex.Message);
                }
            }
        });

        // Define que a thread deve rodar em segundo plano
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Loop principal do programa (envio de mensagens)
        while (true)
        {
            // Lê a mensagem digitada pelo usuário
            Console.Write("> ");
            string mensagem = Console.ReadLine();

            // Converte a mensagem para bytes
            byte[] data = Encoding.UTF8.GetBytes(mensagem);

            // Envia os dados para o IP e porta do destinatário
            udpSender.Send(data, data.Length, ipDestino, portaDestino);
        }
    }
}
