## qeep

Table of Contents

* [What is Qeep?](#what-is-qeep)
* [How to Install](#how-to-install)
* [Using Qeep](#using-budoco)

## What is Qeep?

This app is my variation on Google's "Keep" app. You can make notes using your phone or desktop/laption and those notes will be synced by a server to your other devices. If you are offline, the notes will be saved locally until you are again online.

It is built on .NET 5, aka "dotnet core 5". 
It uses the file system, not a database.

It uses git as part of its "database" so that you can see change history, recover past versions, etc.

For now there is only a web page client, but that web page works for both desktop and phone. 

## How to Install

* Install dotnet core 5 sdk

* Install git

* Clone this repo. Let's say the folder name is "/home/USERNAME/qeep"

* Make a folder not in the repo folder, somewhere else, for example, "/home/USERNAME/qeep_data"

* Create a file in the qeep_data, "users.txt". For each user enter:

```
username:password, 
```

so for,  example:

```
corey:abc123
somebody_else:a_better_password
```

## Using Qeep

Start the server - or not.

Save the file wwwroot/qeep.html. That's your client. When you are offline, it saves note data in browser "localStorage". When you are online it uploads the data to the server. 

Note that for now, on both server and client, passwords are saved in clear, plain text without encryption.
