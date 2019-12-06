using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP1
{
    public partial class View : Form
    {
        Controller Controller;
        private delegate void DelegateStrBool(String s, Boolean bl);
        private delegate void DelegateIntIntStrBool(int a, int b, String s, Boolean bl);
        Dictionary<String, Image> camisa;
        Dictionary<String, Label> label;
        public View()
        {
            InitializeComponent();
            Controller = new Controller(this);
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

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            new CamisaV(this, 1);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            new CamisaV(this,2);
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
    }
}
