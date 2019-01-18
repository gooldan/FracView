using Microsoft.AspNetCore.Mvc;
using ProxyServer.Data;
using ProxyServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: /store/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Calculate(string id, string alpha, string beta, string gamma)
        {
            try
            {
                var fractalOld = _db.Fractals.FirstOrDefault(x => x.alpha == alpha && x.beta == beta && x.gamma == gamma);
                if (fractalOld != null && fractalOld != default(Fractal))
                    return "210";

                /*var fractal = new Fractal();
                fractal.id = id;
                fractal.url = "google.com";
                fractal.alpha = alpha;
                fractal.beta = beta;
                fractal.gamma = gamma;
                _db.Fractals.Add(fractal);
                // сохраняем в бд все изменения
                _db.SaveChanges();*/
                PostRequestAsyncCalculate(id, alpha, beta, gamma);
                return "200";
            }
            catch(Exception e)
            {
                return "500";
            }
        }

        [HttpPost]
        public string FractalInfo(string alpha, string beta, string gamma)
        {
            var fractal = _db.Fractals.FirstOrDefault(x => x.alpha == alpha && x.beta == beta && x.gamma == gamma);
            if (fractal == null || fractal == default(Fractal))
            {
                return "500";                
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(fractal);
            }
        }

        [HttpPost]
        public string GetImageUrl(string alpha, string beta, string gamma)
        {
            var fractal = _db.Fractals.FirstOrDefault(x => x.alpha == alpha && x.beta == beta && x.gamma == gamma);
            if (fractal == null || fractal == default(Fractal))
            {
                return "https://coubsecure-s.akamaihd.net/get/b67/p/coub/simple/cw_timeline_pic/533a0dad81e/9eb6faf4cc41da688ba25/big_1475572434_image.jpg";
            }
            else
            {
                return fractal.url;
            }
        }

        [HttpPost]
        public string UserCalculations(string id)
        {
            var fractal = _db.Fractals.ToList().FindAll(x =>
            {
                if (x.id.Contains("|"))
                {
                    return x.id.Split('|')[0] == id;
                }
                else
                {
                    return x.id == id;
                }
            });
            if (fractal == null || fractal.Count == 0)
            {
                return "500";
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(fractal);
            }
        }

        [HttpPost]
        public string Buy()
        {
            //_db.Orders.Add(order);
            // сохраняем в бд все изменения
            //_db.SaveChanges();
            return "Спасибо, noname, за покупку!";
        }

        private class FractalRequest
        {
            public string a;
            public string b;
        }

        private class FractalResponce
        {
            public string supurl;
        }

        private static readonly HttpClient client = new HttpClient();
        private async Task PostRequestAsyncCalculate(string id, string alpha, string beta, string gamma)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://damp-forest-33235.herokuapp.com/getfrac");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"a\":\""+alpha.ToString()+"\"," +
                              "\"b\":\"" + beta.ToString() + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var resultStr = streamReader.ReadToEnd();
                try
                {
                    var parseData = Newtonsoft.Json.JsonConvert.DeserializeObject<FractalResponce>(resultStr);
                    var fractal = new Fractal();
                    id += "|" + alpha + beta + gamma;
                    fractal.id = id;
                    fractal.url = parseData.supurl;
                    fractal.alpha = alpha;
                    fractal.beta = beta;
                    fractal.gamma = gamma;
                    _db.Fractals.Add(fractal);
                    // сохраняем в бд все изменения
                    _db.SaveChanges();
                    int testA = 1;
                }
                catch(Exception e)
                {
                    throw new Exception("can't parse or save in database " + e.Message);
                }

            }
        }
    }
}
