## ECAM - Energy Company Account Manager

An example of a .NET Core Web API written in C# that allows a user to upload a CSV file of customer meter readings. A SQLite database is seeded with Account details for which the meter readings are tied to. Meter readings must be validated against existing accounts and values must be numeric. New readings must have a date newer than previous readings. Once the CSV file is processed, a summary of successful and failed readings is returned.

