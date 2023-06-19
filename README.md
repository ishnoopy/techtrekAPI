# techtrekAPI
# uses ASP.Net Entity Framework.

NOTE: This API is made for the frontend named "TechtrekFrontend", look for it in my repository.

Instruction:
1. Open the .sln file and it should open up Visual Studio.
2. Change the connection settings in appsettings.json specifically the "MyConnection" property, this includes the database connection credentials such as username, password, port, and database name.
3. (optional) change the CORS, or the allowed origins in the program.cs but only if you want to restrict other server from using your API.
4. Click run.
5. In order to use the API methods, use the login endpoint of the Auth controller first and upon providing the credentials (email_address, password) it will give you a JWT token response.
6. Copy the JWT Token and use it as the Authorization key in your SwaggerUI.
7. Now, you can use all the API endpoints.
