## MongoId

[SPTarkov.Server.Core/Models/Common/MongoId.cs](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Common/MongoId.cs)

A MongoId consists of 24 bits (12 bytes), specifically:

- Timestamp (4 bytes): Records the creation time (seconds since 1970-01-01)
- Machine Identifier (3 bytes): Distinguishes between different machines
- Process ID (2 bytes): Different processes on the same machine
- Counter (3 bytes): Auto-incrementing sequence within the same process

Example: `5f9d9b8e6f8b4a1e3c7d5a30`

**How to Write Your Own MongoId**

- Method 1: Use an online MongoId generator
- Method 2: Modify a few characters in an existing MongoId, then search in items.json to ensure uniqueness (not recommended)
- Method 3: Simply write a random 24-character string—it's highly unlikely to conflict with existing Ids in SPT
- Method 4: Write your own code or have AI generate a simple script to produce MongoIds