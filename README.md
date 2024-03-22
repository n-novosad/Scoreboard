# Scoreboard

There are a couple of moments worth mentioning, which were assumed during the development of the solution

  - Storage type and the way this library will be consumed

     Storage type for real-time games should be in-memory, something that supports distributed systems and can store data in a persistant storage. In-memory cache systems like Redis, Hazelcast can be used for these purposes.
     In the solution, scoreboard service pre-loads data in the assumption, when library is used for Web API services and there are multiple calls to endpoints, a new service will be created to handle a concurrent request. In order to spin it up with a non-blank scoreboard, data preload is used. Ideally, on every call to the service, data has to be synchronized with in-memory cache for distributed systems, but current solution supports only one-way synchronization, e.g. from the service to cache, meaning that there is only one tenant in a distributed system and for multi-tenant case it has to be adapted.
     For reliability purposes, in-memory cache has to be a solution hosted out-of-process to the target service space and, ideally, on a different host, so external distributed systems could be as approach.

 - Asynchronicity

   Used IMemoryCache provides only a sync way to communicate with data. In multi-CPU environments, it's better to utilize asynchronous and parallel handling of concurrent requests for better performance and throughput.
