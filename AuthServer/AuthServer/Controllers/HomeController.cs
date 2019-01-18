using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Models;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Html;

namespace AuthServer.Controllers
{
    public class HomeController : Controller
    {
        private string URL = "https://proxyserverproxyserver5.azurewebsites.net/";//"https://localhost:44370/";//
        UserManager<IdentityUser> _userManager;

        public HomeController(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
        }

        public async Task<IActionResult> Index()
        {
            await PostRequestAsyncMyCalculations();
            if (!string.IsNullOrEmpty(myCalculationsResponce) && myCalculationsResponce != "500")
            {
                var deserializeObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fractal>>(myCalculationsResponce);
                return View(deserializeObj);
            }

            return View(new List<Fractal>());
        }

        /*public async Task<IActionResult> LoadMyData()
        {
            await PostRequestAsyncMyCalculations();
            ViewBag.myData = myCalculationsResponce;
            return View();
        }*/

        public IActionResult About()
        {
            ViewData["Message"] = "Здесь вы можете получить данные с сервера.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Здесь вы можете отправить запрос на расчёт.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Contact(string alpha, string beta, string gamma)
        {
            await PostRequestAsyncCalculate(alpha, beta, gamma);
            switch(responseStr)
            {
                case "200":
                    return Content("Ваши данные успешно отправлены на сервер, в скором времени будет закончен расчёт, а пока попейте чаю");
                case "500":
                    return Content("Что-то пошло не так. Call Mikhail or Egor quickly");
                case "210":
                    return Content("Данный расчёт уже хранится на сервере, перейдите во вкладку \"Получить\", чтобы загрузить результаты.");

            }
            return Content(responseStr);
        }

        [HttpPost]
        public async Task<IActionResult> About(string alpha, string beta, string gamma)
        {
            await PostRequestAsyncGet(alpha, beta, gamma);
            if (string.IsNullOrEmpty(responseStr) || responseStr == "500")
            {
                return View("HaveNoImage");
            }
            var fractalObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Fractal>(responseStr);
            return View("ImageUrlShow", new List<Fractal> { fractalObj });
        }

        private string responseStr;
        private async Task PostRequestAsyncCalculate(string alpha, string beta, string gamma)
        {
            WebRequest request = WebRequest.Create(URL + "home/Calculate"); //WebRequest.Create("https://localhost:44311/Home/PostData");
            request.Method = "POST"; // для отправки используется метод Post         
            string data = $"id={_userManager.GetUserName(User)}&alpha={alpha}&beta={beta}&gamma={gamma}"; // данные для отправки
            // преобразуем данные в массив байтов
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            // устанавливаем тип содержимого - параметр ContentType
            request.ContentType = "application/x-www-form-urlencoded";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            request.ContentLength = byteArray.Length;

            //записываем данные в поток запроса
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            response.Close();
            Console.WriteLine("Запрос выполнен...");
        }

        private async Task PostRequestAsyncGet(string alpha, string beta, string gamma)
        {
            WebRequest request = WebRequest.Create(URL + "home/FractalInfo"); //WebRequest.Create("https://localhost:44311/Home/PostData");
            request.Method = "POST"; // для отправки используется метод Post                                     
            string data = $"alpha={alpha}&beta={beta}&gamma={gamma}"; // данные для отправки
            // преобразуем данные в массив байтов
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            // устанавливаем тип содержимого - параметр ContentType
            request.ContentType = "application/x-www-form-urlencoded";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            request.ContentLength = byteArray.Length;

            //записываем данные в поток запроса
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            response.Close();
            Console.WriteLine("Запрос выполнен...");
        }

        private string myCalculationsResponce;
        private async Task PostRequestAsyncMyCalculations()
        {
            WebRequest request = WebRequest.Create(URL + "home/UserCalculations");
            request.Method = "POST"; // для отправки используется метод Post                                     
            string data = $"id={_userManager.GetUserName(User)}"; // данные для отправки
            // преобразуем данные в массив байтов
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            // устанавливаем тип содержимого - параметр ContentType
            request.ContentType = "application/x-www-form-urlencoded";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            request.ContentLength = byteArray.Length;

            //записываем данные в поток запроса
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    myCalculationsResponce = reader.ReadToEnd();
                }
            }
            response.Close();
            Console.WriteLine("Запрос выполнен...");
        }

        private async Task Test()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://damp-forest-33235.herokuapp.com/getfrac");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"a\":\"-0.23\"," +
                              "\"b\":\"-0.76\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}
