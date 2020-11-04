using cardOfClinic.Entity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace cardOfClinic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog openFileDialog;
        private Historia historia;
        private XmlSerializer xmlSerializer;
        
        string path = "";
        private SaveFileDialog saveFileDialog;
        private SaveFileDialog saveFileDialogPdf;

        public MainWindow()
        {
            InitializeComponent();
            

           
        }

        

        private void otworz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                openFileDialog = null;
                openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files (*.xml)|*.xml";
                if (openFileDialog.ShowDialog() == true)
                {
                    path = openFileDialog.InitialDirectory + openFileDialog.FileName;


                    xmlSerializer = null;
                    xmlSerializer = new XmlSerializer(typeof(Historia));
                    TextReader reader = new StreamReader(path);
                    historia = (Historia)(xmlSerializer.Deserialize(reader));

                    poradnia.Text = historia.poradnia;
                    nrKarty.Text = historia.nr_karty.ToString();
                    dataZarej.Text = historia.data_zarejestrowania.ToString();
                    //Dane Pacjenta
                    nazwisko.Text = historia.pacjent.nazwisko;
                    imie.Text = historia.pacjent.imie;
                    dataUrodz.Text = DateTime.ParseExact(historia.pacjent.data_urodzenia.ToString(), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");
                    telefon.Text = historia.pacjent.telefon;
                    adres.Text = historia.pacjent.adres.ulica + " " + historia.pacjent.adres.numer + " " + historia.pacjent.adres.kod_pocztowy + " " + historia.pacjent.adres.miasto;
                    pesel.Text = historia.pacjent.pesel;
                    plec.Text = historia.pacjent.plec.ToString();
                    podstawaUbez.Text = historia.podstawa_ubezpieczenia;
                    komentarz.Text = historia.komentarz;

                    //Dane upoważnionej os.
                    listView.ItemsSource = historia.authorised;

                    //aktywacja kontrolek
                    poradnia.IsEnabled = true;
                    dataZarej.IsEnabled = true;
                    calendar.IsEnabled = true;
                    nazwisko.IsEnabled = true;
                    imie.IsEnabled = true;
                    dataUrodz.IsEnabled = true;
                    telefon.IsEnabled = true;
                    adres.IsEnabled = true;
                    pesel.IsEnabled = true;
                    plec.IsEnabled = true;
                    podstawaUbez.IsEnabled = true;
                    komentarz.IsEnabled = true;
                    choroby.IsEnabled = true;
                    calendar.IsEnabled = true;
                    imieINazwisko.IsEnabled = true;
                    adresOsoba.IsEnabled = true;
                    DataDowodPesel.IsEnabled = true;
                    telefonOsoba.IsEnabled = true;
                    dodajOsoba.IsEnabled = true;
                    urodzinKalendarz.IsEnabled = true;
                    zapisz.IsEnabled = true;

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void choroby_Click(object sender, RoutedEventArgs e)
        {
            oknoChoroby.IsOpen = true;
            listViewChoroby.ItemsSource = historia.wywiad;
        }

        private void zamknij_Click(object sender, RoutedEventArgs e)
        {
            oknoChoroby.IsOpen = false;
        }

        

        private void calendar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            groupBox.Visibility = Visibility.Visible;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var calendarWindow = sender as System.Windows.Controls.Calendar;

                // ... See if a date is selected.
                if (calendarWindow.SelectedDate.HasValue)
                {
                    // ... Display SelectedDate in Title.
                    DateTime date = calendarWindow.SelectedDate.Value;
                    dataZarej.Text = date.ToShortDateString();
                    groupBox.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void calendartOfGroup_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var calendarWindow = sender as System.Windows.Controls.Calendar;

                // ... See if a date is selected.
                if (calendarWindow.SelectedDate.HasValue)
                {
                    // ... Display SelectedDate in Title.
                    DateTime date = calendarWindow.SelectedDate.Value;
                    dataWizyty.Text = date.ToShortDateString();
                    groupBox1.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void oknoCalendar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            groupBox1.Visibility = Visibility.Visible;
        }

        private void dodaj_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                if(!String.IsNullOrWhiteSpace(dataWizyty.Text)&& !String.IsNullOrWhiteSpace(rozpoznanieIcd10.Text)&& !String.IsNullOrWhiteSpace(rozpoznanie.Text)&& !String.IsNullOrWhiteSpace(nazwiskoLekarza.Text))
                {
                    var tmpWywiad = historia.wywiad;
                    Array.Resize(ref tmpWywiad, tmpWywiad.Length + 1);
                    tmpWywiad[tmpWywiad.Length - 1] = new ChorobyChoroba();
                    tmpWywiad[tmpWywiad.Length-1].data_wizyty = DateTime.Parse(dataWizyty.Text);
                    tmpWywiad[tmpWywiad.Length - 1].rozpoznanie_icd10 = rozpoznanieIcd10.Text;
                    tmpWywiad[tmpWywiad.Length - 1].rozpoznanie = rozpoznanie.Text;
                    tmpWywiad[tmpWywiad.Length - 1].nazwisko_lekarza = nazwiskoLekarza.Text;
                    historia.wywiad = tmpWywiad;
                    listViewChoroby.ItemsSource = historia.wywiad;
                    dataWizyty.Text = null;
                    rozpoznanieIcd10.Text = null;
                    rozpoznanie.Text = null;
                    nazwiskoLekarza.Text = null;
                }
                else
                {
                    MessageBox.Show("Wypełnij wszystkie pola");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dodajOsoba_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(imieINazwisko.Text) && !String.IsNullOrWhiteSpace(adresOsoba.Text) && !String.IsNullOrWhiteSpace(DataDowodPesel.Text) && !String.IsNullOrWhiteSpace(telefonOsoba.Text))
                {
                    var tmpAuthorised = historia.authorised;
                    Array.Resize(ref tmpAuthorised, tmpAuthorised.Length + 1);
                    tmpAuthorised[tmpAuthorised.Length - 1] = new AuthorizedUpowazniony() { imie_nazwisko = imieINazwisko.Text, adres_upowaznionego = new AdresType() { ulica = ulica.Text, numer = nr.Text, kod_pocztowy = kodPocztowy.Text, miasto = miasto.Text }, nr_telefonu = telefonOsoba.Text };
                    if (DataDowodPesel.Text.Length == 11)
                    {
                        tmpAuthorised[tmpAuthorised.Length - 1].ItemElementName = ItemChoiceType.pesel;
                    }
                    else if (DataDowodPesel.Text.Length == 10)
                    {
                        tmpAuthorised[tmpAuthorised.Length - 1].Item = DateTime.ParseExact(DataDowodPesel.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        tmpAuthorised[tmpAuthorised.Length - 1].ItemElementName = ItemChoiceType.data_urodz_upowazn;
                    }
                    else
                    {
                        tmpAuthorised[tmpAuthorised.Length - 1].ItemElementName = ItemChoiceType.nr_dowodu;
                    }
                    historia.authorised = tmpAuthorised;
                    listView.ItemsSource = historia.authorised;
                    imieINazwisko.Text = null;
                    ulica.Text = null;
                    nr.Text = null;
                    kodPocztowy.Text = null;
                    miasto.Text = null;
                    DataDowodPesel.Text = null;
                    telefonOsoba.Text = null;
                    adresOsoba.Text = null;

                }
                else
                {
                    MessageBox.Show("Wypełnij wszystkie pola");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void adresOsoba_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            oknoAdres.IsOpen = true;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(ulica.Text)&& !String.IsNullOrWhiteSpace(nr.Text)&& !String.IsNullOrWhiteSpace(kodPocztowy.Text)&& !String.IsNullOrWhiteSpace(miasto.Text))
            {
                adresOsoba.Text = ulica.Text + " " + nr.Text + " " + kodPocztowy.Text + " " + miasto.Text;
                oknoAdres.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Wypełnij wszystkie pola");
            }
        }

        private void listView_Selected(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((AuthorizedUpowazniony)listView.SelectedItem).imie_nazwisko);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                reset.IsEnabled = true;
                dodajOsoba.IsEnabled = false;
                edytujOsoba.IsEnabled = true;
                var osoba = ((AuthorizedUpowazniony)listView.SelectedItem);
                imieINazwisko.Text = osoba.imie_nazwisko;
                adresOsoba.Text = osoba.adres_upowaznionego.ulica + " " + osoba.adres_upowaznionego.numer + " " + osoba.adres_upowaznionego.kod_pocztowy + " " + osoba.adres_upowaznionego.miasto;
                DataDowodPesel.Text = osoba.Item;
                telefonOsoba.Text = osoba.nr_telefonu;
                ulica.Text = osoba.adres_upowaznionego.ulica;
                nr.Text = osoba.adres_upowaznionego.numer;
                kodPocztowy.Text = osoba.adres_upowaznionego.kod_pocztowy;
                miasto.Text = osoba.adres_upowaznionego.miasto;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void edytujOsoba_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(imieINazwisko.Text) && !String.IsNullOrWhiteSpace(adresOsoba.Text) && !String.IsNullOrWhiteSpace(DataDowodPesel.Text) && !String.IsNullOrWhiteSpace(telefonOsoba.Text))
                {
                    historia.authorised[listView.SelectedIndex].imie_nazwisko = imieINazwisko.Text;
                    historia.authorised[listView.SelectedIndex].adres_upowaznionego.ulica = ulica.Text;
                    historia.authorised[listView.SelectedIndex].adres_upowaznionego.numer = nr.Text;
                    historia.authorised[listView.SelectedIndex].adres_upowaznionego.kod_pocztowy = kodPocztowy.Text;
                    historia.authorised[listView.SelectedIndex].adres_upowaznionego.miasto = miasto.Text;
                    historia.authorised[listView.SelectedIndex].nr_telefonu = telefonOsoba.Text;

                    listView.Items.Refresh();
                    listView.ItemsSource = historia.authorised;
                    edytujOsoba.IsEnabled = false;
                    imieINazwisko.Text = null;
                    ulica.Text = null;
                    nr.Text = null;
                    kodPocztowy.Text = null;
                    miasto.Text = null;
                    telefonOsoba.Text = null;
                    adresOsoba.Text = null;
                }
                else
                {
                    MessageBox.Show("Wypełnij wszystkie pola");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void urodzinKalendarz_MouseUp(object sender, MouseButtonEventArgs e)
        {
            groupBox2.Visibility = Visibility.Visible;
        }

        private void kalenardz_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var calendarWindow = sender as System.Windows.Controls.Calendar;

                // ... See if a date is selected.
                if (calendarWindow.SelectedDate.HasValue)
                {
                    // ... Display SelectedDate in Title.
                    DateTime date = calendarWindow.SelectedDate.Value;
                    DataDowodPesel.Text = date.ToShortDateString();
                    groupBox2.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void zapisz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(poradnia.Text) && !String.IsNullOrWhiteSpace(nrKarty.Text) && !String.IsNullOrWhiteSpace(dataZarej.Text) && !String.IsNullOrWhiteSpace(nazwisko.Text) && !String.IsNullOrWhiteSpace(imie.Text) && !String.IsNullOrWhiteSpace(dataUrodz.Text) && !String.IsNullOrWhiteSpace(telefon.Text) && !String.IsNullOrWhiteSpace(adres.Text) && !String.IsNullOrWhiteSpace(pesel.Text) && !String.IsNullOrWhiteSpace(plec.Text) && !String.IsNullOrWhiteSpace(podstawaUbez.Text) && historia.authorised != null && historia.wywiad != null)
                {
                    historia.poradnia = poradnia.Text;
                    historia.nr_karty = Convert.ToDecimal(nrKarty.Text);
                    historia.data_zarejestrowania = DateTime.Parse(dataZarej.Text);
                    historia.pacjent.nazwisko = nazwisko.Text;
                    historia.pacjent.imie = imie.Text;
                    historia.pacjent.data_urodzenia = DateTime.Parse(dataUrodz.Text);
                    historia.pacjent.telefon = telefon.Text;
                    historia.pacjent.adres = new AdresType() { ulica = ulicaPacjent.Text, numer = nrPacjent.Text, kod_pocztowy = kodPocztowyPacjent.Text, miasto = miastoPacjent.Text };
                    historia.pacjent.pesel = pesel.Text;
                    historia.pacjent.plec = (PlecType)Enum.Parse(typeof(PlecType), plec.Text);
                    historia.podstawa_ubezpieczenia = podstawaUbez.Text;
                    historia.komentarz = komentarz.Text;
                    TextWriter writer = new StreamWriter(path);
                    xmlSerializer.Serialize(writer, historia);
                    MessageBox.Show("plik zapisany");
                    
                }
                else
                {
                    MessageBox.Show("Wypełnij wszystkie pola");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            dodajOsoba.IsEnabled = true;
            edytujOsoba.IsEnabled = false;
            imieINazwisko.Text = null;
            ulica.Text = null;
            nr.Text = null;
            kodPocztowy.Text = null;
            miasto.Text = null;
            telefonOsoba.Text = null;
            adresOsoba.Text = null;
        }

        private void utworz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                poradnia.Text = null;
                nrKarty.Text = null;
                nazwisko.Text = null;
                imie.Text = null;
                dataUrodz.Text = null;
                telefon.Text = null;
                ulicaPacjent.Text = null;
                nrPacjent.Text = null;
                kodPocztowyPacjent.Text = null;
                pesel.Text = null;
                plec.Text = null;
                podstawaUbez.Text = null;
                
                
                dataWizyty.Text = null;
                rozpoznanieIcd10.Text = null;
                rozpoznanie.Text = null;
                nazwiskoLekarza.Text = null;
                

              
                imieINazwisko.Text = null;
                adresOsoba.Text = null;
                DataDowodPesel.Text = null;
                telefonOsoba.Text = null;
                

                poradnia.IsEnabled = true;
                nrKarty.IsEnabled = true;
                dataZarej.IsEnabled = true;
                calendar.IsEnabled = true;
                calendar_Copy.IsEnabled = true;
                nazwisko.IsEnabled = true;
                imie.IsEnabled = true;
                dataUrodz.IsEnabled = true;
                telefon.IsEnabled = true;
                adres.IsEnabled = true;
                pesel.IsEnabled = true;
                plec.IsEnabled = true;
                podstawaUbez.IsEnabled = true;
                komentarz.Text = "Oswiadczam, ze osoba upowazniona do otrzymania informacji o stanie zdrowia i udzielonych swiadczeniach zdrowotnych oraz do uzyskania kopii dokumentacji medycznej, rowniez w przypadku mojej smierci jest: ";
                komentarz.IsEnabled = true;
                choroby.IsEnabled = true;
                imieINazwisko.IsEnabled = true;
                adresOsoba.IsEnabled = true;
                DataDowodPesel.IsEnabled = true;
                urodzinKalendarz.IsEnabled = true;
                telefonOsoba.IsEnabled = true;
                dodajOsoba.IsEnabled = true;
                zapiszJako.IsEnabled = true;

                historia = null;
                historia = new Historia();
                historia.authorised = new AuthorizedUpowazniony[0];
                historia.wywiad = new ChorobyChoroba[0];
                historia.pacjent = new Dane_pacjenta();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void okPacjent_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(ulicaPacjent.Text) && !String.IsNullOrWhiteSpace(nrPacjent.Text) && !String.IsNullOrWhiteSpace(kodPocztowyPacjent.Text) && !String.IsNullOrWhiteSpace(miastoPacjent.Text))
            {
                adres.Text = ulicaPacjent.Text + " " + nrPacjent.Text + " " + kodPocztowyPacjent.Text + " " + miastoPacjent.Text;
                oknoAdresPacjent.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Wypełnij wszystkie pola");
            }
        }

        private void adres_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            oknoAdresPacjent.IsOpen = true;
        }

        private void calendar_Copy_MouseUp(object sender, MouseButtonEventArgs e)
        {
            groupBox2_Copy.Visibility = Visibility.Visible;
        }

        private void kalenardz_Copy_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var calendarWindow = sender as System.Windows.Controls.Calendar;

                // ... See if a date is selected.
                if (calendarWindow.SelectedDate.HasValue)
                {
                    // ... Display SelectedDate in Title.
                    DateTime date = calendarWindow.SelectedDate.Value;
                    dataUrodz.Text = date.ToShortDateString();
                    groupBox2_Copy.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void zapiszJako_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(poradnia.Text) && !String.IsNullOrWhiteSpace(nrKarty.Text) && !String.IsNullOrWhiteSpace(dataZarej.Text) && !String.IsNullOrWhiteSpace(nazwisko.Text) && !String.IsNullOrWhiteSpace(imie.Text) && !String.IsNullOrWhiteSpace(dataUrodz.Text) && !String.IsNullOrWhiteSpace(telefon.Text) && !String.IsNullOrWhiteSpace(adres.Text) && !String.IsNullOrWhiteSpace(pesel.Text) && !String.IsNullOrWhiteSpace(plec.Text) && !String.IsNullOrWhiteSpace(podstawaUbez.Text) && historia.authorised != null && historia.wywiad != null)
                {
                    historia.poradnia = poradnia.Text;
                    historia.nr_karty = Convert.ToDecimal(nrKarty.Text);
                    historia.data_zarejestrowania = DateTime.Parse(dataZarej.Text);
                    historia.pacjent.nazwisko = nazwisko.Text;
                    historia.pacjent.imie = imie.Text;
                    historia.pacjent.data_urodzenia = DateTime.Parse(dataUrodz.Text);
                    historia.pacjent.telefon = telefon.Text;
                    historia.pacjent.adres = new AdresType() { ulica = ulicaPacjent.Text, numer = nrPacjent.Text, kod_pocztowy = kodPocztowyPacjent.Text, miasto = miastoPacjent.Text };
                    historia.pacjent.pesel = pesel.Text;
                    historia.pacjent.plec = (PlecType)Enum.Parse(typeof(PlecType), plec.Text);
                    historia.podstawa_ubezpieczenia = podstawaUbez.Text;
                    historia.komentarz = komentarz.Text;
                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Text files (*.xml)|*.xml";
                    
                    saveFileDialog.CheckPathExists = true;
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        path = saveFileDialog.InitialDirectory+saveFileDialog.FileName;
                        TextWriter writer = new StreamWriter(path);
                        xmlSerializer = new XmlSerializer(typeof(Historia));
                        xmlSerializer.Serialize(writer, historia);
                        MessageBox.Show("plik zapisany");
                        otworz.IsEnabled = false;
                        utworz.IsEnabled = false;

                        poradnia.IsEnabled = false;
                        nrKarty.IsEnabled = false;
                        dataZarej.IsEnabled = false;
                        nazwisko.IsEnabled = false;
                        imie.IsEnabled = false;
                        dataUrodz.IsEnabled = false;
                        telefon.IsEnabled = false;
                        adres.IsEnabled = false;
                        pesel.IsEnabled = false;
                        plec.IsEnabled = false;
                        podstawaUbez.IsEnabled = false;
                        choroby.IsEnabled = false;
                        listView.IsEnabled = false;
                        imieINazwisko.IsEnabled = false;
                        adresOsoba.IsEnabled = false;
                        DataDowodPesel.IsEnabled = false;
                        urodzinKalendarz.IsEnabled = false;
                        dodajOsoba.IsEnabled = false;
                        edytujOsoba.IsEnabled = false;
                        reset.IsEnabled = false;
                        printPDF.Visibility = Visibility.Visible;
                        zapiszJako.IsEnabled = false;
                        printPDF.IsEnabled = true;

                    }
                }
                else
                {
                    MessageBox.Show("Wypełnij wszystkie pola");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void printPDF_Click(object sender, RoutedEventArgs e)
        {
            saveFileDialogPdf = new SaveFileDialog();
            saveFileDialogPdf.Filter = "Text files (*.pdf)|*.pdf";

            saveFileDialogPdf.CheckPathExists = true;
            if (saveFileDialogPdf.ShowDialog() == true)
            {
                Document doc = new Document(PageSize.LETTER, 100, 100, 42, 35);
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(saveFileDialogPdf.InitialDirectory+saveFileDialogPdf.FileName, FileMode.Create));
                doc.Open();


                Paragraph paragraph = new Paragraph("HISTORIA ZDROWIA I CHOROBY");
                paragraph.Font.IsBold();
                paragraph.Alignment = Element.ALIGN_CENTER;
                paragraph.Font.Size = 16;
                doc.Add(paragraph);
                doc.Add(Chunk.NEWLINE);
                doc.Add(Chunk.NEWLINE);

                paragraph = new Paragraph("pieczątka zakładu");
                paragraph.Font.Size = 8;
                doc.Add(paragraph);

                paragraph = new Paragraph("PORADNIA  "+historia.poradnia);
                paragraph.Font.IsBold();
                paragraph.Alignment = Element.ALIGN_CENTER;
                paragraph.Font.Size = 14;
                doc.Add(paragraph);

                paragraph = new iTextSharp.text.Paragraph("Nr Karty  "+historia.nr_karty);
                paragraph.Alignment = Element.ALIGN_RIGHT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                paragraph = new iTextSharp.text.Paragraph("Data Zarejestrowania  "+historia.data_zarejestrowania);
                paragraph.Alignment = Element.ALIGN_RIGHT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                doc.Add(Chunk.NEWLINE);

                paragraph = new iTextSharp.text.Paragraph("Nazwisko  "+historia.pacjent.nazwisko);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(new Phrase("Imie  "+historia.pacjent.imie));
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);


                paragraph = new Paragraph("Data urodzenia  "+historia.pacjent.data_urodzenia);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(new Phrase("Telefon  "+historia.pacjent.telefon));
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                paragraph = new Paragraph("Adres  "+historia.pacjent.adres.ulica+" "+historia.pacjent.adres.numer+" "+historia.pacjent.adres.kod_pocztowy+" "+historia.pacjent.adres.miasto );
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                paragraph = new Paragraph("Pesel  "+historia.pacjent.pesel);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(new Phrase("Plec  "+historia.pacjent.plec));
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                paragraph = new iTextSharp.text.Paragraph("Podstawa ubezpieczenia  "+historia.podstawa_ubezpieczenia);
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                doc.Add(Chunk.NEWLINE);

                paragraph = new iTextSharp.text.Paragraph("Oswiadczam, ze osoba upowazniona do otrzymania informacji o stanie zdrowia i udzielonych swiadczeniach zdrowotnych oraz do uzyskania kopii dokumentacji medycznej, rowniez w przypadku mojej smierci jest:");
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font.Size = 12;
                doc.Add(paragraph);

                doc.Add(Chunk.NEWLINE);

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                Phrase phrase = new Phrase("Imie i Nazwisko");
                phrase.Font.IsBold();
                PdfPCell pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);
                phrase = new Phrase("Adres zamieszkania z kodem pocztowym oraz Pesel lub data urodzenia lub nr dowodu osobistego");
                phrase.Font.IsBold();
                pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);
                phrase = new Phrase("Nr telefonu");
                phrase.Font.IsBold();
                pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);
                foreach(var osoba in historia.authorised)
                {
                    pdfPCell = new PdfPCell(new Phrase(osoba.imie_nazwisko));
                    table.AddCell(pdfPCell);
                    phrase = new Phrase(osoba.adres_upowaznionego.ulica+" "+ osoba.adres_upowaznionego.numer+" "+ osoba.adres_upowaznionego.kod_pocztowy+" "+ osoba.adres_upowaznionego.miasto);
                    phrase.Add(Chunk.NEWLINE);
                    phrase.Add(new Phrase(osoba.Item));
                    pdfPCell = new PdfPCell(phrase);
                    table.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(osoba.nr_telefonu));
                    table.AddCell(pdfPCell);
                }
                
                doc.Add(table);
                doc.Add(Chunk.NEWLINE);


                paragraph = new iTextSharp.text.Paragraph("Data");
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add(Chunk.TABBING);
                paragraph.Add("Podpis pacjenta");
                doc.Add(paragraph);
                doc.Add(Chunk.NEWLINE);

                table = new PdfPTable(4);
                table.WidthPercentage = 100;
                phrase = new Phrase("Data");
                phrase.Font.IsBold();
                pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);
                phrase = new Phrase("Rozpoznanie ICD-10");
                phrase.Font.IsBold();
                pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);
                phrase = new Phrase("Rozpoznanie");
                phrase.Font.IsBold();
                pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);
                phrase = new Phrase("Nazwisko lekarza");
                phrase.Font.IsBold();
                pdfPCell = new PdfPCell(phrase);
                table.AddCell(pdfPCell);

                foreach(var choroba in historia.wywiad)
                {
                    pdfPCell = new PdfPCell(new Phrase(choroba.data_wizyty.ToString()));
                    table.AddCell(pdfPCell);
                    phrase = new Phrase(choroba.rozpoznanie_icd10);
                    pdfPCell = new PdfPCell(phrase);
                    table.AddCell(pdfPCell);
                    phrase = new Phrase(choroba.rozpoznanie);
                    pdfPCell = new PdfPCell(phrase);
                    table.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(choroba.nazwisko_lekarza));
                    table.AddCell(pdfPCell);
                }

                doc.Add(table);



                doc.Close();
                MessageBox.Show("Dokument utworzony");
            }
            }
    }
}
