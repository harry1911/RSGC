using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using Database;
using GCRS.Base;
using Newtonsoft.Json;

namespace GCRS.Web.Controllers
{

    public class Graph
    {
        public string Qname = "";
        public string[] Data = new string[1];
        public double[] Values = new double[1];

        public Graph(string name, string[] col, double[] fila)
        {
            this.Qname = name;
            this.Data = new string[col.Length];
            this.Values = new double[fila.Length];
            for (int i = 0; i < col.Length; i++)
            {
                Data[i] = col[i];
                Values[i] = fila[i];
            }
        }
    }

    public class HomeController : Controller
    {
        private DB db = new DB();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles ="Jefe")]
        public ActionResult Charts()
        {
            ViewBag.Message = "";
            return View();
        }
        public ActionResult QueryRent()
        {
            var q = db.RentRegistries.Include(x => x.RentPublishing).Select(x => new { Name = x.RentPublishing.Realtor.Name}).ToList().GroupBy(y => y.Name, (k,p)=>new { Name=k, Count = p.Count() });
            var reactors = new List<string>();
            var count_reactors = new List<double>();
            foreach (var item in q)
            {
                reactors.Add(item.Name);
                count_reactors.Add(item.Count);
            }            
            var c = new Graph("# rentas", reactors.ToArray(), count_reactors.ToArray());
            return Json(c, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueryHouse()
        {
            var q = db.RentRegistries.Include(x => x.RentPublishing).Select(x => new { idHouse = x.RentPublishing.House }).ToList().GroupBy(y => y.idHouse, (k, p) => new { Name = k, Count = p.Count() });
            var casa = new List<string>();
            var count_casa = new List<double>();
            foreach (var item in q)
            {
                casa.Add(item.Name.Description);
                count_casa.Add(item.Count);
            }
            var c = new Graph("# rentas", casa.ToArray(), count_casa.ToArray());
            return Json(c, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuerySale()
        {
            var q = db.SellRegistries.Include(x => x.SellPublishing).Select(x => new { Name = x.SellPublishing.Realtor.Name }).ToList().GroupBy(y => y.Name, (k, p) => new { Name = k, Count = p.Count() }).OrderBy(x => x.Count);
            var reactors = new List<string>();
            var count_reactors = new List<double>();
            int limit = 10;
            foreach (var item in q)
            {
                if (limit <= 0) break;
                reactors.Add(item.Name);
                count_reactors.Add(item.Count);
                limit--;
            }
            var c = new Graph("# ventas", reactors.ToArray(), count_reactors.ToArray());
            return Json(c, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueryMoney()
        {

            //var q = db.DailySit.Select(x => new { Fecha = new Date(x.Date), Precio = x.Value }).GroupBy(y => y.Fecha, (k, p) => new { Fecha = k, Precio = p.Sum(z => z.Precio) }).ToList();
            var q = db.DailySit.Select(x => new { Fecha = x.Date, Precio = x.Value }).OrderBy(x => x.Fecha).ToList();

            var fechas = new List<string>();
            var ganacia = new List<double>();

            int index = 0;
            double acum = 0;
            for(int i=0;i<q.Count;i++)
            {
                var value = q[index].Fecha.Date;
                if (value.Day != q[i].Fecha.Day || value.Month != q[i].Fecha.Month || value.Year != q[i].Fecha.Year)
                {
                    fechas.Add(String.Format("{0}/{1}/{2}", value.Day, value.Month, value.Year));
                    ganacia.Add(acum);
                    index = i;
                }
                acum += q[i].Precio;
                if(i == q.Count-1)
                {
                    fechas.Add(String.Format("{0}/{1}/{2}", q[i].Fecha.Day, q[i].Fecha.Month, q[i].Fecha.Year));
                    ganacia.Add(acum);
                }
            }

            var c = new Graph("Ganancia", fechas.ToArray(), ganacia.ToArray());
            return Json(c, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}