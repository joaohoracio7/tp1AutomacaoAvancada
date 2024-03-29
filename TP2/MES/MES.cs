﻿using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace MES
{
    // Trata os argumentos para o evento StatusChanged
    public class StatusChangedEventArgs : EventArgs
    {
        // Estamos interessados na mensagem descrevendo o evento
        private string EventMsg;

        // Propriedade para retornar e definir uma mensagem do evento
        public string EventMessage
        {
            get { return EventMsg;}
            set { EventMsg = value;}
        }

        // Construtor para definir a mensagem do evento
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }

    // Este delegate é necessário para especificar os parametros que estamos passando com o nosso evento
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    class Servidor
    {
        // Esta hash table armazena os usuários e as conexões (acessado/consultado por usuário)
        public static Hashtable htUsuarios = new Hashtable(5); // 5 usuarios é o limite definido
        // Esta hash table armazena os usuários e as conexões (acessada/consultada por conexão)
        public static Hashtable htConexoes = new Hashtable(5); // 5 usuários é o limite definido
        // armazena o endereço IP passado
        private IPAddress enderecoIP;
        private TcpClient tcpCliente;
        // O evento e o seu argumento irá notificar o formulário quando um usuário se conecta, desconecta, envia uma mensagem,etc
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
        private static Queue<OP> filaOpsRedonda = new Queue<OP>();
        private static Queue<OP> filaOpsV = new Queue<OP>();
        private static Queue<OP> filaOpsPolo = new Queue<OP>();

        // O construtor define o endereço IP para aquele retornado pela instanciação do objeto
        public Servidor(IPAddress endereco)
        {
            enderecoIP = endereco;
        }

        // A thread que ira tratar o escutador de conexões
        private Thread thrListener;

        // O objeto TCP object que escuta as conexões
        private TcpListener tlsCliente;

        // Ira dizer ao laço while para manter a monitoração das conexões
        bool ServRodando = false;

        // Inclui o usuário nas tabelas hash
        public static void IncluiUsuario(TcpClient tcpUsuario, string strUsername)
        {
            // Primeiro inclui o nome e conexão associada para ambas as hash tables
            Servidor.htUsuarios.Add(strUsername, tcpUsuario);
            Servidor.htConexoes.Add(tcpUsuario, strUsername);

            // Informa a nova conexão para todos os usuário e para o formulário do servidor
            EnviaMensagemAdmin(htConexoes[tcpUsuario] + " entrou..");
        }

        // Remove o usuário das tabelas (hash tables)
        public static void RemoveUsuario(TcpClient tcpUsuario)
        {
            // Se o usuário existir
            if (htConexoes[tcpUsuario] != null)
            {
                // Primeiro mostra a informação e informa os outros usuários sobre a conexão
                EnviaMensagemAdmin(htConexoes[tcpUsuario] + " saiu...");

                // Removeo usuário da hash table
                Servidor.htUsuarios.Remove(Servidor.htConexoes[tcpUsuario]);
                Servidor.htConexoes.Remove(tcpUsuario);
            }
        }

        // Este evento é chamado quando queremos disparar o evento StatusChanged
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                // invoca o  delegate
                statusHandler(null, e);
            }
        }

        // Envia mensagens administrativas
        public static void EnviaMensagemAdmin(string Mensagem)
        {
            StreamWriter swSenderSender;

            // Exibe primeiro na aplicação
            e = new StatusChangedEventArgs("Administrador: " + Mensagem);
            OnStatusChanged(e);

            // Cria um array de clientes TCPs do tamanho do numero de clientes existentes
            TcpClient[] tcpClientes = new TcpClient[Servidor.htUsuarios.Count];
            // Copia os objetos TcpClient no array
            Servidor.htUsuarios.Values.CopyTo(tcpClientes, 0);
            // Percorre a lista de clientes TCP
            for (int i = 0; i < tcpClientes.Length; i++)
            {
                // Tenta enviar uma mensagem para cada cliente
                try
                {
                    // Se a mensagem estiver em branco ou a conexão for nula sai...
                    if (Mensagem.Trim() == "" || tcpClientes[i] == null)
                    {
                        continue;
                    }
                    // Envia a mensagem para o usuário atual no laço
                    swSenderSender = new StreamWriter(tcpClientes[i].GetStream());
                    swSenderSender.WriteLine("Administrador: " + Mensagem);
                    swSenderSender.Flush(); //garante que a mensagem está sendo enviada no momento imediato
                    swSenderSender = null;
                }
                catch // Se houver um problema , o usuário não existe , então remove-o
                {
                    RemoveUsuario(tcpClientes[i]);
                }
            }
        }

        // Envia mensagens de um usuário para todos os outros
        public static void EnviaMensagem(string Origem, string Mensagem)
        {
            if(Origem == "PCP")
            {
                String[] Msg = Mensagem.Split('\t');
                OP op = new OP(int.Parse(Msg[0]), Msg[1], int.Parse(Msg[2]), Msg[3], Msg[4], Msg[5]);
                if(op.getTipoGola() == "redonda")
                {
                    filaOpsRedonda.Enqueue(op);
                }
                else if(op.getTipoGola() == "v")
                {
                    filaOpsV.Enqueue(op);
                }
                else
                {
                    filaOpsPolo.Enqueue(op);
                }
                
                return;
            }
            //else if(Origem == "SDCD")
            //{
            //    // Primeiro exibe a mensagem na aplicação
            //    e = new StatusChangedEventArgs(Origem + " disse : " + Mensagem);
            //    OnStatusChanged(e);

            //    int faz = 0;
            //    string tipo;
            //    String[] Msg = Mensagem.Split(':');
            //    int linha = int.Parse(Msg[2].Trim());

            //    // Caso Linha 1 requisitando pedido
            //    if (linha == 1)
            //    {
            //       if(filaOpsRedonda.Peek() != null)
            //        {
            //            if(filaOpsV.Peek() != null)
            //            {
            //                if (filaOpsRedonda.Peek().getQtdProdutos() < filaOpsV.Peek().getQtdProdutos())
            //                {
            //                    tipo = "redonda";
            //                    if (filaOpsRedonda.Peek().getQtdProdutos() > 3)
            //                    {
            //                        filaOpsRedonda.Peek().setQtdProdutos(filaOpsRedonda.Peek().getQtdProdutos() - 3);
            //                        faz = 3;
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();                                }
            //                    else
            //                    {
            //                        faz = filaOpsRedonda.Peek().getQtdProdutos();
            //                        filaOpsRedonda.Dequeue();
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                }
            //                else
            //                {
            //                    tipo = "v";
            //                    if (filaOpsV.Peek().getQtdProdutos() > 3)
            //                    {
            //                        filaOpsV.Peek().setQtdProdutos(filaOpsV.Peek().getQtdProdutos() - 3);
            //                        faz = 3;
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                    else
            //                    {
            //                        faz = filaOpsV.Peek().getQtdProdutos();
            //                        filaOpsV.Dequeue();
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                tipo = "redonda";
            //                if (filaOpsRedonda.Peek().getQtdProdutos() > 3)
            //                {
            //                    filaOpsRedonda.Peek().setQtdProdutos(filaOpsRedonda.Peek().getQtdProdutos() - 3);
            //                    faz = 3;
            //                    // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                    Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                }
            //                else
            //                {
            //                    faz = filaOpsRedonda.Peek().getQtdProdutos();
            //                    filaOpsRedonda.Dequeue();
            //                    // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                    Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                }
            //            }
            //        }
            //    }
            //    else // Linha 2 requisitando pedido
            //    {
            //        if (filaOpsRedonda.Peek() != null)
            //        {
            //            if (filaOpsPolo.Peek() != null)
            //            {
            //                if (filaOpsRedonda.Peek().getQtdProdutos() < filaOpsPolo.Peek().getQtdProdutos())
            //                {
            //                    tipo = "redonda";
            //                    if (filaOpsRedonda.Peek().getQtdProdutos() > 3)
            //                    {
            //                        filaOpsRedonda.Peek().setQtdProdutos(filaOpsRedonda.Peek().getQtdProdutos() - 3);
            //                        faz = 3;
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                    else
            //                    {
            //                        faz = filaOpsRedonda.Peek().getQtdProdutos();
            //                        filaOpsRedonda.Dequeue();
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                }
            //                else
            //                {
            //                    tipo = "polo";
            //                    if (filaOpsPolo.Peek().getQtdProdutos() > 3)
            //                    {
            //                        filaOpsPolo.Peek().setQtdProdutos(filaOpsPolo.Peek().getQtdProdutos() - 3);
            //                        faz = 3;
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                    else
            //                    {
            //                        faz = filaOpsPolo.Peek().getQtdProdutos();
            //                        filaOpsPolo.Dequeue();
            //                        // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                        Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                tipo = "redonda";
            //                if (filaOpsRedonda.Peek().getQtdProdutos() > 3)
            //                {
            //                    filaOpsRedonda.Peek().setQtdProdutos(filaOpsRedonda.Peek().getQtdProdutos() - 3);
            //                    faz = 3;
            //                    // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                    Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                }
            //                else
            //                {
            //                    faz = filaOpsRedonda.Peek().getQtdProdutos();
            //                    filaOpsRedonda.Dequeue();
            //                    // Atualiza mensagem pra resposta, ela só vai ser útil no SCDC
            //                    Mensagem = faz.ToString() + " camisetas: " + tipo + "na linha: " + linha.ToString();
            //                }
            //            }
            //        }
            //    }
            //    if(faz == 0)
            //    {
            //        return;
            //    }
            //}

            StreamWriter swSenderSender;

            // Cria um array de clientes TCPs do tamanho do numero de clientes existentes
            TcpClient[] tcpClientes = new TcpClient[Servidor.htUsuarios.Count];
            // Copia os objetos TcpClient no array
            Servidor.htUsuarios.Values.CopyTo(tcpClientes, 0);
            // Percorre a lista de clientes TCP
            for (int i = 0; i < tcpClientes.Length; i++)
            {
                // Tenta enviar uma mensagem para cada cliente
                try
                {
                    // Se a mensagem estiver em branco ou a conexão for nula sai...
                    if (Mensagem.Trim() == "" || tcpClientes[i] == null)
                    {
                        continue;
                    }
                    // Envia a mensagem para o usuário atual no laço
                    swSenderSender = new StreamWriter(tcpClientes[i].GetStream());
                    swSenderSender.WriteLine("SDCD" + " faça: " + Mensagem);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch // Se houver um problema , o usuário não existe , então remove-o
                {
                    RemoveUsuario(tcpClientes[i]);
                }
            }
        }

        public void IniciaAtendimento()
        {
            try
            {

                // Pega o IP do primeiro dispostivo da rede
                IPAddress ipaLocal = enderecoIP;

                // Cria um objeto TCP listener usando o IP do servidor e porta definidas
                tlsCliente = new TcpListener(ipaLocal, 2502);

                // Inicia o TCP listener e escuta as conexões
                tlsCliente.Start();

                // O laço While verifica se o servidor esta rodando antes de checar as conexões
                ServRodando = true;

                // Inicia uma nova tread que hospeda o listener
                thrListener = new Thread(MantemAtendimento);
                thrListener.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MantemAtendimento()
        {
            // Enquanto o servidor estiver rodando
            while (ServRodando == true)
            {
                // Aceita uma conexão pendente
                tcpCliente = tlsCliente.AcceptTcpClient();
                // Cria uma nova instância da conexão
                Conexao newConnection = new Conexao(tcpCliente);
            }
        }
    }

    // Esta classe trata as conexões, serão tantas quanto as instâncias do usuários conectados
    class Conexao
    {
        TcpClient tcpCliente;
        // A thread que ira enviar a informação para o cliente
        private Thread thrSender;
        private StreamReader srReceptor;
        private StreamWriter swEnviador;
        private string usuarioAtual;
        private string strResposta;

        // O construtor da classe que que toma a conexão TCP
        public Conexao(TcpClient tcpCon)
        {
            tcpCliente = tcpCon;
            // A thread que aceita o cliente e espera a mensagem
            thrSender = new Thread(AceitaCliente);
            // A thread chama o método AceitaCliente()
            thrSender.Start();
        }

        private void FechaConexao()
        {
            // Fecha os objetos abertos
            tcpCliente.Close();
            srReceptor.Close();
            swEnviador.Close();
        }

        // Ocorre quando um novo cliente é aceito
        private void AceitaCliente()
        {
            srReceptor = new System.IO.StreamReader(tcpCliente.GetStream());
            swEnviador = new System.IO.StreamWriter(tcpCliente.GetStream());

            // Lê a informação da conta do cliente
            usuarioAtual = srReceptor.ReadLine();

            // temos uma resposta do cliente
            if (usuarioAtual != "")
            {
                // Armazena o nome do usuário na hash table
                if (Servidor.htUsuarios.Contains(usuarioAtual) == true)
                {
                    // 0 => significa não conectado
                    swEnviador.WriteLine("0|Este nome de usuário já existe.");
                    swEnviador.Flush();
                    FechaConexao();
                    return;
                }
                else if (usuarioAtual == "Administrator")
                {
                    // 0 => não conectado
                    swEnviador.WriteLine("0|Este nome de usuário é reservado.");
                    swEnviador.Flush();
                    FechaConexao();
                    return;
                }
                else
                {
                    // 1 => conectou com sucesso
                    swEnviador.WriteLine("1");
                    swEnviador.Flush();

                    // Inclui o usuário na hash table e inicia a escuta de suas mensagens
                    Servidor.IncluiUsuario(tcpCliente, usuarioAtual);
                }
            }
            else
            {
                FechaConexao();
                return;
            }
            //
            try
            {
                // Continua aguardando por uma mensagem do usuário
                while ((strResposta = srReceptor.ReadLine()) != "")
                {
                    // Se for inválido remove-o
                    if (strResposta == null)
                    {
                        Servidor.RemoveUsuario(tcpCliente);
                    }
                    else
                    {
                        // envia a mensagem para todos os outros usuários
                        Servidor.EnviaMensagem(usuarioAtual, strResposta);
                    }
                }
            }
            catch
            {
                // Se houve um problema com este usuário desconecta-o
                Servidor.RemoveUsuario(tcpCliente);
            }
        }
    }
}
