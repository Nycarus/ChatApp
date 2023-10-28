## Multi-Room Chat Application
This is an educational .NET 6 web application project for learning Blazor, SignalR, and Entity Framework. The app is able to create connections among users and send messages to users of the same chat room.

## Preview
![Chat Room Image](./readme_images/ChatRoom.PNG)

## How to run?
1. Add Secret keys to the API and Client Application
```
{
  "JwtSettings:Key": "SomeRandomLongJWTKey",
  "ConnectionStrings:DefaultConnection": "Server=host.docker.internal;Port=5432;Database=chatapp;User Id=root Password=root;TrustServerCertificate=True"
}
```
```
{
  "ChatUrl": "http://host.docker.internal:5000/signalr/hubs/chat",
  "API": "http://host.docker.internal:5000"
}
```
2. Create .env file at the root directory, filling the fields of the .env.example values.

3. Navigate to the root directory and run the following command. You can then run the application through your docker container through the url http://localhost:4000.
```
docker compose up
```

4. To stop the containers, run the following command.
```
docker compose down
```