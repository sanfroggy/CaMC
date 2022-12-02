using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
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
using ShopDesktop.interfaces;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Data.Common;
using System.Configuration;
using System.Net;
using System.IO;

namespace ShopDesktop
{
    /// <summary>
    /// Interaction logic for ShopWindow.xaml
    /// </summary>
    public partial class ShopWindow : Window
    {
        //Create relevant variables.
        private const string url = "https://hmlsolutions.com/cloud11/PROJEKTI/";
        private string role = "";
        public ShopWindow()
        {
            //Hide all currently unrelevant content.
            InitializeComponent();
            UpdateLbl.Visibility = Visibility.Collapsed;
            UpdateProdIdLbl.Visibility = Visibility.Collapsed;
            UpdateProdId.Visibility = Visibility.Collapsed;
            UpdateNameLbl.Visibility = Visibility.Collapsed;
            UpdateProdName.Visibility = Visibility.Collapsed;
            UpdateDescrLbl.Visibility = Visibility.Collapsed;
            UpdateProdDescr.Visibility = Visibility.Collapsed;
            UpdateImgLbl.Visibility = Visibility.Collapsed;
            UpdateProdImg.Visibility = Visibility.Collapsed;
            UpdateAmountLbl.Visibility = Visibility.Collapsed;
            UpdateProdAmount.Visibility = Visibility.Collapsed;
            WarningLblAmount.Visibility = Visibility.Collapsed;
            UpdatePriceLbl.Visibility = Visibility.Collapsed;
            UpdateProdPrice.Visibility = Visibility.Collapsed;
            WarningLblPrice.Visibility = Visibility.Collapsed;
            SaveUpdateBtn.Visibility = Visibility.Collapsed;
            SaveCreateBtn.Visibility = Visibility.Collapsed;
            role = DataInterface.role;
            if (role.Equals("user"))
            {
                DeleteProdBtn.Visibility = Visibility.Collapsed;
                UpdateProdBtn.Visibility = Visibility.Collapsed;
                CreateProdBtn.Visibility = Visibility.Collapsed;
            }
        }

        private string token = "";

        record ProductView(int id, string name, string description, string img,
            int amount, double price);

        record ProductUpdate(int id, string name, string description, string img_url,
            int amount, double price);

        record NewProduct(string name, string description, string img_url,
            int amount, double price);
        private void GetProducts(object sender, RoutedEventArgs e)
        {
            //Get token from interface and create a request to get a list of products.
            token = DataInterface.tkn;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
            try
            {
                //Get the list of products as a response and show them on the ListBox.
                var resp = client.GetAsync(url + "controller/product_controller.php");
                string respString = resp.Result.Content.ReadAsStringAsync().Result;
                List<ProductView> prodList = JsonSerializer.Deserialize<List<ProductView>>(respString);
                ProdListBox.ItemsSource = prodList;
            }
            catch (Exception e1)
            {
                Trace.WriteLine("-----------------");
                Trace.WriteLine(e1.Message);
            }
        }

