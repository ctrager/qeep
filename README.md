## qeep

Table of Contents

* [What is Qeep?](#what-is-qeep)
* [How to Install](#how-to-install)
* [Using Qeep](#using-budoco)

## What is Qeep?

This app is my variation on Google's "Qeep" app. You can make notes on or offline. The notes will be saved by the server and synced to other clients when they come online. So, make a note on your phone when you are offline that later gets saved to the server when you have connectivity. Then see the note on your laptop. Update the note on your laptop, see the update on your phone.

It is built on .NET 5, aka "dotnet core 5". 
It uses the file system, not a database.

It uses git as part of its "database" so that you can see change history, recover past versions, etc.

For now there is just a web page client, but that web page works for both desktop and phone. 

## How to Install

* Install dotnet core 5 sdk

* Install git

* git clone this repo. Let's say the folder name is "/home/USERNAME/qeep_server"

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

Save the file wwwroot/qeep.html. That's your client. When you are offline, it saves note data in browser "localStorage". When you are online in uploads the data to the server. 
