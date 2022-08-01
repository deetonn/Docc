# Docc :newspaper:
An example of client-server connections built from scratch. This project contains multi-threaded networking, login system, session ids to track who the server is talking to, requests sent in json format that are able to contain anything and complete freedom in adding new endpoints.

# Get started 
Warning :warning: This is for Microsoft Visual Studio 2022. I'm not sure about any other IDE's/Editors.

 1. Download the source code
 2. Open the **.sln** file.
 3. Build the project (**Ctrl+B**)
 4. Navigate to the Docc folder.
 5. Run *test.bat* (or optionally *server.bat* and *client.bat*)
 6. Once the server has started, type **user.create [name] [password]**. This will save this login so a client can login using it.
 7. Login via the client.
 
Now you're ready to debug and tinker.

# Work in progress
This is currently implemented to be used locally, it also does not encrypt packets being sent across sockets and it's mega scuffed. It will eventually be good though.

# Contribution

Feel free, anything to improve this madness would be nice.