        //Prevent user from entering characters to a field that requires numbers.
        private void UpdateProdAmount_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(UpdateProdAmount.Text, "[^0-9]"))
            {
                WarningLblAmount.Content = "Invalid input.";
                UpdateProdAmount.Text = UpdateProdAmount.Text.Remove(UpdateProdAmount.Text.Length - 1);
            }
            else
            {
                WarningLblAmount.Content = "";
            }
        }

        //Prevent user from entering characters to a field that requires numbers.
        private void UpdateProdPrice_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(UpdateProdPrice.Text, "[^0-9.,]"))
            {
                WarningLblPrice.Content = "Invalid input.";
                UpdateProdPrice.Text = UpdateProdPrice.Text.Remove(UpdateProdPrice.Text.Length - 1);
            }
            else
            {
                WarningLblAmount.Content = "";
            }
        }

        private void ShowUpdateFields(object sender, RoutedEventArgs e)
        {
            //If a product is selected from the list show the field necessary for updating it
            //and fill the fields with it's values.
            if (ProdListBox.SelectedItem != null)
            {
                UpdateProdId.IsReadOnly = false;
                UpdateProdName.IsReadOnly = false;
                UpdateProdDescr.IsReadOnly = false;
                UpdateProdImg.IsReadOnly = false;
                UpdateProdAmount.IsReadOnly = false;
                UpdateProdPrice.IsReadOnly = false;

                UpdateLbl.Content = "Enter new information:";
                UpdateLbl.Visibility = Visibility.Visible;
                UpdateProdId.Visibility = Visibility.Collapsed;
                UpdateProdId.Visibility = Visibility.Hidden;
                UpdateNameLbl.Visibility = Visibility.Visible;
                UpdateProdName.Visibility = Visibility.Visible;
                UpdateDescrLbl.Visibility = Visibility.Visible;
                UpdateProdDescr.Visibility = Visibility.Visible;
                UpdateImgLbl.Visibility = Visibility.Visible;
                UpdateProdImg.Visibility = Visibility.Visible;
                UpdateAmountLbl.Visibility = Visibility.Visible;
                UpdateProdAmount.Visibility = Visibility.Visible;
                WarningLblAmount.Visibility = Visibility.Visible;
                UpdatePriceLbl.Visibility = Visibility.Visible;
                UpdateProdPrice.Visibility = Visibility.Visible;
                WarningLblPrice.Visibility = Visibility.Visible;
                SaveCreateBtn.Visibility = Visibility.Collapsed;
                SaveUpdateBtn.Visibility = Visibility.Visible;

                ProductView selprod = (ProductView)ProdListBox.SelectedItem;
                UpdateProdId.Text = selprod.id.ToString();
                UpdateProdName.Text = selprod.name;
                UpdateProdDescr.Text = selprod.description;
                UpdateProdImg.Text = selprod.img;
                UpdateProdAmount.Text = selprod.amount.ToString();
                UpdateProdPrice.Text = selprod.price.ToString();
            }
        }

        private void ShowViewFields(object sender, RoutedEventArgs e)
        {
            //If a product is selected from the list show the product information in
            //corresponding fields.
            if (ProdListBox.SelectedItem != null)
            {
                UpdateProdId.IsReadOnly = true;
                UpdateProdName.IsReadOnly = true;
                UpdateProdDescr.IsReadOnly = true;
                UpdateProdImg.IsReadOnly = true;
                UpdateProdAmount.IsReadOnly = true;
                UpdateProdPrice.IsReadOnly = true;

                UpdateLbl.Content = "Product information:";
                UpdateLbl.Visibility = Visibility.Visible;
                UpdateProdId.Visibility = Visibility.Visible;
                UpdateProdIdLbl.Visibility = Visibility.Visible;
                UpdateNameLbl.Visibility = Visibility.Visible;
                UpdateProdName.Visibility = Visibility.Visible;
                UpdateDescrLbl.Visibility = Visibility.Visible;
                UpdateProdDescr.Visibility = Visibility.Visible;
                UpdateImgLbl.Visibility = Visibility.Visible;
                UpdateProdImg.Visibility = Visibility.Visible;
                UpdateAmountLbl.Visibility = Visibility.Visible;
                UpdateProdAmount.Visibility = Visibility.Visible;
                WarningLblAmount.Visibility = Visibility.Visible;
                UpdatePriceLbl.Visibility = Visibility.Visible;
                UpdateProdPrice.Visibility = Visibility.Visible;
                WarningLblPrice.Visibility = Visibility.Visible;
                SaveCreateBtn.Visibility = Visibility.Collapsed;
                SaveUpdateBtn.Visibility = Visibility.Collapsed;

                ProductView selprod = (ProductView)ProdListBox.SelectedItem;
                UpdateProdId.Text = selprod.id.ToString();
                UpdateProdName.Text = selprod.name;
                UpdateProdDescr.Text = selprod.description;
                UpdateProdImg.Text = selprod.img;
                UpdateProdAmount.Text = selprod.amount.ToString();
                UpdateProdPrice.Text = selprod.price.ToString();
            }
        }

        private void ShowCreateFields(object sender, RoutedEventArgs e)
        {
            //Show the field necessary for creating a new product.
            UpdateProdId.IsReadOnly = false;
            UpdateProdName.IsReadOnly = false;
            UpdateProdDescr.IsReadOnly = false;
            UpdateProdImg.IsReadOnly = false;
            UpdateProdAmount.IsReadOnly = false;
            UpdateProdPrice.IsReadOnly = false;

            UpdateLbl.Content = "Enter product information:";
            UpdateProdId.Visibility = Visibility.Collapsed;
            UpdateProdIdLbl.Visibility = Visibility.Collapsed;
            UpdateLbl.Visibility = Visibility.Visible;
            UpdateNameLbl.Visibility = Visibility.Visible;
            UpdateProdName.Visibility = Visibility.Visible;
            UpdateDescrLbl.Visibility = Visibility.Visible;
            UpdateProdDescr.Visibility = Visibility.Visible;
            UpdateImgLbl.Visibility = Visibility.Visible;
            UpdateProdImg.Visibility = Visibility.Visible;
            UpdateAmountLbl.Visibility = Visibility.Visible;
            UpdateProdAmount.Visibility = Visibility.Visible;
            WarningLblAmount.Visibility = Visibility.Visible;
            UpdatePriceLbl.Visibility = Visibility.Visible;
            UpdateProdPrice.Visibility = Visibility.Visible;
            WarningLblPrice.Visibility = Visibility.Visible;
            SaveUpdateBtn.Visibility = Visibility.Collapsed;
            SaveCreateBtn.Visibility = Visibility.Visible;

            UpdateProdName.Text = "";
            UpdateProdDescr.Text = "";
            UpdateProdImg.Text = "";
            UpdateProdAmount.Text = "";
            UpdateProdPrice.Text = "";
        }

        private void DeleteProduct(object sender, RoutedEventArgs e)
        {
            //Check if a product is selected.
            if (ProdListBox.SelectedItem != null)
            {
                token = DataInterface.tkn;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
                try
                {
                    //Get the id of the currently selected product and send a DELETE request.
                    ProductView selprod = (ProductView)ProdListBox.SelectedItem;
                    var resp = client.DeleteAsync(url + "controller/product_controller.php?id=" + selprod.id);
                    string respString = resp.Result.Content.ReadAsStringAsync().Result;
                    ResponseBox.Text = respString;
                    GetProducts(sender, e);
                }
                catch (Exception e1)
                {
                    Trace.WriteLine("-----------------");
                    Trace.WriteLine(e1.Message);
                }
            }
        }

        private void UpdateProduct(object sender, RoutedEventArgs e)
        {
            token = DataInterface.tkn;
            //Create a web request.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "controller/product_controller.php");
            request.Method = HttpMethod.Put.ToString();
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + token);

            var prod = new ProductUpdate(Int32.Parse(UpdateProdId.Text), UpdateProdName.Text,
            UpdateProdDescr.Text, UpdateProdImg.Text, Int32.Parse(UpdateProdAmount.Text),
            Double.Parse(UpdateProdPrice.Text));
            var postData = JsonSerializer.Serialize(prod);
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
                Trace.WriteLine(postData);
                ResponseBox.Text = response;
                //Update the ListBox content.
                GetProducts(sender, e);
            }
            catch (Exception e1)
            {
                Trace.WriteLine("-----------------");
                Trace.WriteLine(e1.Message);
            }
        }

        private void CreateProduct(object sender, RoutedEventArgs e)
        {
            token = DataInterface.tkn;
            //Create a web request.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "controller/product_controller.php");
            request.Method = HttpMethod.Post.ToString();
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + token);

            var prod = new NewProduct(UpdateProdName.Text,
            UpdateProdDescr.Text, UpdateProdImg.Text, Int32.Parse(UpdateProdAmount.Text),
            Double.Parse(UpdateProdPrice.Text));
            var postData = JsonSerializer.Serialize(prod);
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
                Trace.WriteLine(postData);
                ResponseBox.Text = response;
                //Update the ListBox content.
                GetProducts(sender, e);
            }
            catch (Exception e1)
            {
                Trace.WriteLine("-----------------");
                Trace.WriteLine(e1.Message);
            }
        }
    }
}
