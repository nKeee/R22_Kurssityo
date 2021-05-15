using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;


namespace R22_Kurssityo
{
    public partial class Form1 : Form
    {
      
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.toimintaalueTableAdapter.Fill(this.dataSet1.toimintaalue);
            this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
            this.postiTableAdapter.Fill(this.dataSet1.posti);
            this.palveluTableAdapter.Fill(this.dataSet1.palvelu);
            this.varauksen_palvelutTableAdapter.Fill(this.dataSet1.varauksen_palvelut);
            this.varausTableAdapter.Fill(this.dataSet1.varaus);
            this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
            this.laskuTableAdapter1.Fill(this.dataSet1.lasku);
        }

        private void btn_tallenna_varaus_Click(object sender, EventArgs e)
        {
            Validate();
            asiakasBindingSource.EndEdit();
            postiBindingSource.EndEdit();
            postiTableAdapter.Update(this.dataSet1);

            bool loytyi = false;
            foreach (DataRow dr in dataSet1.posti.Rows) //Tarkistetaan, löytyykö kirjotettu postinumero jo SQL:stä
            {
                string postinumero = dr["postinro"].ToString();
                if (postinumero == tbPostinro.Text)
                    loytyi = true;
            }
            if(loytyi) //Jos löytyi, niin ei luoda uutta postinumeroa posti tableen
            {
                asiakasTableAdapter.Update(this.dataSet1);
                asiakasTableAdapter.Insert(tbPostinro.Text, tbEnimi.Text, tbSnimi.Text, tbOsoite.Text, tbSposti.Text, tbPuhnro.Text);
                this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
                varausBindingSource.EndEdit();
                varausTableAdapter.Update(this.dataSet1);
                DateTime tyhja = new DateTime(1111, 11, 11);
                long? asiakasnumero = asiakasTableAdapter.ScalarQuery();
                varausTableAdapter.Insert((long)asiakasnumero, (long)cbMokki_varaus.SelectedValue, DateTime.Now.Date, tyhja, dateTimePicker1.Value, dateTimePicker2.Value);
            }

            else //Jos ei löytynyt, niin luodaan uusi postinro
            {
                postiTableAdapter.Insert(tbPostinro.Text, tbPostitoimip.Text);
                //Lähetetään eka postitableen tiedot, koska asiakastablessa käytetään foreign keynä postinro.
                //Ilman tätä järjestystä tulee erroria.
                asiakasTableAdapter.Update(this.dataSet1);
                asiakasTableAdapter.Insert(tbPostinro.Text, tbEnimi.Text, tbSnimi.Text, tbOsoite.Text, tbSposti.Text, tbPuhnro.Text);
                this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
                //Ja nyt varaus sisään
                varausBindingSource.EndEdit();
                varausTableAdapter.Update(this.dataSet1);
                DateTime tyhja = new DateTime(1111, 11, 11);
                long? asiakasnumero = asiakasTableAdapter.ScalarQuery();
                varausTableAdapter.Insert((long)asiakasnumero, (long)cbMokki_varaus.SelectedValue, DateTime.Now.Date, tyhja, dateTimePicker1.Value, dateTimePicker2.Value);
            }
            tbEnimi.Text = "";
            tbSnimi.Text = "";
            tbOsoite.Text = "";
            tbSposti.Text = "";
            tbPuhnro.Text = "";
            tbPostinro.Text = "";
            tbPostitoimip.Text = "";
        }


        private void btn_uusimokki_Click(object sender, EventArgs e)
        {
            Validate();
            mokkiBindingSource.EndEdit();
            postiBindingSource.EndEdit();
            postiTableAdapter.Update(this.dataSet1);

            bool loytyi = false;
            foreach (DataRow dr in dataSet1.posti.Rows) //Tarkistetaan, löytyykö kirjotettu postinumero jo SQL:stä
            {
                string postinumero = dr["postinro"].ToString();
                if (postinumero == tbMokkipostinro.Text)
                    loytyi = true;
            }
            if(loytyi)
            {
                mokkiTableAdapter.Update(this.dataSet1);
                mokkiTableAdapter.Insert((long)cbToimalue.SelectedValue, tbMokkipostinro.Text, tbMokkinimi.Text, tbMokkiosoite.Text, tbMokkikuvaus.Text, Convert.ToInt32(tbMokkihenkimaara.Text), tbMokkivarustelu.Text);
                this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
            }
            else
            {
                postiTableAdapter.Insert(tbMokkipostinro.Text, tbMokkipostitoimipaik.Text);
                //Lähetetään eka postitableen tiedot, koska mökkitablessa käytetään foreign keynä postinro.
                //Ilman tätä järjestystä tulee erroria.
                mokkiTableAdapter.Update(this.dataSet1);
                mokkiTableAdapter.Insert((long)cbToimalue.SelectedValue, tbMokkipostinro.Text, tbMokkinimi.Text, tbMokkiosoite.Text, tbMokkikuvaus.Text, Convert.ToInt32(tbMokkihenkimaara.Text), tbMokkivarustelu.Text);
                this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
            }
            tbMokkihenkimaara.Text = "";
            tbMokkikuvaus.Text = "";
            tbMokkinimi.Text = "";
            tbMokkiosoite.Text = "";
            tbMokkipostinro.Text = "";
            tbMokkipostitoimipaik.Text = "";
            tbMokkivarustelu.Text = "";
        }

