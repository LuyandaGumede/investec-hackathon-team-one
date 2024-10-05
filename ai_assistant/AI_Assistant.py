import ollama
import json

# Simplified system prompt to ensure compliance with the expected structure
modelfile = '''
FROM llama3.1
SYSTEM "You are a financial assistant called 'JJ'. Your role is to provide concise financial advice to users based on their transaction history. For each transaction, you should:

1. Provide a short, actionable financial insight (1 sentence).
2. Highlight if the user is nearing a budget limit or has a low balance for upcoming debit orders.
3. Suggest changes in spending behavior if necessary.

Here is the user's recent three month transaction history:

July 2024 Transactions
 31Jul24 Salary credited (R30,000)
 01Jul24 Rent payment (R8,500)
 02Jul24 Woolworths groceries (R1,800)
 03Jul24 Eskom utility bill (R1,200)
 04Jul24 Netflix subscription (R200)
 06Jul24 Shell fuel (R700)
 08Jul24 Pick n Pay groceries (R1,500)
 10Jul24 Vida e Caffè (R150)
 12Jul24 Discovery medical aid (R1,200)
 15Jul24 Savings transfer (R2,500)
 18Jul24 Mr Price online purchase (R900)
 21Jul24 Woolworths groceries (R1,600)
 25Jul24 Uber Eats (R250)
 28Jul24 Car loan repayment (R3,200)
 30Jul24 Vodacom data plan (R499)

August 2024 Transactions
 31Aug24: Salary credited (R30,000)
 01Aug24: Rent payment (R8,500)
 02Aug24: Woolworths groceries (R2,100)
 03Aug24: Eskom utility bill (R1,250)
 04Aug24: Netflix subscription (R200)
 07Aug24: Shell fuel (R800)
 10Aug24: Spur Steak Ranches (R500)
 12Aug24: Discovery medical aid (R1,200)
 15Aug24: Savings transfer (R2,500)
 18Aug24: Woolworths groceries (R1,850)
 22Aug24: Takealot purchase (R1,300)
 25Aug24: Uber Eats (R350)
 28Aug24: Car loan repayment (R3,200)
 29Aug24: MTN data bundle (R499)

September 2024 Transactions
 30Sep24: Salary credited (R30,000)
 01Sep24: Rent payment (R8,500)
 02Sep24: Woolworths groceries (R2,050)
 03Sep24: Eskom utility bill (R1,350)
 04Sep24: Netflix subscription (R200)
 06Sep24: Shell fuel (R900)
 08Sep24: Woolworths groceries (R1,950)
 10Sep24: Vida e Caffè (R150)
 12Sep24: Discovery medical aid (R1,200)
 15Sep24: Savings transfer (R2,500)
 18Sep24: H&M online purchase (R800)
 22Sep24: Woolworths groceries (R1,700)
 25Sep24: Uber Eats (R200)
 28Sep24: Car loan repayment (R3,200)
 29Sep24: Vodacom data plan (R499)

Always provide concise, actionable financial feedback based on the user’s spending habits. The feedback should be 1 sentence long."
'''

# Create the model using the Ollama SDK
ollama.create(model='financial_assistant', modelfile=modelfile)

# Define the transaction content
transaction_content = {
    "accountNumber": "10000000000",
    "dateTime": "20241005T11:17:18.921Z",
    "centsAmount": 80000,
    "currencyCode": "zar",
    "type": "card",
    "reference": "simulation",
    "card": {
        "id": "2280000"
    },
    "merchant": {
        "category": {
            "code": "0000",
            "key": "unknown_category",
            "name": "Unknown Category"
        },
        "name": "The Coders Bakery",
        "city": "Cape Town",
        "country": {
            "code": "ZA",
            "alpha3": "ZAF",
            "name": "South Africa"
        }
    }
}

# Convert the transaction content to a JSON string
transaction_content_str = json.dumps(transaction_content)

# Send the transaction data as the user's message with a request for financial advice
response = ollama.chat(model='financial_assistant', messages=[
    {
        'role': 'user',
        'content': f"Here is a recent transaction: {transaction_content_str}. Can you provide financial advice?",
    }
])

# Print the assistant's response
print(response['message']['content'])
