# cityshob-task-list

**Description:**

 This is an implementation of the to-do list home assignment for CityShob by Itamar Stollar.
 A short demo of the app can be seen here:
https://drive.google.com/file/d/1z8T3PxYuo2Uzn39SwK6tXva2im9onEEJ/view?usp=drive_link
 
 It inscludes:
 1. Task list server: a console app for managing user task items from all clients
 2. Task list client: a WPF application for visually creating and editing to-do tasks.
 3. Task list DB: stores up-to date state of all user tasks and allows the server to restore them as neccessary.

Implementation details:
1. Implemented with C# for .Net Framework 4.8
2. Uses MSSQL 2022 Express
3. WCF over TCP communication for duplex messaging.
4. Entity Framework for DB access.
5. UI implemented in WPF.

Design considerations and main patterns used:
1. The client-server communication is implemented using WCF with two services:
The ModifyTaskService exposes functionality for performing CRUD operations on user tasks using the request-response pattern,
While the TaskUpdatedService and callback allow registering for user task updates based on the publish-subscribe pattern.
2. The client UI is implemented with WPF using the MVVM pattern to seperate the UI view, the data model and the binding friendly intermediate viewmodel.
It relies on the Caliburn.Micro framework to wire the binding of UI events to the VM actions.
3. The server similarly implements a separation of business logic and data where discrete business logic is encapsulated in services and data management is encapsulated in the repository.
4. In both server and client instanciation and dependencies are managed by a dependency injection container to implement the inversion of control pattern.

External libraries used:
1. Caliburn.Micro - simple MVVM framework
2. Entity Framework 6 - DB access
3. Microsoft.Extensions.DependencyInjection - dependency injection
4. Microsft.Extensions.Logging - default logging

Additional notes:
1. DB connection string is currently defined in the server's app.config
2. Logging is currently wired to the console logger. To modify register a different logger in the DI service registration (Program.cs in the server, AppBootstrapper.cs in the client)

**Deployment:**
1. Pull the solution from the repository.
2. Open in Visual Studio 2022.
3. Restore NuGet Packages for the solution.
4. Build the solution.
5. Restore the database **CityShobUserTasksDB** from the provided DB bak file in the root directory.
6. Execute **CityShobTaskListServer.exe** to run the server.
7. Execute one or more **CityShobTaskListClient.exe** instances to run a client.