        private void btnUusitoimalue_Click(object sender, EventArgs e)
        {
            Validate();
            toimintaalueBindingSource.EndEdit();
            toimintaalueTableAdapter.Update(this.dataSet1);
            toimintaalueTableAdapter.Insert(tbToimintaalue.Text);
            this.toimintaalueTableAdapter.Fill(this.dataSet1.toimintaalue);
            tbToimintaalue.Text = "";
        }

        private void btnPoistaTA_Click(object sender, EventArgs e)
        {
            dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
            this.toimintaalueTableAdapter.Update(this.dataSet1.toimintaalue);
        }

        private void btnLisaapalvelu_Click(object sender, EventArgs e)
        {
            Validate();
            palveluBindingSource.EndEdit();
            palveluTableAdapter.Update(this.dataSet1);
            //palveluTableAdapter.Insert();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tbMokkihenkimaara_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnVahvistaVaraus_Click(object sender, EventArgs e)
        {
            Validate();
            varausBindingSource.EndEdit();
            varausTableAdapter.Update(this.dataSet1);
            dgVaraukset.CurrentRow.Cells[4].Value = DateTime.Now.ToString("dd/M/yyyy");
            this.varausTableAdapter.Update(this.dataSet1.varaus);

            //Seuraavaksi laskutukseen uusi lasku:

            DateTime aloitus = (DateTime)dgVaraukset.CurrentRow.Cells[5].Value;
            DateTime lopetus = (DateTime)dgVaraukset.CurrentRow.Cells[6].Value;
            TimeSpan tp = lopetus - aloitus;
            int erotusPaivina = tp.Days;
            double verotonHinta = erotusPaivina * 100; //muutetaan tarvittaessa verolliseksi hinnaksi

            int arvo = Convert.ToInt32(dgVaraukset.CurrentRow.Cells[0].Value);
            laskuBindingSource1.EndEdit();
            laskuTableAdapter1.Insert(arvo, arvo, verotonHinta, 24);

        }

        private void btnPoistaVaraus_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in this.dgVaraukset.SelectedRows)
            {
                dgVaraukset.Rows.RemoveAt(item.Index);
            }
            this.varausTableAdapter.Update(this.dataSet1.varaus);
        }

        private void tab_laskutus_Enter(object sender, EventArgs e)
        {
            this.laskuTableAdapter1.Fill(this.dataSet1.lasku);
        }

        private void btnPoistaLasku_Click(object sender, EventArgs e)
        {
            Validate();
            foreach (DataGridViewRow laskuRow in dgwLasku.SelectedRows)
            {
                if (laskuRow.Selected)
                    dgwLasku.Rows.RemoveAt(laskuRow.Index);
            }
            this.laskuTableAdapter1.Update(this.dataSet1.lasku);

        }
        private void btnHaeVaraus_Click(object sender, EventArgs e)// oikeasti btnTulostaLasku mutta ei antanut vaihtaa nimeä...
        {
            //Laskun kirjoitus tiedostoon
            var polku = "C:\\temp\\data.txt";
            string lasku = "Lasku id: " + dgwLasku.CurrentRow.Cells[0].Value.ToString() +
                " Asiakas id: " + dgwLasku.CurrentRow.Cells[1].Value.ToString() +
                " Summa: " + dgwLasku.CurrentRow.Cells[3].Value.ToString() +
                " Alv: " + dgwLasku.CurrentRow.Cells[4].Value.ToString();


            using (StreamWriter sw = new StreamWriter(polku))
            {
                sw.WriteLine(lasku);
                sw.Flush();
                sw.Close();
            }
            //MessageBox.Show(cmd.ExecuteReader().Read().ToString());
        
        }

        private void btnPoistaAsiakas_Click(object sender, EventArgs e)
        {
            try
            {
                Validate();
                asiakasBindingSource.EndEdit();
                asiakasTableAdapter.Update(this.dataSet1);
                dgAsiakkaat.Rows.RemoveAt(dgAsiakkaat.CurrentRow.Index);
                this.asiakasTableAdapter.Update(this.dataSet1.asiakas);
            }
            catch
            {
                string message = " Poista varaus ensin varauksenhallinnasta!";
                string title = "Huomio";
                MessageBox.Show(message, title);
                this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
            }

        }

        private void tab_varaus_Enter(object sender, EventArgs e)
        {
            this.varausTableAdapter.Fill(this.dataSet1.varaus);
        }

        private void btnMuokkaaAsiakas_Click(object sender, EventArgs e)
        {
            Validate();
            asiakasTableAdapter.Update(this.dataSet1); 
            
        }


    }
}
