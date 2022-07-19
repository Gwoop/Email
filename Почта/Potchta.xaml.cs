using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Почта
{
    /// <summary>
    /// Логика взаимодействия для Potchta.xaml
    /// </summary>
    public partial class Potchta : Window
    {
        private List<string> mailsAlrady = new List<string>();
        private string login;
        private string password;

     

        public Potchta(string pas, string log)
        {
            InitializeComponent();
            login = log;
            password = pas;
            otkogo.Text += login;
            FontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            string[] lines = new string[8] { "0,5", "0,75", "1", "1,25", "1,5", "1,75", "2", "2,5" };
            for (int i = 0; i < lines.Length; i++)
            {
                LineSpacing.Items.Add(lines[i]);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) //добавить файл
        {
            OpenFileDialog a = new OpenFileDialog();
            a.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (a.ShowDialog() == true)
            {
                fayli.Items.Add(a.FileName);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) //добавление
        {
            if (chely.Text != "")
            {
                rassilka.Items.Add(chely.Text);
            }
            chely.Text = null;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            rassilka.Items.Clear();
        }
        public string path = @"user";
        private void Button_Click_3(object sender, RoutedEventArgs e) //обновляем список для нашего юзера
        {
            try
            {
                string path1 = path + @"\" + login + ".dat";

                using (BinaryWriter writer = new BinaryWriter(File.Open(path1, FileMode.OpenOrCreate)))
                {
                    for (int i = 0; i < vse.Items.Count; i++)
                    {
                        writer.Write((string)vse.Items[i]);
                    }
                    writer.Close();
                }
            }
            catch
            {

            }
        }
        private void vse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chely.Text = vse.SelectedItem.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string path1 = path + @"\" + login + ".dat";
            if (File.Exists(path1))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path1, FileMode.Open)))
                {
                    while (reader.PeekChar() > -1)
                    {
                        string name = reader.ReadString();
                        vse.Items.Add(name);
                    }
                    reader.Close();
                }
            }
            else
            File.Create(path1);
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(VVOD.Document.ContentStart, VVOD.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(VVOD.Document.ContentStart, VVOD.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }
        private void VVOD_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = VVOD.Selection.GetPropertyValue(Inline.FontWeightProperty);
            Bold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = VVOD.Selection.GetPropertyValue(Inline.FontStyleProperty);
            Italic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = VVOD.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            Underline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = VVOD.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamily.SelectedItem = temp;
            temp = VVOD.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSize.Text = temp.ToString();
        }
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamily.SelectedItem != null)
                VVOD.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamily.SelectedItem);
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                VVOD.Selection.ApplyPropertyValue(Inline.FontSizeProperty, FontSize.Text);
            }
            catch
            {

            }
        }
        private void LineSpacingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextSelection selectedText = VVOD.Selection;
            if (LineSpacing.SelectedItem.ToString() != "")
                selectedText.ApplyPropertyValue(TextBlock.LineHeightProperty,
                    Convert.ToDouble(LineSpacing.SelectedItem) * 12 - 2);

        }
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextSelection text = VVOD.Selection;
            Color c = (Color)e.NewValue;
            if (c != null)
                text.ApplyPropertyValue(TextElement.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom(c.ToString())));
        }
        private void ColorPicker_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextSelection text = VVOD.Selection;
            Color c = (Color)e.NewValue;
            if (c != null)
                text.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom(c.ToString())));
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {        
            if(chely.Text != null)
            rassilka.Items.Add(chely.Text);
            chely.Text = null;
            try
            {
                for (int i = 0; i < rassilka.Items.Count; i++)
                {
                    vse.Items.Add(rassilka.Items[i]);
                    string mailGO = Convert.ToString(rassilka.Items[i]);
                    // отправитель - устанавливаем адрес и отображаемое в письме имя
                    MailAddress from = new MailAddress(login, zagolovok.Text);
                    // кому отправляем
                    MailAddress to = new MailAddress(mailGO);
                    // создаем объект сообщения
                    MailMessage m = new MailMessage(from, to);
                    // тема письма
                    m.Subject = thame.Text;
                    // текст письма
                    m.IsBodyHtml = false;
                    TextRange range = new TextRange(VVOD.Document.ContentStart, VVOD.Document.ContentEnd);
                    m.Body = $"{range.Text}\n";
                    for (int z = 0; z < fayli.Items.Count; z++)
                    {
                        string fule = Convert.ToString(fayli.Items[z]);
                        m.Body += $"\n File: {z}: {fule}";
                    }
                    // адрес smtp-сервера и порт, с которого будем отправлять письмо
                    string str = "";
                    if (login.Contains("@gmail.com")) str = "smtp.gmail.com";
                    else if (login.Contains("@mail.ru")) str = "smtp.mail.ru";
                    SmtpClient smtp = new SmtpClient(str, 587);
                    // логин и пароль
                    smtp.Credentials = new NetworkCredential(login, password);
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                }
            }
            catch
            {

            }
         
            fayli.Items.Clear();
            zagolovok.Text = null;
            thame.Text = null;
            rassilka.Items.Clear();
        }

        private void fayli_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                fayli.Items.Add(files[0]);
            }
        }
    }
}
