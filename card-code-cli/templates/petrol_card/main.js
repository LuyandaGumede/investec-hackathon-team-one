// Helper method to get last month's start date
const getLastMonthStartDate = () => {
  const today = new Date();
  const firstDayOfThisMonth = new Date(today.getFullYear(), today.getMonth(), 1);
  const lastMonth = new Date(firstDayOfThisMonth.setMonth(firstDayOfThisMonth.getMonth() - 1));
  return lastMonth.toISOString().split('T')[0];
};

// 1. Get transaction total for last month
const getTransactionTotalForLastMonth = async (accountId) => {
  const requestUrl = `accounts/${accountId}/transactions?fromDate=${getLastMonthStartDate()}`;
  try {
    const response = await axios.get(requestUrl);
    const transactions = response.data.data.transactions;

    let total = 0;
    transactions.forEach(transaction => {
      if (transaction.type === 'DEBIT') {
        total += parseFloat(transaction.amount);
      }
    });

    console.log(`Total Debit Transactions for Last Month: ${total}`);
    return total;
  } catch (error) {
    console.error('Failed to fetch transactions for last month.', error);
    return 0;
  }
};

// 2. Check the current monthly balance
const getCurrentMonthlyBalance = async (accountId) => {
  const requestUrl = `accounts/${accountId}/balance`;
  try {
    const response = await axios.get(requestUrl);
    const currentBalance = parseFloat(response.data.data.currentBalance);
    console.log(`Current Balance: ${currentBalance}`);
    return currentBalance;
  } catch (error) {
    console.error('Failed to fetch current balance.', error);
    return 0;
  }
};

// 3. Determine whether to allow or block the transaction
const decideTransaction = async (lastMonthTotal, currentBalance) => {
  if (currentBalance < lastMonthTotal) {
    console.log("Current balance is less than last month's total. Blocking transaction.");
    await sendAlertForApproval();
    return false;
  }
  return true;
};

// Simulate sending an alert to the user for approval
const sendAlertForApproval = async () => {
  console.log('Sending alert for approval...');
  await new Promise(resolve => setTimeout(resolve, 500)); // Simulate API call latency
  console.log('Alert sent to the user for transaction approval.');
};

// This function runs before a transaction.
const beforeTransaction = async (authorization) => {
  console.log(authorization);

  // Set base URI and authentication (if applicable)
  axios.defaults.baseURL = 'https://team1.sandboxpay.co.za/za/pb/v1/';
  axios.defaults.headers.common['Accept'] = 'application/json';
  // Add API Key if required
  // axios.defaults.headers.common['Authorization'] = 'Bearer YOUR_API_KEY';

  // User initiates a card transaction
  console.log('Initiating card transaction for a given day of the month...');

  // Get transaction total for last month
  const lastMonthTransactionTotal = await getTransactionTotalForLastMonth('4675778129910189600000003');

  // Check the current monthly balance
  const currentBalance = await getCurrentMonthlyBalance('4675778129910189600000003');

  // Decide whether to block or allow the transaction
  const isTransactionAllowed = await decideTransaction(lastMonthTransactionTotal, currentBalance);

  // Output the decision
  console.log(isTransactionAllowed
    ? 'Transaction is allowed.'
    : 'Transaction is blocked. Approval required from the user.');
};

// This function runs after a transaction was successful.
const afterTransaction = async (transaction) => {
  console.log(transaction);
};

// This function runs after a transaction was declined.
const afterDecline = async (transaction) => {
  console.log(transaction);
};