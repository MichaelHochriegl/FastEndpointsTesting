# FastEndpoints Integration Testing showcase

This is a small showcase of how integration testing an API written with [FastEndpoints](https://fast-endpoints.com/) 
can look like.

## How to run on your machine

If you want to run the actual API you can use the provided `docker-compose.yml` located in the `src/Api` folder.
Before you start it with `docker compose up -d` you have to generate a `cert.pfx` in the `src/Api` folder, 
you can do this with the following command:
```
dotnet dev-certs https -ep cert.pfx -p Test1234!
```

After creating the cert you can start everything with `docker compose up -d` and navigate to your browser and go to:
```
https://localhost:5001
```


## About the tests

The tests in this example are not covering everything and have limited asserts, so don't take them as "this is how I should write my tests!",
instead they are meant to show you the general approach on how to test `FastEndpoints` endpoints.

I added some comments littered around the tests to give some explanation on what and why I'm doing certain things.