using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace Pizza
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPizzaService
    {
        [OperationContract]
        int OrderPizza(int pizzaid, string address, int transferprice);

        [OperationContract]
        Topping[] getToppings();

        [OperationContract]
        Pizza[] getPizzas();

        [OperationContract]
         int Osszbevetel();

        [OperationContract]
        Order[] GetOrders();
    }

    [DataContract]
    public class Topping
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Price { get; set; }

        public override string ToString()
        {
            return Name + " -," + Price;
        }
    }

    [DataContract]
    public class Pizza
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Price { get; set; }

        [DataMember]
        public int[] toppings;

        public override string ToString()
        {
            return Name + " -," + Price;
        }
    }

    [DataContract]
    public class Order
    {

        public Order(int id, string address, int price)
        {
            this.Address = address;
            this.Id = id;
            this.Price = price;
        }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Price { get; set; }

        public XElement toXml()
        {
            XElement elem = new XElement("order", new XAttribute("address", Address), new XAttribute("id", Id), new XAttribute("price", Price));
            return elem;
        }
    }
}
