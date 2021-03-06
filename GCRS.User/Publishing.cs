﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GCRS.Base;

namespace GCRS.Base
{
    public class Publishing
    {
        public int PublishingID { get; set; }
        [Display(Name = "Cliente")]
        public virtual User User { get; set; }
        [Display(Name="Descripción")]
        public string Description { get; set; }
        [Display(Name = "Precio")]
        public double Price { get; set; }
        [Display(Name = "Agente")]
        public virtual User Realtor { get; set; }
        [Display(Name = "Comisión")]
        [Range(0,100)]
        public double CommissionPercent { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public bool Showable { get; set; }
    }

    public class SalePublishing : Publishing
    {
        public virtual House House { get; set; }
    }
    public class RentPublishing : Publishing
    {
        [Display(Name = "Casa")]
        public virtual House House { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<RentRegistry> Rents { get; set; }
    }

    #region REQUEST PUBLISHING

    public enum RequestStatus { Request, Pending, Negotiation, Accept, Reject }
    public enum Change { Client, Realtor, Owner };
    public enum RequestType { Sale, Rent };

    public class RequestPublishing
    {
        // Properties of Publishing
        public int RequestPublishingID { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Display(Name = "Agente")]
        public virtual User Realtor { get; set; }
        [Display(Name = "Cliente")]
        public virtual User User { get; set; }
        [Display(Name = "Precio")]
        public double Price { get; set; }
        [Display(Name = "Comisión")]
        [Range(0, 100)]
        public double CommissionPercent { get; set; }
        [Display(Name = "Último Cambio")]
        public Change LastChange { get; set; }
        [Display(Name = "Estado")]
        public RequestStatus RequestStatus { get; set; }


        //My code
        public ICollection<Image> Images { get; set; }

        #region FLOW
        public void RequestToPending(User realtor)
        {
            Realtor = realtor;
            RequestStatus = RequestStatus.Pending;
        }

        public void ToReject()
        {
            RequestStatus = RequestStatus.Reject;
        }

        public void ToAccept(User user)
        {
            if ((LastChange == Change.Client && user.Privileges.Name == "Realtor") || (LastChange == Change.Realtor && user.Privileges.Name == "Client"))
                RequestStatus = RequestStatus.Accept;
        }

        public void PendingToNegotiation(double price, double commission)
        {
            if (price != Price || commission != CommissionPercent)
                LastChange = Change.Realtor;
            RequestStatus = RequestStatus.Negotiation;
        }

        public void NegotiationToPending(double price, double commission)
        {
            if (price != Price || commission != CommissionPercent)
                LastChange = Change.Client;
            RequestStatus = RequestStatus.Pending;
        }
        #endregion
    }

    public class RequestRentPublishing : RequestPublishing
    {
        //public int RequestRentPublishingID { get; set; }
        public ICollection<Room> Rooms { get; set; }
        [Display(Name = "Casa")]
        public House House { get; set; }

        #region FLOW
        public void InitToRequest()
        {
            RequestStatus = RequestStatus.Request;
            LastChange = Change.Client;
            CommissionPercent = 0;//estaba en el nuestro
        }
        #endregion
    }

    public class RequestSalePublishing : RequestPublishing
    {
        //public int RequestSalePublishingID { get; set; }
        public House House { get; set; }
        #region FLOW
        public void InitToRequest()
        {
            RequestStatus = RequestStatus.Request;
            LastChange = Change.Client;
            CommissionPercent = 0;//estaba en el nuestro
        }
        #endregion
    }

    #endregion
}
