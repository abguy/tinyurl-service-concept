# TinyURL style service concept

This is a project with a simple small application to serve as a proof of concept for a TinyURL style service. TinyURL is a service in which users can create short links, such as tinyurl.com/3rp36a3s, that redirect to longer links, such as https://www.example.com.

We do not need to see an actual persistent storage layer. Feel free to mock this out in memory however you best see fit. Lastly, note that a single long URL might map to a few different short URLs.

Although this is a POC, we would still like to see it designed with architecture in mind. To this end, please consider your schema, service methods, and constraints accordingly. You should also design and write your code with thread-safety and performance in mind.

The POC should support:
 * Creating and Deleting short URLs with associated long URLs.
 * Getting the long URL from a short URL.
 * Getting statistics on the number of times a short URL has been "clicked" i.e. the number of times its long URL has been retrieved.
 * Entering a custom short URL or letting the app randomly generate one, while maintaining uniqueness of short URLs.

## How to test

Ensure that you have .Net framework v8.0+ installed. You can download SDK [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

```shell
dotnet test -l "console;verbosity=normal" src/dirs.proj
```
