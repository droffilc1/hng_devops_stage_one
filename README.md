# Number Classifier API

This project is a C#/.NET 6 minimal API that takes a number as input and returns interesting mathematical properties about it along with a fun fact from the Numbers API.

## Features

- **Input Validation:**
  Validates that the provided query parameter is a valid integer. Returns a proper error JSON response for invalid inputs.

- **Mathematical Properties:**

  - **Prime Check:** Determines if the number is prime.
  - **Perfect Check:** Determines if the number is a perfect number.
  - **Armstrong Check:** Determines if the number is an Armstrong number.
  - **Digit Sum:** Calculates the sum of the digits of the number.
  - **Properties Array:**
    Returns one of the following based on the checks:
    - `["armstrong", "odd"]`
    - `["armstrong", "even"]`
    - `["odd"]`
    - `["even"]`

- **Fun Fact:**
  Retrieves a fun fact from the [Numbers API](http://numbersapi.com/#42) under the math category.

- **CORS Enabled:**
  Allows cross-origin requests from any client.

- **JSON Responses:**
  All responses are returned in JSON format.

## API Endpoint

### GET `/api/classify-number`

**Query Parameter:**

- `number`: The number to classify.

### Successful Response (200 OK)

Example for `?number=371`:

```json
{
  "number": 371,
  "is_prime": false,
  "is_perfect": false,
  "properties": ["armstrong", "odd"],
  "digit_sum": 11,
  "fun_fact": "371 is an Armstrong number because 3^3 + 7^3 + 1^3 = 371"
}
```

### Error Response (400 Bad Request)

Example for ?number=alphabet:

```json
{
  "number": "alphabet",
  "error": true
}
```

## Getting Started

### Prerequisites

- .NET 6 SDK or later

## Installation

Clone the Repository:

```bash
git clone https://github.com/droffilc1/NumberClassifierAPI.git
cd NumberClassifierAPI
```

Restore Dependencies:

```bash
dotnet restore
```

Build the Project:

```bash
dotnet build
```

## Running the API Locally

Start the API with:

```bash
dotnet run
```

By default, the API will run on a random port check the terminal. You can test the endpoint by visiting:

```bash
http://localhost:5000/api/classify-number?number=371

```