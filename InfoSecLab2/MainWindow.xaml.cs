using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfoSecLab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly char[,] ruLettersLowerCaseMatrix = new char[6, 5] 
        {
            { 'а', 'б', 'в', 'г', 'д' },
            { 'е', 'ж', 'з', 'и', 'к' },
            { 'л', 'м', 'н', 'о', 'п' },
            { 'р', 'с', 'т', 'у', 'ф' },
            { 'х', 'ц', 'ч', 'ш', 'щ' },
            { 'ы', 'ь', 'э', 'ю', 'я' }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openInputFileDialog = new();
            openInputFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openInputFileDialog.ShowDialog() == true)
            {
                string inputText = System.IO.File.ReadAllText(openInputFileDialog.FileName);
                inputText = inputText.ToLower();
                List<int> encryptedInNumbersText = new List<int>();

                //Шифрование. Каждой букве в тексте ставится в соответствие два числа: номер строки и столбца в матрице.
                //Получившиеся числа записываются в encryptedInNumbersText
                foreach (char letter in inputText)
                {
                    if (letter == ' ')
                    {
                        encryptedInNumbersText.Add(0);
                    }
                    else
                    {
                        for (int i = 0; i < ruLettersLowerCaseMatrix.GetLength(0); i++)
                        {
                            for (int j = 0; j < ruLettersLowerCaseMatrix.GetLength(1); j++)
                            {
                                if (letter == ruLettersLowerCaseMatrix[i, j])
                                {
                                    encryptedInNumbersText.Add(i+1);
                                    encryptedInNumbersText.Add(j+1);
                                }
                            }
                        }
                    }
                }

                //Вывод зашифрованного чисел из encryptedInNumbersText в битовое представление
                //Каждое число записывается в виде последовательности трех

                //Выясление и установка количества битов в зашифрованном тексте
                BitArray bits = new BitArray(3*encryptedInNumbersText.Count);
                //if (bits.Length % 8 != 0) { bits.Length += 8 - bits.Length % 8; }

                //Заполнение битового массива
                int index = 0;
                foreach (int number in encryptedInNumbersText)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        bool bit = ((number >> i) & 1) == 1;
                        bits[index++] = bit;
                    }
                }

                //Сохранение битового массива в бинарном виде в файл
                SaveFileDialog saveOutputFileDialog = new();
                saveOutputFileDialog.FileName = "Encrypted.txt";
                saveOutputFileDialog.Filter = "File|*|All files (*.*)|*.*";
                if (saveOutputFileDialog.ShowDialog() == true)
                {
                    byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
                    bits.CopyTo(ret, 0);
                    File.WriteAllBytes(saveOutputFileDialog.FileName, ret);
                }

            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            //
            OpenFileDialog openInputFileDialog = new();
            openInputFileDialog.Filter = "File|*|All files (*.*)|*.*";
            if (openInputFileDialog.ShowDialog() == true)
            {
                byte[] ret = File.ReadAllBytes(openInputFileDialog.FileName);
                BitArray bits = new BitArray(8*ret.Length);

                //перевод массива байтов в массив битов
                int i_bits = 0; //index for bits
                int j_ret = 0; //index for ret
                while (true)
                {
                    if (j_ret >= ret.Length) { break; }
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 1);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 2);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 4);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 8);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 16);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 32);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 64);
                    i_bits++;
                    bits[i_bits] = Convert.ToBoolean(ret[j_ret] & 128);
                    i_bits++;
                    j_ret++;
                }

                //чтение трех битов из массива bits
                //перевод трех битов в число, которое записывается в decryptedInNumbersText
                List<int> decryptedInNumbersText = new();
                for (int i = 0; i < bits.Length; i += 3)
                {
                    int value = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        if (i + j < bits.Length)
                        {
                            value |= (bits[i + j] ? 1 : 0) << j;
                        }
                    }
                    decryptedInNumbersText.Add(value);
                }

                //расшифрование чисел из decryptedInNumbersText по таблице
                string outputText = "";
                for (int n = 0; n < decryptedInNumbersText.Count; n++)
                {
                    if (decryptedInNumbersText[n] == 0)
                    {
                        outputText += " ";
                    }
                    else
                    {
                        outputText += ruLettersLowerCaseMatrix[decryptedInNumbersText[n] - 1, decryptedInNumbersText[n + 1] - 1];
                        n++;
                    }
                }

                //сохранение расшифрованного текста в файл
                SaveFileDialog saveOutputFileDialog = new();
                saveOutputFileDialog.FileName = "Decrypted";
                saveOutputFileDialog.Filter = "\"Text files (*.txt)|*.txt|All files (*.*)|*.*\"";
                if (saveOutputFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveOutputFileDialog.FileName, outputText);
                }
            }
        }
    }
}