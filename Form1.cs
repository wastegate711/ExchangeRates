using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
using Timer = System.Threading.Timer;

namespace ExchangeRates
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            #region Заполняем ComboBox названиями валюты
            List<Catalog> catalogs = new List<Catalog>();
            string file = @"http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + DateTime.Now.ToString("dd.MM.yyyy");
            XDocument document = XDocument.Load(file);

            foreach (XElement element in document.Root.Elements())
            {
                foreach (XAttribute xa in element.Attributes())
                {
                    catalogs.Add(new Catalog
                    {
                        ID = xa.Value,
                        Name = element.Element("Name").Value
                    });
                }
            }
            #endregion

            comboBox1.DataSource = catalogs;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "ID";
            //comboBox1.Text = "Выберите валюту";
            comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
        }

        string GetCursValute(DateTime dateTime, string valut)
        {
            string dataUrl = @"http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + dateTime.ToString("dd.MM.yyyy");
            XDocument xDocument = XDocument.Load(dataUrl);
            XElement ex = xDocument.Root.XPathSelectElement("Valute[@ID='" + valut + "']");

            return string.Format("{0}:{1}", ex.Element("CharCode").Value, ex.Element("Value").Value);
        }
        /// <summary>
        /// Кнопка "Запрос курса с сайта"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = GetCursValute(dateTimePicker1.Value, comboBox1.SelectedValue.ToString());
        }
        /// <summary>
        /// Заполнить базу данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int cnt = 0;
            using (ModelExchangeRate db = new ModelExchangeRate())
            {
                cnt = db.Currency.Count();
            }
            if (((int)cnt > 0) && MessageBox.Show("В базе данных есть записи. Вы уверены, что хотите продолжить?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            string dataUrl = @"http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + DateTime.Now.ToString("dd.MM.yyyy");
            XDocument xDocument = XDocument.Load(dataUrl);
            XElement root = xDocument.Root;
            List<Valute> valutes = (from valute in root.Elements("Valute")
                                    select new Valute()
                                    {
                                        Id = (string)valute.Attribute("ID"),
                                        NumCode = (string)valute.Element("NumCode"),
                                        CharCode = (string)valute.Element("CharCode"),
                                        Nominal = (int)valute.Element("Nominal"),
                                        Name = (string)valute.Element("Name"),
                                        Value = Convert.ToDecimal((string)valute.Element("Value"))
                                    }).ToList();

            foreach (Valute v in valutes)
            {
                using (ModelExchangeRate db = new ModelExchangeRate())
                {
                    Currency c = new Currency()
                    {
                        Id = v.Id,
                        NumCode = v.NumCode,
                        CharCode = v.CharCode,
                        Nominal = v.Nominal,
                        Name = v.Name
                    };
                    db.Currency.Add(c);
                    db.SaveChanges();
                }
            }

            for (var date = new DateTime(2021, 01, 01); date <= DateTime.Now; date = date.AddDays(1))
            {
                GetDailyRate(date);
            }
        }

        static void GetDailyRate(DateTime dt)
        {
            string dataUrl = @"http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + dt.ToString("dd.MM.yyyy");
            XDocument xDocument = XDocument.Load(dataUrl);
            XElement root = xDocument.Root;
            List<DailyRate> dr = (from valute in root.Elements("Valute")
                                  select new DailyRate()
                                  {
                                      IdCurrency = (string)valute.Attribute("ID"),
                                      Rate = Convert.ToDecimal((string)valute.Element("Value")),
                                      Dt = dt
                                  }).ToList();
            using (ModelExchangeRate db = new ModelExchangeRate())
            {
                db.DailyRate.AddRange(dr);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Запрос курса из базы данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = GetCursValuteFromDB(dateTimePicker1.Value, comboBox1.SelectedValue.ToString());
        }
        string GetCursValuteFromDB(DateTime dateTime, string valut)
        {
            using (ModelExchangeRate db = new ModelExchangeRate())
            {
                SqlParameter id_char = new SqlParameter("@id_char", valut);
                SqlParameter dt = new SqlParameter("@date", dateTime);

                decimal rate = db.Database.SqlQuery<decimal>("SELECT * FROM [dbo].[GetRate] (@id_char,@date)", new object[] { id_char, dt }).FirstOrDefault();
                if (rate == 0)
                {
                    MessageBox.Show("Курс не найден");
                }
                return rate.ToString();
            }
        }

        void Form1_Load(object sender, EventArgs e)
        {
            TimerCallback tm = new TimerCallback(GetDailyRatesIfNeed);
            // создаем таймер
            Timer timer = new Timer(tm, null, 0, 3600000);
        }

        static void GetDailyRatesIfNeed(object obj)
        {
            if (DateTime.Now.Hour == 0)
            {
                GetDailyRate((DateTime)obj);
            }
        }
    }
}

internal class Catalog
{
    public string Name { get; set; }
    public string ID { get; set; }
}
