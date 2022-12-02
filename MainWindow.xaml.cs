using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
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
using ShopDesktop.interfaces;

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

        //Create a record for storing the user information.
        record LoginParam(string username, string password);

        private void CheckLoginInfo(object sender, RoutedEventArgs e)
        {
            //Get entered user information and store it into a new record.
            var user = new LoginParam(UserTextBox.Text, PwdTextBox.Password);
         
            //Create a web request.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "login.php");
            request.Method = HttpMethod.Post.ToString();
            request.ContentType = "application/json";

            //Create the data for the request parameters.
            var postData = JsonSerializer.Serialize(user);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //Get the request stream and write the data in the request.
            Stream dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                //Get the response from the REST api
                //and print the relevant part from the received JSON message.
                var webResponse = (HttpWebResponse)request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                responseReader.Close();
                string[] split = response.Split(':',2);
                string sub = split[1].Substring(1);
                string[] final = sub.Split('"', 2);
                if (final[0] == "Invalid input" || final[0] == "Invalid username or password"
                    || final[0] == "Invalid request")
                {
                    ResponseBox.Text = final[0];
                } else {
                    //If the entered information is correct,
                    //then we send a new request to define the user role.
                    //(Only for the purposes of hiding irrelevant buttons
                    //from the UI.)
                    ResponseBox.Text = "";
                    string token = final[0];
                    DataInterface.tkn = final[0];
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                    try
                    {
                        //Since getting users is only allowed for admins,
                        //we can define the role by analyzing the response.
                        var resp = client.GetAsync(url + "controller/user_controller.php");
                        string respString = resp.Result.Content.ReadAsStringAsync().Result;
                        split = respString.Split(':', 2);
                        sub = split[1].Substring(1);
                        final = sub.Split('"', 2);
                        if (final[0] == "This needs admin role")
                        {
                            DataInterface.role = "user";
                            ShopWindow shop = new ShopWindow();
                            shop.Show();
                            this.Close();
                        } else {
                            DataInterface.role = "admin";
                            ShopWindow shop = new ShopWindow();
                            shop.Show();
                            this.Close();
                        }                 
                    }
                    catch (Exception e2)
                    {
                        Trace.WriteLine("-----------------");
                        Trace.WriteLine(e2.Message);
                    }
                }
                
            }
            catch (Exception e1)
            {
                Trace.WriteLine("-----------------");
                Trace.WriteLine(e1.Message);
            }
        }

        //Check if password textbox has something written in it,
        //whenever the user writes something in the username textbox.
        //If yes, enable the submit button.
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

        //Check if username textbox has something written in it,
        //whenever the user writes something in the password textbox.
        //If yes, enable the submit button.
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
