using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace CalculaVendaGames
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// Array de From2
        /// 
        /// [0] => Steam , [1] => Patreon , [2] => Android , [3] => IOS
        /// </summary>
        private Form2[] _panels = new Form2[4]
            {
                new Form2(),
                new Form2(),
                new Form2(),
                new Form2()
            };

        /// <summary>
        /// Valor de conversão do dólar
        /// </summary>
        private float _dolar2Real = 0;

        public Form1()
        {
            InitializeComponent();
            Task.Run(PanelsInitialSettings);
            StartMyTaskFactory();
        }

        /// <summary>
        /// Inicia as Tasks aninhadas para a obtenção do valor de conversão do dólar
        /// </summary>
        private void StartMyTaskFactory()
        {
            Task<float> t1 = (Task<float>)Task.Factory.StartNew(WebScrap);
            Task t2 = t1.ContinueWith(async num =>
            {
                this._dolar2Real = await num;
                lblDolHj.Invoke(new Action(() =>
                {
                    lblDolHj.Text = $"R$ {this._dolar2Real:N2}";
                }));
            });
        }

        /// <summary>
        /// Faz webscraping para obter o valor de conversão do dólar
        /// </summary>
        /// <returns>Um float para setar o valor de conversão do dólar</returns>
        public float WebScrap()
        {
            var html = @"https://pt.exchange-rates.org/converter/USD/BRL/1";

            HtmlWeb web = new HtmlWeb();
            float dolar2Real = 0;

            try
            {
                var htmlDoc = web.Load(html);

                bool hasHead = htmlDoc.DocumentNode.SelectSingleNode("html/head") != null;
                bool hasBody = htmlDoc.DocumentNode.SelectSingleNode("html/body") != null;

                if (hasHead && hasBody)
                {
                    string spanID = "ctl00_M_lblToAmount";

                    /// DEPRECATED
                    //var query = $"//span[@id='{spanID}']";
                    //var node = htmlDoc.DocumentNode.SelectSingleNode(query);

                    // Pegando Elemento HTML diretamente pelo ID
                    string strDol2Real = htmlDoc.GetElementbyId(spanID).InnerText;

                    dolar2Real = float.Parse(strDol2Real);
                }
                else
                {
                    MessageBox.Show($"Erro ao obter o valor do dólar online.\nPor favor, verifique sua conexão de internet.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", $"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dolar2Real;
        }

        #region Referencias
        /*
        internal async void ScrapeWebsite()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage request = await httpClient.GetAsync("https://en.wikipedia.org/wiki/List_of_programmers");
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            //HtmlParser parser = new HtmlParser();
            //IHtmlDocument document = parser.ParseDocument(response);
        }
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        
        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                    .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection")).ToList();

            List<string> wikiLink = new List<string>();

            foreach (var link in programmerLinks)
            {
                if (link.FirstChild.Attributes.Count > 0)
                    wikiLink.Add("https://en.wikipedia.org/" + link.FirstChild.Attributes[0].Value);
            }

            return wikiLink;

        }
        */
        #endregion

        /// <summary>
        /// Seta os valores iniciais das cópias dinâmicas do formulário
        /// </summary>
        private void PanelsInitialSettings()
        {
            for (int i = 0; i < _panels.Length; i++)
            {
                if (i == 0)
                {
                    this._panels[i].lblSeederTitle.Text = "Steam";
                    this._panels[i].lblSeederCutAmount.Text = "Cut 30%";
                }
                else if (i == 1)
                {
                    this._panels[i].lblSeederTitle.Text = "Patreon";
                    this._panels[i].lblSeederCutAmount.Text = "Cut 5%";
                }
                else if (i == 2)
                {
                    this._panels[i].lblSeederTitle.Text = "Android";
                    this._panels[i].lblSeederCutAmount.Text = "Cut 30%";
                }
                else if (i == 3)
                {
                    this._panels[i].lblSeederTitle.Text = "IOS";
                    this._panels[i].lblSeederCutAmount.Text = "Cut 15%";
                }

                this._panels[i].lblCutDolVal.Text = "";
                this._panels[i].lblCutRealVal.Text = "";
                this._panels[i].lblArrecBrutaDolVal.Text = "";
                this._panels[i].lblArrecBrutaRealVal.Text = "";
                this._panels[i].lblLucLiq35DolVal.Text = "";
                this._panels[i].lblLucLiq35RealVal.Text = "";
                this._panels[i].lblLucLiq45DolVal.Text = "";
                this._panels[i].lblLucLiq45RealVal.Text = "";
            }

            lblDolHj.Text = "";
            lblTBADol.Text = "";
            lblTBAReal.Text = "";
        }

        /// <summary>
        /// Insere dinamicamente os formulários secundários no TablePanel do formulário principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            tableLayoutPanel1.Invoke( new Action(() => 
                {
                    tableLayoutPanel1.GetControlFromPosition(0, 0).Controls.Add(this._panels[0].panel1);
                    tableLayoutPanel1.GetControlFromPosition(1, 0).Controls.Add(this._panels[1].panel1);
                    tableLayoutPanel1.GetControlFromPosition(0, 1).Controls.Add(this._panels[2].panel1);
                    tableLayoutPanel1.GetControlFromPosition(1, 1).Controls.Add(this._panels[3].panel1);
                }));
        }

        /// <summary>
        /// Calcula os valores e insere os resultados em seus respectivos controles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            tableLayoutPanel1.Invoke(new Action(() =>
            {
                if (this._dolar2Real != 0)
                {
                    float baseValDol = float.Parse(txtValCpDol.Text) * float.Parse(txtQntCpVnd.Text);
                    float descBrutoEmDolar = 0;
                    float descBrutoEmReal = 0;
                    float arrecEmDolar = 0;
                    float arrecdEmReais = 0;
                    float lucroLiqEmDolar35 = 0;
                    float lucroLiqEMReal35 = 0;
                    float lucroLiqEmDolar45 = 0;
                    float lucroLiqEMReal45 = 0;

                    for (int i = 0; i < _panels.Length; i++)
                    {
                        if (i == 0)
                        {
                            descBrutoEmDolar = baseValDol * 0.3f;
                        }
                        if (i == 1)
                        {
                            descBrutoEmDolar = baseValDol * 0.05f;
                        }
                        if (i == 2)
                        {
                            descBrutoEmDolar = baseValDol * 0.3f;
                        }
                        if (i == 3)
                        {
                            if (baseValDol * 12 >= 1000000)
                            {
                                this._panels[i].lblSeederCutAmount.Text = "Cut 30%";
                                descBrutoEmDolar = baseValDol * 0.3f;
                            }
                            else
                            {
                                this._panels[i].lblSeederCutAmount.Text = "Cut 15%";
                                descBrutoEmDolar = baseValDol * 0.15f;
                            }


                        }

                        descBrutoEmReal = descBrutoEmDolar * this._dolar2Real;
                        arrecEmDolar = baseValDol - descBrutoEmDolar;
                        arrecdEmReais = arrecEmDolar * this._dolar2Real;
                        lucroLiqEmDolar35 = arrecEmDolar - (arrecEmDolar * 0.35f);
                        lucroLiqEMReal35 = lucroLiqEmDolar35 * this._dolar2Real;
                        lucroLiqEmDolar45 = arrecEmDolar - (arrecEmDolar * 0.45f);
                        lucroLiqEMReal45 = lucroLiqEmDolar45 * this._dolar2Real;

                        lblTBADol.Text = $"US$ {baseValDol:N2}";
                        lblTBAReal.Text = $"R$ {baseValDol * this._dolar2Real:N2}";

                        this._panels[i].lblCutDolVal.Text = $"US$ {descBrutoEmDolar:N2}";
                        this._panels[i].lblCutRealVal.Text = $"R$ {descBrutoEmReal:N2}";
                        this._panels[i].lblArrecBrutaDolVal.Text = $"US$ {arrecEmDolar:N2}";
                        this._panels[i].lblArrecBrutaRealVal.Text = $"R$ {arrecdEmReais:N2}";
                        this._panels[i].lblLucLiq35DolVal.Text = $"US$ {lucroLiqEmDolar35:N2}";
                        this._panels[i].lblLucLiq35RealVal.Text = $"R$ {lucroLiqEMReal35:N2}";
                        this._panels[i].lblLucLiq45DolVal.Text = $"US$ {lucroLiqEmDolar45:N2}";
                        this._panels[i].lblLucLiq45RealVal.Text = $"R$ {lucroLiqEMReal45:N2}";
                    }
                }
                else
                {
                    MessageBox.Show($"O valor de conversão do dólar ainda não foi obtido\nPorFavor, tente novamente.", $"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }));
        }

        private void txtValCpDol_Validating(object sender, CancelEventArgs e)
        {
            string pattern = "^[0-9]+(,[0-9]+)*$";
            //Regex.IsMatch(txtValCpDol.Text, pattern) ? "valid" : "invalid";
            if (!Regex.IsMatch(txtValCpDol.Text, pattern))
            {
                txtValCpDol.ForeColor = Color.Red;
                MessageBox.Show($"Por favor, entre somente com números e muma vírgula.\nEx: 19,99", $"Valor inválido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtValCpDol.Focus();
            }
            else
            {
                txtValCpDol.ForeColor = Color.FromKnownColor(KnownColor.WindowText);
            }
        }

        private void txtQntCpVnd_Validating(object sender, CancelEventArgs e)
        {
            string pattern = "^[0-9]*$";
            //Regex.IsMatch(txtValCpDol.Text, pattern) ? "valid" : "invalid";
            if (!Regex.IsMatch(txtQntCpVnd.Text, pattern))
            {
                txtQntCpVnd.ForeColor = Color.Red;
                MessageBox.Show($"Por favor, entre somente com números.\nEx: 3250", $"Valor inválido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtQntCpVnd.Focus();
            }
            else
            {
                txtQntCpVnd.ForeColor = Color.FromKnownColor(KnownColor.WindowText);
            }
        }
    }
}
