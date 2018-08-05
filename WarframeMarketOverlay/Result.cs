using System.Linq;

namespace WarframeMarketOverlay
{
    public class Result
    {
        public Payload payload { get; set; }

        public int GetLowestSellPrice()
        {
            int price = 0;
            if (this != null)
            {
                int ordersCount = payload.orders.Count();

                int i = 0;
                while (price == 0 && i < ordersCount)   //Finds the first valid price
                {
                    if (payload.orders[i].CheckIfValidSale())
                    {
                        price = payload.orders[i].platinum;
                    }
                    i++;
                }
                while (price > 1 && i < ordersCount)    //Goes through the rest and stops if it reaches the end or finds a price of 1
                {
                    if (payload.orders[i].CheckIfValidSale() && payload.orders[i].platinum < price)
                    {
                        price = payload.orders[i].platinum;
                    }
                    i++;
                }
            }
            return price;
        }
    }
    public class Payload
    {
        public System.Collections.Generic.IList<Order> orders { get; set; }
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
