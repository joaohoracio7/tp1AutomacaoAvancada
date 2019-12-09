using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;

namespace PCP
{
    public partial class View : Form
    {
        // Trata o nome do usuário
        private string NomeUsuario = "PCP";
        private StreamWriter stwEnviador;
        private StreamReader strReceptor; 
        private TcpClient tcpServidor;
        // Necessário para atualizar o formulário com mensagens da outra thread
        private delegate void AtualizaLogCallBack(string strMensagem);
        // Necessário para definir o formulário para o estado "disconnected" de outra thread
        private delegate void FechaConexaoCallBack(string strMotivo);
        private Thread mensagemThread;
        private IPAddress enderecoIP;
        private bool Conectado;
        private List<OP> listaOps;

        public View()
        {
           // Na saida da aplicação : desconectar
           Application.ApplicationExit += new EventHandler(OnApplicationExit);
           InitializeComponent();
           listaOps = new List<OP>();
        }

        private void btnConectar_Click(object sender, System.EventArgs e)
        {
            // se não esta conectando aguarda a conexão
            if (Conectado == false)
            {
                // Inicializa a conexão
                InicializaConexao();
            }
            else // Se esta conectado entao desconecta
            {
                FechaConexao("Desconectado a pedido do usuário.");
            }

        }
        private void InicializaConexao()
        {
            try
            {
                // Trata o endereço IP informado em um objeto IPAdress
                enderecoIP = IPAddress.Parse(txtServidorIP.Text);
                // Inicia uma nova conexão TCP com o servidor chat
                tcpServidor = new TcpClient();
                tcpServidor.Connect(enderecoIP, 2502);

                // AJuda a verificar se estamos conectados ou não
                Conectado = true;

                // Prepara o formulário
                NomeUsuario = txtUsuario.Text;

                // Desabilita e habilita os campos apropriados
                txtServidorIP.Enabled = false;
                txtUsuario.Enabled = false;
                btnEnviar.Enabled = true;
                btnConectar.Text = "Desconectar";

                // Envia o nome do usuário ao servidor
                stwEnviador = new StreamWriter(tcpServidor.GetStream());
                stwEnviador.WriteLine(txtUsuario.Text);
                stwEnviador.Flush();

                //Inicia a thread para receber mensagens e nova comunicação
                mensagemThread = new Thread(new ThreadStart(RecebeMensagens));
                mensagemThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message, "Erro na conexão com servidor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RecebeMensagens()
        {
            // recebe a resposta do servidor
            strReceptor = new StreamReader(tcpServidor.GetStream());
            string ConResposta = strReceptor.ReadLine(); //Verifica as mensagens recebidas do servidor
            // Se o primeiro caracater da resposta é 1 a conexão foi feita com sucesso
            if (ConResposta[0] == '1')
            {
                // Atualiza o formulário para informar que esta conectado
                this.Invoke(new AtualizaLogCallBack(this.AtualizaLog), new object[] { "Conectado com sucesso!" });
            }
            else // Se o primeiro caractere não for 1 a conexão falhou
            {
                string Motivo = "Não Conectado: ";
                // Extrai o motivo da mensagem resposta. O motivo começa no 3o caractere
                Motivo += ConResposta.Substring(2, ConResposta.Length - 2);
                // Atualiza o formulário como o motivo da falha na conexão
                this.Invoke(new FechaConexaoCallBack(this.FechaConexao), new object[] { Motivo }); //"Invoke" atualiza o textbox para a mensagem mais recente
                // Sai do método
               return;
            }

            // Enquanto estiver conectado le as linhas que estão chegando do servidor
            while (Conectado)
            {
                // exibe mensagems no Textbox
                this.Invoke(new AtualizaLogCallBack(this.AtualizaLog), new object[] { strReceptor.ReadLine() });
            }
        }

        private void AtualizaLog(string strMensagem)
        {
            if (strMensagem.Split(' ')[0] == "SDCD")
                return;
            // Anexa texto ao final de cada linha
            txtLog.AppendText(strMensagem + "\r\n");
        }

        private void btnEnviar_Click(object sender, System.EventArgs e)
        {
            EnviaMensagem();
        }

        private void txtMensagem_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Se pressionou a tecla Enter
            if (e.KeyChar == (char)13)
            {
                EnviaMensagem();
            }
        }

        // Envia a mensagem para o servidor
        private void EnviaMensagem()
        {
            OP op = new OP(nomeCliente.Text, qtdProdutos.Value, tipoGola.Text, tipoEstampa.Text);
            listaOps.Add(op);

            if (op.ToString() != "")
            {
                stwEnviador.WriteLine(op.ToString()); //Escreve a linha para a conexão TCP
                stwEnviador.Flush(); //Garante que a mensagem está sendo enviada de imediato
            }
            nomeCliente.Text = "";
        }

        // Fecha a conexão com o servidor
        private void FechaConexao(string Motivo)
        {
            // Mostra o motivo porque a conexão encerrou
            txtLog.AppendText(Motivo + "\r\n");
            // Habilita e desabilita os controles apropriados no formulario
            txtServidorIP.Enabled = true;
            txtUsuario.Enabled = true;
            btnEnviar.Enabled = false;
            btnConectar.Text = "Conectar";

            // Fecha os objetos
            Conectado = false;
            stwEnviador.Close();
            strReceptor.Close();
            tcpServidor.Close();
        }

        // O tratador de evento para a saida da aplicação
        // Caso o usuário finalize a aplicação pela "esc", a comunicação é fechada
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (Conectado == true)
            {
                // Fecha as conexões, streams, etc...
                Conectado = false;
                stwEnviador.Close();
                strReceptor.Close();
                tcpServidor.Close();
            }
        }

        private void BtnRelat_Click(object sender, EventArgs e)
        {
            txtLog.AppendText("Nº OP" + "\t" + "Nome do Cliente" + "\t" + "Qtd" + "\t" + "Gola" + "\t" + "Estampa" + "\t" + "Data do Pedido" + "\r\n");
            foreach (OP op in listaOps)
            {
                txtLog.AppendText(op.ToString() + "\r\n");
            }
            txtLog.AppendText("Total de OPs:" + listaOps.Count.ToString() +"\r\n");
        }
    }
}
