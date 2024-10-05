using Azure.AI.OpenAI;
using Azure.Identity;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using System.ClientModel;
using System.Net.Http.Headers;

namespace Team1Hackathon
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            await RunAnalysis();

            Console.ReadLine();
        }

        static async Task RunAnalysis()
        {
            // Set base URI and authentication (if applicable)
            client.BaseAddress = new Uri("https://team1.sandboxpay.co.za/za/pb/v1/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add API Key if required
            // client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_API_KEY");

            // 0) User initiates a card transaction
            Console.WriteLine("Initiating card transaction for a given day of the month...");

            // 1) Get transaction total for last month
            decimal lastMonthTransactionTotal = await GetTransactionTotalForLastMonth("4675778129910189600000003");

            // 2) Check the current monthly balance
            decimal currentBalance = await GetCurrentMonthlyBalance("4675778129910189600000003");

            // 3) Decide whether to block or allow the transaction
            bool isTransactionAllowed = await DecideTransaction(lastMonthTransactionTotal, currentBalance);

            // Output the decision
            Console.WriteLine(isTransactionAllowed
                ? "Transaction is allowed."
                : "Transaction is blocked. Approval required from the user.");
        }

        // 1. Get transaction total for last month
        static async Task<decimal> GetTransactionTotalForLastMonth(string accountId)
        {
            var requestUrl = $"accounts/{accountId}/transactions?fromDate={GetLastMonthStartDate()}";
            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var transactions = JObject.Parse(content)["data"]["transactions"];

                decimal total = 0;
                foreach (var transaction in transactions)
                {
                    if (transaction["type"].ToString() == "DEBIT")
                    {
                        total += decimal.Parse(transaction["amount"].ToString());
                    }
                }

                Console.WriteLine($"Total Debit Transactions for Last Month: {total}");
                return total;
            }
            else
            {
                Console.WriteLine("Failed to fetch transactions for last month.");
                return 0;
            }
        }

        // 2. Check the current monthly balance
        static async Task<decimal> GetCurrentMonthlyBalance(string accountId)
        {
            var requestUrl = $"accounts/{accountId}/balance";
            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var currentBalance = decimal.Parse(JObject.Parse(content)["data"]["currentBalance"].ToString());
                Console.WriteLine($"Current Balance: {currentBalance}");
                return currentBalance;
            }
            else
            {
                Console.WriteLine("Failed to fetch current balance.");
                return 0;
            }
        }

        // 3. Determine whether to allow or block the transaction
        static async Task<bool> DecideTransaction(decimal lastMonthTotal, decimal currentBalance)
        {
            if (currentBalance < lastMonthTotal)
            {
                // Case 3.1: Debit orders haven't run versus past history
                Console.WriteLine("Current balance is less than last month's total. Blocking transaction.");

                // Simulate sending alert for approval (Card API)
                await SendAlertForApproval();
                return false;
            }

            // Case 3.2: Otherwise, allow transaction
            return true;
        }

        // Simulate sending an alert to the user for approval
        static async Task SendAlertForApproval()
        {
            Console.WriteLine("Sending alert for approval...");
            // Example alert sending logic
            await Task.Delay(500); // Simulate API call latency
            Console.WriteLine("Alert sent to the user for transaction approval.");
        }

        // Helper method to get last month's start date
        static string GetLastMonthStartDate()
        {
            var today = DateTime.Now;
            var firstDayOfThisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = firstDayOfThisMonth.AddMonths(-1);
            return lastMonth.ToString("yyyy-MM-dd");
        }
    }

}
