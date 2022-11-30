using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string url = "https://hmlsolutions.com/cloud11/PROJEKTI/";
        public MainWindow()
        {
            InitializeComponent();
            SubmitBtn.IsEnabled = false;
        }

        void CheckLoginInfo(object sender, RoutedEventArgs e)
        {
            string uname = UserTextBox.Text;
            string pwd = PwdTextBox.Password;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "login.php?" +
                "username=" + uname + "&password=" + pwd);
            request.Method = HttpMethod.Post.ToString();
            //request.ContentType = "application/json";

            //string postData = WebUtility.UrlEncode("username") + "=" + uname + "&" +
            //WebUtility.UrlEncode("password") + "=" + pwd;
            //byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //Here we get the request stream.
            Stream dataStream = request.GetRequestStream();

            //Here we write the data to the request stream.
            //dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                var webResponse = (HttpWebResponse)request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                //ResponseBox.Text = response;
                responseReader.Close();
                string[] split = response.Split(':',2);
                string sub = split[1].Substring(1);
                string[] final = sub.Split('"', 2);
                if (final[0] == "Invalid input" || final[0] == "Invalid username or password"
                    || final[0] == "Invalid request")
                {
                    ResponseBox.Text = final[0];
                } else {
                    ResponseBox.Text = "";
                }
            }
            catch (Exception e1)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e1.Message);
            }
        }

        private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string pwdtext = PwdTextBox.Password;
            if (pwdtext != "" && UserTextBox.Text != "")
            {
                SubmitBtn.IsEnabled = true;
            } else {
                SubmitBtn.IsEnabled = false;
            }
        }

        private void PwdTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string pwdtext = PwdTextBox.Password;
            if (UserTextBox.Text != "" && pwdtext != "")
            {
                SubmitBtn.IsEnabled = true;
            } else {
                SubmitBtn.IsEnabled = false;
            }
        }
    }
}
