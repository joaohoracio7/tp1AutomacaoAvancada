using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP1
{
    public partial class View : Form
    {
        private LinhaDeProducao l1;
        private LinhaDeProducao l2;
        private delegate void DelegateStrBool(String s, Boolean bl);
        private delegate void DelegateIntIntStrBool(int a, int b, String s, Boolean bl);
        Dictionary<String, Image> camisa;
        Dictionary<String, Label> label;

        // Trata o nome do usuário
        private string NomeUsuario = "SDCD";
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

        public View()
        {
            InitializeComponent();
            l1 = new LinhaDeProducao(this, 1);
            l2 = new LinhaDeProducao(this, 2);
            camisa = new Dictionary<string, Image>
            {
                { "redonda", TP1.Properties.Resources.redonda },
                { "v", TP1.Properties.Resources.v },
                { "polo", TP1.Properties.Resources.polo }
            };
            label = new Dictionary<string, Label>
            {
                { "labelCorte1", labelCorte1 },
                { "labelCorte2", labelCorte2 },
                { "labelCostura1", labelCostura1 },
                { "labelCostura2", labelCostura2 },
                { "labelEstampa1", labelEstampa1 },
                { "labelEstampa2", labelEstampa2 },
                { "labelEstampadoraLinha1", labelEstampadoraLinha1 },
                { "labelEstampadoraLinha2", labelEstampadoraLinha2 },
                { "labelEmpacotadoraLinha1", labelEmpacotadoraLinha1 },
                { "labelEmpacotadoraLinha2", labelEmpacotadoraLinha2 }
            };

            // Na saida da aplicação : desconectar
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
        }

        private void Button1_Click(object sender, EventArgs e)
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

        private void Button2_Click(object sender, EventArgs e)
        {
            EnviaMensagem();
        }

        // Método que atualiza as imagens das linhas de produção,
        // dado o número da linha e da etapa
        public void atualizaLinha(int linha, int etapa, string tipo, Boolean visivel)
        {
            if (linha == 1)
            {
                switch (etapa)
                {
                    case 1:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (corte1.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            corte1.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            if (visivel)
                            {
                                corte1.Image = camisa[tipo.ToLower()];
                            }

                            corte1.Visible = visivel;
                            tesouraLinha1.Visible = visivel;
                            tesoura1.Visible = !visivel;
                        }
                        break;

                    case 2:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (costura1.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            costura1.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            if (visivel)
                            {
                                costura1.Image = camisa[tipo.ToLower()];
                            }

                            costura1.Visible = visivel;
                            maquinaLinha1.Visible = visivel;
                            maquina1.Visible = !visivel;
                        }
                        break;

                    case 3:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (estampa1.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            estampa1.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            if (visivel)
                            {
                                estampa1.Image = camisa[tipo.ToLower()];
                            }

                            estampa1.Visible = visivel;
                            estampadoraLinha.Visible = visivel;
                            estampadora.Visible = !visivel;
                        }
                        break;

                    case 4:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (embalando1.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            embalando1.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            embalando1.Visible = visivel;
                            empacotadoraLinha.Visible = visivel;
                            empacotadora.Visible = !visivel;
                        }
                        break;
                }
            }
            else
            {
                switch (etapa)
                {
                    case 1:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (corte2.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            corte2.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            if (visivel)
                            {
                                corte2.Image = camisa[tipo.ToLower()];
                            }

                            corte2.Visible = visivel;
                            tesouraLinha2.Visible = visivel;
                            tesoura2.Visible = !visivel;
                        }
                        break;

                    case 2:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (costura2.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            costura2.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            if (visivel)
                            {
                                costura2.Image = camisa[tipo.ToLower()];
                            }

                            costura2.Visible = visivel;
                            maquinaLinha2.Visible = visivel;
                            maquina2.Visible = !visivel;
                        }
                        break;

                    case 3:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (estampa2.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            estampa2.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            if (visivel)
                            {
                                estampa2.Image = camisa[tipo.ToLower()];
                            }

                            estampa2.Visible = visivel;
                            estampadoraLinha.Visible = visivel;
                            estampadora.Visible = !visivel;
                        }
                        break;

                    case 4:
                        // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
                        if (embalando2.InvokeRequired)
                        {
                            // Caso não seja, invoca um delegate para tal
                            var d = new DelegateIntIntStrBool(atualizaLinha);
                            embalando2.Invoke(d, linha, etapa, tipo, visivel);
                        }
                        else
                        {
                            // Caso seja, executa a ação
                            embalando2.Visible = visivel;
                            empacotadoraLinha.Visible = visivel;
                            empacotadora.Visible = !visivel;
                        }
                        break;
                }
            }
        }

        public void atualizaLabel(string l, Boolean visivel)
        {
            // Testa se a chamada ao método não é feita pela mesma thread que tem o controle do forms
            if (label[l].InvokeRequired)
            {
                // Caso não seja, invoca um delegate para tal
                var d = new DelegateStrBool(atualizaLabel);
                corte1.Invoke(d, l, visivel);
            }
            else
            {
                // Caso seja, executa a ação
                label[l].Visible = visivel;
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

                // Desabilita e habilita os campos apropriados
                txtServidorIP.Enabled = false;
                button1.Text = "Desconectar";

                // Envia o nome do usuário ao servidor
                stwEnviador = new StreamWriter(tcpServidor.GetStream());
                stwEnviador.WriteLine(NomeUsuario);
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
            // Anexa texto ao final de cada linha
            richTextBox1.AppendText(strMensagem + "\r\n");
        }

        // Envia a mensagem para o servidor
        private void EnviaMensagem()
        {
            if (true)
            {
                stwEnviador.WriteLine("Teste"); //Escreve a linha para a conexão TCP
                stwEnviador.Flush(); //Garante que a mensagem está sendo enviada de imediato
            }
        }

        // Fecha a conexão com o servidor
        private void FechaConexao(string Motivo)
        {
            // Mostra o motivo porque a conexão encerrou
            richTextBox1.AppendText(Motivo + "\r\n");
            // Habilita e desabilita os controles apropriados no formulario
            txtServidorIP.Enabled = true;
            button1.Text = "Conectar";

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

    }
}
