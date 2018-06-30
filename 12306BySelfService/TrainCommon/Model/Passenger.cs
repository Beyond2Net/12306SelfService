using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon.Model
{
    public class Passenger
    {
        public string Address { set; get; }
        public string BornDate { set; get; }
        public string Code { set; get; }
        public string CountryCode { set; get; }
        public string Email { set; get; }
        public string FirstLetter { set; get; }
        public string IndexID { set; get; }
        public string MobileNo { set; get; }
        public string PassengerFlag { set; get; }
        public string PassengerIDNo { set; get; }
        public string PassengerIDTypeCode { set; get; }
        public string PassengerIDTypeName { set; get; }
        public string PassengerName { set; get; }
        public string PassengerType { set; get; }
        public string PassengerTypeName { set; get; }
        public string PhoneNo { set; get; }
        public string PostalCode { set; get; }
        public string RecordCount { set; get; }
        public string SexCode { set; get; }
        public string SexName { set; get; }
        public string TotalTimes { set; get; }
        public bool IsSelected { get; set; }
    }
}
