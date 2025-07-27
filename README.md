# RentReportDownloader
A C# console application to automate the retrieval of CSV-based financial reports using a REST API. 

## Features

- Authenticates via token-based API access
- Reads property IDs from a file
- Downloads and saves monthly reports in CSV format
- Gracefully handles HTTP errors like 404 (report not found)
- Flexible configuration via environment variables

## How to Use

1. Set environment variables:
   - `API_USERNAME`
   - `API_PASSWORD`
   - `API_LOCATIONID`

2. Place `propids.txt` in the `./data/` directory (comma-separated IDs)

3. Run the program:
```bash
dotnet run
