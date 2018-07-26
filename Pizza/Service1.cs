using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Threading;
namespace Pizza
{
    public class PizzaService : IPizzaService
    {
        private Pizza[] pizzas;
        private Topping[] toppings;
        private List<Order> orders;

        public PizzaService()
        {
            LoadPizzas();
            LoadToppings();
            orders = new List<Order>();
        }

        public int OrderPizza(int pizzaid, string address, int transferprice)
        {
            int price = transferprice;
            try
            {
                Pizza p = pizzas.Where((x => x.Id == pizzaid)).First();
                price += p.Price;
                foreach (int top in p.toppings)
                {
                    Topping t = toppings.Where((x => x.Id == top)).First();
                    price += t.Price;
                }
            }
            catch (Exception e) { }
            orders.Add(new Order(pizzaid, address, price));
            new Thread(new ThreadStart(exportOrders));
            return price;
        }

        public Pizza GetFamousPizza()
        {
            if (pizzas.Count() > 0)
            {
                var q1 = orders.GroupBy(x => x.Id).OrderBy(y => y.Count());
                var q2 = pizzas.Where(x => x.Id == q1.First().Key);
                return q2.First();
            }
            else
            {
                return null;
            }
        }

        public int Osszbevetel()
        {
            int bev = 0;
            foreach (Order o in orders)
            {
                bev += o.Price;
            }
            return bev;
        }

        public Order[] GetOrders()
        {
            return orders.ToArray();//nem így kéne, de lusta vagyok XML-t parsolni
        }

        private void exportOrders()
        {
            XDocument doc = new XDocument(new XElement("orders"));
            foreach (Order o in orders)
            {
                doc.Add(o.toXml());
            }
            doc.Save("orders.xml");
        }

        private void LoadPizzas()
        {
            XDocument pizzadoc = XDocument.Load("pizzas.xml");
            IEnumerable<XElement> p_elems = pizzadoc.Root.Elements("pizza");
            pizzas = new Pizza[p_elems.Count()];
            int i = 0;
            foreach (XElement p_elem in p_elems)
            {
                int price = Int32.Parse(p_elem.Attribute("addprice").Value);
                string name = p_elem.Attribute("name").Value;
                int id = Int32.Parse(p_elem.Attribute("id").Value);
                IEnumerable<XElement> ts = p_elem.Element("toppings").Elements("topping");
                int[] toppings = new int[ts.Count()];
                int j = 0;
                foreach (XElement t in ts)
                {
                    toppings[j] = Int32.Parse(t.Attribute("id").Value);
                    j++;
                }
                pizzas[i] = new Pizza() { Id = id, Name = name, Price = price, toppings = toppings };
                i++;
            }
        }

        private void LoadToppings()
        {
            XDocument toppingsdoc = XDocument.Load("toppings.xml");
            IEnumerable<XElement> tops = toppingsdoc.Root.Elements("topping");
            toppings = new Topping[tops.Count()];
            int i = 0;
            foreach (XElement t in tops)
            {
                int id = Int32.Parse(t.Attribute("id").Value);
                string name = t.Attribute("name").Value;
                int price = Int32.Parse(t.Attribute("price").Value);
                toppings[i] = new Topping() { Id = id, Name = name, Price = price };
                i++;
            }
        }

        public Pizza[] getPizzas()
        {
            return pizzas;
        }

        public Topping[] getToppings()
        {
            return toppings;
        }
    }
}
