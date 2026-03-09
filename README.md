
# How to run

It is .net 10 minimal API.
You need .net 10 runtime installed.
Then just do `dotnet run` in `Api` folder.
Server will be listening on http://localhost:5075



# Assumptions

1. We can have stale data for 10 minutes which is caching time for both list of the best stories and individual story entries.
2. Best stories list will not be longer than it is right now
3. 

# Enhancements

1. Smarter caches are required to avoid stampede when entries are evicted. 
2. Cached entries could be stored into distributed cache/databse to enable high availability and to prevent cache loss when service restarts.
3. Cache could be warm cache - preloaded with entries that expected to be requested.
4. Move retrieving of individual entries to worker process or hosted service to minimize delays on cold cache.
