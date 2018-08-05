using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Brotli;
using System.Linq;

namespace WarframeMarketOverlay
{
    class WarframeMarketClient
    {
        HttpClient client = null;

        public WarframeMarketClient()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri("https://warframe.market/")
            };
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
        }

        /// <summary>
        /// FIRST IMPLEMENTATION (need Deserializer.cs and Brotli)
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>

        public async Task<int> GetLowestPriceForItem(string itemName) 
        {
            try
            {
                var responseMessage = await GetResponseMessage(itemName);
                if (responseMessage.IsSuccessStatusCode)
                {
                    byte[] byteResponse = await responseMessage.Content.ReadAsByteArrayAsync();
                    byte[] decompress = BrotliDecompress(byteResponse);         //Use this if we go for byte deserialization
                    string text = System.Text.Encoding.ASCII.GetString(decompress);
                    WarframeMarketDeserializer deserializer = new WarframeMarketDeserializer(text);
                    return deserializer.GetLowestSellPrice();
                }
                return 0;
            }
            catch (Exception e)
            {
                throw new Exception("The price for the item wasn't properly fetched. Error message: " + e.Message);
            }
        }

        private async Task<System.Net.Http.HttpResponseMessage> GetResponseMessage(string link)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.warframe.market/v1/items/" + link + "/orders"),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            request.Headers.Referrer = new Uri("https://warframe.market/items/" + link);
            return await client.SendAsync(request);
        }

        private byte[] BrotliDecompress(byte[] input)
        {
            using (System.IO.MemoryStream msInput = new System.IO.MemoryStream(input))
            using (BrotliStream bs = new BrotliStream(msInput, System.IO.Compression.CompressionMode.Decompress))
            using (System.IO.MemoryStream msOutput = new System.IO.MemoryStream())
            {
                bs.CopyTo(msOutput);
                msOutput.Seek(0, System.IO.SeekOrigin.Begin);
                var output = msOutput.ToArray();
                return output;
            }
        }

        /// <summary>
        /// SECOND IMPLEMENTATION
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>


        //public async Task<int> GetLowestPriceForItem(string itemName) 
        //{
        //    var response = await client.GetAsync("https://api.warframe.market/v1/items/" + itemName + "/orders");
        //    var result = await response.Content.ReadAsAsync<Result>();
        //    return GetLowestSellPrice(result);
        //}

        //public int GetLowestSellPrice(Result result)
        //{
        //    int price = 0;
        //    if (result != null)
        //    {
        //        int ordersCount = result.payload.orders.Count();

        //        int i = 0;
        //        while (price == 0 && i < ordersCount)
        //        {
        //            if (result.payload.orders[i].CheckIfValidSale())
        //            {
        //                price = result.payload.orders[i].platinum;
        //            }
        //            i++;
        //        }
        //        while (price > 1 && i < ordersCount)
        //        {
        //            if (result.payload.orders[i].CheckIfValidSale() && result.payload.orders[i].platinum < price)
        //            {
        //                price = result.payload.orders[i].platinum;
        //            }
        //            i++;
        //        }
        //    }
        //    return price;
        //}

        //public class Result
        //{
        //    public Payload payload { get; set; }
        //}
        //public class Payload
        //{
        //    public System.Collections.Generic.IList<Order> orders { get; set; }
        //}
        //public class Order
        //{
        //    public int platinum { get; set; }
        //    public int quantity { get; set; }
        //    public bool visible { get; set; }
        //    public string region { get; set; }
        //    public string last_update { get; set; }
        //    public User user { get; set; }
        //    public string platform { get; set; }
        //    public string id { get; set; }
        //    public string order_type { get; set; }
        //    public string creation_date { get; set; }

        //    public bool CheckIfValidSale()
        //    {
        //        if (IsVisible() && IsSell() && IsUserInGame())
        //            return true;
        //        else
        //            return false;
        //    }

        //    public bool IsVisible()
        //    {
        //        return visible;
        //    }
        //    public bool IsSell()
        //    {
        //        return order_type == "sell";
        //    }

        //    public bool IsUserInGame()
        //    {
        //        return user.IsInGame();
        //    }
        //}
        //public class User
        //{
        //    public string status { get; set; }
        //    public int reputation { get; set; }
        //    public string avatar { get; set; }
        //    public string region { get; set; }
        //    public string id { get; set; }
        //    public string ingame_name { get; set; }
        //    public string last_seen { get; set; }
        //    public int reputation_bonus { get; set; }

        //    public bool IsInGame()
        //    {
        //        return status == "ingame";
        //    }

        //}

    }
}
