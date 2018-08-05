using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WarframeMarketOverlay
{
    class WarframeMarketDeserializer
    {
        private Result result;

        public WarframeMarketDeserializer()
        {
            result = null;
        }

        public WarframeMarketDeserializer(string sourceJSON)
        {
            try
            {
                result = JsonConvert.DeserializeObject<Result>(sourceJSON);
            }
            catch (Exception e)
            {
                throw new Exception("The return JSON could not be parsed. Error Message: " + e.Message);
            }
        }

        public void Deserialize(string sourceJSON)
        {
            try
            {
                result = JsonConvert.DeserializeObject<Result>(sourceJSON);
            }
            catch(Exception e)
            {
                throw new Exception("The return JSON could not be parsed. Error Message: " + e.Message);
            }
        }

        public int GetLowestSellPrice()
        {
            int price = 0;
            if (result != null)
            {
                int ordersCount = result.payload.orders.Count();
                
                int i = 0;
                while (price == 0 && i < ordersCount)
                {
                    if (result.payload.orders[i].CheckIfValidSale())
                    {
                        price = result.payload.orders[i].platinum;
                    }
                    i++;
                }
                while (price > 1 && i < ordersCount)
                {
                    if (result.payload.orders[i].CheckIfValidSale() && result.payload.orders[i].platinum < price)
                    {
                        price = result.payload.orders[i].platinum;
                    }
                    i++;
                }
            }
            return price;
        }

        public class Result
        {
            public Payload payload { get; set; }
        }
        public class Payload
        {
            public IList<Order> orders { get; set; }
        }
        public class Order
        {
            public int platinum { get; set; }
            public int quantity { get; set; }
            public bool visible { get; set; }
            public string region { get; set; }
            public string last_update { get; set; }
            public User user { get; set; }
            public string platform { get; set; }
            public string id { get; set; }
            public string order_type { get; set; }
            public string creation_date { get; set; }

            public bool CheckIfValidSale()
            {
                if (IsVisible() && IsSell() && IsUserInGame())
                    return true;
                else
                    return false;
            }

            public bool IsVisible()
            {
                return visible;
            }
            public bool IsSell()
            {
                return order_type == "sell";
            }

            public bool IsUserInGame()
            {
                return user.IsInGame();
            }
        }
        public class User
        {
            public string status { get; set; }
            public int reputation { get; set; }
            public string avatar { get; set; }
            public string region { get; set; }
            public string id { get; set; }
            public string ingame_name { get; set; }
            public string last_seen { get; set; }
            public int reputation_bonus { get; set; }

            public bool IsInGame()
            {
                return status == "ingame";
            }

        }
    }
}
