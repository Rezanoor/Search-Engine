//Author: Reza Nourbakhsh - rezanoorbakhsh@ymail.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace Orchard_Marketing_Solution
{
    class Program
    {

        static void Main(string[] args)
        {
            
            //Data from promotion sites
            
            Address address = new Address();
            address.Street = "22 Balmaringa";
            address.Suburb = "Gordon!";
            address.Postcode = "2072";
            address.Latitude = +40.689060m;
            address.Longitude = -74.044636m;
            OnlinePromotion onlinepromotion = new OnlinePromotion();
            onlinepromotion.Name = "Nike*-40% Off Storewide!";
            onlinepromotion.Address = address;
            onlinepromotion.PromotionCode = "124REZ";
            onlinepromotion.ProviderKey = "1";


            //Getting data from database
            Address situation = new Address();
            situation.Street = "22 Balmaringa!";
            situation.Suburb = "Gordon!";
            situation.Postcode = "2072";
            situation.Latitude = +40.689060m;
            situation.Longitude = -74.044636m;
            Promotion promotion = new Promotion();
            promotion.Address = situation;
            promotion.Name = "Nike 40% Off Storewide";
            promotion.PromotionCode = "124ZER";

            //Test case 1
            IPromotionMatcher objOzSite;
            objOzSite = PromotionCollection.getPromotionObj("Oz Winner");
            Console.Write(objOzSite.IsMatch(onlinepromotion, promotion));

            //Test case 2
            IPromotionMatcher objSpSite;
            objSpSite = PromotionCollection.getPromotionObj("Surface Promotion");
            Console.Write(objSpSite.IsMatch(onlinepromotion, promotion));

            //Test case 3
            IPromotionMatcher objHopSite;
            objHopSite = PromotionCollection.getPromotionObj("Hopeless Online Promotion");
            Console.Write(objHopSite.IsMatch(onlinepromotion, promotion));

            //To view
            Console.ReadLine();
        }
    }

    public class Address
    {
        //To match the example I added street property
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class Promotion
    {
        
        public Address Address { get; set; }
        public string PromotionCode { get; set; }
        public string Name { get; set; }
 
    }

    public class OnlinePromotion
    {
        public Address Address { get; set; }
        public string PromotionCode { get; set; }
        public string ProviderKey { get; set; }
        public string Name { get; set; }

    }

  

    public interface IPromotionMatcher
    {
        bool IsMatch(OnlinePromotion onlinePromotion, Promotion promotion);
    }

    //Here I implemented the interface based on the factory design pattern
    //The purpose is providing a flexible code for adding more promotion site in the future

    //FACTORY class
    public class PromotionCollection
    {
        //Called by the client / tester
        static public IPromotionMatcher getPromotionObj(string site)
        {
            IPromotionMatcher objPromotion = null;
            //-------------------------------------------------------------------------------------------------------------
            //NOT: A Collection(array, link list, hashtable) can be added to store the list of the online promotion sites
            //--------------------------------------------------------------------------------------------------------------
            switch (site)
            {
                case "Oz Winner":
                    objPromotion = new OzWinner();
                    break;
                case "Surface Promotion":
                    objPromotion = new SurfacePromotion();
                    break;
                case "Hopeless Online Promotion":
                    objPromotion = new HopelessOnlinePromotion();
                    break;
                default:
                    return objPromotion;
                    
            }
            return objPromotion;
        }  
    }
    
    //Oz Winner checker/handler
    //Oz Winner class
    public class OzWinner : IPromotionMatcher
    {
        //The specific distance - business rule
        const int DISTANCE = 500;

        #region IpromotionMatcher Members
        //Here IsMatch calls the distance method to calculate the distance and compare than with DISTANCE=500m
        public bool IsMatch(OnlinePromotion onlinePromotion, Promotion promotion)
        {
            bool IsMatchFlag = false;
            try
            {
                //Check the promotion code and the distance
                if (promotion.PromotionCode == onlinePromotion.PromotionCode && DISTANCE > distance(Convert.ToDouble(onlinePromotion.Address.Latitude), Convert.ToDouble(onlinePromotion.Address.Longitude),
                Convert.ToDouble(promotion.Address.Latitude), Convert.ToDouble(promotion.Address.Longitude)))
                {
                    IsMatchFlag = true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("An error occurred:" + ex.Message);
            }
            return IsMatchFlag;
        }

        #endregion
        //This method calculates the distance between two point based on the Lat and Long
        private double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;  
            dist = dist * 1.609344;
            return (dist);
        }
        //This method converts decimal degrees to radians             
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        //This method converts radians to decimal degrees             
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
    
    //Surface Promotion checker/handler
    //Surface Promotion class
    public class SurfacePromotion : IPromotionMatcher
    {
        //The regular expression that should match base on the business rules
        //All punctuation except % and $
        const string REGEXP = @"[\p{P}-[%$]]";

        #region IpromotionMatcher Members
        //Here IsMatch remove all the punctuation in both promotion, onlinepromotion name and address and compare them 
        //Note that the punctuation except % and $ is replaced by null to make it easy for comparing and this must be done in both promotion and online promotion to enhance the accuracy
        public bool IsMatch(OnlinePromotion onlinePromotion, Promotion promotion)
        {
            bool IsMatchFlag = false;
            try
            {
                //Apply the filtering
                Regex regex = new Regex(REGEXP, RegexOptions.IgnoreCase);
                //Remove all punctuation except $ and % from onlinepromotion name and adderss
                string onlinePromotionNamePunctuationless = regex.Replace(onlinePromotion.Name, string.Empty);
                string promotionNamePunctuationless = regex.Replace(promotion.Name, string.Empty);
               
                //Remove all punctuation except $ and % from promotion name and adderss
                string onlinePromotionAddressPunctuationless = regex.Replace(onlinePromotion.Address.Street+","+onlinePromotion.Address.Suburb+","+onlinePromotion.Address.Postcode, " ");
                string promotionAddressPunctuationless = regex.Replace(promotion.Address.Street+","+ promotion.Address.Suburb+","+promotion.Address.Postcode, " ");
                
                //Campare the name and address of onlinepromotion and promotion
                if (String.Compare(onlinePromotionNamePunctuationless, promotionNamePunctuationless, true) == 1 && String.Compare(onlinePromotionAddressPunctuationless, promotionAddressPunctuationless, true) == 1)
                {
                    IsMatchFlag = true;
                }
                
            }
            catch(Exception ex)
            {
                Console.Write("An error occurred:" + ex.Message);
            }
            return IsMatchFlag;

        }

        #endregion
    }

    //Hopeless online promotion checker/handler
    //Hopeless online promotion class
    public class HopelessOnlinePromotion : IPromotionMatcher
    {
        //Based on the business rules
        const int BACKWARDLENGTH = 3;

        #region IpromotionMatcher Members
        //Here the IsMatch calls promotionCodeCorrection which reverses the last three charachter of the onlinepromotion code
        public bool IsMatch(OnlinePromotion onlinePromotion, Promotion promotion)
        {
            bool IsMatchFlag = false;
            try
            {
                //IsMatchFlag = promotion.PromotionCode == promotionCodeCorrection(onlinePromotion.PromotionCode) ? true : false ;
                if (promotion.PromotionCode == promotionCodeCorrection(onlinePromotion.PromotionCode))
                {
                    IsMatchFlag = true;
                }
                
            }
            catch (Exception ex)
            {
                Console.Write("An error occurred:" + ex.Message);
            }
            return IsMatchFlag;
        }

        #endregion
        //This method reversed the last three charachter of the promotion code
        private string promotionCodeCorrection(string promotionCode)
        {
            string correctedPromotionCode = "";
            try
            {
                char[] arr = promotionCode.ToCharArray();
                char temp;
                temp = arr[promotionCode.Length - 1];
                arr[promotionCode.Length - 1] = arr[promotionCode.Length - BACKWARDLENGTH];
                arr[promotionCode.Length - BACKWARDLENGTH] = temp;
                correctedPromotionCode = new string(arr);

            }
            catch (Exception ex)
            {
                Console.Write("An error occurred:" + ex.Message);
            }
             return correctedPromotionCode;
        }
       
    }


}
