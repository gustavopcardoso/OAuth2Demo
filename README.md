# OAuth2Demo

The purpose of this demo is to show a very basic authentication flow. The Client will send its credentials to Auth0 and will receive a JWT. With the token in hands, a GET request will be sent to the Nginx server (running locally at port 8080) that will forward the request to the API. Once inside the API, the .NET pipeline will perform a validation of the JWT through the Authentication Middleware.

The Client application can be runned locally. The docker compose file creates two containers: the API image pulled from my dockerhub registry and a Nginx instance acting as a proxy. 

Some of the technologies and concepts used in this demo are listed below:

- **Authorization protocol: OAuth2**

The standard protocol which IAMs works on top of to provide the necessary permissions for the user to a digital resource (i.e. the API). Further details about this       protocol can be checked at https://datatracker.ietf.org/doc/html/rfc6749#section-1.2.

- **Identity platform: Auth0** (https://auth0.com/) 

Auth0 works as the Authorization Server and Identity Provider platform which holds the ID and Secret of the Client application for validation. Once validated, a JWT is returned to the client.

- **Grant type: Client Credentials**

No need for input username and password, thus no end-user involved. The validation is performed against the credentials registered for the Client application.

*(PS: There’s a lot more details to cover than the ones described above, as well as more tools to use, such as a vault to store the secrets. My intention is to improve this demo over time.)*
