using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GCRS.Base
{
    public class BuyRequest
    {
        public enum States { Request, Pending, Negotiation, Reject, Sell }

        [Key]
        public int BuyRequestID { get; set; }

        [Display(Name = "Cliente")]
        public virtual User Client { get; set; }

        [Display(Name = "Publicación")]
        public virtual SalePublishing SellData { get; set; }

        [Display(Name = "Precio")]
        public double RequestPrice { get; set; }

        [Display(Name = "Pidió negociación")]
        public bool Negotiation { get; set; }

        [Display(Name = "Estado de la venta")]
        public States BuyState { get; set; }

        [Display(Name = "Último cambio")]
        public Change LastModification { get; set; }

        public void InitToPending(User client, SalePublishing sellPublishing, bool negotiation)
        {
            Client = client;
            SellData = sellPublishing;
            Negotiation = negotiation;
            LastModification = Change.Client;
            if(!negotiation) RequestPrice = SellData.Price;
            BuyState = States.Pending;
        }

        public void RequestToPending(double price)
        {
            LastModification = Change.Client;
            BuyState = States.Pending;
        }

        public void PendingToRequest()
        {
            BuyState = States.Request;
        }

        public void PendingToRequest(double price)
        {
            if (price != RequestPrice)
                LastModification = Change.Realtor;
            BuyState = States.Request;
        }

        public void PendingToNegotiation()
        {
            BuyState = States.Negotiation;
        }

        public void PendingToNegotiation(double price)
        {
            if (price != RequestPrice)
                LastModification = Change.Realtor;
            BuyState = States.Negotiation;
        }

        public void NegotiationToPending(double price)
        {
            LastModification = Change.Owner;
            BuyState = States.Pending;
        }

        public void ToReject()
        {
            BuyState = States.Reject;
        }

        public void ToSell()
        {
            BuyState = States.Sell;
        }
    }
}
