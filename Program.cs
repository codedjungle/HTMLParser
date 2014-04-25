using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Data.SQLite;
using System.Data;
using OpenQA.Selenium.Remote;
using System.Collections.ObjectModel;
 

namespace WebDriverTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the Chrome Driver
        
            using (var driver = new ChromeDriver())
            {
                // Go to the home page
                driver.Navigate().GoToUrl("http://www.amazon.com/s/ref=lp_2368343011_nr_n_0?rh=n%3A1036592%2Cn%3A%211036682%2Cn%3A1040660%2Cn%3A2368343011%2Cn%3A2368365011&bbn=2368343011&ie=UTF8&qid=1397584947&rnid=2368343011");
               
                bool  pagnNextLink = true;
                #region sample to input a form
                // Get User Name field, Password field and Login Button
                //var userNameField = driver.FindElementById("usr");
                //var userPasswordField = driver.FindElementById("pwd");
                //var loginButton = driver.FindElementByXPath("//input[@value='Login']");

                //// Type user name and password
                //userNameField.SendKeys("admin");
                //userPasswordField.SendKeys("12345");

                //// and click the login button
                //loginButton.Click(); 
                #endregion

                ReadOnlyCollection<IWebElement>  products = driver.FindElementsByClassName("newaps");

                SQLiteDatabase db = new SQLiteDatabase();
                db.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS products (id integer primary key, 
                            product varchar(1024), url varchar(1024), price varchar(16), sku varchar(64), description,
                            attribute_key_value_json varchar(2048) , crawled_on datetime

                    )");
                int page = 1;
                Int64 total_products = 0;
                total_products += products.Count;

                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("------------------- Page {0} , Products in page {1}, Total Products {2} ",page, products.Count, total_products);
                Console.WriteLine("---------------------------------------------");


                do
                {
                      
                    
                    int index = 0;
                    foreach (IWebElement product in products)
                    {

                        IList<IWebElement> prices = (IList<IWebElement>)driver.FindElementsByCssSelector(".bld.lrg.red,.price.bld");
                        IList<IWebElement> names = (IList<IWebElement>)driver.FindElementsByCssSelector(".prod.celwidget");
                        IWebElement anchor = product.FindElement(By.TagName("a"));                       
                        
                        Dictionary<String, String> data = new Dictionary<String, String>();
                        
                        data.Add("product", product.Text.Replace("'", "''"));
                        data.Add("price", prices[index].Text.Replace("'", "''"));
                        data.Add("sku", names[index].GetAttribute("name").Replace("'", "''"));
                        data.Add("url", anchor.GetAttribute("href").Replace("'", "''")); 
                        
                        db.Insert("products", data);
                        Console.WriteLine(names[index].GetAttribute("name").Replace("'", "''"));

                        index++;
                    }
                    total_products += products.Count;
                    if (driver.FindElementById("pagnNextLink") != null)
                    {
                        //driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
                        string _nextUrl = driver.FindElementById("pagnNextLink").GetAttribute("href");
                        driver.Navigate().GoToUrl(_nextUrl);

                        
                        products = null;
                        products = driver.FindElementsByClassName("newaps");
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        wait.Until(i => i.FindElement(By.Id("footer")));
                        page++;
                    }
                    else{
                        pagnNextLink = false;
                    }

                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine("------------------- Page {0} , Products in page {1}, Total Products {2} ",page, products.Count, total_products);
                    Console.WriteLine("---------------------------------------------");

                }
                while (pagnNextLink);

               


                Console.WriteLine("Total products : {0},", products.Count);




                // Extract resulting message and save it into result.txt
                //var result = driver.FindElementByXPath("//div[@id='case_login']/h3").Text;
                //sFile.WriteAllText("result.txt", result);

                // Take a screenshot and save it into screen.png
                //driver.GetScreenshot().SaveAsFile(@"screen.png", ImageFormat.Png);
            }
            Console.ReadKey();
        }
    }
}
